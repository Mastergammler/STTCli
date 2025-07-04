using System.ComponentModel.DataAnnotations.Schema;

public class TimeEntry
{
    public long Id { set; get; }
    public DateTime Start { set; get; }
    public DateTime? End { set; get; }

    public long ItemId { set; get; }
    public ListItem Item { set; get; }

    [NotMapped]
    public TimeSpan Duration => (End ?? DateTime.UtcNow) - Start;

    public override string ToString()
    {
        var timespan = (End ?? DateTime.UtcNow) - Start;
        return $"{Start.ToLocalTime().ToString(Symbols.TIME_PORTION_ONLY)}  {Item.Name} ({timespan.Format().Trim()})";
    }
}