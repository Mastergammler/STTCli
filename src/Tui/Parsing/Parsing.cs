using static Symbols;

public record TimePeriod(DateTime start, DateTime end);

public static class Parsing
{
    public static readonly DateTime TIME_TEMPLATE = new DateTime(1, 1, 1, 23, 0, 0);

    public static Result<int> PositiveResult(this string numStr)
    {
        if (int.TryParse(numStr, out int num))
        {
            return num >= 0 ? new Success<int>(num) : new Failure<int>(REQ_POS_NUM_ERR.With(num));
        }

        return new Failure<int>(INVALID_NUM_ERR.With(numStr));
    }

    public static Result<int> ParseInt(this string numStr, Func<int, bool> validator)
    {
        if (int.TryParse(numStr, out int num))
        {
            return validator(num) ? new Success<int>(num) : new Failure<int>(NUM_VALIDATION_ERR.With(num));
        }
        return new Failure<int>(INVALID_NUM_ERR.With(numStr));
    }

    public static Result<DateTime> DateResult(this string dateStr)
    {
        if (DateTime.TryParse(dateStr, out DateTime parsed))
        {
            return Result.Value(parsed);
        }
        return Result.Fail(parsed, INVALID_DATE_ERR.With(dateStr));
    }

    public static Result<TimePeriod> ParseTimespan(string timespanExpr, DateTime? dateToday = null)
    {
        //TODO: Time - is this the best way to handle date usage?
        // -> Also possible error for UTC edge case? (I guess not that relevant)
        var today = dateToday?.Date ?? DateTime.UtcNow.Date;

        var seq = new SequenceBuilder(timespanExpr);
        ITimespanParsingStrategy strategy = seq.Pattern switch
        {
            ErrorResultParser.Pattern => new ErrorResultParser(),
            TimespanKeywordParser.Pattern => new TimespanKeywordParser(today),
            DayspanParser.Pattern => new DayspanParser(today),
            IsoDateParser.Pattern => new IsoDateParser(today),
            YearDateParser.Pattern => new YearDateParser(today),
            SingleNumberParser.Pattern => new SingleNumberParser(today),
            QuarterParser.PatternShort or
            QuarterParser.PatternLong => new QuarterParser(today),
            PrevTimespanParser.Pattern => new PrevTimespanParser(today),
            _ => new UnknownPatternParser()
        };

        return strategy.Parse(seq);
    }

    public static DateTime? ParseDate(string input)
    {
        DateTime? dateLocalTime = null;
        var inputInvariant = input.ToLower();

        if (DateTime.TryParse(input, out DateTime result)) dateLocalTime = result;
        else if (inputInvariant.Equals("eod") ||
                 inputInvariant.Equals("today") ||
                 inputInvariant.Equals("t")) dateLocalTime = DateTime.Today.AddTime(TIME_TEMPLATE);
        else if (inputInvariant.Equals("eow"))
        {
            DayOfWeek dow = DateTime.Today.DayOfWeek;
            // if today is sunday, it's end of next week
            var offset = 7 - (int)dow;
            dateLocalTime = DateTime.Today.AddDays(offset).AddTime(TIME_TEMPLATE);
        }
        else if (inputInvariant.Equals("eom"))
        {
            dateLocalTime = DateTime.Today.AddMonths(1).AddDays(-DateTime.Today.Day).AddTime(TIME_TEMPLATE);
        }
        //NOTE: these might intefere with eow, eod, eoy etc!!!!
        else if (inputInvariant.EndsWith("d") ||
                 inputInvariant.EndsWith("w"))
        {
            bool isWeek = inputInvariant[^1..].Equals("w");
            var numberPortion = inputInvariant[0..^1];

            if (int.TryParse(numberPortion, out int number))
            {
                if (isWeek) number = number * 7;
                dateLocalTime = DateTime.Today.AddDays(number).AddTime(TIME_TEMPLATE);
            }
            //TODO: error handling?
        }
        else if (inputInvariant.Length == 4)
        {
            // assuming that it's all values
            // month's starting with leading zeros
            // using ISO formatting
            var monthStr = inputInvariant[0..2];
            var dayStr = inputInvariant[2..];

            //TODO: this can still crash, if it's a invalid month value, i should maybe validate this
            if (int.TryParse(dayStr, out int day) && int.TryParse(monthStr, out int month))
            {
                dateLocalTime = (new DateTime(DateTime.Today.Year, month, day)).AddTime(TIME_TEMPLATE);
            }
            //TODO: error handling etc?
        }

        return dateLocalTime?.ToUniversalTime();
    }

    public static DateTime AddTime(this DateTime dateTime, DateTime template)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, template.Hour, template.Minute, template.Second);
    }

    public static string[] ParseCmdInput(string input)
    {
        List<string> parts = [];

        bool withinQuotes = false;
        int lastIndex = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == ' ' && !withinQuotes)
            {
                parts.Add(input[lastIndex..i]);
                // we want to be exclusive
                lastIndex = i + 1;
            }
            else if (input[i] == '"')
            {
                if (withinQuotes || (i > 0 && input[i - 1] != ' '))
                {
                    parts.Add(input[lastIndex..i]);
                }

                lastIndex = i + 1;
                withinQuotes = !withinQuotes;
            }
        }

        if (lastIndex < input.Length) parts.Add(input[lastIndex..]);

        return parts.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
    }

    //TODO: refactor to some more generalized parser strategies
    private static Func<string, bool> IsTag = s => s.StartsWith("#") || s.StartsWith("~#");

    public static FilterOptions ParseOptions(Tag[] tags, Memory<string> args)
    {
        var opt = new FilterOptions();
        var dict = tags.ToDictionary(t => t.Name);
        foreach (string arg in args.Span)
        {
            if (IsTag(arg))
            {
                var tagSet = ParseTags(arg, dict);
                opt.Tags.Add(tagSet);
            }

            // TODO: handle other cases
        }

        return opt;
    }

    private static TagSet ParseTags(string tagString, IDictionary<string, Tag> tags)
    {
        TagSet ts = new();

        string[] ands = tagString.Split('&');

        foreach (string op in ands)
        {
            string tagName = op;
            bool isComplement = false;
            if (op.StartsWith('~'))
            {
                tagName = op[1..];
                isComplement = true;
            }

            if (tags.ContainsKey(tagName))
            {
                long tagValue = tags[tagName].Bit;

                if (isComplement)
                {
                    if (ts.Excluded == 0) ts.Excluded = tagValue;
                    ts.Excluded |= tagValue; // punch out new zeros
                }
                else
                {
                    if (ts.Included == 0) ts.Included = tagValue;
                    else ts.Included |= tagValue; // add new 1s together
                }
            }

            //NOTE: for the purpose of parsing we just ignore invalid values
            // - else we create a crash for every invalid user input?
            // -> Or would this be the correct handling? Because i would want to know that something is wrong?
        }

        return ts;
    }
}

public class FilterOptions
{
    public List<TagSet> Tags { get; } = [];
}

public class TagSet
{
    public long Included { set; get; } = 0;
    public long Excluded { set; get; } = 0;
    public long Tags => Included | Excluded;
}