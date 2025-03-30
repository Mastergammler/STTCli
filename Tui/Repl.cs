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

        int maxIdWith = db.Items.Max(i => i.Id).ToString().Length;
        int maxNamewith = db.Items.Max(i => i.Name.Length);
        int dateMax = DATE_FORMAT.Length;

        int idMax = Math.Max(maxIdWith, ID_HEADER.Length);
        int nameMax = Math.Max(maxNamewith, NAME_HEADER.Length);
        int createdMax = Math.Max(dateMax, CREATED_HEADER.Length);
        int finishedMax = Math.Max(dateMax, FINISHED_HEADER.Length);

        //TODO: add extra padding 1 char on each side
        //-> create Table component to handle this completely

        Print($"|{Padded(ID_HEADER, idMax)}|{Padded(NAME_HEADER, nameMax, false)}|{Padded(CREATED_HEADER, createdMax, false)}|{Padded(FINISHED_HEADER, finishedMax, false)}|");

        foreach (var item in db.Items)
        {
            Print($"|{Padded(item.Id.ToString(), idMax)}|{Padded(item.Name, nameMax, false)}|{Padded(item.Created.ToString(DATE_FORMAT), createdMax, false)}|{Padded(item.Finished?.ToString(DATE_FORMAT) ?? FINISHED_PLACEHOLDER, finishedMax, false)}|");
        }
    }

    private string Padded(string str, int maxWidth, bool padLeft = true)
    {
        if (padLeft) return str.PadLeft(maxWidth, ' ');
        else return str.PadRight(maxWidth, ' ');
    }

    public void CreatItem(string input)
    {
        string[] parts = input.Split(' ');

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