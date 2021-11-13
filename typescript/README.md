# SatisfactoryExtractor

Source for the satisfactory-extractor NPM packages.

These packages automate the extraction of [Satisfactory game](https://www.satisfactorygame.com/) assets for use in websites, apps, or mods.

## Installing


`yarn add satisfactory-extractor`

or

`npm install satisfactory-extractor`

## Usage
Generally, extracting assets is not something you want to do at runtime, but during the build of your application.

Add the following script to, or to an existing script in, your `packages.json`:

```json
{
    "scripts": {
        "extract-satisfactory": "ts-node "
    }
}
`

### .NET Core 3.1 or .NET 6.0 (Linux or Windows)

For a more complete example with commandline argument parsing, see [ExtractSatisfactoryAssets's implementation](./blob/main/dotnet/src/ExtractSatisfactoryAssets/Program.cs).

For a more concise example, see [NuGetTest's implementation](./blob/main/dotnet/test/NuGetTest/Program.cs).


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
