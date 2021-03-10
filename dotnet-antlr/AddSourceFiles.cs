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
                var n = p.@namespace != null ? p.@namespace.Replace('.', '/') : "";
                p.CopyFile(path, p.options.output_directory.Replace('\\', '/') + n + "/" + m);
            }
        }
    }
}
