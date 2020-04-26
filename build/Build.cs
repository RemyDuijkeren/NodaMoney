using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
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
using static Nuke.Common.Logger;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.CoverallsNet.CoverallsNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.GitHub.GitHubTasks;
using static Nuke.Common.Tools.GitReleaseManager.GitReleaseManagerTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[AzurePipelines(
    suffix: null,
    image: AzurePipelinesImage.WindowsLatest,
    AutoGenerate = false,
    InvokedTargets = new[] { nameof(Publish) },
    NonEntryTargets = new[] { nameof(Restore), nameof(Compile), nameof(Test), nameof(Pack), nameof(Coverage), nameof(NuGetPush) },
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

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

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
        .Produces(ArtifactsDirectory / "TestResults" / "*.trx")
        .Produces(ArtifactsDirectory / "TestResults" / "*.xml")
        .Executes(() =>
        {
            var testResults = ArtifactsDirectory / "TestResults";

            //var publishConfigurations =
            //    from project in Solution.GetProjects("*.Tests")
            //    from framework in project.GetTargetFrameworks()
            //    select new { project, framework };

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
            AzurePipelines?.PublishTestResults(
                title: AzurePipelines.StageDisplayName,
                type: AzurePipelinesTestResultsType.VSTest,
                files: new string[] { testResults / "*.trx" });

            //Info("PublishTestResults:");
            //testResults.GlobFiles("*.trx").ForEach(x =>
            //    AzurePipelines?.PublishTestResults(
            //        type: AzurePipelinesTestResultsType.VSTest,
            //        title: $"{AzurePipelines.StageDisplayName}",
            //        files: new string[] { x }));
        });

    Target Benchmark => _ => _
        .After(Compile)
        .Produces(ArtifactsDirectory / "BenchmarkResults")
        .Executes(() =>
        {
            // Use test methods to execute the benchmarks (xunit.shadowCopy=false && build=Release)
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .SetFilter("FullyQualifiedName~PerformanceSpec")
                .SetFramework("net48")
                .When(ExecutingTargets.Contains(Compile) && Configuration == Configuration.Release, s => s
                    .SetNoBuild(true)
                    .SetNoRestore(true)));

            TestsDirectory.GlobDirectories("**/BenchmarkDotNet.Artifacts/results").ForEach(d =>
                    CopyDirectoryRecursively(
                        source: d,
                        target: ArtifactsDirectory / "BenchmarkResults",
                        DirectoryExistsPolicy.Merge,
                        FileExistsPolicy.OverwriteIfNewer));
            
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

    Target NuGetPush => _ => _
        .DependsOn(Pack)
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

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory / "coverage.xml")
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
                .SetCoverletOutput(ArtifactsDirectory / "coverage.xml")
                .SetCoverletOutputFormat(CoverletOutputFormat.opencover));

            var file = ArtifactsDirectory / "coverage.net48.xml";
            if (IsServerBuild)
            {
                CoverallsNet(s => s
                    .SetRepoToken(CoverallsRepoToken)
                    .EnableOpenCover()
                    .SetInput(ArtifactsDirectory / "coverage.net48.xml")
                    .SetCommitId(AzurePipelines.SourceVersion)
                    .SetCommitBranch(AzurePipelines.SourceBranchName)
                    .SetCommitAuthor(AzurePipelines.RequestedFor)
                    .SetCommitEmail(AzurePipelines.RequestedForEmail)
                    //.SetCommitMessage(AzurePipelines.Sou) // Build.SourceVersionMessage
                    //.SetJobId(int.Parse(AzurePipelines.BuildNumber))
                    .SetServiceName(AzurePipelines.GetType().Name));           
            }
            else
            {
                CoverallsNet(s => s
                    .SetRepoToken(CoverallsRepoToken)
                    .EnableOpenCover()
                    .SetInput(ArtifactsDirectory / "coverage.net48.xml")
                    .SetCommitId(GitVersion.Sha)
                    .SetCommitBranch(GitVersion.BranchName)
                    .SetServiceName("Local"));
            }
        });

    Target Publish  => _ => _
        .DependsOn(Clean, Pack, Coverage, NuGetPush);
}
