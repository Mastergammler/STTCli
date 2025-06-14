using static Repl;

public class ProjectContextCmd(SttContext db, UiContext state, Repl repl) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp("Usage: project <project name>")) return;

        string projectName = args.Span[0];

        var entities = db.Projects.Where(p => p.Name.ToLower().Contains(projectName.ToLower()));
        if (ValidateSingleMatch(projectName, entities, e => e.Name))
        {
            repl.SetInputMode<ProjectCommands>(entities.Single().Name);
            state.CurrentProject = entities.Single();
        }
    }
}