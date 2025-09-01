namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System;


    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trfirst.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            string path = config.ParserLocation != null ? config.ParserLocation
                : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            if (!Directory.Exists(path))
                throw new Exception("Path of parser does not exist.");
            path = Path.GetFullPath(path);
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path = path + "/";
            var fp = new TrashGlobbing.Glob(path)
                 .RegexContents("(Generated/)?bin/.*(?!ref)/Test.dll$")
                 .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                 .Select(f => f.FullName.Replace('\\', '/'))
                 .ToList();
            var exists = fp.Count == 1;
            if (config.ParserLocation != null && !exists)
            {
                System.Console.Error.WriteLine("Parser doesn't exist");
                System.Console.Error.WriteLine("fp " + String.Join(" ", fp));
                var is_generated_cs = new TrashGlobbing.Glob(path)
                              .RegexContents("(Generated/)*.cs")
                              .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                              .Select(f => f.FullName.Replace('\\', '/'))
                              .ToList();
                System.Console.Error.WriteLine("is_generated_cs = " + String.Join(" ", is_generated_cs));
                var is_generated_java = new TrashGlobbing.Glob(path)
                            .RegexContents("(Generated/)*.java")
                            .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                            .Select(f => f.FullName.Replace('\\', '/'))
                            .ToList();
                System.Console.Error.WriteLine("is_generated_java = " + String.Join(" ", is_generated_java));
                if (is_generated_cs.Count > 0 && is_generated_java.Count == 0)
                {
                    throw new Exception("-p specified, but the parser doesn't exist. Did you do a 'dotnet build'?");
                }
                else if (is_generated_cs.Count == 0 && is_generated_java.Count > 0)
                {
                    throw new Exception("-p specified, but the parser is a java program. Trparse doesn't work with that.");
                }
                else if (is_generated_java.Count > 0 && is_generated_cs.Count > 0)
                {
                    throw new Exception("-p specified, but the parser is a mix of C# and Java. Trparse works with only a C# target parser."); ;
                }
                else if (is_generated_java.Count == 0 && is_generated_cs.Count == 0)
                {
                    throw new Exception("-p specified, but I don't see any C# or Java. Trparse works with only a C# target parser, and it must be built."); ;
                }
                throw new Exception("-p specified, but the parser doesn't exist.");
            }
            string full_path = null;
            if (exists)
            {
                full_path = fp.First();
                exists = File.Exists(full_path);
            }
            {
                var grun = new Grun(config);
                grun.Run();
            }
        }
    }
}
