namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using AntlrTreeEditing.AntlrDOM;
    using EditableAntlrTree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
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
                    LoggerNs.TimedStderrOutput.WriteLine("reading from stdin");
                }
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
                lines = lines.Trim();
            }
            else
            {
                if (config.Verbose)
                {
                    LoggerNs.TimedStderrOutput.WriteLine("reading from file >>>" + config.File + "<<<");
                }
                lines = File.ReadAllText(config.File);
            }
            Dictionary<string, string> rename_map = new Dictionary<string, string>();
            if (config.RenameMap != null)
            {
                foreach (var s in config.RenameMap)
                {
                    var l1 = s.Split(';').ToList();
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
            serializeOptions.Converters.Add(new ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var trees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                if (config.Verbose)
                {
                    foreach (var n in trees)
                        System.Console.WriteLine(TreeOutput.OutputTree(n, lexer, parser).ToString());
                }
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
                {
                    string expr;
                    if (config.Expr == null)
                        expr = "//(parserRuleSpec | lexerRuleSpec)//(RULE_REF | TOKEN_REF)";
                    else
                        expr = config.Expr;
                    var nodes = engine.parseExpression(
                        expr, new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToList();
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                    foreach (var node in nodes)
                    {
                        string c = null;
                        AntlrText t = null;
                        for (int k = 0; k < node.ChildNodes.Length; k++)
                        {
                            var n = node.ChildNodes.item(k);
                            t = n as AntlrText;
                            if (n != null)
                            {
                                c = t.NodeValue as string;
                                break;
                            }
                        }
                        if (c == null || t == null) throw new Exception("Cannot find text.");
                        if (rename_map.TryGetValue(c, out string new_name))
                        {
                            t.NodeValue = new_name;
                            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Replaced " + t);
                        }
                    }
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Finished editing.");
                    var tuple = new ParsingResultSet()
                    {
                        Text = text,
                        FileName = fn,
                        Nodes = trees,
                        Lexer = lexer,
                        Parser = parser
                    };
                    results.Add(tuple);
                }
            }
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            System.Console.WriteLine(js1);
        }
    }
}
