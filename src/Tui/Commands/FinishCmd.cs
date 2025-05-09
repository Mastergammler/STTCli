using static Repl;

public class FinishCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length == 0)
        {
            Print("<item filter> is required");
            return;
        }

        if (long.TryParse(args.Span[0], out long id))
        {
            var item = db.Items.SingleOrDefault(i => i.Id == id);
            if (item is not null)
            {
                if (item.IsFinished)
                {
                    Print($"Item {item.Id} is already finished!");
                }
                else
                {
                    item.Finished = DateTime.UtcNow;
                }
            }
            else
            {
                Print($"No item with id {id} found!");
            }
        }
        else
        {
            string searchString = args.Span[0];
            var foundItems = db.Items.Where(i => i.Name.ToLower().Contains(searchString.ToLower()))
                                     .AsEnumerable()
                                     .Where(i => i.IsFinished == false)
                                     .ToArray();

            foreach (var item in foundItems)
            {
                item.Finished = DateTime.UtcNow;
            }

            Print($"Updated {foundItems.Count()} items ({string.Join(",", foundItems.Select(f => f.Id))})");
        }

        db.SaveChanges();
    }
}