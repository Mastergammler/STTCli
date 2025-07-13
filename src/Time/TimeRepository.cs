using Microsoft.EntityFrameworkCore;

public class TimeRepository(SttContext db)
{
    //FIXME: there could still be a running entry for a different item,
    // but you could create a new item, that would then run at the same time
    public TimeEntry CreateTimeEntry(ListItem item, DateTime? start = null, DateTime? end = null)
    {
        var activeEntryForItem = db.ActiveEntries.SingleOrDefault(e => e.Item == item);
        if (activeEntryForItem is null)
        {
            activeEntryForItem = new TimeEntry
            {
                Start = start ?? Time.Now(),
                Item = item,
                // end may be null
                End = end
            };
            db.Add(activeEntryForItem);
        }

        return activeEntryForItem;
    }

    public IEnumerable<TimeEntry> FindActiveItems() => db.TimeEntries.Include(e => e.Item)
                                                                     .Where(e => e.End == null)
                                                                     .ToArray();

    //TODO: TT - handle entries over night
    public IEnumerable<TimeEntry> FindItemsWithin(DateTime startTime, DateTime endTime, FilterOptions filter)
    {
        var query = db.TimeEntries.Include(e => e.Item)
                                  .Where(e => e.Start >= startTime && e.Start <= endTime);

        if (filter.Tags.Any())
        {
            query = filter.Tags.Select(ts => query.Where(q => (q.Item.Tags & ts.Tags) == ts.Included))
                               .Aggregate((a, b) => a.Concat(b));
        }

        return query.OrderBy(e => e.Start)
                    .ToArray();
    }

    public IEnumerable<TimeEntry> FindEntriesByParent(long parentItemId) => db.TimeEntries.Include(e => e.Item)
                                                                                          .Where(e => e.Item.ParentId == parentItemId)
                                                                                          .ToArray();

    //NOTE: because of microsofts akward interface design we can't return a IDictionary, becaues then the 
    // GetValueOrDefault method is not available ...
    public Dictionary<long, TimeSpan> AccumulationsFor(HashSet<long> parentIds)
    {
        return db.TimeEntries.Where(e => e.Item.ParentId != null && parentIds.Contains(e.Item.ParentId.Value))
                             .GroupBy(e => e.Item.ParentId.Value)
                             .ToArray()
                             .Select(g => new
                             {
                                 ProjectId = g.Key,
                                 ProjectTime = g.Total()
                             })
                             .ToDictionary(e => e.ProjectId, e => e.ProjectTime);
    }

    public (TimeEntry? pre, TimeEntry? suc) FindNeighbourEntries(DateTime time)
    {
        var sameDayEntries = db.TimeEntries.Where(e => e.End >= time.Date && e.Start < time.Date.Midnight());

        var closestBefore = sameDayEntries.Where(d => d.End < time).OrderByDescending(d => d.End).FirstOrDefault();
        var closestAfter = sameDayEntries.Where(d => d.Start > time).OrderBy(d => d.Start).FirstOrDefault();

        return (closestBefore, closestAfter);
    }

    public void Commit() => db.SaveChanges();
}