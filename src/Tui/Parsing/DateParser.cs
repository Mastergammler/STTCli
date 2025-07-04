public class IsoDateParser(DateTime today) : ITimespanParsingStrategy
{
    public const long Pattern = 0b0111011101;

    public Result<TimePeriod> Parse(SequenceBuilder sequence)
    {
        if (sequence.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        Result<DateTime> dateResult;
        var yearPart = sequence.Get(0);
        if (yearPart.CharCount == 2)
        {
            string curYearPrefix = today.Year.ToString()[..2];
            string dateExpr = $"{curYearPrefix}{sequence.Expression}";
            dateResult = dateExpr.DateResult();
        }
        else
        {
            dateResult = sequence.Expression.DateResult();
        }

        return dateResult.Map(Time.SingleDay);
    }
}

public class YearDateParser(DateTime today) : ITimespanParsingStrategy
{
    public const long Pattern = 0b011101;

    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        string curYearPrefix = $"{today.Year}-";
        string dateExpr = $"{curYearPrefix}{seq.Expression}";

        return dateExpr.DateResult().Map(Time.SingleDay);
    }
}

public class SingleNumberParser(DateTime today) : ITimespanParsingStrategy
{
    public const long Pattern = 0b01;

    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        int length = seq.Get(0).CharCount;

        Result<DateTime> dateResult = Result.Fail<DateTime>($"Date has to be either <dd> or <MM><dd> respectively. Length {length} not supported");

        if (length == 4)
        {
            string monthStr = seq.Expression[..2];
            string dayStr = seq.Expression[2..];
            string dateStr = $"{today.Year}-{monthStr}-{dayStr}";
            dateResult = dateStr.DateResult();
        }
        else if (length == 2)
        {
            string dateStr = $"{today.Year}-{today.Month}-{seq.Expression}";
            dateResult = dateStr.DateResult();
        }

        return dateResult.Map(Time.SingleDay);
    }
}