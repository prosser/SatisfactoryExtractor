namespace Rosser.SatisfactoryExtractor;

public class GameFinderException : Exception
{
    public GameFinderException() { }
    public GameFinderException(string message) : base(message) { }
    public GameFinderException(string message, Exception? innerException) : base(message, innerException) { }
}
