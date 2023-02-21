namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using AntlrTreeEditing.AntlrDOM;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;

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

        public int Run(string parser_type = "")
        {
            int result = 0;
            try
            {
                var data = new List<AntlrJson.ParsingResultSet>();
                string txt = config.Input;
                if (config.ReadFileNameStdin)
                {
                    List<string> inputs = new List<string>();
                    for (; ; )
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
                else if (config.Input == null && (config.Files == null || config.Files.Count() == 0))
                {
                    string lines = null;
                    for (; ; )
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
                            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine(TreeOutput.OutputTree(t, d.Lexer, d.Parser).ToString());
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
                System.Console.WriteLine(e.ToString());
                result = 1;
            }
            finally
            {
            }
            return result;
        }

        int DoParse(string parser_type, string txt, string prefix, string input_name, int row_number, List<AntlrJson.ParsingResultSet> data)
        {
            Type type = null;
            if (parser_type == null || parser_type == "")
            {
                string path = config.ParserLocation != null ? config.ParserLocation
                    : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path + "Generated-CSharp/bin/Debug/net7.0/";
                var exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Debug/net7.0/";
                full_path = Path.GetFullPath(full_path);
                Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
                Assembly asm = Assembly.LoadFile(full_path + config.Dll + ".dll");
                var xxxxxx = asm1.GetTypes();
                Type[] types = asm.GetTypes();
                type = asm.GetType("Program");
            }
            else
            {
                // Get this assembly.
                System.Reflection.Assembly a = this.GetType().Assembly;
                string path = a.Location;
                path = Path.GetDirectoryName(path);
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path;
                var exists = File.Exists(full_path + parser_type + ".dll");
                full_path = Path.GetFullPath(full_path);
                Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
                Assembly asm = Assembly.LoadFile(full_path + parser_type + ".dll");
                var xxxxxx = asm1.GetTypes();
                Type[] types = asm.GetTypes();
                type = asm.GetType("Program");
            }

            MethodInfo methodInfo = type.GetMethod("SetupParse2");
            object[] parm1 = new object[] { txt, config.Quiet };
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
            System.Console.Error.WriteLine(prefix + "CSharp " + row_number + " " + input_name + " " + result + " " + (after - before).TotalSeconds);
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
            var tuple = new AntlrJson.ParsingResultSet() { Text = (r5 as string), FileName = input_name, Nodes = new AntlrNode[] { converted_tree }, Parser = parser, Lexer = lexer };
            data.Add(tuple);
            return (bool)res3 ? 1 : 0;
        }
    }
}
