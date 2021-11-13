namespace Rosser.SatisfactoryExtractor;

using Microsoft.Extensions.Configuration;
using Rosser.SatisfactoryExtractor.Validation;

public static class Program
{
    private const string InputDirKey = "inputDir";
    private const string OutputDirKey = "outputDir";
    private const string ChannelKey = "channel";

    private const string LinuxUsage = @"Usage:
ExtractSatisfactoryAssets PARAMETERS

PARAMETERS:
-i PATH,--input-dir PATH    Path to the directory where the game is installed.

-o PATH,--output-dir PATH   Path to the directory where extracted assets will be saved.";

    private const string WindowsUsage = @"Usage:
ExtractSatisfactoryAssets.exe OPTIONS

OPTIONS:
-c CHANNEL,--channel CHANNEL    CHANNEL = experimental or earlyaccess
                                If channel is specified, input dir must not be.
                                Default = experimental

-i PATH,--input-dir PATH        Path to the directory where the game is installed.
                                If specified, channel must not be.

-o PATH,--output-dir PATH       Path to the directory where extracted assets will be saved.";

    public static int Main(string[] args)
    {
        try
        {
            var switchMappings = new Dictionary<string, string>
            {
                ["--input-dir"] = InputDirKey,
                ["-i"] = InputDirKey,
                ["--output-dir"] = OutputDirKey,
                ["-o"] = OutputDirKey
            };

            if (OperatingSystem.IsWindows())
            {
                switchMappings.Add("--channel", "channel");
                switchMappings.Add("-c", "channel");
            }

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddCommandLine(args, switchMappings)
                .Build();

            string inputDir = config[InputDirKey];
            string outputDir = config[OutputDirKey];
            Channel? channelArg = config[ChannelKey] switch
            {
                "experimental" => Channel.Experimental,
                "earlyaccess" => Channel.EarlyAccess,
                _ => null
            };

            FluentValidator validator = new FluentValidator(true)
                .Single(new[]
                {
                    new FluentValidatorTest { Name = "--input-dir", Test = () => !string.IsNullOrEmpty(inputDir) },
                    new FluentValidatorTest { Name = "--channel", Test = () => channelArg.HasValue}
                })
                .True("--input-dir", () => channelArg.HasValue || OperatingSystem.IsWindows() || !string.IsNullOrWhiteSpace(inputDir))
                .True("--output-dir", () => !string.IsNullOrWhiteSpace(outputDir));

            if (!string.IsNullOrWhiteSpace(inputDir))
            {
                validator = validator.DirectoryExists(config[InputDirKey]);
            }

            if (string.IsNullOrWhiteSpace(inputDir))
            {
                inputDir = GameFinder.FindGame(new GameFinderOptions
                {
                    Channel = channelArg ?? Channel.Experimental,
                });
            }

            GameAssetsExtractor.ExtractGameAssets(inputDir, outputDir);
            return 0;
        }
        catch (FluentValidatorException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ex.Message);
            Console.ResetColor();

            PrintUsage();

            return 1;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static void PrintUsage()
    {
        Console.Error.WriteLine(OperatingSystem.IsLinux()
            ? LinuxUsage
            : WindowsUsage);
    }
}

