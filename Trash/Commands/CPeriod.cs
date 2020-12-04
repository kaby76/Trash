namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Text.Json;

    class CPeriod
    {
        public void Help()
        {
            System.Console.WriteLine(@".
Print out the parse tree for the file at the top of stack.

Example:
    .
");
        }

        public void Execute(Repl repl, ReplParser.PeriodContext tree, bool piped)
        {
            if (repl.stack.Any())
            {
                var doc = repl.stack.Peek();
                var pr = ParsingResultsFactory.Create(doc);
                IParseTree pt = pr.ParseTree;
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = false;
                var tuple = new ParseInfo()
                {
                    Text = doc.Code,
                    FileName = doc.FullPath,
                    Stream = pr.TokStream,
                    Nodes = new IParseTree[] { pt },
                    Lexer = pr.Lexer,
                    Parser = pr.Parser
                };
                string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
                repl.input_output_stack.Push(js1);
            }
        }
    }
}