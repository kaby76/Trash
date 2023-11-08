namespace Trash
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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
            List<string> merged_list = new List<string>();
            foreach (var z in config.Files)
            {
                var glob = new TrashGlobbing.Glob();
                var list_pp = glob
                    .RegexContents(TrashGlobbing.Glob.GlobToRegex(z), true)
                    .Where(f => f is FileInfo)
                    .Select(f => f.FullName.Replace('\\', '/'))
	                .ToList();
                foreach (var y in list_pp)
                {
                    merged_list.Add(y);
                }
            }
            foreach (var z in merged_list)
            {
                System.Console.WriteLine(z);
            }
            return 0;
        }
    }
}
