namespace Trash
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Antlr4.StringTemplate;
    using AntlrJson;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using ParseTreeEditing.UnvParseTreeDOM;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
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

        public int Execute(Config config)
        {
            if (config.pom)
                ModifyWithPom(config);
            else if (config.desc)
                ModifyWithDesc(config);
            else
                DoNonPomDirectedGenerate(config);
            ModifyWithGrammarParse(config);
            GenerateViaConfig(config);

            if (failed_modules.Any())
            {
                System.Console.WriteLine(String.Join(" ", failed_modules));
                return 1;
            }
            return 0;
        }

        private void ModifyWithGrammarParse(Config config)
        {
            if (config.arithmetic)
            {
                var test = new Test();
                config.Tests.Add(test);
                config.Files = new List<string>() { "Arithmetic.g4" };
                config.start_rule = "file_";
                test.os_targets = new List<string>() { GetOSTarget().ToString() };
                test.start_rule = config.start_rule;
            }

            // Let's first parse the input grammar files and gather information
            // about them. Note, people pump in all sorts of bullshit, so
            // be ready to handle the worse of the worse.
            foreach (var test in config.Tests)
            {
                if (!test.tool_grammar_files.Any())
                    test.tool_grammar_files = config.Files.ToList();
                test.tool_grammar_tuples = new List<GrammarTuple>();
                foreach (var f in test.tool_grammar_files)
                {
                    // We're going to assume that the grammars are in
                    // the current directory. That's because this is the Maven
                    // Antlr4 tool plugin so it doesn't fish around to find the grammar,
                    // except possibly in the "source directory".
                    string sgfn; // Where the grammar is.
                    string tgfn; // Where the grammar existed in the generated output parser directory.
                    var p = test.package.Replace(".", "/");
                    var pre = p == "" ? "" : p + "/";
                    if (test.target == "Antlr4cs" || test.target == "CSharp") pre = ""; // Erase. Packages don't need to be placed in a directory named for the package.
                    List<ParsingResultSet> pr = new List<ParsingResultSet>();
                    if (f == "Arithmetic.g4")
                    {
                        sgfn = f;
                        tgfn = pre + f;
                        string code = null;
                        if (config.template_sources_directory == null)
                        {
                            System.Reflection.Assembly a = this.GetType().Assembly;
                            code = ReadAllResource(a, "trgen.templates." + f);
                        }
                        else
                        {
                            code = File.ReadAllText(f);
                        }

                        DoParse(code, f, pr);
                    }
                    else
                    {
                        if (File.Exists(f))
                        {
                            sgfn = f;
                            //tgfn = per_grammar.package.Replace(".", "/") + (test.target == "Go" ? per_grammar.grammar_name + "/" : "") + f;
                            tgfn = pre + f;
                        }
                        else if (File.Exists(test.current_directory + f))
                        {
                            sgfn = test.current_directory + f;
                            tgfn = pre + f;
                        }
                        else if (File.Exists(test.target + "/" + f))
                        {
                            sgfn = test.target + "/" + f;
                            tgfn = pre + f;
                        }
                        else
                        {
                            System.Console.Error.WriteLine("Unknown grammar file" + f);
                            throw new Exception();
                        }

                        string code = null;
                        code = File.ReadAllText(sgfn);
                        DoParse(code, sgfn, pr);
                    }

                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                        new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    List<UnvParseTreeElement> is_par = null;
                    List<UnvParseTreeElement> is_lex = null;
                    List<string> name_ = null;
                    List<string> ss = null;
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(pr.First().Nodes.First(), pr.First().Parser))
                    {
                        is_par = engine.parseExpression(
                                @"/grammarSpec/grammarDecl/grammarType/PARSER",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                            .ToList();
                        is_lex = engine.parseExpression(
                                @"/grammarSpec/grammarDecl/grammarType/LEXER",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                        name_ = engine.parseExpression(
                                @"/grammarSpec/grammarDecl/identifier/(TOKEN_REF | RULE_REF)/text()",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as UnvParseTreeText).NodeValue as string).ToList();
                        ss = engine.parseExpression(
                                @"//parserRuleSpec[ruleBlock//TOKEN_REF/text()='EOF']/RULE_REF/text()",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as UnvParseTreeText).NodeValue as string).ToList();
                    }

                    var is_parser_grammar = is_par.Count() != 0;
                    var is_lexer_grammar = is_lex.Count() != 0;
                    var is_combined = !is_parser_grammar && !is_lexer_grammar;
                    var name = name_.First();
                    string autom_name = name;
                    name = is_parser_grammar ? new Regex("Parser$").Replace(name, "") : name;
                    name = is_lexer_grammar ? new Regex("Lexer$").Replace(name, "") : name;

                    var start_symbol = ss.FirstOrDefault();
                    string grammar_name = null;
                    grammar_name = name;
                    if (start_symbol != null && test.start_rule == null)
                    {
                        if (test.grammar_name == null
                            || test.grammar_name == grammar_name)
                        {
                            test.start_rule = start_symbol;
                            config.start_rule = start_symbol;
                        }
                    }
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

                    // The following code computes the names of the parser and lexer generated files,
                    // grammar name, constructor to call, etc. It duplicates some of the code in the Antlr
                    // tool:
                    // https://github.com/antlr/antlr4/blob/1b144fa7b40f6d1177c9e4f400a6a04f4103d02e/tool/src/org/antlr/v4/codegen/Target.java#L433
                    // => https://github.com/antlr/antlr4/blob/1b144fa7b40f6d1177c9e4f400a6a04f4103d02e/tool/src/org/antlr/v4/tool/Grammar.java#L595
                    // => https://github.com/antlr/antlr4/blob/1b144fa7b40f6d1177c9e4f400a6a04f4103d02e/tool/src/org/antlr/v4/tool/Grammar.java#L1164
                    // => https://github.com/antlr/antlr4/blob/1b144fa7b40f6d1177c9e4f400a6a04f4103d02e/tool/src/org/antlr/v4/codegen/target/GoTarget.java#L118
                    if (is_parser_grammar)
                    {
                        //var genfn = (test.target == "Go" ? name.Replace("Parser", "") + "/" : "") + name + Suffix(_config);
                        var p1 = test.package;
                        var pre1 = p1 == "" ? "" : p1 + "/";
                        var p2 = test.package.Replace("/", ".");
                        var pre2 = p2 == "" ? "" : p2 + ".";
                        string genfn; // name of the generated parser/lexer file in the output directory.
                        string genincfn; // name of the include file for parser/lexer, for C++.
                        string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                        string qual_autom_name; // The name of the parser or lexer function, fully qualified with package.
                        string goname; // The name of the parser or lexer functionj for Go.
                        if (test.target == "Go")
                        {
                            genfn = pre1 + autom_name.Replace("Parser", "_parser").ToLower() + Suffix(test.target);
                            genincfn = "";
                            if (test.package != null && test.package != "")
                                antlr_args = GetOSTarget() == OSTarget.Windows
                                    ? "-o " + test.package + " -lib " + test.package +
                                      " -package " + test.package
                                    : " -package " + test.package;
                            else
                                antlr_args = "";
                            qual_autom_name = pre2 + autom_name;
                            goname = pre2 + "New" + autom_name;
                        }
                        else
                        {
                            genfn = pre1 + autom_name + Suffix(test.target);
                            genincfn = pre1 + autom_name + ".h";
                            if (test.package != null && test.package != "")
                                antlr_args = GetOSTarget() == OSTarget.Windows
                                    ? "-o " + test.package + " -lib " + test.package +
                                      " -package " + test.package
                                    : " -package " + test.package;
                            else
                                antlr_args = "";
                            qual_autom_name = pre2 + autom_name;
                            goname = "";
                        }

                        var g = new GrammarTuple(GrammarTuple.Type.Parser, sgfn, tgfn, name, genfn, genincfn,
                            qual_autom_name, goname, antlr_args);
                        test.tool_grammar_tuples.Add(g);
                    }
                    else if (is_lexer_grammar)
                    {
                        //var genfn = (test.target == "Go" ? name.Replace("Lexer", "") + "/" : "") + name + Suffix(_config);
                        var p1 = test.package;
                        var pre1 = p1 == "" ? "" : p1 + "/";
                        var p2 = test.package.Replace("/", ".");
                        var pre2 = p2 == "" ? "" : p2 + ".";
                        string genfn; // name of the generated parser/lexer file in the output directory.
                        string genincfn; // name of the include file for parser/lexer, for C++.
                        string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                        string qual_autom_name; // The name of the parser or lexer function, fully qualified with package.
                        string goname; // The name of the parser or lexer functionj for Go.
                        if (test.target == "Go")
                        {
                            genfn = pre1 + autom_name.Replace("Lexer", "_lexer").ToLower() + Suffix(test.target);
                            genincfn = "";
                            if (test.package != null && test.package != "")
                                antlr_args = GetOSTarget() == OSTarget.Windows
                                    ? "-o " + test.package + " -lib " + test.package +
                                      " -package " + test.package
                                    : " -package " + test.package;
                            else
                                antlr_args = "";
                            qual_autom_name = pre2 + autom_name;
                            goname = pre2 + "New" + autom_name;
                        }
                        else
                        {
                            genfn = pre1 + autom_name + Suffix(test.target);
                            genincfn = pre1 + autom_name + ".h";
                            if (test.package != null && test.package != "")
                                antlr_args = GetOSTarget() == OSTarget.Windows
                                    ? "-o " + test.package + " -lib " + test.package +
                                      " -package " + test.package
                                    : " -package " + test.package;
                            else
                                antlr_args = "";
                            qual_autom_name = pre2 + autom_name;
                            goname = "";
                        }

                        var g = new GrammarTuple(GrammarTuple.Type.Lexer, sgfn, tgfn, name, genfn, genincfn, qual_autom_name,
                            goname, antlr_args);
                        test.tool_grammar_tuples.Add(g);
                    }
                    else
                    {
                        {
                            //var genfn = (test.target == "Go" ? name + "/" : "") + name + "Parser" + Suffix(_config);
                            var p1 = test.package;
                            var pre1 = p1 == "" ? "" : p1 + "/";
                            var p2 = test.package.Replace("/", ".");
                            var pre2 = p2 == "" ? "" : p2 + ".";
                            string genfn; // name of the generated parser/lexer file in the output directory.
                            string genincfn; // name of the include file for parser/lexer, for C++.
                            string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                            string qual_autom_name; // The name of the parser or lexer function, fully qualified with package.
                            string goname; // The name of the parser or lexer functionj for Go.
                            if (test.target == "Go")
                            {
                                genfn = pre1 + autom_name.ToLower() + "_parser" + Suffix(test.target);
                                genincfn = "";
                                qual_autom_name = pre2
                                                  + name
                                                  + "Parser";
                                goname = pre2
                                         + "New" + name
                                         + "Parser";
                                if (test.package != null && test.package != "")
                                    antlr_args = GetOSTarget() == OSTarget.Windows
                                        ? "-o " + test.package + " -lib " + test.package +
                                          " -package " + test.package
                                        : " -package " + test.package;
                                else
                                    antlr_args = "";
                            }
                            else
                            {
                                genfn = pre1 + name + "Parser" + Suffix(test.target);
                                genincfn = pre1 + name + "Parser.h";
                                qual_autom_name = pre2
                                                  + name
                                                  + "Parser";
                                goname = "";
                                if (test.package != null && test.package != "")
                                    antlr_args = GetOSTarget() == OSTarget.Windows
                                        ? "-o " + test.package + " -lib " + test.package +
                                          " -package " + test.package
                                        : " -package " + test.package;
                                else
                                    antlr_args = "";
                            }

                            var g = new GrammarTuple(GrammarTuple.Type.Parser, sgfn, tgfn, name, genfn, genincfn,
                                qual_autom_name, goname, antlr_args);
                            test.tool_grammar_tuples.Add(g);
                        }
                        {
                            //var genfn = (test.target == "Go" ? name + "/" : "") + name + "Lexer" + Suffix(_config);
                            var p1 = test.package;
                            var pre1 = p1 == "" ? "" : p1 + "/";
                            var p2 = test.package.Replace("/", ".");
                            var pre2 = p2 == "" ? "" : p2 + ".";
                            string genfn; // name of the generated parser/lexer file in the output directory.
                            string genincfn; // name of the include file for parser/lexer, for C++.
                            string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                            string qual_autom_name; // The name of the parser or lexer function, fully qualified with package.
                            string goname; // The name of the parser or lexer functionj for Go.
                            if (test.target == "Go")
                            {
                                genfn = pre1 + autom_name.ToLower() + "_lexer" + Suffix(test.target);
                                genincfn = "";
                                qual_autom_name = pre2
                                                  + name
                                                  + "Lexer";
                                goname = pre2
                                         + "New" + name
                                         + "Lexer";
                                if (test.package != null && test.package != "")
                                    antlr_args = GetOSTarget() == OSTarget.Windows
                                        ? "-o " + test.package + " -lib " + test.package +
                                          " -package " + test.package
                                        : " -package " + test.package;
                                else
                                    antlr_args = "";
                            }
                            else
                            {
                                genfn = pre1 + autom_name + "Lexer" + Suffix(test.target);
                                genincfn = pre1 + name + "Lexer.h";
                                qual_autom_name = pre2
                                                  + name
                                                  + "Lexer";
                                goname = "";
                                if (test.package != null && test.package != "")
                                    antlr_args = GetOSTarget() == OSTarget.Windows
                                        ? "-o " + test.package + " -lib " + test.package +
                                          " -package " + test.package
                                        : " -package " + test.package;
                                else
                                    antlr_args = "";
                            }

                            var g = new GrammarTuple(GrammarTuple.Type.Lexer, sgfn, tgfn, name, genfn, genincfn,
                                qual_autom_name, goname, antlr_args);
                            test.tool_grammar_tuples.Add(g);
                        }
                    }
                }

                // Pick a damn grammar if none specified. If more than one fuck it.
                if (test.grammar_name == null)
                {
                    var a = test.tool_grammar_tuples
                        .Where(t => t.WhatType == GrammarTuple.Type.Parser)
                        .FirstOrDefault()?.GrammarName;
                    if (a != null) test.grammar_name = a;
                    if (test.grammar_name == null)
                    {
                        var b = test.tool_grammar_tuples
                            .Where(t => t.WhatType == GrammarTuple.Type.Combined).FirstOrDefault()?.GrammarName;
                        if (b != null) test.grammar_name = b;
                    }
                }

                if (test.grammar_name == null)
                {
                    throw new Exception("Can't figure out the grammar name.");
                }

                // Sort tool_grammar_tuples because there are dependencies!
                // Note, we can't do that by name because some grammars, like
                // grammars-v4/r, won't build that way.
                // Use Trash compiler to get dependencies.
                ComputeSort(test);

                // How to call the parser in the source code. Remember, there are
                // actually up to two tests in the pom file, one for running the
                // Antlr tool, and the other to test the generated parser.
                test.fully_qualified_parser_name =
                    test.tool_grammar_tuples
                        .Where(t => t.WhatType == GrammarTuple.Type.Parser &&
                                    (t.GrammarName == test.grammar_name + "Parser"
                                     || t.GrammarName == test.grammar_name))
                        .Select(t => t.GrammarAutomName).First();
                test.fully_qualified_go_parser_name =
                    test.tool_grammar_tuples
                        .Where(t => t.WhatType == GrammarTuple.Type.Parser &&
                                    (t.GrammarName == test.grammar_name + "Parser"
                                     || t.GrammarName == test.grammar_name))
                        .Select(t => t.GrammarGoNewName).First();

                // Where the parser generated code lives.
                var parser_generated_file_name =
                    (string)test.fully_qualified_parser_name.Replace('.', '/')
                    + Suffix(test.target);
                var parser_generated_include_file_name =
                    (string)test.fully_qualified_parser_name.Replace('.', '/') + ".h";
                var parser_src_grammar_file_name =
                    test.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Parser"))
                        .Where(t => test.grammar_name == null || t.GrammarName == test.grammar_name)
                        .Select(t => t.GrammarFileName)
                        .First();
                test.fully_qualified_lexer_name =
                    test.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Lexer"))
                        .Select(t => t.GrammarAutomName)
                        .First();
                test.fully_qualified_go_lexer_name =
                    test.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Lexer"))
                        .Select(t => t.GrammarGoNewName)
                        .First();
                var lexer_generated_file_name =
                    test.fully_qualified_lexer_name.Replace('.', '/') + Suffix(test.target);
                var lexer_generated_include_file_name =
                    test.fully_qualified_lexer_name.Replace('.', '/') + ".h";
                var lexer_src_grammar_file_name =
                    test.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Lexer"))
                        .Select(t => t.GrammarFileName)
                        .First();
                test.tool_src_grammar_files = new HashSet<string>()
                {
                    lexer_src_grammar_file_name,
                    parser_src_grammar_file_name
                };
                //per_grammar.tool_grammar_tuples = new List<GrammarTuple>()
                //    {
                //        new GrammarTuple(lexer_grammar_file_name, lexer_generated_file_name, lexer_generated_include_file_name, per_grammar.fully_qualified_lexer_name),
                //        new GrammarTuple(parser_grammar_file_name, parser_generated_file_name, parser_generated_include_file_name, per_grammar.fully_qualified_parser_name),
                //    };
                test.tool_grammar_files = test.tool_grammar_tuples
                    .Select(t => t.GrammarFileName).ToHashSet().ToList();
                test.parser_grammar_file_name = parser_src_grammar_file_name;
                //test.generated_files = test.tool_grammar_tuples.Select(t => t.GeneratedFileName)
                //    .ToHashSet().ToList();
                test.lexer_grammar_file_name = lexer_src_grammar_file_name;
            }
        }

        public static string version = "0.21.14";

        // For maven-generated code.
        public List<string> failed_modules = new List<string>();

        public string Suffix(string target)
        {
            return target switch
            {
                "Antlr4cs" => ".cs",
                "Cpp" => ".cpp",
                "CSharp" => ".cs",
                "Dart" => ".dart",
                "Go" => ".go",
                "Java" => ".java",
                "JavaScript" => ".js",
                "PHP" => ".php",
                "Python2" => ".py",
                "Python3" => ".py",
                "Swift" => ".swift",
                "TypeScript" => ".ts",
                "Antlr4ng" => ".ts",
                _ => throw new NotImplementedException(),
            };
        }

        public string ignore_list_of_files = ".trgen-ignore";

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

        public static OSTarget GetOSTarget()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSTarget.Unix;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSTarget.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSTarget.Mac;
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
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return (home + "/.m2/antlr4-4.12.0-complete.jar").Replace('\\', '/');
        }

        public static string TargetName(string target)
        {
            return target;
            //return target switch
            //{
            //    "Antlr4cs" => "Antlr4cs",
            //    "Cpp" => "Cpp",
            //    "CSharp" => "CSharp",
            //    "Dart" => "Dart",
            //    "Go" => "Go",
            //    "Java" => "Java",
            //    "JavaScript" => "JavaScript",
            //    "PHP" => "PHP",
            //    "Python2" => "Python2",
            //    "Python3" => "Python3",
            //    "Swift" => "Swift",
            //    "TypeScript" => "TypeScript",
            //    _ => throw new NotImplementedException(),
            //};
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
                "PHP",
                "Python2",
                "Python3",
                "Swift",
                "TypeScript",
            };
            var filter = String.Join("/|", all_but.Where(t => t != TargetName(target)));
            return filter;
        }

        private void ModifyWithDesc(Config config)
        {
            System.Console.Error.WriteLine(Environment.CurrentDirectory);
            string file_name = Environment.CurrentDirectory + Path.DirectorySeparatorChar + @"desc.xml";
            if (!File.Exists(file_name))
            {
                DoNonPomDirectedGenerate(config);
                return;
            }

            XmlTextReader reader = new XmlTextReader(file_name);
            reader.Namespaces = false;
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(reader.NameTable);
            int gen = 0;
            List<string> test_targets = config.targets.ToList();
            {
                var xtargets = navigator
                    .Select("/desc/targets", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                if (xtargets.Count > 1)
                    throw new Exception("Too many <targets> elements, there should be only one.");
                if (xtargets.Count == 0)
                    throw new Exception("A <desc><targets> element is required.");
                test_targets = xtargets.First().Split(';').ToList();
                if (config.targets == null || !config.targets.Any()) config.targets = test_targets;
            }
            List<string> test_ostargets = config.os_targets.ToList();
            {
                var xtargets = navigator
                    .Select("/desc/os-targets", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                if (xtargets.Count > 1)
                    throw new Exception("Too many <os-targets> elements, there should be only one.");
                if (xtargets.Count != 0)
                {
                    test_ostargets = xtargets.First().Split(';').ToList();
                }
            }
            {
                var xgrammars = navigator
                    .Select("/desc/grammar-files", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                if (xgrammars.Count > 1)
                    throw new Exception("Too many <grammar-files> elements, there should be only one.");
                if (xgrammars.Count == 1)
                {
                    var grammars = xgrammars.First().Split(';');
                    var merged_list = new List<string>();
                    foreach (var x in grammars)
                    {
                        var xx = x.Trim().Replace("\\", "/");

                        // Split the dirname and basename of the path x.
                        var dir = Dirname(xx);
                        var bn = Basename(xx);

                        // Add the dirname to the current directory.
                        if (dir == ".") dir = ""; // erase.
                        if (dir != "" && !dir.EndsWith("/")) dir += "/";
                        var cwd = Environment.CurrentDirectory.Replace("\\", "/");
                        if (!cwd.EndsWith("/")) cwd += "/";
                        cwd = cwd + dir;

                        // Set the pattern to be the full path name.
                        var fp = cwd + bn;
                        var pp = TrashGlobbing.Glob.GlobToRegex(fp);

                        var list_pp = new TrashGlobbing.Glob(cwd)
                            .RegexContents(pp, false)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName
                                .Replace('\\', '/')
                                .Replace(cwd, ""))
                            .ToList();
                        foreach (var y in list_pp)
                        {
                            merged_list.Add(dir + y);
                        }
                    }

                    config.Files = merged_list;
                }
            }
            {
                var spec_grammar_name = navigator
                    .Select("/desc/grammar-name", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .FirstOrDefault();
                if (config.grammar_name == null && spec_grammar_name != null)
                {
                    config.grammar_name = spec_grammar_name.Trim();
                }
            }
            {
                var spec_examplesy = navigator
                    .Select("/desc/inputs", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .FirstOrDefault();
                if (config.example_files == null && spec_examplesy != null)
                {
                    config.example_files = spec_examplesy;
                }
            }
            {
                var spec_entry_point = navigator
                    .Select("/desc/entry-point", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .FirstOrDefault();
                if (config.start_rule == null && spec_entry_point != null)
                {
                    config.start_rule = spec_entry_point.Trim();
                }
            }
            {
                var parsing_type = navigator
                    .Select("/desc/parsing-type", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .FirstOrDefault();
                if (config.parsing_type == null) config.parsing_type = parsing_type;
            }
            {
                var spec_grammar_name = navigator
                    .Select("/desc/grammar-name", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .FirstOrDefault();
                if (config.grammar_name == null && spec_grammar_name != null)
                {
                    config.grammar_name = spec_grammar_name.Trim();
                }
            }
            var xtests = navigator
                .Select("/desc/test", nsmgr)
                .Cast<XPathNavigator>()
                .ToList();
            if (!xtests.Any())
            {
                var all = config.force ? config.targets : test_targets.Intersect(config.targets);
                foreach (var target in all)
                {
                    if (!test_ostargets.Contains(GetOSTarget().ToString())) continue;
                    var test = new Test();
                    test.os_targets = new List<string>() { GetOSTarget().ToString() };
                    test.target = target;
                    test.grammar_name = config.grammar_name;
                    test.start_rule = config.start_rule;
                    test.example_files = config.example_files;
                    if (test.example_files == null)
                    {
                        test.example_files = "examples";
                    }
                    if (!config.Files.Any())
                    {
                        var list = new List<string>();
                        var tool_grammar_files_pattern = ".+g4$";
                        var list_pp = new TrashGlobbing.Glob()
                            .RegexContents(tool_grammar_files_pattern, false)
                            .Where(f => f is FileInfo)
                            .Select(f => f.Name.Replace('\\', '/').Replace(Environment.CurrentDirectory, ""))
                            .ToList();
                        foreach (var y in list_pp)
                        {
                            list.Add(y);
                        }
                        config.Files = list;
                    }
                    test.tool_grammar_files = config.Files.ToList();
                    test.package = test.target == "Go" ? "parser" : test.package;
                    test.package = test.target == "Antlr4cs" ? "Test" : test.package;
                    if (test.parsing_type == null) test.parsing_type = config.parsing_type;
                    if (test.parsing_type == null) test.parsing_type = "group";
                    config.Tests.Add(test);
                }
            }
            else
            {
                // Create a test for each target listed in <test> elements.
                foreach (var xmltest in xtests)
                {
                    List<string> spec_antlr_tool_args = new List<string>();
                    var test_name = xmltest
                        .Select("name", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .ToList()
                        .FirstOrDefault();
                    {
                        var xostargets = xmltest
                            .Select("os-targets", nsmgr)
                            .Cast<XPathNavigator>()
                            .Select(t => t.Value)
                            .ToList();
                        if (xostargets.Count > 1)
                            throw new Exception("Too many <os-targets> elements, there should be only one.");
                        if (xostargets.Count != 0)
                        {
                            test_ostargets = xostargets.First().Split(';').ToList();
                        } 
                    }
                    var parsing_type = xmltest
                        .Select("parsing-type", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var xtargets = xmltest
                        .Select("targets", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .ToList();
                    if (xtargets.Count > 1)
                        throw new Exception("Too many <targets> elements, there should be only one.");
                    if (xtargets.Count == 0)
                    {
                        test_targets = config.targets.ToList();
                    }
                    else
                    {
                        test_targets = xtargets.First().Split(';').ToList();
                    }

                    List<string> tool_grammar_files = null;
                    var xgrammars = xmltest
                        .Select("grammar-files", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .ToList();
                    if (xgrammars.Count > 1)
                        throw new Exception("Too many <grammar-files> elements, there should be only one.");
                    if (xgrammars.Count == 1)
                    {
                        var grammars = xgrammars.First().Split(';');
                        var merged_list = new List<string>();
                        foreach (var x in grammars)
                        {
                            var xx = x.Trim().Replace("\\", "/");

                            // Split the dirname and basename of the path x.
                            var dir = Dirname(xx);
                            var bn = Basename(xx);

                            // Add the dirname to the current directory.
                            if (dir == ".") dir = ""; // erase.
                            if (dir != "" && !dir.EndsWith("/")) dir += "/";
                            var cwd = Environment.CurrentDirectory.Replace("\\", "/");
                            if (!cwd.EndsWith("/")) cwd += "/";
                            cwd = cwd + dir;

                            // Set the pattern to be the full path name.
                            var fp = cwd + bn;
                            var pp = TrashGlobbing.Glob.GlobToRegex(fp);

                            var list_pp = new TrashGlobbing.Glob(cwd)
                                .RegexContents(pp, false)
                                .Where(f => f is FileInfo)
                                .Select(f => f.FullName
                                    .Replace('\\', '/')
                                    .Replace(cwd, ""))
                                .ToList();
                            foreach (var y in list_pp)
                            {
                                merged_list.Add(dir + y);
                            }
                        }
                        tool_grammar_files = merged_list;
                    }

                    var spec_source_directory = xmltest
                        .Select("source-directory", nsmgr)
                        .Cast<XPathNavigator>()
                        .Where(t => t.Value != "")
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_grammar_name = xmltest
                        .Select("grammar-name", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_lexer_name = xmltest
                        .Select("lexer-name", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_entry_point = xmltest
                        .Select("entry-point", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_package_name = xmltest
                        .Select("package-name", nsmgr)
                        .Cast<XPathNavigator>()
                        .Where(t => t.Value != "")
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_examplesy = xmltest
                        .Select("inputs", nsmgr)
                        .Cast<XPathNavigator>()
                        .Where(t => t.Value != "")
                        .Select(t => t.Value)
			            .FirstOrDefault();
                    var all = config.force ? config.targets : test_targets.Intersect(config.targets);
                    foreach (var target in all)
                    {
                        if (!test_ostargets.Contains(GetOSTarget().ToString())) continue;
                        var test = new Test();
                        test.target = target;
                        test.os_targets = new List<string>() { GetOSTarget().ToString() };
                        test.package = test.target == "Go" ? "parser" : test.package;
                        test.package = test.target == "Antlr4cs" ? "Test" : test.package;
                        if (tool_grammar_files != null)
                        {
                            test.tool_grammar_files = tool_grammar_files;
                        }
                        else
                        {
                            if (!config.Files.Any())
                            {
                                var list = new List<string>();
                                var tool_grammar_files_pattern = ".+g4$";
                                var list_pp = new TrashGlobbing.Glob()
                                    .RegexContents(tool_grammar_files_pattern, false)
                                    .Where(f => f is FileInfo)
                                    .Select(f => f.Name.Replace('\\', '/').Replace(Environment.CurrentDirectory, ""))
                                    .ToList();
                                foreach (var y in list_pp)
                                {
                                    list.Add(y);
                                }
                                test.tool_grammar_files = list;
                            }
                            else test.tool_grammar_files = config.Files.ToList();
                        }
                        if (spec_grammar_name != null)
                        {
                            test.grammar_name = spec_grammar_name.Trim();
                        } else if (config.grammar_name != null)
                        {
                            test.grammar_name = config.grammar_name.Trim();
                        }

                        if (spec_examplesy != null)
                        {
                            test.example_files = spec_examplesy;
                        } else if (config.example_files != null)
                        {
                            test.example_files = config.example_files.Trim();
                        }
                        else
                        {
                            test.example_files = "examples";
                        }

                        if (spec_antlr_tool_args.Contains("-package"))
                        {
                            var ns = spec_antlr_tool_args[spec_antlr_tool_args.IndexOf("-package") + 1];
                            test.package = ns;
                        }
                        else if (spec_package_name != null)
                        {
                            test.package = spec_package_name;
                        }

                        if (spec_source_directory != null)
                        {
                            test.current_directory = spec_source_directory
                                .Replace("${basedir}", "")
                                .Trim();
                            if (test.current_directory.StartsWith('/'))
                                test.current_directory = test.current_directory.Substring(1);
                            if (test.current_directory != "" && !test.current_directory.EndsWith("/"))
                            {
                                test.current_directory = test.current_directory + "/";
                            }
                        }
                        else
                        {
                            test.current_directory = "";
                        }

                        // Check for existence of .trgen-ignore file.
                        // If there is one, read and create pattern of what to ignore.
                        if (File.Exists(ignore_list_of_files))
                        {
                            var ignore = new StringBuilder();
                            var lines = File.ReadAllLines(ignore_list_of_files);
                            var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                            test.ignore_string = string.Join("|", ignore_lines);
                        }
                        else test.ignore_string = null;

                        if (!(test.target == "JavaScript" || test.target == "Dart" || test.target == "TypeScript"))
                        {
                            List<string> additional = new List<string>();
                            config.antlr_tool_args = additional;
                            // On Linux, the flies are automatically place in the package,
                            // and they cannot be changed!
                            if (test.package != null && test.package != "")
                            {
                                if (GetOSTarget() == OSTarget.Windows)
                                {
                                    additional.Add("-o");
                                    additional.Add(test.package.Replace('.', '/'));
                                }

                                additional.Add("-lib");
                                additional.Add(test.package.Replace('.', '/'));
                            }
                        }

                        test.package = (spec_package_name != null && spec_package_name.Any()
                            ? spec_package_name.First() + "/"
                            : "");
                        test.package = test.target == "Go" ? "parser" : test.package;
                        test.package = test.target == "Antlr4cs" ? "Test" : test.package;
                        test.start_rule = config.start_rule != null && config.start_rule != ""
                            ? config.start_rule
                            : spec_entry_point;
                        test.test_name = test_name ?? (gen++).ToString();
                        test.parsing_type = parsing_type;
                        if (test.parsing_type == null) test.parsing_type = config.parsing_type;
                        if (test.parsing_type == null) test.parsing_type = "group";
                        config.Tests.Add(test);
                    }
                }
                // Any target that was not created under a <test>, create here if config.targets mentions that target.
                foreach (var t in config.targets)
                {
                    bool test_handled = false;
                    foreach (var xt in config.Tests)
                    {
                        if (xt.target == t)
                        {
                            test_handled = true;
                            break;
                        }
                    }

                    if (test_handled) continue;
                    System.Console.WriteLine("Need a test for " + t);
                }
            }
        }

        static string Dirname(string path)
        {
            // Split the path into parts separated by '/'
            string[] parts = path.Split('/');

            // If there is only one part, return "."
            if (parts.Length == 1)
            {
                return ".";
            }

            // Remove the last part and join the remaining parts with '/'
            return string.Join("/", parts.Take(parts.Length - 1));
        }

        // Returns the base name (i.e., the file name) of a given path
        static string Basename(string path)
        {
            // Split the path into parts separated by '/'
            string[] parts = path.Split('/');

            // Return the last part
            return parts.LastOrDefault();
        }

        public void ModifyWithPom(Config config)
        {
            System.Console.Error.WriteLine(Environment.CurrentDirectory);
            XmlTextReader reader = new XmlTextReader(Environment.CurrentDirectory + Path.DirectorySeparatorChar + @"pom.xml");
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
                return;
            }
            //
            // Process grammar pom.xml here.
            //
            List<string> pom_includes = null;
            List<string> pom_grammars = null;
            List<string> pom_antlr_tool_args = null;
            List<string> pom_source_directory = null;
            List<XPathNavigator> pom_all_else = null;
            List<string> pom_grammar_name = null;
            List<string> pom_lexer_name = null;
            List<string> pom_entry_point = null;
            List<string> pom_package_name = null;
            List<string> pom_case_insensitive_type = null;
            List<string> pom_example_files = null;
            var test = new Test();
            config.Tests.Add(test);
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
                // Disable for now.
                // throw new Exception();
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
                // Disable for now.
                // throw new Exception();
            }
            // The grammar name in pom is a mess. That's because
            // people define multiple parser grammars in the pom includes/grammars,
            // and a driver (see grammars-v4/r). So, take it on faith.
            test.grammar_name = pom_grammar_name.First();
            // Pom is a mess. There are many cases here in computing the namespace/package.
            // People even add bullshit @headers in the grammar; minds of a planaria.
            // -package arg specified; source top level
            //   => keep .g4 at top level, generate to directory
            //      corresponding to arg.
            if (pom_antlr_tool_args.Contains("-package"))
            {
                var ns = pom_antlr_tool_args[pom_antlr_tool_args.IndexOf("-package") + 1];
                test.package = ns;
            }
            else if (pom_package_name.Any())
            {
                test.package = pom_package_name.First();
            }
            // Make sure all the grammar files actually existance.
            // People don't check their bullshit.
            var merged_list = new HashSet<string>();
            foreach (var p in pom_includes) merged_list.Add(p);
            foreach (var p in pom_grammars) merged_list.Add(p);
            foreach (var x in merged_list.ToList())
            {
                if (!new TrashGlobbing.Glob()
                 .RegexContents(x)
                 .Where(f => f is FileInfo)
                 .Select(f => f.FullName.Replace('\\', '/').Replace(Environment.CurrentDirectory, ""))
                 .Any())
                {
                    System.Console.Error.WriteLine("Error in pom.xml: <include>" + x + "</include> is for a file that does not exist.");
                    throw new Exception();
                }
            }
            // Check existance of example files.
            if (pom_example_files.Any())
            {
                test.example_files = pom_example_files.First();
                if (!Directory.Exists(pom_example_files.First()))
                {
                    System.Console.Error.WriteLine("Examples directory doesn't exist " + pom_example_files.First());
                }
            }
            else
            {
                test.example_files = "examples";
            }
            if (pom_source_directory.Any())
            {
                test.current_directory = pom_source_directory
                    .First()
                    .Replace("${basedir}", "")
                    .Trim();
                if (test.current_directory.StartsWith('/')) test.current_directory = test.current_directory.Substring(1);
                if (test.current_directory != "" && !test.current_directory.EndsWith("/"))
                {
                    test.current_directory = test.current_directory + "/";
                }
            }
            else
            {
                test.current_directory = "";
            }
            test.case_insensitive_type = null;
            if (pom_case_insensitive_type.Any())
            {
                if (pom_case_insensitive_type.First().ToUpper() == "UPPER")
                    test.case_insensitive_type = CaseInsensitiveType.Upper;
                else if (pom_case_insensitive_type.First().ToUpper() == "LOWER")
                    test.case_insensitive_type = CaseInsensitiveType.Lower;
                else
                {
                    System.Console.Error.WriteLine("Case fold has invalid value: '"
                    + pom_case_insensitive_type.First() + "'.");
                }
                //else throw new Exception("Case fold has invalid value: '"
                //    + pom_case_insensitive_type.First() + "'.");
            }
            else test.case_insensitive_type = null;
            // Check for existence of .trgen-ignore file.
            // If there is one, read and create pattern of what to ignore.
            if (File.Exists(ignore_list_of_files))
            {
                var ignore = new StringBuilder();
                var lines = File.ReadAllLines(ignore_list_of_files);
                var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                test.ignore_string = string.Join("|", ignore_lines);
            }
            else test.ignore_string = null;
            if (!(test.target == "JavaScript" || test.target == "Dart" || test.target == "TypeScript"))
            {
                List<string> additional = new List<string>();
                config.antlr_tool_args = additional;
                // On Linux, the flies are automatically place in the package,
                // and they cannot be changed!
                if (test.package != null && test.package != "")
                {
                    if (GetOSTarget() == OSTarget.Windows)
                    {
                        additional.Add("-o");
                        additional.Add(test.package.Replace('.', '/'));
                    }
                    additional.Add("-lib");
                    additional.Add(test.package.Replace('.', '/'));
                }
            }
            test.package = test.target == "Go" ? "parser" : test.package;
            test.package = test.target == "Antlr4cs" ? "Test" : test.package;
            test.package = (pom_package_name != null && pom_package_name.Any() ? pom_package_name.First() + "/" : "");
            test.start_rule = config.start_rule != null && config.start_rule != "" ? config.start_rule : pom_entry_point.First();
            if (test.parsing_type == null) test.parsing_type = config.parsing_type;
            if (test.parsing_type == null) test.parsing_type = "group";
            test.os_targets = new List<string>() { GetOSTarget().ToString() };
        }

        public void DoNonPomDirectedGenerate(Config config)
        {
            {
                var all = "CSharp;Cpp;Dart;Go;Java;JavaScript;PHP;Python3;TypeScript".Split(';').ToList();
                if (config.targets != null && config.targets.Count() > 0) all = config.targets.ToList();
                foreach (var target in all)
                {
                    var test = new Test();
                    test.os_targets = new List<string>() { GetOSTarget().ToString() };
                    test.target = target;
                    test.grammar_name = config.grammar_name;
                    test.start_rule = config.start_rule;
                    if (!config.Files.Any())
                    {
                        var list = new List<string>();
                        var tool_grammar_files_pattern = ".+g4$";
                        var list_pp = new TrashGlobbing.Glob()
                            .RegexContents(tool_grammar_files_pattern, false)
                            .Where(f => f is FileInfo)
                            .Select(f => f.Name.Replace('\\', '/').Replace(Environment.CurrentDirectory, ""))
                            .ToList();
                        foreach (var y in list_pp)
                        {
                            list.Add(y);
                        }
                        config.Files = list;
                    }
                    test.tool_grammar_files = config.Files.ToList();
                    test.package = test.target == "Go" ? "parser" : test.package;
                    test.package = test.target == "Antlr4cs" ? "Test" : test.package;
                    if (test.parsing_type == null) test.parsing_type = config.parsing_type;
                    if (test.parsing_type == null) test.parsing_type = "group";
                    config.Tests.Add(test);
                }
            }
            foreach (var test in config.Tests)
            {
                test.current_directory = config.root_directory;
                if (test.current_directory != "" && !test.current_directory.EndsWith("/"))
                    test.current_directory = test.current_directory + "/";
                // Check for existence of .trgen-ignore file.
                // If there is one, read and create pattern of what to ignore.
                if (File.Exists(ignore_list_of_files))
                {
                    var ignore = new StringBuilder();
                    var lines = File.ReadAllLines(ignore_list_of_files);
                    var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                    test.ignore_string = string.Join("|", ignore_lines);
                }
                else test.ignore_string = null;

                test.start_rule = config.start_rule;
                test.example_files = "examples";
                test.fully_qualified_lexer_name = "";
                test.fully_qualified_parser_name = "";
                test.package = test.target == "Go" ? "parser" : "";
                var all_grammars_pattern = "^(?!.*(" +
                                           (test.ignore_string != null
                                               ? test.ignore_string + "|"
                                               : "")
                                           + "/ignore/|/Generated/|/Generated-[^/]*/|/target/|/examples/|/.git/|/.gitignore/|/.ignore/|"
                                           + Command.AllButTargetName(test.target)
                                           + "/)).+[.]g4"
                                           + "$";
                var grammar_list = new TrashGlobbing.Glob(test.current_directory)
                    .RegexContents(all_grammars_pattern)
                    .Where(f =>
                    {
                        if (f.Attributes.HasFlag(FileAttributes.Directory)) return false;
                        if (f is DirectoryInfo) return false;
                        return true;
                    })
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .Where(f =>
                    {
                        if (test.fully_qualified_parser_name != "ArithmeticParser" &&
                            f == "./Arithmetic.g4") return false;
                        if (f == "./files") return false;
                        return true;
                    }).Select(f => f.Replace(test.current_directory, "")).ToHashSet();
            }
        }

        public void GenerateViaConfig(Config config)
        {
            foreach (var test in config.Tests)
            {
                try
                {
                    // Create a directory containing target build files.
                    Directory.CreateDirectory((string)config.output_directory
                        + '-'
                        + test.target
                        + (test.test_name != null ? ('-' + test.test_name) : ""));
                }
                catch (Exception)
                {
                    throw;
                }

                // Find all source files.
                test.all_target_files = new List<string>();
                var all_source_pattern = "^(?!.*(" +
                                         (test.ignore_string != null
                                             ? test.ignore_string + "|"
                                             : "")
                                         + "ignore/|Generated/|Generated-[^/]*/|target/|examples/|.git/|.gitignore|"
                                         + Command.AllButTargetName(test.target)
                                         + "/)).+"
                                         + "$";
                test.grammar_directory_source_files = new TrashGlobbing.Glob()
                    .RegexContents(all_source_pattern)
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
                GenFromTemplates(config, test);
                AddSource(config, test);
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
            var names = a.GetManifestResourceNames();
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        byte[] ReadBytesResource(System.Reflection.Assembly a, string resourceName)
        {
            var names = a.GetManifestResourceNames();
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return reader.ReadAllBytes();
            }
        }

        public void AddSource(Config config, Test test)
        {
            var cd = Environment.CurrentDirectory + "/";
            cd = cd.Replace('\\', '/');
            var set = new HashSet<string>();
            foreach (var path in test.grammar_directory_source_files)
            {
                // Construct proper starting directory based on namespace.
                var from = path;
                var f = from.Substring(cd.Length);
                string to = null;
                if (test.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileName).Any())
                {
                    to = config.output_directory
                         + "-"
                         + test.target
                         + (test.test_name != null ? ("-" + test.test_name) : "")
                         + "/"
                         + test.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileName).First();
                }
                else
                {
                    // Now remove target directory.
                    if (test.target == "Go" && f.EndsWith(".go"))
                    {
                        to = config.output_directory
                             + "-"
                             + test.target
                             + (test.test_name != null ? ("-" + test.test_name) : "")
                             + "/" + "parser" + f.Substring(test.target.Length);
                    }
                    else {
                        f = (
                            f.StartsWith(
                                Command.TargetName(test.target) + '/')
                            ? f.Substring((Command.TargetName(test.target) + '/').Length)
                            : f
                            );
                    }
                    // Remove "src/main/java", a royal hangover from the Maven plugin.
                    f = (
                            f.StartsWith("src/main/java/")
                            ? f.Substring("src/main/java".Length)
                            : f
                            ); 
                    to = FixedName(f, config, test);
                }
                if (from.Contains("pom.xml") && test.target != "Java") continue;
                System.Console.Error.WriteLine("Copying source file from "
                  + from
                  + " to "
                  + to);
                test.all_target_files.Add(to);
                this.CopyFile(from, to);
            }
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
     
        private void GenFromTemplates(Config config, Test test)
        {
            var append_namespace = (!(test.target == "CSharp" || test.target == "Antlr4cs"));
            List<string> template_directory_files_to_copy;
            ZipArchive za = null;
            string prefix_to_remove = "";
            if (config.template_sources_directory == null)
            {
                System.Reflection.Assembly a = this.GetType().Assembly;
                // Load resource file that contains the names of all files in templates/ directory,
                // which were obtained by doing "cd templates/; find . -type f > files" at a Bash
                // shell.
                var zip = ReadBytesResource(a, "trgen.foobar.zip");
                MemoryStream stream = new MemoryStream(zip);
                var regex_string = "^(?!.*(" + AllButTargetName(test.target) + "/)).*$";
                var regex = new Regex(regex_string);
                za = new ZipArchive(stream);
                template_directory_files_to_copy = za.Entries.Where(f =>
                {
                    var fn = f.FullName;
                    if (test.fully_qualified_parser_name != "ArithmeticParser" && fn == "Arithmetic.g4.stg")
                        return false;
                    if (fn == "files") return false;
                    var v = regex.IsMatch(fn);
                    return v;
                }).Select(f => f.FullName).ToList();
            }
            else
            {
                prefix_to_remove = config.template_sources_directory + '/';
                prefix_to_remove = prefix_to_remove.Replace("\\", "/");
                prefix_to_remove = prefix_to_remove.Replace("//", "/");

                var regex_string = "^(?!.*(files|" + AllButTargetName(test.target) + "/)).*$";
                template_directory_files_to_copy = new TrashGlobbing.Glob(config.template_sources_directory)
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
                        if (test.fully_qualified_parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
                        if (f == "./files") return false;
                        return true;
                    })
                    .Select(f =>
                        f.Substring(prefix_to_remove.Length))
                    .ToList();
            }

            var set = new HashSet<string>();
            foreach (var file in template_directory_files_to_copy)
            {
                var from = file;

                // copy the file straight up if it doesn't begin
                // with target directory name. Otherwise,
                // remove the target dir name.
                if (file.Contains("Arithmetic.g4")
                    && test.grammar_name != "Arithmetic"
                    && test.tool_src_grammar_files.Any())
                {
                    continue;
                }

                var base_name = Basename(from);
                var dir_name = Dirname(from);

                Template t;

                //if (config.template_sources_directory == null)
                //{
                //    var e = file.Substring(prefix_to_remove.Length);
                //    var to = FixedTemplatedFileName(e, config, test);
                //}

                string to = FixedTemplatedFileName(from, config, test);
                var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');

                Directory.CreateDirectory(q);
                string content;
                if (config.template_sources_directory == null)
                {
                    content = za.Entries.Where(x => x.FullName == file).Select(x =>
                    {
                        using (var r = new StreamReader(x.Open()))
                        {
                            var ss = r.ReadToEnd();
                            return ss;
                        }
                    }).FirstOrDefault();
                }
                else
                {
                    content = File.ReadAllText(prefix_to_remove + from);
                }

                System.Console.Error.WriteLine("Rendering template file from "
                                               + from
                                               + " to "
                                               + to);

                // There are three types of files in template area:
                // StringTemplateGroup, StringTemplate, and Plain.
                if (base_name.StartsWith("stg-"))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("stg-".Length);
                    TemplateGroupString tg = new TemplateGroupString(content);
                    t = tg.GetInstanceOf("start");
                }
                else if (base_name.StartsWith("stg."))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("stg.".Length);
                    TemplateGroupString tg = new TemplateGroupString(content);
                    t = tg.GetInstanceOf("start");
                }
                else if (base_name.EndsWith(".stg"))
                {
                    to = to.Substring(0, to.Length - ".stg".Length);
                    TemplateGroupString tg = new TemplateGroupString(content);
                    t = tg.GetInstanceOf("start");
                }
                else if (base_name.StartsWith("st-"))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("st-".Length);
                    t = new Template(content);
                }
                else if (base_name.StartsWith("st."))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("st.".Length);
                    t = new Template(content);
                }
                else if (base_name.EndsWith(".st"))
                {
                    to = to.Substring(0, to.Length - ".st".Length);
                    t = new Template(content);
                }
                else
                {
                    // Copy as is.
                    File.WriteAllText(to, content);
                    continue;
                }

                string output_dir = to;
                for (;;)
                {
                    var d = Dirname(output_dir);
                    if (d != null && d != "" && d != ".")
                    {
                        output_dir = d;
                    }
                    else break;
                }
                output_dir = output_dir + "/";
                var yo1 = test.grammar_directory_source_files
                    .Select(t =>
                        FixedName(t, config, test)
                            .Substring(output_dir.Length))
                    .Where(t => t.Contains(Suffix(test.target)))
                    .ToList();
		        t.Add("target", test.target);
                t.Add("additional_sources", yo1);
                t.Add("additional_targets", test.all_target_files.Where(xx =>
                    {
                        var ext = Path.GetExtension(xx);
                        return Suffix(test.target).Contains(ext);
                    })
                    .Select(t => t.Substring(config.output_directory.Length))
                    .ToList());
                t.Add("antlr_encoding", test.antlr_encoding);
                t.Add("antlr_tool_args", config.antlr_tool_args);
                t.Add("antlr_tool_path", config.antlr_tool_path);
                if (test.start_rule == null) t.Add("cap_start_symbol", Cap("no_start_rule_declared"));
                else t.Add("cap_start_symbol", Cap(test.start_rule));
                t.Add("case_insensitive_type", test.case_insensitive_type);
                t.Add("cli_bash", test.os_targets.Contains(OSTarget.Unix.ToString()));
                t.Add("cli_cmd", GetOSTarget() == OSTarget.Windows);
                t.Add("cmake_target", GetOSTarget() == OSTarget.Windows
                    ? "-G \"Visual Studio 17 2022\" -A x64"
                    : "");
                t.Add("example_files_unix", RemoveTrailingSlash(test.example_files.Replace('\\', '/')));
                t.Add("example_files_win", RemoveTrailingSlash(test.example_files.Replace('/', '\\')));
                t.Add("exec_name", GetOSTarget() == OSTarget.Windows ? "Test.exe" : "Test");
                t.Add("go_lexer_name", test.fully_qualified_go_lexer_name);
                t.Add("go_parser_name", test.fully_qualified_go_parser_name);
                t.Add("grammar_file", test.tool_grammar_files.First());
                t.Add("grammar_name", test.grammar_name);
                t.Add("has_name_space", test.package != null && test.package != "");
                t.Add("is_combined_grammar", test.tool_grammar_files.Count() == 1);
                t.Add("lexer_grammar_file", test.lexer_grammar_file_name);
                t.Add("lexer_name", test.fully_qualified_lexer_name);
                t.Add("name_space", test.package.Replace("/", "."));
                t.Add("package_name", test.package.Replace(".", "/"));
                t.Add("group_parsing", test.parsing_type == "group");
                t.Add("individual_parsing", test.parsing_type == "individual");
                t.Add("os_type", config.os_targets.First().ToString());
                t.Add("os_win", GetOSTarget() == OSTarget.Windows);
                t.Add("parser_name", test.fully_qualified_parser_name);
                t.Add("parser_grammar_file", test.parser_grammar_file_name);
                t.Add("path_sep_colon", config.path_sep == PathSepType.Colon);
                t.Add("path_sep_semi", config.path_sep == PathSepType.Semi);
                t.Add("start_symbol", test.start_rule);
                t.Add("temp_dir", GetOSTarget() == OSTarget.Windows
                    ? "c:/temp"
                    : "/tmp");
                t.Add("tool_grammar_files", test.tool_grammar_files);
                t.Add("tool_grammar_tuples", test.tool_grammar_tuples);
                t.Add("version", Command.version);
                var o = t.Render();
                File.WriteAllText(to, o);
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

        void ComputeSort(Test test)
        {
            Digraph<string> graph = new Digraph<string>();
            foreach (var t in test.tool_grammar_tuples)
            {
                var f = t.OriginalSourceFileName;
                // First approximation. If a parser, make dependent on lexer.
                if (t.WhatType == GrammarTuple.Type.Parser)
                {
                    foreach (var u in test.tool_grammar_tuples)
                    {
                        if (u.WhatType == GrammarTuple.Type.Lexer)
                        {
                            var v = u.OriginalSourceFileName;
                            DirectedEdge<string> e = new DirectedEdge<string>() { From = v, To = f };
                            graph.AddEdge(e);
                        }
                    }
                }
            }
            var subset = graph.Vertices.ToList();
            var sort = new TopologicalSort<string, DirectedEdge<string>>(graph, subset);
            List<string> order = sort.Topological_sort();
            test.tool_grammar_tuples.Sort(new GrammarOrderCompare(order));
        }

        string FixedName(string from, Config config, Test test)
        {
            string to = null;
            var cwd = System.Environment.CurrentDirectory.Replace("\\", "/");
            if (cwd != "" && !cwd.EndsWith("/")) cwd = cwd + "/";

            if (from.StartsWith(cwd))
                from = from.Substring(cwd.Length);

            if (from.StartsWith(test.target + "/"))
                from = from.Substring(test.target.Length+1);

            // Split the dirname and basename of the path x.
            var dir = Dirname(from);
            if (dir == ".") dir = "";
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";
            if (dir.StartsWith(cwd)) dir = dir.Substring(cwd.Length);
            if (dir.StartsWith(test.target + "/")) dir = dir.Substring(test.target.Length + 1);
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";

            var bn = Basename(from);

            to = dir + bn;

            to = config.output_directory
                 + "-"
                 + test.target
                 + (test.test_name != null ? ("-" + test.test_name) : "")
                 + '/'
                 + (test.target == "Go" && test.package != "" && (bn.EndsWith(".g4") || bn.EndsWith(".go")) ? test.package + "/" : "")
                 + to;
            to = to.Replace('\\', '/');
            return to;
        }

        string FixedTemplatedFileName(string from, Config config, Test test)
        {
            string to = null;
            var cwd = System.Environment.CurrentDirectory.Replace("\\", "/");
            if (cwd != "" && !cwd.EndsWith("/")) cwd = cwd + "/";

            // Split the dirname and basename of the path x.
            var dir = Dirname(from);
            if (dir == ".") dir = "";
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";
            if (dir.StartsWith(cwd)) dir = dir.Substring(cwd.Length);
            if (dir.StartsWith(test.target + "/")) dir = dir.Substring(test.target.Length + 1);
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";

            var bn = Basename(from);

            to = dir + bn;

            to = config.output_directory
                 + "-"
                 + test.target
                 + (test.test_name != null ? ("-" + test.test_name) : "")
                 + '/'
                 + to;
            to = to.Replace('\\', '/');
            return to;
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
                        catch(Exception)
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

        int DoParse(string txt, string input_name, List<AntlrJson.ParsingResultSet> data)
        {
            System.Reflection.Assembly a = this.GetType().Assembly;
            var path = a.Location;// + Path.DirectorySeparatorChar;
            var full_path = Path.GetFullPath(path);
            full_path = Path.GetDirectoryName(full_path);
            full_path = full_path.Replace("\\", "/");
            if (!full_path.EndsWith("/")) full_path = full_path + "/";
            Assembly asm = Assembly.LoadFile(full_path + "antlr4.dll");
            Type[] types = asm.GetTypes();
            Type type = asm.GetType("Program");

            MethodInfo methodInfo = type.GetMethod("SetupParse2");
            object[] parm1 = new object[] { txt, false };
            var res = methodInfo.Invoke(null, parm1);

            MethodInfo methodInfo2 = type.GetMethod("Parse2");
            object[] parm2 = new object[] { };
            DateTime before = DateTime.Now;
            var res2 = methodInfo2.Invoke(null, parm2);
            DateTime after = DateTime.Now;

            MethodInfo methodInfo3 = type.GetMethod("AnyErrors");
            object[] parm3 = new object[] { };
            var res3 = methodInfo3.Invoke(null, parm3);
            var result = "";
            if ((bool)res3)
            {
                result = "fail";
            }
            else
            {
                result = "success";
            }
            System.Console.Error.WriteLine("CSharp " + " " + input_name + " " + result + " " + (after - before).TotalSeconds);
            var parser = type.GetProperty("Parser").GetValue(null, new object[0]) as Antlr4.Runtime.Parser;
            var lexer = type.GetProperty("Lexer").GetValue(null, new object[0]) as Antlr4.Runtime.Lexer;
            var tokstream = type.GetProperty("TokenStream").GetValue(null, new object[0]) as ITokenStream;
            var charstream = type.GetProperty("CharStream").GetValue(null, new object[0]) as ICharStream;
            var commontokstream = tokstream as CommonTokenStream;
            var r5 = type.GetProperty("Input").GetValue(null, new object[0]);
            var tree = res2 as IParseTree;
            var t2 = tree as ParserRuleContext;
            //if (!config.Quiet) System.Console.Error.WriteLine("Time to parse: " + (after - before));
            //if (!config.Quiet) System.Console.Error.WriteLine("# tokens per sec = " + tokstream.Size / (after - before).TotalSeconds);
            //if (!config.Quiet && config.Verbose) System.Console.Error.WriteLine(LanguageServer.TreeOutput.OutputTree(tree, lexer, parser, commontokstream));
            
            var converted_tree = new ConvertToDOM().BottomUpConvert(t2, null, parser, lexer, commontokstream, charstream);
            var tuple = new AntlrJson.ParsingResultSet() { Text = (r5 as string), FileName = "stdin", Nodes = new UnvParseTreeNode[] { converted_tree }, Parser = parser, Lexer = lexer };
            data.Add(tuple);
            return (bool)res3 ? 1 : 0;
        }

    }

    public static class BinaryReaderExtensions
    {

        public static byte[] ReadBytesToEnd(this BinaryReader binaryReader)
        {

            var length = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
            return binaryReader.ReadBytes((int)length);
        }

        public static byte[] ReadAllBytes(this BinaryReader binaryReader)
        {

            binaryReader.BaseStream.Position = 0;
            return binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
        }

        public static byte[] ReadBytes(this BinaryReader binaryReader, Range range)
        {

            var (offset, length) = range.GetOffsetAndLength((int)binaryReader.BaseStream.Length);
            binaryReader.BaseStream.Position = offset;
            return binaryReader.ReadBytes(length);
        }
    }
}
