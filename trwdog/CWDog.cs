namespace Trash
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    class CWDog
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trwdog.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }

        public void Execute(Config config, string[] command)
        {
            int secs = (int)config.Timeout;
            int delay = secs * 1000;

            var t = new Thread(delegate ()
            {
                int exit_code = 0;
                string cmd = command[0];
                string rest = string.Join(" ", command.Skip(1).Select(a=> '"' + a + '"'));
                try
                {
                    if (cmd.EndsWith(".bat") && !Path.IsPathFullyQualified(cmd))
                    {
                        var p = FindExePath(cmd);
                        if (p != null && p != "") cmd = p;
                    }
                    var processInfo = new ProcessStartInfo(cmd, rest);
                    //System.Console.WriteLine("Cwd " + Directory.GetCurrentDirectory());
                    //System.Console.WriteLine("Starting " + cmd + " " + rest);
                    //processInfo.CreateNoWindow = true;
                    //processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //processInfo.UseShellExecute = true;
                    //processInfo.RedirectStandardError = true;
                    //processInfo.RedirectStandardOutput = true;
                    using (Process process = Process.Start(processInfo))
                    {
                        if (process == null)
                        {
                            System.Console.Error.WriteLine("Cannot start process--Process.Start() returned null.");
                            exit_code = 1;
                            System.Environment.Exit(exit_code);
                        }
                        //System.Console.WriteLine("started");
                        process.EnableRaisingEvents = true;
                        //process.BeginOutputReadLine();
                        var has_exited = process.WaitForExit(delay);
                        if (has_exited)
                        {
                            exit_code = process.ExitCode;
                        }
                        else
                        {
                            System.Console.Error.WriteLine("Process is taking longer than " + secs + " seconds. Killing process.");
                            process.Kill(true);
                            exit_code = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine(ex.Message);
                    exit_code = 1;
                }
                //System.Console.WriteLine("Finished Post Process");
                System.Environment.Exit(exit_code);
            });
            t.Start();
        }
    }
}
