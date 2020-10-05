using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Document = Workspaces.Document;
using Project = Microsoft.CodeAnalysis.Project;
using Workspace = Workspaces.Workspace;

namespace Trash
{
    public class Grun
    {
        private Repl _repl;

        public Grun(Repl repl)
        {
            _repl = repl;
        }

        public List<Document> Grammars
        {
            get; set;
        }

        public List<Document> ImportGrammars
        {
            get; set;
        }

        public List<Document> SupportCode
        {
            get; set;
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

        public void ReadWorkspace(string csproj)
        {
        }

        public void CreateMsbuildWorkspace(Workspace workspace)
        {
        }

        public void Build()
        {

            // Create temporary directory.
            var path = Path.GetTempPath();
            path = path + Path.DirectorySeparatorChar + "Antlrvsix" + new Random().Next();
            var grammars = _repl._workspace.AllDocuments().Where(d => d.FullPath.EndsWith(".g4")).ToList();
            try
            {
                Directory.CreateDirectory(path);
                var old = Environment.CurrentDirectory;
                Environment.CurrentDirectory = path;
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"
<Project Sdk=""Microsoft.NET.Sdk"" >
	<PropertyGroup>
		<TargetFramework>netstandard2.0</TargetFramework>
	</PropertyGroup>
	<ItemGroup>");
                    foreach (var grammar in grammars)
                    {
                        sb.AppendLine("<Antlr4 Include=\"" + Path.GetFileName(grammar.FullPath) + "\" />");
                    }

                    sb.AppendLine(@"</ItemGroup>
	<ItemGroup>
		<PackageReference Include=""Antlr4.Runtime.Standard"" Version =""4.8.0"" />
		<PackageReference Include=""Antlr4BuildTasks"" Version = ""8.3"" />
		<PackageReference Include= ""AntlrTreeEditing"" Version = ""1.7"" />
	</ItemGroup>
	<PropertyGroup>
		<RestoreProjectStyle>PackageReference</RestoreProjectStyle>
	</PropertyGroup>
	<PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Debug|AnyCPU'"" >
		<NoWarn>1701;1702;3021</NoWarn>
	</PropertyGroup>
</Project>");
                    var fn_csproj = path + Path.DirectorySeparatorChar + "Test.csproj";
                    System.IO.File.WriteAllText(fn_csproj, sb.ToString());
                }

                // Copy all files in workspace to temporary directory.
                foreach (var doc in _repl._workspace.AllDocuments())
                {
                    var fn = Path.GetFileName(doc.FullPath);
                    var code = doc.Code;
                    System.IO.File.WriteAllText(fn, code);
                }
                // Add in Main.cs to get parse.
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(@"
namespace Easy
{
    using Antlr4.Runtime;
    using System.Text;
    using System.Runtime.CompilerServices;
    public class Program
    {
        static IParseTree Parse(string input)
        {
            var str = new AntlrInputStream(input);
            var lexer = new ");
                    var lexer_name = "";
                    var parser_name = "";
                    var lexer = grammars.Where(d => d.FullPath.EndsWith("Lexer.g4")).ToList();
                    var parser = grammars.Where(d => d.FullPath.EndsWith("Parser.g4")).ToList();
                    if (lexer.Count == 1)
                    {
                        lexer_name = Path.GetFileName(lexer.First().FullPath.Replace(".g4", ""));
                        parser_name = Path.GetFileName(parser.First().FullPath.Replace(".g4", ""));
                    }
                    else if (lexer.Count == 0)
                    {
                        // Combined.
                        var combined = grammars.Where(d => d.FullPath.EndsWith(".g4")).ToList();
                        if (combined.Count == 1)
                        {
                            lexer_name = combined.First().FullPath.Replace(".g4", "");
                            lexer_name = lexer_name + "Lexer";
                            parser_name = lexer_name + "Parser";
                        }
                    }
                    sb.Append(lexer_name);
                    sb.AppendLine(@"(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ");
                    sb.Append(parser_name);
                    sb.Append(@"(tokens);
            var tree = parser.");

                    sb.AppendLine(@"();
        }
    }
}");
                    System.IO.File.WriteAllText("Program.cs", sb.ToString());
                }
                List<string> arguments = new List<string>();
                arguments.Add("build");
                ProcessStartInfo startInfo = new ProcessStartInfo("dotnet", JoinArguments(arguments))
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
                Environment.CurrentDirectory = old;

                //Compilation? compilation = msbuild_project.GetCompilationAsync().Result;
                //IAssemblySymbol assembly = compilation.Assembly;
                //var file = compilation.AssemblyName;
                //Assembly asm = Assembly.LoadFile(file);
                //Type type = asm.GetType("Program");
                //MethodInfo methodInfo = type.GetMethod("Main");
                //methodInfo.Invoke(null, null);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        public void Run()
        {
        }

        public void BuildIt()
        {
            // Create a workspace.
            var msbuild_workspace = MSBuildWorkspace.Create();

            var project = msbuild_workspace.OpenProjectAsync("Test.csproj").Result;
            var compilation = project.GetCompilationAsync().Result;
            var assembly = compilation.Assembly;

        }
    }
}
