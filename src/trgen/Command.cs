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
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Trash
{
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
            if (!config.output_directory.EndsWith('/')) config.output_directory += '/';
            else if (config.hasDesc)
                ModifyWithDesc(config);
            else
                GenerateFromGrammarFilesOnly(config);
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

                    var cwd = Environment.CurrentDirectory.Replace("\\", "/");
                    cwd = cwd.Replace("\\", "/");
                    if (!cwd.EndsWith("/")) cwd += "/";
                    var v = f.Replace(cwd, "");

                    ParsingResultSet parsing_result_set;
                    var pre = p == "" ? "" : p + "/";
                    if (test.target == "Antlr4cs" || test.target == "CSharp") pre = ""; // Erase. Packages don't need to be placed in a directory named for the package.
                    List<ParsingResultSet> pr = new List<ParsingResultSet>();
                    if (f == "st.Arithmetic.g4")
                    {
                        sgfn = f;
                        tgfn = pre + v;
                        string code = null;
                        if (config.template_sources_directory == null)
                        {
                            System.Reflection.Assembly a = this.GetType().Assembly;
                            var zip = ReadBytesResource(a, "trgen.foobar.zip");
                            MemoryStream stream = new MemoryStream(zip);
                            var regex_string = "^(?!.*(" + AllButTargetName(test.target) + "/)).*$";
                            var regex = new Regex(regex_string);
                            var za = new ZipArchive(stream);
                            code = za.Entries.Where(x => x.FullName == f).Select(x =>
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
                            code = File.ReadAllText(f);
                        }

                        parsing_result_set = DoParse(code, f);
                        pr.Add(parsing_result_set);
                    }
                    else
                    {
                        if (File.Exists(f))
                        {
                            sgfn = f;
                            //tgfn = per_grammar.package.Replace(".", "/") + (test.target == "Go" ? per_grammar.grammar_name + "/" : "") + f;
                            tgfn = pre + v;
                        }
                        else if (File.Exists(test.current_directory + f))
                        {
                            sgfn = test.current_directory + f;
                            tgfn = pre + v;
                        }
                        else if (File.Exists(test.target + "/" + f))
                        {
                            sgfn = test.target + "/" + f;
                            tgfn = pre + v;
                        }
                        else
                        {
                            System.Console.Error.WriteLine("Unknown grammar file" + f);
                            throw new Exception();
                        }

                        string code = null;
                        code = File.ReadAllText(sgfn);
                        parsing_result_set = DoParse(code, sgfn);
                        pr.Add(parsing_result_set);
                    }

                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                        new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                    List<UnvParseTreeElement> is_par = null;
                    List<UnvParseTreeElement> is_lex = null;
                    List<string> name_ = null;
                    List<string> ss = null;
                    using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                           ate.Try(pr.First().Nodes.First()))
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
                    var grammar_name = name_.First();
                    var start_symbol = ss.FirstOrDefault();

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
                        string antlr_args = "";
                        var pre1 = test.package == "" ? "" : test.package + "/";
                        var pre2 = test.package.Replace("/", ".") == "" ? "" : test.package.Replace("/", ".") + ".";
                        var g = new GrammarTuple() {
                                AntlrArgs = antlr_args,
                                GrammarFileNameTarget = tgfn,
                                GrammarFileNameSource = sgfn,
                                GrammarName = grammar_name,
                                OriginalSourceFileName = sgfn,
                                ParsingResultSet = parsing_result_set,
                                StartSymbol = start_symbol,
                                WhatType = GrammarTuple.Type.Parser,
                            };
                        if (test.target == "Go")
                        {
                            g.GrammarAutomName = pre2 + g.GrammarName;
                            g.GeneratedFileName = pre1 + g.GrammarName.ToLower().Replace("parser", "") + "_parser" + Suffix(test.target);
                            g.GeneratedIncludeFileName = "";
                            g.GrammarGoNewName = pre2 + "New" + g.GrammarName;
                        }
                        else
                        {
                            g.GrammarAutomName = pre2 + g.GrammarName;
                            g.GeneratedFileName = pre1 + g.GrammarAutomName + Suffix(test.target);
                            g.GeneratedIncludeFileName = pre1 + g.GrammarAutomName + ".h";
                            g.GrammarGoNewName = "";
                        }
                        test.tool_grammar_tuples.Add(g);
                    }
                    else if (is_lexer_grammar)
                    {
                        string antlr_args = "";
                        var g = new GrammarTuple()
                            {
                                AntlrArgs = antlr_args,
                                GrammarFileNameTarget = tgfn,
                                GrammarFileNameSource = sgfn,
                                GrammarName = grammar_name,
                                OriginalSourceFileName = sgfn,
                                ParsingResultSet = parsing_result_set,
                                StartSymbol = start_symbol,
                                WhatType = GrammarTuple.Type.Lexer,
                            };
                        var pre1 = test.package == "" ? "" : test.package + "/";
                        var pre2 = test.package.Replace("/", ".") == "" ? "" : test.package.Replace("/", ".") + ".";
                        if (test.target == "Go")
                        {
                            g.GrammarAutomName = pre2 + g.GrammarName;
                            g.GeneratedFileName = pre1 + g.GrammarName.ToLower().Replace("parser", "") + "_parser" + Suffix(test.target);
                            g.GeneratedIncludeFileName = "";
                            g.GrammarGoNewName = pre2 + "New" + g.GrammarName;
                        }
                        else
                        {
                            g.GrammarAutomName = pre2 + g.GrammarName;
                            g.GeneratedFileName = pre1 + g.GrammarAutomName + Suffix(test.target);
                            g.GeneratedIncludeFileName = pre1 + g.GrammarAutomName + ".h";
                            g.GrammarGoNewName = "";
                        }
                        test.tool_grammar_tuples.Add(g);
                    }
                    else
                    {
                        {
                            string antlr_args = ""; // Antlr tool arguments, such as -package, -o, -lib.
                            var g = new GrammarTuple() {
                                AntlrArgs = antlr_args,
                                GrammarFileNameTarget = tgfn,
                                GrammarFileNameSource = sgfn,
                                GrammarName = grammar_name + "Parser",
                                OriginalSourceFileName = sgfn,
                                ParsingResultSet = parsing_result_set,
                                StartSymbol = start_symbol,
                                WhatType = GrammarTuple.Type.Parser,
                            };
                            var pre1 = test.package == "" ? "" : test.package + "/";
                            var pre2 = test.package.Replace("/", ".") == "" ? "" : test.package.Replace("/", ".") + ".";
                            if (test.target == "Go")
                            {
                                g.GrammarAutomName = pre2 + g.GrammarName;
                                g.GeneratedFileName = pre1 + g.GrammarName.ToLower().Replace("parser", "") + "_parser" + Suffix(test.target);
                                g.GeneratedIncludeFileName = "";
                                g.GrammarGoNewName = pre2 + "New" + g.GrammarName;
                            }
                            else
                            {
                                g.GrammarAutomName = pre2 + g.GrammarName;
                                g.GeneratedFileName = pre1 + g.GrammarAutomName + Suffix(test.target);
                                g.GeneratedIncludeFileName = pre1 + g.GrammarAutomName + ".h";
                                g.GrammarGoNewName = "";
                            }
                            test.tool_grammar_tuples.Add(g);
                        }
                        {
                            string antlr_args; // Antlr tool arguments, such as -package, -o, -lib.
                            if (test.target == "Go")
                            {
                                antlr_args = "";
                            }
                            else
                            {
                                if (test.package != null && test.package != "")
                                    antlr_args = GetOSTarget() == OSPlatform.Windows
                                        ? "-o " + test.package + " -lib " + test.package +
                                          " -package " + test.package
                                        : " -package " + test.package;
                                else
                                    antlr_args = "";
                            }
                            var pre1 = test.package == "" ? "" : test.package + "/";
                            var pre2 = test.package.Replace("/", ".") == "" ? "" : test.package.Replace("/", ".") + ".";
                            var g = new GrammarTuple() {
                                AntlrArgs = antlr_args,
                                GrammarFileNameTarget = tgfn,
                                GrammarFileNameSource = sgfn,
                                GrammarName = grammar_name + "Lexer",
                                OriginalSourceFileName = sgfn,
                                ParsingResultSet = parsing_result_set,
                                StartSymbol = start_symbol,
                                WhatType = GrammarTuple.Type.Lexer,
                            };
                            if (test.target == "Go")
                            {
                                g.GrammarAutomName = pre2 + g.GrammarName;
                                g.GeneratedFileName = pre1 + g.GrammarName.ToLower().Replace("parser", "") + "_parser" + Suffix(test.target);
                                g.GeneratedIncludeFileName = "";
                                g.GrammarGoNewName = pre2 + "New" + g.GrammarName;
                            }
                            else
                            {
                                g.GrammarAutomName = pre2 + g.GrammarName;
                                g.GeneratedFileName = pre1 + g.GrammarAutomName + Suffix(test.target);
                                g.GeneratedIncludeFileName = pre1 + g.GrammarAutomName + ".h";
                                g.GrammarGoNewName = "";
                            }
                            test.tool_grammar_tuples.Add(g);
                        }
                    }
                }

                // Sort tool_grammar_tuples because there are dependencies!
                // Note, we can't do that by name because some grammars, like
                // grammars-v4/r, won't build that way.
                // Use Trash compiler to get dependencies.
                var dependency_graph = ComputeSort(test);

                System.Console.Error.WriteLine("Dependency graph of grammar:");
                System.Console.Error.WriteLine(dependency_graph.ToString());


                // Mark all grammars that have no edges "in" are top level.
                var subset = dependency_graph.Vertices.ToList();
                foreach (var n in subset)
                {
                    if (!dependency_graph.Edges.Any(e => e.To == n))
                    {
                        foreach (var z in test.tool_grammar_tuples.Where(t => t.GrammarName == n))
                        {
                            z.IsTopLevel = true;
                        }
                    }
                }

                // Mark all lexer grammars that have an edge to from a parser
                // grammar as also top-level.
                foreach (var n in subset)
                {
                    // Get a candidate top-level parser grammar.
                    var p = test.tool_grammar_tuples.Where(t => t.GrammarName == n).FirstOrDefault();
                    if (p == null) continue;
                    if (!p.IsTopLevel) continue;
                    if (p.WhatType != GrammarTuple.Type.Parser) continue;

                    // Get top-level lexer grammar for that parser grammar.
                    IEnumerable<string> ls = dependency_graph.Edges.Where(e => e.From == n).Select(e => e.To);
                    var tll = ls.Select(x =>
                    {
                        var l = test.tool_grammar_tuples.Where(t =>
                            x == t.GrammarName && t.WhatType == GrammarTuple.Type.Lexer);
                        if (l.Any()) return l.First();
                        return null;
                    }).Where(t => t != null).ToList();
                    foreach (var t in tll) t.IsTopLevel = true;
                }

                // Pick top-level parser grammar.
                GrammarTuple top_level_parser_grammar = null;
                
                if(test.grammar_name == null)
                {
                    var all = test.tool_grammar_tuples
                        .Where(t => t.WhatType == GrammarTuple.Type.Parser && t.IsTopLevel).ToList();
                    if (!all.Any())
                    {
                        throw new Exception("Can't figure out the grammar name.");
                    }
                    if (all.Count > 1)
                    {
                        throw new Exception("Can't figure out the grammar name.");
                    }
                    top_level_parser_grammar = all.First();
                    Regex r = new Regex("^(.*)Parser$");
                    var name = all.First().GrammarName;
                    if (name != null) test.grammar_name = r.Replace(name, "$1");
                }
                if (test.grammar_name == null)
                {
                    throw new Exception("Can't figure out the grammar name.");
                }

                // Find top-level parser tuple if not set yet.
                if (top_level_parser_grammar == null)
                {
                    var all = test.tool_grammar_tuples
                        .Where(t => t.WhatType == GrammarTuple.Type.Parser
                                    && t.IsTopLevel
                                    && (t.GrammarName == test.grammar_name
                                       || t.GrammarName == test.grammar_name+"Parser")).ToList();
                    if (!all.Any())
                    {
                        throw new Exception("Can't figure out the top-level parser tuple.");
                    }
                    if (all.Count > 1)
                    {
                        throw new Exception("Can't figure out the top-level parser tuple.");
                    }
                    top_level_parser_grammar = all.First();
                }

                // Pick top-level lexer grammar.
                GrammarTuple top_level_lexer_grammar = null;

                {
                    var all_lexers =
                        test.tool_grammar_tuples.Where(
                            t => t.WhatType == GrammarTuple.Type.Lexer).ToList();
                    var all = all_lexers.Where(t =>
                    {
                        if (dependency_graph.Edges.Where(
                                e => e.From == top_level_parser_grammar.GrammarName
                                     && e.To == t.GrammarName).Any())
                            return true;
                        else return false;
                    }).ToList();
                    if (!all.Any())
                    {
                        throw new Exception("Can't figure out the top-level lexer tuple.");
                    }
                    if (all.Count > 1)
                    {
                        throw new Exception("Can't figure out the top-level lexer tuple.");
                    }
                    top_level_lexer_grammar = all.First();
                    
                    // Make sure to mark top level lexer grammar as
                    // top leve.
                    all.First().IsTopLevel = true;
                }

                System.Console.Error.WriteLine("Top-level grammars "
                                               + String.Join(" ", test.tool_grammar_tuples.Where(t => t.IsTopLevel)
                                                   .Select(t => t.GrammarName)));

                // Pick from all top-level grammars,
                // the grammars that are tested. Other top level grammars
                // are still processed by the Antlr Tool but not tested
                // for parsing, e.g., grammars-v4/csharp.

                if (test.start_rule == null)
                {
                    var b = test.tool_grammar_tuples
                        .Where(t =>
                        {
                            if (!t.IsTopLevel) return false;
                            if (t.WhatType == GrammarTuple.Type.Parser)
                            {
                                if (t.GrammarName == test.grammar_name) return true;
                                if (t.GrammarName == test.grammar_name + "Parser") return true;
                                return false;
                            } else if (t.WhatType == GrammarTuple.Type.Combined)
                            {
                                if (t.GrammarName == test.grammar_name) return true;
                                return false;
                            }
                            return false;
                        }).FirstOrDefault()?.StartSymbol;
                    if (b != null) test.start_rule = b;
                    else
                    {
                        throw new Exception("Can't figure out the start rule.");
                    }
                }

                System.Console.Error.WriteLine("Start rule " + test.start_rule);


                // Update top-level automaton names in grammar tuples GrammarAutomName and GrammarGoNewName
                // We are only interested in top-level grammars as they appear in some form in the
                // build files.
                {
                    var pre1 = test.package == "" ? "" : test.package + "/";
                    var pre2 = test.package.Replace("/", ".") == "" ? "" : test.package.Replace("/", ".") + ".";
                    if (test.target == "Go")
                    {
                        top_level_parser_grammar.GrammarAutomName = pre2 + top_level_parser_grammar.GrammarName;
                        top_level_parser_grammar.GeneratedFileName = pre1 + top_level_parser_grammar.GrammarName.ToLower().Replace("parser","") + "_parser" +  Suffix(test.target);
                        top_level_parser_grammar.GeneratedIncludeFileName = "";
                        top_level_parser_grammar.GrammarGoNewName = pre2 + "New" + top_level_parser_grammar.GrammarName;
                    }
                    else
                    {
                        top_level_parser_grammar.GrammarAutomName = pre2 + top_level_parser_grammar.GrammarName;
                        top_level_parser_grammar.GeneratedFileName = pre1 + top_level_parser_grammar.GrammarAutomName + Suffix(test.target);
                        top_level_parser_grammar.GeneratedIncludeFileName = pre1 + top_level_parser_grammar.GrammarAutomName + ".h";
                        top_level_parser_grammar.GrammarGoNewName = "";
                    }
                }

                {
                    var pre1 = test.package == "" ? "" : test.package + "/";
                    var pre2 = test.package.Replace("/", ".") == "" ? "" : test.package.Replace("/", ".") + ".";
                    if (test.target == "Go")
                    {
                        top_level_lexer_grammar.GrammarAutomName = pre2 + top_level_lexer_grammar.GrammarName;
                        top_level_lexer_grammar.GeneratedFileName = pre1 + top_level_lexer_grammar.GrammarName.ToLower().Replace("lexer","") + "_lexer" + Suffix(test.target);
                        top_level_lexer_grammar.GeneratedIncludeFileName = "";
                        top_level_lexer_grammar.GrammarGoNewName = pre2 + "New" + top_level_lexer_grammar.GrammarName;
                    }
                    else
                    {
                        top_level_lexer_grammar.GrammarAutomName = pre2 + top_level_lexer_grammar.GrammarName;
                        top_level_lexer_grammar.GeneratedFileName = pre1 + top_level_lexer_grammar.GrammarAutomName + Suffix(test.target);
                        top_level_lexer_grammar.GeneratedIncludeFileName = pre1 + top_level_lexer_grammar.GrammarAutomName + ".h";
                        top_level_lexer_grammar.GrammarGoNewName = "";
                    }
                }

                // Determine how to call the parser in the source code.
                string parser_src_grammar_file_name = null;
                string lexer_src_grammar_file_name = null;

                test.fully_qualified_parser_name = top_level_parser_grammar.GrammarAutomName;
                test.fully_qualified_go_parser_name = top_level_parser_grammar.GrammarGoNewName;
                parser_src_grammar_file_name = top_level_parser_grammar.GrammarFileNameTarget;
                test.parser_grammar_file_name = parser_src_grammar_file_name;
		test.fully_qualified_lexer_name = top_level_lexer_grammar.GrammarAutomName;
		if (test.tool_grammar_files.Count == 1)
			test.fully_qualified_listener_name = test.grammar_name + "Listener";
		else
			test.fully_qualified_listener_name = test.grammar_name + "ParserListener";
                test.fully_qualified_go_lexer_name = top_level_lexer_grammar.GrammarGoNewName;
                lexer_src_grammar_file_name = top_level_lexer_grammar.GrammarFileNameTarget;
                test.lexer_grammar_file_name = lexer_src_grammar_file_name;

                // Where the parser generated code lives.
                test.tool_src_grammar_files = new HashSet<string>()
                {
                    lexer_src_grammar_file_name,
                    parser_src_grammar_file_name
                };
                test.tool_grammar_files = test.tool_grammar_tuples
                    .Where(t => t.IsTopLevel)
                    .Select(t => t.GrammarFileNameTarget).ToHashSet().ToList();
                test.parser_grammar_file_name = parser_src_grammar_file_name;
                test.lexer_grammar_file_name = lexer_src_grammar_file_name;
            }
        }

        public static string version = "0.23.32";

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
                "Rust" => ".rs",
                "Swift" => ".swift",
                "TypeScript" => ".ts",
                "Antlr4ng" => ".ts",
                _ => throw new NotImplementedException(),
            };
        }

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


        public static string GetOSName(OSPlatform platform)
        {
            if (platform == OSPlatform.Linux)
                return "Linux";
            if (platform == OSPlatform.Windows)
                return "Windows";
            if (platform == OSPlatform.OSX)
                return "OSX";
            throw new Exception("Cannot determine operating system!");
        }

        public static OSPlatform GetOSPlatformFromName(string name)
        {
            if (name == "Linux")
                return OSPlatform.Linux;
            if (name == "Windows")
                return OSPlatform.Windows;
            if (name == "OSX")
                return OSPlatform.OSX;
            throw new Exception("Cannot determine operating system!");
        }

        public static OSPlatform GetOSTarget()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
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
            return (home + "/.m2/antlr4-4.13.1-complete.jar").Replace('\\', '/');
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
                "Antlr4ng",
                "Cpp",
                "CSharp",
                "Dart",
                "Go",
                "Java",
                "JavaScript",
                "PHP",
                "Python3",
                "Rust",
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
                GenerateFromGrammarFilesOnly(config);
                return;
            }

            XmlTextReader reader = new XmlTextReader(file_name);
            reader.Namespaces = false;
            XPathDocument document = new XPathDocument(reader);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(reader.NameTable);
            int gen = 0;
            List<string> test_targets = new List<string>();
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
                if (xtargets[0] == "*")
                    test_targets = new List<string>() {
                        "Antlr4ng",
                        "Cpp",
                        "CSharp",
                        "Dart",
                        "Go",
                        "Java",
                        "JavaScript",
                        "Python3",
                        "Rust",
                        "TypeScript",
                    };
                else
                    test_targets = xtargets.First().Split(';').ToList();
                if (config.targets == null || !config.targets.Any()) config.targets = test_targets;
            }
            List<OSPlatform> test_ostargets = new List<OSPlatform>() { GetOSTarget() };
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
                    var z = xtargets.First().Split(';').ToList();
                    if (!z.Any()) test_ostargets = z.Select(t => GetOSPlatformFromName(t)).ToList();
                }
            }
            /*{
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
            }*/
            //{
            //    // Add the dirname to the current directory.
            //    var cwd = Environment.CurrentDirectory.Replace("\\", "/");
            //    if (!cwd.EndsWith("/")) cwd += "/";
            //    var list_pp = new TrashGlobbing.Glob(cwd)
            //        .RegexContents(".*g4$", false)
            //        .Where(f => f is FileInfo)
            //        .Select(f => f.FullName
            //            .Replace('\\', '/')
            //            .Replace(cwd, ""))
            //        .ToList();
            //    config.Files = list_pp;
            //}
            {
                // Add any .g4's from each of the import directories.
                var imports = navigator
                    .Select("/desc/imports", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .ToList();
                if (imports.Count > 1)
                    throw new Exception("Too many <os-targets> elements, there should be only one.");
                if (imports.Count != 0)
                {
                    var test_imports = imports.First().Split(';').ToList();
                    test_imports.Insert(0, ".");
                    config.imports = test_imports;
                }
                else config.imports = new List<string>() { "." };
                foreach (var i in config.imports)
                {
                    var cwd = Environment.CurrentDirectory.Replace("\\", "/");
                    if (!cwd.EndsWith("/")) cwd += "/";
                    cwd += i;
                    if (!cwd.EndsWith("/")) cwd += "/";
                    var list_pp = new TrashGlobbing.Glob(cwd)
                        .RegexContents(".*g4$", false)
                        .Where(f => f is FileInfo)
                        .Select(f => f.FullName
                            .Replace('\\', '/')
                            .Replace(cwd, ""))
                        .ToList();
                    var l = new List<string>();
                    l.AddRange(config.Files);
                    l.AddRange(list_pp);
                    config.Files = l;
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
            {
                var opt = navigator
                    .Select("/desc/file-encoding", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .FirstOrDefault();
                if (config.file_encoding == null && opt != null)
                {
                    config.file_encoding = opt.Trim();
                }
            }
            {
                var opt = navigator
                    .Select("/desc/binary", nsmgr)
                    .Cast<XPathNavigator>()
                    .Select(t => t.Value)
                    .FirstOrDefault();
                if (config.binary == null && opt != null)
                {
                    if (opt == "true" || opt == "")
                        config.binary = true;
                }
            }

            var xtests = navigator
                .Select("/desc/test", nsmgr)
                .Cast<XPathNavigator>()
                .ToList();
            if (!xtests.Any())
            {
                var all = config.force ? config.targets : test_targets.Intersect(config.targets);
                foreach (var os_target in config.os_targets)
                {
                    if (os_target != GetOSTarget()) continue;
                    foreach (var target in all)
                    {
                        var test = new Test(config);
                        test.os_target = GetOSName(os_target);
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

		    var test_binary = xmltest
		        .Select("binary", nsmgr)
                        .Cast<XPathNavigator>()
			.Select(t => t.Value)
			.ToList()
			.FirstOrDefault();

		    var test_file_encoding = xmltest
			.Select("file-encoding", nsmgr)
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
                            test_ostargets = xostargets.First().Split(';')
                                .Select(t=>GetOSPlatformFromName(t)).ToList();
                        }
                        else test_ostargets = new List<OSPlatform>() { GetOSTarget() };
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
                    foreach (var os_target in test_ostargets)
                    {
                        if (os_target != GetOSTarget()) continue;
                        foreach (var target in all)
                        {
                            if (!test_ostargets.Contains(os_target)) continue;
                            var test = new Test(config);
                            test.target = target;
                            test.os_target = GetOSName(os_target);
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

                            if (!(test.target == "JavaScript" || test.target == "Dart" || test.target == "TypeScript"))
                            {
                                List<string> additional = new List<string>();
                                config.antlr_tool_args = additional;
                                // On Linux, the flies are automatically place in the package,
                                // and they cannot be changed!
                                if (test.package != null && test.package != "")
                                {
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
			    test.file_encoding = (test_file_encoding != null && test_file_encoding != "") ? test_file_encoding : config.file_encoding;
			    test.binary = (test_binary != null) ? true : config.binary;
                            config.Tests.Add(test);
                        }
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

        public void GenerateFromGrammarFilesOnly(Config config)
        {
            {
                config.imports = new List<string>() { "." };
                var all = "Antlr4ng;CSharp;Cpp;Dart;Go;Java;JavaScript;PHP;Python3;Rust;TypeScript".Split(';').ToList();
                if (config.targets != null && config.targets.Count() > 0) all = config.targets.ToList();
                foreach (var target in all)
                {
                    var test = new Test(config);
                    test.os_target = GetOSName(GetOSTarget());
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
                        if (config.generateArithmeticExample)
                        {
                            config.Files = new List<string>() { "st.Arithmetic.g4" };
                        }
                        else
                        {
                            config.Files = list;
                        }
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

                test.start_rule = config.start_rule;
                test.example_files = "examples";
                test.fully_qualified_lexer_name = null;
                test.fully_qualified_parser_name = null;
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
                    if (test.test_name == "0") test.test_name = null;
                    test.output_directory =
                        (string)config.output_directory
                        + "Generated"
                        + '-'
                        + test.target
                        + (test.test_name != null ? ('-' + test.test_name) : "");
                    // Create a directory containing target build files.
                    Directory.CreateDirectory(test.output_directory);
                }
                catch (Exception)
                {
                    throw;
                }

                // Find all source files, including imported grammars.
                test.all_target_files = new List<string>();
                test.grammar_directory_source_files = new List<string>();
                foreach (var dir in config.imports)
                {
                    var cwd = Environment.CurrentDirectory.Replace("\\", "/");
                    if (!cwd.EndsWith("/")) cwd += "/";
                    cwd += dir;
                    if (!cwd.EndsWith("/")) cwd += "/";

                    // Convert to directory path.
                    var ddd = Path.GetFullPath(cwd);
                    ddd = ddd.Replace("\\", "/");
                    if (!ddd.EndsWith("/")) ddd += "/";

                    var all_source_pattern = "^(?!.*(" +
                                             (test.ignore_string != null
                                                 ? test.ignore_string + "|"
                                                 : "")
                                             + "ignore/|Generated/|Generated-[^/]*/|target/|examples/|.git/|.gitignore|"
                                             + Command.AllButTargetName(test.target)
                                             + "/)).+"
                                             + "$";
                    var l = new TrashGlobbing.Glob(cwd)
                        .RegexContents(all_source_pattern)
                        .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                        .Select(f => f.FullName.Replace('\\', '/'))
                        .ToList();
                    test.grammar_directory_source_files.AddRange(l);
                }
                GenFromTemplates(config, test);
                foreach (var dir in config.imports)
                {
                    var set = new HashSet<string>();
                    foreach (var path in test.grammar_directory_source_files)
                    {
                        var import_dir = Environment.CurrentDirectory.Replace("\\", "/");
                        if (!import_dir.EndsWith("/")) import_dir += "/";
                        import_dir += dir;
                        if (!import_dir.EndsWith("/")) import_dir += "/";

                        // Get directory of imported files.
                        import_dir = Path.GetFullPath(import_dir);
                        import_dir = import_dir.Replace("\\", "/");
                        if (!import_dir.EndsWith("/")) import_dir += "/";
                        
                        // Get base directory for file to copy.
                        var from = path;
                        var base_dir = Path.GetDirectoryName(path);
                        base_dir = base_dir.Replace("\\", "/");
                        if (!base_dir.EndsWith("/")) base_dir += "/";

                        // If base directory of file isn't the same as import dir, then skip.
                        if (!base_dir.StartsWith(import_dir))
                            continue;

                        var f = from.Substring(import_dir.Length); ;
                        string to = null;
                        if (test.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileNameTarget).Any())
                        {
                            to = test.output_directory
                                 + "/"
                                 + test.tool_grammar_tuples.Where(t => f == t.OriginalSourceFileName).Select(t => t.GrammarFileNameTarget).First();
                        }
                        else
                        {
                            // Now remove target directory.
                            if (test.target == "Go" && f.EndsWith(".go"))
                            {
                                to = test.output_directory
                                     + "/" + "parser" + f.Substring(test.target.Length);
                            }
                            else
                            {
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
                        var content = File.ReadAllText(from);
                        InstantiateTemplateFile(config, test, from, to, content);
                    }
                }
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
                    if (test.fully_qualified_parser_name != "ArithmeticParser" && fn == "st.Arithmetic.g4")
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

                InstantiateTemplateFile(config, test, from, to, content);
            }
        }

        private void InstantiateTemplateFile(Config config, Test test, string from, string to, string content)
        {
            var base_name = Basename(from);
            var dir_name = Dirname(from);

            Template t;

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
                System.Console.Error.WriteLine("Copying template file from "
                                               + from
                                               + " to "
                                               + to);
                var target_dir_name = Dirname(to);
                Directory.CreateDirectory(target_dir_name);
                File.WriteAllText(to, content);
                return;
            }

            System.Console.Error.WriteLine("Rendering template file from "
                                           + from
                                           + " to "
                                           + to);

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

            Regex re = new Regex("^st[.]");
            test.tool_grammar_files = test.tool_grammar_files.Select(s => re.Replace(s, "")).ToList();
            test.lexer_grammar_file_name = re.Replace(test.lexer_grammar_file_name, "");
            test.parser_grammar_file_name = re.Replace(test.parser_grammar_file_name, "");
            foreach (var tu in test.tool_grammar_tuples)
            {
                tu.GrammarFileNameTarget = re.Replace(tu.GrammarFileNameTarget, "");
            }

            output_dir = output_dir + "/";
            var yo1 = test.grammar_directory_source_files
                .Select(t =>
                    FixedName(t, config, test)
                        .Substring(output_dir.Length))
                .Where(t => t.Contains(Suffix(test.target)))
                .ToList();
            t.Add("official_tool", config.generator_name == "official");
            t.Add("antlrng_tool", config.generator_name == "antlr-ng");
            t.Add("target", test.target);
            t.Add("test", test);
            t.Add("additional_sources", yo1);
            t.Add("additional_targets", test.all_target_files.Where(xx =>
                {
                    var ext = Path.GetExtension(xx);
                    return Suffix(test.target).Contains(ext);
                })
                .Select(t => t.Substring(
                    (config.output_directory + "Generated").Length
                ))
                .ToList());
            t.Add("antlr_encoding", test.antlr_encoding);
            t.Add("antlr_tool_args", config.antlr_tool_args);
	    t.Add("antlr_tool_path", config.antlr_tool_path);
	    t.Add("binary", test.binary == null ? false : true);
	    t.Add("file_encoding", test.file_encoding == null ? "" : test.file_encoding);
            if (test.start_rule == null) t.Add("cap_start_symbol", Cap("no_start_rule_declared"));
            else t.Add("cap_start_symbol", Cap(test.start_rule));
            t.Add("case_insensitive_type", test.case_insensitive_type);
            t.Add("cli_bash", test.os_target != "Windows");
            t.Add("cli_cmd", GetOSTarget() == OSPlatform.Windows);
            t.Add("cmake_target", GetOSTarget() == OSPlatform.Windows
                ? "-G \"Visual Studio 17 2022\" -A x64"
                : "");
            t.Add("example_files_unix", RemoveTrailingSlash(test.example_files.Replace('\\', '/')));
            t.Add("example_files_win", RemoveTrailingSlash(test.example_files.Replace('/', '\\')));
            t.Add("example_dir_unix", RemoveTrailingSlash(GetBaseDir(test.example_files.Replace('\\', '/'))));
            t.Add("example_dir_win", RemoveTrailingSlash(GetBaseDir(test.example_files.Replace('/', '\\'))));
            t.Add("exec_name", GetOSTarget() == OSPlatform.Windows ? "Test.exe" : "Test");
            t.Add("go_lexer_name", test.fully_qualified_go_lexer_name);
            t.Add("go_parser_name", test.fully_qualified_go_parser_name);
            t.Add("grammar_file", test.tool_grammar_files.First());
            t.Add("grammar_name", test.grammar_name);
            t.Add("has_name_space", test.package != null && test.package != "");
            t.Add("is_combined_grammar", test.tool_grammar_files.Count() == 1);
            t.Add("lexer_grammar_file", re.Replace(test.lexer_grammar_file_name, ""));
            t.Add("lexer_name", test.fully_qualified_lexer_name);
            t.Add("rust_lexer_name", test.fully_qualified_lexer_name.ToLower());
            t.Add("rust_listener_name", test.fully_qualified_listener_name.ToLower());
            t.Add("rust_parser_name", test.fully_qualified_parser_name.ToLower());
            t.Add("name_space", test.package.Replace("/", "."));
            t.Add("package_name", test.package.Replace(".", "/"));
            t.Add("group_parsing", test.parsing_type == "group");
            t.Add("individual_parsing", test.parsing_type == "individual");
            t.Add("os_type", test.os_target);
            t.Add("os_win", GetOSTarget() == OSPlatform.Windows);
            t.Add("parser_name", test.fully_qualified_parser_name);
            t.Add("parser_grammar_file", re.Replace(test.parser_grammar_file_name, ""));
            t.Add("path_sep_colon", config.path_sep == PathSepType.Colon);
            t.Add("path_sep_semi", config.path_sep == PathSepType.Semi);
            t.Add("start_symbol", test.start_rule);
            t.Add("temp_dir", GetOSTarget() == OSPlatform.Windows
                ? "c:/temp"
                : "/tmp");
            t.Add("tool_grammar_files", test.tool_grammar_files.Select(s => re.Replace(s, "")));
            t.Add("tool_grammar_tuples", test.tool_grammar_tuples.Where(t => t.IsTopLevel).ToList());
            t.Add("version", Command.version);
            var o = t.Render();
            File.WriteAllText(to, o);
        }

        // As a first approximation, get the first directory from a
        // globbing pattern.
        static string GetBaseDir(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            // normalize separators to simplify processing
            var s = str.Replace('\\', '/');

            // Expect strings starting with "../" per your contract.
            if (!s.StartsWith("../"))
            {
                // Fallback: return up to first wildcard or first path separator
                int wildcardIdx = s.IndexOfAny(new char[] { '*', '?', '[' });
                int slashIdx = s.IndexOf('/');
                int end = -1;
                if (slashIdx != -1) end = slashIdx;
                if (wildcardIdx != -1 && (end == -1 || wildcardIdx < end)) end = wildcardIdx;
                if (end == -1) return s;
                return s.Substring(0, end);
            }

            // Find first path segment after the leading "../"
            int afterParent = 3; // index after "../"
            int nextSlash = s.IndexOf('/', afterParent);
            int wildcardPos = s.IndexOfAny(new char[] { '*', '?', '[' }, afterParent);

            int endIndex;
            if (nextSlash != -1 && wildcardPos != -1) endIndex = Math.Min(nextSlash, wildcardPos);
            else if (nextSlash != -1) endIndex = nextSlash;
            else if (wildcardPos != -1) endIndex = wildcardPos;
            else endIndex = s.Length;

            var segment = s.Substring(afterParent, Math.Max(0, endIndex - afterParent));

            if (string.IsNullOrEmpty(segment))
                return ".."; // nothing after "../", return parent reference

            return "../" + segment;
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

        Digraph<string> ComputeSort(Test test)
        {
            Digraph<string> graph = new Digraph<string>();
            foreach (var t in test.tool_grammar_tuples)
            {
                if (t.WhatType == GrammarTuple.Type.Combined)
                {
                    throw new Exception("Combined grammars not expected at this point. Internal error.");
                }
            }
            // Add vertices.
            foreach (var t in test.tool_grammar_tuples)
            {
                var v = t.GrammarName;
                graph.AddVertex(v);
            }
            // Add edges.
            foreach (var t in test.tool_grammar_tuples)
            {
                // Look at grammar file contents to draw dependencies out.
                var v = t.GrammarName;
                var parsing_result_set = t.ParsingResultSet;
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext =
                       ate.Try(parsing_result_set.Nodes, parsing_result_set.Parser))
                {
                    // Add an edge from the current grammar to "imported" grammar.
                    // Note, we never include parser to lexer grammar depends.
                    // That will be added--with extreme care--later on.
                    var foo = engine.parseExpression(
                            @"//delegateGrammars/delegateGrammar[not(ASSIGN)]/identifier/(RULE_REF | TOKEN_REF)/text()",
                            new StaticContextBuilder()).evaluate(dynamicContext,
                            new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as UnvParseTreeText).NodeValue as string).ToList();
                    foreach (var id in foo)
                    {
                        // Search for the grammar name in grammar tuples.
                        // Search for the grammar file name since that's what import does.
                        // Fix basic case of "import" for lexers, in https://stackoverflow.com/questions/79590155/how-to-extract-raw-contents-including-comments-within-braces-in-specific-situa
                        var files = test.tool_grammar_tuples.Where(t => t.GrammarFileNameSource.EndsWith("/" + id + ".g4")).ToList();
                        if (!files.Any()) throw new Exception("Cannot find imported file " + id + ".g4");
                        // Add an edge from the current grammar to grammars that are imported.
                        foreach (var f in files)
                        {
                            if (graph.Edges.Any(e2 => e2.From == v && e2.To == f.GrammarName)) continue;
                            if (t.WhatType == GrammarTuple.Type.Parser && f.WhatType == GrammarTuple.Type.Lexer) continue;
                            if (t.WhatType == GrammarTuple.Type.Lexer && f.WhatType == GrammarTuple.Type.Parser) continue;
                            DirectedEdge<string> e = new DirectedEdge<string>() { From = v, To = f.GrammarName };
                            graph.AddEdge(e);
                        }
                    }

                    // Add an edge from the parser to lexer grammar if explicit.
                    var bar = engine.parseExpression(
                            @"//option[identifier/RULE_REF/text() = 'tokenVocab']/optionValue/identifier/(RULE_REF | TOKEN_REF)/text()",
                            new StaticContextBuilder()).evaluate(dynamicContext,
                            new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as UnvParseTreeText).NodeValue as string).ToList();
                    foreach (var id in bar)
                    {
                        // Make sure to mark lexer grammar as "top level".
                        // "id" is a grammar file name. So, we look
                        // for grammars with that file name (append .g4), and
                        // pick off the lexer name.
                        foreach (var tup in test.tool_grammar_tuples)
                        {
                            if (tup.GrammarFileNameTarget == id + ".g4" &&
                                tup.WhatType == GrammarTuple.Type.Lexer)
                            {
                                if (graph.Edges.Any(e2 => e2.From == v && e2.To == tup.GrammarName)) continue;
                                DirectedEdge<string> e = new DirectedEdge<string>() { From = v, To = tup.GrammarName };
                                graph.AddEdge(e);
                            }
                        }
                    }

                    // If there is no explicit "tokenVocab" statement, add in edge
                    // from this *parser grammar* to *lexer grammar*.
                    if (t.WhatType == GrammarTuple.Type.Parser)
                    {
                        var find = new Regex("Parser$").Replace(t.GrammarName, "Lexer");
                        var to = test.tool_grammar_tuples.Where(l =>
                        {
                            if (l.WhatType == GrammarTuple.Type.Lexer &&
                                find == l.GrammarName) return true;
                            return false;
                        });
                        foreach (var x in to)
                        {
                            if (graph.Edges.Any(e2 => e2.From == v && e2.To == x.GrammarName)) continue;
                            DirectedEdge<string> e = new DirectedEdge<string>() { From = v, To = x.GrammarName };
                            graph.AddEdge(e);
                        }
                    }
                }
            }

            var sort = new TopologicalSort<string, DirectedEdge<string>>(graph, graph.Vertices.ToList());
            List<string> order = sort.Topological_sort();
            order.Reverse();
            test.tool_grammar_tuples.Sort(new GrammarOrderCompare(order));
            return graph;
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

            to = test.output_directory
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

            to = test.output_directory
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
                            var ix = _order.IndexOf(x.GrammarName);
                            if (ix < 0) throw new Exception();
                            var iy = _order.IndexOf(y.GrammarName);
                            if (iy < 0) throw new Exception();
                            return ix.CompareTo(iy);
                        }
                        catch(Exception)
                        { }
                        return 0;
                    }
                }
            }
        }

        AntlrJson.ParsingResultSet DoParse(string txt, string input_name)
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
            object[] parm1 = new object[] { txt, input_name, false };
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
            if ((bool)res3)
            {
                throw new Exception("Aborting. Correct syntax errors in grammar file " + input_name + " in order to generate driver.");
            }
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
            
            var converted_tree = new ConvertToDOM().BottomUpConvert(t2, null, parser, lexer, commontokstream);
            var tuple = new AntlrJson.ParsingResultSet() { FileName = "stdin", Nodes = new UnvParseTreeNode[] { converted_tree }, Parser = parser, Lexer = lexer };
            return tuple;
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
