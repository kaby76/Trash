using Algorithms;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using trcover;

namespace Trash
{
    internal class Grun
    {
        private Config config;

        public Grun(Config config)
        {
            this.config = config;
        }

        public int Run(string parser_type)
        {
            Type type;
            string full_path = null;
            Antlr4.Runtime.Parser parser = null;
            Antlr4.Runtime.Lexer lexer = null;
            Antlr4.Runtime.ITokenStream token_stream;
            if (parser_type == null || parser_type == "")
            {
                string path = config.ParserLocation != null ? config.ParserLocation
                    : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                full_path = path + "Generated-CSharp/bin/Debug/net7.0/";
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
                var assembly_name = parser_type switch
                {
                    "ANTLRv4" => "antlr4",
                    "ANTLRv3" => "antlr3",
                    "ANTLRv2" => "antlr2",
                    "pegen_v3_10" => "pegen",
                    "rex" => "rex",
                    "Bison" => "bison",
                    _ => throw new Exception("Unknown file extension, cannot load in a built-in parser.")
                };
                // Get this assembly.
                System.Reflection.Assembly a = this.GetType().Assembly;
                string path = a.Location;
                path = Path.GetDirectoryName(path);
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                full_path = path;
                var exists = File.Exists(full_path + assembly_name + ".dll");
                full_path = Path.GetFullPath(full_path);
                Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
                Assembly asm = Assembly.LoadFile(full_path + assembly_name + ".dll");
                var xxxxxx = asm1.GetTypes();
                Type[] types = asm.GetTypes();
                type = asm.GetType("Program");
            }
            MethodInfo methodInfo = type.GetMethod("SetupParse2");
            object[] parm1 = new object[] { "", config.Quiet };
            var res = methodInfo.Invoke(null, parm1);
            parser = type.GetProperty("Parser").GetValue(null, new object[0]) as Antlr4.Runtime.Parser;
            lexer = type.GetProperty("Lexer").GetValue(null, new object[0]) as Antlr4.Runtime.Lexer;
            token_stream = type.GetProperty("TokenStream").GetValue(null, new object[0]) as ITokenStream;

            // We have a pre-build grammar, but we need to get the grammar text
            // itself. Go to the dll, and work up to find the .g4's.
            // Then, parse the Antlr4 parser grammar to create NFAs that
            // model the regular expression on the RHS of each rule.
            // First, get grammars, parse, computing dfa's per rule per grammar file.
            var model = new Model();
            model.ComputeModel(full_path, parser, lexer, token_stream);

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
                        var r = DoParse(model, type, txt, "", inputs[f], f);
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
                    result = DoParse(model, type, txt, "", "stdin", 0);
                }
                else if (config.Input != null)
                {
                    txt = config.Input;
                    result = DoParse(model, type, txt, "", "string", 0);
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
                        var r = DoParse(model, type, txt, "", file, 0);
                        result = result == 0 ? r : result;
                    }
                }

