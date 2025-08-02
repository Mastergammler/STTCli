public class PrevTimespanParser(DateTime today) : ISequenceParsingStrategy<TimePeriod>
{
    public const long Pattern = 0b101101;

    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern != Pattern) throw new ArgumentException("Parser not appropriate for current sequence!");

        var chars = seq.Get(0);
        var num = seq.Get(2);

        return num.Str.PositiveResult()
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

    private TimePeriod Days(int num)
    {
        var refDate = today.AddDays(-num);

        var start = refDate.Date;
        var end = refDate.Midnight();

        return new(start, end);
    }

    private TimePeriod Weeks(int num)
    {
        var refDate = today.AddDays(-num * 7);
        return new(refDate.Sow(), refDate.Eow().Midnight());
    }

    private TimePeriod Quarters(int num)
    {
        var pastQuarter = today.PastQuarter(num);
        var soq = Time.QuarterStart(pastQuarter.year, pastQuarter.quarter);
        var eoq = soq.Eoq();

        return new(soq, eoq.Midnight());
    }

    private TimePeriod Years(int num)
    {
        var refDate = today.AddYears(-num);
        return new(refDate.Soy(), refDate.Eoy().Midnight());
    }
}