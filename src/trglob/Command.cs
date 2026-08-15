using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        // Remove files whose path matches any negation pattern.
        foreach (var negPat in negativePatterns)
        {
            var regexStr = TrashGlobbing.Glob.GlobToRegex(negPat.Replace("\\", "/"));
            var regex = new Regex(regexStr);
            allFiles = allFiles.Where(f => !regex.IsMatch(f)).ToList();
        }

        allFiles.Sort();
        foreach (var y in allFiles)
        {
            System.Console.WriteLine(y);
        }

        return 0;
    }
}
