public class QuarterParser(DateTime today) : ISequenceParsingStrategy<TimePeriod>
{
    public const long PatternShort = 0b1001;
    public const long PatternLong = 0b01111001;

    public Result<TimePeriod> Parse(SequenceBuilder seq)
    {
        if (seq.Pattern == PatternShort)
        {
            return HandleQuarterResult(seq, today.Year);
        }
        else if (seq.Pattern == PatternLong)
        {
            var yearPart = seq.Get(0);

            Result<int> yearResult = yearPart.Str.PositiveResult(); ;
            if (yearPart.CharCount == 2)
            {
                // truncating the last 2 digits
                int yearThousands = (today.Year / 100) * 100;
                yearResult = yearResult.Map(y => y + yearThousands);
            }

            return yearResult.Map(y => HandleQuarterResult(seq, y));
        }
        else
        {
            throw new ArgumentException("Parser not appropriate for current sequence!");
        }
    }

    private Result<TimePeriod> HandleQuarterResult(SequenceBuilder seq, int year)
    {
        var stringPart = seq.Get(seq.LastIdx - 1).Str;
        var numPart = seq.Get(seq.LastIdx).Str;

        var strResult = Source.Of(stringPart).Ensure(s => s.Equals("q"), i => "Only 'q' supported");

        return numPart.ParseInt(i => i >= 1 && i <= 4)
                      .Combine(strResult, (a, b) => a)
                      .Map(q =>
                      {
                          var qStart = Time.QuarterStart(year, q);
                          var qEnd = qStart.Eoq();
                          return new TimePeriod(qStart, qEnd.Midnight());
                      });
    }
}