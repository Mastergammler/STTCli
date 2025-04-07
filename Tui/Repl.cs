public class Repl(SttContext db, SttRepository repo)
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
            case "stats": ListDbStats(); break;
            case "exit":
                db.SaveChanges();
                is_running = false; break;
            //TODO: add tags/items
            case "add": CreatItem(input); break;
            //TODO: list items/tags
            case "list": ShowItemList(input); break;
            default:
                Print($"Unknown command: '{cmd}'");
                break;
        }
    }

    public void ShowItemList(string input)
    {
        //TODO: parse tag filters & filter items before querying

        var tags = db.Tags.ToArray();

        AsciiTable table = new();
        table.AddColumns("ID", "Name", "Tags", "Created", "Finished");
        table.AddData(db.Items.Select(i => new object[]
        {
            i.Id,
            i.Name,
            DisplayTags(tags, i.Tags),
            i.Created.ToString(DATE_FORMAT),
            i.Finished
        }));
        table.Print(4);
    }

    private static string DisplayTags(Tag[] tags, long bitSet)
    {
        var tagNames = tags.Where(t => (t.Bit & bitSet) > 0).Select(t => t.Name);
        return string.Join(", ", tagNames);
    }

    public void CreatItem(string input)
    {
        string[] parts = ParseCmdInput(input);

        if (parts.Length < 2) Print("Usage: add <name> #<tag1> #<tag2> ...");

        string name = parts[1];
        var tags = parts.Where(p => p.StartsWith("#")).ToArray();
        var msg = repo.CreateItem(name, tags);

        Print(msg);
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

        //FIXME: this doesn't parse all things correctly
        if (lastIndex < input.Length) parts.Add(input[lastIndex..]);

        return parts.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
    }

    public void ListDbStats()
    {
        int itemCount = db.Items.Count();
        int tagCount = db.Tags.Count();

        Print($"The db holds:");
        Print($"- {itemCount} items");
        Print($"- {tagCount} tags");
    }

    private void Print(string text)
    {
        string placeholder = " ";
        Console.WriteLine($"{placeholder.PadLeft(INPUT_CHARS.Length, ' ')}{text}");
    }
}