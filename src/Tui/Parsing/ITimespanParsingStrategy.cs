public interface ITimespanParsingStrategy
{
    public Result<TimePeriod> Parse(SequenceBuilder seq);
}

public class ErrorResultParser : ITimespanParsingStrategy
{
    public const long Pattern = 0b0;

    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        var errorSymbols = string.Join(", ", seq.Find(CharType.INVALID).Select(cs => $"'{cs.Str}'"));
        return new Failure<TimePeriod>($"Invalid expression: '{seq.Expression}' - Unknown symobls: [{errorSymbols}]");
    }
}

public class UnknownPatternParser : ITimespanParsingStrategy
{
    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        return new Failure<TimePeriod>($"Not implemented: Pattern {Convert.ToString(seq.Pattern, 2)} does not have a dedicated parser!");
    }
}