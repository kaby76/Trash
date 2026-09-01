using org.w3c.dom;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsponge.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        var input = AntlrJson.ParsingResultIO.Read(config.File);
        if (input.IsBundle)
        {
            var artifacts = AntlrJson.ParsingResultIO.MaterializeSources(
                input,
                result => Reconstruct(result.Nodes));
            ExtractBundle(config, artifacts);
            return;
        }
        foreach (var parse_info in input.Results)
        {
            var nodes = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            var fn = parse_info.FileName;
            if (config.OutputDirectory != null)
            {
                Directory.CreateDirectory(config.OutputDirectory);
                if (!(config.OutputDirectory.EndsWith("\\") || config.OutputDirectory.EndsWith("/")))
                    config.OutputDirectory = config.OutputDirectory + "/";
                fn = config.OutputDirectory + Path.GetFileName(fn);
            }

            if (File.Exists(fn) && (!(bool)config.Clobber))
                throw new System.Exception("Attempting to overwrite '" + fn +
                                           "'. Use -c/--clobber option if it is intended.");
            System.Console.Error.WriteLine("Writing to " + fn);
            File.WriteAllText(fn, Reconstruct(nodes));
        }
    }

    private static void ExtractBundle(
        Config config, IReadOnlyList<AntlrJson.Artifact> artifacts)
    {
        if (string.IsNullOrWhiteSpace(config.OutputDirectory))
            throw new ArgumentException("--output-directory is required for bundle input.");

        var root = Path.GetFullPath(config.OutputDirectory);
        var rootPrefix = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : root + Path.DirectorySeparatorChar;
        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var destinations = new Dictionary<string, AntlrJson.Artifact>(comparer);

        foreach (var artifact in artifacts)
        {
            var relative = artifact.Name.Replace('/', Path.DirectorySeparatorChar);
            var destination = Path.GetFullPath(Path.Combine(root, relative));
            if (!destination.StartsWith(rootPrefix, OperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal))
                throw new InvalidDataException($"Bundle member '{artifact.Name}' escapes output directory.");
            if (!destinations.TryAdd(destination, artifact))
                throw new InvalidDataException($"Multiple bundle members map to '{destination}'.");
            if (File.Exists(destination) && !config.Clobber)
                throw new IOException($"Attempting to overwrite '{destination}'. Use -c/--clobber if intended.");
        }

        Directory.CreateDirectory(root);
        foreach (var pair in destinations)
        {
            var directory = Path.GetDirectoryName(pair.Key);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            if (config.Verbose)
                System.Console.Error.WriteLine("Writing to " + pair.Key);
            File.WriteAllBytes(pair.Key, pair.Value.Data);
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
                sb.Append(a.StringValue);
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

    private string Reconstruct(IEnumerable<UnvParseTreeNode> nodes)
    {
        var sb = new StringBuilder();
        foreach (var node in nodes)
            sb.Append(Reconstruct(node));
        return sb.ToString();
    }
}
