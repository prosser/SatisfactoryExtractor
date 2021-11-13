namespace Rosser.SatisfactoryExtractor.Validation;

public class FluentValidatorException : Exception
{
    public FluentValidatorException(string message) : base(message) { }
    public FluentValidatorException(string message, Exception? innerException) : base(message, innerException) { }
}
