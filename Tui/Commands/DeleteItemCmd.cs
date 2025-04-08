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

        if (long.TryParse(args.Span[1], out long id))
        {
            //TODO: uses the same things as the list ones
            //-> so this should be unified
            switch (args.Span[0])
            {
                case "items": db.Remove(db.Items.FirstOrDefault(i => i.Id == id)); break;
                case "tags": db.Remove(db.Tags.FirstOrDefault(i => i.ID == id)); break;
                default: Print($"Unknown list type '{args.Span[0]}'"); break;
            }

            db.SaveChanges();
        }
        else
        {
            Print($"{args.Span[0]} is not a entity ID.");
        }
    }
}