namespace Trash
{
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

    public class Generate
    {
        private Repl _repl;

        public Generate(Repl repl)
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

        public void Run(Repl repl, string[] arguments)
        {
            var path = Environment.CurrentDirectory;
            var generated_path = path + Path.DirectorySeparatorChar + "Generated";
            var grammars = _repl._workspace.AllDocuments().Where(d => d.FullPath.EndsWith(".g4")).ToList();
            var old = Environment.CurrentDirectory;
            try
            {
                string @namespace = null;
                string start = null;
                // Parse args.
                int i = 0;
                for (; i < arguments.Length; ++i)
                {
                    var arg = arguments[i];
                    if (arg == "-package")
                    {
                        @namespace = arguments[i + 1];
                        ++i;
                    }
                    else if (arg == "-start")
                    {
                        start = arguments[i + 1];
                        ++i;
                    }
                    else if (!arg.StartsWith("-"))
                        break;
                }
                if (start == null)
                    start = arguments[i];

                try
                {
                    // Create a directory containing a C# project with grammars.
                    Directory.CreateDirectory(generated_path);
                }
                catch (Exception)
                {
                    throw;
                }

                // Create driver from dotnet and antlr template.
                {
                    List<string> args = new List<string>();
                    args.Add("new");
                    args.Add("antlr");
                    foreach (var grammar in grammars)
                    {
                        args.Add("-g");
                        args.Add("\"" + Path.GetFileName(grammar.FullPath) + "\"");
                    }
                    if (@namespace != null)
                    {
                        args.Add("-n");
                        args.Add("\"" + @namespace + "\"");
                    }
                    args.Add("-s");
                    args.Add(start);
                    ProcessStartInfo startInfo = new ProcessStartInfo("dotnet-antlr", JoinArguments(args))
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        //RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.ErrorDataReceived += HandleOutputDataReceived;
                    process.OutputDataReceived += HandleOutputDataReceived;
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    var success = process.ExitCode == 0;
                    if (!success) throw new Exception("Build failed.");
                }

                {
                    Environment.CurrentDirectory = generated_path;
                    List<string> args = new List<string>();
                    args.Add("build");
                    ProcessStartInfo startInfo = new ProcessStartInfo("dotnet", JoinArguments(args))
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        //RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.ErrorDataReceived += HandleOutputDataReceived;
                    process.OutputDataReceived += HandleOutputDataReceived;
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    var success = process.ExitCode == 0;
                    if (!success) throw new Exception("Build failed.");
                }
            }
            finally
            {
                Environment.CurrentDirectory = old;
            }
        }
    }

    public class TestAssemblyLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public TestAssemblyLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
        }

        protected override Assembly Load(AssemblyName name)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}
