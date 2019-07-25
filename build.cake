///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");


///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var projects = GetFiles("./**/*.csproj");
var projectPaths = projects.Select(project => project.GetDirectory().ToString());
var artifactsDir = "./Artifacts";
var projFile = "./discord_cli.csproj";
var coverageThreshold = 100;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information("Running tasks...");
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    var settings = new DeleteDirectorySettings {
        Recursive = true,
        Force = true
    };
    // Clean solution directories.
    foreach(var path in projectPaths)
    {
        Information($"Cleaning path {path} ...");
        var directoriesToDelete = new DirectoryPath[]{
            Directory($"{path}/obj"),
            Directory($"{path}/bin"),
            Directory(artifactsDir)
        };
        foreach(var dir in directoriesToDelete)
        {
            Information($"Cleaning path {dir} ...");
            if (DirectoryExists(dir))
            {
                DeleteDirectory(dir, settings);
            }
        }
    }
    // Delete artifact output too
    if (DirectoryExists(artifactsDir))
    {
        Information($"Cleaning path {artifactsDir} ...");
        DeleteDirectory(artifactsDir, settings);
    }
});

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var path in projectPaths)
    {
        Information($"Restoring {path}...");
        DotNetCoreRestore(path);
    }
});
Task("Publish")
    .Description("Releases the different parts of the project, including the dll's")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCorePublishSettings
     {
         Framework = "netcoreapp3.0",
         Configuration = "Release",
         //Runtime = "win10-x64",
         Runtime = "win-x64",
         OutputDirectory = artifactsDir
     };

     DotNetCorePublish(projFile, settings);
});

Task("Build")
    .Description("Builds all the different parts of the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
     {
         Framework = "netcoreapp3.0",
         Configuration = "Debug"
         ,         OutputDirectory = artifactsDir
     };

     DotNetCoreBuild(projFile, settings);
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("Publish the application!")
    .IsDependentOn("Publish")
    ;

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////


RunTarget(target);