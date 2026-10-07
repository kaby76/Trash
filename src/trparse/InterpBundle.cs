using System;
using System.IO;
using System.Linq;
using AntlrJson;

namespace Trash;

/// <summary>Stages interpreter tables from a PAX bundle for the path-based runtimes.</summary>
internal sealed class InterpBundle : IDisposable
{
    public string DirectoryPath { get; }

    private InterpBundle(string directoryPath) => DirectoryPath = directoryPath;

    public static InterpBundle Open(Stream input)
    {
        var tables = ArtifactBundle.Read(input)
            .Where(artifact => artifact.Name.EndsWith(".interp", StringComparison.OrdinalIgnoreCase) ||
                               artifact.Name.EndsWith(".tokens", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (!tables.Any(artifact => artifact.Name.EndsWith(".interp", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidDataException("--allstar without -L requires a PAX/tar bundle containing .interp files on stdin.");
        var names = new System.Collections.Generic.HashSet<string>(
            OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        foreach (var artifact in tables)
        {
            string name = artifact.Name;
            if (name.IndexOfAny(new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' }) >= 0 ||
                name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                Path.GetFileName(name) != name ||
                IsReservedDeviceName(name))
                throw new InvalidDataException($"Interpreter bundle member '{name}' is not a portable root-level filename.");
            if (!names.Add(name))
                throw new InvalidDataException($"Duplicate interpreter bundle member '{name}'.");
        }

        var directory = Directory.CreateTempSubdirectory("trparse-interp-").FullName;
        try
        {
            foreach (var artifact in tables)
                File.WriteAllBytes(Path.Combine(directory, artifact.Name), artifact.Data);
            return new InterpBundle(directory);
        }
        catch
        {
            Directory.Delete(directory, recursive: true);
            throw;
        }
    }

    public void Dispose() => Directory.Delete(DirectoryPath, recursive: true);

    private static bool IsReservedDeviceName(string name)
    {
        string stem = name.Split('.')[0].ToUpperInvariant();
        return stem is "CON" or "PRN" or "AUX" or "NUL" or
            "COM1" or "COM2" or "COM3" or "COM4" or "COM5" or "COM6" or "COM7" or "COM8" or "COM9" or
            "LPT1" or "LPT2" or "LPT3" or "LPT4" or "LPT5" or "LPT6" or "LPT7" or "LPT8" or "LPT9";
    }
}
