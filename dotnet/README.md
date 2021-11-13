# .NET Satisfactory Assets Extractor

Multiplatform (Linux & Windows) tools and libraries to extract JSON and image assets from [Satisfactory](https://satisfactorygame.com) written in C#.

## Installing

| How you want to use it          | README link                                         |
| ------------------------------- | --------------------------------------------------- |
| Commandline using `dotnet tool` | [README](./src/ExtractSatisfactoryAssets/README.md) |
| In your own .NET program        | [README](./src/SatisfactoryExtractor/README.md)     |


## Building

### Prerequisites

You can either build with Visual Studio 2022 (make sure you have the .NET Core 3.1 workload installed), or
using the free SDK:

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/3.1)

### Visual Studio 2022

Open `SatisfactoryExtractor.sln` and build.

### dotnet SDK

From a PowerShell terminal session:

```powershell
dotnet build
```
