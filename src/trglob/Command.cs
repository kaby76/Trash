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
        if (!cwd.EndsWith("\\")) cwd += "/";
        DirectoryInfo cwdi = new DirectoryInfo(cwd);
        foreach (var p in config.Files)
        {
            var glob = new TrashGlobbing.Glob();
            var z = p.Replace("\\", "/");
            var list_pp = glob
                .GlobContents(cwdi, z, true)
                .Select(f =>
                {
                    var n = f.FullName.Replace('\\', '/');
                    var r = n;
                    if (!config.Full)
                    {
                        r = System.IO.Path.GetRelativePath(cwd, n);
                        r = r.Replace('\\', '/');
                    }
                    return r;
                })
                .ToList();
            list_pp.Sort();
            list_pp = list_pp.Distinct().ToList();
            foreach (var y in list_pp)
            {
                System.Console.WriteLine(y);
            }
        }

        return 0;
    }
}
