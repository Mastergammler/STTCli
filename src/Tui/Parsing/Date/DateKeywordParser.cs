public class DateKeywordParser(DateTime today) : ISequenceParsingStrategy<DateTime>
{
    public const int Pattern = (int)SequencePatterns.L;

    public Result<DateTime> Parse(SequenceBuilder seq)
    {
        return seq.Expression switch
        {
            { } s when "today".StartsWith(s) => Result.Value(Time.Today()),
            { } s when "eod".Equals(s) => Result.Value(Time.Today().WithTime()),
            { } s when "eow".Equals(s) => Result.Value(today.Eow()),
            { } s when "sow".Equals(s) => Result.Value(today.Sow()),
            { } s when "som".Equals(s) => Result.Value(today.Som()),
            { } s when "eom".Equals(s) => Result.Value(today.Eom()),
            { } s when "soy".Equals(s) => Result.Value(today.Soy()),
            { } s when "eoy".Equals(s) => Result.Value(today.Eoy()),

            _ => new Failure<DateTime>($"Unknown date keyword: '{seq.Expression}'")
        };
    }
}