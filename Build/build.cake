#tool "nuget:?package=xunit.runner.console"
//#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
     
     
var target = Argument("target", "Package");
var configuration = Argument("Configuration", "Release");
var solutionPath = Argument("SolutionPath", @"../Src/ProductManagement.sln");

//var artifacts = MakeAbsolute(Directory("./artifacts"));

var outputDir = Directory("./output");
var buildSettings = new DotNetCoreBuildSettings
     {
         Framework = "netstandard3.1",
         Configuration = "Release",
         OutputDirectory = outputDir
     };


Task("Clean")
    .Does(() =>
{
    if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
    CleanDirectories(string.Format("../Src/**/obj/{0}",configuration));
    CleanDirectories(string.Format("../Src/**/bin/{0}",configuration));
});


Task("Restore-Nuget")
    .Does(()=> {
     DotNetCoreRestore(solutionPath);
});


Task("Build")
    .Does(()=>
{
     DotNetCoreBuild(
        solutionPath,
        new DotNetCoreBuildSettings {
            NoRestore = true,
            Configuration = configuration
        }
);
});

Task("Run-Unit-tests")
    .DoesForEach(
        GetFiles("../Tests/**/*.Tests.Unit.csproj"),
        testProject => 
{
    DotNetCoreTest(
        testProject.FullPath,
        new DotNetCoreTestSettings{
            NoBuild = true,
            NoRestore = true,
            Configuration = configuration
        }
    );
});

Task("Dot-Cover")
    .Does(()=> {
  //   DotCoverCover(tool => {
    //      tool.XUnit2("../Tests/**/*.Tests.Unit.dll",new XUnit2Settings { ShadowCopy = false });
 // },
//  new FilePath("./result.dcvr"),
 // new DotCoverCoverSettings());
});



Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-Nuget")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Dot-Cover");

RunTarget("Default");
