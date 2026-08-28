using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AntlrJson;

public sealed class ParsingResultInput
{
    internal ParsingResultInput(
        ParsingResultSet[] results,
        IReadOnlyList<Artifact> artifacts,
        IReadOnlyList<string> resultArtifactNames,
        bool isBundle)
    {
        Results = results;
        Artifacts = artifacts;
        ResultArtifactNames = resultArtifactNames;
        IsBundle = isBundle;
    }

    public ParsingResultSet[] Results { get; }
    public IReadOnlyList<Artifact> Artifacts { get; }
    public IReadOnlyList<string> ResultArtifactNames { get; }
    public bool IsBundle { get; }
}

/// <summary>
/// Reads legacy ParsingResultSet JSON or a PAX artifact bundle and writes
/// parsing results as a PAX artifact bundle.
/// </summary>
public static class ParsingResultIO
{
    public static ParsingResultInput Read(string fileName = null)
    {
        if (string.IsNullOrEmpty(fileName))
            return Read(Console.OpenStandardInput());
        using var input = File.OpenRead(fileName);
        return Read(input);
    }

    public static ParsingResultInput Read(Stream input)
    {
        using var buffer = new MemoryStream();
        input.CopyTo(buffer);
        var bytes = buffer.ToArray();
        if (LooksLikeJson(bytes))
        {
            var results = JsonSerializer.Deserialize<ParsingResultSet[]>(bytes, JsonOptions())
                ?? Array.Empty<ParsingResultSet>();
            return new ParsingResultInput(
                results, Array.Empty<Artifact>(), Array.Empty<string>(), false);
        }

        using var archive = new MemoryStream(bytes, writable: false);
        var artifacts = ArtifactBundle.Read(archive);
        var resultsList = new List<ParsingResultSet>();
        var names = new List<string>();
        foreach (var artifact in artifacts)
        {
            if (!artifact.Name.EndsWith(".pt", StringComparison.OrdinalIgnoreCase))
                continue;
            resultsList.Add(ArtifactBundle.DeserializeParsingResult(artifact.Data));
            names.Add(artifact.Name);
        }
        return new ParsingResultInput(
            resultsList.ToArray(), artifacts, names, true);
    }

    public static void WriteBundle(
        Stream output,
        ParsingResultInput input,
        IEnumerable<ParsingResultSet> results,
        bool indented = false)
    {
        var materialized = results.ToArray();
        var outputArtifacts = new List<Artifact>();
        if (input.IsBundle && materialized.Length == input.ResultArtifactNames.Count)
        {
            var resultIndex = 0;
            foreach (var artifact in input.Artifacts)
            {
                if (artifact.Name.EndsWith(".pt", StringComparison.OrdinalIgnoreCase))
                {
                    outputArtifacts.Add(new Artifact(
                        artifact.Name,
                        ArtifactBundle.SerializeParsingResult(
                            materialized[resultIndex++], indented)));
                }
                else
                {
                    outputArtifacts.Add(artifact);
                }
            }
        }
        else
        {
            if (input.IsBundle)
            {
                outputArtifacts.AddRange(input.Artifacts.Where(
                    artifact => !artifact.Name.EndsWith(".pt", StringComparison.OrdinalIgnoreCase)));
            }

            var names = ResultNames(materialized);
            for (var index = 0; index < materialized.Length; index++)
            {
                outputArtifacts.Add(new Artifact(
                    names[index],
                    ArtifactBundle.SerializeParsingResult(materialized[index], indented)));
            }
        }
        ArtifactBundle.Write(output, outputArtifacts);
    }

    public static void WriteFilteredBundle(
        Stream output,
        ParsingResultInput input,
        string extension,
        Func<ParsingResultSet, byte[]> render)
    {
        var outputArtifacts = new List<Artifact>();
        if (input.IsBundle)
        {
            var resultIndex = 0;
            foreach (var artifact in input.Artifacts)
            {
                if (artifact.Name.EndsWith(".pt", StringComparison.OrdinalIgnoreCase))
                {
                    outputArtifacts.Add(new Artifact(
                        ArtifactBundle.ChangeExtension(artifact.Name, extension),
                        render(input.Results[resultIndex++])));
                }
                else
                {
                    outputArtifacts.Add(artifact);
                }
            }
        }
        else
        {
            var names = ResultNames(input.Results);
            for (var index = 0; index < input.Results.Length; index++)
            {
                outputArtifacts.Add(new Artifact(
                    ArtifactBundle.ChangeExtension(names[index], extension),
                    render(input.Results[index])));
            }
        }
        ArtifactBundle.Write(output, outputArtifacts);
    }

    public static byte[] Utf8(string text) => new UTF8Encoding(false).GetBytes(text);

    public static JsonSerializerOptions JsonOptions(bool indented = false)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            MaxDepth = 10000
        };
        options.Converters.Add(new ParsingResultSetSerializer());
        return options;
    }

    private static bool LooksLikeJson(byte[] bytes)
    {
        var index = 0;
        if (bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
            index = 3;
        while (index < bytes.Length && char.IsWhiteSpace((char)bytes[index]))
            index++;
        return index < bytes.Length && (bytes[index] == (byte)'[' || bytes[index] == (byte)'{');
    }

    private static string[] ResultNames(IReadOnlyList<ParsingResultSet> results)
    {
        var used = new HashSet<string>(StringComparer.Ordinal);
        var names = new string[results.Count];
        for (var index = 0; index < results.Count; index++)
        {
            var fileName = results[index].FileName;
            var candidate = SafeRelativeName(fileName, index + 1);
            candidate = ArtifactBundle.ChangeExtension(candidate, ".pt");
            var unique = candidate;
            var suffix = 2;
            while (!used.Add(unique))
            {
                var stem = ArtifactBundle.ChangeExtension(candidate, "");
                unique = stem + "." + suffix++ + ".pt";
            }
            names[index] = unique;
        }
        return names;
    }

    private static string SafeRelativeName(string fileName, int index)
    {
        var fallback = "result-" + index;
        if (string.IsNullOrWhiteSpace(fileName))
            return fallback;
        var normalized = fileName.Replace('\\', '/');
        if (Path.IsPathRooted(fileName) ||
            (normalized.Length >= 2 && char.IsLetter(normalized[0]) && normalized[1] == ':'))
            normalized = Path.GetFileName(fileName);
        normalized = normalized.TrimStart('/');
        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(part => part != "." && part != "..")
            .ToArray();
        return parts.Length == 0 ? fallback : string.Join('/', parts);
    }
}
