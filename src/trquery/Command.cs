namespace Trash
{
    using Antlr4.Runtime;
    using AntlrJson;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using ParseTreeEditing.UnvParseTreeDOM;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trquery.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var file_name = config.Query.FirstOrDefault();
            string inp = null;
            ICharStream str = null;
            if (file_name == null && config.Input != null)
            {
                inp = config.Input;
                str = CharStreams.fromString(inp);
            }
            else if (file_name != null)
            {
                FileStream fs = new FileStream(file_name, FileMode.Open);
                str = new Antlr4.Runtime.AntlrInputStream(fs);
            }
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("Query = >>>" + file_name + "<<<");
            }
            var slexer = new ParseTreeScriptLexer(str);
            var stokens = new CommonTokenStream(slexer);
            var sparser = new ParseTreeScriptParser(stokens);
            var stree = sparser.commands();

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
                lines = lines.Trim();
            }
            else
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }
                lines = File.ReadAllText(config.File);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var trees = parse_info.Nodes;
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                if (config.Verbose)
                {
                    foreach (var n in trees)
                        LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(n, lexer, parser).ToString());
                }
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
                {
                    foreach (var scommand in stree.command())
                    {
                        var command = scommand.GetChild(0).GetText();
                        if (command == "inserta")
                        {
                            var expr = RemoveQuotes(scommand.GetChild(1).GetText());
                            var value = RemoveQuotes(scommand.GetChild(2).GetText());
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                                .ToList();
                            if (config.Verbose)
                                LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                            foreach (var node in nodes)
                            {
                                TreeEdits.InsertAfter(node, value);
                            }
                        }
                        else if (command == "insertb")
                        {
                            var expr = RemoveQuotes(scommand.GetChild(1).GetText());
                            var value = RemoveQuotes(scommand.GetChild(2).GetText());
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                                .ToList();
                            if (config.Verbose)
                                LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                            foreach (var node in nodes)
                            {
                                TreeEdits.InsertBefore(node, value);
                            }
                        }
                        else if (command == "replace")
                        {
                            var expr = RemoveQuotes(scommand.GetChild(1).GetText());
                            var value = RemoveQuotes(scommand.GetChild(2).GetText());
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                                .ToList();
                            if (config.Verbose)
                                LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                            foreach (var node in nodes)
                            {
                                TreeEdits.Replace(node, value);
                            }
                        }
                        else if (command == "delete")
                        {
                            var expr = RemoveQuotes(scommand.GetChild(1).GetText());
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                                .ToList();
                            TreeEdits.Delete(nodes);
                        }
                    }

                    var tuple = new ParsingResultSet()
                    {
                        Text = text,
                        FileName = fn,
                        Nodes = trees,
                        Lexer = lexer,
                        Parser = parser
                    };
                    results.Add(tuple);
                    if (config.Verbose)
                    {
                        foreach (var node in trees)
                            System.Console.Error.WriteLine(TreeOutput.OutputTree(node, lexer, parser).ToString());
                    }
                }
            }
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            System.Console.WriteLine(js1);
        }

        private string RemoveQuotes(string v)
        {
            var v2 = v.Substring(1);
            var v3 = v2.Substring(0, v2.Length - 1);
            return v3;
        }
    }
}
