using static Repl;

public static class Validator
{
    public static bool ShowHelp(this Memory<string> args, string hint, int requiredArgs = 1)
    {
        if (args.Length < requiredArgs ||
            args.Length > 0 && (args.Span[0].Equals("help") ||
                                args.Span[0].Equals("-h") ||
                                args.Span[0].Equals("?")))
        {
            Print(hint);
            return true;
        }

        return false;
    }
}