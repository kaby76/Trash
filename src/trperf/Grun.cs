using Algorithms;
using Antlr4.Runtime.Misc;

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

                        Doit(inputs[f], txt);
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
            full_path = Path.GetFullPath(full_path).Replace("\\", "/");
            Assembly asm = Assembly.LoadFile(full_path + "Test.dll");
            Type[] types = asm.GetTypes();
            Type program = asm.GetType("Program");
            var methods = program.GetMethods();
            if (config.HeatMap)
            {
                var p = program.GetProperty("HeatMap");
                p.SetValue(null, true, new object[0]);
            }

            {
                MethodInfo methodInfo = program.GetMethod("SetupParse2");
                object[] parm = new object[] { txt, false };
                var res = methodInfo.Invoke(null, parm);
            }
            // Set perf.
            var r2 = program.GetProperty("Parser").GetValue(null, new object[0]);
            var parser = r2 as Parser;

            parser.Profile = true;
            {
                MethodInfo methodInfo = program.GetMethod("Parse2");
                object[] parm = new object[] { };
                DateTime before = DateTime.Now;
                var res = methodInfo.Invoke(null, parm);
                DateTime after = DateTime.Now;
                System.Console.Error.WriteLine("Time to parse: " + (after - before));
                var r3 = program.GetProperty("TokenStream").GetValue(null, new object[0]);
                var tokenstream = r3 as CommonTokenStream;
                var tokens = tokenstream.GetTokens();
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

                        if (config.Columns[c] == 'c')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            System.Console.Write("Input");
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
                            if (r.LL_MaxLook > r.SLL_MaxLook)
                            {
                                System.Console.Write(r.LL_MaxLook);
                            }
                            else
                            {
                                System.Console.Write(r.SLL_MaxLook);
                            }
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

                        if (config.Columns[c] == 'c')
                        {
                            if (do_tab) System.Console.Write("\t");
                            do_tab = true;
                            string input = "";
                            if (r.LL_MaxLook > r.SLL_MaxLook)
                            {
                                if (r.LL_MaxLookEvent != null)
                                {
                                    var si = r.LL_MaxLookEvent.startIndex;
                                    var ei = r.LL_MaxLookEvent.stopIndex;
                                    var tsi = tokens[si];
                                    var tei = tokens[ei];
                                    var csi = tsi.StartIndex;
                                    var cei = tei.StopIndex;
                                    input = tokenstream.TokenSource.InputStream.GetText(Interval.Of(csi, cei));
                                    input = new xpath.org.eclipse.wst.xml.xpath2.processor.@internal.OutputParseTree()
                                        .PerformEscapes(input);
                                }
                            }
                            else
                            {
                                if (r.SLL_MaxLookEvent != null)
                                {
                                    var si = r.SLL_MaxLookEvent.startIndex;
                                    var ei = r.SLL_MaxLookEvent.stopIndex;
                                    var tsi = tokens[si];
                                    var tei = tokens[ei];
                                    var csi = tsi.StartIndex;
                                    var cei = tei.StopIndex;
                                    input = tokenstream.TokenSource.InputStream.GetText(Interval.Of(csi, cei));
                                    input = new xpath.org.eclipse.wst.xml.xpath2.processor.@internal.OutputParseTree()
                                        .PerformEscapes(input);
                                }
                            }

                            System.Console.Write(input);
                        }
                    }

                    System.Console.WriteLine();
                }

                if (config.HeatMap)
                {
                    StringBuilder b = new StringBuilder();
                    b.Append(@"<!DOCTYPE html>
<html>
<head>
<style>
    body {
    }

    .tooltip {
      position: relative;
      display: inline-block;
      cursor: default;
    }

    .tooltip .tooltiptext {
      visibility: hidden;
      padding: 0.25em 0.5em;
      background-color: black;
      color: #fff;
      text-align: left;
      border-radius: 0.25em;
      white-space: nowrap;
        font-size: 0.25em;
      
      /* Position the tooltip */
      position: absolute;
      z-index: 1;
      top: 100%;
      left: 0%;
      transition-property: visibility;
      transition-delay: 0s;
    }

    .tooltip:hover .tooltiptext {
      visibility: visible;
      transition-delay: 0.3s;
    }

</style>
</head>
<body>
");
                    b.Append("<pre><code>");
                    Type pcts = asm.GetType("ProfilingCommonTokenStream");
                    // Pass 1. Find max.
                    var hit_count = pcts.GetField("HitCount").GetValue(tokenstream) as Dictionary<int, int>;
                    var call_stack =
                        pcts.GetField("CallStacks").GetValue(tokenstream) as Dictionary<int, Dictionary<string, int>>;
                    int max = 0;
                    for (int i = 0; i < tokens.Count(); ++i)
                    {
                        var token = tokens[i];
                        if (hit_count.ContainsKey(token.TokenIndex))
                        {
                            if (hit_count[token.TokenIndex] > max)
                            {
                                max = hit_count[token.TokenIndex];
                            }
                        }
                    }

                    // Pass 2. Identify list of tokens and chars to output.
                    List<SumType<char, IToken>> output = new List<SumType<char, IToken>>();
                    for (int i = 0; i < txt.Length; ++i)
                    {
                        var c = txt[i];
                        var ts = tokens.Where(t =>
                        {
                            var t1 = t.StartIndex;
                            var t2 = t.StopIndex;
                            return t1 <= i && i < t.StopIndex + 1;
                        }).ToList();
                        bool found = false;
                        foreach (var token in ts)
                        {
                            var char_index_start = token.StartIndex;
                            var char_index_stop = token.StopIndex;
                            var str_text = txt.Substring(char_index_start, char_index_stop - char_index_start + 1);
                            if (str_text == token.Text)
                            {
                                if (hit_count.ContainsKey(token.TokenIndex))
                                {
                                    found = true;
                                    if (output.Any() && output.Last() == new SumType<char, IToken>(token))
                                    {
                                        break;
                                    }

                                    output.Add(new SumType<char, IToken>(token));
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            output.Add(new SumType<char, IToken>(c));
                        }
                    }


                    // Pass 3. Output.
                    for (int i = 0; i < output.Count; ++i)
                    {
                        var o = output[i];
                        if (o.Value is IToken token)
                        {
                            if (token.Text.Contains('\n'))
                            {
                                b.Append(token.Text);
                            }
                            else
                            {
                                var count = hit_count[token.TokenIndex];
                                int scaled_count = count * 100 / max;
                                Dictionary<string, int> cs = call_stack[token.TokenIndex];
                                string cs_str = cs.Aggregate("", (acc, kv) => acc + kv.Key + " " + kv.Value + "<br>");
                                b.Append("<div class=\"tooltip\">");
                                b.Append("<b style=\"background-color:" + fun(scaled_count) + ";\">");
                                b.Append(token.Text);
                                b.Append("</b>");
                                b.Append("<span class=\"tooltiptext\">");
                                b.Append("LA() count = " + count + "<br>" + cs_str + ">");
                                b.Append("</span></div>");
                            }
                        }
                        else if (o.Value is char c)
                        {
                            if (c == '\n')
                                b.Append("<br>");
                            else
                                b.Append(c);
                        }
                    }
                    b.Append("</code></pre></body>");
                    File.WriteAllText("cover.html", b.ToString());
                }
            }

            string fun(int v)
            {
                switch (v)
                {
                    case int n when (n >= 90): return "rgba(255, 0, 0, 1.0)";
                    case int n when (n < 90 && n >= 80): return "rgba(255, 0, 0, 0.9)";
                    case int n when (n < 80 && n >= 70): return "rgba(255, 0, 0, 0.8)";
                    case int n when (n < 70 && n >= 60): return "rgba(255, 0, 0, 0.7)";
                    case int n when (n < 60 && n >= 50): return "rgba(255, 125, 0, 0.6)";
                    case int n when (n < 50 && n >= 40): return "rgba(255, 125, 0, 0.5)";
                    case int n when (n < 40 && n >= 30): return "rgba(255, 125, 0, 0.4)";
                    case int n when (n < 30 && n >= 20): return "rgba(255, 255, 0, 0.3)";
                    case int n when (n < 20 && n >= 10): return "rgba(255, 255, 0, 0.2)";
                    case int n when (n < 10 && n >= 0): return "rgba(255, 255, 0, 0.1)";
                    default:
                        return "0";
                }
            }

        }
    }
}

