// This is a sample written for .NET 6.0 You must have Visual Studio 2022 or VS Code w/ .NET 6.0 SDK to compile it!
// (and you must have .NET 6.0 runtime installed to run it...)

using System;
using System.IO;

using Rosser.SatisfactoryExtractor;

// configure options
var options = new GameFinderOptions
{
    Channel = Channel.Experimental,
    // OnlyEpic = true, // uncomment to only search for Epic Games installations
    // OnlySteam = true, // uncomment to only search for Steam installations
};

// just want to find the game? use GameFinder...
Console.WriteLine($"Found Satisfactory {options.Channel} in \"{GameFinder.FindGame(options)}\"");

// extract to "satisfactory-assets" under the current working directory
string targetPath = Path.Combine(Environment.CurrentDirectory, "satisfactory-assets");

// extract the assets
GameAssetsExtractor.ExtractGameAssets(options, targetPath);
