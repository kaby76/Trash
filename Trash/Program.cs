namespace Trash
{
    class Program
    {
        static void Main(string[] args)
        {
            var repl = new Repl(args);
            repl.Execute();
        }
    }
}
