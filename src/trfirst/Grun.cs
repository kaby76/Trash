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
    using Document = Workspaces.Document;
    using Workspace = Workspaces.Workspace;

    public class Grun
    {
        Config config;

        public Grun(Config co)
        {
            config = co;
        }

        public List<Document> Grammars { get; set; }

        public List<Document> ImportGrammars { get; set; }

        public List<Document> SupportCode { get; set; }

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

        public void ReadWorkspace(string csproj)
        {
        }

        public void CreateMsbuildWorkspace(Workspace workspace)
        {
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
            var full_path = path + "Generated/bin/Debug/net6.0/";
            var exists = File.Exists(full_path + "Test.dll");
            if (!exists) full_path = path + "bin/Debug/net6.0/";
            full_path = Path.GetFullPath(full_path);
            Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
            Assembly asm = Assembly.LoadFile(full_path + "Test.dll");
            var xxxxxx = asm1.GetTypes();
            Type[] types = asm.GetTypes();
            Type type = asm.GetType("Program");
            var methods = type.GetMethods();
            MethodInfo methodInfo = type.GetMethod("SetupParse2");
            object[] parm = new object[] { "" };
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
            for (int r = 0; r < n_rules; ++r)
            {
                var ss = parser.Atn.ruleToStartState[r];
                parser.State = ss.stateNumber;
                parser.Context = null;
                Antlr4.Runtime.Misc.IntervalSet tokens = parser.GetExpectedTokens();
                System.Console.Out.Write(parser.RuleNames[r]);
                var list = new List<string>();
                foreach (int x in tokens.ToIntegerList())
                {
                    if (x == -1) list.Add("EOF");
                    else
                    {
                        var name = vocab.GetSymbolicName(x);
                        var lname = vocab.GetLiteralName(x);
                        var dname = vocab.GetDisplayName(x);
                        if (name == null && lname != null) name = lname;
                        if (name == null && dname != null) name = dname;
                        if (name == null) name = "(" + x + ")";
                        list.Add(name);
                    }
                }
                list.Sort();
                System.Console.Out.WriteLine(" " + String.Join(" ", list));
            }
        }
    }
}
