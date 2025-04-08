public interface ICommand
{
    void Execute(Memory<string> args);
}