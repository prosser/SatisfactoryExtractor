# SatisfactoryExtractor

Source code for the `Rosser.SatisfactoryExtractor` NuGet package.

## Installing

Add the following to your .NET Core 3.1 or later .csproj:

```xml
<PackageReference Include="Rosser.SatisfactoryExtractor" Version="1.0.0" />
```

Or just search for `Rosser.SatisfactoryExtractor` using Visual Studio's `Manage NuGet packages...` feature.

## Usage

The package exposes several types in the `Rosser.SatisfactorExtractor` namespace, the most important being:

| Type                  | Description                               |
| --------------------- | ----------------------------------------- |
| `GameAssetsExtractor` | Class that performs extraction            |
| `GameFinder`          | Locates game installations (Windows-only) |


For a concise sample of how to use these classes, see [the sample program](../../samples/SampleProgram/Program.cs).
