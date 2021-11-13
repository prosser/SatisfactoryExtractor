# ExtractSatisfactoryAssets

A simple program to extract JSON and image assets from [Satisfactory](https://satisfactorygame.com).

## Installation

Prerequisites:

- [.NET 6.0](https://dotnet.microsoft.com/download) (recommended) or [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)

### dotnet tool

Run `dotnet tool install Rosser.SatisfactoryExtractor.Tool`

## Usage

The dotnet tool command name is `extractsatisfactory`.

### Windows
```
dotnet extractsatisfactory -o PATH [OPTIONS]

-o PATH,--output-dir PATH       Path to the directory where extracted assets will be saved.

OPTIONS:
-c CHANNEL,--channel CHANNEL    CHANNEL = experimental or earlyaccess
                                If channel is specified, input dir must not be.
                                Default = experimental

-i PATH,--input-dir PATH        Path to the directory where the game is installed.
                                If specified, channel must not be.

```

#### Examples

Find an experimental channel installation automatically and extract the assets to `c:\temp\satisfactory-assets`.

```powershell
dotnet extractsatisfactory -o c:\temp\satisfactory-assets
```

Find an early access channel installation automatically and extract the assets to `c:\temp\satisfactory-assets`.

```powershell
dotnet extractsatisfactory -c earlyaccess -o c:\temp\satisfactory-assets
```

Extract the game assets from `C:\Program Files\Epic Games\SatisfactoryExperimental` to `c:\temp\satisfactory-assets`:

```shell
dotnet extractsatisfactory -i "C:\Program Files\Epic Games\SatisfactoryExperimental" -o c:\temp\satisfactory-assets
```


### Linux

```
dotnet extractsatisfactory -i PATH -o PATH

-i PATH,--input-dir PATH    Path to the directory where the game is installed.

-o PATH,--output-dir PATH   Path to the directory where extracted assets will be saved.
```

#### Examples

Extract the game assets from `/mnt/satisfactory` to `~/satisfactory-assets`:

```shell
dotnet extractsatisfactory -i /mnt/satisfactory -o ~/satisfactory-assets
```

or

```shell
dotnet extractsatisfactory --input-dir /mnt/satisfactory --output-dir ~/satisfactory-assets
```
