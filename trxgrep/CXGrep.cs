namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;
    using System.Text.Json;

    class CXGrep
    {
        public void Help()
        {
            System.Console.WriteLine(@"
This program is part of the Trash toolkit.

trxgrep <string>
Find all sub-trees in the parsed file at the top of stack using the given XPath expression string.

Example:
    trparse A.g4 | trxgrep ""//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']""
");
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.First();
            System.Console.Error.WriteLine("Expr = '" + expr + "'");
            IParseTree[] atrees;
            Parser parser;
            Lexer lexer;
            string text;
            string fn;
            ITokenStream tokstream;
            string lines = null;
            for (; ; )
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            AntlrJson.ParsingResultSet parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
            text = parse_info.Text;
            fn = parse_info.FileName;
            atrees = parse_info.Nodes;
            parser = parse_info.Parser;
            lexer = parse_info.Lexer;
            tokstream = parse_info.Stream;
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            IParseTree root = atrees.First().Root();
            var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser))
            {
                var l = atrees.Select(t => ate.FindDomNode(t));
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, l.ToArray() )
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToArray();
                System.Console.Error.WriteLine("Result size " + nodes.Count());
                var parse_info_out = new AntlrJson.ParsingResultSet(){Text = text, FileName = fn, Lexer = lexer, Parser = parser, Stream = tokstream, Nodes = nodes };
                string js1 = JsonSerializer.Serialize(parse_info_out, serializeOptions);
                System.Console.WriteLine(js1);
            }
        }
    }
}
