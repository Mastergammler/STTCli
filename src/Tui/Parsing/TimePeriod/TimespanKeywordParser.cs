public class TimespanKeywordParser(DateTime today) : ISequenceParsingStrategy<TimePeriod>
{
    public const long Pattern = 0b10;

    public Result<TimePeriod> Parse(SequenceBuilder builder)
    {
        int length = builder.TotalCharCount;

        return builder.Expression switch
        {
            { } s when "today".StartsWith(s) => new Success<TimePeriod>(Today()),
            { } s when "yesterday".StartsWith(s) => new Success<TimePeriod>(Yesterady()),
            { } s when "week".StartsWith(s) => new Success<TimePeriod>(Week()),
            { } s when "month".StartsWith(s) => new Success<TimePeriod>(Month()),
            { } s when "quarter".StartsWith(s) => new Success<TimePeriod>(Quarter()),
            { } s when "year".StartsWith(s) => new Success<TimePeriod>(Year()),
            _ => new Failure<TimePeriod>($"Unknown timespan keyword: '{builder.Expression}'")
        };
    }

    private TimePeriod Today() => new(today.Date, today.Date.AddDays(1));
    private TimePeriod Yesterady() => new(today.Date.AddDays(-1), today.Date);
    private TimePeriod Week()
    {
        var sow = today.Sow();
        return new(sow, sow.AddDays(8));
    }
    private TimePeriod Month()
    {
        var som = today.Som();
        return new(som, som.AddMonths(1));
    }

    private TimePeriod Quarter() => new(today.Soq(), today.Eoq().AddDays(1));
    private TimePeriod Year()
    {
        var soy = today.Soy();
        return new(soy, soy.AddYears(1));
    }
}