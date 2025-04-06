public class Repl(SttContext db)
{
    const string INPUT_CHARS = ">> ";
    const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

    const string ID_HEADER = "ID";
    const string NAME_HEADER = "Item Name";
    const string CREATED_HEADER = "Created";
    const string FINISHED_HEADER = "Finshed";
    const string FINISHED_PLACEHOLDER = " - ";

    bool is_running = true;

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
        string cmd = input.Split(' ').FirstOrDefault() ?? string.Empty;

        switch (cmd)
        {
            case "items": QueryItems(); break;
            case "exit":
                db.SaveChanges();
                is_running = false; break;
            case "add": CreatItem(input); break;
            case "list": ShowItemList(input); break;
            default:
                Print($"Unknown command: '{cmd}'");
                break;
        }
    }

    public void ShowItemList(string input)
    {
        //TODO: parse tag filters & filter items before querying
        AsciiTable table = new();
        table.AddColumns("ID", "Name", "Created", "Finished");
        table.AddData(db.Items.Select(i => new object[] { i.Id, i.Name, i.Created.ToString(DATE_FORMAT), i.Finished }));
        table.Print(4);
    }

    public void CreatItem(string input)
    {
        string[] parts = ParseCmdInput(input);

        if (parts.Length < 2) Print("Usage: add <name> #<tag1> #<tag2> ...");

        //TODO: handle tags

        var item = new ListItem
        {
            Name = parts[1],
            Created = DateTime.UtcNow,
        };
        db.Add(item);
        db.SaveChanges();

        Print($"Added item {item.Id}-{item.Created}.");
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

        return parts.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
    }

    public void QueryItems()
    {
        int count = db.Items.Count();
        Print($"The db holds {count} items");
    }

    private void Print(string text)
    {
        string placeholder = " ";
        Console.WriteLine($"{placeholder.PadLeft(INPUT_CHARS.Length, ' ')}{text}");
    }
}