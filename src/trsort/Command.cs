﻿using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.@internal.ast;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Antlr4.Runtime.Misc;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsort.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        var expr = "//ruleSpec[parserRuleSpec]";
        if (config.Verbose)
        {
            //System.Console.Error.WriteLine("from = >>>" + from + "<<<");
            //System.Console.Error.WriteLine("to = >>>" + to + "<<<");
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
        var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
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

            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                var list = new List<Pair<string, ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode>>();
                foreach (UnvParseTreeElement node in nodes)
                {
                    // Get name.
                    var name = engine.parseExpression("./parserRuleSpec/RULE_REF/text()",
                            new StaticContextBuilder()).evaluate(dynamicContext,
                            new object[] { node }) 
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeText))
                        .ToList().First().NodeValue as string;
                    // Add to list of {name x node} pairs to be sorted.
                    list.Add(new Pair<string, ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode>(name, node));
               //     if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + defs.Count + " defs.");
                }
                // reverse sort list by name.
                list.Sort((x, y) => y.a.CompareTo(x.a));

                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("list:");
                    foreach (var p in list)
                    {
                        System.Console.Error.WriteLine(p.a);
                    }
                }
                // Fix up grammar.
                if (list.Count > 0)
                {
                    // Find first rule in grammar.
                    var to = engine.parseExpression("//ruleSpec[parserRuleSpec][1]",
                            new StaticContextBuilder()).evaluate(dynamicContext,
                            new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                        .First();
                    foreach (var p in list)
                    {
                        TreeEdits.MoveToFirstChild(p.b, to.ParentNode as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode);
                        to = p.b;
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
        }

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
        string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
        System.Console.WriteLine(js1);
    }
}
