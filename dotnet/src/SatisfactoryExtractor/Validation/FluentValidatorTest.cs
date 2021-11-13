namespace Rosser.SatisfactoryExtractor.Validation;

public record FluentValidatorTest
{
    /// <summary>The name of the item being tested</summary>
    public string Name { get; set; } = default!;
    /// <summary>The test to perform. Should return true if the test passes and false if it fails.</summary>
    public Func<bool> Test { get; set; } = default!;
}
