using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.CoverallsNet;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.CoverallsNet.CoverallsNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.GitHub.GitHubTasks;
using static Nuke.Common.Tools.GitReleaseManager.GitReleaseManagerTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
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
    [CI] readonly AppVeyor AppVeyor;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath CoverageFile => RootDirectory / "artifacts" / "coverage.xml";

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
        .Produces(CoverageFile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .When(ExecutingTargets.Contains(Coverage), s => s
                    .EnableCollectCoverage()
                    .SetCoverletOutput(CoverageFile)
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)));
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
        .OnlyWhenStatic(() => AppVeyor.PullRequestNumber == 0) // if build not started by PR
        .OnlyWhenStatic(() => AppVeyor.RepositoryTag) // if build has started by pushed tag
        .Requires(() => NuGetApiKey)
        .Requires(() => AppVeyor)
        .Executes(() =>
        {
            var packages = ArtifactsDirectory.GlobFiles("*.nupkg");

            DotNetNuGetPush(s => s
                .SetWorkingDirectory(ArtifactsDirectory)
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NuGetApiKey));
        });

    Target Coverage => _ => _
        .DependsOn(Test)
        .OnlyWhenStatic(() => AppVeyor.PullRequestNumber == 0) // if build not started by PR
        .Requires(() => CoverallsRepoToken)
        .Executes(() =>
        {
            if (AppVeyor != null)
            {
                CoverallsNet(s => s
                    .SetRepoToken(CoverallsRepoToken)
                    .EnableOpenCover()
                    .SetInput(CoverageFile) 
                    .SetCommitId(AppVeyor.RepositoryCommitSha)
                    .SetCommitBranch(AppVeyor.RepositoryBranch)
                    .SetCommitAuthor(AppVeyor.RepositoryCommitAuthor)
                    .SetCommitEmail(AppVeyor.RepositoryCommitAuthorEmail)
                    .SetCommitMessage(AppVeyor.RepositoryCommitMessage)
                    .SetJobId(AppVeyor.BuildNumber)
                    .SetServiceName(AppVeyor.GetType().Name));
            }
            else
            {
                CoverallsNet(s => s
                    .SetRepoToken(CoverallsRepoToken)
                    .EnableOpenCover()
                    .SetInput(CoverageFile)
                    .SetCommitId(GitVersion.Sha)
                    .SetCommitBranch(GitVersion.BranchName)
                    .SetServiceName("Local"));
            }
        });

    Target Publish  => _ => _
        .DependsOn(Clean, Pack, Coverage, NuGetPush);
}
