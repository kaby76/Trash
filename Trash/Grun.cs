using Algorithms;

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
    using System.Runtime.Loader;
    using System.Text;
    using Document = Workspaces.Document;
    using Workspace = Workspaces.Workspace;

    public class Grun
    {
        private Repl _repl;

        public Grun(Repl repl)
        {
            _repl = repl;
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

                if (argument.IndexOfAny(new[] {'"', ' '}) < 0)
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

        public void Run(Repl repl, string input)
        {
            var path = Environment.CurrentDirectory;
            path = path + Path.DirectorySeparatorChar + "Generated";
            var grammars = _repl._workspace.AllDocuments().Where(d => d.FullPath.EndsWith(".g4")).ToList();
            var old = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = path;

                //var alc = new TestAssemblyLoadContext(path + "/bin/Debug/netcoreapp3.1/Test.dll");
                //Assembly asm = alc.LoadFromAssemblyPath(path + "/bin/Debug/netcoreapp3.1/Test.dll");
                Assembly asm = Assembly.LoadFile(path + "/bin/Debug/netcoreapp3.1/Test.dll");

                Type[] types = asm.GetTypes();
                Type type = asm.GetType("Easy.Program");
                var methods = type.GetMethods();
                MethodInfo methodInfo = type.GetMethod("Parse");
                string txt = input;
                if (input == null)
                {
                    var tuple = repl.input_output_stack.Pop();
                    txt = tuple.Item4;
                }
                object[] parm = new object[] {txt};
                var res = methodInfo.Invoke(null, parm);
                var tree = res as IParseTree;
                var t2 = tree as ParserRuleContext;

                var m2 = type.GetProperty("Parser");
                object[] p2 = new object[0];
                var r2 = m2.GetValue(null, p2);

                // return the tree.
                Environment.CurrentDirectory = old;
                repl.input_output_stack.Push(
                    new MyTuple<IParseTree[], Parser, Workspaces.Document, string>(
                        new IParseTree[] { t2 },
                        (Parser) r2,
                        null,
                        null));

            }
            finally
            {
                Environment.CurrentDirectory = old;
            }
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
