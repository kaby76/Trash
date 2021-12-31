namespace Trash
{
    using Algorithms;
    using Antlr4.Runtime.Tree;
    using Antlr4.StringTemplate;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trgen.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public int Execute(Config co)
        {
            config = co;
            if (config.template_sources_directory != null)
                config.template_sources_directory = Path.GetFullPath(config.template_sources_directory);
            var path = Environment.CurrentDirectory;
            string cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            var per_grammar = new PerGrammar();
            per_grammar.current_directory = cd;
            this.root_directory = cd;

            bool do_maven = false;
            if (config.maven != null && !(bool)config.maven)
                do_maven = false;
            else if (config.maven != null && (bool)config.maven)
                do_maven = true;
            else if (File.Exists(cd + Path.DirectorySeparatorChar + @"pom.xml"))
                do_maven = true;
            else
                do_maven = false;

            if (do_maven)
            {
                DoMavenGenerate(cd);
                if (failed_modules.Any())
                {
                    // List out failed grammars. I really should say what failed,
                    // what succeeded, what skipped, but I don't. TODO.
                    System.Console.WriteLine(String.Join(" ", failed_modules));
                    return 1;
                }
            }
            else
            {
                DoNonMavenGenerate(per_grammar);
            }
            return 0;
        }

        /// <summary>
        /// pom_includes is the entry in pom.xml listing the antrl4 grammar .g4 files.
        /// It should not list.g4 files that are "imported" by other grammars.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/includes/include
        /// </summary>
        List<string> pom_includes = null;

        /// <summary>
        /// pom_grammars is the entry in pom.xml listing, via an alternative to
        /// "includes/include", the antrl4 grammar .g4 files.
        /// It should not list.g4 files that are "imported" by other grammars.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/grammars
        /// </summary>
        List<string> pom_grammars = null;

        /// <summary>
        /// pom_antlr_tool_args is the entry in pom.xml listing the Antlr4
        /// tool arguments. It is typically used for setting the package/namespace
        /// for the generated files, e.g., "-package org.antlr.parser.st4"
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/arguments/argument
        /// </summary>
        List<string> pom_antlr_tool_args = null;

        /// <summary>
        /// pom_source_directory is the entry in pom.xml for Maven to
        /// find the Java source files used with the generated parser.
        /// This is where you typically specify the Java/ directory for the
        /// Java parser and lexer base classes, but it could also contain driver
        /// code.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/sourceDirectory
        /// </summary>
        List<string> pom_source_directory = null;

        /// <summary>
        /// pom_all_else is a list of strings for anything in the pom.xml
        /// file that is a "catch-all" for anything that I didn't understand,
        /// or need for antlr4-maven-plugin.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/*[not(self::sourceDirectory or self::arguments or self::includes or self::visitor or self::listener)]
        /// </summary>
        List<XPathNavigator> pom_all_else = null;

        /// <summary>
        /// This is the name of the parser to test, used by the Antlr4 test
        /// plugin.
        /// pom_grammar_name is a string list, but it should be just one.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/grammarName
        /// </summary>
        List<string> pom_grammar_name = null;

        /// <summary>
        /// This is the name of the lexer used in the Antlr4 test of the parser.
        /// pom_lexer_name is a string list, but it should be just one.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/grammarName
        /// </summary>
        List<string> pom_lexer_name = null;

        /// <summary>
        /// This is the name of the start rule used in the Antlr4 test of the parser.
        /// pom_entry_point is a string list, but it should be just one.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/entryPoint
        /// </summary>
        List<string> pom_entry_point = null;

        /// <summary>
        /// This is the name of the package name used in the Antlr4 test of the parser.
        /// pom_package_name is a string list, but it should be just one.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/packageName
        /// </summary>
        List<string> pom_package_name = null;

        /// <summary>
        /// This is used in the Antlr4 test of the parser for case-insensitive parsing.
        /// The values can be UPPER, LOWER, TRUE, or NONE.
        /// pom_case_insensitive_type is a string list, but it should be just one.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/caseInsensitiveType
        /// </summary>
        List<string> pom_case_insensitive_type = null;

        /// <summary>
        /// This is used in the Antlr4 test of the parser for the directory
        /// containing the files to parse.
        /// pom_case_insensitive_type is a string list, but it should be just one.
        /// The xpath for the list is
        /// //plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/exampleFiles
        /// </summary>
        List<string> pom_example_files = null;

        public Config config;
        public static string version = "0.13.3";

        // For maven-generated code.
        public string root_directory;

        // For maven-generated code.
        public List<string> failed_modules = new List<string>();

        public string Suffix(Config config)
        {
            return config.target switch
            {
                "Antlr4cs" => ".cs",
                "Cpp" => ".cpp",
                "CSharp" => ".cs",
                "Dart" => ".dart",
                "Go" => ".go",
                "Java" => ".java",
                "JavaScript" => ".js",
                "Php" => ".php",
                "Python2" => ".py",
                "Python3" => ".py",
                "Swift" => ".swift",
                "TypeScript" => ".ts",
                _ => throw new NotImplementedException(),
            };
        }

        public string ignore_string = null;
        public string ignore_file_name = ".trgen-ignore";
        public string SetupFfn = ".trgen.rc";

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
                return "/tmp/antlr-4.9.3-complete.jar";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return (home + "/Downloads/antlr-4.9.3-complete.jar").Replace('\\', '/');
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "~/Downloads/antlr-4.9.3-complete.jar";
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static string TargetName(string target)
        {
            return target switch
            {
                "Antlr4cs" => "Antlr4cs",
                "Cpp" => "Cpp",
                "CSharp" => "CSharp",
                "Dart" => "Dart",
                "Go" => "Go",
                "Java" => "Java",
                "JavaScript" => "JavaScript",
                "Php" => "Php",
                "Python2" => "Python2",
                "Python3" => "Python3",
                "Swift" => "Swift",
                "TypeScript" => "TypeScript",
                _ => throw new NotImplementedException(),
            };
        }

        public static string AllButTargetName(string target)
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

        public static string TargetSpecificSrcDirectory(Config config)
        {
            return TargetName(config.target);
        }

        public void DoMavenGenerate(string cd)
        {
            // Read pom.xml in current directory.
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
                        DoMavenGenerate(cd + sd + "/");
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
                return;
            }

            //
            // Determine if we skip this grammar.
            //
            if (config.todo_pattern != null)
            {
                var te = new Regex(config.todo_pattern).IsMatch(cd);
                if (!te)
                {
                    System.Console.Error.WriteLine("Skipping.");
                    return;
                }
            }
            else if (config.skip_pattern != null)
            {
                var te = new Regex(config.skip_pattern).IsMatch(cd);
                if (te)
                {
                    System.Console.Error.WriteLine("Skipping.");
                    return;
                }
            }

            //
            // Process grammar pom.xml here.
            //
            PerGrammar per_grammar = new PerGrammar();

            // Get grammar and testing information from pom.xml.
            // I'd love to have these self documenting, but C# only allows
            // self documentation for fields, not local variables.
            pom_includes = navigator
                .Select("//plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/includes/include", nsmgr)
                .Cast<XPathNavigator>()
                .Select(t => t.Value)
                .ToList();
            pom_grammars = navigator
                .Select("//plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/grammars", nsmgr)
                .Cast<XPathNavigator>()
                .Select(t => t.Value)
                .ToList();
            pom_antlr_tool_args = navigator
                .Select("//plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/arguments/argument", nsmgr)
                .Cast<XPathNavigator>()
                .Where(t => t.Value != "")
                .Select(t => t.Value)
                .ToList();
            pom_source_directory = navigator
                .Select("//plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/sourceDirectory", nsmgr)
                .Cast<XPathNavigator>()
                .Where(t => t.Value != "")
                .Select(t => t.Value)
                .ToList();
            pom_all_else = navigator
                .Select("//plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/*[not(self::sourceDirectory or self::arguments or self::includes or self::visitor or self::listener)]", nsmgr)
                .Cast<XPathNavigator>()
                .ToList();
            pom_grammar_name = navigator
                .Select("//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/grammarName", nsmgr)
                .Cast<XPathNavigator>()
                .Select(t => t.Value)
                .ToList();
            pom_lexer_name = navigator
                .Select("//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/lexerName", nsmgr)
                .Cast<XPathNavigator>()
                .Select(t => t.Value)
                .ToList();
            pom_entry_point = navigator
                .Select("//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/entryPoint", nsmgr)
                .Cast<XPathNavigator>()
                .Select(t => t.Value)
                .ToList();
            pom_package_name = navigator
                .Select("//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/packageName", nsmgr)
                .Cast<XPathNavigator>()
                .Where(t => t.Value != "")
                .Select(t => t.Value)
                .ToList();
            pom_case_insensitive_type = navigator
                .Select("//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/caseInsensitiveType", nsmgr)
                .Cast<XPathNavigator>()
                .Where(t => t.Value != "")
                .Select(t => t.Value)
                .ToList();
            pom_example_files = navigator
                .Select("//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/exampleFiles", nsmgr)
                .Cast<XPathNavigator>()
                .Where(t => t.Value != "")
                .Select(t => t.Value)
                .ToList();

            var pom_all_test = navigator
                .Select("//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/*", nsmgr)
                .Cast<XPathNavigator>()
                .Select(t => t)
                .ToList();

            // Check all other config options in antlr4-maven-plugin configuration.
            bool pom_all_else_bad = false;
            foreach (var e in pom_all_else)
            {
                System.Console.Error.WriteLine("Invalid element \"//plugins/plugin[artifactId='antlr4-maven-plugin']/configuration/"
                    + e.Name
                    + ". Correct the pom.xml!");
                pom_all_else_bad = true;
            }
            if (pom_all_else_bad)
            {
                throw new Exception();
            }


            // Go through all elements under configuration and check if nonsense.
            bool pom_all_test_bad = false;
            foreach (var e in pom_all_test)
            {
                // straight from https://github.com/antlr/antlr4test-maven-plugin
                if (e.Name == "grammarName") continue;
                if (e.Name == "caseInsensitiveType") continue;
                if (e.Name == "entryPoint") continue;
                if (e.Name == "binary") continue;
                if (e.Name == "enabled") continue;
                if (e.Name == "verbose") continue;
                if (e.Name == "showTree") continue;
                if (e.Name == "exampleFiles") continue;
                if (e.Name == "packageName") continue;
                if (e.Name == "testFileExtension") continue;
                if (e.Name == "fileEncoding") continue;
                if (e.Name == "grammarInitializer") continue;

                System.Console.Error.WriteLine("Invalid element \"//plugins/plugin[artifactId='antlr4test-maven-plugin']/configuration/"
                    + e.Name
                    + ". Correct the pom.xml!");
                pom_all_test_bad = true;
            }
            if (pom_all_test_bad)
            {
                throw new Exception();
            }


            // grammarName is required. https://github.com/antlr/antlr4test-maven-plugin#grammarname
            if (!pom_grammar_name.Any())
            {
                return;
            }

            // The grammar name in pom is a mess. That's because
            // people define multiple parser grammars in the pom includes/grammars,
            // and a driver (see grammars-v4/r). So, take it on faith.
            per_grammar.grammar_name = pom_grammar_name.First();

            // Pom is a mess. There are many cases here in computing the namespace/package.
            // People even add bullshit @headers in the grammar; minds of a planaria.
            // -package arg specified; source top level
            //   => keep .g4 at top level, generate to directory
            //      corresponding to arg.
            if (config.target == "JavaScript" || config.target == "Dart")
            {
                config.name_space = null;
            }
            else if (config.target == "Go")
            {
                config.name_space = null;
            }
            else if (pom_antlr_tool_args.Contains("-package"))
            {
                var ns = pom_antlr_tool_args[pom_antlr_tool_args.IndexOf("-package") + 1];
                config.name_space = ns;
            }
            else if (pom_package_name.Any())
            {
                config.name_space = pom_package_name.First();
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

            // Make sure all the grammar files actually existance.
            // People don't check their bullshit.
            var merged_list = new HashSet<string>();
            foreach (var p in pom_includes) merged_list.Add(p);
            foreach (var p in pom_grammars) merged_list.Add(p);
            foreach (var x in merged_list.ToList())
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

            // Check existance of example files.
            if (pom_example_files.Any())
            {
                per_grammar.example_files = pom_example_files.First();
                if (!Directory.Exists(pom_example_files.First()))
                {
                    System.Console.Error.WriteLine("Examples directory doesn't exist " + pom_example_files.First());
                }
            }
            else
            {
                per_grammar.example_files = "examples";
            }

            if (pom_source_directory.Any())
            {
                per_grammar.source_directory = pom_source_directory
                    .First()
                    .Replace("${basedir}", "")
                    .Trim();
                if (per_grammar.source_directory.StartsWith('/')) per_grammar.source_directory = per_grammar.source_directory.Substring(1);
                if (per_grammar.source_directory != "" && !per_grammar.source_directory.EndsWith("/"))
                {
                    per_grammar.source_directory = per_grammar.source_directory + "/";
                }
            }
            else
            {
                per_grammar.source_directory = "";
            }

            if (pom_case_insensitive_type.Any())
            {
                if (pom_case_insensitive_type.First().ToUpper() == "UPPER")
                    config.case_insensitive_type = CaseInsensitiveType.Upper;
                else if (pom_case_insensitive_type.First().ToUpper() == "LOWER")
                    config.case_insensitive_type = CaseInsensitiveType.Lower;
                else throw new Exception("Case fold has invalid value: '"
                    + pom_case_insensitive_type.First() + "'.");
            }
            else config.case_insensitive_type = null;

            // Check for existence of .trgen-ignore file.
            // If there is one, read and create pattern of what to ignore.
            if (File.Exists(ignore_file_name))
            {
                var ignore = new StringBuilder();
                var lines = File.ReadAllLines(ignore_file_name);
                var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                ignore_string = string.Join("|", ignore_lines);
            }

            if (!(config.target == "JavaScript" || config.target == "Dart"))
            {
                List<string> additional = new List<string>();
                config.antlr_tool_args = additional;
                // On Linux, the flies are automatically place in the package,
                // and they cannot be changed!
                if (config.name_space != null && config.name_space != "")
                {
                    if (config.env_type == EnvType.Windows)
                    {
                        additional.Add("-o");
                        additional.Add(config.name_space.Replace('.', '/'));
                    }
                    additional.Add("-lib");
                    additional.Add(config.name_space.Replace('.', '/'));
                }
            }


            string lexer_generated_file_name = null;
            string parser_src_grammar_file_name = null;
            string parser_grammar_file_name = null;
            string parser_generated_file_name = null;
            string lexer_grammar_file_name = null;
            string lexer_src_grammar_file_name = null;
            string lexer_generated_include_file_name = null;

            per_grammar.package = (pom_package_name != null && pom_package_name.Any() ? pom_package_name.First() + "/" : "");

            // Let's first parse the input grammar files and gather information
            // about them. Note, people pump in all sorts of bullshit, so
            // be ready to handle the worse of the worse.
            per_grammar.tool_grammar_tuples = new List<GrammarTuple>();
            foreach (var f in merged_list)
            {
                // We're going to assume that the grammars are in
                // the current directory. That's because this is the Maven
                // Antlr4 tool plugin so it doesn't fish around to find the grammar,
                // except possibley in the "source directory".
                string sgfn;
                string tgfn;
                if (File.Exists(f))
                {
                    sgfn = f;
                    tgfn = per_grammar.package.Replace(".", "/") + (config.target == "Go" ? per_grammar.grammar_name + "/" : "") + f;
                }
                else if (File.Exists(per_grammar.source_directory + f))
                {
                    sgfn = per_grammar.source_directory + f;
                    tgfn = per_grammar.package.Replace(".", "/") + (config.target == "Go" ? per_grammar.grammar_name + "/" : "") + f;
                }
                else if (File.Exists(config.target + "/" + f))
                {
                    sgfn = config.target + "/" + f;
                    tgfn = per_grammar.package.Replace(".", "/") + (config.target == "Go" ? per_grammar.grammar_name + "/" : "") + f;
                }
                else
                {
                    System.Console.Error.WriteLine("Error in pom.xml: <include>" + f + "</include> is for a file that does not exist.");
                    throw new Exception();
                }

                var doc = Docs.Class1.ReadDoc(sgfn);
                var pr = LanguageServer.ParsingResultsFactory.Create(doc);
                var workspace = doc.Workspace;
                _ = new LanguageServer.Module().Compile(workspace);
                if (pr.Errors.Any())
                {
                    System.Console.Error.WriteLine("Your grammar "
                        + sgfn
                        + " does not compile as an Antlr4 grammar! Please check it.");
                    throw new Exception();
                }

                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                List<IParseTree> nodes = null;
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(pr.ParseTree, pr.Parser))
                {
                    nodes = engine.parseExpression(
                        @"/grammarSpec/grammarDecl",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as IParseTree).ToList();
                }

                if (nodes == null)
                {
                    System.Console.Error.WriteLine("Your grammar "
                        + sgfn
                        + " does not compile as an Antlr4 grammar! Please check it.");
                    throw new Exception();
                }
                if (nodes.Count() == 0 || nodes.Count() > 1)
                {
                    System.Console.Error.WriteLine("Your grammar "
                        + sgfn
                        + " does not compile as an Antlr4 grammar! Please check it.");
                    throw new Exception();
                }
                var grammarDecl = nodes.First() as LanguageServer.ANTLRv4Parser.GrammarDeclContext;
                if (nodes.Count() == 0 || nodes.Count() > 1)
                {
                    System.Console.Error.WriteLine("Your grammar "
                        + sgfn
                        + " does not compile as an Antlr4 grammar! Please check it.");
                    throw new Exception();
                }
                var is_parser_grammar = grammarDecl.grammarType()?.PARSER() != null;
                var is_lexer_grammar = grammarDecl.grammarType()?.LEXER() != null;
                var is_combined = !is_parser_grammar && !is_lexer_grammar;
                var name = grammarDecl.identifier().GetText();
                if (!(is_combined && !is_parser_grammar && !is_lexer_grammar
                    || !is_combined && is_parser_grammar && !is_lexer_grammar
                    || !is_combined && !is_parser_grammar && is_lexer_grammar))
                {
                    System.Console.Error.WriteLine("Your grammar "
                        + sgfn
                        + " is malformed! Please check it.");
                    throw new Exception();
                }
                if (!is_combined && !is_parser_grammar && !is_lexer_grammar)
                {
                    System.Console.Error.WriteLine("Your grammar "
                        + sgfn
                        + " is malformed! Please check it.");
                    throw new Exception();
                }

                if (is_parser_grammar)
                {
                    var genfn = (config.target == "Go" ? name.Replace("Parser", "") + "/" : "") + name + Suffix(config);
                    var genincfn = name + ".h";
                    var autom_name = ((config.name_space != null && config.name_space != "")
                            ? config.name_space + '.' : "")
                        + name;
                    var goname = name.Replace("Parser","") + ".New" + name;
                    var antlr_args = (config.target == "Go" ? "-o " + per_grammar.grammar_name + " -lib " + per_grammar.grammar_name + " -package " + per_grammar.grammar_name : "");
                    var g = new GrammarTuple(sgfn, tgfn, name, genfn, genincfn, autom_name, goname, antlr_args);
                    per_grammar.tool_grammar_tuples.Add(g);
                }
                else if (is_lexer_grammar)
                {
                    var genfn = (config.target == "Go" ? name.Replace("Lexer", "") + "/" : "") + name + Suffix(config);
                    var genincfn = name + ".h";
                    var autom_name = ((config.name_space != null && config.name_space != "")
                            ? config.name_space + '.' : "")
                        + name;
                    var goname = name.Replace("Lexer","") + ".New" + name;
                    var antlr_args = (config.target == "Go" ? "-o " + per_grammar.grammar_name + " -lib " + per_grammar.grammar_name + " -package " + per_grammar.grammar_name : "");
                    var g = new GrammarTuple(sgfn, tgfn, name, genfn, genincfn, autom_name, goname, antlr_args);
                    per_grammar.tool_grammar_tuples.Add(g);
                }
                else
                {
                    {
                        var genfn = (config.target == "Go" ? name + "/" : "") + name + "Parser" + Suffix(config);
                        var genincfn = name + "Parser.h";
                        var autom_name = ((config.name_space != null && config.name_space != "")
                                ? config.name_space + '.' : "")
                            + name
                            + "Parser";
                        var goname = name + '.'
                            + "New" + name
                            + "Parser";
                        var antlr_args = (config.target == "Go" ? "-o " + per_grammar.grammar_name + " -lib " + per_grammar.grammar_name + " -package " + per_grammar.grammar_name : "");
                        var g = new GrammarTuple(sgfn, tgfn, name, genfn, genincfn, autom_name, goname, antlr_args);
                        per_grammar.tool_grammar_tuples.Add(g);
                    }
                    {
                        var genfn = (config.target == "Go" ? name + "/" : "") + name + "Lexer" + Suffix(config);
                        var genincfn = name + "Lexer.h";
                        var autom_name = ((config.name_space != null && config.name_space != "")
                                ? config.name_space + '.' : "")
                            + name
                            + "Lexer";
                        var goname = name + '.'
                             + "New" + name
                             + "Lexer";
                        var antlr_args = (config.target == "Go" ? "-o " + per_grammar.grammar_name + " -lib " + per_grammar.grammar_name + " -package " + per_grammar.grammar_name : "");
                        var g = new GrammarTuple(sgfn, tgfn, name, genfn, genincfn, autom_name, goname, antlr_args);
                        per_grammar.tool_grammar_tuples.Add(g);
                    }
                }
            }

            // Sort tool_grammar_tuples because there are dependencies!
            // Note, we can't do that by name because some grammars, like
            // grammars-v4/r, won't build that way.
            // Use Trash compiler to get dependencies.
            ComputeSort(per_grammar);

            // How to call the parser in the source code. Remember, there are
            // actually up to two tests in the pom file, one for running the
            // Antlr tool, and the other to test the generated parser.
            per_grammar.fully_qualified_parser_name =
                per_grammar.tool_grammar_tuples
                .Where(t => t.GrammarAutomName.EndsWith(pom_grammar_name.First())
                    || t.GrammarAutomName.EndsWith(pom_grammar_name.First() + "Parser"))
                .Select(t => t.GrammarAutomName).First();
            config.fully_qualified_go_parser_name =
                per_grammar.tool_grammar_tuples
                .Where(t => t.GrammarAutomName.EndsWith(pom_grammar_name.First())
                    || t.GrammarAutomName.EndsWith(pom_grammar_name.First() + "Parser"))
                .Select(t => t.GrammarGoNewName).First();

            // Where the parser generated code lives.
            parser_generated_file_name =
                (string)per_grammar.fully_qualified_parser_name.Replace('.', '/')
                + Suffix(config);
            var parser_generated_include_file_name = (string)per_grammar.fully_qualified_parser_name.Replace('.', '/') + ".h";
            parser_src_grammar_file_name =
                per_grammar.tool_grammar_tuples
                .Where(t => t.GrammarName == pom_grammar_name.First()
                    || t.GrammarName == pom_grammar_name.First() + "Parser").Select(t
                    => t.GrammarFileName).First();

            per_grammar.fully_qualified_lexer_name =
                per_grammar.tool_grammar_tuples
                .Where(t => t.GrammarAutomName.EndsWith(pom_grammar_name.First())
                    || t.GrammarAutomName.EndsWith(pom_grammar_name.First() + "Lexer"))
                .Select(t => t.GrammarAutomName).First();
            config.fully_qualified_go_lexer_name =
                per_grammar.tool_grammar_tuples
                .Where(t => t.GrammarAutomName.EndsWith(pom_grammar_name.First())
                    || t.GrammarAutomName.EndsWith(pom_grammar_name.First() + "Lexer"))
                .Select(t => t.GrammarGoNewName).First();
            lexer_generated_file_name = per_grammar.fully_qualified_lexer_name.Replace('.', '/') + Suffix(config);
            lexer_generated_include_file_name = per_grammar.fully_qualified_lexer_name.Replace('.', '/') + ".h";
            lexer_src_grammar_file_name =
                per_grammar.tool_grammar_tuples
                .Where(t => t.GrammarName == pom_grammar_name.First()
                    || t.GrammarName == pom_grammar_name.First() + "Lexer").Select(t
                    => t.GrammarFileName).First();
            per_grammar.tool_src_grammar_files = new HashSet<string>()
                {
                    lexer_src_grammar_file_name,
                    parser_src_grammar_file_name
                };
            //per_grammar.tool_grammar_tuples = new List<GrammarTuple>()
            //    {
            //        new GrammarTuple(lexer_grammar_file_name, lexer_generated_file_name, lexer_generated_include_file_name, per_grammar.fully_qualified_lexer_name),
            //        new GrammarTuple(parser_grammar_file_name, parser_generated_file_name, parser_generated_include_file_name, per_grammar.fully_qualified_parser_name),
            //    };
            per_grammar.tool_grammar_files = per_grammar.tool_grammar_tuples.Select(t => t.GrammarFileName).ToHashSet().ToList();
            per_grammar.parser_grammar_file_name = parser_src_grammar_file_name;
            per_grammar.start_rule = pom_entry_point.First();
            per_grammar.generated_files = per_grammar.tool_grammar_tuples.Select(t => t.GeneratedFileName).ToHashSet().ToList();
            per_grammar.lexer_grammar_file_name = lexer_src_grammar_file_name;
            GenerateSingle(per_grammar);
        }

        public void GenerateSingle(PerGrammar per_grammar)
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
            // Find all source files.
            per_grammar.all_target_files = new List<string>();
            per_grammar.all_source_files = new Domemtech.Globbing.Glob()
                    .RegexContents(this.config.all_source_pattern)
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
            GenFromTemplates(this, per_grammar);
            AddSource(per_grammar);
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
            var names = a.GetManifestResourceNames();
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void AddSource(PerGrammar per_grammar)
        {
            var cd = Environment.CurrentDirectory + "/";
            cd = cd.Replace('\\', '/');
            var set = new HashSet<string>();
            foreach (var path in per_grammar.all_source_files)
            {
                // Construct proper starting directory based on namespace.
                var from = path;
                var f = from.Substring(cd.Length);
                string to = null;
                if (per_grammar.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileName).Any())
                {
                    to = this.config.output_directory
                        + per_grammar.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileName).First();
                }
                else
                {
                    // Now remove target directory.
                    f = (
                            f.StartsWith(
                                Command.TargetName(this.config.target) + '/')
                            ? f.Substring((Command.TargetName(this.config.target) + '/').Length)
                            : f
                            );
                    // Remove "src/main/java", a royal hangover from the Maven plugin.
                    f = (
                            f.StartsWith("src/main/java/")
                            ? f.Substring("src/main/java".Length)
                            : f
                            );

                    if (config.name_space != null)
                    {
                        to = this.config.output_directory
                            + config.name_space.Replace('.', '/') + '/'
                            + f;
                    }
                    if (to == null)
                    {
                        to = this.config.output_directory
                            + (
                                f.StartsWith(per_grammar.source_directory)
                                ? f.Substring(per_grammar.source_directory.Length)
                                : f
                                );
                    }
                }
                System.Console.Error.WriteLine("Copying source file from "
                  + from
                  + " to "
                  + to);
                per_grammar.all_target_files.Add(to);
                this.CopyFile(from, to);
            }
        }

        private void GenFromTemplates(Command p, PerGrammar per_grammar)
        {
            var append_namespace = (!(p.config.target == "CSharp" || p.config.target == "Antlr4cs"));
            if (config.template_sources_directory == null)
            {
                System.Reflection.Assembly a = this.GetType().Assembly;
                // Load resource file that contains the names of all files in templates/ directory,
                // which were obtained by doing "cd templates/; find . -type f > files" at a Bash
                // shell.
                var orig_file_names = ReadAllResourceLines(a, "trgen.templates.files");
                var prefix = "trgen.templates.";
                var regex_string = "^(?!.*(" + AllButTargetName(config.target) + "/)).*$";
                var regex = new Regex(regex_string);
                var files_to_copy = orig_file_names.Where(f =>
                {
                    if (per_grammar.fully_qualified_parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
                    if (f == "./files") return false;
                    var v = regex.IsMatch(f);
                    return v;
                }).Select(f => f.Substring(("./").Length)).ToList();
                var set = new HashSet<string>();
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    // copy the file straight up if it doesn't begin
                    // with target directory name. Otherwise,
                    // remove the target dir name.
                    if (file.EndsWith("Arithmetic.g4")
                        && per_grammar.grammar_name != "Arithmetic"
                        && per_grammar.tool_src_grammar_files.Any())
                    {
                        continue;
                    }
                    string to = null;
                    if (from.StartsWith(config.target)) to = from.Substring(config.target.Length + 1);
                    else to = from;
                    if (per_grammar.tool_grammar_tuples.Where(t => from.Substring(config.target.Length) == t.OriginalSourceFileName).Select(t => t.GrammarFileName).Any())
                    {
                        to = this.config.output_directory
                            + per_grammar.tool_grammar_tuples.Where(t => to == t.OriginalSourceFileName).Select(t => t.GrammarFileName).First();
                    }
                    else
                    {
                        to = (config.output_directory).Replace('\\', '/') + to;
                    }
                    to = to.Replace('\\', '/');
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = ReadAllResource(a, prefix + from.Replace('/','.'));
                    System.Console.Error.WriteLine("Rendering template file from "
                        + from
                        + " to "
                        + to);
                    Template t = new Template(content);
                    t.Add("additional_sources", per_grammar.all_target_files.Where(t =>
                    {
                        var ext = Path.GetExtension(t);
                        return Suffix(config).Contains(ext);
                    })
                        .Select(t => t.Substring(p.config.output_directory.Length))
                        .ToList());
                    t.Add("antlr_encoding", config.antlr_encoding);
                    t.Add("antlr_tool_args", config.antlr_tool_args);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
                    t.Add("cap_start_symbol", Cap(per_grammar.start_rule));
                    t.Add("case_insensitive_type", config.case_insensitive_type);
                    t.Add("cli_bash", (EnvType)p.config.env_type == EnvType.Unix);
                    t.Add("cli_cmd", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("cmake_target", p.config.env_type == EnvType.Windows
                        ? "-G \"MSYS Makefiles\"" : "");
                    t.Add("example_files_unix", RemoveTrailingSlash(per_grammar.example_files.Replace('\\', '/')));
                    t.Add("example_files_win", RemoveTrailingSlash(per_grammar.example_files.Replace('/', '\\')));
                    t.Add("exec_name", p.config.env_type == EnvType.Windows ?
                        "Test.exe" : "Test");
                    t.Add("go_lexer_name", config.fully_qualified_go_lexer_name);
                    t.Add("go_parser_name", config.fully_qualified_go_parser_name);
                    t.Add("grammar_file", per_grammar.tool_grammar_files.First());
                    t.Add("grammar_name", per_grammar.grammar_name);
                    t.Add("has_name_space", p.config.name_space != null);
                    t.Add("is_combined_grammar", per_grammar.tool_grammar_files.Count() == 1);
                    t.Add("lexer_grammar_file", per_grammar.lexer_grammar_file_name);
                    t.Add("lexer_name", per_grammar.fully_qualified_lexer_name);
                    t.Add("name_space", p.config.name_space);
                    t.Add("os_win", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("parser_name", per_grammar.fully_qualified_parser_name);
                    t.Add("parser_grammar_file", per_grammar.parser_grammar_file_name);
                    t.Add("path_sep_colon", p.config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", p.config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", per_grammar.start_rule);
                    t.Add("temp_dir", p.config.env_type == EnvType.Windows
                        ? "c:/temp" : "/tmp");
                    t.Add("tool_grammar_files", per_grammar.tool_grammar_files);
                    t.Add("tool_grammar_tuples", per_grammar.tool_grammar_tuples);
                    t.Add("version", Command.version);
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
            }
            else
            {
                var regex_string = "^(?!.*(files|" + AllButTargetName(config.target) + "/)).*$";
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
                        if (per_grammar.fully_qualified_parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
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
                    if (file.EndsWith("Arithmetic.g4")
                        && per_grammar.grammar_name != "Arithmetic"
                        && per_grammar.tool_src_grammar_files.Any())
                    {
                        continue;
                    }
                    var from = file;
                    var e = file.Substring(prefix_to_remove.Length);
                    var to = e.StartsWith(TargetName(p.config.target))
                         ? e.Substring((TargetName(p.config.target)).Length + 1)
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
                    t.Add("additional_sources", per_grammar.all_target_files.Where(t =>
                    {
                        var ext = Path.GetExtension(t);
                        return Suffix(config).Contains(ext);
                    })
                        .Select(t => t.Substring(p.config.output_directory.Length))
                        .ToList());
                    t.Add("antlr_encoding", config.antlr_encoding);
                    t.Add("antlr_tool_args", config.antlr_tool_args);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
                    t.Add("cap_start_symbol", Cap(per_grammar.start_rule));
                    t.Add("case_insensitive_type", config.case_insensitive_type);
                    t.Add("cli_bash", (EnvType)p.config.env_type == EnvType.Unix);
                    t.Add("cli_cmd", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("cmake_target", p.config.env_type == EnvType.Windows
                        ? "-G \"MSYS Makefiles\"" : "");
                    t.Add("example_files_unix", RemoveTrailingSlash(per_grammar.example_files.Replace('\\', '/')));
                    t.Add("example_files_win", RemoveTrailingSlash(per_grammar.example_files.Replace('/', '\\')));
                    t.Add("exec_name", p.config.env_type == EnvType.Windows ?
                      "Test.exe" : "Test");
                    t.Add("go_lexer_name", config.fully_qualified_go_lexer_name);
                    t.Add("go_parser_name", config.fully_qualified_go_parser_name);
                    t.Add("grammar_file", per_grammar.tool_grammar_files.First());
                    t.Add("grammar_name", per_grammar.grammar_name);
                    t.Add("has_name_space", p.config.name_space != null);
                    t.Add("is_combined_grammar", per_grammar.tool_grammar_files.Count() == 1);
                    t.Add("lexer_name", per_grammar.fully_qualified_lexer_name);
                    t.Add("lexer_grammar_file", per_grammar.lexer_grammar_file_name);
                    t.Add("name_space", p.config.name_space);
                    t.Add("os_win", (EnvType)p.config.env_type == EnvType.Windows);
                    t.Add("parser_name", per_grammar.fully_qualified_parser_name);
                    t.Add("parser_grammar_file", per_grammar.parser_grammar_file_name);
                    t.Add("path_sep_colon", p.config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", p.config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", per_grammar.start_rule);
                    t.Add("temp_dir", p.config.env_type == EnvType.Windows
                        ? "c:/temp" : "/tmp");
                    t.Add("tool_grammar_files", per_grammar.tool_grammar_files);
                    t.Add("tool_grammar_tuples", per_grammar.tool_grammar_tuples);
                    t.Add("version", Command.version);
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

        public void CopyFile(string from, string to)
        {
            from = from.Replace('\\', '/');
            to = to.Replace('\\', '/');
            var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
            Directory.CreateDirectory(q);
            File.Copy(from, to, true);
        }

        public void DoNonMavenGenerate(PerGrammar per_grammar)
        {
            var cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            per_grammar.source_directory = cd;
            if (per_grammar.source_directory != "" && !per_grammar.source_directory.EndsWith("/"))
            {
                per_grammar.source_directory = per_grammar.source_directory + "/";
            }
            per_grammar.start_rule = config.start_rule;
            per_grammar.example_files = "examples";
            per_grammar.fully_qualified_lexer_name = "";
            per_grammar.fully_qualified_parser_name = "";
            var parser_src_grammar_file_name = "";
            var parser_grammar_file_name = "";
            var parser_generated_file_name = "";
            var lexer_src_grammar_file_name = "";
            var lexer_grammar_file_name = "";
            var lexer_generated_file_name = "";
            var lexer_generated_include_file_name = "";
            var parser_generated_include_file_name = "";
            bool use_arithmetic = false;

            if (config.InputFile != null && config.InputFile != "")
            {
                var g = config.InputFile;
                g = Path.GetFileName(g);
                g = Path.GetFileNameWithoutExtension(g);
                per_grammar.grammar_name = g.Replace("Parser", "");
            }

            if (config.target == "JavaScript" || config.target == "Dart")
            {
                config.name_space = null;
            }
            else if (config.target == "Go")
            {
                config.name_space = "parser";
            }
            else
            {
                config.name_space = null;
            }

            for (; ; )
            {
                // Probe for parser grammar. 
                {
                    var parser_grammars_pattern =
                        "^((?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/))("
                        + TargetSpecificSrcDirectory(config) + "/)"
                        + "((?!.*Lexer)|.*Parser)).g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(parser_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        if (any.Count() >= 2)
                        {
                            // Remove from "any" list "preprocessor" grammars.
                            var new_any = new List<string>();
                            foreach (var a in any)
                            {
                                if (!a.ToLower().Contains("preprocessor"))
                                {
                                    new_any.Add(a);
                                }
                            }
                            any = new_any;
                        }
                        parser_src_grammar_file_name = any.First();
                        break;
                    }
                }
                {
                    var parser_grammars_pattern =
                        "^(?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/|Lexer)).*[.]g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(parser_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        if (any.Count() >= 2)
                        {
                            // Remove from "any" list "preprocessor" grammars.
                            var new_any = new List<string>();
                            foreach (var a in any)
                            {
                                if (!a.ToLower().Contains("preprocessor"))
                                {
                                    new_any.Add(a);
                                }
                            }
                            any = new_any;
                        }
                        parser_src_grammar_file_name = any.First();
                        break;
                    }
                }
                parser_src_grammar_file_name = "Arithmetic.g4";
                per_grammar.start_rule = "file_";
                use_arithmetic = true;
                break;
            }
            per_grammar.fully_qualified_parser_name = Path.GetFileName(parser_src_grammar_file_name).Replace("Parser.g4", "").Replace(".g4", "") + "Parser";
            parser_grammar_file_name = Path.GetFileName(parser_src_grammar_file_name);
            parser_generated_file_name = per_grammar.fully_qualified_parser_name + Suffix(config);
            var temp = Path.GetFileNameWithoutExtension(Path.GetFileName(parser_grammar_file_name));
            per_grammar.grammar_name = temp.Replace("Parser", "");

            for (; ; )
            {
                // Probe for lexer grammar. 
                {
                    var lexer_grammars_pattern =
                           "^((?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/))("
                        + TargetSpecificSrcDirectory(config) + "/)"
                        + "((?!.*Parser)|.*Lexer)).g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(lexer_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        if (any.Count() >= 2)
                        {
                            // Remove from "any" list "preprocessor" grammars.
                            var new_any = new List<string>();
                            foreach (var a in any)
                            {
                                if (!a.ToLower().Contains("preprocessor"))
                                {
                                    new_any.Add(a);
                                }
                            }
                            any = new_any;
                        }
                        lexer_src_grammar_file_name = any.First();
                        break;
                    }
                }
                {
                    var lexer_grammars_pattern =
                        "^(?!.*(" + (ignore_string != null ? ignore_string + "|" : "") + "ignore/|Generated/|target/|examples/|Parser)).*[.]g4$";
                    var any =
                        new Domemtech.Globbing.Glob()
                            .RegexContents(lexer_grammars_pattern)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(cd, ""))
                            .ToList();
                    if (any.Any())
                    {
                        if (any.Count() >= 2)
                        {
                            // Remove from "any" list "preprocessor" grammars.
                            var new_any = new List<string>();
                            foreach (var a in any)
                            {
                                if (!a.ToLower().Contains("preprocessor"))
                                {
                                    new_any.Add(a);
                                }
                            }
                            any = new_any;
                        }
                        lexer_src_grammar_file_name = any.First();
                        break;
                    }
                }
                lexer_src_grammar_file_name = "Arithmetic.g4";
                per_grammar.start_rule = "file_";
                use_arithmetic = true;
                break;
            }

            per_grammar.fully_qualified_lexer_name = Path.GetFileName(lexer_src_grammar_file_name).Replace("Lexer.g4", "").Replace(".g4", "") + "Lexer";
            lexer_grammar_file_name = Path.GetFileName(lexer_src_grammar_file_name);
            lexer_generated_file_name = per_grammar.fully_qualified_lexer_name + Suffix(config);

            if (!use_arithmetic)
                per_grammar.tool_src_grammar_files = new HashSet<string>()
                    {
                        lexer_src_grammar_file_name,
                        parser_src_grammar_file_name
                    };
            else
                per_grammar.tool_src_grammar_files = new HashSet<string>();
            per_grammar.tool_grammar_files = new List<string>()
                {
                    lexer_grammar_file_name,
                    parser_grammar_file_name
                };
            per_grammar.generated_files = new List<string>()
                {
                    lexer_generated_file_name,
                    parser_generated_file_name,
                };
            per_grammar.tool_grammar_tuples = new List<GrammarTuple>()
                {
                    new GrammarTuple(lexer_grammar_file_name, lexer_grammar_file_name, null, lexer_generated_file_name, lexer_generated_include_file_name, per_grammar.fully_qualified_lexer_name, "", ""),
                    new GrammarTuple(parser_grammar_file_name, parser_grammar_file_name, null, parser_generated_file_name, parser_generated_include_file_name, per_grammar.fully_qualified_parser_name, "", ""),
                };
            per_grammar.parser_grammar_file_name = parser_grammar_file_name;
            per_grammar.lexer_grammar_file_name = lexer_grammar_file_name;
            if (per_grammar.start_rule == null)
            {
                throw new Exception("Start rule not specified. Use '-s parser-rule-name' to set.");
            }
            if (per_grammar.grammar_name == null)
            {
                throw new Exception("Internal error. config.grammar_name null.");
            }
            // lexer and parser are set if the grammar is partitioned.
            // rest is set if there are grammar is combined.
            GenerateSingle(per_grammar);
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

        void ComputeSort(PerGrammar per_grammar)
        {
            Digraph<string> graph = new Digraph<string>();
            Workspaces.Workspace workspace = new Workspaces.Workspace();
            foreach (var t in per_grammar.tool_grammar_tuples)
            {
                var f = t.OriginalSourceFileName;
                var doc = Docs.Class1.ReadDoc(f);
                var pr = LanguageServer.ParsingResultsFactory.Create(doc);
                workspace = doc.Workspace;
            }
            _ = new LanguageServer.Module().Compile(workspace);
            foreach (var t in per_grammar.tool_grammar_tuples)
            {
                var f = t.OriginalSourceFileName;
                var doc = Docs.Class1.ReadDoc(f);
                var pr = LanguageServer.ParsingResultsFactory.Create(doc);
                workspace = doc.Workspace;
                var imports = pr.Imports;
                foreach (var d in imports)
                {
                    var dd = d.Replace("\\", "/");
                    // Import file names are in absolute path names. Change
                    // it back to relative paths.
                    var v = per_grammar.tool_grammar_tuples.Select(t => t.OriginalSourceFileName.Replace("\\","/"))
                        .Where(t => dd.EndsWith(t)).FirstOrDefault();
                    if (v == null) continue;
                    DirectedEdge<string> e = new DirectedEdge<string>() { From = v, To = f };
                    graph.AddEdge(e);
                }
            }
            var subset = graph.Vertices.ToList();
            var sort = new TopologicalSort<string, DirectedEdge<string>>(graph, subset);
            List<string> order = sort.Topological_sort();
            per_grammar.tool_grammar_tuples.Sort(new GrammarOrderCompare(order));
        }

        class GrammarOrderCompare : IComparer<GrammarTuple>
        {
            List<string> _order;
            public GrammarOrderCompare(List<string> order)
            {
                _order = order;
            }
            public int Compare(GrammarTuple x, GrammarTuple y)
            {
                if (x == null)
                {
                    if (y == null)
                        return 0;
                    else
                        return -1;
                }
                else
                {
                    if (y == null)
                        return 1;
                    else
                    {
                        try
                        {
                            var ix = _order.IndexOf(x.OriginalSourceFileName);
                            var iy = _order.IndexOf(y.OriginalSourceFileName);
                            return ix.CompareTo(iy);
                        }
                        catch(Exception e)
                        { }
                        // ...and y is not null, compare the
                        // if one is a parser vs lexer.
                        //
                        if (x.GrammarAutomName.EndsWith("Lexer") && y.GrammarAutomName.EndsWith("Lexer"))
                            return x.GrammarAutomName.CompareTo(y.GrammarAutomName);
                        else if (x.GrammarAutomName.EndsWith("Lexer") && y.GrammarAutomName.EndsWith("Parser"))
                            return -1;
                        else if (x.GrammarAutomName.EndsWith("Parser") && y.GrammarAutomName.EndsWith("Parser"))
                            return x.GrammarAutomName.CompareTo(y.GrammarAutomName);
                        else if (x.GrammarAutomName.EndsWith("Parser") && y.GrammarAutomName.EndsWith("Lexer"))
                            return 1;
                        else return 0;
                    }
                }
            }
        }
    }
}
