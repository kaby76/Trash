using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Trash
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Read();
            var cwd = Directory.GetCurrentDirectory();
            var results = new Dictionary<string, string>();
            LanguageServer.Antlr3Import.Try("console.g", input, ref results);
            System.Console.Write(results.First().Value);
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
}
