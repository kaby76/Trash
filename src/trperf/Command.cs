using Antlr4.Runtime.Tree;
using AntlrJson;
using LanguageServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Workspaces;

namespace Trash
{
    class Command
    {
        public Workspace _workspace { get; set; } = new Workspace();

        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trperf.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            // There are two ways to do this. One is a
            // bootstrapped method using LanguageServer, the
            // other is by using the generated code, with the loading
            // and running of the parser. We need to determine which way.
            // If Generated/exists, and it's a CSharp program that compiles,
            // use that to parse the input.
            //
            // If Generated/ does not exist, then parse as Antlr4.
            // If Type=="gen", then parse using Generated/.
            // If Type=="antlr2", then parse using Antlr2.
            // Etc.

            string path = config.ParserLocation != null ? config.ParserLocation
                : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            if (!Directory.Exists(path))
                throw new Exception("Path of parser does not exist.");
            path = Path.GetFullPath(path);
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path = path + "/";
            var fp = new Domemtech.Globbing.Glob(path)
                .RegexContents("(Generated/)?bin/.*/Test.dll$")
                .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                .Select(f => f.FullName.Replace('\\', '/'))
                .ToList();
            var exists = fp.Count == 1;
            if (config.ParserLocation != null && !exists)
            {
                var is_generated_cs = new Domemtech.Globbing.Glob(path)
                    .RegexContents("(Generated/)*.cs")
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
                var is_generated_java = new Domemtech.Globbing.Glob(path)
                    .RegexContents("(Generated/)*.java")
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
                if (is_generated_cs.Count > 0 && is_generated_java.Count == 0)
                {
                    throw new Exception("-p specified, but the parser doesn't exist. Did you do a 'dotnet build'?");
                } else if (is_generated_cs.Count == 0 && is_generated_java.Count > 0)
                {
                    throw new Exception("-p specified, but the parser is a java program. Trperf doesn't work with that.");
                } else if (is_generated_java.Count > 0 && is_generated_cs.Count > 0)
                {
                    throw new Exception("-p specified, but the parser is a mix of C# and Java. Trperf works with only a C# target parser."); ;
                }
                else if (is_generated_java.Count == 0 && is_generated_cs.Count == 0)
                {
                    throw new Exception("-p specified, but I don't see any C# or Java. Trperf works with only a C# target parser, and it must be built."); ;
                }
                throw new Exception("-p specified, but the parser doesn't exist.");
            }
            string full_path = null;
            if (exists)
            {
                full_path = fp.First();
                exists = File.Exists(full_path);
            }

            var grun = new Grun(config);
            grun.Run();
        }
    }
}
