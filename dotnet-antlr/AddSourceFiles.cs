using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dotnet_antlr
{
    class AddSourceFiles
    {
        public static void AddSource(Program p)
        {
            var cd = Environment.CurrentDirectory + "/";
            // Find all source files.
            p.all_source_files = new Domemtech.Globbing.Glob()
                    .RegexContents(p.config.all_source_pattern)
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                    .ToList();

            var set = new HashSet<string>();
            foreach (var path in p.all_source_files)
            {
                // Construct proper starting directory based on namespace.
                var f = path.Replace('\\', '/');
                var c = cd.Replace('\\', '/');
                var e = f.Replace(c, "");
                var m = Path.GetFileName(f);
                var n = (p.config.name_space != null && p.config.flatten != null
                    && !(bool)p.config.flatten) ? p.config.name_space.Replace('.', '/') : "";
                p.CopyFile(path, p.config.output_directory.Replace('\\', '/') + n + "/" + m);
            }
        }
    }
}
