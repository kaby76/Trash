namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.@internal.types;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.IO;
    using System.Linq;
    using System.Text.Json;


    class CRename
    {
        public string Help()
        {
            return @"
This program is part of the Trash toolkit.

trrename <string> <string>
Rename a symbol, the first parameter as specified by the xpath expression string,
to a new name, the second parameter as a string. The result may place all changed
grammars that use the symbol on the stack.

Example:
    cat pt.data | trrename ""//parserRuleSpec//labeledAlt//RULE_REF[text() = 'e']"" xxx | trtext > new-grammar.g4
";
        }

        public void Execute(Config config)
        {
            var expr = config.Expr;
            System.Console.Error.WriteLine("Expr = '" + expr + "'");
            var to_sym = config.NewName;
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
                lines = File.ReadAllText(config.File);
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
            doc.ParseTree = null;
            doc.Changed = true;
            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            ref_pd.ParseTree = null;
            //ref_pd.Changed = true;
            _ = new Module().GetQuickInfo(0, doc);
            //Compile(workspace);
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            IParseTree root = atrees.First().Root();
            var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser))
            {
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                
                var results = LanguageServer.Transform.Rename(nodes, to_sym, doc);

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
