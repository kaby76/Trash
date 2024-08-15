using Antlr4.Runtime;
using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trxgrep.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        var expr = config.Expr.First();
        if (config.Verbose)
        {
            System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
        }

        UnvParseTreeNode[] atrees;
        Parser parser;
        Lexer lexer;
        string text;
        string fn;
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

        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
        serializeOptions.WriteIndented = config.Format;
        serializeOptions.MaxDepth = 10000;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
        var results = new List<ParsingResultSet>();
        bool do_rs = !config.NoParsingResultSets;
        List<UnvParseTreeNode> d = new List<UnvParseTreeNode>();
        List<AntlrDynamicContext> dc = new List<AntlrDynamicContext>();
        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
        foreach (var parse_info in data)
        {
            fn = parse_info.FileName;
            atrees = parse_info.Nodes;
            parser = parse_info.Parser;
            lexer = parse_info.Lexer;
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext = ate.Try(atrees, parser);
            dc.Add(dynamicContext);
            d.Add(dynamicContext.Document);
        }

        int i = 0;
        foreach (var parse_info in data)
        {
            var dynamicContext = dc[i++];
            var a = dynamicContext.Document;
            fn = parse_info.FileName;
            atrees = parse_info.Nodes;
            parser = parse_info.Parser;
            lexer = parse_info.Lexer;
            ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode[] l =
                new ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode[1] { a };
            var nodes = engine.parseExpression(expr,
                    new StaticContextBuilder()).evaluate(dynamicContext, l)
                .Select(x => (x.NativeValue)).ToArray();
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Length + " nodes.");
            List<UnvParseTreeNode> res = new List<UnvParseTreeNode>();
            foreach (var v in nodes)
            {
                if (v is ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)
                {
                    var q = v as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement;
                    res.Add(q);
                }
                else if (v is ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeText)
                {
                    var q = v as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeText;
                    var s = q.Data;
                    do_rs = false;
                    System.Console.WriteLine(s);
                }
                else if (v is ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeAttr)
                {
                    var q = v as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeAttr;
                    var s = q.StringValue;
                    do_rs = false;
                    System.Console.WriteLine(s);
                }
                else if (v is ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeDocument)
                {
                    var q = v as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeDocument;
                    do_rs = false;
                    System.Console.WriteLine(v);
                }
                else
                {
                    do_rs = false;
                    System.Console.WriteLine(v);
                }
            }

            var parse_info_out = new AntlrJson.ParsingResultSet()
                { FileName = fn, Lexer = lexer, Parser = parser, Nodes = res.ToArray() };
            results.Add(parse_info_out);
        }

        if (do_rs)
        {
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            System.Console.WriteLine(js1);
        }
    }
}