                System.Console.Write("<pre><code>");
                for (int i = 0;; ++i)
                {
                    var t = model._input.Get(i);
                    if (t == null) break;
                    if (t.Type == -1) break;
                    var q = token_count.TryGetValue(t, out int v);
                    if (v > 0)
                    {
                        System.Console.Write("<b style=\"background-color:"
                        + "rgba(255, 99, 71, " + fun(v) + ");\">");
                    }
                    System.Console.Write(t.Text);
                    if (v > 0)
                    {
                        System.Console.Write("</b>");
                    }
                }
                System.Console.WriteLine("</code></pre>");
                System.Console.WriteLine("<br><br>");
                System.Console.WriteLine("Percent rules covered " + 100 * rule_count.Count / 1.0 / model._parser.RuleNames.Length);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                result = 1;
            }
            return result;
        }

        private string fun(int v)
        {
            switch (v)
            {
                case int n when (n >= 75): return "1.0";
                case int n when (n < 75 && n >= 50): return "0.75";
                case int n when (n < 50 && n >= 25): return "0.5";
                case int n when (n < 25 && n >= 1): return "0.25";
                default:
                    return "0";
            }
        }

        int DoParse(Model model, Type type, string txt, string prefix, string input_name, int row_number)
        {
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

            //if (!config.Quiet) System.Console.Error.WriteLine("Time to parse: " + (after - before));
            //if (!config.Quiet) System.Console.Error.WriteLine("# tokens per sec = " + tokstream.Size / (after - before).TotalSeconds);
            //if (!config.Quiet && config.Verbose) System.Console.Error.WriteLine(LanguageServer.TreeOutput.OutputTree(tree, lexer, parser, commontokstream));
            
            // Compute coverage.
            System.Console.Error.WriteLine("Analyzing...");
            ComputeCoverage(model, tree);

            return (bool)res3 ? 1 : 0;
        }

        
        public Dictionary<int, int> rule_count = new Dictionary<int, int>();

        private void ComputeCoverage(Model model, IParseTree tree)
        {
            // Traverse tree and parse each node.
            Stack<IParseTree> stack = new Stack<IParseTree>();
            stack.Push(tree);
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is ITerminalNode)
                {}
                else
                {
                    var x = n as ParserRuleContext;
                    var y = x.RuleIndex;
                    rule_count.TryGetValue(x.RuleIndex, out int rc);
                    rule_count[x.RuleIndex] = rc + 1;
                    var fod = model.Rules.Where(r =>
                    {
                        if (r.lhs_rule_number == y) return true;
                        return false;
                    }).FirstOrDefault();
                    if (fod != null)
                    {
                        ParseRHS(model, x);
                    }
                    for (int i = n.ChildCount - 1; i >= 0; i--)
                    {
                        stack.Push(n.GetChild(i));
                    }
                }
            }
        }

        public Dictionary<IToken, int> token_count = new Dictionary<IToken, int>();

        private HashSet<string> visited;
        private List<SymbolEdge<string>> ParseRecurse(Model model, Digraph<string, SymbolEdge<string>> nfa, IEnumerable<IParseTree> input, SymbolEdge<string> edge)
        {
            input = input.ToList();
            IParseTree c = input.FirstOrDefault();
            if (c == null)
            {
                return new List<SymbolEdge<string>>();
            }
            if (visited.Contains(edge.To))
                return null;
            foreach (var t in nfa.Edges.Where(e => e.From == edge.To))
            {
                var ll = t._symbol;
                IEnumerable<IParseTree> rest = null;
                if (ll == null)
                {
                    rest = input;
                }
                else
                {
                    if (c is ParserRuleContext cc)
                    {
                        var symbol = model._parser.RuleNames[cc.RuleIndex];
                        if (symbol != ll.GetText())
                            continue;
                    }
                    else if (c is TerminalNodeImpl c2)
                    {
                        var ty = c2.Symbol.Type;
                        var dic = model._lexer.Vocabulary;
                        var s1 = dic.GetLiteralName(ty);
                        var s2 = dic.GetSymbolicName(ty);
                        var s3 = dic.GetDisplayName(ty);
                        if (!(ll.GetText() == s1 || ll.GetText() == s2 || ll.GetText() == s3))
                            continue;
                    }
                    rest = input.Skip(1);
                }
                var parse = ParseRecurse(model, nfa, rest, t);
                if (parse != null)
                {
                    var new_parse = new List<SymbolEdge<string>>() { t };
                    foreach (var p in parse) new_parse.Add(p);
                    return new_parse;
                }
            }
            return null;
        }

        private List<SymbolEdge<string>> ParseUsingStack(Model model, Digraph<string, SymbolEdge<string>> nfa,
            IEnumerable<IParseTree> input, SymbolEdge<string> edge)
        {
            var visited = new Dictionary<string, int>();
            var stack = new Stack<Tuple<List<IParseTree>, SymbolEdge<string>, List<SymbolEdge<string>>>>();
            stack.Push(
                new Tuple<List<IParseTree>, SymbolEdge<string>, List<SymbolEdge<string>>>
                    (input.ToList(), edge, new List<SymbolEdge<string>>()));
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var currentInput = current.Item1;
                var currentEdge = current.Item2;
                var up_to_now_pat = current.Item3;

                currentInput = currentInput.ToList();
                var current_input_count = currentInput.Count;
                IParseTree c = currentInput.FirstOrDefault();
                if (c == null)
                {
                    return new List<SymbolEdge<string>>();
                }
                if (visited.TryGetValue(currentEdge.To, out int input_read))
                {
                    if (input_read <= current_input_count) continue;
                }
                visited[currentEdge.To] = current_input_count;

                foreach (var t in nfa.Edges.Where(e => e.From == currentEdge.To))
                {
                    var ll = t._symbol;
                    IEnumerable<IParseTree> rest = null;
                    if (ll == null)
                    {
                        rest = currentInput;
                    }
                    else
                    {
                        if (c is ParserRuleContext cc)
                        {
                            var symbol = model._parser.RuleNames[cc.RuleIndex];
                            if (symbol != ll.GetText())
                                continue;
                        }
                        else if (c is TerminalNodeImpl c2)
                        {
                            var ty = c2.Symbol.Type;
                            var dic = model._lexer.Vocabulary;
                            var s1 = dic.GetLiteralName(ty);
                            var s2 = dic.GetSymbolicName(ty);
                            var s3 = dic.GetDisplayName(ty);
                            if (!(ll.GetText() == s1 || ll.GetText() == s2 || ll.GetText() == s3))
                                continue;
                        }
                        rest = currentInput.Skip(1);
                    }

                    var new_list = up_to_now_pat.ToList(); 
                    new_list.Add(t);
                    var newTuple = new Tuple<List<IParseTree>, SymbolEdge<string>, List<SymbolEdge<string>>>(
                        rest.ToList(),
                        t,
                        new_list
                        );
                    stack.Push(newTuple);

                    if (rest.All(x => x == null))
                    {
                        return new_list;
                    }
                }
            }
            return null;
        }

        private void ParseRHS(Model model, ParserRuleContext x)
        {
            visited = new HashSet<string>();
            var rules = model.Rules.Where(r => r.lhs_rule_number == x.RuleIndex).ToList();
            if (rules.Count() != 1) throw new Exception();
            var rule = rules.First();
            Digraph<string, SymbolEdge<string>> nfa = rule.rhs;
            List<IParseTree> input = x.children != null ? x.children.ToList() : new List<IParseTree>();
            List<SymbolEdge<string>> parse = null;
            foreach (string state in nfa.StartVertices)
            {
                foreach (var t in nfa.Edges.Where(e => e.From == state))
                {
                    parse = ParseUsingStack(model, nfa, input, t);
                    //parse = ParseRecurse(model, nfa, input, t);
                    if (parse != null)
                        break;
                }
                if (parse != null)
                    break;
            }

            if (parse != null)
            {
                foreach (var e in parse)
                {
                    if (e._symbol != null)
                    {
                        var token = e._symbol.Symbol;
                        if (token_count.TryGetValue(token, out int v))
                        {
                        }
                        v++;
                        token_count[token] = v;
                    }
                }
            }
        }


        Dictionary<string, bool> has_significant_edges = new Dictionary<string, bool>();
        private List<string> Closure(Digraph<string, SymbolEdge<string>> nfa, List<string> currentStates)
        {
            HashSet<string> visited = new HashSet<string>();
            Stack<string> stack = new Stack<string>();
            foreach (var state in currentStates) { stack.Push(state); }
            List<string> close = new List<string>();
            while (stack.Count > 0)
            {
                var state = stack.Pop();
                visited.Add(state);
                if (!has_significant_edges.TryGetValue(state, out bool has))
                {
                    has_significant_edges[state] = false;
                    foreach (var t in nfa.Edges.Where(e => e.From == state))
                    {
                        if (t._symbol != null)
                        {
                            has_significant_edges[state] = true;
                            has = true;
                        }
                        else if (! visited.Contains(t.To))
                        {
                            stack.Push(t.To);
                        }
                    }
                }
                if (has) close.Add(state);
            }
            return close;
        }
    }
}