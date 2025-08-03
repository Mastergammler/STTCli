using static SequencePatterns;

public class IsoDateParser(DateTime today) : ISequenceParsingStrategy<TimePeriod>, ISequenceParsingStrategy<DateTime>
{
    public const int Pattern = (int)N_N_N;

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

    Result<DateTime> ISequenceParsingStrategy<DateTime>.Parse(SequenceBuilder seq)
    {
        return ((ISequenceParsingStrategy<TimePeriod>)this).Parse(seq).Map(d => d.Start);
    }
}

public class YearDateParser(DateTime today) : ISequenceParsingStrategy<TimePeriod>
{
    public const int Pattern = (int)N_N;

    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        string curYearPrefix = $"{today.Year}-";
        string dateExpr = $"{curYearPrefix}{seq.Expression}";

        return dateExpr.DateResult().Map(Time.SingleDay);
    }
}

public class SingleNumberParser(DateTime today) : ISequenceParsingStrategy<TimePeriod>, ISequenceParsingStrategy<DateTime>
{
    public const int Pattern = (int)N;

    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        int length = seq.Get(0).CharCount;

        Result<DateTime> dateResult = Result.Fail<DateTime>($"Date has to be either <dd> or <MM><dd> respectively. Length {length} not supported");

        if (length == 3 || length == 4)
        {
            int splitIdx = length == 4 ? 2 : 1;
            string monthStr = seq.Expression[..splitIdx];
            string dayStr = seq.Expression[splitIdx..];
            string dateStr = $"{today.Year}-{monthStr.PadLeft(2, '0')}-{dayStr}";
            dateResult = dateStr.DateResult();
        }
        else if (length == 1 || length == 2)
        {
            string dateStr = $"{today.Year}-{today.Month}-{seq.Expression.PadLeft(2, '0')}";
            dateResult = dateStr.DateResult();
        }

        return dateResult.Map(Time.SingleDay);
    }

    Result<DateTime> ISequenceParsingStrategy<DateTime>.Parse(SequenceBuilder seq)
    {
        return ((ISequenceParsingStrategy<TimePeriod>)this).Parse(seq).Map(d => d.Start);
    }
}