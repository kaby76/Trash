using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Trash
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Antlr4.StringTemplate;
    using AntlrJson;
    using AntlrTreeEditing.AntlrDOM;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
            if (config.Files == null || !config.Files.Any())
            {
                var list = new List<string>();
                var tool_grammar_files_pattern = "^(?!.*(/Generated|/Generated-[^/]|/target|/examples)).+g4$";
                var list_pp = new TrashGlobbing.Glob()
                    .RegexContents(tool_grammar_files_pattern)
                    .Where(f => f is FileInfo)
                    .Select(f => f.Name.Replace('\\', '/').Replace(Environment.CurrentDirectory, ""));
                foreach (var y in list_pp)
                {
                    list.Add(y);
                }
                config.Files = list;
            }


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
                config.Files = new HashSet<string>() { "Arithmetic.g4" };
                config.start_rule = "file_";
                test.per_grammar.start_rule = config.start_rule;
            }

            // Let's first parse the input grammar files and gather information
            // about them. Note, people pump in all sorts of bullshit, so
            // be ready to handle the worse of the worse.
            foreach (var test in config.Tests)
            {
                test.per_grammar.tool_grammar_tuples = new List<GrammarTuple>();
                foreach (var f in config.Files)
                {
                    // We're going to assume that the grammars are in
                    // the current directory. That's because this is the Maven
                    // Antlr4 tool plugin so it doesn't fish around to find the grammar,
                    // except possibly in the "source directory".
                    string sgfn; // Where the grammar is.
                    string tgfn; // Where the grammar existed in the generated output parser directory.
                    var p = test.per_grammar.package.Replace(".", "/");
                    var pre = p == "" ? "" : p + "/";
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
                            //tgfn = per_grammar.package.Replace(".", "/") + (_config.target == "Go" ? per_grammar.grammar_name + "/" : "") + f;
                            tgfn = pre + f;
                        }
                        else if (File.Exists(test.per_grammar.current_directory + f))
                        {
                            sgfn = test.per_grammar.current_directory + f;
                            tgfn = pre + f;
                        }
                        else if (File.Exists(config.target + "/" + f))
                        {
                            sgfn = config.target + "/" + f;
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


                        //doc = Docs.Class1.ReadDoc(sgfn);
                        //pr = LanguageServer.ParsingResultsFactory.Create(doc);
                        //workspace = doc.Workspace;
                        //_ = new LanguageServer.Module().Compile(workspace);
                        //if (pr.Errors.Any())
                        //{
                        //    System.Console.Error.WriteLine("Your grammar "
                        //        + sgfn
                        //        + " does not compile as an Antlr4 grammar! Please check it.");
                        //    throw new Exception();
                        //}
                    }

                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                        new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                    List<AntlrElement> is_par = null;
                    List<AntlrElement> is_lex = null;
                    List<string> name_ = null;
                    List<string> ss = null;
                    using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(pr.First().Nodes.First(), pr.First().Parser))
                    {
                        is_par = engine.parseExpression(
                                @"/grammarSpec/grammarDecl/grammarType/PARSER",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement))
                            .ToList();
                        is_lex = engine.parseExpression(
                                @"/grammarSpec/grammarDecl/grammarType/LEXER",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToList();
                        name_ = engine.parseExpression(
                                @"/grammarSpec/grammarDecl/identifier/(TOKEN_REF | RULE_REF)/text()",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrText).NodeValue as string).ToList();
                        ss = engine.parseExpression(
                                @"//parserRuleSpec[ruleBlock//TOKEN_REF/text()='EOF']/RULE_REF/text()",
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrText).NodeValue as string).ToList();
                    }

                    //if (nodes == null)
                    //{
                    //    System.Console.Error.WriteLine("Your grammar "
                    //        + sgfn
                    //        + " does not compile as an Antlr4 grammar! Please check it.");
                    //    throw new Exception();
                    //}
                    //if (nodes.Count() == 0 || nodes.Count() > 1)
                    //{
                    //    System.Console.Error.WriteLine("Your grammar "
                    //        + sgfn
                    //        + " does not compile as an Antlr4 grammar! Please check it.");
                    //    throw new Exception();
                    //}
                    //var grammarDecl = nodes.First();
                    //if (nodes.Count() == 0 || nodes.Count() > 1)
                    //{
                    //    System.Console.Error.WriteLine("Your grammar "
                    //        + sgfn
                    //        + " does not compile as an Antlr4 grammar! Please check it.");
                    //    throw new Exception();
                    //}
                    var is_parser_grammar = is_par.Count() != 0;
                    var is_lexer_grammar = is_lex.Count() != 0;
                    var is_combined = !is_parser_grammar && !is_lexer_grammar;
                    var name = name_.First();
                    var start_symbol = ss.FirstOrDefault();
                    if (test.per_grammar.start_rule == null && start_symbol != null)
                    {
                        test.per_grammar.start_rule = start_symbol;
                        config.start_rule = start_symbol;
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
                        //var genfn = (_config.target == "Go" ? name.Replace("Parser", "") + "/" : "") + name + Suffix(_config);
                        var p1 = test.per_grammar.package;
                        var pre1 = p1 == "" ? "" : p1 + "/";
                        var p2 = test.per_grammar.package.Replace("/", ".");
                        var pre2 = p2 == "" ? "" : p2 + ".";
                        string genfn; // name of the generated parser/lexer file in the output directory.
                        string genincfn; // name of the include file for parser/lexer, for C++.
                        string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                        string autom_name; // The name of the parser or lexer function, fully qualified with package.
                        string goname; // The name of the parser or lexer functionj for Go.
                        if (config.target == "Go")
                        {
                            genfn = pre1 + name.Replace("Parser", "_parser").ToLower() + Suffix(test.per_grammar.target);
                            genincfn = "";
                            if (test.per_grammar.package != null && test.per_grammar.package != "")
                                antlr_args = config.env_type == OSType.Windows
                                    ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                      " -package " + test.per_grammar.package
                                    : " -package " + test.per_grammar.package;
                            else
                                antlr_args = "";
                            autom_name = pre2 + name;
                            goname = pre2 + "New" + name;
                        }
                        else
                        {
                            genfn = pre1 + name + Suffix(test.per_grammar.target);
                            genincfn = pre1 + name + ".h";
                            if (test.per_grammar.package != null && test.per_grammar.package != "")
                                antlr_args = config.env_type == OSType.Windows
                                    ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                      " -package " + test.per_grammar.package
                                    : " -package " + test.per_grammar.package;
                            else
                                antlr_args = "";
                            autom_name = pre2 + name;
                            goname = "";
                        }

                        var g = new GrammarTuple(GrammarTuple.Type.Parser, sgfn, tgfn, name, genfn, genincfn,
                            autom_name, goname, antlr_args);
                        test.per_grammar.tool_grammar_tuples.Add(g);
                    }
                    else if (is_lexer_grammar)
                    {
                        //var genfn = (_config.target == "Go" ? name.Replace("Lexer", "") + "/" : "") + name + Suffix(_config);
                        var p1 = test.per_grammar.package;
                        var pre1 = p1 == "" ? "" : p1 + "/";
                        var p2 = test.per_grammar.package.Replace("/", ".");
                        var pre2 = p2 == "" ? "" : p2 + ".";
                        string genfn; // name of the generated parser/lexer file in the output directory.
                        string genincfn; // name of the include file for parser/lexer, for C++.
                        string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                        string autom_name; // The name of the parser or lexer function, fully qualified with package.
                        string goname; // The name of the parser or lexer functionj for Go.
                        if (config.target == "Go")
                        {
                            genfn = pre1 + name.Replace("Lexer", "_lexer").ToLower() + Suffix(test.per_grammar.target);
                            genincfn = "";
                            if (test.per_grammar.package != null && test.per_grammar.package != "")
                                antlr_args = config.env_type == OSType.Windows
                                    ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                      " -package " + test.per_grammar.package
                                    : " -package " + test.per_grammar.package;
                            else
                                antlr_args = "";
                            autom_name = pre2 + name;
                            goname = pre2 + "New" + name;
                        }
                        else
                        {
                            genfn = pre1 + name + Suffix(test.per_grammar.target);
                            genincfn = pre1 + name + ".h";
                            if (test.per_grammar.package != null && test.per_grammar.package != "")
                                antlr_args = config.env_type == OSType.Windows
                                    ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                      " -package " + test.per_grammar.package
                                    : " -package " + test.per_grammar.package;
                            else
                                antlr_args = "";
                            autom_name = pre2 + name;
                            goname = "";
                        }

                        var g = new GrammarTuple(GrammarTuple.Type.Lexer, sgfn, tgfn, name, genfn, genincfn, autom_name,
                            goname, antlr_args);
                        test.per_grammar.tool_grammar_tuples.Add(g);
                    }
                    else
                    {
                        {
                            //var genfn = (_config.target == "Go" ? name + "/" : "") + name + "Parser" + Suffix(_config);
                            var p1 = test.per_grammar.package;
                            var pre1 = p1 == "" ? "" : p1 + "/";
                            var p2 = test.per_grammar.package.Replace("/", ".");
                            var pre2 = p2 == "" ? "" : p2 + ".";
                            string genfn; // name of the generated parser/lexer file in the output directory.
                            string genincfn; // name of the include file for parser/lexer, for C++.
                            string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                            string
                                autom_name; // The name of the parser or lexer function, fully qualified with package.
                            string goname; // The name of the parser or lexer functionj for Go.
                            if (config.target == "Go")
                            {
                                genfn = pre1 + name.ToLower() + "_parser" + Suffix(test.per_grammar.target);
                                genincfn = "";
                                autom_name = pre2
                                             + name
                                             + "Parser";
                                goname = pre2
                                         + "New" + name
                                         + "Parser";
                                if (test.per_grammar.package != null && test.per_grammar.package != "")
                                    antlr_args = config.env_type == OSType.Windows
                                        ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                          " -package " + test.per_grammar.package
                                        : " -package " + test.per_grammar.package;
                                else
                                    antlr_args = "";
                            }
                            else
                            {
                                genfn = pre1 + name + "Parser" + Suffix(test.per_grammar.target);
                                genincfn = pre1 + name + "Parser.h";
                                autom_name = pre2
                                             + name
                                             + "Parser";
                                goname = "";
                                if (test.per_grammar.package != null && test.per_grammar.package != "")
                                    antlr_args = config.env_type == OSType.Windows
                                        ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                          " -package " + test.per_grammar.package
                                        : " -package " + test.per_grammar.package;
                                else
                                    antlr_args = "";
                            }

                            var g = new GrammarTuple(GrammarTuple.Type.Parser, sgfn, tgfn, name, genfn, genincfn,
                                autom_name, goname, antlr_args);
                            test.per_grammar.tool_grammar_tuples.Add(g);
                        }
                        {
                            //var genfn = (_config.target == "Go" ? name + "/" : "") + name + "Lexer" + Suffix(_config);
                            var p1 = test.per_grammar.package;
                            var pre1 = p1 == "" ? "" : p1 + "/";
                            var p2 = test.per_grammar.package.Replace("/", ".");
                            var pre2 = p2 == "" ? "" : p2 + ".";
                            string genfn; // name of the generated parser/lexer file in the output directory.
                            string genincfn; // name of the include file for parser/lexer, for C++.
                            string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                            string
                                autom_name; // The name of the parser or lexer function, fully qualified with package.
                            string goname; // The name of the parser or lexer functionj for Go.
                            if (config.target == "Go")
                            {
                                genfn = pre1 + name.ToLower() + "_lexer" + Suffix(test.per_grammar.target);
                                genincfn = "";
                                autom_name = pre2
                                             + name
                                             + "Lexer";
                                goname = pre2
                                         + "New" + name
                                         + "Lexer";
                                if (test.per_grammar.package != null && test.per_grammar.package != "")
                                    antlr_args = config.env_type == OSType.Windows
                                        ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                          " -package " + test.per_grammar.package
                                        : " -package " + test.per_grammar.package;
                                else
                                    antlr_args = "";
                            }
                            else
                            {
                                genfn = pre1 + name + "Lexer" + Suffix(test.per_grammar.target);
                                genincfn = pre1 + name + "Lexer.h";
                                autom_name = pre2
                                             + name
                                             + "Lexer";
                                goname = "";
                                if (test.per_grammar.package != null && test.per_grammar.package != "")
                                    antlr_args = config.env_type == OSType.Windows
                                        ? "-o " + test.per_grammar.package + " -lib " + test.per_grammar.package +
                                          " -package " + test.per_grammar.package
                                        : " -package " + test.per_grammar.package;
                                else
                                    antlr_args = "";
                            }

                            var g = new GrammarTuple(GrammarTuple.Type.Lexer, sgfn, tgfn, name, genfn, genincfn,
                                autom_name, goname, antlr_args);
                            test.per_grammar.tool_grammar_tuples.Add(g);
                        }
                    }
                }

                // Pick a damn grammar if none specified. If more than one fuck it.
                if (test.per_grammar.grammar_name == null)
                {
                    var a = test.per_grammar.tool_grammar_tuples.Where(t => t.WhatType == GrammarTuple.Type.Parser)
                        .FirstOrDefault()?.GrammarName;
                    if (a != null) test.per_grammar.grammar_name = a;
                    if (test.per_grammar.grammar_name == null)
                    {
                        var b = test.per_grammar.tool_grammar_tuples
                            .Where(t => t.WhatType == GrammarTuple.Type.Combined).FirstOrDefault()?.GrammarName;
                        if (b != null) test.per_grammar.grammar_name = b;
                    }
                }

                if (test.per_grammar.grammar_name == null)
                {
                    throw new Exception("Can't figure out the grammar name.");
                }

                // Sort tool_grammar_tuples because there are dependencies!
                // Note, we can't do that by name because some grammars, like
                // grammars-v4/r, won't build that way.
                // Use Trash compiler to get dependencies.
                ComputeSort(test.per_grammar);

                // How to call the parser in the source code. Remember, there are
                // actually up to two tests in the pom file, one for running the
                // Antlr tool, and the other to test the generated parser.
                test.per_grammar.fully_qualified_parser_name =
                    test.per_grammar.tool_grammar_tuples
                        .Where(t => t.WhatType == GrammarTuple.Type.Parser &&
                                    (t.GrammarName == test.per_grammar.grammar_name + "Parser"
                                     || t.GrammarName == test.per_grammar.grammar_name))
                        .Select(t => t.GrammarAutomName).First();
                test.per_grammar.fully_qualified_go_parser_name =
                    test.per_grammar.tool_grammar_tuples
                        .Where(t => t.WhatType == GrammarTuple.Type.Parser &&
                                    (t.GrammarName == test.per_grammar.grammar_name + "Parser"
                                     || t.GrammarName == test.per_grammar.grammar_name))
                        .Select(t => t.GrammarGoNewName).First();

                // Where the parser generated code lives.
                var parser_generated_file_name =
                    (string)test.per_grammar.fully_qualified_parser_name.Replace('.', '/')
                    + Suffix(test.per_grammar.target);
                var parser_generated_include_file_name =
                    (string)test.per_grammar.fully_qualified_parser_name.Replace('.', '/') + ".h";
                var parser_src_grammar_file_name =
                    test.per_grammar.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Parser")).Select(t
                            => t.GrammarFileName).First();
                test.per_grammar.fully_qualified_lexer_name =
                    test.per_grammar.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Lexer"))
                        .Select(t => t.GrammarAutomName).First();
                test.per_grammar.fully_qualified_go_lexer_name =
                    test.per_grammar.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Lexer"))
                        .Select(t => t.GrammarGoNewName).First();
                var lexer_generated_file_name =
                    test.per_grammar.fully_qualified_lexer_name.Replace('.', '/') + Suffix(test.per_grammar.target);
                var lexer_generated_include_file_name =
                    test.per_grammar.fully_qualified_lexer_name.Replace('.', '/') + ".h";
                var lexer_src_grammar_file_name =
                    test.per_grammar.tool_grammar_tuples
                        .Where(t => t.GrammarAutomName.EndsWith("Lexer")).Select(t
                            => t.GrammarFileName).First();
                test.per_grammar.tool_src_grammar_files = new HashSet<string>()
                {
                    lexer_src_grammar_file_name,
                    parser_src_grammar_file_name
                };
                //per_grammar.tool_grammar_tuples = new List<GrammarTuple>()
                //    {
                //        new GrammarTuple(lexer_grammar_file_name, lexer_generated_file_name, lexer_generated_include_file_name, per_grammar.fully_qualified_lexer_name),
                //        new GrammarTuple(parser_grammar_file_name, parser_generated_file_name, parser_generated_include_file_name, per_grammar.fully_qualified_parser_name),
                //    };
                test.per_grammar.tool_grammar_files = test.per_grammar.tool_grammar_tuples
                    .Select(t => t.GrammarFileName).ToHashSet().ToList();
                test.per_grammar.parser_grammar_file_name = parser_src_grammar_file_name;
                test.per_grammar.generated_files = test.per_grammar.tool_grammar_tuples.Select(t => t.GeneratedFileName)
                    .ToHashSet().ToList();
                test.per_grammar.lexer_grammar_file_name = lexer_src_grammar_file_name;
            }
        }

        public static string version = "0.20.1";

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

        public static OSType GetEnvType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSType.Unix;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSType.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSType.Mac;
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
            return target switch
            {
                "Antlr4cs" => "Antlr4cs",
                "Cpp" => "Cpp",
                "CSharp" => "CSharp",
                "Dart" => "Dart",
                "Go" => "Go",
                "Java" => "Java",
                "JavaScript" => "JavaScript",
                "PHP" => "PHP",
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
                "PHP",
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

        private void ModifyWithDesc(Config config)
        {
            //
            // Process desc.xml.
            //
            System.Console.Error.WriteLine(Environment.CurrentDirectory);
            XmlTextReader reader = new XmlTextReader(Environment.CurrentDirectory + Path.DirectorySeparatorChar + @"desc.xml");
            reader.Namespaces = false;
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(reader.NameTable);
            int gen = 0;
            var xtests = navigator
                .Select("//desc/test", nsmgr)
                .Cast<XPathNavigator>()
                .ToList();
            if (!xtests.Any())
            {
                // Get all targets, create one test per target.
                var xtargets = navigator
                    .Select("//desc/targets", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                if (xtargets.Count > 1)
                    throw new Exception("Too many <targets> elements, there should be only one.");
                if (xtargets.Count == 0)
                    throw new Exception("No <targets> elements specified, there must be one.");
                var targets = xtargets.First().Split(';');
                foreach (var target in targets)
                {
                    var test = new Test();
                    test.per_grammar.target = target;
                    config.Tests.Add(test);
                }
            }
            else
            {
                foreach (var xmltest in xtests)
                {
                    List<string> spec_antlr_tool_args = new List<string>();
                    var test = new Test();
                    config.Tests.Add(test);
                    var test_name = xmltest
                        .Select("name", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .ToList()
                        .FirstOrDefault();
                    var spec_grammar_file_names = xmltest
                        .Select("grammars", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .ToList();
                    if (!spec_grammar_file_names.Any()) spec_grammar_file_names.Add("*.g4");
                    var spec_source_directory = navigator
                        .Select("sourceDirectory", nsmgr)
                        .Cast<XPathNavigator>()
                        .Where(t => t.Value != "")
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_grammar_name = navigator
                        .Select("grammarName", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    if (spec_grammar_name != null)
                    {
                        test.per_grammar.grammar_name = spec_grammar_name.Trim();
                    }
                    var spec_lexer_name = navigator
                        .Select("lexerName", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_entry_point = navigator
                        .Select("entryPoint", nsmgr)
                        .Cast<XPathNavigator>()
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_package_name = navigator
                        .Select("packageName", nsmgr)
                        .Cast<XPathNavigator>()
                        .Where(t => t.Value != "")
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    var spec_example_directory = navigator
                        .Select("inputs", nsmgr)
                        .Cast<XPathNavigator>()
                        .Where(t => t.Value != "")
                        .Select(t => t.Value)
                        .FirstOrDefault();
                    if (spec_example_directory != null)
                    {
                        test.per_grammar.example_files = spec_example_directory;
                        if (!Directory.Exists(spec_example_directory))
                        {
                            System.Console.Error.WriteLine("Examples directory doesn't exist " +
                                                           spec_example_directory.First());
                        }
                    }
                    else
                    {
                        test.per_grammar.example_files = "examples";
                    }
                    if (spec_antlr_tool_args.Contains("-package"))
                    {
                        var ns = spec_antlr_tool_args[spec_antlr_tool_args.IndexOf("-package") + 1];
                        config.name_space = ns;
                    }
                    else if (spec_package_name != null)
                    {
                        config.name_space = spec_package_name;
                    }
                    var merged_list = new HashSet<string>();
                    foreach (var x in spec_grammar_file_names)
                    {
                        var pp = TrashGlobbing.Glob.GlobToRegex(x);
                        var list_pp = new TrashGlobbing.Glob()
                            .RegexContents(pp)
                            .Where(f => f is FileInfo)
                            .Select(f => f.FullName.Replace('\\', '/').Replace(Environment.CurrentDirectory, ""));
                        foreach (var y in list_pp)
                        {
                            merged_list.Add(y);
                        }
                    }

                    if (spec_source_directory != null)
                    {
                        test.per_grammar.current_directory = spec_source_directory
                            .Replace("${basedir}", "")
                            .Trim();
                        if (test.per_grammar.current_directory.StartsWith('/'))
                            test.per_grammar.current_directory = test.per_grammar.current_directory.Substring(1);
                        if (test.per_grammar.current_directory != "" && !test.per_grammar.current_directory.EndsWith("/"))
                        {
                            test.per_grammar.current_directory = test.per_grammar.current_directory + "/";
                        }
                    }
                    else
                    {
                        test.per_grammar.current_directory = "";
                    }

                    // Check for existence of .trgen-ignore file.
                    // If there is one, read and create pattern of what to ignore.
                    if (File.Exists(ignore_list_of_files))
                    {
                        var ignore = new StringBuilder();
                        var lines = File.ReadAllLines(ignore_list_of_files);
                        var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                        test.per_grammar.ignore_string = string.Join("|", ignore_lines);
                    }
                    else test.per_grammar.ignore_string = null;

                    if (!(config.target == "JavaScript" || config.target == "Dart" || config.target == "TypeScript"))
                    {
                        List<string> additional = new List<string>();
                        config.antlr_tool_args = additional;
                        // On Linux, the flies are automatically place in the package,
                        // and they cannot be changed!
                        if (config.name_space != null && config.name_space != "")
                        {
                            if (config.env_type == OSType.Windows)
                            {
                                additional.Add("-o");
                                additional.Add(config.name_space.Replace('.', '/'));
                            }

                            additional.Add("-lib");
                            additional.Add(config.name_space.Replace('.', '/'));
                        }
                    }

                    test.per_grammar.package = (spec_package_name != null && spec_package_name.Any()
                        ? spec_package_name.First() + "/"
                        : "");
                    test.per_grammar.package = config.target == "Go" ? "parser" : test.per_grammar.package;
                    test.per_grammar.start_rule = config.start_rule != null && config.start_rule != ""
                        ? config.start_rule
                        : spec_entry_point;
                    test.per_grammar.test_name = test_name ?? (gen++).ToString();
                }
            }
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
            test.per_grammar.grammar_name = pom_grammar_name.First();
            // Pom is a mess. There are many cases here in computing the namespace/package.
            // People even add bullshit @headers in the grammar; minds of a planaria.
            // -package arg specified; source top level
            //   => keep .g4 at top level, generate to directory
            //      corresponding to arg.
            if (pom_antlr_tool_args.Contains("-package"))
            {
                var ns = pom_antlr_tool_args[pom_antlr_tool_args.IndexOf("-package") + 1];
                config.name_space = ns;
            }
            else if (pom_package_name.Any())
            {
                config.name_space = pom_package_name.First();
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
                test.per_grammar.example_files = pom_example_files.First();
                if (!Directory.Exists(pom_example_files.First()))
                {
                    System.Console.Error.WriteLine("Examples directory doesn't exist " + pom_example_files.First());
                }
            }
            else
            {
                test.per_grammar.example_files = "examples";
            }
            if (pom_source_directory.Any())
            {
                test.per_grammar.current_directory = pom_source_directory
                    .First()
                    .Replace("${basedir}", "")
                    .Trim();
                if (test.per_grammar.current_directory.StartsWith('/')) test.per_grammar.current_directory = test.per_grammar.current_directory.Substring(1);
                if (test.per_grammar.current_directory != "" && !test.per_grammar.current_directory.EndsWith("/"))
                {
                    test.per_grammar.current_directory = test.per_grammar.current_directory + "/";
                }
            }
            else
            {
                test.per_grammar.current_directory = "";
            }
            test.per_grammar.case_insensitive_type = null;
            if (pom_case_insensitive_type.Any())
            {
                if (pom_case_insensitive_type.First().ToUpper() == "UPPER")
                    test.per_grammar.case_insensitive_type = CaseInsensitiveType.Upper;
                else if (pom_case_insensitive_type.First().ToUpper() == "LOWER")
                    test.per_grammar.case_insensitive_type = CaseInsensitiveType.Lower;
                else
                {
                    System.Console.Error.WriteLine("Case fold has invalid value: '"
                    + pom_case_insensitive_type.First() + "'.");
                }
                //else throw new Exception("Case fold has invalid value: '"
                //    + pom_case_insensitive_type.First() + "'.");
            }
            else test.per_grammar.case_insensitive_type = null;
            // Check for existence of .trgen-ignore file.
            // If there is one, read and create pattern of what to ignore.
            if (File.Exists(ignore_list_of_files))
            {
                var ignore = new StringBuilder();
                var lines = File.ReadAllLines(ignore_list_of_files);
                var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                test.per_grammar.ignore_string = string.Join("|", ignore_lines);
            }
            else test.per_grammar.ignore_string = null;
            if (!(config.target == "JavaScript" || config.target == "Dart" || config.target == "TypeScript"))
            {
                List<string> additional = new List<string>();
                config.antlr_tool_args = additional;
                // On Linux, the flies are automatically place in the package,
                // and they cannot be changed!
                if (config.name_space != null && config.name_space != "")
                {
                    if (config.env_type == OSType.Windows)
                    {
                        additional.Add("-o");
                        additional.Add(config.name_space.Replace('.', '/'));
                    }
                    additional.Add("-lib");
                    additional.Add(config.name_space.Replace('.', '/'));
                }
            }
            test.per_grammar.package = (pom_package_name != null && pom_package_name.Any() ? pom_package_name.First() + "/" : "");
            test.per_grammar.package = config.target == "Go" ? "parser" : test.per_grammar.package;
            test.per_grammar.start_rule = config.start_rule != null && config.start_rule != "" ? config.start_rule : pom_entry_point.First();
        }

        public void DoNonPomDirectedGenerate(Config config)
        {
            foreach (var test in config.Tests)
            {
                test.per_grammar.current_directory = config.root_directory;
                if (test.per_grammar.current_directory != "" && !test.per_grammar.current_directory.EndsWith("/"))
                    test.per_grammar.current_directory = test.per_grammar.current_directory + "/";
                // Check for existence of .trgen-ignore file.
                // If there is one, read and create pattern of what to ignore.
                if (File.Exists(ignore_list_of_files))
                {
                    var ignore = new StringBuilder();
                    var lines = File.ReadAllLines(ignore_list_of_files);
                    var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                    test.per_grammar.ignore_string = string.Join("|", ignore_lines);
                }
                else test.per_grammar.ignore_string = null;

                test.per_grammar.start_rule = config.start_rule;
                test.per_grammar.example_files = "examples";
                test.per_grammar.fully_qualified_lexer_name = "";
                test.per_grammar.fully_qualified_parser_name = "";
                test.per_grammar.package = config.target == "Go" ? "parser" : "";
                var all_grammars_pattern = "^(?!.*(" +
                                           (test.per_grammar.ignore_string != null
                                               ? test.per_grammar.ignore_string + "|"
                                               : "")
                                           + "/ignore/|/Generated/|/Generated-[^/]*/|/target/|/examples/|/.git/|/.gitignore/|/.ignore/|"
                                           + Command.AllButTargetName(config.target)
                                           + "/)).+[.]g4"
                                           + "$";
                var grammar_list = new TrashGlobbing.Glob(test.per_grammar.current_directory)
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
                        if (test.per_grammar.fully_qualified_parser_name != "ArithmeticParser" &&
                            f == "./Arithmetic.g4") return false;
                        if (f == "./files") return false;
                        return true;
                    }).Select(f => f.Replace(test.per_grammar.current_directory, "")).ToHashSet();
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
                        + test.per_grammar.target
                        + (test.per_grammar.test_name != null ? ('-' + test.per_grammar.test_name) : ""));
                }
                catch (Exception)
                {
                    throw;
                }

                // Find all source files.
                test.per_grammar.all_target_files = new List<string>();
                var all_source_pattern = "^(?!.*(" +
                                         (test.per_grammar.ignore_string != null
                                             ? test.per_grammar.ignore_string + "|"
                                             : "")
                                         + "ignore/|Generated/|Generated-[^/]*/|target/|examples/|.git/|.gitignore|"
                                         + Command.AllButTargetName(test.per_grammar.target)
                                         + "/)).+"
                                         + "$";
                test.per_grammar.all_source_files = new TrashGlobbing.Glob()
                    .RegexContents(all_source_pattern)
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
                GenFromTemplates(this, config, test);
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

        public void AddSource(Config config, Test test)
        {
            var cd = Environment.CurrentDirectory + "/";
            cd = cd.Replace('\\', '/');
            var set = new HashSet<string>();
            foreach (var path in test.per_grammar.all_source_files)
            {
                // Construct proper starting directory based on namespace.
                var from = path;
                var f = from.Substring(cd.Length);
                string to = null;
                if (test.per_grammar.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileName).Any())
                {
                    to = config.output_directory
                         + "-"
                         + test.per_grammar.target
                         + (test.per_grammar.test_name != null ? ("-" + test.per_grammar.test_name) : "")
                         + "/"
                         + test.per_grammar.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileName).First();
                }
                else
                {
                    // Now remove target directory.
                    if (config.target == "Go" && f.EndsWith(".go"))
                    {
                        to = config.output_directory
                             + "-"
                             + test.per_grammar.target
                             + (test.per_grammar.test_name != null ? ("-" + test.per_grammar.test_name) : "")
                             + "/" + "parser" + f.Substring(config.target.Length);
                    }
                    else {
                        f = (
                            f.StartsWith(
                                Command.TargetName(test.per_grammar.target) + '/')
                            ? f.Substring((Command.TargetName(test.per_grammar.target) + '/').Length)
                            : f
                            );
                    }
                    // Remove "src/main/java", a royal hangover from the Maven plugin.
                    f = (
                            f.StartsWith("src/main/java/")
                            ? f.Substring("src/main/java".Length)
                            : f
                            );
                    if (config.name_space != null
                        && !(test.per_grammar.target == "Antlr4cs" || test.per_grammar.target == "CSharp"))
                    {
                        to = config.output_directory
                             + "-"
                             + test.per_grammar.target
                             + (test.per_grammar.test_name != null ? ("-" + test.per_grammar.test_name) : "")
                             + '/'
                             + config.name_space.Replace('.', '/') + '/'
                             + f;
                    }
                    if (to == null)
                    {
                        to = config.output_directory
                             + "-"
                             + test.per_grammar.target
                             + (test.per_grammar.test_name != null ? ("-" + test.per_grammar.test_name) : "")
                             + '/'
                             + f;
                    }
                }
                if (from.Contains("pom.xml") && config.target != "Java") continue;
                System.Console.Error.WriteLine("Copying source file from "
                  + from
                  + " to "
                  + to);
                test.per_grammar.all_target_files.Add(to);
                this.CopyFile(from, to);
            }
        }

        private void GenFromTemplates(Command p, Config config, Test test)
        {
            var append_namespace = (!(config.target == "CSharp" || config.target == "Antlr4cs"));
            if (config.template_sources_directory == null)
            {
                System.Reflection.Assembly a = this.GetType().Assembly;
                // Load resource file that contains the names of all files in templates/ directory,
                // which were obtained by doing "cd templates/; find . -type f > files" at a Bash
                // shell.
                var orig_file_names = ReadAllResourceLines(a, "trgen.templates.files");
                var prefix = "trgen.templates.";
                var regex_string = "^(?!.*(" + AllButTargetName(test.per_grammar.target) + "/)).*$";
                var regex = new Regex(regex_string);
                var files_to_copy = orig_file_names.Where(f =>
                {
                    if (test.per_grammar.fully_qualified_parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
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
                        && test.per_grammar.grammar_name != "Arithmetic"
                        && test.per_grammar.tool_src_grammar_files.Any())
                    {
                        continue;
                    }
                    var to = FixedName(from, config, test);
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = ReadAllResource(a, prefix + from.Replace('/','.'));
                    System.Console.Error.WriteLine("Rendering template file from "
                        + from
                        + " to "
                        + to);
                    Template t = new Template(content);
                    var yo1 = test.per_grammar.all_source_files
                        .Select(t =>
                            FixedName(t, config, test)
                            .Substring(config.output_directory.Length))
                        .Where(t => t.Contains(Suffix(test.per_grammar.target)))
                        .ToList();
                    t.Add("additional_sources", yo1);
			        t.Add("additional_targets", test.per_grammar.all_target_files.Where(xx =>
			        {
				        var ext = Path.GetExtension(xx);
				        return Suffix(test.per_grammar.target).Contains(ext);
			        })
			              .Select(t => t.Substring(config.output_directory.Length))
					        .ToList());
                    t.Add("antlr_encoding", test.per_grammar.antlr_encoding);
                    t.Add("antlr_tool_args", config.antlr_tool_args);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
                    t.Add("cap_start_symbol", Cap(test.per_grammar.start_rule));
                    t.Add("case_insensitive_type", test.per_grammar.case_insensitive_type);
                    t.Add("cli_bash", (OSType)config.env_type == OSType.Unix);
                    t.Add("cli_cmd", (OSType)config.env_type == OSType.Windows);
                    t.Add("cmake_target", config.env_type == OSType.Windows
                        ? "-G \"Visual Studio 17 2022\" -A x64" : "");
                    t.Add("example_files_unix", RemoveTrailingSlash(test.per_grammar.example_files.Replace('\\', '/')));
                    t.Add("example_files_win", RemoveTrailingSlash(test.per_grammar.example_files.Replace('/', '\\')));
                    t.Add("exec_name", config.env_type == OSType.Windows ?
                        "Test.exe" : "Test");
                    t.Add("go_lexer_name", test.per_grammar.fully_qualified_go_lexer_name);
                    t.Add("go_parser_name", test.per_grammar.fully_qualified_go_parser_name);
                    t.Add("grammar_file", test.per_grammar.tool_grammar_files.First());
                    t.Add("grammar_name", test.per_grammar.grammar_name);
                    t.Add("has_name_space", test.per_grammar.package != null && test.per_grammar.package != "");
                    t.Add("is_combined_grammar", test.per_grammar.tool_grammar_files.Count() == 1);
                    t.Add("lexer_grammar_file", test.per_grammar.lexer_grammar_file_name);
                    t.Add("lexer_name", test.per_grammar.fully_qualified_lexer_name);
                    t.Add("name_space", test.per_grammar.package.Replace("/", "."));
                    t.Add("package_name", test.per_grammar.package.Replace(".", "/"));
                    t.Add("os_type", ((OSType)config.env_type).ToString());
                    t.Add("os_win", (OSType)config.env_type == OSType.Windows);
                    t.Add("parser_name", test.per_grammar.fully_qualified_parser_name);
                    t.Add("parser_grammar_file", test.per_grammar.parser_grammar_file_name);
                    t.Add("path_sep_colon", config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", test.per_grammar.start_rule);
                    t.Add("temp_dir", config.env_type == OSType.Windows
                        ? "c:/temp" : "/tmp");
                    t.Add("tool_grammar_files", test.per_grammar.tool_grammar_files);
                    t.Add("tool_grammar_tuples", test.per_grammar.tool_grammar_tuples);
                    t.Add("version", Command.version);
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
            }
            else
            {
                var regex_string = "^(?!.*(files|" + AllButTargetName(config.target) + "/)).*$";
                var files_to_copy = new TrashGlobbing.Glob(config.template_sources_directory)
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
                        if (test.per_grammar.fully_qualified_parser_name != "ArithmeticParser" && f == "./Arithmetic.g4") return false;
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
                        && test.per_grammar.grammar_name != "Arithmetic"
                        && test.per_grammar.tool_src_grammar_files.Any())
                    {
                        continue;
                    }
                    var from = file;
                    var e = file.Substring(prefix_to_remove.Length);
                    var to = e.StartsWith(TargetName(config.target))
                         ? e.Substring((TargetName(config.target)).Length + 1)
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
		            var yo1 = test.per_grammar.all_source_files
			              .Select(t => FixedName(t, config, test)
					         .Substring(config.output_directory.Length))
			              .Where(t => t.Contains(Suffix(test.per_grammar.target)))
			              .ToList();
                    t.Add("additional_sources", yo1);
                    t.Add("antlr_encoding", test.per_grammar.antlr_encoding);
                    t.Add("antlr_tool_args", config.antlr_tool_args);
                    t.Add("antlr_tool_path", config.antlr_tool_path);
                    t.Add("cap_start_symbol", Cap(test.per_grammar.start_rule));
                    t.Add("case_insensitive_type", test.per_grammar.case_insensitive_type);
                    t.Add("cli_bash", (OSType)config.env_type == OSType.Unix);
                    t.Add("cli_cmd", (OSType)config.env_type == OSType.Windows);
                    t.Add("cmake_target", config.env_type == OSType.Windows
                        ? "-G \"Visual Studio 17 2022\" -A x64" : "");
                    t.Add("example_files_unix", RemoveTrailingSlash(test.per_grammar.example_files.Replace('\\', '/')));
                    t.Add("example_files_win", RemoveTrailingSlash(test.per_grammar.example_files.Replace('/', '\\')));
                    t.Add("exec_name", config.env_type == OSType.Windows ?
                      "Test.exe" : "Test");
                    t.Add("go_lexer_name", test.per_grammar.fully_qualified_go_lexer_name);
                    t.Add("go_parser_name", test.per_grammar.fully_qualified_go_parser_name);
                    t.Add("grammar_file", test.per_grammar.tool_grammar_files.First());
                    t.Add("grammar_name", test.per_grammar.grammar_name);
                    t.Add("has_name_space", test.per_grammar.package != null && test.per_grammar.package != "");
                    t.Add("is_combined_grammar", test.per_grammar.tool_grammar_files.Count() == 1);
                    t.Add("lexer_grammar_file", test.per_grammar.lexer_grammar_file_name);
		            t.Add("lexer_name", test.per_grammar.fully_qualified_lexer_name);
                    t.Add("name_space", test.per_grammar.package.Replace("/", "."));
                    t.Add("package_name", test.per_grammar.package.Replace(".", "/"));
                    t.Add("os_type", ((OSType)config.env_type).ToString());
                    t.Add("os_win", (OSType)config.env_type == OSType.Windows);
                    t.Add("parser_name", test.per_grammar.fully_qualified_parser_name);
                    t.Add("parser_grammar_file", test.per_grammar.parser_grammar_file_name);
                    t.Add("path_sep_colon", config.path_sep == PathSepType.Colon);
                    t.Add("path_sep_semi", config.path_sep == PathSepType.Semi);
                    t.Add("start_symbol", test.per_grammar.start_rule);
                    t.Add("temp_dir", config.env_type == OSType.Windows
                        ? "c:/temp" : "/tmp");
                    t.Add("tool_grammar_files", test.per_grammar.tool_grammar_files);
                    t.Add("tool_grammar_tuples", test.per_grammar.tool_grammar_tuples);
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
            foreach (var t in per_grammar.tool_grammar_tuples)
            {
                var f = t.OriginalSourceFileName;
                // First approximation. If a parser, make dependent on lexer.
                if (t.WhatType == GrammarTuple.Type.Parser)
                {
                    foreach (var u in per_grammar.tool_grammar_tuples)
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
            per_grammar.tool_grammar_tuples.Sort(new GrammarOrderCompare(order));
        }

        string FixedName(string from, Config config, Test test)
        {
            string to = null;
            if (from.StartsWith(test.per_grammar.target)) to = from.Substring(test.per_grammar.target.Length + 1);
            else to = from;
            if (test.per_grammar.tool_grammar_tuples.Where(t => from.Substring(test.per_grammar.target.Length) == t.OriginalSourceFileName).Select(t => t.GrammarFileName).Any())
            {
                to = config.output_directory
                     + "-"
                     + test.per_grammar.target
                     + (test.per_grammar.test_name != null ? ("-" + test.per_grammar.test_name) : "")
                     + '/'
                     + test.per_grammar.tool_grammar_tuples.Where(t => to == t.OriginalSourceFileName).Select(t => t.GrammarFileName).First();
            }
            else
            {
                to = config.output_directory
                     + "-"
                     + test.per_grammar.target
                     + (test.per_grammar.test_name != null ? ("-" + test.per_grammar.test_name) : "")
                     + '/'
                     + to;

            }
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
            
            var converted_tree = ConvertToDOM.BottomUpConvert(t2, null, parser, lexer, commontokstream, charstream);
            var tuple = new AntlrJson.ParsingResultSet() { Text = (r5 as string), FileName = "stdin", Nodes = new AntlrNode[] { converted_tree }, Parser = parser, Lexer = lexer };
            data.Add(tuple);
            return (bool)res3 ? 1 : 0;
        }

    }
}
