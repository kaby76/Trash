namespace Trash.Commands
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;
    using System.Text.Json;

    class CXGrep
    {
        public void Help()
        {
            System.Console.WriteLine(@"xgrep <string>
Find all sub-trees in the parsed file at the top of stack using the given XPath expression string.

Example:
    xgrep ""//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']""
");
        }

        public void Execute(Repl repl, ReplParser.XgrepContext tree, bool piped)
        {
            var expr = repl.GetArg(tree.arg());
            IParseTree[] atrees;
            Parser parser;
            Lexer lexer;
            Workspaces.Document doc;
            string code;
            CommonTokenStream tokstream;
            if (piped)
            {
                var lines = repl.input_output_stack.Pop();
                doc = repl.stack.Peek();
                var pr = ParsingResultsFactory.Create(doc);
                lexer = pr.Lexer;
                parser = pr.Parser;
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = false;
                var nodes = JsonSerializer.Deserialize<MyTuple<string, string, Parser, Lexer, ITokenStream, IParseTree[]>>(lines, serializeOptions);
                atrees = null;// nodes;
                parser = null;//pair.Item2;
            }
            else
            {
                doc = repl.stack.Peek();
                var pr = ParsingResultsFactory.Create(doc);
                parser = pr.Parser;
                tokstream = pr.TokStream;
                IParseTree atree = pr.ParseTree;
                atrees = new IParseTree[] { atree };
            }
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            IParseTree root = atrees.First().Root();
            var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser))
            {
                var l = atrees.Select(t => ate.FindDomNode(t));
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, l.ToArray() )
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToArray();

                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = false;
                string js1 = JsonSerializer.Serialize(nodes, serializeOptions);
                repl.input_output_stack.Push(js1);
            }
        }
    }
}
