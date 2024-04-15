namespace Trash
{
    using Antlr4.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

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
                        Doit(inputs[f], txt);
                    }
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
                    Doit("stdin", txt);
                }
                else if (config.Input != null)
                {
                    txt = config.Input;
                    Doit("string", txt);
                }
                else if (config.Files != null)
                {
                    foreach (var file in config.Files)
                    {
                        txt = File.ReadAllText(file);
                        Doit(file, txt);
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                result = 1;
            }
            return result;
        }

        void Doit(string fn, string txt)
        {
            string path = config.ParserLocation != null ? config.ParserLocation
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
            full_path = Path.GetFullPath(full_path).Replace("\\", "/");
            Assembly asm = Assembly.LoadFile(full_path + "Test.dll");
            Type[] types = asm.GetTypes();
            Type type = asm.GetType("Program");
            var methods = type.GetMethods();
            {
                MethodInfo methodInfo = type.GetMethod("SetupParse2");
                object[] parm = new object[] { txt, false };
                var res = methodInfo.Invoke(null, parm);
            }
            // Set perf.
            var r2 = type.GetProperty("Parser").GetValue(null, new object[0]);
            var parser = r2 as Parser;
            parser.Profile = true;
            {
                MethodInfo methodInfo = type.GetMethod("Parse2");
                object[] parm = new object[] { };
                DateTime before = DateTime.Now;
                var res = methodInfo.Invoke(null, parm);
                DateTime after = DateTime.Now;
                System.Console.Error.WriteLine("Time to parse: " + (after - before));
                bool do_tab = false;
                if (config.HeaderNames)
                {
                    for (int c = 0; c < config.Columns.Length; ++c)
                    {
                        if (config.Columns[c] == 'F')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("File");
                        }

                        if (config.Columns[c] == 'd')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Decision");
                        }

                        if (config.Columns[c] == 'r')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Rule");
                        }

                        if (config.Columns[c] == 'i')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Invocations");
                        }

                        if (config.Columns[c] == 'T')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Time");
                        }

                        if (config.Columns[c] == 'k')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Total-k");
                        }

                        if (config.Columns[c] == 'm')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Max-k");
                        }

                        if (config.Columns[c] == 'f')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Fallback");
                        }

                        if (config.Columns[c] == 'a')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Ambiguities");
                        }

                        if (config.Columns[c] == 'e')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Errors");
                        }

                        if (config.Columns[c] == 't')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Transitions");
                        }
                    }
                    System.Console.WriteLine();
                }
                var di = parser.ParseInfo.getDecisionInfo();
                for (int i = 0; i < di.Length; i++)
                {
                    var r = di[i];
                    var decision = r.decision;
                    var atn = parser.Atn;
                    var state = atn.decisionToState[decision];
                    var rule_index = state.ruleIndex;
                    var rule_name = parser.RuleNames[rule_index];
                    var maxLook = Math.Max(r.LL_MaxLook, r.SLL_MaxLook);
                    do_tab = false;
                    for (int c = 0; c < config.Columns.Length; ++c)
                    {
                        if (config.Columns[c] == 'F')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(fn);
                        }

                        if (config.Columns[c] == 'd')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(decision);
                        }

                        if (config.Columns[c] == 'r')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(rule_name);
                        }

                        if (config.Columns[c] == 'i')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(r.invocations);
                        }

                        if (config.Columns[c] == 'T')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(r.timeInPrediction / (1000.0 * 1000.0));
                        }

                        if (config.Columns[c] == 'k')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write((r.LL_TotalLook + r.SLL_TotalLook));
                        }

                        if (config.Columns[c] == 'm')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(maxLook);
                        }

                        if (config.Columns[c] == 'f')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(r.LL_Fallback);
                        }

                        if (config.Columns[c] == 'a')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(r.errors.Count);
                        }

                        if (config.Columns[c] == 'e')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write(r.errors.Count);
                        }

                        if (config.Columns[c] == 't')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write((r.SLL_ATNTransitions + r.LL_ATNTransitions));
                        }
                    }
                    System.Console.WriteLine();
                }
            }
        }
    }
}
