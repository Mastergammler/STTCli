public class Repl
{
    public const string INPUT_CHARS = ">> ";
    public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string SHORT_DATE = "dd MMM HH:mm";

    bool is_running = true;

    private Dictionary<string, ICommand> _commands;

    SttContext Db { get; }
    SttRepository Repo { get; }
    SttCache Cache { get; } = new();

    public Repl(SttContext db, SttRepository repo)
    {
        Db = db;
        Repo = repo;

        _commands = new()
        {
            ["stats"] = new DbStatsCmd(db),
            ["list"] = new ListCmd(db, Cache),
            ["add"] = new CreateItemCmd(repo),
            ["delete"] = new DeleteItemCmd(db),
            ["finish"] = new FinishCmd(db),
            ["exit"] = new QuitCmd(this),
        };
    }

    public void MainLoop()
    {
        Console.WriteLine("***  Welcome to the ScheduleTimerTask CLI  ***");

        while (is_running)
        {
            Console.Write(">> ");
            var input = Console.ReadLine();
            Eval(input);
        }
    }

    public void Eval(string input)
    {
        string[] cmd = ParseCmdInput(input);
        if (cmd.Length == 0) return;

        if (_commands.ContainsKey(cmd[0]))
        {
            _commands[cmd[0]].Execute(cmd.AsMemory(1));
        }
        else
        {
            Print($"Unknown command: '{cmd[0]}'");
        }
    }

    private class QuitCmd(Repl parent) : ICommand
    {
        public void Execute(Memory<string> args)
        {
            parent.Db.SaveChanges();
            parent.is_running = false;
        }
    }

    public string[] ParseCmdInput(string input)
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
                if (withinQuotes)
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

    // UTIL
    public static string DisplayTags(Tag[] tags, long bitSet)
    {
        var tagNames = tags.Where(t => (t.Bit & bitSet) > 0).Select(t => t.Name).Order();
        return string.Join(", ", tagNames);
    }
    public static void Print(string text)
    {
        string placeholder = " ";
        Console.WriteLine($"{placeholder.PadLeft(INPUT_CHARS.Length, ' ')}{text}");
    }
}