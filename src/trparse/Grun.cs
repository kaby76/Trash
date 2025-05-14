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

    public int Run(string parser_type = null)
    {
        int result = 0;
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

                DateTime before = DateTime.Now;
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

                    var r = DoParse(parser_type, txt, "", inputs[f], f, data);
                    result = result == 0 ? r : result;
                }

                DateTime after = DateTime.Now;
                System.Console.Error.WriteLine("Total Time: " + (after - before).TotalSeconds);
            }
            else if (config.ReadFileNameFile != null)
            {
                List<string> inputs = new List<string>();
                inputs = File.ReadAllLines(config.ReadFileNameFile).ToList();
                DateTime before = DateTime.Now;
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

                    var r = DoParse(parser_type, txt, "", inputs[f], f, data);
                    result = result == 0 ? r : result;
                }

                DateTime after = DateTime.Now;
                System.Console.Error.WriteLine("Total Time: " + (after - before).TotalSeconds);
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
                result = DoParse(parser_type, txt, "", "stdin", 0, data);
            }
            else if (config.Input != null)
            {
                txt = config.Input;
                result = DoParse(parser_type, txt, "", "string", 0, data);
            }
            else if (config.Files != null)
            {
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

                    var r = DoParse(parser_type, txt, "", file, 0, data);
                    result = result == 0 ? r : result;
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

    int DoParse(string parser_type, string txt, string prefix, string input_name, int row_number,
        List<AntlrJson.ParsingResultSet> data)
    {
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
                var full_path = path + "Generated-CSharp/bin/Debug/net8.0/";
                var exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Debug/net8.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "Generated-CSharp/bin/Release/net8.0/";
                exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Release/net8.0/";
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
                _ => throw new Exception(
                    "Unknown built-in parser type, should be one of ANTLRv4, ANTLRv3, ANTLRv2, pegen_v3_10, rex, Bison.")
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

        MethodInfo methodInfo = type.GetMethod("SetupParse2");
        object[] parm1 = new object[] { txt, config.Quiet };
        var res = methodInfo.Invoke(null, parm1);

        var result = "";
        object res2 = null;
        object res3 = null;
        DateTime before = DateTime.Now;
        DateTime after = DateTime.Now;
        if (!config.Ambig)
        {
            MethodInfo methodInfo2 = type.GetMethod("Parse2");
            object[] parm2 = new object[] { };
            before = DateTime.Now;
            res2 = methodInfo2.Invoke(null, parm2);
            after = DateTime.Now;

            MethodInfo methodInfo3 = type.GetMethod("AnyErrors");
            object[] parm3 = new object[] { };
            res3 = methodInfo3.Invoke(null, parm3);
            if ((bool)res3)
            {
                result = "fail";
            }
            else
            {
                result = "success";
            }
        }
        else
        {
            MethodInfo methodInfo2 = type.GetMethod("Parse3");
            object[] parm2 = new object[] { };
            before = DateTime.Now;
            res2 = methodInfo2.Invoke(null, parm2);
            after = DateTime.Now;

            MethodInfo methodInfo3 = type.GetMethod("AnyErrors");
            object[] parm3 = new object[] { };
            res3 = methodInfo3.Invoke(null, parm3);
            if ((bool)res3)
            {
                result = "fail";
            }
            else
            {
                result = "success";
            }
        }

        System.Console.Error.WriteLine(prefix + "CSharp " + row_number + " " + input_name + " " + result + " " +
                                       (after - before).TotalSeconds);
        var parser = type.GetProperty("Parser").GetValue(null, new object[0]) as Antlr4.Runtime.Parser;
        var lexer = type.GetProperty("Lexer").GetValue(null, new object[0]) as Antlr4.Runtime.Lexer;
        var tokstream = type.GetProperty("TokenStream").GetValue(null, new object[0]) as ITokenStream;
        var charstream = type.GetProperty("CharStream").GetValue(null, new object[0]) as ICharStream;
        var commontokstream = tokstream as CommonTokenStream;
        var r5 = type.GetProperty("Input").GetValue(null, new object[0]);
        if (!config.Ambig)
        {
            var tree = res2 as IParseTree;
            var t2 = tree as ParserRuleContext;
            var converted_tree = new ConvertToDOM(config.LineNumbers).BottomUpConvert(t2, null, parser, lexer, commontokstream, charstream);
            var tuple = new AntlrJson.ParsingResultSet()
            {
                FileName = input_name,
                Nodes = new UnvParseTreeNode[] { converted_tree },
                Parser = parser,
                Lexer = lexer
            };
            data.Add(tuple);
        }
        else
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
                        new ConvertToDOM(config.LineNumbers).BottomUpConvert(t2, null, parser, lexer, commontokstream,
                            charstream);
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
                        new ConvertToDOM(config.LineNumbers).BottomUpConvert(t2, null, parser, lexer, commontokstream,
                            charstream);
                    var tuple = new AntlrJson.ParsingResultSet()
                    {
                        FileName = input_name + "." + tt.Item1,
                        Nodes = new UnvParseTreeNode[] { converted_tree },
                        Parser = parser,
                        Lexer = lexer
                    };
                    data.Add(tuple);
                }
            }
        }
        return (bool)res3 ? 1 : 0;
    }
}
