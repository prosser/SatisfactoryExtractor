namespace Rosser.SatisfactoryExtractor;
using Rosser.SatisfactoryExtractor.Resources;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

public class GameAssetsExtractor
{
    public static void ExtractGameAssets(GameFinderOptions options, string targetDir)
    {
        string installPath = GameFinder.FindGame(options);
        ExtractGameAssets(installPath, targetDir);
    }

    public static void ExtractGameAssets(string installPath, string targetDir)
    {
        DirectoryInfo iconDir = Directory.CreateDirectory(Path.Combine(targetDir, "icons"));
        ExtractJson(installPath, targetDir);

        DirectoryInfo pakDir = new(Path.Combine(installPath, "FactoryGame", "Content", "Paks"));

        ImportPaks(pakDir, iconDir);
    }

    private static void ExtractJson(string installPath, string targetDir)
    {
        DirectoryInfo jsonDir = Directory.CreateDirectory(Path.Combine(targetDir, "json"));

        string sourceDocsPath = Path.Combine(installPath, "CommunityResources", "Docs", "Docs.json");
        string targetDocsPath = Path.Combine(jsonDir.FullName, "Docs.json");
        using StreamReader reader = new(sourceDocsPath);
        string json = reader.ReadToEnd();
        using FileStream outStream = File.OpenWrite(targetDocsPath);
        var encoderSettings = new TextEncoderSettings();
        encoderSettings.AllowRange(UnicodeRanges.BasicLatin);
        encoderSettings.AllowCharacters('_', '\'', '-');

        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(encoderSettings),
        };

        using var doc = JsonDocument.Parse(json);

        var writer = new Utf8JsonWriter(outStream);
        JsonSerializer.Serialize(writer, doc.RootElement.Clone(), serializerOptions);
    }

    private static void ImportPaks(DirectoryInfo pakDir, DirectoryInfo targetDir)
    {
        // As of Update 5, 4.22 no longer works. Updated to the latest version that did work.
        const string UNREAL_ENGINE_VERSION = "ue4.27";

        targetDir.Create();

        string[] variants = new[] { "512", "256", "128", "64" }
            .SelectMany(x => new[] { $"*_{x}.uasset", $"*_{x}_New.uasset", $"*_{x}_new.uasset" })
            .Concat(new[] { "*Icon*.uasset" })
            .ToArray();

        // add variants that we don't need to permute
        string[] commands = variants
            .Select(v => $"-path=\"{pakDir}\" -out=\"{targetDir}\" -png -export {v} -game={UNREAL_ENGINE_VERSION}")
            .ToArray();

        Console.WriteLine($"Exporting PAK content from {pakDir}");

        var extractor = new ResourceManager();
        foreach (string arguments in commands)
        {
            Console.WriteLine($"umodel {arguments}");
            extractor.Execute(ToolType.UModel, arguments);
        }
    }
}