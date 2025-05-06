/// <summary>
///  Entity to save list filters
///  Basically just saves the filter expression that also get's parsed usually
/// </summary>
public class Filter
{
    public long Id { set; get; }
    public string Name { set; get; }
    public string Expression { set; get; }
}