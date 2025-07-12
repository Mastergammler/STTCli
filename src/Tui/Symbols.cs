public static class Symbols
{
    public static readonly string NL = Environment.NewLine;

    public const string S_ID = "$";
    public const string S_NAME = ">>";
    public const string S_TAG = "#";
    public const string S_REM_TAG = "-#";

    //  Arguments
    public const string BULK_ARG = "-b";
    public const string FINISHED_ARG = "-f";
    public const string OVERRIDE_ARG = "-force";
    public const string SILENT_ARG = "-silent";
    public const string NO_ARG = "-n";
    public const string YES_ARG = "-y";
    public const string START_ARG = "-s";
    public const string END_ARG = "-e";
    public const string SURROUND_ARG = "-sur";

    // Messages
    public const string NO_ITEMS_MSG = "No items found for search: '{0}'";
    public const string NO_BULK_MSG = $"{BULK_ARG} not specified, but found multiple entries!{{0}}";
    public const string INVALID_DATE_ERR = "Unable to interpret '{0}' as a valid date";
    public const string INVALID_NUM_ERR = "'{0}' is not a valid number!";
    public const string REQ_POS_NUM_ERR = "Number must be positive, but was '{0}'";
    public const string NUM_VALIDATION_ERR = "Number '{0}' didn't pass the required validation";
    public const string NUM_TOO_SMALL_ERR = "Number must be bigger than '{0}'";
    public const string NUM_INVALID_HOUR = "'{0}' is not a valid hour value";
    public const string NUM_INVALID_MIN = "'{0}' is not a valid minute value";
    public const string NUM_INVALID_SEC = "'{0}' is not a valid second value";
    public const string MULTI_ARG_ERR = "Argument '{0}' was specified multiple times. (Not supported)";
    public const string TT_ONLY_FOR_TASKS_ERR = "Time tracking is only allowed for Tasks! (Level was {0})";

    // Keywords
    public const string KEYWORD_THIS = "this";

    public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string SHORT_DATE = "dd MMM";
    public const string SHORT_DATE_TIME = "dd MMM HH:mm";
    public const string TIME_PORTION_ONLY = "HH:mm";

    // UNICODE
    public const string UNI_BULLET = "\u2022";
    public const string UNI_ARRHEAD_R = "\u27A4";
    //TODO: adjust indent dynamically
    public const string UL = $"\n    {UNI_BULLET} ";


    // Converters
    public static readonly Func<DateTime?, bool, string?> DEADLINE_FORMATTER = (d, noTime) =>
    {
        string? formatted = null;
        if (d is not null)
        {
            var dateStr = d.Value.ToLocalTime().ToString(noTime ? SHORT_DATE : SHORT_DATE_TIME);
            var timeTillCompletion = d.Value - DateTime.UtcNow;
            var days = (int)timeTillCompletion.TotalDays;

            if (days < 0) formatted = dateStr;
            else formatted = $"{days}d {UNI_ARRHEAD_R} {dateStr}";
        }

        return formatted;
    };
    public static readonly Func<DateTime?, string?> DEADLINE_FORMAT_NOTIME = d => DEADLINE_FORMATTER(d, true);
    public static readonly Func<DateTime?, string?> DEADLINE_FORMAT = d => DEADLINE_FORMATTER(d, false);
}