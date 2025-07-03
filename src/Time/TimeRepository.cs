using Microsoft.EntityFrameworkCore;

public class TimeRepository(SttContext db)
{
    public TimeEntry CreateTimeEntry(ListItem item)
    {
        var activeEntryForItem = db.ActiveEntries.SingleOrDefault(e => e.Item == item);
        if (activeEntryForItem is null)
        {
            activeEntryForItem = new TimeEntry
            {
                Start = DateTime.UtcNow,
                Item = item
            };
            db.Add(activeEntryForItem);
        }

        return activeEntryForItem;
    }

    public IEnumerable<TimeEntry> FindActiveItems() => db.TimeEntries.Include(e => e.Item)
                                                                     .Where(e => e.End == null)
                                                                     .ToArray();

    public void Commit() => db.SaveChanges();
}