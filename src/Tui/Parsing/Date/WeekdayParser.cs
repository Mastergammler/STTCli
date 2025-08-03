using static SequencePatterns;

public class WeekdayParser(DateTime today) : ISequenceParsingStrategy<DateTime>, ISequenceParsingStrategy<TimePeriod>
{
    public const int PatternShort = (int)DL;
    public const int OffsetPattern = (int)NDL;
    public const int NegOffsetPatternShort = (int)_DL;
    public const int NegOffsetPattern = (int)_NDL;

    Result<DateTime> ISequenceParsingStrategy<DateTime>.Parse(SequenceBuilder seq)
    {
        return seq.Pattern switch
        {
            PatternShort => Evaluate(Result.Value(0), seq.Get(1).Str),
            OffsetPattern => Evaluate(seq.Get(0).Str.PositiveResult(), seq.Get(2).Str),
            NegOffsetPatternShort => Evaluate(Result.Value(-1), seq.Get(2).Str),
            NegOffsetPattern => Evaluate(seq.Get(1).Str.PositiveResult().Map(n => -n), seq.Get(3).Str),
            _ => throw new ArgumentException("Parser not appropriate for current sequence!")
        };
    }

    private Result<DateTime> Evaluate(Result<int> num, string keyphrase)
    {
        string keyphraseInv = keyphrase.ToLower();
        Result<DayOfWeek> weekday = keyphraseInv switch
        {
            "t" or "s" => Result.Fail<DayOfWeek>($"Ambiguous input: '{keyphraseInv}' has multiple matches"),
            { } s when "monday".StartsWith(s) => Result.Value(DayOfWeek.Monday),
            { } s when "tuesday".StartsWith(s) => Result.Value(DayOfWeek.Tuesday),
            { } s when "wednesday".StartsWith(s) => Result.Value(DayOfWeek.Wednesday),
            { } s when "thursday".StartsWith(s) => Result.Value(DayOfWeek.Thursday),
            { } s when "friday".StartsWith(s) => Result.Value(DayOfWeek.Friday),
            { } s when "saturday".StartsWith(s) => Result.Value(DayOfWeek.Saturday),
            { } s when "sunday".StartsWith(s) => Result.Value(DayOfWeek.Sunday),
            _ => Result.Fail<DayOfWeek>($"Unknown keyphrase '{keyphrase}'")
        };

        return num.Combine(weekday, (num, weekday) => today.Weekday(weekday, num));
    }

    Result<TimePeriod> ISequenceParsingStrategy<TimePeriod>.Parse(SequenceBuilder seq)
    {
        var dateResult = ((ISequenceParsingStrategy<DateTime>)this).Parse(seq);
        return dateResult.Map(d => new TimePeriod(d, d.Midnight()));
    }
}