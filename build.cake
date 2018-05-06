using System.Text.RegularExpressions;

// ARGUMENTS

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var source = Argument<string>("source", null);
var apiKey = Argument<string>("apikey", null);
var version = Argument<string>("targetversion", null);

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

Task("Publish")
    .IsDependentOn("Run-Tests")
    .IsDependentOn("Prepare-Release")
    .IsDependentOn("Publish-NuGet");

Task("Publish-Local")
    .IsDependentOn("Run-Tests")
    .IsDependentOn("Update-Version")
    .IsDependentOn("Package-Nuget");

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

Task("Prepare-Release")
    .IsDependentOn("Update-Version")
    .Does(() =>
    {
        // Add
        var buildFile = MakeAbsolute(File("./src/Directory.Build.props"));
        Git($"add {buildFile.FullPath}");

        // Commit
        Git($"commit -m \"Updated version to {version}\"");

        // Tag
        Git($"tag \"v{version}\"");

        //Push
        Git("push");

        Git("push --tags");

        void Git(string command)
        {
            StartProcess("git", new ProcessSettings {
                Arguments = command
            });
        }
    });

Task("Update-Version")
    .Does(() =>
    {
        Information("Setting version to " + version);

        if(string.IsNullOrWhiteSpace(version))
        {
            throw new CakeException("No version specified! You need to pass in -targetversion=\"x.y.z\"");
        }

        var buildFile = MakeAbsolute(File("./src/Directory.Build.props"));

        Information("Updating version in " + buildFile.GetFilename().FullPath);

        var content = System.IO.File.ReadAllText(buildFile.FullPath, Encoding.UTF8);

        var versionRegex = new Regex(@"<Version>.+<\/Version>");

        content = versionRegex.Replace(content, $"<Version>{version}</Version>");

        System.IO.File.WriteAllText(buildFile.FullPath, content, Encoding.UTF8);
    });

Task("Default")
    .IsDependentOn("Run-Tests");

// EXECUTION

RunTarget(target);