using System;
using System.Collections.Generic;
using System.IO;

namespace dotnet_antlr
{
    class AddSourceFiles
    {
        public static void AddSource(Program p)
        {
            var cd = Environment.CurrentDirectory + "/";
            var set = new HashSet<string>();
            foreach (var path in p.all_source_files)
            {
                // Construct proper starting directory based on namespace.
                var f = path.Replace('\\', '/');
                var c = cd.Replace('\\', '/');
                var e = f.Replace(c, "");
                var m = Path.GetFileName(f);
                var n = p.config.name_space != null ? p.config.name_space.Replace('.', '/') : "";
                p.CopyFile(path, p.config.output_directory.Replace('\\', '/') + n + "/" + m);
            }
        }
    }
}
