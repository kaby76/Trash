namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;
    using System.Text.Json;

    class CUngroup
    {
        public string Help()
        {
            return @"
This program is part of the Trash toolkit.

trungroup <string>
Perform an ungroup transformation of the 'element' node(s) specified by the string.

Example:
    trparse A.g4 | trungroup ""//parserRuleSpec[RULE_REF/text() = 'a']//ruleAltList""
";
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.First();
            System.Console.Error.WriteLine("Expr = '" + expr + "'");
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
            var text = parse_info.Text;
            var fn = parse_info.FileName;
            var atrees = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            var tokstream = parse_info.Stream;
            var doc = Docs.Class1.CreateDoc(parse_info);
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            IParseTree root = atrees.First().Root();
            var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser))
            {
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();

                var results = LanguageServer.Transform.Ungroup(nodes, doc);
                
                Docs.Class1.EnactEdits(results);
                var pr = ParsingResultsFactory.Create(doc);
                IParseTree pt = pr.ParseTree;
                var tuple = new ParsingResultSet()
                {
                    Text = doc.Code,
                    FileName = doc.FullPath,
                    Stream = pr.TokStream,
                    Nodes = new IParseTree[] { pt },
                    Lexer = pr.Lexer,
                    Parser = pr.Parser
                };
                string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
                System.Console.WriteLine(js1);
            }
        }
    }
}
