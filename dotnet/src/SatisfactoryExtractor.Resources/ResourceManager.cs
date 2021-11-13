namespace Rosser.SatisfactoryExtractor.Resources;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

public sealed class ResourceManager : IDisposable
{
    private const string BaseResourceName = "Rosser.SatisfactoryExtractor.Resources.";
    private readonly List<string> createdDirectories = new();
    private readonly Dictionary<ToolType, string> programs = new();

    public ResourceManager()
    {
        string unpackDir = this.UnpackFiles();
        if (OsHelper.IsLinux)
        {
            string toolPath = Path.Combine(unpackDir, "umodel");
            this.programs.Add(ToolType.UModel, toolPath);
            Process.Start("chmod", $"+x {toolPath}").WaitForExit();
        }
        else if (OsHelper.IsWindows)
        {
            this.programs.Add(ToolType.UModel, Path.Combine(unpackDir, "umodel_64.exe"));
        }
        else
        {
            throw new NotSupportedException();
        }

        this.createdDirectories.Add(unpackDir);
    }

    ~ResourceManager()
    {
        this.DeleteCreatedDirectories();
    }

    public void Dispose()
    {
        this.DeleteCreatedDirectories();
        GC.SuppressFinalize(this);
    }

    public void Execute(ToolType tool, string arguments)
    {
        Process.Start(this.programs[tool], arguments).WaitForExit();
    }

    private void DeleteCreatedDirectories()
    {
        foreach (string dir in this.createdDirectories)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch { }
        }

        this.createdDirectories.Clear();
    }

    private string UnpackFiles()
    {
        var assembly = Assembly.GetExecutingAssembly();
        AssemblyName assemblyName = assembly.GetName();
        string outDir = Path.Combine(Path.GetTempPath(), assemblyName.Name + "_" + assemblyName.Version.ToString(3));
        Directory.CreateDirectory(outDir);
        foreach (string resourceName in assembly.GetManifestResourceNames().Where(x => x.Contains(OsHelper.PlatformName)))
        {
            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                   ?? throw new InvalidOperationException($"Missing resource {resourceName}");

            string fileName = resourceName.Substring(BaseResourceName.Length + OsHelper.PlatformName.Length + 1);

            switch (Path.GetExtension(resourceName))
            {
                case ".gz":
                    {
                        using GZipStream gzip = new(stream, CompressionMode.Decompress);
                        string outPath = Path.Combine(outDir, Path.GetFileNameWithoutExtension(fileName));
                        using FileStream file = File.Create(outPath);
                        gzip.CopyTo(file);
                    }

                    break;

                case ".zip":
                    {
                        using ZipArchive zipArchive = new(stream);
                        zipArchive.ExtractToDirectory(outDir);
                    }

                    break;
                case ".resources":
                    break;

                default:
                    {
                        string outPath = Path.Combine(outDir, fileName);
                        using FileStream file = File.Create(outPath);
                        stream.CopyTo(file);
                    }

                    break;
            }
        }

        return outDir;
    }
}