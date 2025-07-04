using static Symbols;

public class DayspanParser(DateTime today) : ITimespanParsingStrategy
{
    public const long Pattern = 0b0110;

    // Expected pattern num|letter
    public Result<TimePeriod> Parse(SequenceBuilder sequence)
    {
        if (sequence.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        var num = sequence.Get(0);
        var chars = sequence.Get(1);

        return num.Str.PositiveResult()
                      .Ensure(i => i > 0, i => NUM_TOO_SMALL_ERR.With(i))
                      .Map(i => Evaluate(i, chars.Str));
    }

    private Result<TimePeriod> Evaluate(int num, string keyphrase)
    {
        return keyphrase switch
        {
            { } s when "days".StartsWith(keyphrase) => Result.Value(Days(num)),
            { } s when "weeks".StartsWith(keyphrase) => Result.Value(Weeks(num)),
            { } s when "quarters".StartsWith(keyphrase) => Result.Value(Quarters(num)),
            { } y when "years".StartsWith(keyphrase) => Result.Value(Years(num)),
            _ => Result.Fail<TimePeriod>($"Unknown keyphrase '{keyphrase}'")
        };
    }

    // Includes Today
    private TimePeriod Days(int num)
    {
        var end = today.Midnight();
        var start = end.AddDays(-num);

        return new(start, end);
    }

    // includes current week
    private TimePeriod Weeks(int num)
    {
        var sow = today.AddDays(-(num - 1) * 7).Sow();
        var eow = today.Eow();
        return new(sow, eow.Midnight());
    }

    // includes current quarter
    private TimePeriod Quarters(int num)
    {
        var eoq = today.Eoq();
        var quarter = today.Quarter();
        var prevQuarter = today.PastQuarter(num);

        return new(Time.QuarterStart(prevQuarter.year, prevQuarter.quarter), eoq.Midnight());
    }

    // includes current year
    private TimePeriod Years(int num)
    {
        var startYear = today.AddYears(-(num - 1));
        return new(startYear.Soy(), today.Midnight());
    }
}