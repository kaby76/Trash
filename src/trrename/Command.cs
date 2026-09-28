using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XQuery.DataModel;
using XQuery.Parser;

namespace Trash;

class Command
{
    public string Help()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("trrename.readme.md")!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Execute(Config config)
    {
        var renameMap = ReadRenameMap(config);
        var input = ParsingResultIO.Read(config.File);
        if (renameMap.Count == 0)
        {
            using var unchanged = Console.OpenStandardOutput();
            ParsingResultIO.WriteBundle(unchanged, input, input.Results, config.Format);
            return;
        }

        // Evaluate every source spelling once. A -> B, B -> C must not turn
        // original A references into C, regardless of map order.
        var module = new XQueryParser(BuildQuery(config.Expr, renameMap)).ParseModule();
        var results = new List<ParsingResultSet>();
        foreach (var result in input.Results)
        {
            var context = new EvaluationContext()
                .WithContextItem(AdapterDocument.Build(result.Nodes));
            new ParseTreeUpdateEvaluator(context).EvaluateModule(module);
            results.Add(new ParsingResultSet
            {
                FileName = result.FileName,
                Nodes = result.Nodes,
                Lexer = result.Lexer,
                Parser = result.Parser,
                StartSymbol = result.StartSymbol,
                MetaStartSymbol = result.MetaStartSymbol,
            });
            if (config.Verbose)
                Console.Error.WriteLine($"Renamed symbols in {result.FileName}.");
        }

        using var output = Console.OpenStandardOutput();
        ParsingResultIO.WriteBundle(output, input, results, config.Format);
    }

    private static Dictionary<string, string> ReadRenameMap(Config config)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        var inline = config.RenameMapOption != null
            ? new[] { config.RenameMapOption }
            : config.RenameMap?.ToArray() ?? [];
        var pairs = inline.Length != 0
            ? inline.SelectMany(item => item.Split(';', StringSplitOptions.RemoveEmptyEntries))
            : config.RenameMapFile != null
                ? File.ReadAllText(config.RenameMapFile)
                    .Split([';', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                : [];
        foreach (var pair in pairs)
        {
            var comma = pair.IndexOf(',');
            if (comma <= 0 || comma != pair.LastIndexOf(',') || comma == pair.Length - 1)
                throw new InvalidDataException($"Invalid rename pair '{pair}'; expected oldName,newName.");
            var oldName = pair[..comma].Trim();
            var newName = pair[(comma + 1)..].Trim();
            if (oldName.Length == 0 || newName.Length == 0)
                throw new InvalidDataException($"Invalid rename pair '{pair}'; names must not be empty.");
            map[oldName] = newName;
        }
        return map;
    }

    private static string BuildQuery(string expression, IReadOnlyDictionary<string, string> map)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("The node-selection XQuery expression is empty.");
        var query = new StringBuilder("for $node in (").Append(expression)
            .Append(")\nreturn ");
        foreach (var (oldName, newName) in map)
            query.Append("if (string($node) = ").Append(Literal(oldName))
                .Append(") then replace value of node $node with ")
                .Append(Literal(newName)).Append("\nelse ");
        return query.Append("()").ToString();
    }

    private static string Literal(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";
}
