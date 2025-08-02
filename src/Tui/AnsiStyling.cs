

/// <summary>
///     0 -  15 Default colors
///   232 - 255 Grayscales
/// </summary
public enum Ansi256Color
{
    DARK_BLUE = 17,
    DARK_GRAY = 59,
    DARK_GREEN = 22,
    DARK_RED = 52,
    DARK_YELLOW = 58,
    ORANGE = 130,
    YELLOW = 178,
    LIGHT_GRAY = 188,
    // grayscale color
    GRAY = 245,
}

public static class AnsiStyling
{
    public static string ANSI_START = "\u001b[";
    public static string ANSI_END = "m";
    public static string ANSI_RESET = "\u001b[0m";

    public static string SET_TEXT = "38;";
    public static string SET_BG = "48;";
    public static string COLOR_MODE_256 = "5;";

    /// <summary>
    ///     https://www.hackitu.de/termcolor256/
    /// </summary>
    public static string Create(Ansi256Color? textColorId, Ansi256Color? bgColorId)
    {
        string textStyling = textColorId != null ? $"{SET_TEXT}{COLOR_MODE_256}{(byte)textColorId}" : string.Empty;
        string bgStyling = bgColorId != null ? $"{SET_BG}{COLOR_MODE_256}{(byte)bgColorId}" : string.Empty;

        if (bgStyling.Length > 0 && textStyling.Length > 0) textStyling += ";";

        return $"{ANSI_START}{textStyling}{bgStyling}{ANSI_END}";
    }

    public static string Text(Ansi256Color textColor) => Create(textColor, null);
    public static string Bg(Ansi256Color bgColor) => Create(null, bgColor);


    public static string Reset() => ANSI_RESET;

    public static string WithBg(this string text, Ansi256Color color)
    {
        return $"{Bg(color)} {text} {Reset()}";
    }

    public static string WithColor(this string text, Ansi256Color color)
    {
        return $"{Text(color)}{text}{Reset()}";
    }
}