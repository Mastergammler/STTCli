using static Symbols;
public record TimePeriod(DateTime Start, DateTime End);

public static class Parsing
{
    public static readonly DateTime TIME_TEMPLATE_LOCAL = new DateTime(1, 1, 1, 23, 0, 0);

    /// <summary>
    ///  Tries to parse the number and evaluates that it is not below 0
    ///  0 is seen as a valid result as well!
    /// </summary
    public static Result<int> PositiveResult(this string numStr)
    {
        if (int.TryParse(numStr, out int num))
        {
            return num >= 0 ? new Success<int>(num) : new Failure<int>(REQ_POS_NUM_ERR.With(num));
        }

        return new Failure<int>(INVALID_NUM_ERR.With(numStr));
    }

    public static Result<int> ParseInt(this string numStr, Func<int, bool> validator, string? msg = null)
    {
        if (int.TryParse(numStr, out int num))
        {
            return validator(num) ? new Success<int>(num)
                                  : new Failure<int>(msg?.With(num) ?? NUM_VALIDATION_ERR.With(num));
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

    public static Result<DateTime> DateResult(this Result<string> result)
    {
        return result switch
        {
            Success<string> s => s.value.DateResult(),
            Failure<string> f => new Failure<DateTime>(f.Error),
            _ => throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}")
        };
    }

    public static Result<TimePeriod> ParseTimespan(string timespanExpr, DateTime? dateToday = null)
    {
        //TODO: Time - is this the best way to handle date usage?
        // -> Also possible error for UTC edge case? (I guess not that relevant)
        var today = dateToday?.Date ?? Time.Today();

        var seq = new SequenceBuilder(timespanExpr);
        ISequenceParsingStrategy<TimePeriod> strategy = seq.Pattern switch
        {
            (int)SequencePatterns.ERR => new ErrorResultParser<TimePeriod>(),
            (int)SequencePatterns.L => new TimespanKeywordParser(today),
            (int)SequencePatterns.NL => new DayspanParser(today),
            (int)SequencePatterns.N_N_N => new IsoDateParser(today),
            (int)SequencePatterns.N_N => new YearDateParser(today),
            (int)SequencePatterns.N => new SingleNumberParser(today),
            (int)SequencePatterns.LN => new QuarterParser(today),
            (int)SequencePatterns.N_LN => new QuarterParser(today),
            (int)SequencePatterns.L_N => new PrevTimespanParser(today),
            (int)SequencePatterns.DL => new WeekdayParser(today),
            (int)SequencePatterns.NDL => new WeekdayParser(today),
            (int)SequencePatterns._DL => new WeekdayParser(today),
            (int)SequencePatterns._NDL => new WeekdayParser(today),
            _ => new UnknownPatternParser<TimePeriod>()
        };

        return strategy.Parse(seq);
    }

    public static Result<DateTime> ParseDate(string dateExpression, DateTime? dateToday = null)
    {
        var today = dateToday?.Date ?? Time.Today();

        var seq = new SequenceBuilder(dateExpression);
        ISequenceParsingStrategy<DateTime> parser = seq.Pattern switch
        {
            (int)SequencePatterns.ERR => new ErrorResultParser<DateTime>(),
            (int)SequencePatterns.N_N_N => new IsoDateParser(today),
            (int)SequencePatterns.N => new SingleNumberParser(today),
            (int)SequencePatterns._N => new TimeParser(today),
            (int)SequencePatterns.L => new DateKeywordParser(today),
            (int)SequencePatterns.N_N => new DatedTimeParser(today),
            (int)SequencePatterns.DL => new WeekdayParser(today),
            (int)SequencePatterns.NDL => new WeekdayParser(today),
            (int)SequencePatterns._DL => new WeekdayParser(today),
            (int)SequencePatterns._NDL => new WeekdayParser(today),
            (int)SequencePatterns.DL_N => new WeekdayTimeDelegate(today),
            (int)SequencePatterns.NDL_N => new WeekdayTimeDelegate(today),
            (int)SequencePatterns._DL_N => new WeekdayTimeDelegate(today),
            (int)SequencePatterns._NDL_N => new WeekdayTimeDelegate(today),
            _ => new UnknownPatternParser<DateTime>()
        };

        return parser.Parse(seq);
    }

    public static Result<DateTime> ParseTime(string dateExpression, DateTime? dateToday = null)
    {
        var today = dateToday?.Date ?? Time.Today();

        var seq = new SequenceBuilder(dateExpression);
        ISequenceParsingStrategy<DateTime> parser = seq.Pattern switch
        {
            (int)SequencePatterns.ERR => new ErrorResultParser<DateTime>(),
            (int)SequencePatterns._N => new TimeParser(today),
            (int)SequencePatterns.N => new TimeParser(today),
            (int)SequencePatterns.N_N => new DatedTimeParser(today),
            (int)SequencePatterns.DL_N => new WeekdayTimeDelegate(today),
            (int)SequencePatterns.NDL_N => new WeekdayTimeDelegate(today),
            (int)SequencePatterns._DL_N => new WeekdayTimeDelegate(today),
            (int)SequencePatterns._NDL_N => new WeekdayTimeDelegate(today),
            _ => new UnknownPatternParser<DateTime>()
        };

        return parser.Parse(seq);
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

    public static FilterOptions ParseFiltering(Tag[] tags, Memory<string> args)
    {
        var opt = new FilterOptions();
        var tagsByName = tags.ToDictionary(t => t.Name);
        foreach (string arg in args.Span)
        {
            if (IsTag(arg))
            {
                var tagSet = ParseTags(arg, tagsByName);
                opt.Tags.Add(tagSet);
            }
            else if (arg.Equals(FINISHED_ARG))
            {
                opt.FinishedOnly = true;
            }
            else if (arg.Equals(ALL_ARG))
            {
                opt.IncludeAll = true;
            }
            else if (arg.StartsWith(S_TOP))
            {
                Parsing.PositiveResult(arg[1..]).Execute(i => opt.Limit = i);
            }

            // TODO: handle other cases
        }

        return opt;
    }

    public static TagSet ParseTags(string tagExpression, IDictionary<string, Tag> tags)
    {
        TagSet ts = new();
        ts.Name = tagExpression;
        ts.OrExpr = tagExpression.Contains("|");
        ts.AndExpr = tagExpression.Contains("&");

        // when none is given, means a single tag, so we hanlde it like a AND expression
        if (ts.AndExpr == false && ts.OrExpr == false) ts.AndExpr = true;

        // NOTE: Limitation: currently we do not handle every single expression case
        // if & and | expressions are mixed, we use the ExlusionaryUnion approach 
        // to determine the tags, but this means, we're ignoring here what was 
        // an OR-input and what was a AND-input
        string[] tagExprs = tagExpression.Split(['&', '|']);

        foreach (string op in tagExprs)
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
    public bool FinishedOnly { get; set; } = false;
    public bool IncludeAll { get; set; } = false;
    public int Level { get; set; } = 0;

    public int Limit { get; set; } = 0;
    public bool TopList => Limit > 0;
}

public class TagSet
{
    public string Name { set; get; }
    public bool AndExpr { set; get; }
    public bool OrExpr { set; get; }
    public bool ExUnionExpr => AndExpr && OrExpr;

    public long Included { set; get; } = 0;
    public long Excluded { set; get; } = 0;
    public long Tags => Included | Excluded;
}