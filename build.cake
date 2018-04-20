// ARGUMENTS

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var source = Argument<string>("source", null);
var apiKey = Argument<string>("apikey", null);


var output = Directory("build");
var outputNuGet = output + Directory("nuget");

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(new DirectoryPath[] {
            output,
            outputNuGet
        });

        CleanDirectories("./src/**/" + configuration);
    });

// TASKS

Task("Restore-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./EsiNet.sln");
});

Task("Build")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
{
	MSBuild("./EsiNet.sln", settings =>
		settings.SetConfiguration(configuration));
});

Task("Run-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    StartProcess("dotnet", new ProcessSettings {
        Arguments = "test ./src/Tests/Tests.csproj"
        });
});

Task("Package-NuGet")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach(var project in GetFiles("./src/**/EsiNet*.csproj"))
        {
            Information("Packaging " + project.GetFilename().FullPath);

            var content = System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);

            DotNetCorePack(project.GetDirectory().FullPath, new DotNetCorePackSettings {
                Configuration = configuration,
                OutputDirectory = outputNuGet
            });
        }
    });
    
Task("Publish-NuGet")
    .IsDependentOn("Package-Nuget")
    .Does(() =>
    {
        if(string.IsNullOrWhiteSpace(apiKey))
        {
            throw new CakeException("No NuGet API key provided. You need to pass in --apikey=\"xyz\"");
        }

        var packages =
            GetFiles(outputNuGet.Path.FullPath + "/*.nupkg") -
            GetFiles(outputNuGet.Path.FullPath + "/*.symbols.nupkg");

        foreach(var package in packages)
        {
            NuGetPush(package, new NuGetPushSettings {
                Source = source,
                ApiKey = apiKey
            });
        }
    });

Task("Default")
    .IsDependentOn("Run-Tests");

// EXECUTION

RunTarget(target);