#tool nuget:?package=xunit.runner.console
#tool nuget:?package=GitVersion.CommandLine
#tool nuget:?package=OpenCover
#tool nuget:?package=coveralls.net
#addin nuget:?package=Cake.Figlet
#addin nuget:?package=Cake.Coveralls

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solutionFile = "./NodaMoney.sln";
var artifactsDir = Directory("./artifacts/");
var srcProjects = GetFiles("./src/**/*.csproj");
var testProjects = GetFiles("./tests/**/*.csproj");

Setup(context =>
{
   Information(Figlet("NodaMoney"));
});

Task("Clean")
.Does(() =>
{
    CleanDirectory(artifactsDir);

    foreach(var path in srcProjects.Select(csproj => csproj.GetDirectory()))
    {
        CleanDirectory(path + "/bin/" + configuration);
        CleanDirectory(path + "/obj/" + configuration);
    }

    foreach(var path in testProjects.Select(csproj => csproj.GetDirectory()))
    {
        CleanDirectory(path + "/bin/" + configuration);
        CleanDirectory(path + "/obj/" + configuration);
    }
});

Task("Restore")
.Does(() =>
{
    DotNetCoreRestore(solutionFile);
});

Task("Version").
Does(() =>
{
    var versionInfo = GitVersion();
    var buildVersion = EnvironmentVariable("APPVEYOR_BUILD_NUMBER") ?? "0";
    var assemblyVersion =  versionInfo.Major + ".0.0.0"; // Minor and Patch versions should work with base Major version
    var fileVersion = versionInfo.MajorMinorPatch + "." + buildVersion;
    var informationalVersion = versionInfo.FullSemVer;
    var nuGetVersion = versionInfo.NuGetVersion;

    Information("BuildVersion: " + buildVersion);
    Information("AssemblyVersion: " + assemblyVersion);
    Information("FileVersion: " + fileVersion);
    Information("InformationalVersion: " + informationalVersion);
    Information("NuGetVersion: " + nuGetVersion);
    
    if (AppVeyor.IsRunningOnAppVeyor)
    {
        AppVeyor.UpdateBuildVersion(informationalVersion + ".build." + buildVersion);
    }	
    
    Information("Update Directory.build.props");
    var file = File("./src/Directory.build.props");
    XmlPoke(file, "/Project/PropertyGroup/Version", nuGetVersion);
    XmlPoke(file, "/Project/PropertyGroup/AssemblyVersion", assemblyVersion);
    XmlPoke(file, "/Project/PropertyGroup/FileVersion", fileVersion);
    XmlPoke(file, "/Project/PropertyGroup/InformationalVersion", informationalVersion);
});

Task("Build")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Version")
.Does(() =>
{
    DotNetCoreBuild(solutionFile, new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = arg => arg.AppendSwitch("/p:DebugType","=","Full") // needed for OpenCover
    });
});

Task("Test")
.IsDependentOn("Build")
.Does(() =>
{
    foreach(var csproj in testProjects)
    {
        DotNetCoreTest(csproj.ToString(), new DotNetCoreTestSettings { Configuration = configuration });
    }
});

Task("Coverage")
.IsDependentOn("Test")
.Does(() =>
{
    var openCoverSettings = new OpenCoverSettings
    {
        OldStyle = true,
        MergeOutput = true
    }
    .WithFilter("+[NodaMoney*]* -[*.Tests*]*");

    var xunit2Settings = new XUnit2Settings { ShadowCopy = false };

    foreach(var testLib in GetFiles("./tests/**/bin/Release/*/NodaMoney*.Tests.dll"))
    {
        OpenCover(
            context => { context.XUnit2(testLib.FullPath, xunit2Settings); },
            artifactsDir.Path + "/coverage.xml",
            openCoverSettings);
    }
});

Task("Package")
.IsDependentOn("Coverage")
.Does(() =>
{
    var packSettings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = artifactsDir,
        NoBuild = true
    };
 
    foreach(var csproj in srcProjects)
    {
        DotNetCorePack(csproj.ToString(), packSettings);
    }
 });

Task("Upload-Coverage-CoverallsIo")
.WithCriteria(() => HasEnvironmentVariable("COVERALLS_REPO_TOKEN"))
.WithCriteria(() => !AppVeyor.Environment.PullRequest.IsPullRequest)
.IsDependentOn("Coverage")
.Does(() =>
{
    if (AppVeyor.IsRunningOnAppVeyor)
    {
        CoverallsNet(artifactsDir.Path + "/coverage.xml", CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
        {
            RepoToken = EnvironmentVariable("COVERALLS_REPO_TOKEN"),
            CommitId = AppVeyor.Environment.Repository.Commit.Id,
            CommitBranch = AppVeyor.Environment.Repository.Branch,
            CommitAuthor = AppVeyor.Environment.Repository.Commit.Author,
            CommitEmail = AppVeyor.Environment.Repository.Commit.Email,
            CommitMessage = AppVeyor.Environment.Repository.Commit.Message,
            JobId = Convert.ToInt32(EnvironmentVariable("APPVEYOR_BUILD_NUMBER")),
            ServiceName = "appveyor"
        });
    }
    else
    {
        CoverallsNet(artifactsDir.Path + "/coverage.xml", CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
        {
            RepoToken = EnvironmentVariable("COVERALLS_REPO_TOKEN"),
            ServiceName = "local"
        });        
    }
});

Task("Upload-AppVeyor-Artifacts")
.WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
.WithCriteria(() => !AppVeyor.Environment.PullRequest.IsPullRequest)
.IsDependentOn("Package")
.Does(() =>
{
    foreach(var package in GetFiles(artifactsDir.ToString() + "/*.nupkg"))
    {
        AppVeyor.UploadArtifact(package);
    }
});

Task("Publish-NuGet")
.WithCriteria(() => HasEnvironmentVariable("NUGET_API_KEY"))
.WithCriteria(() => AppVeyor.Environment.Repository.Tag.IsTag)
.IsDependentOn("Package")
.Does(() =>
{	
    DotNetCoreNuGetPush("*.nupkg", new DotNetCoreNuGetPushSettings
    {
        WorkingDirectory = artifactsDir,
        Source = "https://www.nuget.org/",
        ApiKey = EnvironmentVariable("NUGET_API_KEY")
    });
});
 
Task("Default")
.IsDependentOn("Package");

Task("AppVeyor")
.IsDependentOn("Package")
.IsDependentOn("Upload-AppVeyor-Artifacts")
.IsDependentOn("Upload-Coverage-CoverallsIo")
.IsDependentOn("Publish-NuGet");

RunTarget(target);