namespace Trash.Commands
{
    class CAnalyze
    {
        public void Help()
        {
            System.Console.WriteLine(@"analyze
Trash can perform an analysis of a grammar. The analysis includes a count of symbol
type, cycles, and unused symbols.

Example:
    analyze
");
        }

        public void Execute(Repl repl, ReplParser.AnalyzeContext tree)
        {
            var doc = repl.stack.Peek();
            repl._docs.AnalyzeDoc(doc);
        }
    }
}
