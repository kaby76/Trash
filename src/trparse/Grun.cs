using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Trash;

public class Grun
{
    Config config;

    // Accumulated performance stats across all DoParse calls in this run
    private double _totalParseSeconds;
    private long _totalTokens;
    private long _firstFileTokens;
    private double _firstFileParseSeconds;
    private int _fileCount;

    public Grun(Config co)
    {
        config = co;
    }

    private static string JoinArguments(IEnumerable<string> arguments)
    {
        if (arguments == null)
            throw new ArgumentNullException("arguments");

        StringBuilder builder = new StringBuilder();
        foreach (string argument in arguments)
        {
            if (builder.Length > 0)
                builder.Append(' ');

            if (argument.IndexOfAny(new[] { '"', ' ' }) < 0)
            {
                builder.Append(argument);
                continue;
            }

            // escape a backslash appearing before a quote
            string arg = argument.Replace("\\\"", "\\\\\"");
            // escape double quotes
            arg = arg.Replace("\"", "\\\"");

            // wrap the argument in outer quotes
            builder.Append('"').Append(arg).Append('"');
        }

        return builder.ToString();
    }

    private void HandleOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        System.Console.WriteLine(e.Data);
    }

    public int Run(string parser_type)
    {
        int result = 0;
        DateTime overallBefore = DateTime.Now;
        try
        {
            var data = new List<AntlrJson.ParsingResultSet>();
            string txt = config.Input;
            if (config.ReadFileNameStdin)
            {
                List<string> inputs = new List<string>();
                for (;;)
                {
                    var line = System.Console.In.ReadLine();
                    line = line?.Trim();
                    if (line == null || line == "")
                    {
                        break;
                    }

                    inputs.Add(line);
                }

                for (int f = 0; f < inputs.Count(); ++f)
                {
                    try
                    {
                        txt = File.ReadAllText(inputs[f]);
                    }
                    catch
                    {
                        txt = inputs[f];
                    }

                    var (r, _, _) = DoParse(parser_type, txt, "", inputs[f], f, data);
                    result = result == 0 ? r : result;
                }
            }
            else if (config.ReadFileNameFile != null)
            {
                List<string> inputs = new List<string>();
                inputs = File.ReadAllLines(config.ReadFileNameFile).ToList();
                for (int f = 0; f < inputs.Count(); ++f)
                {
                    try
                    {
                        txt = File.ReadAllText(inputs[f]);
                    }
                    catch
                    {
                        txt = inputs[f];
                    }

                    var (r, _, _) = DoParse(parser_type, txt, "", inputs[f], f, data);
                    result = result == 0 ? r : result;
                }
            }
            else if (config.Input == null && (config.Files == null || config.Files.Count() == 0))
            {
                string lines = null;
                for (;;)
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }

                txt = lines;
                (result, _, _) = DoParse(parser_type, txt, "", "stdin", 0, data);
            }
            else if (config.Input != null)
            {
                txt = config.Input;
                (result, _, _) = DoParse(parser_type, txt, "", "string", 0, data);
            }
            else if (config.Files != null)
            {
                int f = 0;
                foreach (var file in config.Files)
                {
                    try
                    {
                        txt = File.ReadAllText(file);
                    }
                    catch
                    {
                        txt = file;
                    }

                    var (r, _, _) = DoParse(parser_type, txt, "", file, f, data);
                    result = result == 0 ? r : result;
                    f++;
                }
            }

            if (config.Verbose)
            {
                foreach (var d in data)
                {
                    foreach (var t in d.Nodes)
                    {
                        if (config.Verbose)
                            LoggerNs.TimedStderrOutput.WriteLine(new TreeOutput(d.Lexer, d.Parser).OutputTree(t)
                                .ToString());
                    }
                }
            }

            foreach (var d in data)
            {
                foreach (var t1 in d.Nodes)
                {
                    var count = 0;
                    foreach (var t2 in d.Nodes)
                    {
                        if (t1 == t2) count++;
                        if (count > 1) throw new Exception();
                    }
                }
            }

            DateTime overallAfter = DateTime.Now;
            PrintPerfSummary((overallAfter - overallBefore).TotalSeconds);

            if (config.NoParsingResultSets) return result;
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            string js1 = JsonSerializer.Serialize(data.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            if (!config.Quiet) System.Console.WriteLine(js1);
        }
        catch (Exception e)
        {
            System.Console.Error.WriteLine(e.ToString());
            result = 1;
            System.Console.Out.WriteLine();
        }

        return result;
    }

    private void UpdateStats(double parseSeconds, long tokenCount)
    {
        if (_fileCount == 0)
        {
            _firstFileTokens = tokenCount;
            _firstFileParseSeconds = parseSeconds;
        }
        _totalParseSeconds += parseSeconds;
        _totalTokens += tokenCount;
        _fileCount++;
    }

    private void PrintPerfSummary(double overallSeconds)
    {
        if (config.Quiet) return;
        var warmTokens = _totalTokens - _firstFileTokens;
        var warmSeconds = _totalParseSeconds - _firstFileParseSeconds;
        var warmTps = (_fileCount > 1 && warmSeconds > 0)
            ? ((long)(warmTokens / warmSeconds)).ToString()
            : "n.a.";
        var firstTps = _firstFileParseSeconds > 0 ? (_firstFileTokens / _firstFileParseSeconds) : 0;
        var speedup = (_fileCount > 1 && warmSeconds > 0 && firstTps > 0)
            ? ((warmTokens / warmSeconds) / firstTps).ToString("F2")
            : "n.a.";
        System.Console.Error.WriteLine("PT: " + _totalParseSeconds);
        System.Console.Error.WriteLine("OT: " + (overallSeconds - _totalParseSeconds));
        System.Console.Error.WriteLine("TT: " + overallSeconds);
        System.Console.Error.WriteLine("TPS: " + (_totalParseSeconds > 0 ? (long)(_totalTokens / _totalParseSeconds) : 0));
        System.Console.Error.WriteLine("Post-warmup TPS: " + warmTps);
        System.Console.Error.WriteLine("Post-warmup speed up: " + speedup);
    }

    (int ExitCode, double ParseSeconds, long TokenCount) DoParse(string parser_type,
        string txt,
        string prefix,
        string input_name,
        int row_number,
        List<AntlrJson.ParsingResultSet> data)
    {
        // Interp-file-based Earley parsing path
        bool hasPInterp = !string.IsNullOrEmpty(config.PInterp);
        bool hasLInterp = !string.IsNullOrEmpty(config.LInterp);
        if (hasPInterp != hasLInterp)
        {
            System.Console.Error.WriteLine(
                "Error: --pinterp and --linterp must be specified together.");
            return (1, 0, 0);
        }
        string resolvedPInterp = hasPInterp
            ? ResolveInterpPath(config.PInterp, config.Lib)
            : null;
        string resolvedLInterp = hasLInterp
            ? ResolveInterpPath(config.LInterp, config.Lib)
            : null;

        // Auto-discover interp files when --lib is given without explicit --pinterp/--linterp.
        if (resolvedPInterp == null && resolvedLInterp == null && !string.IsNullOrEmpty(config.Lib))
        {
            var discovered = DiscoverInterpPair(config.Lib);
            resolvedPInterp = discovered.pinterp;
            resolvedLInterp = discovered.linterp;
        }

        if (resolvedPInterp != null && resolvedLInterp != null)
        {
            DateTime interpBefore = DateTime.Now;
            AntlrJson.ParsingResultSet rs;
            long interpTokenCount;
            string interpLabel;
            if (config.AllStar)
            {
                AllStarAtnParser.AllStarParser.Trace = config.Verbose;
                (rs, interpTokenCount) = AllStarAtnParser.AllStarRunner.Run(
                    resolvedPInterp, resolvedLInterp, txt, input_name, config.LineNumbers);
                interpLabel = "ALL(*)";
            }
            else
            {
                (rs, interpTokenCount) = EarleyAtnParser.InterpRunner.Run(
                    resolvedPInterp, resolvedLInterp, txt, input_name, config.LineNumbers);
                interpLabel = "Earley";
            }
            DateTime interpAfter = DateTime.Now;
            double interpParseSeconds = (interpAfter - interpBefore).TotalSeconds;
            data.Add(rs);
            if (!config.Quiet)
            {
                long tps = interpParseSeconds > 0 ? (long)(interpTokenCount / interpParseSeconds) : 0L;
                System.Console.Error.WriteLine(prefix + interpLabel + " " + row_number + " " + input_name + " success "
                    + interpParseSeconds + " s " + interpTokenCount + " tokens " + tps + " tps");
            }
            UpdateStats(interpParseSeconds, interpTokenCount);
            return (0, interpParseSeconds, interpTokenCount);
        }

        Type type = null;
        if (parser_type == null || parser_type == "")
        {
            var extension = Path.GetExtension(input_name);
            // There are two choices:
            // If a Generated-CSharp directory exists, use that.
            // If the directory does not exist, pick based on
            // file extension.
            parser_type = extension switch
            {
                ".g4" => "ANTLRv4",
                ".g3" => "ANTLRv3",
                ".g2" => "ANTLRv2",
                ".peg" => "pegen_v3_10",
                ".rex" => "rex",
                ".y" => "Bison",
                ".lark" => "Lark",
                ".cf" => "LBNF",
                ".ebnf" => "W3CEBNF",
                ".xtext" => "Xtext",
                ".jj" => "Javacc",
                ".jjt" => "Javacc",
                ".abnf" => "ABNF",
                ".iso14977" => "Iso14977",
                ".iso" => "Iso14977",
                ".pegjs" => "Pegjs",
                ".pest" => "Pest",
                ".ixml" => "ixml",
                _ => null
            };
            var subdir = parser_type switch
            {
                "ANTLRv4" => "antlr4",
                "ANTLRv3" => "antlr3",
                "ANTLRv2" => "antlr2",
                "pegen_v3_10" => "pegen",
                "rex" => "rex",
                "Bison" => "bison",
                "Lark" => "lark",
                "LBNF" => "lbnf",
                "W3CEBNF" => "w3cebnf",
                "Xtext" => "xtext",
                "Javacc" => "javacc",
                "ABNF" => "abnf",
                "Iso14977" => "iso14977",
                "Pegjs" => "pegjs",
                "Pest" => "pest",
                "Grammophone" => "grammophone",
                "Princeton" => "princeton",
                "ixml" => "ixml",
                _ => null
            };
            if (subdir != null)
            {
                // Get this assembly.
                System.Reflection.Assembly a = this.GetType().Assembly;
                string path = a.Location;
                path = Path.GetDirectoryName(path);
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path;
                var exists = File.Exists(full_path + subdir + ".dll");
                full_path = Path.GetFullPath(full_path);
                Assembly asm = Assembly.LoadFile(full_path + subdir + ".dll");
                Type[] types = asm.GetTypes();
                type = asm.GetType("Program");
            }
            else
            {
                string path = config.ParserLocation != null
                    ? config.ParserLocation
                    : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path + "Generated-CSharp/bin/Debug/net10.0/";
                var exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Debug/net10.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "Generated-CSharp/bin/Release/net10.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Release/net10.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (exists)
                {
                    full_path = Path.GetFullPath(full_path);
                    Assembly asm = Assembly.LoadFile(full_path + config.Dll + ".dll");
                    Type[] types = asm.GetTypes();
                    type = asm.GetType("Program");
                }
            }
        }
        else if (parser_type == "gen")
        {
            System.Console.Error.WriteLine("Using Generated-CSharp parser.");
            string path = config.ParserLocation != null
                ? config.ParserLocation
                : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path = path + "/";
            var candidates = new[]
            {
                path + "Generated-CSharp/bin/Debug/net10.0/",
                path + "bin/Debug/net10.0/",
                path + "Generated-CSharp/bin/Release/net10.0/",
                path + "bin/Release/net10.0/",
            };
            string found_path = null;
            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate + config.Dll + ".dll"))
                {
                    found_path = Path.GetFullPath(candidate);
                    break;
                }
            }
            if (found_path != null)
            {
                Assembly asm = Assembly.LoadFile(found_path + config.Dll + ".dll");
                type = asm.GetType("Program");
            }
            else
            {
                System.Console.Error.WriteLine(
                    "gen: could not find '" + config.Dll + ".dll' in any of:");
                foreach (var c in candidates)
                    System.Console.Error.WriteLine("  " + Path.GetFullPath(c));
                System.Console.Error.WriteLine(
                    "Did you run 'dotnet build' in the Generated-CSharp directory?");
            }
        }
        else
        {
            System.Console.Error.WriteLine("Using built-in parser.");
            var subdir = parser_type switch
            {
                "ANTLRv4" => "antlr4",
                "ANTLRv3" => "antlr3",
                "ANTLRv2" => "antlr2",
                "pegen_v3_10" => "pegen",
                "rex" => "rex",
                "Bison" => "bison",
                "Lark" => "lark",
                "LBNF" => "lbnf",
                "W3CEBNF" => "w3cebnf",
                "Xtext" => "xtext",
                "Javacc" => "javacc",
                "ABNF" => "abnf",
                "Iso14977" => "iso14977",
                "Pegjs" => "pegjs",
                "Pest" => "pest",
                "Grammophone" => "grammophone",
                "Princeton" => "princeton",
                "ixml" => "ixml",
                _ => throw new Exception(
                    "Unknown built-in parser type. Supported: ANTLRv4, ANTLRv3, ANTLRv2, Bison, Lark, rex, pegen_v3_10, LBNF, W3CEBNF, Xtext, Javacc, ABNF, Iso14977, Pegjs, Pest, Grammophone, Princeton, ixml, gen.")
            };
            // Get this assembly.
            System.Reflection.Assembly a = this.GetType().Assembly;
            string path = a.Location;
            path = Path.GetDirectoryName(path);
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/")) path = path + "/";
            var full_path = path;
            var exists = File.Exists(full_path + subdir + ".dll");
            full_path = Path.GetFullPath(full_path);
            Assembly asm = Assembly.LoadFile(full_path + subdir + ".dll");
            Type[] types = asm.GetTypes();
            type = asm.GetType("Program");
        }

        if (type == null)
        {
            System.Console.Error.WriteLine(
                "No parser found for input '" + input_name + "'. " +
                "Specify a grammar type with -t, point to a generated parser with -p, " +
                "or use --pinterp / --linterp for Earley ATN-based parsing.");
            return (1, 0, 0);
        }

        MethodInfo methodInfo = type.GetMethod("SetupParse2");
        object[] parm1 = new object[] { txt, input_name, config.Quiet };
        var res = methodInfo.Invoke(null, parm1);

        var result = "";
        object res2 = null;
        DateTime before = DateTime.Now;
        DateTime after = DateTime.Now;
        {
            MethodInfo methodInfo2 = type.GetMethod("Parse2");
            object[] parm2 = new object[] { };
            before = DateTime.Now;
            res2 = methodInfo2.Invoke(null, parm2);
            after = DateTime.Now;

            MethodInfo methodInfo3 = type.GetMethod("AnyErrors");
            object[] parm3 = new object[] { };
            var res3 = methodInfo3.Invoke(null, parm3);
            if ((bool)res3)
            {
                result = "fail";
            }
            else
            {
                result = "success";
            }
        }

        double parseSeconds = (after - before).TotalSeconds;
        var parser = type.GetProperty("Parser").GetValue(null, new object[0]) as Antlr4.Runtime.Parser;
        var lexer = type.GetProperty("Lexer").GetValue(null, new object[0]) as Antlr4.Runtime.Lexer;
        var tokstream = type.GetProperty("TokenStream").GetValue(null, new object[0]) as ITokenStream;
        var charstream = type.GetProperty("CharStream").GetValue(null, new object[0]) as ICharStream;
        var commontokstream = tokstream as CommonTokenStream;
        long tokenCount = commontokstream != null ? (long)commontokstream.Size : 0L;
        var r5 = type.GetProperty("Input").GetValue(null, new object[0]);

        if (!config.Quiet)
        {
            long tps = parseSeconds > 0 ? (long)(tokenCount / parseSeconds) : 0L;
            System.Console.Error.WriteLine(prefix + "CSharp " + row_number + " " + input_name + " " + result + " "
                + parseSeconds + " s " + tokenCount + " tokens " + tps + " tps");
        }

        {
            var tuples = res2 as List<Tuple<string, IParseTree>>;
            // Each ambiguous parse tree is for an alt.
            // Two ways to group this:
            // 1) All trees under one file, one decision.
            // 2) Or, each tree under one file, one decision, one alt.
            if (config.GroupBy)
            {
                string decision_str = "";
                var list_of_trees = new List<UnvParseTreeNode>();
                foreach (var tt in tuples)
                {
                    var t1 = tt.Item1 as string;
                    decision_str = t1;
                    var t2 = tt.Item2 as ParserRuleContext;
                    var converted_tree =
                        new ConvertToDOM(config.LineNumbers).BottomUpConvert(t2, null, parser, lexer, commontokstream);
                    list_of_trees.Add(converted_tree);
                }
                var tuple = new AntlrJson.ParsingResultSet()
                {
                    FileName = input_name + "." + decision_str,
                    Nodes = list_of_trees.ToArray(),
                    Parser = parser,
                    Lexer = lexer
                };
                data.Add(tuple);
            } else {
                foreach (var tt in tuples)
                {
                    var list_of_trees = new List<UnvParseTreeNode>();
                    var t1 = tt.Item1 as string;
                    var t2 = tt.Item2 as ParserRuleContext;
                    var converted_tree =
                        new ConvertToDOM(config.LineNumbers).BottomUpConvert(t2, null, parser, lexer, commontokstream);
                    var tuple = new AntlrJson.ParsingResultSet()
                    {
                        FileName = input_name + (tt.Item1 != null ? "." + tt.Item1 : ""),
                        Nodes = new UnvParseTreeNode[] { converted_tree },
                        Parser = parser,
                        Lexer = lexer
                    };
                    data.Add(tuple);
                }
            }
        }

        UpdateStats(parseSeconds, tokenCount);
        return (result == "success" ? 0 : 1, parseSeconds, tokenCount);
    }

    private static string ResolveInterpPath(string path, string lib)
    {
        if (string.IsNullOrEmpty(lib) || Path.IsPathRooted(path))
            return path;
        return Path.Combine(lib, path);
    }

    /// <summary>
    /// Scans <paramref name="dir"/> for matched parser + lexer .interp pairs.
    /// Handles both naming conventions produced by trinterp:
    ///   • Combined grammar "Foo"  → Foo.interp + FooLexer.interp
    ///   • Explicit grammars "FooParser"/"FooLexer" → FooParser.interp + FooLexer.interp
    /// Returns the pair when exactly one match is found; throws otherwise.
    /// </summary>
    private static (string pinterp, string linterp) DiscoverInterpPair(string dir)
    {
        if (!Directory.Exists(dir))
            throw new DirectoryNotFoundException($"--lib directory not found: '{dir}'");

        var pairs = new List<(string p, string l)>();
        foreach (var pf in Directory.GetFiles(dir, "*.interp"))
        {
            var stem = Path.GetFileNameWithoutExtension(pf);
            if (stem.EndsWith("Lexer")) continue; // skip lexer files

            // Case 1: combined grammar — Foo.interp pairs with FooLexer.interp
            var lf = Path.Combine(dir, stem + "Lexer.interp");
            if (File.Exists(lf)) { pairs.Add((pf, lf)); continue; }

            // Case 2: explicit parser grammar — FooParser.interp pairs with FooLexer.interp
            if (stem.EndsWith("Parser"))
            {
                var baseName = stem.Substring(0, stem.Length - "Parser".Length);
                lf = Path.Combine(dir, baseName + "Lexer.interp");
                if (File.Exists(lf)) pairs.Add((pf, lf));
            }
        }

        if (pairs.Count == 0)
            throw new FileNotFoundException(
                $"No matched parser/lexer .interp pair found in '{dir}'. " +
                "Use --pinterp / --linterp to specify them explicitly.");
        if (pairs.Count > 1)
            throw new InvalidOperationException(
                $"Multiple parser/lexer .interp pairs found in '{dir}': " +
                string.Join(", ", pairs.Select(p => Path.GetFileNameWithoutExtension(p.p))) +
                ". Use --pinterp / --linterp to specify which pair to use.");

        return pairs[0];
    }
}
