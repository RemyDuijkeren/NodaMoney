// This build assumes the following directory structure (https://gist.github.com/davidfowl/ed7564297c61fe9ab814):
//  \build    	- Build customizations (custom msbuild files/psake/fake/albacore/etc) scripts
//  \artifacts	- Build outputs go here. Doing a build.cmd generates artifacts here (nupkgs, zips, etc.)
//	\docs		- Documentation stuff, markdown files, help files, etc
//	\lib		- Binaries which are linked to in the source but are not distributed through NuGet
//	\packages	- Nuget packages
//	\samples    - Sample projects
//  \src		- Main projects (the source code)
//	\tests      - Test projects
//	\tools		- Binaries which are used as part of the build script (e.g. test runners, external tools)

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "xunit.runner.console"
#tool "GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
 
//////////////////////////////////////////////////////////////////////
/// GLOBAL VARIABLES
/////////////////////////////////////////////////////////////////////
var rootDir = Directory("./");
var artifactsDir = Directory("./artifacts/");
var srcProjects = GetFiles("./src/**/*.csproj");
var testProjects = GetFiles("./tests/**/*.csproj");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Clean")
.Does(() =>
{
    DotNetCoreClean(rootDir);

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
    DotNetCoreRestore(rootDir);
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
        Information("Send updated version to AppVeyor");
        AppVeyor.UpdateBuildVersion(informationalVersion + ".build." + buildVersion);
    }	
	
    Information("Update Directory.build.props");
    var file = File(rootDir.ToString() + "src/Directory.build.props");
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
    DotNetCoreBuild(rootDir, new DotNetCoreBuildSettings { Configuration = configuration });
});

Task("Test")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    foreach(var csproj in testProjects)
    {
        DotNetCoreTest(csproj.ToString(), new DotNetCoreTestSettings { Configuration = configuration });
    }
});

Task("Package")
.IsDependentOn("Build")
.IsDependentOn("Test")
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
 .WithCriteria(() => AppVeyor.Environment.Repository.Branch == "master")
 //.WithCriteria(() => AppVeyor.Environment.Repository.Tag.IsTag)
 .IsDependentOn("Package")
 .Does(() =>
 {	
    DotNetCoreNuGetPush("*.nupkg", new DotNetCoreNuGetPushSettings
    {
        WorkingDirectory = artifactsDir,
        //Source = "https://staging.nuget.org/packages?replace=true",
        ApiKey = EnvironmentVariable("NUGET_API_KEY")
    });
});
 
//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Default")
.IsDependentOn("Package");

Task("AppVeyor")
.IsDependentOn("Package")
.IsDependentOn("Upload-AppVeyor-Artifacts")
.IsDependentOn("Publish-NuGet");
 
//////////////////////////////////////////////////////////////////////
// EXECUTION
////////////////////////////////////////////////////////////////////// 
RunTarget(target);