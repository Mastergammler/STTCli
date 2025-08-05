using static Symbols;
using static SequencePatterns;

public class TimeParser(DateTime today) : ISequenceParsingStrategy<DateTime>
{
    public const int Pattern = (int)_N;
    public const int PatternDirect = (int)N;

    public Result<DateTime> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != Pattern && seq.Pattern != PatternDirect)
            throw new ArgumentException("Parser not appropriate for current sequence!");

        int seqIdx = seq.Pattern == PatternDirect ? 0 : 1;

        int length = seq.Get(seqIdx).CharCount;
        string timeStr = seq.Get(seqIdx).Str;

        if (length == 3 || length == 4)
        {
            var splitIdx = timeStr.Length == 4 ? 2 : 1;
            var hourStr = timeStr[..splitIdx];
            var minStr = timeStr[splitIdx..];

            var minResult = minStr.ParseInt(i => i >= 0 && i < 60, NUM_INVALID_MIN);
            return hourStr.ParseInt(i => i >= 0 && i < 24, NUM_INVALID_HOUR)
                          .Combine(minResult, (h, m) => new DateTime(today.Year, today.Month, today.Day, h, m, 0, DateTimeKind.Local))
                          .Map(d => d.ToUniversalTime());
        }
        else
        {
            return Result.Fail<DateTime>($"Time expression expected in format <Hmm> or <HHmm>. Length {length} not supported.");
        }
    }
}

//TODO: this should be a delegate parser also, similar to weekday delegate
// -> to be consistent
public class DatedTimeParser(DateTime today) : ISequenceParsingStrategy<DateTime>
{
    public const int Pattern = (int)N_N;

    public Result<DateTime> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        var datePart = seq.Get(0).Str;
        var timePart = seq.Get(2).Str;

        var hourString = Source.Of(timePart)
                               .Ensure(s => s.Length == 3 || s.Length == 4, s => "Time part expected in format <Hmm> or <HHmm>.")
                               .Map(s => s.PadLeft(4, '0'))
                               .Map(s => $"{s[..2]}:{s[2..]}:00");
        var yearString = Source.Of(datePart)
                               .Ensure(s => s.Length == 3 || s.Length == 4, s => "Date part expected in format <Mdd> or <MMdd>.")
                               .Map(s => s.PadLeft(4, '0'))
                               .Map(s => $"{today.Year}-{s[..2]}-{s[2..]}");

        return yearString.Combine(hourString, (yStr, hStr) => $"{yStr}T{hStr}")
                         .DateResult();
    }
}