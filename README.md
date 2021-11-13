# SatisfactoryExtractor

Source for the `satisfactory-extractor` NPM and `Rosser.SatisfactoryExtractor` NuGet packages.

These packages automate the extraction of [Satisfactory game](https://www.satisfactorygame.com/) assets for use in websites, apps, or mods.

## I just want to extract the assets

For Linux instructions, [go here](./dotnet/src/ExtractSatisfactoryAssets/README.md#linux).

For Windows:

```
dotnet tool install --global Rosser.SatisfactoryExtractor.Tool --version 1.0.0
dotnet extractsatisfactory -o ./satisfactory-assets
```

## Installing

Pick your toolchain:

[Node.js](./typescript/README.md)
[.NET (Linux or Windows)](./dotnet/README.md)

## Usage
Generally, extracting assets is not something you want to do at runtime, but during the build of your application.

To extract assets, you can either use the command-line application `SatisfactoryExtractor.exe` (or via `dotnet tool extract-satisfactory`), or call it from your .NET or Node.js application.

## License

This software is covered under the MIT license. See [LICENSE](./LICENSE) for legal details.

## Contributing

Pull Requests are happily accepted with these conditions:

- Clean: all PRs must build cleanly. No disabled or removed tests (unless the code it's testing has gone away!)
- Tested: All new code have sufficient unit test coverage (the build should catch this).
- Single-subject: must generally address a single feature or issue.
- Minimal drift: this is a multi-platform repository with both Node and .NET packages. If you add functionality to one, you should add it to the other, too! If you are not comfortable with one of those stacks, I may be able to help port your changes.
- No new binary dependencies
- Legal: You must have legal rights to all content, code or otherwise, to be merged.
