namespace Rosser.SatisfactoryExtractor;

using Rosser.SatisfactoryExtractor.Validation;

public class GameFinderOptions
{
    /// <summary>Satisfactory update channel</summary>
    public Channel Channel { get; set; } = Channel.Experimental;

    /// <summary>true to only look for the game in Steam paths</summary>
    public bool OnlySteam { get; set; }

    /// <summary>true to only look for the game in Epic Games paths</summary>
    public bool OnlyEpic { get; set; }

    /// <summary>
    /// Gets a value indicating whether the options are valid.
    /// </summary>
    public bool IsValid => new FluentValidator()
        .Single(new[] {
            new FluentValidatorTest { Name = nameof(this.OnlyEpic), Test = () => this.OnlyEpic },
            new FluentValidatorTest { Name = nameof(this.OnlySteam), Test = () => this.OnlySteam },
        }).IsValid;
}
