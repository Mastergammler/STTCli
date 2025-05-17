using static Parsing;

public class Repl
{
    public const string INPUT_CHARS = ">> ";
    public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string SHORT_DATE = "dd MMM HH:mm";
    public const int DEFAULT_INDENT = 4;

    bool is_running = true;

    private Dictionary<string, ICommand> _commands;

    public Repl(ICommandFactory factory)
    {
        _commands = new()
        {
            ["stats"] = factory.Create<DbStatsCmd>(),
            ["show"] = factory.Create<ShowListCmd>(),
            ["list"] = factory.Create<ListCmd>(),

            ["add"] = factory.Create<CreateItemCmd>(),
            ["new"] = factory.Create<NewFilterCmd>(),

            ["edit"] = factory.Create<EditCmd>(),
            ["delete"] = factory.Create<DeleteItemCmd>(),
            ["finish"] = factory.Create<FinishCmd>(),
            ["deadline"] = factory.Create<DeadlineCmd>(),

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
            Print($"Available options are:\n{"".PadLeft(DEFAULT_INDENT)}{string.Join($"\n{"".PadLeft(DEFAULT_INDENT)}", _commands.Keys)}");
        }
    }

    private class QuitCmd(Repl parent) : ICommand
    {
        public void Execute(Memory<string> args)
        {
            //FIXME: i might wanna have this, even thou i still save on every change atm
            //parent.Db.SaveChanges();
            parent.is_running = false;
        }
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
}