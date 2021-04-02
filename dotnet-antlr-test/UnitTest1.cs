using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace dotnet_antlr_test
{
    //[TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public void TestMethod1()
        {
            // Try generating Arithmetic test.
            string d1 = null;
            try
            {
                var l = typeof(dotnet_antlr.Program).Assembly.Location;
                var dir = Path.GetTempPath();
                d1 = dir + "Foobar";
                try { Directory.Delete(d1, true); } catch { }
                Directory.CreateDirectory(d1);
                Directory.SetCurrentDirectory(d1);
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add(l);
                    //arguments.Add("-help");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                Directory.SetCurrentDirectory(d1 + "/Generated");
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add("build");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add("run");
                    arguments.Add("-input");
                    arguments.Add("1+2");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode != 0) throw new Exception("Bad result.");
                }
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add("run");
                    arguments.Add("-input");
                    arguments.Add("1+");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode == 0) throw new Exception("Bad result.");
                }
            }
            finally
            {
            }
        }

        //[TestMethod]
        public void TestMethod2()
        {
            // Try test for CSharp grammar.
            string d1 = null;
            try
            {
                var l = typeof(dotnet_antlr.Program).Assembly.Location;
                var dir = Path.GetTempPath();
                d1 = dir + "Foobar2";
                try { Directory.Delete(d1, true); } catch { }
                Directory.CreateDirectory(d1);
                Directory.SetCurrentDirectory(d1);
                System.Reflection.Assembly a = this.GetType().Assembly;
                var names = a.GetManifestResourceNames();                
                using (Stream stream = a.GetManifestResourceStream("dotnet_antlr_test.resources.CSharpParser.g4"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    var grammar = reader.ReadToEnd();
                    File.WriteAllText("CSharpParser.g4", grammar);
                }
                using (Stream stream = a.GetManifestResourceStream("dotnet_antlr_test.resources.CSharpLexer.g4"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    var grammar = reader.ReadToEnd();
                    File.WriteAllText("CSharpLexer.g4", grammar);
                }
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add(l);
                    arguments.Add("-s");
                    arguments.Add("compilation_unit");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                Directory.SetCurrentDirectory(d1 + "/Generated");
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add("build");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add("run");
                    arguments.Add("-file");
                    arguments.Add("Program.cs");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode != 0) throw new Exception("Bad result.");
                }
                using (Process process = new Process())
                {
                    List<string> arguments = new List<string>();
                    arguments.Add("run");
                    arguments.Add("-input");
                    arguments.Add("1+");
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "c:\\Program Files\\dotnet\\dotnet.exe",
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode == 0) throw new Exception("Bad result.");
                }
            }
            finally
            {
            }
        }
    }
}
