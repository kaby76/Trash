namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using AntlrJson;
    using System.Text.Json;
    using System;
    using Algorithms;

    class CNewDot
    {
        public void Help()
        {
            System.Console.WriteLine(@".
Print out the parse tree for the file at the top of stack.

Example:
    .
");
        }

        public void Execute(Repl repl, ReplParser.NewdotContext tree, bool piped)
        {
            if (repl.stack.Any())
            {
                var doc = repl.stack.Peek();
                var pr = ParsingResultsFactory.Create(doc);
                var pt = pr.ParseTree;
                var serializeOptions = new JsonSerializerOptions();
                if (tree.arg()?.GetText() == "2")
                {
                    serializeOptions.Converters.Add(new AntlrJson.Impl2.ParseTreeConverter(pr.Code, pr.TokStream, pr.Lexer, pr.Parser));
                    serializeOptions.WriteIndented = true;
                    string js1 = JsonSerializer.Serialize(pt, serializeOptions);
                    repl.tree_stack.Push(new System.Tuple<IParseTree[], Parser, Workspaces.Document, string>(null, null, null, js1));
                }
                else
                {
                    serializeOptions.Converters.Add(new AntlrJson.Impl1.ParseTreeConverter(pr.Parser));
                    serializeOptions.Converters.Add(new AntlrJson.Impl1.TokenConverter());
                    serializeOptions.Converters.Add(new AntlrJson.Impl1.TokenStreamConverter());
                    serializeOptions.WriteIndented = true;
                    MyTuple<IParseTree, ITokenStream> tuple = new MyTuple<IParseTree, ITokenStream>(pt, pr.TokStream);
                    string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
                    repl.tree_stack.Push(new System.Tuple<IParseTree[], Parser, Workspaces.Document, string>(null, null, null, js1));
                }
            }
        }
    }
}
