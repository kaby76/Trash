using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trglob.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public int Execute(Config config)
    {
        string cwd = System.Environment.CurrentDirectory;
        cwd = cwd.Replace("\\", "/");
        if (!cwd.EndsWith("/")) cwd += "/";
        DirectoryInfo cwdi = new DirectoryInfo(cwd);

        // Separate positive patterns from negation patterns (prefix '!').
        var positivePatterns = config.Files.Where(p => !p.StartsWith("!")).ToList();
        var negativePatterns = config.Files
            .Where(p => p.StartsWith("!"))
            .Select(p => p.Substring(1))
            .ToList();

        // Collect files from all positive patterns.
        var allFiles = new List<string>();
        foreach (var p in positivePatterns)
        {
            var glob = new TrashGlobbing.Glob();
            var z = p.Replace("\\", "/");
            var matches = glob
                .GlobContents(cwdi, z, true)
                .Select(f =>
                {
                    var n = f.FullName.Replace('\\', '/');
                    if (!config.Full)
                    {
                        var r = System.IO.Path.GetRelativePath(cwd, n);
                        return r.Replace('\\', '/');
                    }
                    return n;
                });
            allFiles.AddRange(matches);
        }

        allFiles = allFiles.Distinct().ToList();

        // Remove files matching any negation pattern, using GlobContents so that
        // inclusion and exclusion share the same matching semantics (segment-aware,
        // ** handling, separator normalisation, etc.).
        foreach (var negPat in negativePatterns)
        {
            var negGlob = new TrashGlobbing.Glob();
            var z = negPat.Replace("\\", "/");
            var negMatches = negGlob
                .GlobContents(cwdi, z, true)
                .Select(f =>
                {
                    var n = f.FullName.Replace('\\', '/');
                    if (!config.Full)
                    {
                        var r = System.IO.Path.GetRelativePath(cwd, n);
                        return r.Replace('\\', '/');
                    }
                    return n;
                })
                .ToHashSet();
            allFiles = allFiles.Where(f => !negMatches.Contains(f)).ToList();
        }

        allFiles.Sort();
        foreach (var y in allFiles)
        {
            System.Console.WriteLine(y);
        }

        return 0;
    }
}
