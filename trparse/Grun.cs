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
            try
            {
                var data = new List<AntlrJson.ParsingResultSet>();
                string txt = config.Input;
                if (config.Input == null && config.Files == null)
                {
                    string lines = null;
                    for (; ; )
                    {
                        lines = System.Console.In.ReadToEnd();
                        if (lines != null && lines != "") break;
                    }
                    txt = lines;
                    Doit(txt, data);
                }
                else if (config.Input != null)
                {
                    txt = config.Input;
                    Doit(txt, data);
                }
                else if (config.Files != null)
                {
                    foreach (var file in config.Files)
                    {
                        txt = File.ReadAllText(file);
                        Doit(txt, data);
                    }
                }
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
                serializeOptions.WriteIndented = true;
                string js1 = JsonSerializer.Serialize(data.ToArray(), serializeOptions);
                System.Console.WriteLine(js1);
            }
            finally
            {
            }
        }

        void Doit(string txt, List<AntlrJson.ParsingResultSet> data)
        {
            var path = Environment.CurrentDirectory;
            path = path + Path.DirectorySeparatorChar + "Generated";
            var old = Environment.CurrentDirectory;
            var full_path = path + "\\bin\\Debug\\net5.0\\";
            //     Environment.CurrentDirectory = full_path;
            var exists = File.Exists(full_path + "Test.dll");
            Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
            Assembly asm = Assembly.LoadFile(full_path + "Test.dll");
            var xxxxxx = asm1.GetTypes();
            Type[] types = asm.GetTypes();
            Type type = asm.GetType("Program");
            var methods = type.GetMethods();
            MethodInfo methodInfo = type.GetMethod("Parse");
            object[] parm = new object[] { txt };
            DateTime before = DateTime.Now;
            var res = methodInfo.Invoke(null, parm);
            DateTime after = DateTime.Now;
            System.Console.Error.WriteLine("Time to parse: " + (after - before));
            var tree = res as IParseTree;
            var t2 = tree as ParserRuleContext;
            var m2 = type.GetProperty("Parser");
            object[] p2 = new object[0];
            var r2 = m2.GetValue(null, p2);
            var m3 = type.GetProperty("Lexer");
            object[] p3 = new object[0];
            var r3 = m3.GetValue(null, p3);
            var m4 = type.GetProperty("TokenStream");
            object[] p4 = new object[0];
            var r4 = m4.GetValue(null, p4);
            System.Console.Error.WriteLine("# tokens per sec = " + (r4 as ITokenStream).Size / (after - before).TotalSeconds);
            Environment.CurrentDirectory = old;
            var tuple = new AntlrJson.ParsingResultSet() { Text = txt, FileName = "stdin", Stream = r4 as ITokenStream, Nodes = new IParseTree[] { t2 }, Parser = r2 as Parser, Lexer = r3 as Lexer };
            data.Add(tuple);
        }

        //public void BuildIt()
        //{
        //    // Create a workspace.
        //    //var msbuild_workspace = MSBuildWorkspace.Create();

        //    //var project = msbuild_workspace.OpenProjectAsync("Test.csproj").Result;
        //    //var compilation = project.GetCompilationAsync().Result;
        //    //var assembly = compilation.Assembly;
        //    //Compilation? compilation = msbuild_project.GetCompilationAsync().Result;
        //    //IAssemblySymbol assembly = compilation.Assembly;
        //    //var file = compilation.AssemblyName;
        //}
    }
}
