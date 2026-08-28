using System;
using ParseTreeEditing.UnvParseTreeDOM;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trtree.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        if (config.Bundle)
        {
            ExecuteBundle(config);
            return;
        }

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var data = AntlrJson.ParsingResultIO.Read(config.File).Results;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
        bool more_than_one_fn = data.Count() > 1 || config.DisplayName;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        foreach (var in_tuple in data)
        {
            var nodes = in_tuple.Nodes;
            var lexer = in_tuple.Lexer;
            var parser = in_tuple.Parser;
            var fn = in_tuple.FileName;
            var prefix = more_than_one_fn ? fn + ": " : "";
            System.Console.Write(Render(in_tuple, config, prefix));
        }
        System.Console.WriteLine();
    }

    private static void ExecuteBundle(Config config)
    {
        var input = AntlrJson.ParsingResultIO.Read(config.File);
        using var output = Console.OpenStandardOutput();
        AntlrJson.ParsingResultIO.WriteFilteredBundle(
            output, input, ".tree",
            result => AntlrJson.ParsingResultIO.Utf8(Render(result, config, "")));
    }

    private static string Render(AntlrJson.ParsingResultSet result, Config config, string prefix)
    {
        var sb = new StringBuilder();
        foreach (var node in result.Nodes)
        {
            if (config.AntlrStyle)
                sb.AppendLine(new TreeOutput(result.Lexer, result.Parser, prefix).OutputTreeAntlrStyle(node).ToString());
            else if (config.ParenIndentStyle)
                sb.AppendLine(new TreeOutput(result.Lexer, result.Parser, prefix).OutputTree(node).ToString());
            else if (config.IndentStyle)
                sb.AppendLine(new TreeOutput(result.Lexer, result.Parser, prefix).OutputTreeIndentStyle(node).ToString());
            else if (config.BlockTreeStyle)
                sb.AppendLine(new TreeOutput(result.Lexer, result.Parser, prefix).OutputTreeBlockStyle(node).ToString());
        }
        return sb.ToString();
    }
}
