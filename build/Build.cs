using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.CoverallsNet;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Tools.ReportGenerator;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.CoverallsNet.CoverallsNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.IO.CompressionTasks;
using Log = Serilog.Log;

[GitHubActions("continuous",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new []{ "master" },
    OnPushExcludePaths = new []{ "docs/**"},
    OnPullRequestBranches = new []{ "master" },
    OnPullRequestExcludePaths = new []{ "docs/**" },
    InvokedTargets = new[] { nameof(Continuous) },
    EnableGitHubToken = true)]
[GitHubActions("release",
    GitHubActionsImage.WindowsLatest,
    OnPushTags = new []{ "*.*.*" },
    InvokedTargets = new[] { nameof(Release) },
    EnableGitHubToken = true,
    ImportSecrets = new[] { nameof(NuGetApiKey) })]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] [Secret] readonly string NuGetApiKey;
    [Parameter] [Secret] readonly string CoverallsRepoToken;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [CI] readonly GitHubActions GitHubActions;

    [PathExecutable]
    readonly Tool Git;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultDirectory => ArtifactsDirectory / "TestResults";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
            var testResults = ArtifactsDirectory / "TestResults";

            var publishConfigurations =
                from project in Solution.GetProjects("*.Tests")
                from framework in project.GetTargetFrameworks()
                select new { project, framework };

            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetFilter("FullyQualifiedName!~PerformanceSpec")
                .SetLoggers("trx")
                .SetResultsDirectory(testResults)
                .EnableNoBuild()
                .EnableNoRestore()
                .EnableCollectCoverage()
                .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                .SetCoverletOutput(testResults / "coverage.xml")
                .When(IsServerBuild, s => s.EnableUseSourceLink()));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .EnableIncludeSource()
                .EnableNoBuild()
                .EnableNoRestore());
        });
    
    Target PushGitHubPackage => _ => _
        .DependsOn(Pack)
        .TriggeredBy(Pack)
        .Consumes(Pack)
        .OnlyWhenStatic(() => IsServerBuild)
        .OnlyWhenStatic(() => GitHubActions != null)
        .Executes(() =>
        {
            // GitHub doesn't allow symbols (.snupkg) yet
            DotNetNuGetPush(s => s
                .SetTargetPath(ArtifactsDirectory / "*.nupkg")
                .SetSource($"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json") 
                .SetApiKey(GitHubActions.Token));
        });

    Target PushNuGet => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .OnlyWhenStatic(() => IsServerBuild)
        .OnlyWhenStatic(() => NuGetApiKey != null)
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
                .SetTargetPath(ArtifactsDirectory / "*.nupkg")
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NuGetApiKey));
        });
    
    Target Benchmark => _ => _
        .After(Compile)
        .Produces(ArtifactsDirectory / "BenchmarkResults")
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            Log.Information("Use test methods to execute the benchmarks (xunit.shadowCopy=false && build=Release)");
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .SetFilter("FullyQualifiedName~PerformanceSpec")
                .SetFramework("net48"));
            // .When(ExecutingTargets.Contains(Compile) && Configuration == Configuration.Release, s => s
            //     .SetNoBuild(true)
            //     .SetNoRestore(true)));

            Log.Information("Move Benchmark results to Artifacts folder:");
            TestsDirectory.GlobDirectories("**/BenchmarkDotNet.Artifacts/results").ForEach(d =>
                CopyDirectoryRecursively(
                    source: d,
                    target: ArtifactsDirectory / "BenchmarkResults",
                    DirectoryExistsPolicy.Merge,
                    FileExistsPolicy.OverwriteIfNewer));
        });

    AbsolutePath CoverageReportDirectory => ArtifactsDirectory / "coverage-report";
    AbsolutePath CoverageReportArchive => ArtifactsDirectory / "coverage-report.zip";

    Target Coverage => _ => _
        .DependsOn(Test)
        .Consumes(Test)
        .TriggeredBy(Test)
        .Produces(CoverageReportArchive)
        .Executes(() =>
        {
            ReportGenerator(s => s
                .SetReports(TestResultDirectory / "*.xml")
                .SetReportTypes(ReportTypes.HtmlInline_AzurePipelines)
                .SetTargetDirectory(CoverageReportDirectory)
                .SetFramework("netcoreapp2.1"));

            // TestResultDirectory.GlobFiles("*.xml").ForEach(f =>
            //     AzurePipelines?.PublishCodeCoverage(
            //         coverageTool: AzurePipelinesCodeCoverageToolType.Cobertura,
            //         summaryFile: f,
            //         reportDirectory: ArtifactsDirectory / "coverage-report"));

            CompressZip(
                directory: CoverageReportDirectory,
                archiveFile: CoverageReportArchive,
                fileMode: FileMode.Create);
        });

    Target CoverageCoveralls => _ => _
        .DependsOn(Compile)
        .TriggeredBy(Coverage)
        .Produces(ArtifactsDirectory / "coverage-opencover.xml")
        //.OnlyWhenStatic(() => AzurePipelines.BuildReason != AzurePipelinesBuildReason.PullRequest) // if build not started by PR
        .Requires(() => CoverallsRepoToken)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetFilter("FullyQualifiedName!~PerformanceSpec")
                .SetFramework("net48")
                .EnableNoBuild()
                .EnableNoRestore()
                .EnableCollectCoverage()
                .SetCoverletOutput(ArtifactsDirectory / "coverage-opencover.xml")
                .SetCoverletOutputFormat(CoverletOutputFormat.opencover));

            CoverallsNet(s => s
                .SetRepoToken(CoverallsRepoToken)
                .EnableOpenCover()
                .SetInput(ArtifactsDirectory / "coverage-opencover.net48.xml"));
            //.SetCommitId(GitVersion.Sha)
            //.SetCommitBranch(GitVersion.BranchName));
            //.When(IsServerBuild, s => s
            //.SetCommitAuthor(AzurePipelines.RequestedFor)
            //.SetCommitEmail(AzurePipelines.RequestedForEmail)
            //.SetCommitMessage(AzurePipelines.Sou) // Build.SourceVersionMessage
            //.SetJobId(int.Parse(AzurePipelines.BuildUri))
            //.SetServiceName(AzurePipelines.GetType().Name)));
        });
    
    Target Continuous => _ => _
        .DependsOn(Clean, Restore, Compile, Test, Pack);
    
    Target Release => _ => _
        .DependsOn(Clean, Restore, Compile, Test, Pack, PushNuGet);
}
