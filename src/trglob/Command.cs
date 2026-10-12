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

        // Each positional argument may contain a semicolon-separated list,
        // as used by desc.xml <inputs>. Exclusions apply to the entire union.
        var patterns = config.Files
            .SelectMany(p => p.Split(';', System.StringSplitOptions.TrimEntries |
                System.StringSplitOptions.RemoveEmptyEntries))
            .SelectMany(ExpandBraces)
            .ToList();
        var positivePatterns = patterns.Where(p => !p.StartsWith("!")).ToList();
        var negativePatterns = patterns
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

    // GlobContents splits paths at '/', so a brace alternative containing
    // a directory separator must be expanded before it reaches that matcher.
    private static IEnumerable<string> ExpandBraces(string pattern)
    {
        int open = -1;
        bool inClass = false;
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == '[' && !inClass) inClass = true;
            else if (pattern[i] == ']' && inClass) inClass = false;
            else if (pattern[i] == '{' && !inClass)
            {
                open = i;
                break;
            }
        }
        if (open < 0)
        {
            yield return pattern;
            yield break;
        }

        int depth = 0;
        int close = -1;
        var alternatives = new List<string>();
        int start = open + 1;
        inClass = false;
        for (int i = open; i < pattern.Length; i++)
        {
            if (pattern[i] == '[' && !inClass)
            {
                inClass = true;
                continue;
            }
            if (pattern[i] == ']' && inClass)
            {
                inClass = false;
                continue;
            }
            if (inClass) continue;
            if (pattern[i] == '{') depth++;
            else if (pattern[i] == '}')
            {
                if (--depth == 0)
                {
                    alternatives.Add(pattern.Substring(start, i - start));
                    close = i;
                    break;
                }
            }
            else if (pattern[i] == ',' && depth == 1)
            {
                alternatives.Add(pattern.Substring(start, i - start));
                start = i + 1;
            }
        }

        if (close < 0)
        {
            yield return pattern;
            yield break;
        }

        foreach (var alternative in alternatives)
        {
            var expanded = pattern.Substring(0, open) + alternative +
                pattern.Substring(close + 1);
            foreach (var result in ExpandBraces(expanded))
                yield return result;
        }
    }
}
