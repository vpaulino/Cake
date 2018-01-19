#addin nuget:?package=SharpZipLib&version=0.86.0
#addin nuget:?package=Cake.Compression&version=0.1.4


using Cake.Compression;

var target = Argument<string>("target","FullBuild");
var configuration = Argument<string>("configuration","Release");

var unitTestsResultFileName = "UnitTestResults.xml";
var testsResultFolder = "./TestsResults/";


Task("Clean")
    .Does(()=>
    {
        CleanDirectories("./Tests/**/TestResults");
        CleanDirectory("./artifacts");
        CleanDirectory("./publish");
    });

Task("NugetRestore")

.Does(()=>
{
    
    NuGetRestore("TestCake.sln");
});

Task("Build")
    .IsDependentOn("NugetRestore")
    .Does(()=>
    {
        //DotNetCoreBuild("TestCake.sln");
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
        };

        DotNetCoreBuild("TestCake.sln", settings);

    });

Task("Tests")
.IsDependentOn("Build")
    .Does(()=>
    {
           var projectFiles = GetFiles("./Tests/**/*.csproj");
                var settings = new DotNetCoreTestSettings
                {
                    // Outputing test results as XML so that VSTS can pick it up
                    ArgumentCustomization = (args)=>
                    {
                                //args.Append("--logger \"trx;LogFileName=TestResults.xml\"")
                                args.Append($"--logger \"trx;LogFileName={unitTestsResultFileName}\"");
                                args.Append("--filter \"TestCategory=Unit|Category=Unit\"");
                                 return args;
                    } 
                };
            foreach(var file in projectFiles)
            {
                               
                DotNetCoreTest(file.FullPath,settings);
            }
    });


Task("NugetsPack")
//.IsDependentOn("Tests")
.Does(()=>{
           
            var publishFolder =  $"./publish/nugets/";
              var settings = new DotNetCorePackSettings
            {
                Configuration = configuration,
                OutputDirectory =  publishFolder,
                NoDependencies = false,
                NoRestore = true,
                VersionSuffix = "rc"

            };

             var projectFiles = GetFiles("./Src/Libs/**/*.csproj");

             foreach(var file in projectFiles)
            {
                 
                DotNetCorePack(file.FullPath, settings);                 
                 
            }
           
             
});

Task("Publish")
    .IsDependentOn("Tests")
    .Does(()=>{
            
            var projectFiles = GetFiles("./Src/**/*Host.csproj");
            foreach(var file in projectFiles)
            {
                 var publishProjectName = file.GetFilename().ToString().Replace(".csproj",string.Empty);
                 var hostPublishFolder =  $"./publish/{publishProjectName}/";
                var settings = new DotNetCorePublishSettings
                {
                   
                    Configuration = configuration,
                    OutputDirectory = hostPublishFolder,
                
                };
                
                DotNetCorePublish(file.ToString(), settings);
               
            }
             
        
    });

Task("Arquive")
.IsDependentOn("Publish")
.Does(()=>{
     
            var projectFiles = GetFiles("./Src/**/*Host.csproj");
            foreach(var file in projectFiles)
            {
                 var publishProjectName = file.GetFilename().ToString().Replace(".csproj",string.Empty);
                 var hostPublishFolder =  $"./publish/{publishProjectName}/";
                var settings = new DotNetCorePublishSettings
                {
                   
                    Configuration = configuration,
                    OutputDirectory = hostPublishFolder,
                
                };
                
                 ZipCompress(hostPublishFolder, $"./artifacts/{publishProjectName}.zip");
             }
 
});

Task("FullBuild")
.IsDependentOn("Clean")
.IsDependentOn("Build")
.IsDependentOn("Tests")
.IsDependentOn("Publish");


    RunTarget(target);
