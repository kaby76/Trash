using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.XPath;
using Antlr4.StringTemplate;
using System.Text.RegularExpressions;

namespace dotnet_antlr
{

    public partial class Program
    {
        public Config config;
        public static string version = "3.0.4";
        public List<string> failed_modules = new List<string>();
        public IEnumerable<string> all_source_files = null;
        public string antlr_runtime_path;
        public string root_directory;
        public string target_specific_src_directory;
        public HashSet<string> tool_grammar_files = null;
        public HashSet<string> tool_src_grammar_files = null;
        public List<GrammarTuple> tool_grammar_tuples = null;
        public List<string> generated_files = null;
        public List<string> additional_grammar_files = null;
        public bool? case_fold = null;
        public string lexer_src_grammar_file_name = null;
        public string lexer_grammar_file_name = null;
        public string lexer_generated_file_name = null;
        public string parser_src_grammar_file_name = null;
        public string parser_grammar_file_name = null;
        public string parser_generated_file_name = null;
        public string suffix;

        string SetupFfn = ".dotnet-antlr.rc";
        public string target_directory;
        public string source_directory;

        public static LineTranslationType GetLineTranslationType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LineTranslationType.LF;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LineTranslationType.CRLF;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LineTranslationType.CR;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static EnvType GetEnvType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return EnvType.Unix;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return EnvType.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return EnvType.Mac;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static PathSepType GetPathSep()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return PathSepType.Colon;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PathSepType.Semi;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static string GetAntlrToolPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "~/Downloads/antlr-4.9.2-complete.jar";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return (home + "/Downloads/antlr-4.9.2-complete.jar").Replace('\\', '/');
            }
            throw new Exception("Cannot determine operating system!");
        }

        static string TargetName(TargetType target)
        {
            return target switch
            {
                TargetType.Antlr4cs => "Antlr4cs",
                TargetType.CSharp => "CSharp",
                TargetType.Java => "Java",
                TargetType.JavaScript => "JavaScript",
                TargetType.Cpp => "Cpp",
                TargetType.Dart => "Dart",
                TargetType.Go => "Go",
                TargetType.Php => "Php",
                TargetType.Python2 => "Python2",
                TargetType.Python3 => "Python3",
                TargetType.Swift => "Swift",
                _ => throw new NotImplementedException(),
            };
        }

        static string AllButTargetName(TargetType target)
        {
            var all_but = new List<string>() {
                "CSharp",
                "Java",
                "JavaScript",
                "Cpp",
                "Dart",
                "Go",
                "Php",
                "Python2",
                "Python3",
                "Swift",
                "Antlr4cs"};
            var filter = String.Join("/|", all_but.Where(t => t != TargetName(target)));
            return filter;
        }

        static void Main(string[] args)
        {
            try
            {
                new Program().MainInternal(args);
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.ToString());
                System.Environment.Exit(1);
            }
        }

        public void MainInternal(string[] args)
        {
            config = new Config();
            // Get default from OS, or just default.
            config.line_translation = GetLineTranslationType();
            config.env_type = GetEnvType();
            config.path_sep = GetPathSep();
            config.antlr_tool_path = GetAntlrToolPath();
            config.target = TargetType.CSharp;
            config.tool_grammar_files_pattern = "^(?!.*(/Generated|/target|/examples)).+g4$";
            config.output_directory = "Generated/";

            // Get any defaults from ~/.dotnet-antlr.rc
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (System.IO.File.Exists(home + Path.DirectorySeparatorChar + SetupFfn))
            {
                var jsonString = System.IO.File.ReadAllText(home + Path.DirectorySeparatorChar + SetupFfn);
                var o = JsonSerializer.Deserialize<Config>(jsonString);
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }
            }

            // Parse options, stop if we see a bogus option, or something like --help.
            var result = Parser.Default.ParseArguments<Config>(args);
            bool stop = false;
            result.WithNotParsed(
                o =>
                {
                    stop = true;
                });
            if (stop) return;

            result.WithParsed(o =>
            {
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }

                if (o.target != null && o.target == TargetType.Antlr4cs) config.name_space = "Test";
                if (o.name_space != null) config.name_space = o.name_space;
                if (config.target == TargetType.Antlr4cs || config.target == TargetType.CSharp)
                    config.flatten = true;
                if (o.all_source_pattern != null) config.all_source_pattern = config.all_source_pattern;
                else config.all_source_pattern = "^(?!.*(Generated/|target/|examples/|" + AllButTargetName((TargetType)config.target) + "/)).+(" + config.target switch
                {
                    TargetType.Antlr4cs => "[.]cs",
                    TargetType.CSharp => "[.]cs",
                    TargetType.Java => "[.]java",
                    TargetType.JavaScript => "[.]js",
                    TargetType.Cpp => "([.]h|[.]cpp)",
                    TargetType.Dart => "[.]dart",
                    TargetType.Go => "[.]go",
                    TargetType.Php => "[.]php",
                    TargetType.Python2 => "[.]py",
                    TargetType.Python3 => "[.]py",
                    TargetType.Swift => "[.]swift",
                    _ => throw new NotImplementedException(),
                } + "|[.]g4)$";
            });

            suffix = config.target switch
            {
                TargetType.CSharp => ".cs",
                TargetType.Java => ".java",
                TargetType.JavaScript => ".js",
                TargetType.Cpp => ".cpp",
                TargetType.Dart => ".dart",
                TargetType.Go => ".go",
                TargetType.Php => ".php",
                TargetType.Python2 => ".py",
                TargetType.Python3 => ".py",
                TargetType.Swift => ".swift",
                TargetType.Antlr4cs => ".cs",
                _ => throw new NotImplementedException(),
            };
            target_specific_src_directory = config.target switch
            {
                TargetType.CSharp => "CSharp",
                TargetType.Java => "Java",
                TargetType.JavaScript => "JavaScript",
                TargetType.Cpp => "Cpp",
                TargetType.Dart => "Dart",
                TargetType.Go => "Go",
                TargetType.Php => "Php",
                TargetType.Python2 => "Python2",
                TargetType.Python3 => "Python3",
                TargetType.Swift => "Swift",
                TargetType.Antlr4cs => "Antlr4cs",
                _ => throw new NotImplementedException(),
            };
            if (config.template_sources_directory != null)
                config.template_sources_directory = Path.GetFullPath(config.template_sources_directory);
            var path = Environment.CurrentDirectory;
            var cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            root_directory = cd;

            if (config.maven != null && (bool)config.maven)
            {
                FollowPoms(cd);
                if (failed_modules.Any())
                {
                    System.Console.WriteLine(String.Join(" ", failed_modules));
                    throw new Exception();
                }
            }
            else
            {
                // Find tool grammars.
                GeneratedNames();
                GenerateSingle(cd);
            }
        }

        public void FollowPoms(string cd)
        {
            Environment.CurrentDirectory = cd;
            System.Console.Error.WriteLine(cd);

            if (config.skip_list.Where(s => cd.Remove(cd.Length - 1).EndsWith(s)).Any())
            {
                System.Console.Error.WriteLine("Skipping.");
                return;
            }
            target_directory = System.IO.Path.GetFullPath(cd + Path.DirectorySeparatorChar + (string)config.output_directory);

            XmlTextReader reader = new XmlTextReader(cd + Path.DirectorySeparatorChar + @"pom.xml");
            reader.Namespaces = false;
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(reader.NameTable);

            // determine if this pom only directs for subdirectories.
            var sub_dirs = navigator
                .Select("//modules/module", nsmgr)
                .Cast<XPathNavigator>()
                .Select(t => t.Value)
                .ToList();
            if (sub_dirs.Any())
            {
                foreach (var sd in sub_dirs)
                {
                    try
                    {
                        FollowPoms(cd + sd + "/");
                    }
                    catch (Exception e)
                    {
                        var module = (cd + sd + "/").Replace(root_directory, "");
                        module = module.Remove(module.Length - 1);
                        System.Console.Error.WriteLine(
                            "Failed: "
                            + cd + sd + "/");
                        System.Console.Error.WriteLine(e.ToString());
                        failed_modules.Add(module);
                    }
                }
            }
            else
            {
                // Get antlr4-maven-plugin settings.
                var pom_includes = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4-maven-plugin\"]/configuration/includes/include", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                var pom_arguments = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4-maven-plugin\"]/configuration/arguments/argument", nsmgr)
                    .Cast<XPathNavigator>()
                    .Where(t => t.Value != "")
                    .Select(t => t.Value)
                    .ToList();
                var pom_source_directory = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4-maven-plugin\"]/configuration/sourceDirectory", nsmgr)
                    .Cast<XPathNavigator>()
                    .Where(t => t.Value != "")
                    .Select(t => t.Value)
                    .ToList();
                var pom_all_else = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4-maven-plugin\"]/configuration/*[not(self::sourceDirectory or self::arguments or self::includes or self::visitor or self::listener)]", nsmgr)
                    .Cast<XPathNavigator>()
                    .ToList();

                var pom_grammar_name = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4test-maven-plugin\"]/configuration/grammarName", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                var pom_lexer_name = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4test-maven-plugin\"]/configuration/lexerName", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                var pom_entry_point = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4test-maven-plugin\"]/configuration/entryPoint", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                var pom_package_name = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4test-maven-plugin\"]/configuration/packageName", nsmgr)
                    .Cast<XPathNavigator>()
                    .Where(t => t.Value != "")
                    .Select(t => t.Value)
                    .ToList();

                // grammarName is required. https://github.com/antlr/antlr4test-maven-plugin#grammarname
                if (!pom_grammar_name.Any())
                {
                    return;
                }
                // entryPoint required. https://github.com/antlr/antlr4test-maven-plugin#grammarname
                if (!pom_entry_point.Any())
                {
                    return;
                }
                // Check existance of files.
                foreach (var x in pom_includes)
                {
                    if (!new Domemtech.Globbing.Glob()
                     .RegexContents(x)
                     .Where(f => f is FileInfo)
                     .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                     .Any())
                    {
                        System.Console.Error.WriteLine("Error in pom.xml: <include>" + x + "</include> is for a file that does not exist.");
                        throw new Exception();
                    }
                }
                // Check package naem. If there's a package name without an args list for
                // the Antlr tool to generate for that package name, this will not be target
                // independent.
                if (pom_package_name.Any() && pom_package_name.First() != "")
                {
                    bool package_option = false;
                    foreach (var a in pom_arguments)
                    {
                        if (a == "-package")
                        {
                            package_option = true;
                            break;
                        }
                    }
                    if (!package_option)
                    {
                        System.Console.Error.WriteLine("You have a package reference for the parser in the test of it, "
                            + "but you don't have "
                            + "a package option on the Antlr tool generator configuration. Therfore, it's likely you have a package defined in your grammar directly, which is "
                            + "not target independent code, so it will not work other than for the default target.");
                    }
                }
                // Check all other config options in antlr4-maven-plugin configuration.
                if (pom_all_else.Any())
                {
                    System.Console.Error.WriteLine("Antlr4 maven config contains stuff that I don't understand.");
                }

                if (pom_source_directory.Any())
                {
                    source_directory = pom_source_directory
                        .First()
                        .Replace("${basedir}", "")
                        .Trim();
                    while (source_directory != "" && source_directory.StartsWith("/"))
                    {
                        source_directory = source_directory.Substring(1);
                    }
                    if (source_directory != "" && !source_directory.EndsWith("/"))
                    {
                        source_directory = source_directory + "/";
                    }
                }
                else
                {
                    source_directory = "";
                }

                config.parser_name = pom_grammar_name.First() + "Parser";
                for (; ; )
                {
                    // Probe for parser grammar. 
                    {
                        var parser_grammars_pattern =
                            "^((?!.*(Generated/|target/|examples/))("
                            + target_specific_src_directory + "/)(" + pom_grammar_name.First() + "|" + pom_grammar_name.First() + "Parser)).g4$";
                        var any =
                            new Domemtech.Globbing.Glob()
                                .RegexContents(parser_grammars_pattern)
                                .Where(f => f is FileInfo)
                                .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                                .ToList();
                        if (any.Any())
                        {
                            parser_src_grammar_file_name = any.First();
                            break;
                        }
                    }
                    {
                        var parser_grammars_pattern =
                            "^(" + pom_grammar_name.First() + "|" + pom_grammar_name.First() + "Parser).g4$";
                        var any =
                            new Domemtech.Globbing.Glob()
                                .RegexContents(parser_grammars_pattern)
                                .Where(f => f is FileInfo)
                                .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                                .ToList();
                        if (any.Any())
                        {
                            parser_src_grammar_file_name = any.First();
                            break;
                        }
                    }
                    {
                        var parser_grammars_pattern =
                            "^(" + source_directory + ")"
                            + "(" + pom_grammar_name.First() + "|" + pom_grammar_name.First() + "Parser).g4$";
                        var any =
                            new Domemtech.Globbing.Glob()
                                .RegexContents(parser_grammars_pattern)
                                .Where(f => f is FileInfo)
                                .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                                .ToList();
                        if (any.Any())
                        {
                            parser_src_grammar_file_name = any.First();
                            break;
                        }
                    }
                    throw new Exception("Cannot find the parser grammar (" + pom_grammar_name.First() + " | " + pom_grammar_name.First() + "Parser).g4");
                }
                parser_grammar_file_name =
                    (config.flatten != null && !(bool)config.flatten && pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + Path.GetFileName(parser_src_grammar_file_name);
                parser_generated_file_name =
                    (config.flatten != null && !(bool)config.flatten && pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + (string)config.parser_name + suffix;

                config.lexer_name = pom_lexer_name.Any() ? pom_lexer_name.First() : pom_grammar_name.First() + "Lexer";
                for (; ; )
                {
                    // Probe for lexer grammar. 
                    {
                        var lexer_grammars_pattern =
                            "^((?!.*(Generated/|target/|examples/))("
                            + target_specific_src_directory + "/)(" + pom_grammar_name.First() + "|" + pom_grammar_name.First() + "Lexer)).g4$";
                        var any =
                            new Domemtech.Globbing.Glob()
                                .RegexContents(lexer_grammars_pattern)
                                .Where(f => f is FileInfo)
                                .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                                .ToList();
                        if (any.Any())
                        {
                            lexer_src_grammar_file_name = any.First();
                            break;
                        }
                    }
                    {
                        var lexer_grammars_pattern =
                            "^(" + pom_grammar_name.First() + "|" + pom_grammar_name.First() + "Lexer).g4$";
                        var any =
                            new Domemtech.Globbing.Glob()
                                .RegexContents(lexer_grammars_pattern)
                                .Where(f => f is FileInfo)
                                .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                                .ToList();
                        if (any.Any())
                        {
                            lexer_src_grammar_file_name = any.First();
                            break;
                        }
                    }
                    {
                        var lexer_grammars_pattern =
                            "^(" + source_directory + ")"
                            + "(" + pom_grammar_name.First() + "|" + pom_grammar_name.First() + "Lexer).g4$";
                        var any =
                            new Domemtech.Globbing.Glob()
                                .RegexContents(lexer_grammars_pattern)
                                .Where(f => f is FileInfo)
                                .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                                .ToList();
                        if (any.Any())
                        {
                            lexer_src_grammar_file_name = any.First();
                            break;
                        }
                    }
                    throw new Exception("Cannot find the lexer grammar (" + pom_grammar_name.First() + " | " + pom_grammar_name.First() + "Lexer).g4)");
                }

                lexer_grammar_file_name =
                    (config.flatten != null && !(bool)config.flatten && pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + Path.GetFileName(lexer_src_grammar_file_name);
                lexer_generated_file_name =
                    (config.flatten != null && !(bool)config.flatten && pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + config.lexer_name + suffix;

                if (pom_package_name.Any()) config.name_space = pom_package_name.First();
                tool_src_grammar_files = new HashSet<string>()
                {
                    lexer_src_grammar_file_name,
                    parser_src_grammar_file_name
                };
                tool_grammar_tuples = new List<GrammarTuple>()
                {
                    new GrammarTuple(lexer_grammar_file_name, lexer_generated_file_name, config.lexer_name),
                    new GrammarTuple(parser_grammar_file_name, parser_generated_file_name, config.parser_name),
                };
                tool_grammar_files = new HashSet<string>()
                {
                    lexer_grammar_file_name,
                    parser_grammar_file_name
                };
                config.start_rule = pom_entry_point.First();
                generated_files = new List<string>()
                {
                    lexer_generated_file_name,
                    parser_generated_file_name,
                };
                GenerateSingle(cd);
            }
        }

        public void GenerateSingle(string cd)
        {
            try
            {
                // Create a directory containing target build files.
                Directory.CreateDirectory((string)config.output_directory);
            }
            catch (Exception)
            {
                throw;
            }


            // Include all other grammar files, but not if they are the main grammars.
            var additional_grammars_pattern = "^(?!.*(Generated/|target/|examples/|"
                + String.Join("|", tool_src_grammar_files)
                + ")).+g4$";
            additional_grammar_files = new Domemtech.Globbing.Glob()
                    .RegexContents(additional_grammars_pattern)
                    .Where(f => f is FileInfo)
                    .Select(f => f.FullName.Replace(cd, ""))
                    .ToList();

            // Find all source files.
            all_source_files = new Domemtech.Globbing.Glob()
                    .RegexContents(config.all_source_pattern)
                    .Where(f => f is FileInfo)
                    .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                    .ToList();

            AddSourceFiles.AddSource(this);
            GenFromTemplates(this);
        }

        IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        string[] ReadAllResourceLines(System.Reflection.Assembly a, string resourceName)
        {
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return EnumerateLines(reader).ToArray();
            }
        }

        string ReadAllResource(System.Reflection.Assembly a, string resourceName)
        {
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private void GenFromTemplates(Program p)
        {
            if (config.template_sources_directory == null)
            {
                System.Reflection.Assembly a = this.GetType().Assembly;
                // Load resource file that contains the names of all files in templates/ directory,
                // which were obtained by doing "cd templates/; find . -type f > files" at a Bash
                // shell.
                var orig_file_names = ReadAllResourceLines(a, "AntlrTemplating.templates.files");
                var regex_string = "^(?!.*(" + AllButTargetName((TargetType)config.target) + "/)).*$";
                var regex = new Regex(regex_string);
                var files_to_copy = orig_file_names.Where(f =>
                {
                    if (config.parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
                    if (f == "./files") return false;
                    var v = regex.IsMatch(f);
                    return v;
                }).ToList();
                var prefix_to_remove = "AntlrTemplating.templates.";
                var set = new HashSet<string>();
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    var m = Path.GetFileName(file);
                    var n = (p.config.name_space != null
                        && p.config.flatten != null && !(bool)p.config.flatten)
                        ? p.config.name_space.Replace('.', '/') : "";
                    var to = ((string)config.output_directory).Replace('\\', '/') + n + "/" + m;
                    from = prefix_to_remove + from.Replace('/', '.').Substring(2);
                    to = to.Replace('\\', '/');
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = ReadAllResource(a, from);
                    System.Console.Error.WriteLine("File is " + from);
                    Template t = new Template(content);
                    t.Add("cap_start_symbol", Cap(config.start_rule));
                    t.Add("cli_bash", (EnvType)p.config.env_type == EnvType.Unix);
                    t.Add("cli_cmd", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("has_name_space", p.config.name_space != null);
                    t.Add("lexer_name", config.lexer_name);
                    t.Add("name_space", p.config.name_space);
                    t.Add("parser_name", config.parser_name);
                    t.Add("path_sep_colon", p.config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", p.config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", config.start_rule);
                    t.Add("tool_grammar_files", this.tool_grammar_files);
                    t.Add("tool_grammar_tuples", this.tool_grammar_tuples);
                    t.Add("version", Program.version);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
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
                    .Select(f => f.FullName.Replace('\\','/'))
                    .Where(f =>
                    {
                        if (config.parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
                        if (f == "./files") return false;
                        return true;
                    }).ToList();
                var prefix_to_remove = config.template_sources_directory + '/';
                var set = new HashSet<string>();
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    var e = file.Substring(prefix_to_remove.Length);
                    var m = Path.GetFileName(file);
                    var n = (p.config.name_space != null
                        && p.config.flatten != null && !(bool)p.config.flatten)
                        ? p.config.name_space.Replace('.', '/') : "";
                    var to = ((string)config.output_directory).Replace('\\', '/') + n + "/" + m;
                    to = to.Replace('\\', '/');
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = File.ReadAllText(from);
                    System.Console.Error.WriteLine("File is " + from);
                    Template t = new Template(content);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
                    t.Add("cap_start_symbol", Cap(config.start_rule));
                    t.Add("cli_bash", (EnvType)p.config.env_type == EnvType.Unix);
                    t.Add("cli_cmd", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("has_name_space", p.config.name_space != null);
                    t.Add("lexer_name", config.lexer_name);
                    t.Add("name_space", p.config.name_space);
                    t.Add("parser_name", config.parser_name);
                    t.Add("path_sep_colon", p.config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", p.config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", config.start_rule);
                    t.Add("tool_grammar_files", this.tool_grammar_files);
                    t.Add("tool_grammar_tuples", this.tool_grammar_tuples);
                    t.Add("version", Program.version);
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
            }
        }

        static string Cap(string str)
        {
            if (str.Length == 0)
                return str;
            else if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            else
                return char.ToUpper(str[0]) + str.Substring(1);
        }

        public class GrammarTuple
        {
            public GrammarTuple(string grammar_file_name, string generated_file_name, string grammar_autom_name)
            {
                GrammarFileName = grammar_file_name;
                GeneratedFileName = generated_file_name;
                GrammarAutomName = grammar_autom_name;
            }
            public string GrammarFileName { get; set; }
            public string GeneratedFileName { get; set; }
            public string GrammarAutomName { get; set; }
        }

        public void AddCaseFold()
        {
            if (config.case_fold == null) return;
            StringBuilder sb = new StringBuilder();
            if (config.target == TargetType.CSharp)
            {
                sb.AppendLine(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version);
                if (config.name_space != null) sb.AppendLine("namespace " + config.name_space + @"
{");
                sb.Append(@"
/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using System;
using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime
{
    /// <summary>
    /// This class supports case-insensitive lexing by wrapping an existing
    /// <see cref=""ICharStream""/> and forcing the lexer to see either upper or
    /// lowercase characters. Grammar literals should then be either upper or
    /// lower case such as 'BEGIN' or 'begin'. The text of the character
    /// stream is unaffected. Example: input 'BeGiN' would match lexer rule
    /// 'BEGIN' if constructor parameter upper=true but getText() would return
    /// 'BeGiN'.
    /// </summary>
    public class CaseChangingCharStream : ICharStream
    {
        private ICharStream stream;
        private bool upper;

        /// <summary>
        /// Constructs a new CaseChangingCharStream wrapping the given <paramref name=""stream""/> forcing
        /// all characters to upper case or lower case.
        /// </summary>
        /// <param name=""stream"">The stream to wrap.</param>
        /// <param name=""upper"">If true force each symbol to upper case, otherwise force to lower.</param>
        public CaseChangingCharStream(ICharStream stream, bool upper)
        {
            this.stream = stream;
            this.upper = upper;
        }

        public int Index
        {
            get
            {
                return stream.Index;
            }
        }

        public int Size
        {
            get
            {
                return stream.Size;
            }
        }

        public string SourceName
        {
            get
            {
                return stream.SourceName;
            }
        }

        public void Consume()
        {
            stream.Consume();
        }

        [return: NotNull]
        public string GetText(Interval interval)
        {
            return stream.GetText(interval);
        }

        public int LA(int i)
        {
            int c = stream.LA(i);

            if (c <= 0)
            {
                return c;
            }

            char o = (char)c;

            if (upper)
            {
                return (int)char.ToUpperInvariant(o);
            }

            return (int)char.ToLowerInvariant(o);
        }

        public int Mark()
        {
            return stream.Mark();
        }

        public void Release(int marker)
        {
            stream.Release(marker);
        }

        public void Seek(int index)
        {
            stream.Seek(index);
        }
    }
}");
                if (config.name_space != null) sb.AppendLine("}");
                string fn = (string)config.output_directory + "CaseChangingCharStream.cs";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
            else if (config.target == TargetType.Java)
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
package " + config.name_space + @";

import org.antlr.v4.runtime.*;

public class ErrorListener extends ConsoleErrorListener
{
    public boolean had_error = false;
    
    @Override
    public void syntaxError(Recognizer<?, ?> recognizer,
        Object offendingSymbol,
        int line,
        int charPositionInLine,
        String msg,
        RecognitionException e)
    {
        had_error = true;
        super.syntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
    }
}
");
                string fn = (string)config.output_directory + "ErrorListener.java";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
        }

        public void CopyFile(string from, string to)
        {
            from = from.Replace('\\', '/');
            to = to.Replace('\\', '/');
            var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
            Directory.CreateDirectory(q);
            File.Copy(from, to, true);
        }

        public void GeneratedNames()
        {
            var cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            config.lexer_name = "";
            config.parser_name = "";
            parser_src_grammar_file_name = "";
            parser_grammar_file_name = "";
            parser_generated_file_name = "";
            lexer_src_grammar_file_name = "";
            lexer_grammar_file_name = "";
            lexer_generated_file_name = "";
            bool use_arithmetic = false;
            for (; ; )
            {
                // Probe for parser grammar. 
                {
                    var parser_grammars_pattern =
                        "^((?!.*(Generated/|target/|examples/))("
                        + target_specific_src_directory + "/)"
                        + "(.*|.*Parser)).g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(parser_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        parser_src_grammar_file_name = any.First();
                        break;
                    }
                }
                {
                    var parser_grammars_pattern =
                        "^(?!.*(Generated/|target/|examples/))(.*|.*Parser).g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(parser_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        parser_src_grammar_file_name = any.First();
                        break;
                    }
                }
                parser_src_grammar_file_name = "Arithmetic.g4";
                config.start_rule = "file_";
                use_arithmetic = true;
                break;
            }
            config.parser_name = Path.GetFileName(parser_src_grammar_file_name).Replace("Parser.g4", "").Replace(".g4", "") + "Parser";
            parser_grammar_file_name = Path.GetFileName(parser_src_grammar_file_name);
            parser_generated_file_name = config.parser_name + suffix;

            for (; ; )
            {
                // Probe for lexer grammar. 
                {
                    var lexer_grammars_pattern =
                           "^((?!.*(Generated/|target/|examples/))("
                        + target_specific_src_directory + "/)"
                        + "(.*|.*Lexer)).g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(lexer_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        lexer_src_grammar_file_name = any.First();
                        break;
                    }
                }
                {
                    var lexer_grammars_pattern =
                        "^(?!.*(Generated/|target/|examples/))(.*|.*Lexer).g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(lexer_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        lexer_src_grammar_file_name = any.First();
                        break;
                    }
                }
                lexer_src_grammar_file_name = "Arithmetic.g4";
                config.start_rule = "file_";
                use_arithmetic = true;
                break;
            }

            config.lexer_name = Path.GetFileName(lexer_src_grammar_file_name).Replace("Lexer.g4", "").Replace(".g4", "") + "Lexer";
            lexer_grammar_file_name = Path.GetFileName(lexer_src_grammar_file_name);
            lexer_generated_file_name = config.lexer_name + suffix;
            if (!use_arithmetic)
                tool_src_grammar_files = new HashSet<string>()
                    {
                        lexer_src_grammar_file_name,
                        parser_src_grammar_file_name
                    };
            else
                tool_src_grammar_files = new HashSet<string>();
            tool_grammar_files = new HashSet<string>()
                {
                    lexer_grammar_file_name,
                    parser_grammar_file_name
                };
            generated_files = new List<string>()
                {
                    lexer_generated_file_name,
                    parser_generated_file_name,
                };
            tool_grammar_tuples = new List<GrammarTuple>()
                {
                    new GrammarTuple(lexer_grammar_file_name, lexer_generated_file_name, config.lexer_name),
                    new GrammarTuple(parser_grammar_file_name, parser_generated_file_name, config.parser_name),
                };
            // lexer and parser are set if the grammar is partitioned.
            // rest is set if there are grammar is combined.
        }

        public static string Localize(LineTranslationType encoding, string code)
        {
            var is_win = code.Contains("\r\n");
            var is_mac = code.Contains("\n\r");
            var is_uni = code.Contains("\n") && !(is_win || is_mac);
            if (encoding == LineTranslationType.CRLF)
            {
                if (is_win) return code;
                else if (is_mac) return code.Replace("\n\r", "\r\n");
                else if (is_uni) return code.Replace("\n", "\r\n");
                else return code;
            }
            if (encoding == LineTranslationType.CR)
            {
                if (is_win) return code.Replace("\r\n", "\n\r");
                else if (is_mac) return code;
                else if (is_uni) return code.Replace("\n", "\n\r");
                else return code;
            }
            if (encoding == LineTranslationType.LF)
            {
                if (is_win) return code.Replace("\r\n", "\n");
                else if (is_mac) return code.Replace("\n\r", "\n");
                else if (is_uni) return code;
                else return code;
            }
            return code;
        }
    }
}