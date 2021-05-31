namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Collections.Generic;
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
            //var expr = config.Expr;
            //System.Console.Error.WriteLine("Expr = '" + expr + "'");
            //var to_sym = config.NewName;
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
            serializeOptions.Converters.Add(new ParseTreeConverter());
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
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

                var l1 = config.RenameMap.Split(';').ToList();
                var rename_map = new Dictionary<string, string>();
                foreach (var l in l1)
                {
                    var l2 = l.Split(',').ToList();
                    if (l2.Count != 2)
                        throw new System.Exception("rename map not correct");
                    rename_map[l2[0]] = l2[1];
                }

                System.Collections.Generic.Dictionary<string, string> res = null;
                res = LanguageServer.Transform.Rename(rename_map, doc);
                if (res != null && res.Count > 0)
                {
                    var pr = ParsingResultsFactory.Create(doc);
                    Docs.Class1.EnactEdits(res);
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
                    results.Add(tuple);
                }
                else
                {
                    System.Console.Write(lines);
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.Write(js1);

            //org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            //IParseTree root = atrees.First().Root();
            //var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            //using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser))
            //{
            //    var nodes = engine.parseExpression(expr,
            //            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
            //        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();

            //    System.Collections.Generic.Dictionary<string, string> results = null;
            //    if (nodes != null && nodes.Count > 0)
            //    {
            //        results = LanguageServer.Transform.Rename(nodes, to_sym, doc);
            //    }
            //    if (results != null && results.Count > 0)
            //    {
            //        Docs.Class1.EnactEdits(results);
            //        var pr = ParsingResultsFactory.Create(doc);
            //        IParseTree pt = pr.ParseTree;
            //        var tuple = new ParsingResultSet()
            //        {
            //            Text = doc.Code,
            //            FileName = doc.FullPath,
            //            Stream = pr.TokStream,
            //            Nodes = new IParseTree[] { pt },
            //            Lexer = pr.Lexer,
            //            Parser = pr.Parser
            //        };
            //        string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
            //        System.Console.WriteLine(js1);
            //    }
            //    else
            //    {
            //        System.Console.WriteLine(lines);
            //    }
            //}
        }
    }
}
