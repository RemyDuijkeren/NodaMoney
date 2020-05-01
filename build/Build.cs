using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.CoverallsNet;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Xunit;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Tools.ReportGenerator;
using static Nuke.Common.Logger;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.CoverallsNet.CoverallsNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.GitHub.GitHubTasks;
using static Nuke.Common.Tools.GitReleaseManager.GitReleaseManagerTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.IO.CompressionTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[AzurePipelines(
    suffix: null,
    image: AzurePipelinesImage.WindowsLatest,
    AutoGenerate = true,
    InvokedTargets = new[] { nameof(Test), nameof(Pack) },
    NonEntryTargets = new[] { nameof(Restore), nameof(Compile), nameof(Coverage), nameof(CoverageCoveralls) },
    ExcludedTargets = new[] { nameof(Clean) },
    TriggerBranchesExclude = new[] { "gh-pages" },
    TriggerPathsExclude = new[] { "docs/" , "tools/" }
    )]
partial class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("NuGet Api Key")]
    readonly string NuGetApiKey; // EnvironmentVariable NUGET_API_KEY

    [Parameter("Coveralls Repository Token")]
    readonly string CoverallsRepoToken; // EnvironmentVariable COVERALLS_REPO_TOKEN

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    [CI] readonly AzurePipelines AzurePipelines;

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
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.FullSemVer)
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
                .SetLogger("trx")
                .SetResultsDirectory(testResults)
                .EnableNoBuild()
                .EnableNoRestore()
                .EnableCollectCoverage()
                .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                .SetCoverletOutput(testResults / "coverage.xml")
                .When(IsServerBuild, s => s
                    .EnableUseSourceLink()));

            Info("PublishTestResults:");
            TestResultDirectory.GlobFiles("*.trx").ForEach(f =>
                AzurePipelines?.PublishTestResults(
                    type: AzurePipelinesTestResultsType.VSTest,
                    title: $"{AzurePipelines.StageDisplayName}",
                    files: new string[] { f }));

        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.NuGetVersion)
                .EnableIncludeSymbols()
                .EnableNoBuild());
        });

    Target Benchmark => _ => _
        .After(Compile)
        .Produces(ArtifactsDirectory / "BenchmarkResults")
        .Executes(() =>
        {
            Info("Use test methods to execute the benchmarks (xunit.shadowCopy=false && build=Release)");
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .SetFilter("FullyQualifiedName~PerformanceSpec")
                .SetFramework("net48")
                .When(ExecutingTargets.Contains(Compile) && Configuration == Configuration.Release, s => s
                    .SetNoBuild(true)
                    .SetNoRestore(true)));

            Info("Move Benchmark results to Artifacts folder:");
            TestsDirectory.GlobDirectories("**/BenchmarkDotNet.Artifacts/results").ForEach(d =>
                    CopyDirectoryRecursively(
                        source: d,
                        target: ArtifactsDirectory / "BenchmarkResults",
                        DirectoryExistsPolicy.Merge,
                        FileExistsPolicy.OverwriteIfNewer));

        });

    Target Publish => _ => _
        .DependsOn(Clean, Test, Pack)
        .Consumes(Pack)
        .OnlyWhenStatic(() => GitRepository.IsOnMasterBranch())
        .OnlyWhenStatic(() => false) // if build has started by pushed tag
        .OnlyWhenStatic(() => AzurePipelines.BuildReason != AzurePipelinesBuildReason.PullRequest)
        .Requires(() => NuGetApiKey)
        .Requires(() => AzurePipelines)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            var packages = ArtifactsDirectory.GlobFiles("*.nupkg");

            DotNetNuGetPush(s => s
                .SetWorkingDirectory(ArtifactsDirectory)
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NuGetApiKey));
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

            TestResultDirectory.GlobFiles("*.xml").ForEach(f =>
                AzurePipelines?.PublishCodeCoverage(
                    coverageTool: AzurePipelinesCodeCoverageToolType.Cobertura,
                    summaryFile: f,
                    reportDirectory: ArtifactsDirectory / "coverage-report"));

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
                .SetInput(ArtifactsDirectory / "coverage-opencover.net48.xml")
                .SetCommitId(GitVersion.Sha)
                .SetCommitBranch(GitVersion.BranchName)
                .When(IsServerBuild, s => s
                    .SetCommitAuthor(AzurePipelines.RequestedFor)
                    .SetCommitEmail(AzurePipelines.RequestedForEmail)
                    //.SetCommitMessage(AzurePipelines.Sou) // Build.SourceVersionMessage
                    //.SetJobId(int.Parse(AzurePipelines.BuildUri))
                    .SetServiceName(AzurePipelines.GetType().Name)));
        });

    void FinishReleaseOrHotfix()
    {
        Git($"checkout master");
        Git($"merge --no-ff --no-edit {GitRepository.Branch}");
        Git($"tag {GitVersion.MajorMinorPatch}");

        Git($"branch -D {GitRepository.Branch}");

        Git($"push origin master {GitVersion.MajorMinorPatch}");
    }
}
