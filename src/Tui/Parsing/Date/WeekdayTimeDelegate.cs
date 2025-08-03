using static SequencePatterns;

public class WeekdayTimeDelegate(DateTime today) : ISequenceParsingStrategy<DateTime>
{
    public const int PatternShort = (int)DL_N;
    public const int OffsetPattern = (int)NDL_N;
    public const int NegOffsetPatternShort = (int)_DL_N;
    public const int NegOffsetPattern = (int)_NDL_N;

    public Result<DateTime> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != PatternShort &&
            seq.Pattern != OffsetPattern &&
            seq.Pattern != NegOffsetPattern &&
            seq.Pattern != NegOffsetPatternShort)
            throw new ArgumentException("Parser not appropriate for current sequence!");

        var subSeq = seq.SplitLast(2);
        ISequenceParsingStrategy<DateTime> weekdayParser = new WeekdayParser(today);
        var dateResult = weekdayParser.Parse(subSeq.front);

        if (dateResult is Success<DateTime> s)
        {
            ISequenceParsingStrategy<DateTime> timeParser = new TimeParser(s.value);
            return timeParser.Parse(subSeq.back);
        }

        return dateResult;
    }
}

