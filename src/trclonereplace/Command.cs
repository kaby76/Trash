using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text.Json;

namespace Trash;

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
        var expr = config.Expr.FirstOrDefault();
        if (expr == null) throw new Exception("No expression specified.");
        if (config.Verbose)
        {
            System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
        }
        string lines = null;
        if (!(config.File != null && config.File != ""))
        {
            if (config.Verbose)
            {
                LoggerNs.TimedStderrOutput.WriteLine("reading from stdin");
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
                LoggerNs.TimedStderrOutput.WriteLine("reading from file >>>" + config.File + "<<<");
            }

            lines = File.ReadAllText(config.File);
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
            var fn = parse_info.FileName;
            var trees = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            if (config.Verbose)
            {
                foreach (var n in trees)
                    System.Console.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
            }

            /* Step 1: Find all parser symbols on the right hand side that match,
             * then recursively follow the parser rule for that symbol, and collect
             * addisional symbols to clone/replace. Stop recursing when you find
             * the symbol already in the list.
             */
            List<UnvParseTreeElement> todo = new List<UnvParseTreeElement>();
            List<UnvParseTreeElement> list = new List<UnvParseTreeElement>();

            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
            {
                // Initialize to do list.
                var nodes = engine.parseExpression(
                        expr, new StaticContextBuilder())
                    .evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();

                if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                foreach (UnvParseTreeElement node in nodes)
                {
                    todo.Clear();
                    todo.Add(node);
                    var rule_containing = engine.parseExpression(
                            "./ancestor::parserRuleSpec", new StaticContextBuilder())
                        .evaluate(dynamicContext, new object[] { node })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    
                    var node_name = engine.parseExpression(
                            "./RULE_REF/text()", new StaticContextBuilder())
                        .evaluate(dynamicContext, new object[] { rule_containing.First() })
                        .Select(x => x.StringValue)
                        .ToList().FirstOrDefault();

                    // Repeat recording rules that need to be cloned.
                    List<string> done = new List<string>();
                    List<UnvParseTreeElement> done_rules = new List<UnvParseTreeElement>();
                    for (;;)
                    {
                        foreach (var t in todo)
                        {
                            // Get parser rule with LHS symbol = node named.
                            string name = engine.parseExpression(
                                    "text()", new StaticContextBuilder())
                                .evaluate(dynamicContext, new object[] { t })
                                .Select(x => x.StringValue)
                                .ToList().FirstOrDefault();

                            if (done.Contains(name)) continue;

                            var rules = engine.parseExpression(
                                    "//parserRuleSpec[RULE_REF/text() = '" + name + "']", new StaticContextBuilder())
                                .evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                                .ToList();
                            if (config.Verbose)
                                LoggerNs.TimedStderrOutput.WriteLine("Found " + rules.Count + " nodes.");
                            var rule = rules.FirstOrDefault();
                            done_rules.Add(rule);
                            done.Add(name);

                            // Get rhs symbols.
                            var more = engine.parseExpression(
                                    "./ruleBlock//RULE_REF", new StaticContextBuilder())
                                .evaluate(dynamicContext, new object[] { rule })
                                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                                .ToList();
                            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + more.Count + " nodes.");
                            foreach (var m in more) list.Add(m);
                        }

                        todo.Clear();
                        todo.AddRange(list);
                        list.Clear();
                        if (todo.Count == 0) break;
                    }

                    var rename_map = new Dictionary<string, string>();
                    foreach (var z in done)
                    {
                        rename_map[z] = z + (config.Suffix == "" ? "_" + node_name : config.Suffix);
                    }

                    /* Clone all rules in list. */
                    foreach (var r in done_rules)
                    {
                        var new_rule = TreeEdits.CopyTreeRecursive(r);

                        // Modify name and RHS symbol names.
                        {
                            string c = null;
                            UnvParseTreeText t = null;
                            for (int k = 0; k < node.ChildNodes.Length; k++)
                            {
                                var n = node.ChildNodes.item(k);
                                t = n as UnvParseTreeText;
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

                        string ren = ".//RULE_REF";
                        var ren_nodes = engine.parseExpression(
                                ren, new StaticContextBuilder())
                            .evaluate(dynamicContext, new object[] { new_rule })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        
                        
                        foreach (var ren_node in ren_nodes)
                        {
                            string c = null;
                            UnvParseTreeText t = null;
                            for (int k = 0; k < node.ChildNodes.Length; k++)
                            {
                                var n = ren_node.ChildNodes.item(k);
                                t = n as UnvParseTreeText;
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

                        // Add after rule containing node.
                        TreeEdits.InsertAfter(rule_containing.First(), new_rule);
                    }
                }
            }

            var tuple = new ParsingResultSet()
                {
                    FileName = fn,
                    Nodes = trees,
                    Lexer = lexer,
                    Parser = parser
                };
                results.Add(tuple);
        }

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
        string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
        System.Console.WriteLine(js1);
    }
}
