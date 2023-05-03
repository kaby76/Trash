using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using trcover;

namespace Trash
{
    using AntlrJson;
    using ParseTreeEditing.UnvParseTreeDOM;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trconvert.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            int exit_code = 0;

            // There are two ways to do this. One is a
            // bootstrapped method using LanguageServer, the
            // other is by using the generated code, with the loading
            // and running of the parser. We need to determine which way.
            // If Generated/exists, and it's a CSharp program that compiles,
            // use that to parse the input.
            //
            // If Generated-CSharp/ does not exist, then parse as Antlr4.
            // If Type=="gen", then parse using Generated/.
            // If Type=="antlr2", then parse using Antlr2.
            // Etc.

            string path = config.ParserLocation != null
                ? config.ParserLocation
                : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            if (!Directory.Exists(path))
                throw new Exception("Path of parser does not exist.");
            path = Path.GetFullPath(path);
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path = path + "/";
            var fp = new TrashGlobbing.Glob(path)
                .RegexContents("(Generated-CSharp/)?bin/.*(?!ref)/" + config.Dll + ".dll$")
                .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                .Select(f => f.FullName.Replace('\\', '/'))
                .ToList();
            var exists = fp.Count == 1;
            if (config.ParserLocation != null && !exists)
            {
                System.Console.Error.WriteLine("Parser doesn't exist");
                System.Console.Error.WriteLine("fp " + String.Join(" ", fp));
                var is_generated_cs = new TrashGlobbing.Glob(path)
                    .RegexContents("(Generated-CSharp/)*.cs")
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
                System.Console.Error.WriteLine("is_generated_cs = " + String.Join(" ", is_generated_cs));
                var is_generated_java = new TrashGlobbing.Glob(path)
                    .RegexContents("(Generated-Java/)*.java")
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
                    throw new Exception(
                        "-p specified, but the parser is a java program. Trparse doesn't work with that.");
                }
                else if (is_generated_java.Count > 0 && is_generated_cs.Count > 0)
                {
                    throw new Exception(
                        "-p specified, but the parser is a mix of C# and Java. Trparse works with only a C# target parser.");
                    ;
                }
                else if (is_generated_java.Count == 0 && is_generated_cs.Count == 0)
                {
                    throw new Exception(
                        "-p specified, but I don't see any C# or Java. Trparse works with only a C# target parser, and it must be built.");
                    ;
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
            if (config.Type == null && !exists)
            {
                // Come up with a type to parse.
                // If there is no generated parser, then use one of the built-in parsers.
                // Hack for now.
                // Take suffix of first file, get type of parser,
                // then use that to determine parse.
                var ext = Path.GetExtension(config.Files.First());
                var parser_type = ext switch
                {
                    ".g4" => "ANTLRv4",
                    ".g3" => "ANTLRv3",
                    ".g2" => "ANTLRv2",
                    ".gram" => "pegen_v3_10",
                    ".rex" => "rex",
                    ".y" => "Bison",
                    _ => throw new Exception("Unknown file extension, cannot load in a built-in parser.")
                };
                config.Type = parser_type;
            }

            Environment.ExitCode = grun.Run(config.Type);
        }
    }
}

