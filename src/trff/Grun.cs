using trfirst;

namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
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


        public void Run()
        {
            Doit();
        }

        void Doit()
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
            full_path = Path.GetFullPath(full_path);
            Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
            Assembly asm = Assembly.LoadFile(full_path + "Test.dll");
            var xxxxxx = asm1.GetTypes();
            Type[] types = asm.GetTypes();
            Type type = asm.GetType("Program");
            var methods = type.GetMethods();
            MethodInfo methodInfo = type.GetMethod("SetupParse2");
            object[] parm = new object[] { "", "", true };
            DateTime before = DateTime.Now;
            var res = methodInfo.Invoke(null, parm);
            DateTime after = DateTime.Now;
            var parser = type.GetProperty("Parser").GetValue(null, new object[0]) as Antlr4.Runtime.Parser;
            var lexer = type.GetProperty("Lexer").GetValue(null, new object[0]) as Antlr4.Runtime.Lexer;
            var tokstream = type.GetProperty("TokenStream").GetValue(null, new object[0]) as ITokenStream;
            var commontokstream = tokstream as CommonTokenStream;
            var r5 = type.GetProperty("Input").GetValue(null, new object[0]);
            var n_rules = parser.RuleNames.Count();
            var vocab = parser.Vocabulary;
            var start_rule_name = type.GetProperty("StartSymbol").GetValue(null, new object[0]) as string;
            var start_rule = Array.IndexOf(parser.RuleNames, start_rule_name);

            var first = new First(parser, start_rule);
            System.Console.Out.WriteLine("FIRST:");
            for (int r = 0; r < n_rules; ++r)
            {
                Antlr4.Runtime.Misc.IntervalSet tokens = first.FIRST(r);
                System.Console.Out.Write(parser.RuleNames[r]);
                var list = new List<string>();
                foreach (int x in tokens.ToIntegerList())
                {
                    if (x == -1) list.Add("EOF");
                    else
                    {
                        string name = null;
                        var lname = vocab.GetLiteralName(x);
                        if (name == null && lname != null) name = lname;
                        var dname = vocab.GetDisplayName(x);
                        if (name == null && dname != null) name = dname;
                        var sname = vocab.GetSymbolicName(x);
                        if (name == null && sname != null) name = sname;
                        if (name == null) name = "(" + x + ")";
                        list.Add(name);
                    }
                }
                list.Sort();
                System.Console.Out.WriteLine("\t" + String.Join(" ", list));
            }

            System.Console.Out.WriteLine();
            System.Console.Out.WriteLine("FOLLOW:");
            for (int r = 0; r < n_rules; ++r)
            {
                Antlr4.Runtime.Misc.IntervalSet tokens = first.FOLLOW(r);
                System.Console.Out.Write(parser.RuleNames[r]);
                var list = new List<string>();
                foreach (int x in tokens.ToIntegerList())
                {
                    if (x == -1) list.Add("EOF");
                    else
                    {
                        string name = null;
                        var lname = vocab.GetLiteralName(x);
                        if (name == null && lname != null) name = lname;
                        var dname = vocab.GetDisplayName(x);
                        if (name == null && dname != null) name = dname;
                        var sname = vocab.GetSymbolicName(x);
                        if (name == null && sname != null) name = sname;
                        if (name == null) name = "(" + x + ")";
                        list.Add(name);
                    }
                }
                list.Sort();
                System.Console.Out.WriteLine("\t" + String.Join(" ", list));
            }

        }
    }
}
