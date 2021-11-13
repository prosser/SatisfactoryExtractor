namespace Rosser.SatisfactoryExtractor;

using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Rosser.SatisfactoryExtractor.Models;
using System.Text.Json;

#pragma warning disable IDE0079 // Remove unnecessary suppression
[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Has runtime checks")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
public class GameFinder
{
    public static string FindGame(GameFinderOptions options)
    {
        if (OsHelper.IsLinux)
        {
            throw new NotSupportedException("Not supported in Linux.");
        }

        try
        {
            if (!options.OnlyEpic)
            {
                try
                {
                    return GetSteamGamePath(options.Channel);
                }
                catch (Exception)
                {
                    if (options.OnlySteam)
                    {
                        throw;
                    }
                }
            }

            if (!options.OnlySteam)
            {
                return GetEpicGamesPath(options.Channel);
            }
        }
        catch (GameFinderException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new GameFinderException($"Could not find {options.Channel} installation: {ex.Message ?? "unknown error"}. Options={options}", ex);
        }

        throw new Exception("An unknown error ocurred.");
    }

    private static string? GetRegistryString(string path, string valueName)
    {
        using RegistryKey? key = Registry.LocalMachine.OpenSubKey(path);
        return key?.GetValueKind(valueName) == RegistryValueKind.String
            ? key.GetValue(valueName) as string
            : null;
    }

    private static string FindWindowsSteamLibrariesVdf()
    {
        string? steamPath = GetRegistryString(@"SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath");
        if (steamPath is null)
        {
            throw new GameFinderException("Steam is not installed properly.");
        }

        // Steam can have multiple library locations, which are in the VDF format in a known location.
        string librariesVdf = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

        return librariesVdf;
    }

    private static string FindWindowsEpicManifests()
    {
        string? appDataPath = GetRegistryString(@"SOFTWARE\WOW6432Node\Epic Games\EpicGamesLauncher", "AppDataPath");
        return appDataPath is null
            ? throw new GameFinderException("Epic Games registry entry not found")
            : Path.Combine(appDataPath, "Manifests");
    }

    private static string GetEpicGamesPath(Channel channel)
    {
        // This is the Epic Games catalog item id for the Experimental fork of Satisfactory.
        // We could use DisplayName, but that would be less accurate.
        string catalogId = channel switch
        {
            Channel.Experimental => "ef4a63daa7d4420e91420a72050be89d",
            Channel.EarlyAccess => "b915dfe8dcf74770841c82a4162dc954",
            _ => throw new NotSupportedException("Invalid channel")
        };

        // in the Manifests folder, there will be one .item file per installed game.
        string manifestsFolder = FindWindowsEpicManifests();

        if (!Directory.Exists(manifestsFolder))
        {
            throw new DirectoryNotFoundException($"Epic Games manifests directory \"{manifestsFolder}\" does not exist");
        }

        foreach (string itemFileName in Directory.GetFiles(manifestsFolder))
        {
            if (!itemFileName.EndsWith(".item"))
            {
                continue;
            }

            string itemPath = Path.Combine(manifestsFolder, itemFileName);

            // Epic's manifests are in JSON format. Games are keyed by the catalog item id.
            using StreamReader reader = new(itemPath);
            EpicManifest? manifest = JsonSerializer.Deserialize<EpicManifest>(reader.ReadToEnd());
            if (manifest?.CatalogItemId == catalogId)
            {
                return manifest.InstallLocation;
            }
        }

        throw new GameFinderException("Could not find installed game in any Epic Games library");
    }

    private static string GetSteamGamePath(Channel channel)
    {
        // look in the Windows registry to find Steam's installation path
        const string steamAppId = "526870";

        string librariesVdf = FindWindowsSteamLibrariesVdf();

        if (!File.Exists(librariesVdf))
        {
            throw new FileNotFoundException(librariesVdf);
        }

        JObject manifest = VdfConvert.Deserialize(File.ReadAllText(librariesVdf)).Value.ToJson() as JObject
            ?? throw new GameFinderException($"Could not deserialize Steam VDF at {librariesVdf}");

        // find the library that contains the Satisfactory game
        foreach (JProperty prop in manifest.Children().Cast<JProperty>())
        {
            var library = prop.Value as JObject;
            string? libraryPath = library?["path"]?.Value<string>();
            // sanity checking...
            if (library is null ||
                libraryPath is null || !Directory.Exists(libraryPath) ||
                library.GetValue("apps") is not JObject apps ||
                // if the library contains the game, this will be present.
                !apps.ContainsKey(steamAppId))
            {
                continue;
            }

            string steamappsDir = Path.Combine(libraryPath, "steamapps");

            // Steam thinks the game is installed, so look for the game's local install path in its VDF manifest.
            string appManifestPath = Path.Combine(steamappsDir, $"appmanifest_{steamAppId}.acf");
            if (!File.Exists(appManifestPath))
            {
                continue;
            }

            // the manifest exists, so parse it
            if (VdfConvert.Deserialize(File.ReadAllText(appManifestPath)).Value.ToJson() is not JObject appManifest)
            {
                continue;
            }

            // verify that this is the correct channel
            Channel appChannel = appManifest["UserConfig"]?["betakey"]?.Value<string>() switch
            {
                "experimental" => Channel.Experimental,
                _ => Channel.EarlyAccess
            };
            if (appChannel != channel)
            {
                continue;
            }

            string? installDir = appManifest["installdir"]?.Value<string>();
            if (installDir is null)
            {
                continue;
            }

            // Steam thinks the game is installed in this library, but sanity check that it actually still exists.
            string installPath = Path.Combine(steamappsDir, "common", installDir);
            if (!Directory.Exists(installPath))
            {
                continue;
            }

            return installPath;
        }

        throw new GameFinderException("Could not find installed game in any Steam library");
    }
}