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
        public static string version = "3.1.1";
        public List<string> failed_modules = new List<string>();
        public List<string> all_source_files = null;
        public List<string> all_target_files = null;
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
        public string lexer_generated_include_file_name = null;
        public string parser_src_grammar_file_name = null;
        public string parser_grammar_file_name = null;
        public string parser_generated_file_name = null;
        public string parser_generated_include_file_name = null;
        public string suffix;
        string ignore_string = null;
        string ignore_file_name = ".dotnet-antlr-ignore";
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return PathSepType.Colon;
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "~/Downloads/antlr-4.9.2-complete.jar";
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static string TargetName(TargetType target)
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

        public static string AllButTargetName(TargetType target)
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

        public static void Main(string[] args)
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
            config.flatten = false;

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

            if (File.Exists(ignore_file_name))
            {
                var ignore = new StringBuilder();
                var lines = File.ReadAllLines(ignore_file_name);
                var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                ignore_string = string.Join("|", ignore_lines);
            }

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
                if (o.flatten != null) config.flatten = o.flatten;
                if (o.all_source_pattern != null) config.all_source_pattern = config.all_source_pattern;
                else config.all_source_pattern =
                    "^(?!.*(" +
                     (ignore_string != null ? ignore_string + "|" : "")
                     + "ignore/|Generated/|target/|examples/|"
                     + AllButTargetName((TargetType)config.target)
                     + "/)).+"
                     + "$";
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
                    // List out failed grammars. I really should say what failed,
                    // what succeeded, what skipped, but I don't. TODO.
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
                if (config.todo_list == null)
                {
                    // Do the old "skip_list" way.
                    if (config.skip_list.Where(s => cd.Remove(cd.Length - 1).EndsWith(s)).Any())
                    {
                        System.Console.Error.WriteLine("Skipping.");
                        return;
                    }
                }
                else
                {
                    var te = !(new Regex(config.todo_list).IsMatch(cd));
                    if (config.todo_list != null && te)
                    {
                        System.Console.Error.WriteLine("Skipping.");
                        return;
                    }
                }
                target_directory = System.IO.Path.GetFullPath(cd + Path.DirectorySeparatorChar + (string)config.output_directory);

                // Get antlr4-maven-plugin settings.
                var pom_includes = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4-maven-plugin\"]/configuration/includes/include", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                var pom_antlr_tool_args = navigator
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
                var pom_case_insensitive_type = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4test-maven-plugin\"]/configuration/caseInsensitiveType", nsmgr)
                    .Cast<XPathNavigator>()
                    .Where(t => t.Value != "")
                    .Select(t => t.Value)
                    .ToList();
                var pom_example_files = navigator
                    .Select("//plugins/plugin[artifactId=\"antlr4test-maven-plugin\"]/configuration/exampleFiles", nsmgr)
                    .Cast<XPathNavigator>()
                    .Where(t => t.Value != "")
                    .Select(t => t.Value)
                    .ToList();

                // grammarName is required. https://github.com/antlr/antlr4test-maven-plugin#grammarname
                if (!pom_grammar_name.Any())
                {
                    return;
                }
                config.grammar_name = pom_grammar_name.First();
                // Pom is a mess. There are many cases here.
                // -package arg specified; source top level
                //   => keep .g4 at top level, generate to directory
                //      corresponding to arg.
                if (pom_antlr_tool_args.Contains("-package"))
                {
                    var ns = pom_antlr_tool_args[pom_antlr_tool_args.IndexOf("-package") + 1];
                    config.name_space = ns;
                }
                else
                {
                    config.name_space = null;
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
                    foreach (var a in pom_antlr_tool_args)
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

                // Check existance of example files.
                if (pom_example_files.Any())
                {
                    config.example_files = pom_example_files.First();
                    if (!Directory.Exists(pom_example_files.First()))
                    {
                        System.Console.Error.WriteLine("Examples directory doesn't exist " + pom_example_files.First());
                    }
                }
                else
                {
                    config.example_files = "examples";
                }

                if (pom_source_directory.Any())
                {
                    source_directory = pom_source_directory
                        .First()
                        .Replace("${basedir}", "")
                        .Trim();
                    if (source_directory.StartsWith('/')) source_directory = source_directory.Substring(1);
                    if (source_directory != "" && !source_directory.EndsWith("/"))
                    {
                        source_directory = source_directory + "/";
                    }
                }
                else
                {
                    source_directory = "";
                }

                if (pom_case_insensitive_type.Any())
                {
                    if (pom_case_insensitive_type.First().ToUpper() == "UPPER")
                        config.case_insensitive_type = CaseInsensitiveType.Upper;
                    else if (pom_case_insensitive_type.First().ToUpper() == "LOWER")
                        config.case_insensitive_type = CaseInsensitiveType.Lower;
                    else config.case_insensitive_type = null;
                }
                else config.case_insensitive_type = null;

                // Check for existence of .dotnet-antlr-ignore file.
                // If there is one, read and create pattern of what to ignore.
                if (File.Exists(ignore_file_name))
                {
                    var ignore = new StringBuilder();
                    var lines = File.ReadAllLines(ignore_file_name);
                    var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                    ignore_string = string.Join("|", ignore_lines);
                }
                config.Package = (pom_package_name.Any() ? pom_package_name.First() : "");
                if (config.target == TargetType.Go) config.Package = "parser";
                config.antlr_tool_args = pom_antlr_tool_args.ToList();
                if (config.antlr_tool_args.Count() > 1)
                {
                    List<string> additional = config.antlr_tool_args.ToList();
                    config.antlr_tool_args = additional;
                    additional.Add("-o");
                    additional.Add(config.Package.Replace('.','/'));
                }

                config.fully_qualified_parser_name =
                    ((config.Package != null && config.Package != "") ? config.Package + '.' : "")
  //                  + (config.target == TargetType.Go ? "New" : "")
                    + pom_grammar_name.First()
                    + "Parser";
                config.fully_qualified_go_parser_name =
                    ((config.Package != null && config.Package != "") ? config.Package + '.' : "")
                    + (config.target == TargetType.Go ? "New" : "")
                    + pom_grammar_name.First()
                    + "Parser";
                for (; ; )
                {
                    // Probe for parser grammar. 
                    {
                        var parser_grammars_pattern =
                            "^"
                            + source_directory
                            + ((config.Package != null && config.Package != "") ? config.Package + '.' : "")
                            + "((?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/))("
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
                            "^"
                            + source_directory
                            + "(" + pom_grammar_name.First() + " |" + pom_grammar_name.First() + "Parser).g4$";
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
                if (pom_package_name.Any())
                {
                    parser_grammar_file_name =
                        pom_package_name.First().Replace('.', '/') + '/'
                        + Path.GetFileName(parser_src_grammar_file_name);
                }
                else if (source_directory != null && source_directory != "")
                {
                    parser_grammar_file_name = Path.GetFileName(parser_src_grammar_file_name);
                }
                else
                {
                    parser_grammar_file_name =
                        Path.GetFileName(parser_src_grammar_file_name);
                }
                parser_generated_file_name = (string)config.fully_qualified_parser_name.Replace('.','/') + suffix;
                parser_generated_include_file_name = (string)config.fully_qualified_parser_name.Replace('.', '/') + ".h";

                config.fully_qualified_lexer_name =
                    ((config.Package != null && config.Package != "") ? config.Package + '.' : "")
                    + (pom_lexer_name.Any() ? pom_lexer_name.First() : pom_grammar_name.First()
                    + "Lexer");
                config.fully_qualified_go_lexer_name =
                    ((config.Package != null && config.Package != "") ? config.Package + '.' : "")
                    + (config.target == TargetType.Go ? "New" : "")
                    + (pom_lexer_name.Any() ? pom_lexer_name.First() : pom_grammar_name.First()
                    + "Lexer");
                for (; ; )
                {
                    // Probe for lexer grammar. 
                    {
                        var lexer_grammars_pattern =
                            "^((?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/))("
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
                if (pom_package_name.Any())
                {
                    lexer_grammar_file_name =
                        pom_package_name.First().Replace('.', '/') + '/'
                        + Path.GetFileName(lexer_src_grammar_file_name);
                }
                else if (source_directory != null && source_directory != "")
                {
                    lexer_grammar_file_name = Path.GetFileName(lexer_src_grammar_file_name);
                }
                else
                {
                    lexer_grammar_file_name =
                        Path.GetFileName(lexer_src_grammar_file_name);
                }
                lexer_generated_file_name = config.fully_qualified_lexer_name.Replace('.','/') + suffix;
                lexer_generated_include_file_name = config.fully_qualified_lexer_name.Replace('.', '/') + ".h";

                if (pom_package_name.Any()) config.name_space = pom_package_name.First();
                
                tool_src_grammar_files = new HashSet<string>()
                {
                    lexer_grammar_file_name,
                    parser_grammar_file_name
                };
                tool_grammar_tuples = new List<GrammarTuple>()
                {
                    new GrammarTuple(lexer_grammar_file_name, lexer_generated_file_name, lexer_generated_include_file_name, config.fully_qualified_lexer_name),
                    new GrammarTuple(parser_grammar_file_name, parser_generated_file_name, parser_generated_include_file_name, config.fully_qualified_parser_name),
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
            AddSource();
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

        public void AddSource()
        {
            var cd = Environment.CurrentDirectory + "/";
            cd = cd.Replace('\\', '/');
            // Find all source files.
            this.all_target_files = new List<string>();
            this.all_source_files = new Domemtech.Globbing.Glob()
                    .RegexContents(this.config.all_source_pattern)
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();

            var set = new HashSet<string>();
            foreach (var path in this.all_source_files)
            {
                // Construct proper starting directory based on namespace.
                var from = path;
                var f = from.Substring(cd.Length);
                // First, remove source_directory.
                f = (
                        f.StartsWith(source_directory)
                        ? f.Substring((source_directory).Length)
                        : f
                        );
                // Now remove target directory.
                f = (
                        f.StartsWith(
                            Program.TargetName((TargetType)this.config.target) + '/')
                        ? f.Substring((Program.TargetName((TargetType)this.config.target) + '/').Length)
                        : f
                        );
                // Remove "src/main/java", a royal hangover from the Maven plugin.
                f = (
                        f.StartsWith("src/main/java/")
                        ? f.Substring("src/main/java".Length)
                        : f
                        );

                string to = null;
                if (Path.GetExtension(from) == ".g4" && config.name_space != null)
                {
                    to = this.config.output_directory
                        + config.name_space.Replace('.','/') + '/'
                        + f;
                }
                if (to == null)
                {
                    to = this.config.output_directory
                        + f;
                }
                System.Console.Error.WriteLine("Copying source file from "
                  + from
                  + " to "
                  + to);
                this.all_target_files.Add(to);
                this.CopyFile(from, to);
            }
        }

        private void GenFromTemplates(Program p)
        {
            var append_namespace = (!(p.config.target == TargetType.CSharp || p.config.target == TargetType.Antlr4cs));
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
                    if (config.fully_qualified_parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
                    if (f == "./files") return false;
                    var v = regex.IsMatch(f);
                    return v;
                }).ToList();
                var prefix_to_remove = "AntlrTemplating.templates.";
                System.Console.Error.WriteLine("Prefix to remove " + prefix_to_remove);
                var set = new HashSet<string>();
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    // copy the file straight up if it doesn't begin
                    // with target directory name. Otherwise,
                    // remove the target dir name.
                    var to = from.StartsWith("./" + TargetName((TargetType)p.config.target))
                        ? from.Substring(("./" + TargetName((TargetType)p.config.target)).Length + 1)
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
                    t.Add("additional_sources", p.all_target_files.Where(t =>
                        {
                            var ext = Path.GetExtension(t);
                            return suffix.Contains(ext);
                        })
                        .Select(t => t.Substring(p.config.output_directory.Length))
                        .ToList());
                    t.Add("antlr_tool_args", config.antlr_tool_args);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
                    t.Add("cap_start_symbol", Cap(config.start_rule));
                    t.Add("case_insensitive_type", config.case_insensitive_type);
                    t.Add("cli_bash", (EnvType)p.config.env_type == EnvType.Unix);
                    t.Add("cli_cmd", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("cmake_target", p.config.env_type == EnvType.Windows
                        ? "-G \"MSYS Makefiles\"" : "");
                    t.Add("example_files_unix", RemoveTrailingSlash(p.config.example_files.Replace('\\', '/')));
                    t.Add("example_files_win", RemoveTrailingSlash(p.config.example_files.Replace('/', '\\')));
                    t.Add("exec_name", p.config.env_type == EnvType.Windows ?
                        "Test.exe" : "Test");
                    t.Add("go_lexer_name", config.fully_qualified_go_lexer_name);
                    t.Add("go_parser_name", config.fully_qualified_go_parser_name);
                    t.Add("grammar_file", p.tool_grammar_files.First());
                    t.Add("grammar_name", config.grammar_name);
                    t.Add("has_name_space", p.config.name_space != null);
                    t.Add("is_combined_grammar", p.tool_grammar_files.Count() == 1);
                    t.Add("lexer_grammar_file", p.lexer_grammar_file_name);
                    t.Add("lexer_name", config.fully_qualified_lexer_name);
                    t.Add("name_space", p.config.name_space);
                    t.Add("parser_name", config.fully_qualified_parser_name);
                    t.Add("parser_grammar_file", p.parser_grammar_file_name);
                    t.Add("path_sep_colon", p.config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", p.config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", config.start_rule);
                    t.Add("temp_dir", p.config.env_type == EnvType.Windows
                        ? "c:/temp" : "/tmp");
                    t.Add("tool_grammar_files", this.tool_grammar_files);
                    t.Add("tool_grammar_tuples", this.tool_grammar_tuples);
                    t.Add("version", Program.version);
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
                        if (config.fully_qualified_parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
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
		            var to = e.StartsWith(TargetName((TargetType)p.config.target))
			             ? e.Substring((TargetName((TargetType)p.config.target)).Length + 1)
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
                    t.Add("additional_sources", p.all_target_files.Where(t =>
                        {
                            var ext = Path.GetExtension(t);
                            return suffix.Contains(ext);
                        })
                        .Select(t => t.Substring(p.config.output_directory.Length))
                        .ToList());
                    t.Add("antlr_tool_args", config.antlr_tool_args);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
                    t.Add("cap_start_symbol", Cap(config.start_rule));
                    t.Add("case_insensitive_type", config.case_insensitive_type);
                    t.Add("cli_bash", (EnvType)p.config.env_type == EnvType.Unix);
                    t.Add("cli_cmd", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("cmake_target", p.config.env_type == EnvType.Windows
                        ? "-G \"MSYS Makefiles\"" : "");
                    t.Add("example_files_unix", RemoveTrailingSlash(p.config.example_files.Replace('\\', '/')));
                    t.Add("example_files_win", RemoveTrailingSlash(p.config.example_files.Replace('/', '\\')));
                    t.Add("exec_name", p.config.env_type == EnvType.Windows ?
                      "Test.exe" : "Test");
                    t.Add("go_lexer_name", config.fully_qualified_go_lexer_name);
                    t.Add("go_parser_name", config.fully_qualified_go_parser_name);
                    t.Add("grammar_file", p.tool_grammar_files.First());
                    t.Add("grammar_name", config.grammar_name);
                    t.Add("has_name_space", p.config.name_space != null);
		            t.Add("is_combined_grammar", p.tool_grammar_files.Count() == 1);
                    t.Add("lexer_name", config.fully_qualified_lexer_name);
                    t.Add("lexer_grammar_file", p.lexer_grammar_file_name);
                    t.Add("name_space", p.config.name_space);
                    t.Add("parser_name", config.fully_qualified_parser_name);
                    t.Add("parser_grammar_file", p.parser_grammar_file_name);
                    t.Add("path_sep_colon", p.config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", p.config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", config.start_rule);
                    t.Add("temp_dir", p.config.env_type == EnvType.Windows
                        ? "c:/temp" : "/tmp");
                    t.Add("tool_grammar_files", this.tool_grammar_files);
                    t.Add("tool_grammar_tuples", this.tool_grammar_tuples);
                    t.Add("version", Program.version);
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
            }
        }

        static string RemoveTrailingSlash(string str)
        {
            for (; ; )
            {
                if (str.EndsWith('/'))
                    str = str.Substring(0, str.Length - 1);
                else if (str.EndsWith('\\'))
                    str = str.Substring(0, str.Length - 1);
                else break;
            }
            return str;
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
            public GrammarTuple(string grammar_file_name, string generated_file_name, string generated_include_file_name, string grammar_autom_name)
            {
                GrammarFileName = grammar_file_name;
                GeneratedFileName = generated_file_name;
                GeneratedIncludeFileName = generated_include_file_name;
                GrammarAutomName = grammar_autom_name;
            }
            public string GrammarFileName { get; set; }
            public string GeneratedFileName { get; set; }
            public string GeneratedIncludeFileName { get; set; }
            public string GrammarAutomName { get; set; }
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
            config.fully_qualified_lexer_name = "";
            config.fully_qualified_parser_name = "";
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
                        "^((?!.*(ignore/|Generated/|target/|examples/))("
                        + target_specific_src_directory + "/)"
                        + "((?!.*Lexer)|.*Parser)).g4$";
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
                        "^(?!.*(ignore/|Generated/|target/|examples/))((?!.*Lexer)|.*Parser).g4$";
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
            config.fully_qualified_parser_name = Path.GetFileName(parser_src_grammar_file_name).Replace("Parser.g4", "").Replace(".g4", "") + "Parser";
            parser_grammar_file_name = Path.GetFileName(parser_src_grammar_file_name);
            parser_generated_file_name = config.fully_qualified_parser_name + suffix;

            for (; ; )
            {
                // Probe for lexer grammar. 
                {
                    var lexer_grammars_pattern =
                           "^((?!.*(ignore/|Generated/|target/|examples/))("
                        + target_specific_src_directory + "/)"
                        + "((?!.*Parser)|.*Lexer)).g4$";
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
                        "^(?!.*(ignore/|Generated/|target/|examples/))((?!.*Parser)|.*Lexer).g4$";
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

            config.fully_qualified_lexer_name = Path.GetFileName(lexer_src_grammar_file_name).Replace("Lexer.g4", "").Replace(".g4", "") + "Lexer";
            lexer_grammar_file_name = Path.GetFileName(lexer_src_grammar_file_name);
            lexer_generated_file_name = config.fully_qualified_lexer_name + suffix;
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
                    new GrammarTuple(lexer_grammar_file_name, lexer_generated_file_name, lexer_generated_include_file_name, config.fully_qualified_lexer_name),
                    new GrammarTuple(parser_grammar_file_name, parser_generated_file_name, parser_generated_include_file_name, config.fully_qualified_parser_name),
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