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

    // Messages
    public const string NO_ITEMS_MSG = "No items found for search: '{0}'";
    public const string NO_BULK_MSG = $"{BULK_ARG} not specified, but found multiple entries!{{0}}";
    public const string INVALID_DATE_ERR = "Unable to interpret '{0}' as a valid date";
    public const string INVALID_NUM_ERR = "'{0}' is not a valid number!";
    public const string REQ_POS_NUM_ERR = "Number must be positive, but was '{0}'";
    public const string NUM_VALIDATION_ERR = "Number '{0}' didn't pass the required validation";
    public const string NUM_TOO_SMALL_ERR = "Number must be bigger than '{0}'";
    public const string MULTI_ARG_ERR = "Argument '{0}' was specified multiple times. (Not supported)";
    public const string TT_ONLY_FOR_TASKS_ERR = "Time tracking is only allowed for Tasks! (Level was {0})";

    // Keywords
    public const string KEYWORD_THIS = "this";

    public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string SHORT_DATE = "dd MMM HH:mm";
    public const string TIME_PORTION_ONLY = "HH:mm";

    // UNICODE
    public const string UNI_BULLET = "\u2022";
    //TODO: adjust indent dynamically
    public const string UL = $"\n    {UNI_BULLET} ";
}