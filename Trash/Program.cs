using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LanguageServer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Trash
{
    class Program
    {
        static void Main(string[] args)
        {
            var repl = new Repl();
            repl.Run();
        }

        static string Read()
        {
            string stdin = null;
            if (Console.IsInputRedirected)
            {
                using (Stream stream = Console.OpenStandardInput())
                {
                    byte[] buffer = new byte[1000];  // Use whatever size you want
                    StringBuilder builder = new StringBuilder();
                    int read = -1;
                    while (true)
                    {
                        AutoResetEvent gotInput = new AutoResetEvent(false);
                        Thread inputThread = new Thread(() =>
                        {
                            try
                            {
                                read = stream.Read(buffer, 0, buffer.Length);
                                gotInput.Set();
                            }
                            catch (ThreadAbortException)
                            {
                                Thread.ResetAbort();
                            }
                        })
                        {
                            IsBackground = true
                        };

                        inputThread.Start();

                        // Timeout expired?
                        if (!gotInput.WaitOne(100))
                        {
                            inputThread.Abort();
                            break;
                        }

                        // End of stream?
                        if (read == 0)
                        {
                            stdin = builder.ToString();
                            break;
                        }

                        // Got data
                        builder.Append(Console.InputEncoding.GetString(buffer, 0, read));
                    }
                    return builder.ToString();
                }
            }
            return "";
        }
    }

    class Repl
    {
        public Repl()
        {
        }

        public void Run()
        {
            string input;
            do
            {
                Console.Write("> ");
                input = Console.ReadLine() + ";";
            } while (Execute(input));
        }

        public bool Execute(string input)
        {
            try
            {
                var str = new AntlrInputStream(input);
                var lexer = new ReplLexer(str);
                var tokens = new CommonTokenStream(lexer);
                var parser = new ReplParser(tokens);
                var tree = parser.cmd();
                if (tree.read() != null)
                {
                    var r = tree.read();
                    var f = r.ffn().GetText();
                    f = f.Substring(1, f.Length - 2);
                    var s1 = new AntlrFileStream(f.Substring(1, f.Length - 2));
                    var l1 = new ANTLRv4Lexer(s1);
                    var t1 = new CommonTokenStream(l1);
                    var p1 = new ANTLRv4Parser(t1);
                    var tr = p1.grammarSpec();
                }
                else if (tree.import_() != null)
                {
                    var import = tree.import_();
                    var type = import.type()?.GetText();
                    var f = import.ffn().GetText();
                    f = f.Substring(1, f.Length - 2);
                    if (type == "antlr3")
                    {
                        var ii = System.IO.File.ReadAllText(f);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        LanguageServer.Antlr3Import.Try(f, ii, ref res);
                        System.Console.Write(res.First().Value);
                    }
                }
                else if (tree.quit() != null)
                {
                    return false;
                }    
            }
            catch
            {
                System.Console.WriteLine("Err");
            }
            return true;
        }
    }

}
