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
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trtokens.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    private string OutputTokens(UnvParseTreeElement tree)
    {
        var frontier = TreeEdits.Frontier(tree).ToList();
        if (frontier.Count == 0) return "";
        var first = frontier.First();
        var last = frontier.Last();
        StringBuilder sb = new StringBuilder();
        foreach (UnvParseTreeNode i in frontier)
        {
            var a = i as UnvParseTreeAttr;

            var e = i as UnvParseTreeElement;
            if (e == null) continue;

            sb.AppendLine("[@" + e.GetText());
        }

        return sb.ToString();
    }

    public void Execute(Config config)
    {
        var input = AntlrJson.ParsingResultIO.Read(config.File);
        if (config.Bundle)
        {
            using var output = System.Console.OpenStandardOutput();
            AntlrJson.ParsingResultIO.WriteFilteredBundle(
                output, input, ".tokens", Render);
            return;
        }

        foreach (var parse_info in input.Results)
            System.Console.Write(Encoding.UTF8.GetString(Render(parse_info)));
    }

    private static byte[] Render(AntlrJson.ParsingResultSet parseInfo)
    {
        var text = new StringBuilder();
        foreach (var node in parseInfo.Nodes)
        {
            text.AppendLine(TokenOutput.OutputTokens(
                node, parseInfo.Lexer, parseInfo.Parser).ToString());
        }
        return AntlrJson.ParsingResultIO.Utf8(text.ToString());
    }
}
