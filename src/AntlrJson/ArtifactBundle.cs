using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AntlrJson;

public sealed record Artifact(string Name, byte[] Data);

public static class ArtifactBundle
{
    public static IReadOnlyList<Artifact> Read(Stream input)
    {
        var artifacts = new List<Artifact>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        using var reader = new TarReader(input, leaveOpen: true);
        TarEntry entry;
        while ((entry = reader.GetNextEntry(copyData: false)) != null)
        {
            if (entry.EntryType == TarEntryType.Directory)
                continue;
            if (entry.EntryType != TarEntryType.RegularFile &&
                entry.EntryType != TarEntryType.V7RegularFile)
                throw new InvalidDataException($"Unsupported bundle entry type '{entry.EntryType}' for '{entry.Name}'.");

            var name = ValidateMemberName(entry.Name);
            if (!names.Add(name))
                throw new InvalidDataException($"Duplicate bundle member '{name}'.");
            using var data = new MemoryStream();
            entry.DataStream?.CopyTo(data);
            artifacts.Add(new Artifact(name, data.ToArray()));
        }
        return artifacts;
    }

    public static void Write(Stream output, IEnumerable<Artifact> artifacts)
    {
        var materialized = artifacts.ToArray();
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var artifact in materialized)
        {
            var name = ValidateMemberName(artifact.Name);
            if (!names.Add(name))
                throw new InvalidDataException($"Duplicate bundle member '{name}'.");
        }

        using var writer = new TarWriter(output, TarEntryFormat.Pax, leaveOpen: true);
        foreach (var artifact in materialized)
        {
            var name = ValidateMemberName(artifact.Name);
            var entry = new PaxTarEntry(TarEntryType.RegularFile, name)
            {
                DataStream = new MemoryStream(artifact.Data, writable: false),
                ModificationTime = DateTimeOffset.UnixEpoch
            };
            writer.WriteEntry(entry);
        }
    }

    public static string ValidateMemberName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.IndexOf('\0') >= 0)
            throw new InvalidDataException("Bundle member name is empty or contains NUL.");
        name = name.Replace('\\', '/');
        if (name.StartsWith('/') || Path.IsPathRooted(name) ||
            (name.Length >= 2 && char.IsLetter(name[0]) && name[1] == ':'))
            throw new InvalidDataException($"Bundle member '{name}' is absolute or drive-qualified.");
        var parts = name.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts.Any(p => p == "." || p == ".."))
            throw new InvalidDataException($"Bundle member '{name}' contains an unsafe path component.");
        return string.Join('/', parts);
    }

    public static IReadOnlyDictionary<string, string> RelativeInputNames(
        IEnumerable<string> inputNames, string baseDirectory = null)
    {
        var names = inputNames.Distinct(StringComparer.Ordinal).ToArray();
        if (names.Length == 0)
            return new Dictionary<string, string>();

        var explicitBase = string.IsNullOrWhiteSpace(baseDirectory)
            ? null
            : Path.GetFullPath(baseDirectory);
        var fullPaths = names.ToDictionary(
            name => name,
            name => Path.GetFullPath(name, explicitBase ?? Environment.CurrentDirectory),
            StringComparer.Ordinal);
        var root = explicitBase ?? CommonDirectory(fullPaths.Values);
        root = Path.GetFullPath(root);

        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in fullPaths)
        {
            var relative = Path.GetRelativePath(root, pair.Value).Replace('\\', '/');
            if (relative == ".." || relative.StartsWith("../", StringComparison.Ordinal))
                throw new InvalidDataException($"Input '{pair.Key}' is outside base directory '{root}'.");
            result.Add(pair.Key, ValidateMemberName(relative));
        }
        return result;
    }

    public static string ChangeExtension(string memberName, string extension)
    {
        var slash = memberName.LastIndexOf('/');
        var dot = memberName.LastIndexOf('.');
        if (dot <= slash)
            dot = memberName.Length;
        return memberName[..dot] + extension;
    }

    public static byte[] SerializeParsingResult(ParsingResultSet result, bool indented = false)
    {
        var options = ParsingResultOptions(indented);
        var arrayJson = JsonSerializer.Serialize(new[] { result }, options);
        return Encoding.UTF8.GetBytes(arrayJson[1..^1]);
    }

    public static ParsingResultSet DeserializeParsingResult(byte[] json)
    {
        var objectJson = Encoding.UTF8.GetString(json);
        var results = JsonSerializer.Deserialize<ParsingResultSet[]>("[" + objectJson + "]", ParsingResultOptions());
        if (results == null || results.Length != 1)
            throw new InvalidDataException("A .pt artifact must contain exactly one ParsingResultSet object.");
        return results[0];
    }

    private static JsonSerializerOptions ParsingResultOptions(bool indented = false)
    {
        var options = new JsonSerializerOptions { WriteIndented = indented, MaxDepth = 10000 };
        options.Converters.Add(new ParsingResultSetSerializer());
        return options;
    }

    private static string CommonDirectory(IEnumerable<string> fullPaths)
    {
        var directories = fullPaths.Select(path =>
            Path.GetDirectoryName(path) ?? Path.GetPathRoot(path) ?? Environment.CurrentDirectory).ToArray();
        var candidate = directories[0];
        while (directories.Any(directory => !IsWithin(candidate, directory)))
        {
            candidate = Path.GetDirectoryName(candidate);
            if (candidate == null)
                throw new InvalidDataException("Inputs do not share a common filesystem root.");
        }
        return candidate;
    }

    private static bool IsWithin(string root, string path)
    {
        var relative = Path.GetRelativePath(root, path);
        return relative != ".." && !relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal);
    }
}
