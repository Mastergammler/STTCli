using Microsoft.EntityFrameworkCore;

using static Repl;

public class DeleteItemCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 2)
        {
            Print("Requires at least 2 parameres: <type> <id>");
            return;
        }

        string type = args.Span[0];
        string idStr = args.Span[1];

        if (long.TryParse(idStr, out long id))
        {
            //TODO: uses the same things as the list ones
            //-> so this should be unified
            //TODO: this is a visitor right?, For selection which items to use etc
            // -> the visitor would be responsible for this list handling / selecting the entities everywhere
            // -> And then we would have something for each CRUD thingy?
            switch (type)
            {
                case "item": db.Remove(db.Items.FirstOrDefault(i => i.Id == id)); break;
                case "tag": db.Remove(db.Tags.FirstOrDefault(i => i.ID == id)); break;
                case "filter": db.Remove(db.Filters.FirstOrDefault(i => i.Id == id)); break;
                case "entry": db.Remove(db.TimeEntries.FirstOrDefault(i => i.Id == id)); break;
                default: Print($"Unknown list type '{type}'"); break;
            }

            db.SaveChanges();
        }
        // range delete
        else if (idStr.Contains("-"))
        {

            string[] parts = idStr.Split('-');
            if (!long.TryParse(parts[0], out long startId))
            {
                Print($"Unable to parse {parts[0]} to a number!");
                return;
            }
            if (!long.TryParse(parts[1], out long endId))
            {
                Print($"Unable to parse {parts[1]} to a number!");
                return;
            }

            switch (type)
            {
                case "items":
                    db.Items.Where(e => e.Id >= startId && e.Id < endId).ExecuteDelete();
                    break;
                case "tags":
                    db.Tags.Where(e => e.ID >= startId && e.ID < endId).ExecuteDelete();
                    break;
                default: Print($"Unknown list type '{type}'"); break;
            }
            db.SaveChanges();
        }
        else
        {
            Print($"{type} is not a type of entity.");
        }

    }
}