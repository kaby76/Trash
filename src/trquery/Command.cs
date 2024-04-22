using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Trash
{

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
            string input = null;
            if (config.CommandFile != null)
            {
                input = File.ReadAllText(config.CommandFile);
            }
            else
            {
                var query = config.Query;
                if (query.Any())
                {
                    input = String.Join(" ", query);
                }
            }

            var cs = CharStreams.fromString(input);
            var slexer = new QueryLexer(cs);
            var stokens = new CommonTokenStream(slexer);
            var sparser = new QueryParser(stokens);
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

                foreach (var scommand in stree.command())
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                        new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var command = scommand.GetChild(0).GetText();
                    if (command == "insert")
                    {
                        var expr_tree = scommand.expr();
                        var si = expr_tree.SourceInterval;
                        var start = stokens.Get(si.a);
                        var bi = start.StartIndex;
                        var stop = stokens.Get(si.b);
                        var ei = stop.StopIndex;
                        var expr = cs.GetText(new Interval(bi, ei));
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("insert expr " + expr);
                        var value = RemoveQuotes(scommand.GetChild(2).GetText());
                        var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                        using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                               ate.Try(trees, parser))
                        {
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                                .ToList();
                            if (config.Verbose)
                            {
                                LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                                foreach (var n in trees)
                                    LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(n, lexer, parser)
                                        .ToString());
                            }

                            if (config.Verbose)
                                LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                            foreach (var node in nodes)
                            {
                                TreeEdits.InsertAfter(node, value);
                            }
                        }
                    }
                    else if (command == "replace")
                    {
                        var expr_tree = scommand.expr();
                        var si = expr_tree.SourceInterval;
                        var start = stokens.Get(si.a);
                        var bi = start.StartIndex;
                        var stop = stokens.Get(si.b);
                        var ei = stop.StopIndex;
                        var expr = cs.GetText(new Interval(bi, ei));
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("replace expr " + expr);
                        var value = RemoveQuotes(scommand.GetChild(2).GetText());
                        var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                        using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                               ate.Try(trees, parser))
                        {
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                                .ToList();
                            if (config.Verbose)
                            {
                                LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                                foreach (var n in trees)
                                    LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(n, lexer, parser)
                                        .ToString());
                            }

                            if (config.Verbose)
                                LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                            foreach (var node in nodes)
                            {
                                TreeEdits.Replace(node, value);
                            }
                        }
                    }
                    else if (command == "delete")
                    {
                        var expr_tree = scommand.expr();
                        var si = expr_tree.SourceInterval;
                        var start = stokens.Get(si.a);
                        var bi = start.StartIndex;
                        var stop = stokens.Get(si.b);
                        var ei = stop.StopIndex;
                        var expr = cs.GetText(new Interval(bi, ei));
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("delete expr " + expr);
                        var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                        using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                               ate.Try(trees, parser))
                        {
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                                .ToList();
                            if (config.Verbose)
                                LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                            if (config.Verbose)
                            {
                                LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                                foreach (var n in trees)
                                    LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(n, lexer, parser)
                                        .ToString());
                            }
                            TreeEdits.Delete(nodes);
                            if (config.Verbose)
                            {
                                LoggerNs.TimedStderrOutput.WriteLine("Resulted in this:");
                                foreach (var n in trees)
                                    LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(n, lexer, parser)
                                        .ToString());
                            }
                        }
                    }
		    else if (command == "delete-reattach")
		    {
			    var expr_tree = scommand.expr();
			    var si = expr_tree.SourceInterval;
			    var start = stokens.Get(si.a);
			    var bi = start.StartIndex;
			    var stop = stokens.Get(si.b);
			    var ei = stop.StopIndex;
			    var expr = cs.GetText(new Interval(bi, ei));
			    if (config.Verbose)
				    LoggerNs.TimedStderrOutput.WriteLine("delete-reattach expr " + expr);
			    var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
			    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
				    ate.Try(trees, parser))
			    {
				    var nodes = engine.parseExpression(expr,
					    new StaticContextBuilder()).evaluate(dynamicContext,
					    new object[] { dynamicContext.Document })
						.Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
						.ToList();
				    if (config.Verbose)
					    LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
				    if (config.Verbose)
				    {
					    LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
					    foreach (var n in trees)
							    LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(n, lexer, parser)
						    .ToString());
				    }
				    TreeEdits.DeleteAndReattachChildren(nodes);
				    if (config.Verbose)
				    {
					    LoggerNs.TimedStderrOutput.WriteLine("Resulted in this:");
					    foreach (var n in trees)
							    LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(n, lexer, parser)
						    .ToString());
				    }
			    }
		    }
                }
                var tuple = new ParsingResultSet()
                {
                    Text = ParseTreeEditing.UnvParseTreeDOM.TreeEdits.Reconstruct(trees),
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
