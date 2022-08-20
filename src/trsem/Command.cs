namespace Trash
{
    using Antlr4.StringTemplate;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsem.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public string ignore_string = null;
        public string ignore_file_name = ".trgen-ignore";
        public int Execute(Config config)
        {
            var suffix = config.target switch
            {
                TargetType.Antlr4cs => ".cs",
                TargetType.Cpp => ".cpp",
                TargetType.CSharp => ".cs",
                TargetType.Dart => ".dart",
                TargetType.Go => ".go",
                TargetType.Java => ".java",
                TargetType.JavaScript => ".js",
                TargetType.Php => ".php",
                TargetType.Python2 => ".py",
                TargetType.Python3 => ".py",
                TargetType.Swift => ".swift",
                TargetType.TypeScript => ".ts",
                _ => throw new NotImplementedException(),
            };
            var target_specific_src_directory = config.target switch
            {
                TargetType.Antlr4cs => "Antlr4cs",
                TargetType.Cpp => "Cpp",
                TargetType.CSharp => "CSharp",
                TargetType.Dart => "Dart",
                TargetType.Go => "Go",
                TargetType.Java => "Java",
                TargetType.JavaScript => "JavaScript",
                TargetType.Php => "Php",
                TargetType.Python2 => "Python2",
                TargetType.Python3 => "Python3",
                TargetType.Swift => "Swift",
                TargetType.TypeScript => "TypeScript",
                _ => throw new NotImplementedException(),
            };
            if (config.template_sources_directory != null)
                config.template_sources_directory = Path.GetFullPath(config.template_sources_directory);
            // Check for existence of .trgen-ignore file.
            // If there is one, read and create pattern of what to ignore.
            if (File.Exists(ignore_file_name))
            {
                var ignore = new StringBuilder();
                var lines = File.ReadAllLines(ignore_file_name);
                var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                ignore_string = string.Join("|", ignore_lines);
            }
            var cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            var source_directory = cd;
            if (source_directory != "" && !source_directory.EndsWith("/"))
            {
                source_directory = source_directory + "/";
            }
            List<string> files = new List<string>();
            // Probe for .sem files.
            for (; ; )
            {
                {
                    var pattern =
                    "^((?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/))("
                    + target_specific_src_directory + "/)"
                    + ").sem$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        files.AddRange(any);
                        break;
                    }
                }
                {
                    var pattern =
                        "^(?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/|Lexer)).*[.]sem$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        files.AddRange(any);
                        break;
                    }
                }
                throw new Exception("Cannot find .sem file.");
            }
            foreach (var file in files)
            {
                var exists = File.Exists(file);
                if (!exists)
                {
                    continue;
                }
                var jsonString = System.IO.File.ReadAllText(file);
                var o = JsonSerializer.Deserialize<Sem>(jsonString);

                // A classifier has several forms, but the most basic is just
                // the node name of the leaf node for a token.
                // For an Antlr4 grammar, the classifier is just RULE_REF.
                // But, if we are interested in refining the classifiers to
                // differentiate between defs and refs, we refine this:
                //   (def-nonterminal)  RULE_REF[parent::parserRuleSpec]
                // In order to be efficiently implemented, all classifiers
                // are computed in one pass of a visitor tree walk.
                GenFromTemplates(config, o);
            }
            return 0;
        }

        public static void CopyFile(string from, string to)
        {
            from = from.Replace('\\', '/');
            to = to.Replace('\\', '/');
            var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
            Directory.CreateDirectory(q);
            File.Copy(from, to, true);
        }

        private static void GenFromTemplates(Config config, Sem sem)
        {
            var append_namespace = (!(config.target == TargetType.CSharp || config.target == TargetType.Antlr4cs));
            if (config.template_sources_directory == null)
            {
                System.Reflection.Assembly a = typeof(Command).Assembly;
                // Load resource file that contains the names of all files in templates/ directory,
                // which were obtained by doing "cd templates/; find . -type f > files" at a Bash
                // shell.
                var orig_file_names = ReadAllResourceLines(a, "trsem.templates.files");
                var regex_string = "^(?!.*(" + AllButTargetName((TargetType)config.target) + "/)).*$";
                var regex = new Regex(regex_string);
                var files_to_copy = orig_file_names.Where(f =>
                {
                    if (f == "./files") return false;
                    var v = regex.IsMatch(f);
                    return v;
                }).ToList();
                var prefix_to_remove = "trsem.templates.";
                System.Console.Error.WriteLine("Prefix to remove " + prefix_to_remove);
                var set = new HashSet<string>();
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    // copy the file straight up if it doesn't begin
                    // with target directory name. Otherwise,
                    // remove the target dir name.
                    var to = from.StartsWith("./" + TargetName((TargetType)config.target))
                        ? from.Substring(("./" + TargetName((TargetType)config.target)).Length + 1)
                        : from.Substring(2);
                    to = ((string)config.output_directory).Replace('\\', '/') + to;
                    from = prefix_to_remove + from.Replace('/', '.').Substring(2);
                    to = to.Replace('\\', '/');
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = ReadAllResource(a, from);
                    System.Console.Error.WriteLine("Rendering template file from "
                        + from
                        + " to "
                        + to);
                    Template t = new Template(content);
                    t.Add("classifiers", sem.Classes);
                    t.Add("ParserName", "ANTLRv4Parser");
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
            }
            else
            {
                var regex_string = "^(?!.*(files|" + AllButTargetName((TargetType)config.target) + "/)).*$";
                var files_to_copy = new Domemtech.Globbing.Glob(config.template_sources_directory)
                    .RegexContents(regex_string)
                    .Where(f =>
                    {
                        if (f.Attributes.HasFlag(FileAttributes.Directory)) return false;
                        if (f is DirectoryInfo) return false;
                        return true;
                    })
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .Where(f =>
                    {
                        if (f == "./files") return false;
                        return true;
                    }).ToList();
                var prefix_to_remove = config.template_sources_directory + '/';
                prefix_to_remove = prefix_to_remove.Replace("\\", "/");
                prefix_to_remove = prefix_to_remove.Replace("//", "/");
                System.Console.Error.WriteLine("Prefix to remove " + prefix_to_remove);
                var set = new HashSet<string>();
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    var e = file.Substring(prefix_to_remove.Length);
                    var to = e.StartsWith(TargetName((TargetType)config.target))
                         ? e.Substring((TargetName((TargetType)config.target)).Length + 1)
                         : e;
                    to = ((string)config.output_directory).Replace('\\', '/') + to;
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = File.ReadAllText(from);
                    System.Console.Error.WriteLine("Rendering template file from "
                        + from
                        + " to "
                        + to);
                    Template t = new Template(content);
           //         t.Add("antlr_encoding", config.antlr_encoding);
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
            }
        }

        static string ReadAllResource(System.Reflection.Assembly a, string resourceName)
        {
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        static string[] ReadAllResourceLines(System.Reflection.Assembly a, string resourceName)
        {
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return EnumerateLines(reader).ToArray();
            }
        }

        static string AllButTargetName(TargetType target)
        {
            var all_but = new List<string>() {
                "Antlr4cs",
                "Cpp",
                "CSharp",
                "Dart",
                "Go",
                "Java",
                "JavaScript",
                "Php",
                "Python2",
                "Python3",
                "Swift",
                "TypeScript",
            };
            var filter = String.Join("/|", all_but.Where(t => t != TargetName(target)));
            return filter;
        }

        static IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        static string TargetName(TargetType target)
        {
            return target switch
            {
                TargetType.Antlr4cs => "Antlr4cs",
                TargetType.Cpp => "Cpp",
                TargetType.CSharp => "CSharp",
                TargetType.Dart => "Dart",
                TargetType.Go => "Go",
                TargetType.Java => "Java",
                TargetType.JavaScript => "JavaScript",
                TargetType.Php => "Php",
                TargetType.Python2 => "Python2",
                TargetType.Python3 => "Python3",
                TargetType.Swift => "Swift",
                TargetType.TypeScript => "TypeScript",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
