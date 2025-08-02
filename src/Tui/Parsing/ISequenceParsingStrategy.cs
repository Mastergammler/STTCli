public interface ISequenceParsingStrategy<T>
{
    public Result<T> Parse(SequenceBuilder seq);
}

public class ErrorResultParser<T> : ISequenceParsingStrategy<T>
{
    public const long Pattern = 0b0;

    public Result<T> Parse(SequenceBuilder seq)
    {
        var errorSymbols = string.Join(", ", seq.Find(CharType.INVALID).Select(cs => $"'{cs.Str}'"));
        return new Failure<T>($"Invalid expression: '{seq.Expression}' - Unknown symobls: [{errorSymbols}]");
    }
}

public class UnknownPatternParser<T> : ISequenceParsingStrategy<T>
{
    public Result<T> Parse(SequenceBuilder seq)
    {
        return new Failure<T>($"Not implemented: Pattern {Convert.ToString(seq.Pattern, 2)} does not have a dedicated parser!");
    }
}