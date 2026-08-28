using org.w3c.dom;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trtext.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public string Reconstruct(Node tree)
    {
        Stack<Node> stack = new Stack<Node>();
        stack.Push(tree);
        StringBuilder sb = new StringBuilder();
        while (stack.Any())
        {
            var n = stack.Pop();
            if (n is UnvParseTreeAttr a)
            {
                var s = a.Name as String;
                if (s == null) ;
                else if (s == "Line") ;
                else if (s == "Column") ;
                else if (s == "ChildCount") ;
                else sb.Append(a.StringValue);
            }
            else if (n is UnvParseTreeText t)
            {
                sb.Append(t.NodeValue);
            }
            else if (n is UnvParseTreeElement e)
            {
                for (int i = n.ChildNodes.Length - 1; i >= 0; i--)
                {
                    stack.Push(n.ChildNodes.item(i));
                }
            }
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
                output, input, ".txt",
                result => AntlrJson.ParsingResultIO.Utf8(Render(result, config, false)));
            return;
        }

        bool more_than_one_fn = input.Results.Length > 1;
        foreach (var result in input.Results)
            System.Console.Write(Render(result, config, more_than_one_fn));
    }

    private string Render(AntlrJson.ParsingResultSet obj1, Config config, bool includeFileName)
    {
        var output = new StringBuilder();
        bool files_with_matches = config.FilesWithMatches;
        bool files_without_match = config.FilesWithoutMatch;
        bool count = config.Count;
        var fn = obj1.FileName;
        var nodes = obj1.Nodes;
        if (files_with_matches)
        {
            if (nodes.Any()) output.AppendLine(fn);
        }
        else if (files_without_match)
        {
            if (!nodes.Any()) output.AppendLine(fn);
        }
        else if (count)
        {
            if (includeFileName) output.Append(fn + ":");
            output.AppendLine(nodes.Count().ToString());
        }
        else
        {
            foreach (var node in nodes)
            {
                if (includeFileName) output.Append(fn + ":");
                output.AppendLine(Reconstruct(node));
            }
        }
        return output.ToString();
    }
}
