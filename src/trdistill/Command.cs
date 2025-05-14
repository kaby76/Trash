using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using AntlrJson;
using org.eclipse.wst.xml.xpath2.processor.util;
using org.eclipse.wst.xml.xpath2.processor;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trdistill.readme.md"))
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
        AntlrJson.ParsingResultSet[] data =
            JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
        List<ParsingResultSet> results = new List<ParsingResultSet>();
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

            var ty = new Regex("Parser$").Replace(Path.GetFileNameWithoutExtension(parser.GrammarFileName), "");
            switch (ty)
            {
                case "ANTLRv4":
                {
                    var new_parse_info = StripAndFormat(parse_info);
                    break;
                }
                default:
                {
                    System.Console.WriteLine("Unhandled grammar type.");
                    break;
                }
            }
        }

        string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
        System.Console.WriteLine(js1);
    }

    private object StripAndFormat(ParsingResultSet parse_info)
    {
        {
            // replace ws
            var reformat = new Reformat();

        }

    }
}
