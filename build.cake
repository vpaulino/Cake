#addin nuget:?package=SharpZipLib&version=0.86.0
#addin nuget:?package=Cake.Compression&version=0.1.4
#addin nuget:?package=Cake.VersionReader
#addin nuget:?package=Cake.Incubator

using Cake.VersionReader;
using Cake.Incubator;
using Cake.Compression;

var target = Argument<string>("target","FullBuild");
var configuration = Argument<string>("configuration","Release");

var artifactsPath = Argument<string>("artifactsPath","./artifacts");
var publishPath = Argument<string>("publishPath","./publish");
var sourcePath = Argument<string>("sourcePath","./Src");
var testsPath = Argument<string>("testsPath","./Tests");
var solutionName = Argument<string>("solutionName","TestCake.sln");
var buildNumber =  Argument<int>("buildNumber",999999);
var versionSufix = "1.0.0";
var pre = HasArgument("pre");
var unitTestsResultFileName = "UnitTestResults.trx";
var applicationToBuildPathPattern = Argument<string>("appName","**");

Information($"pre: {pre}");
Information($"buildNumber: {buildNumber}");

TaskSetup((ctx)=>
{
    if(ctx.Task.Name.Equals("Tests"))
    EnsureDirectoryExists($"{artifactsPath}/TestResults");
    
});


Task("Clean")
    .Does(()=>
    {
        CleanDirectories($"{testsPath}/**/TestResults");
        CleanDirectory(artifactsPath);
        CleanDirectory(publishPath);
    });

Task("NugetRestore")
.DoesForEach(GetFiles($"{sourcePath}/{applicationToBuildPathPattern}/**/*.csproj"), (file) => 
    {
        var projectPath = file.FullPath;
        NuGetRestore(projectPath);
    })
     .DeferOnError();

 
 Task("Build")
 .IsDependentOn("Clean")
.IsDependentOn("NugetRestore")
.DoesForEach(GetFiles($"{sourcePath}/{applicationToBuildPathPattern}/**/*.csproj"), (file) => 
    {
        var projectPath = file.FullPath;
        
         var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoRestore = true,
        };

        DotNetCoreBuild(projectPath, settings);
    });

Task("Tests")
.IsDependentOn("Build")
 .DoesForEach(GetFiles($"{testsPath}/{applicationToBuildPathPattern}/**/*.csproj"), (file) => 
    {
        var projectName = file.GetFilename().ToString().Replace(".csproj",string.Empty);

        var settings = new DotNetCoreTestSettings
        {            
            // Outputing test results as XML so that VSTS can pick it up
            ArgumentCustomization = (args)=>
            {
                        //args.Append("--logger \"trx;LogFileName=TestResults.xml\"")
                        args.Append($"--logger \"trx;LogFileName={projectName}.trx\"");
                        args.Append("--filter \"TestCategory=Unit|Category=Unit\"");
                        return args;
            } 
        };

         DotNetCoreTest(file.FullPath,settings);                  
    })
    .Finally(()=>
    {
        CopyFiles($"{testsPath}/{applicationToBuildPathPattern}/TestResults/*.trx",$"{artifactsPath}/TestResults");
    })
    .DeferOnError();


Task("NugetPack")
.IsDependentOn("Tests")
 .DoesForEach(GetFiles($"{sourcePath}/Libs/**/*.csproj"), (file) => 
    {
            var versionXPath = "/Project/PropertyGroup/VersionPrefix";

            var versionSufix = XmlPeek(file.FullPath, versionXPath + "/text()");
           if(pre)
           {
                versionSufix = $"{versionSufix}-pre{buildNumber}";
           }

            var publishFolder =  $"{publishPath}/nugets/";
              var settings = new DotNetCorePackSettings
            {
                Configuration = configuration,
                OutputDirectory =  publishFolder,
                NoDependencies = false,
                NoRestore = true,
                VersionSuffix = versionSufix

            };
      
            DotNetCorePack(file.FullPath, settings);                 
             
});

Task("Publish")
.IsDependentOn("Tests")
 .DoesForEach(GetFiles($"{sourcePath}/{applicationToBuildPathPattern}/**/*Host.csproj"), (file) => 
    {
                 var publishProjectName = file.GetFilename().ToString().Replace(".csproj",string.Empty);
                 var assemblyPath = GetProjectAssembly(file.FullPath,configuration);
                 var versionSuffix = GetFullVersionNumber(assemblyPath);
                 var hostPublishFolder =  $"{publishPath}/{publishProjectName}-{versionSuffix}/";
                
                var settings = new DotNetCorePublishSettings
                {
                   
                    Configuration = configuration,
                    OutputDirectory = hostPublishFolder,
                };
                
                DotNetCorePublish(file.ToString(), settings);
    });

Task("Arquive")
 .IsDependentOn("Tests")
.DoesForEach(GetDirectories($"{publishPath}/*"), (directory) => 
{    
        Information($"{directory}.zip");
        ZipCompress(directory, $"{directory}.zip");

}).DeferOnError();

Task("FullBuild")
.IsDependentOn("Clean")
.IsDependentOn("Build")
.IsDependentOn("Tests")
.IsDependentOn("Publish");


    RunTarget(target);
