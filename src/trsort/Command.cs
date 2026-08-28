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
        var input = ParsingResultIO.Read(config.File);

        int modeCount = (config.Alphabetic ? 1 : 0) + (config.Bfs ? 1 : 0) + (config.Dfs ? 1 : 0);
        if (modeCount > 1)
            throw new System.Exception(
                "Conflicting sort options: specify at most one of --alphabetic, --bfs, --dfs.");

        var data = input.Results;
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
                // config.Expr is the start rule name, or null to auto-detect.
                CReorder.BfsSort(trees, parser, config.Expr);
            }
            else if (config.Dfs)
            {
                // config.Expr is the start rule name, or null to auto-detect.
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
        using var output = System.Console.OpenStandardOutput();
        ParsingResultIO.WriteBundle(output, input, results, config.Format);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
    }
}
