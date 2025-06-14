using static Parsing;

public class Repl
{
    public const string INPUT_CHARS = ">> ";
    public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string SHORT_DATE = "dd MMM HH:mm";
    public const int DEFAULT_INDENT = 4;

    bool is_running = true;

    private string input_prefix = INPUT_CHARS;
    private ICommand _evalCommand;
    private ICommand _defaultCommand;
    private ICommandFactory _factory;

    public Repl(ICommandFactory factory)
    {
        _factory = factory;
        _factory.Init(this);
        _defaultCommand = _factory.Create<ReplCommands>();
        _evalCommand = _defaultCommand;
    }

    public void MainLoop()
    {
        Console.WriteLine("***  Welcome to the ScheduleTimerTask CLI  ***");

        while (is_running)
        {
            Console.Write(input_prefix);
            var input = Console.ReadLine();
            Eval(input);
        }
    }

    public void SetInputMode<T>(string inputName) where T : ICommand
    {
        input_prefix = $"{inputName} >> ";
        _evalCommand = _factory.Create<T>();
    }

    public void ResetInputMode()
    {
        input_prefix = INPUT_CHARS;
        _evalCommand = _defaultCommand;
    }

    public void Eval(string input)
    {
        string[] cmd = ParseCmdInput(input);
        if (cmd.Length == 0) return;

        _evalCommand.Execute(cmd.AsMemory());
    }

    public void Exit()
    {
        is_running = false;
    }

    // UTIL
    public static string DisplayTags(Tag[] tags, long bitSet)
    {
        var tagNames = tags.Where(t => (t.Bit & bitSet) > 0).Select(t => t.Name).Order();
        return string.Join(" ", tagNames);
    }

    public static void Print(string text)
    {
        string placeholder = " ";
        Console.WriteLine($"{placeholder.PadLeft(INPUT_CHARS.Length, ' ')}{text}");
    }

    public static bool ValidateSingleMatch<T>(string searchStr, IEnumerable<T> items, Func<T, string> nameSelector)
    {
        bool singleMatch = true;

        if (items.Count() == 0)
        {
            Print($"No match found for name '{searchStr}'");
            singleMatch = false;
        }
        else if (items.Count() > 1)
        {
            string matches = string.Join("\n - ", items.Select(nameSelector));
            Print($"Found multiple matches:\n - {matches}");
            singleMatch = false;
        }

        return singleMatch;
    }
}