using Antlr4.Runtime;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using XQuery.Engine;
using XPathParser = XQuery.Parser.XPathParser;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trxpath.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        string expr;
        if (config.QueryFile != null && config.QueryFile != "")
        {
            expr = System.IO.File.ReadAllText(config.QueryFile).Trim();
        }
        else if (config.Expr != null && config.Expr.Any())
        {
            expr = config.Expr.First();
        }
        else
        {
            System.Console.Error.WriteLine("Error: provide an XPath expression as an argument or via --query <file>.");
            return;
        }

        if (config.Verbose)
        {
            System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
        }

        UnvParseTreeNode[] atrees;
        Parser parser;
        Lexer lexer;
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

        var exprNode = new XPathParser(expr).Parse();
        var evaluator = new XPathEvaluator();

        var results = new List<ParsingResultSet>();
        bool do_rs = !config.NoParsingResultSets;

        foreach (var parse_info in data)
        {
            fn = parse_info.FileName;
            atrees = parse_info.Nodes;
            parser = parse_info.Parser;
            lexer = parse_info.Lexer;

            var adapterDoc = AdapterDocument.Build(atrees);
            var xdmResults = evaluator.Evaluate(exprNode, adapterDoc);

            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + xdmResults.Count + " nodes.");

            List<UnvParseTreeNode> res = new List<UnvParseTreeNode>();
            foreach (var item in xdmResults)
            {
                if (item is AdapterElement ae)
                {
                    res.Add(ae.Source);
                }
                else if (item is AdapterText at)
                {
                    do_rs = false;
                    System.Console.WriteLine(at.Source.Data);
                }
                else if (item is AdapterAttribute aa)
                {
                    do_rs = false;
                    System.Console.WriteLine(aa.Source.StringValue);
                }
                else if (item is AdapterDocument)
                {
                    do_rs = false;
                    System.Console.WriteLine(item);
                }
                else
                {
                    do_rs = false;
                    System.Console.WriteLine(item.StringValue);
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
