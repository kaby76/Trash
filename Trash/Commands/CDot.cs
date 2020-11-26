namespace Trash.Commands
{
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
                var pt = pr.ParseTree;
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.Impl2.ParseTreeConverter(pr.Code, pr.TokStream, pr.Lexer, pr.Parser));
                serializeOptions.WriteIndented = false;
                string js1 = JsonSerializer.Serialize(pt, serializeOptions);
                repl.tree_stack.Push(new System.Tuple<IParseTree[], Parser, Workspaces.Document, string>(null, null, null, js1));
            }
        }
    }
}
