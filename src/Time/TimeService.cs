public enum FillType { DEFAULT = 0, START = 1, END = 2, MIDDLE = 3 }
public class TimeService(TimeRepository repo, SttContext db)
{
    //TODO: message that others have been stopped
    public void StartTracking(ListItem item)
    {
        var endTime = Time.Now();
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
        var endTime = Time.Now();
        foreach (var entry in entries)
        {
            entry.End = endTime;
        }
        repo.Commit();

        return entries;
    }

    //FIXME: there is no check for overriding an existing time entry
    // -> Because the neighbour detection doesn't know about it
    public Result<TimeEntry> FillItem(ListItem item, DateTime itemTime, FillType fill = FillType.DEFAULT)
    {
        var refDate = itemTime.Date;
        var neigh = repo.FindNeighbourEntries(itemTime);

        DateTime startTime;
        DateTime endTime;

        //TODO: refactor validation handling
        var neighResult = Source.Of(neigh)
                                .Ensure(n => n.pre != null || n.suc != null, "Cannot fill an empty day!")
                                .EnsureIf(n => n.pre != null, n => n.pre.End != null, "Predecessor must be finished!");

        if (neighResult is Failure<(TimeEntry?, TimeEntry?)> f) return new Failure<TimeEntry>(f.Error);

        if (neigh.pre is null && neigh.suc != null)
        {
            if (fill == FillType.END || fill == FillType.MIDDLE)
                return Result.Fail<TimeEntry>($"Fill type {fill} is invalid when predecessor is null!");

            fill = FillType.START;
        }
        else if (neigh.pre != null && neigh.suc is null)
        {
            if (fill == FillType.START || fill == FillType.MIDDLE)
                return Result.Fail<TimeEntry>($"Fill type {fill} is invalid when successor is null!");
            fill = FillType.END;
        }

        switch (fill)
        {
            case FillType.DEFAULT:
            case FillType.END:
                startTime = neigh.pre.End.Value;
                endTime = itemTime;
                break;
            case FillType.START:
                startTime = itemTime;
                endTime = neigh.suc.Start;
                break;
            case FillType.MIDDLE:
                startTime = neigh.pre.End.Value;
                endTime = neigh.suc.Start;
                break;
            default: throw new NotImplementedException($"Handling for value {fill} not implemented!");
        }

        var newEntity = repo.CreateTimeEntry(item, startTime, endTime);
        repo.Commit();

        return Source.Of(newEntity);
    }
}