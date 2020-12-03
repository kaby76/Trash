namespace Trash.Commands
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Text.Json;

    class CDot
    {
        public void Help()
        {
            System.Console.WriteLine(@".
Print out the parse tree for the file at the top of stack.

Example:
    .
");
        }

        public void Execute(Repl repl, ReplParser.DotContext tree, bool piped)
        {
            if (repl.stack.Any())
            {
                var doc = repl.stack.Peek();
                var pr = ParsingResultsFactory.Create(doc);
                IParseTree pt = pr.ParseTree;
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = false;
                var tuple = new MyTuple<string, ITokenStream, IParseTree[], Lexer, Parser>()
                {
                    Item1 = doc.Code, Item2 = pr.TokStream, Item3 = new IParseTree[] {pt}, Item4 = pr.Lexer,
                    Item5 = pr.Parser
                };
                string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
                repl.input_output_stack.Push(js1);
            }
        }
    }
}
