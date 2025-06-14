public class UiContext
{
    public bool WithinProjectContext => CurrentProject is not null;
    public ListItem? CurrentProject { set; get; }

    public void Reset()
    {
        CurrentProject = null;
    }
}