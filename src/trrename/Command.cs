namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trrename.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
                }
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }
                lines = File.ReadAllText(config.File);
            }
            Dictionary<string, string> rename_map = new Dictionary<string, string>();
            if (config.RenameMap != null)
            {
                var l1 = config.RenameMap.Split(';').ToList();
                foreach (var l in l1)
                {
                    var l2 = l.Split(',').ToList();
                    if (l2.Count != 2)
                        throw new System.Exception("Rename map not correct. '"
                            + l
                            + "' doesn't have correct number of commans, should be 'oldval,newval'.");
                    rename_map[l2[0]] = l2[1];
                }
            }
            else if (config.RenameMapFile != null)
            {
                var contents = File.ReadAllText(config.RenameMapFile);
                var TrimNewLineChars = new char[] { '\n', '\r' };
                var ll = contents.Split(TrimNewLineChars, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var l in ll)
                {
                    var l2 = l.Split(',').ToList();
                    if (l2.Count != 2)
                        throw new System.Exception("Rename map not correct. '"
                            + l
                            + "' doesn't have correct number of commans, should be 'oldval,newval'.");
                    rename_map[l2[0]] = l2[1];
                }
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
                var trees = parse_info.Nodes.Select(t => t as AltAntlr.MyParserRuleContext).ToList();
                var parser = parse_info.Parser as AltAntlr.MyParser;
                var lexer = parse_info.Lexer as AltAntlr.MyLexer;
                var tokstream = parse_info.Stream as AltAntlr.MyTokenStream;
                var before_tokens = tokstream.GetTokens();
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
                {
                    var expr = "//(parserRuleSpec | lexerRuleSpec)//(RULE_REF | TOKEN_REF)[text()='"
                        + string.Join("' or text()='", rename_map.Select(r => r.Key))
                        + "']";
                    var nodes = engine.parseExpression(
                        expr, new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    foreach (var node in nodes)
                    {
                        var new_name = rename_map[node.GetText()];
                        var new_node = TreeEdits.CopyTreeRecursive(node);
                        (new_node.Payload as AltAntlr.MyToken).Text = new_name;
                        TreeEdits.Replace(tokstream, node, new_node);
                    }
                    var tuple = new ParsingResultSet()
                    {
                        Text = tokstream.Text,
                        FileName = fn,
                        Stream = tokstream,
                        Nodes = trees.ToArray(),
                        Lexer = lexer,
                        Parser = parser
                    };
                    results.Add(tuple);
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.Write(js1);
        }
    }
}
