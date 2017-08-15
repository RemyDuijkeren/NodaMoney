//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "xunit.runner.console"
#tool "GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
 
//////////////////////////////////////////////////////////////////////
/// Build Variables
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
    CleanDirectory(artifactsDir);
    DotNetCoreClean(rootDir);

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
    Information("Implement Version");
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
 
 
//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Default")
//.IsDependentOn("Test")
.IsDependentOn("Package");
 
//////////////////////////////////////////////////////////////////////
// EXECUTION
////////////////////////////////////////////////////////////////////// 
RunTarget(target);