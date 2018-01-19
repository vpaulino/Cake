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
                                args.Append($"--logger \"trx;LogFileName={testsResultFolder+unitTestsResultFileName}\"");
                                 args.Append("--filter \"TestCategory=Unit|Category=Unit\"");
                                 return args;
                    } 
                };
            foreach(var file in projectFiles)
            {
                               
                DotNetCoreTest(file.FullPath,settings);
            }
    });

Task("Publish")
    .IsDependentOn("Tests")
    .Does(()=>{
            
            var projectFiles = GetFiles("./Src/**/App*.csproj");
            foreach(var file in projectFiles)
            {
                 var publishProjectName = file.GetFilename().ToString().Replace(".csproj",string.Empty);
                var settings = new DotNetCorePublishSettings
                {
                   
                    Configuration = configuration,
                    OutputDirectory = $"./publish/{publishProjectName}/",
                
                };
                
                DotNetCorePublish(file.ToString(), settings);
            }
            
 
        
    });

Task("FullBuild")
.IsDependentOn("Clean")
.IsDependentOn("Build")
.IsDependentOn("Tests");

    RunTarget(target);
