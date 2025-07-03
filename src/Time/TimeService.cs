public class TimeService(TimeRepository repo, SttContext db)
{
    //TODO: message that others have been stopped
    public void StartTracking(ListItem item)
    {
        var endTime = DateTime.UtcNow;
        //TODO: UGLY
        foreach (var entry in db.ActiveEntries.Where(e => e.Item != item))
        {
            entry.End = endTime;
        }

        repo.CreateTimeEntry(item);
        repo.Commit();
    }

    public IEnumerable<TimeEntry> StopTracking()
    {
        var entries = db.ActiveEntries.ToArray();
        var endTime = DateTime.UtcNow;
        foreach (var entry in entries)
        {
            entry.End = endTime;
        }
        repo.Commit();

        return entries;
    }
}