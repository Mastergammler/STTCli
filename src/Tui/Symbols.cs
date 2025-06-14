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

    // Messages
    public const string NO_ITEMS_MSG = "No items found for search: '{0}'";
    public const string NO_BULK_MSG = $"{BULK_ARG} not specified, but found multiple entries!{{0}}";
    public const string INVALID_DATE_ERR = "Unable to interpret '{0}' as a valid date";
    public const string INVALID_NUM_ERR = "'{0}' is not a valid number!";
    public const string MULTI_ARG_ERR = "Argument '{0}' was specified multiple times. (Not supported)";

    // Keywords
    public const string KEYWORD_THIS = "this";
}