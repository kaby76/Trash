namespace dotnet_antlr
{
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

    public class Program
    {
        public static string version = "2.2";
        public List<string> failed_modules = new List<string>();
        public IEnumerable<string> all_source_files = null;
        public LineTranslationType line_translation;
        public EnvType env_type;
        public PathSepType path_sep_type;
        public string antlr_tool_path;
        public string antlr_runtime_path;
        public bool antlr4cs = false;
        public TargetType target = TargetType.CSharp;
        public string @namespace = null;
        public string outputDirectory = "Generated/";
        public string target_directory;
        public string source_directory;
        public string root_directory;
        public string target_specific_src_directory;
        public HashSet<string> tool_grammar_files = null;
        public HashSet<string> tool_src_grammar_files = null;
        public List<string> generated_files = null;
        public List<string> additional_grammar_files = null;
        public bool profiling = false;
        public bool? case_fold = null;
        public string lexer_name = null;
        public string lexer_src_grammar_file_name = null;
        public string lexer_grammar_file_name = null;
        public string lexer_generated_file_name = null;
        public string parser_name = null;
        public string parser_src_grammar_file_name = null;
        public string parser_grammar_file_name = null;
        public string parser_generated_file_name = null;
        public string startRule;
        public string suffix;
        public IEnumerable<string> skip_list;
        public bool maven = false;
        string SetupFfn = ".dotnet-antlr.rc";
        bool do_templates = true;

        public enum TargetType
        {
            Cpp,
            CSharp,
            Dart,
            Go,
            Java,
            JavaScript,
            Php,
            Python2,
            Python3,
            Swift,
            Antlr4cs,
        }

        public enum EnvType
        {
            Unix,
            Windows,
            Mac,
        }

        public enum LineTranslationType
        {
            Native,
            LF,
            CRLF,
            CR,
        }

        public enum PathSepType
        {
            Semi,
            Colon,
        }

        public static LineTranslationType GetLineTranslationType()
        {
            //System.Console.Error.WriteLine("FrameworkDescription " + RuntimeInformation.FrameworkDescription);
            //System.Console.Error.WriteLine("OSArchitecture " + RuntimeInformation.OSArchitecture);
            //System.Console.Error.WriteLine("OSDescription " + RuntimeInformation.OSDescription);
            //System.Console.Error.WriteLine("ProcessArchitecture " + RuntimeInformation.ProcessArchitecture);
            //System.Console.Error.WriteLine("RuntimeIdentifier " + RuntimeInformation.RuntimeIdentifier);

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

        public static PathSepType GetPathType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return PathSepType.Semi;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PathSepType.Colon;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public class Options
        {
            [Option('a', "antlr4cs", Required=false, Default=false, HelpText = "Generate code for Antlr4cs runtime.")]
            public bool Antlr4cs { get; set; }

            [Option('c', "case-fold", Required=false, HelpText="Fold case of lexer. True = upper, false = lower.")]
            public bool? CaseFold { get; set; }

            [Option('e', "encoding", Required = false)]
            public LineTranslationType? Encoding { get; set; }

            [Option('f', "file", Required=false, HelpText="The name of an input file to parse.")]
            public string InputFile { get; set; }

            [Option('g', "grammar-files", Required = false, HelpText = "A list of vertical bar separated grammar file paths.")]
            public string GrammarFiles { get; set; }

            [Option('k', "skip-list", Required = false, Separator = ',', HelpText = "A skip list for pom.xml.")]
            public IEnumerable<string> SkipList { get; set; }

            [Option('m', "maven", Required = false, Default = false, HelpText = "Read Antlr pom file and convert.")]
            public bool Maven { get; set; }

            [Option('n', "namespace", Required=false, HelpText="The namespace for all generated files.")]
            public string DefaultNamespace { get; set; }

            [Option('o', "output-directory", Required=false, HelpText="The output directory for the project.")]
            public string OutputDirectory { get; set; }

            [Option('p', "package", Required=false, HelpText="PackageReference's to include, in name/version pairs.")]
            public string Packages { get; set; }

            [Option('s', "start-rule", Required=false, HelpText="Start rule name.")]
            public string StartRule { get; set; }

            [Option('t', "target", Required = false, Default=TargetType.CSharp, HelpText = "The target language for the project.")]
            public TargetType Target { get; set; }

            [Option('x', "profile", Required = false, Default = false, HelpText = "Add in Antlr profiling code.")]
            public bool Profiling { get; set; }

            [Option("envtype", Required = false)]
            public EnvType? EnvType { get; set; }

            [Option("pathtype", Required = false)]
            public PathSepType? PathType { get; set; }

            [Option("linetranslationtype", Required = false)]
            public LineTranslationType? LineTranslationType { get; set; }

            [Option("antlrtoolpath", Required = false)]
            public string AntlrToolPath { get; set; }
        }

        static string TargetName(TargetType target)
        {
            return target switch
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
                System.Console.Error.WriteLine(e);
                System.Environment.Exit(1);
            }
        }

        public void MainInternal(string[] args)
        {
            // Get default from OS.
            line_translation = GetLineTranslationType();
            env_type = GetEnvType();
            path_sep_type = GetPathType();

            // Get any defaults from ~/.dotnet-antlr.rc
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (System.IO.File.Exists(home + Path.DirectorySeparatorChar + SetupFfn))
            {
                var jsonString = System.IO.File.ReadAllText(home + Path.DirectorySeparatorChar + SetupFfn);
                var options = JsonSerializer.Deserialize<Options>(jsonString);
                path_sep_type = (PathSepType)options.PathType;
            }

            string tool_grammar_files_pattern = "^(?!.*(/Generated|/target|/examples)).+g4$";

            // Parse options, stop if we see a bogus option, or something like --help.
            var result = Parser.Default.ParseArguments<Options>(args);
            bool stop = false;
            result.WithNotParsed(o => { stop = true; });
            if (stop) return;
            
            result.WithParsed(o =>
            {
                target = o.Target;
                profiling = o.Profiling;
                antlr4cs = o.Antlr4cs;
                maven = o.Maven;
                if (o.Encoding != null) line_translation = (LineTranslationType)o.Encoding == LineTranslationType.Native ? GetLineTranslationType() : (LineTranslationType)o.Encoding;
                if (antlr4cs) @namespace = "Test";
                if (o.CaseFold != null) case_fold = o.CaseFold;
                if (o.DefaultNamespace != null) @namespace = o.DefaultNamespace;
                if (o.GrammarFiles != null) tool_grammar_files_pattern = o.GrammarFiles;
                if (o.StartRule != null) startRule = o.StartRule;
                if (o.OutputDirectory != null) outputDirectory = o.OutputDirectory;
                if (o.SkipList != null) skip_list = o.SkipList;
            });

            if (antlr4cs) target = TargetType.Antlr4cs;
            suffix = target switch
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
            target_specific_src_directory = target switch
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

            var path = Environment.CurrentDirectory;
            var cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            root_directory = cd;

            if (maven)
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
        
            if (skip_list.Where(s => cd.Remove(cd.Length-1).EndsWith(s)).Any())
            {
                System.Console.Error.WriteLine("Skipping.");
                return;
            }
            target_directory = System.IO.Path.GetFullPath(cd + Path.DirectorySeparatorChar + outputDirectory);

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

                parser_name = pom_grammar_name.First() + "Parser";
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
                    (pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + Path.GetFileName(parser_src_grammar_file_name);
                parser_generated_file_name =
                    (pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + parser_name + suffix;

                lexer_name = pom_lexer_name.Any() ? pom_lexer_name.First() : pom_grammar_name.First() + "Lexer";
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
                    (pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + Path.GetFileName(lexer_src_grammar_file_name);
                lexer_generated_file_name =
                    (pom_package_name.Any() && pom_package_name.First() != ""
                    ? pom_package_name.First().Replace('.', '/') + '/'
                    : "")
                    + lexer_name + suffix;

                if (pom_package_name.Any()) @namespace = pom_package_name.First();
                tool_src_grammar_files = new HashSet<string>()
                {
                    lexer_src_grammar_file_name,
                    parser_src_grammar_file_name
                };
                tool_grammar_files = new HashSet<string>()
                {
                    lexer_grammar_file_name,
                    parser_grammar_file_name
                };
                startRule = pom_entry_point.First();
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
                Directory.CreateDirectory(outputDirectory);
            }
            catch (Exception)
            {
                throw;
            }

            if (do_templates)
            {
                GenFromTemplates(this);
            }
            else
            {

                // Include all other grammar files, but not if they are the main grammars.
                var additional_grammars_pattern = "^(?!.*(Generated/|target/|examples/|"
                    + String.Join("|", tool_src_grammar_files)
                    + ")).+g4$";
                additional_grammar_files = new Domemtech.Globbing.Glob()
                        .RegexContents(additional_grammars_pattern)
                        .Where(f => f is FileInfo)
                        .Select(f => f.FullName.Replace(cd, ""))
                        .ToList();

                System.Console.Error.WriteLine("additional grammars " + String.Join(" ", additional_grammar_files));

                // Find all source files.
                var all_source_pattern = "^(?!.*(Generated/|target/|examples/" + (!antlr4cs ? "|Antlr4cs/" : "") + ")).+" + target switch
                {
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
                } + "$";
                all_source_files = new Domemtech.Globbing.Glob()
                        .RegexContents(all_source_pattern)
                        .Where(f => f is FileInfo)
                        .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                        .ToList();

                AddSourceFiles.AddSource(this);
                GenBuild.AddBuildFile(this);
                GenGrammars.AddGrammars(this);
                GenMain.AddMain(this);
                GenListener.AddErrorListener(this);
                AddCaseFold();
            }
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
            Type ty = this.GetType();
            System.Reflection.Assembly a = ty.Assembly;
            var res = a.GetManifestResourceNames();
            var cd = "AntlrTemplating.templates.";
            var orig_files = ReadAllResourceLines(a, "AntlrTemplating.templates.files");
            var files = res;
            var filter_string = "^.*/Arithmetic/(?!.*("
                + AllButTargetName(target)
                + ")).*$";
            var regex_string = filter_string;
            var regex = new Regex(regex_string);
            var new_files = orig_files.Where(f => regex.IsMatch(f)).ToList();
            var set = new HashSet<string>();
            foreach (var f in new_files)
            {
                // Construct proper file name.
                // Construct proper starting directory based on namespace.
                var from = f.Replace('\\', '/');
                var c = cd.Replace('\\', '/');
                var e = f.Replace(c, "");
                var m = Path.GetFileName(f);
                var n = p.@namespace != null ? p.@namespace.Replace('.', '/') : "";
                var to = p.outputDirectory.Replace('\\', '/') + n + "/" + m;
                from = cd + from.Replace('/', '.').Substring(2);
                to = to.Replace('\\', '/');
                var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                Directory.CreateDirectory(q);
                string content = ReadAllResource(a, from);
                Template t = new Template(content);
                t.Add("version", Program.version);
                t.Add("cli_bash", this.env_type == EnvType.Unix);
                t.Add("cli_cmd", this.env_type == EnvType.Windows);
                t.Add("path_sep_semi", this.path_sep_type == PathSepType.Semi);
                t.Add("path_sep_colon", this.path_sep_type == PathSepType.Colon);
                var o = t.Render();
                File.WriteAllText(to, o);

            }
        }

        public void AddCaseFold()
        {
            if (case_fold == null) return;
            StringBuilder sb = new StringBuilder();
            if (target == TargetType.CSharp)
            {
                sb.AppendLine(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version);
                if (@namespace != null) sb.AppendLine("namespace " + @namespace + @"
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
                if (@namespace != null) sb.AppendLine("}");
                string fn = outputDirectory + "CaseChangingCharStream.cs";
                System.IO.File.WriteAllText(fn, sb.ToString());
            }
            else if (target == TargetType.Java)
            {
                sb.Append(@"
// Template generated code from Antlr4BuildTasks.dotnet-antlr v " + version + @"
package " + @namespace + @";

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
                string fn = outputDirectory + "ErrorListener.java";
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
            lexer_name = "";
            parser_name = "";
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
                startRule = "file_";
                use_arithmetic = true;
                break;
            }
            parser_name = Path.GetFileName(parser_src_grammar_file_name).Replace("Parser.g4", "").Replace(".g4", "") + "Parser";
            parser_grammar_file_name = Path.GetFileName(parser_src_grammar_file_name);
            parser_generated_file_name = parser_name + suffix;

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
                startRule = "file_";
                use_arithmetic = true;
                break;
            }

            lexer_name = Path.GetFileName(lexer_src_grammar_file_name).Replace("Lexer.g4", "").Replace(".g4", "") + "Lexer";
            lexer_grammar_file_name = Path.GetFileName(lexer_src_grammar_file_name);
            lexer_generated_file_name = lexer_name + suffix;
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