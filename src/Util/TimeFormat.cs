public static class Format
{
    public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string NAMED_DATE = "dd MMM";
    public const string NAMED_DATE_TIME = "dd MMM HH:mm";
    public const string TIME_PORTION_ONLY = "HH:mm";
    public const string SHORT_DATE_DE = "dd.MM.yyyy";

    public static string Hours(this TimeSpan span)
    {
        if (span.Hours > 0)
        {
            string hours = ((int)span.TotalHours).ToString().PadLeft(2, ' ');
            return $"{hours}:{span.Minutes:D2} h";
        }
        else
        {
            return $"   {span.Minutes.ToString().PadLeft(2, '0')} m";
        }
    }

    public static string ShortDate(this DateTime date) => date.ToLocalTime().ToString(SHORT_DATE_DE);
    public static string NamedDate(this DateTime date) => date.ToLocalTime().ToString(NAMED_DATE);
    public static string NamedDateTime(this DateTime date) => date.ToLocalTime().ToString(NAMED_DATE_TIME);
    public static string Time(this DateTime date) => date.ToLocalTime().ToString(TIME_PORTION_ONLY);
}