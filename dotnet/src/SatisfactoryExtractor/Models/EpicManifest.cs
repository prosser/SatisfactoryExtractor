namespace Rosser.SatisfactoryExtractor.Models;
using System;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "External definition")]
internal record EpicManifest
{
    public int FormatVersion { get; set; }

    public bool bIsIncompleteInstall { get; set; }
    public string LaunchCommand { get; set; } = default!;
    public string LaunchExecutable { get; set; } = default!;
    public string ManifestLocation { get; set; } = default!;
    public bool bIsApplication { get; set; }
    public bool bIsExecutable { get; set; }
    public bool bIsManaged { get; set; }
    public bool bNeedsValidation { get; set; }
    public bool bRequiresAuth { get; set; }
    public bool bAllowMultipleInstances { get; set; }
    public bool bCanRunOffline { get; set; }
    public bool bAllowUriCmdArgs { get; set; }
    public string[] BaseURLs { get; set; } = default!;
    public string BuildLabel { get; set; } = default!;
    public string[] AppCategories { get; set; } = default!;
    public string[] ChunkDbs { get; set; } = default!;
    public string[] CompatibleApps { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string InstallationGuid { get; set; } = default!;
    public string InstallLocation { get; set; } = default!;
    public string InstallSessionId { get; set; } = default!;
    public string[] InstallTags { get; set; } = default!;
    public string[] InstallComponents { get; set; } = default!;
    public string HostInstallationGuid { get; set; } = default!;
    public string[] PrereqIds { get; set; } = default!;
    public string StagingLocation { get; set; } = default!;
    public string TechnicalType { get; set; } = default!;
    public string VaultThumbnailUrl { get; set; } = default!;
    public string VaultTitleText { get; set; } = default!;
    public long InstallSize { get; set; }
    public string MainWindowProcessName { get; set; } = default!;
    public string[] ProcessNames { get; set; } = default!;
    public string[] BackgroundProcessNames { get; set; } = default!;
    public string MandatoryAppFolderName { get; set; } = default!;
    public string OwnershipToken { get; set; } = default!;
    public string CatalogNamespace { get; set; } = default!;
    public string CatalogItemId { get; set; } = default!;
    public string AppName { get; set; } = default!;
    public string AppVersionString { get; set; } = default!;
    public string MainGameCatalogNamespace { get; set; } = default!;
    public string MainGameCatalogItemId { get; set; } = default!;
    public string MainGameAppName { get; set; } = default!;
    public string[] AllowedUriEnvVars { get; set; } = default!;
}
