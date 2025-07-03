public class TimeEntry
{
    public long Id { set; get; }
    public DateTime Start { set; get; }
    public DateTime? End { set; get; }

    public long ItemId { set; get; }
    public ListItem Item { set; get; }
}