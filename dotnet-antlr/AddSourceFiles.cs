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
            cd = cd.Replace('\\', '/');
            // Find all source files.
            p.all_target_files = new List<string>();
            p.all_source_files = new Domemtech.Globbing.Glob()
                    .RegexContents(p.config.all_source_pattern)
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();

            var set = new HashSet<string>();
            foreach (var path in p.all_source_files)
            {
                // Construct proper starting directory based on namespace.
                var from = path;
                var f = from.Substring(cd.Length);
                var to =  from.Substring(cd.Length);
                to = p.config.output_directory
                    + (
                        f.StartsWith(
                            Program.TargetName((TargetType)p.config.target) + '/')
                        ? f.Substring((Program.TargetName((TargetType)p.config.target) + '/').Length)
                        : f
                        );

                System.Console.Error.WriteLine("Copying source file from "
                  + from
                  + " to "
                  + to);
                p.all_target_files.Add(to);
                p.CopyFile(from, to);
            }
        }
    }
}
