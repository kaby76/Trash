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

namespace Trash.trquery;

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
            IEnumerable<string> query = config.Query;
            if (query.Any())
            {
                input = String.Join(" ", query);
            }
            else
            {
                throw new Exception("Query required, none supplied on command line.");
            }
        }

        ICharStream cs = CharStreams.fromString(input);
        var slexer = new QueryLexer(cs);
        CommonTokenStream stokens = new CommonTokenStream(slexer);
	var sparser = new QueryParser(stokens);
	var listener_lexer = new ErrorListener<int>(false, false, System.Console.Error);
	var listener_parser = new ErrorListener<IToken>(false, false, System.Console.Error);
	slexer.RemoveErrorListeners();
	sparser.RemoveErrorListeners();
	slexer.AddErrorListener(listener_lexer);
	sparser.AddErrorListener(listener_parser);
        var stree = sparser.commands();
	if (listener_parser.had_error || listener_lexer.had_error)
	{
		throw new Exception("Bad query. " + input);
	}

        string lines = null;
        if (!(config.File != null && config.File != ""))
        {
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("reading from stdin");
            }

            for (;;)
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

        JsonSerializerOptions serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
        serializeOptions.WriteIndented = config.Format;
        serializeOptions.MaxDepth = 10000;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        ParsingResultSet[] data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
        List<ParsingResultSet> results = new List<ParsingResultSet>();
        foreach (ParsingResultSet parse_info in data)
        {
            UnvParseTreeNode[] trees = parse_info.Nodes;
            string fn = parse_info.FileName;
            Parser parser = parse_info.Parser;
            Lexer lexer = parse_info.Lexer;
            if (config.Verbose)
            {
                foreach (UnvParseTreeNode n in trees)
                    LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
            }

            foreach (var scommand in stree.command())
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var command = scommand.GetChild(0).GetText();
                if (command == "grep")
                {
                    var expr_tree = scommand.expr()[0];
                    var si = expr_tree.SourceInterval;
                    IToken start = stokens.Get(si.a);
                    int bi = start.StartIndex;
                    IToken stop = stokens.Get(si.b);
                    int ei = stop.StopIndex;
                    string expr = cs.GetText(new Interval(bi, ei));
                    if (config.Verbose)
                        LoggerNs.TimedStderrOutput.WriteLine("grep expr " + expr);
                    ConvertToDOM ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(trees, parser))
                    {
                        List<UnvParseTreeNode> nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n)
                                    .ToString());
                        }
                        if (scommand.MATCH_REQUIRED() != null)
                        {
                            throw new Exception("No match found for XPath expression, where it is required.");
                        }
                        trees = nodes.ToArray();
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Resulted in this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                    }
                }
                else if (command == "insert")
                {
                    string expr;
                    {
                        var expr_tree = scommand.expr()[0];
                        var si = expr_tree.SourceInterval;
                        IToken start = stokens.Get(si.a);
                        int bi = start.StartIndex;
                        IToken stop = stokens.Get(si.b);
                        int ei = stop.StopIndex;
                        expr = cs.GetText(new Interval(bi, ei));
                    }
                    string @string;
                    {
                        var string_tree = scommand.@string();
                        var si = string_tree.SourceInterval;
                        IToken start = stokens.Get(si.a);
                        int bi = start.StartIndex;
                        IToken stop = stokens.Get(si.b);
                        int ei = stop.StopIndex;
                        @string = cs.GetText(new Interval(bi, ei));
                        @string = RemoveQuotes(@string);
                    }
                    if (config.Verbose)
                        LoggerNs.TimedStderrOutput.WriteLine("insert expr " + expr);
                    ConvertToDOM ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(trees, parser))
                    {
                        List<UnvParseTreeNode> nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        if (scommand.MATCH_REQUIRED() != null)
                        {
                            throw new Exception("No match found for XPath expression, where it is required.");
                        }
                        foreach (UnvParseTreeNode node in nodes)
                        {
                            if (scommand.BEFORE() != null)
                                TreeEdits.InsertBefore(node, @string);
                            else if (scommand.AFTER() != null)
                                TreeEdits.InsertAfter(node, @string);
                            else
                                TreeEdits.InsertBefore(node, @string);
                        }
                    }
                }
                else if (command == "replace")
                {
                    var expr_tree = scommand.expr()[0];
                    var si = expr_tree.SourceInterval;
                    IToken start = stokens.Get(si.a);
                    int bi = start.StartIndex;
                    IToken stop = stokens.Get(si.b);
                    int ei = stop.StopIndex;
                    string expr = cs.GetText(new Interval(bi, ei));
                    string value = RemoveQuotes(scommand.GetChild(2).GetText());
                    if (config.Verbose)
                        LoggerNs.TimedStderrOutput.WriteLine("replace expr " + expr + " " + value);
                    ConvertToDOM ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(trees, parser))
                    {
                        List<UnvParseTreeNode> nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        if (scommand.MATCH_REQUIRED() != null)
                        {
                            throw new Exception("No match found for XPath expression, where it is required.");
                        }
                        foreach (UnvParseTreeNode node in nodes)
                        {
                            TreeEdits.Replace(node, value);
                        }
                    }
                }
                else if (command == "delete")
                {
                    var expr_tree = scommand.expr()[0];
                    var si = expr_tree.SourceInterval;
                    IToken start = stokens.Get(si.a);
                    int bi = start.StartIndex;
                    IToken stop = stokens.Get(si.b);
                    int ei = stop.StopIndex;
                    string expr = cs.GetText(new Interval(bi, ei));
                    if (config.Verbose)
                        LoggerNs.TimedStderrOutput.WriteLine("delete expr " + expr);
                    ConvertToDOM ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(trees, parser))
                    {
                        List<UnvParseTreeNode> nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                        if (scommand.MATCH_REQUIRED() != null)
                        {
                            throw new Exception("No match found for XPath expression, where it is required.");
                        }
                        TreeEdits.Delete(nodes);
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Resulted in this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                    }
                }
                else if (command == "delete-reattach")
                {
                    var expr_tree = scommand.expr()[0];
                    var si = expr_tree.SourceInterval;
                    IToken start = stokens.Get(si.a);
                    int bi = start.StartIndex;
                    IToken stop = stokens.Get(si.b);
                    int ei = stop.StopIndex;
                    string expr = cs.GetText(new Interval(bi, ei));
                    if (config.Verbose)
                        LoggerNs.TimedStderrOutput.WriteLine("delete-reattach expr " + expr);
                    ConvertToDOM ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(trees, parser))
                    {
                        List<UnvParseTreeNode> nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                        if (scommand.MATCH_REQUIRED() != null)
                        {
                            throw new Exception("No match found for XPath expression, where it is required.");
                        }
                        TreeEdits.DeleteAndReattachChildren(nodes);
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Resulted in this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                    }
                }
                else if (command == "move")
                {
                    var expr_from = scommand.expr()[0];
                    var expr_to = scommand.expr()[1];
                    string expr_from_text;
                    {
                        var si = expr_from.SourceInterval;
                        IToken start = stokens.Get(si.a);
                        int bi = start.StartIndex;
                        IToken stop = stokens.Get(si.b);
                        int ei = stop.StopIndex;
                        expr_from_text = cs.GetText(new Interval(bi, ei));
                    }
                    string expr_to_text;
                    {
                        var si = expr_to.SourceInterval;
                        IToken start = stokens.Get(si.a);
                        int bi = start.StartIndex;
                        IToken stop = stokens.Get(si.b);
                        int ei = stop.StopIndex;
                        expr_to_text = cs.GetText(new Interval(bi, ei));
                    }
                    if (config.Verbose)
                        LoggerNs.TimedStderrOutput.WriteLine("move " + expr_from_text + " " + expr_to_text);
                    ConvertToDOM ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(trees, parser))
                    {
                        List<UnvParseTreeNode> nodes = engine.parseExpression(expr_from_text,
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                            .ToList();
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Operating on this:");
                            foreach (UnvParseTreeNode n in nodes)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }

                        foreach (UnvParseTreeNode n in nodes)
                        {
                            var too = engine.parseExpression(expr_to_text,
                                    new StaticContextBuilder()).evaluate(dynamicContext,
                                    new object[] { n })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                                .ToList();
                            if (!too.Any() || too.Count > 1)
                                throw new Exception();
                            var to = too.FirstOrDefault();
                            if (scommand.MATCH_REQUIRED() != null)
                            {
                                throw new Exception("No match found for XPath expression, where it is required.");
                            }
                            TreeEdits.MoveBefore(new List<UnvParseTreeNode>() { n as UnvParseTreeNode }, to);
                        }

                        if (config.Verbose)
                        {
                            LoggerNs.TimedStderrOutput.WriteLine("Resulted in this:");
                            foreach (UnvParseTreeNode n in trees)
                                LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
                        }
                    }
                }
                else throw new Exception("unknown command");
            }

            ParsingResultSet tuple = new ParsingResultSet()
            {
                FileName = fn,
                Nodes = trees,
                Lexer = lexer,
                Parser = parser
            };
            results.Add(tuple);
            if (config.Verbose)
            {
                foreach (UnvParseTreeNode node in trees)
                    System.Console.Error.WriteLine(new TreeOutput(lexer, parser).OutputTree(node).ToString());
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
