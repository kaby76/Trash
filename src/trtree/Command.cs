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
                if (lines != null && lines != "")
                {
                    break;
                }
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
        serializeOptions.WriteIndented = false;
        serializeOptions.MaxDepth = 10000;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
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
        using var input = string.IsNullOrEmpty(config.File)
            ? Console.OpenStandardInput()
            : File.OpenRead(config.File);
        var inputArtifacts = AntlrJson.ArtifactBundle.Read(input);
        var outputArtifacts = new System.Collections.Generic.List<AntlrJson.Artifact>();
        foreach (var artifact in inputArtifacts)
        {
            if (!artifact.Name.EndsWith(".pt", StringComparison.OrdinalIgnoreCase))
            {
                outputArtifacts.Add(artifact);
                continue;
            }

            var result = AntlrJson.ArtifactBundle.DeserializeParsingResult(artifact.Data);
            var tree = Render(result, config, "");
            outputArtifacts.Add(new AntlrJson.Artifact(
                AntlrJson.ArtifactBundle.ChangeExtension(artifact.Name, ".tree"),
                new UTF8Encoding(false).GetBytes(tree)));
        }

        using var output = Console.OpenStandardOutput();
        AntlrJson.ArtifactBundle.Write(output, outputArtifacts);
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
