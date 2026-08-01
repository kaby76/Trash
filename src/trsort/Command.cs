using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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
        string lines = null;
        if (!(config.File != null && config.File != ""))
        {
            if (config.Verbose)
                System.Console.Error.WriteLine("reading from stdin");

            for (;;)
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }
        }
        else
        {
            if (config.Verbose)
                System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");

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

            if (config.Bfs)
            {
                if (string.IsNullOrEmpty(config.Expr))
                    throw new System.Exception("--bfs requires an XPath expression argument identifying start rules.");
                CReorder.BfsSort(trees, parser, config.Expr);
            }
            else if (config.Dfs)
            {
                if (string.IsNullOrEmpty(config.Expr))
                    throw new System.Exception("--dfs requires an XPath expression argument identifying start rules.");
                CReorder.DfsSort(trees, parser, config.Expr);
            }
            else
            {
                // Default: alphabetic sort.
                CReorder.AlphaSort(trees, parser);
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
