using System.Diagnostics;

namespace Trash
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Antlr4.StringTemplate;
    using AntlrJson;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using ParseTreeEditing.UnvParseTreeDOM;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trgen.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public int Execute(Config config)
        {
            GenerateViaConfig(config);
            if (failed_modules.Any())
            {
                System.Console.WriteLine(String.Join(" ", failed_modules));
                return 1;
            }
            return 0;
        }


        public static string version = "0.22.0";

        // For maven-generated code.
        public List<string> failed_modules = new List<string>();

        public string Suffix(string target)
        {
            return target switch
            {
                "Antlr4cs" => ".cs",
                "Cpp" => ".cpp",
                "CSharp" => ".cs",
                "Dart" => ".dart",
                "Go" => ".go",
                "Java" => ".java",
                "JavaScript" => ".js",
                "PHP" => ".php",
                "Python2" => ".py",
                "Python3" => ".py",
                "Swift" => ".swift",
                "TypeScript" => ".ts",
                "Antlr4ng" => ".ts",
                _ => throw new NotImplementedException(),
            };
        }

        public string ignore_list_of_files = ".trgen-ignore";

        public static LineTranslationType GetLineTranslationType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LineTranslationType.LF;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LineTranslationType.CRLF;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LineTranslationType.CR;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static string GetOSTarget()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSX";
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static PathSepType GetPathSep()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return PathSepType.Colon;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PathSepType.Semi;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return PathSepType.Colon;
            }
            throw new Exception("Cannot determine operating system!");
        }

        public static string GetAntlrToolPath()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return (home + "/.m2/antlr4-4.13.1-complete.jar").Replace('\\', '/');
        }

        public static string TargetName(string target)
        {
            return target;
            //return target switch
            //{
            //    "Antlr4cs" => "Antlr4cs",
            //    "Cpp" => "Cpp",
            //    "CSharp" => "CSharp",
            //    "Dart" => "Dart",
            //    "Go" => "Go",
            //    "Java" => "Java",
            //    "JavaScript" => "JavaScript",
            //    "PHP" => "PHP",
            //    "Python2" => "Python2",
            //    "Python3" => "Python3",
            //    "Swift" => "Swift",
            //    "TypeScript" => "TypeScript",
            //    _ => throw new NotImplementedException(),
            //};
        }

        public static string AllButTargetName(string target)
        {
            var all_but = new List<string>() {
                "Antlr4cs",
                "Antlr4ng",
                "Cpp",
                "CSharp",
                "Dart",
                "Go",
                "Java",
                "JavaScript",
                "PHP",
                "Python3",
                "Swift",
                "TypeScript",
            };
            var filter = String.Join("/|", all_but.Where(t => t != TargetName(target)));
            return filter;
        }

        static string Dirname(string path)
        {
            // Split the path into parts separated by '/'
            string[] parts = path.Split('/');

            // If there is only one part, return "."
            if (parts.Length == 1)
            {
                return ".";
            }

            // Remove the last part and join the remaining parts with '/'
            return string.Join("/", parts.Take(parts.Length - 1));
        }

        // Returns the base name (i.e., the file name) of a given path
        static string Basename(string path)
        {
            // Split the path into parts separated by '/'
            string[] parts = path.Split('/');

            // Return the last part
            return parts.LastOrDefault();
        }

        public void GenerateViaConfig(Config config)
        {
            // Run trgen -c CSharp first.
            Process p = new Process();
            p.StartInfo.FileName = "trgen";
            p.StartInfo.WorkingDirectory = ".";
            p.StartInfo.Arguments = "-t CSharp -o Generated-vsc/parser/";
            p.Start();
            p.WaitForExit();

            // cd to generated directory.
            var output_dir = "Generated-vsc/";
            var cwd = System.Environment.CurrentDirectory.Replace("\\", "/");
            if (cwd != "" && !cwd.EndsWith("/")) cwd = cwd + "/";
            System.Environment.CurrentDirectory = output_dir;
            GenFromTemplates(config);
        }

        IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        string[] ReadAllResourceLines(System.Reflection.Assembly a, string resourceName)
        {
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return EnumerateLines(reader).ToArray();
            }
        }

        string ReadAllResource(System.Reflection.Assembly a, string resourceName)
        {
            var names = a.GetManifestResourceNames();
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        byte[] ReadBytesResource(System.Reflection.Assembly a, string resourceName)
        {
            var names = a.GetManifestResourceNames();
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return reader.ReadAllBytes();
            }
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
     
        private void GenFromTemplates(Config config)
        {
            List<string> template_directory_files_to_copy;
            ZipArchive za = null;
            string prefix_to_remove = "";

            System.Reflection.Assembly a = this.GetType().Assembly;
            // Load resource file that contains the names of all files in templates/ directory,
            // which were obtained by doing "cd templates/; find . -type f > files" at a Bash
            // shell.
            var zip = ReadBytesResource(a, "trgenvsc.foobar.zip");
            MemoryStream stream = new MemoryStream(zip);
            var regex_string = "^.*$";
            var regex = new Regex(regex_string);

            za = new ZipArchive(stream);

            template_directory_files_to_copy = za.Entries.Where(f =>
            {
                var fn = f.FullName;
                var v = regex.IsMatch(fn);
                return v;
            }).Select(f => f.FullName).ToList();

            var set = new HashSet<string>();
            foreach (var file in template_directory_files_to_copy)
            {
                var from = file;
                var base_name = Basename(from);
                var dir_name = Dirname(from);
                if (dir_name == from || dir_name + "/" == from)
                    continue;
                Template t;

                //if (config.template_sources_directory == null)
                //{
                //    var e = file.Substring(prefix_to_remove.Length);
                //    var to = FixedTemplatedFileName(e, config, test);
                //}

                string to = FixedTemplatedFileName(from, config);
                var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');

                Directory.CreateDirectory(q);
                string content = za.Entries
                    .Where(x => x.FullName == from)
                    .Select(x =>
                {
                    using (var r = new StreamReader(x.Open()))
                    {
                        var ss = r.ReadToEnd();
                        return ss;
                    }
                }).FirstOrDefault();
                System.Console.Error.WriteLine("Rendering template file from "
                                               + from
                                               + " to "
                                               + to);

                // There are three types of files in template area:
                // StringTemplateGroup, StringTemplate, and Plain.
                if (base_name.StartsWith("stg-"))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("stg-".Length);
                    TemplateGroupString tg = new TemplateGroupString(content);
                    t = tg.GetInstanceOf("start");
                }
                else if (base_name.StartsWith("stg."))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("stg.".Length);
                    TemplateGroupString tg = new TemplateGroupString(content);
                    t = tg.GetInstanceOf("start");
                }
                else if (base_name.EndsWith(".stg"))
                {
                    to = to.Substring(0, to.Length - ".stg".Length);
                    TemplateGroupString tg = new TemplateGroupString(content);
                    t = tg.GetInstanceOf("start");
                }
                else if (base_name.StartsWith("st-"))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("st-".Length);
                    t = new Template(content);
                }
                else if (base_name.StartsWith("st."))
                {
                    to = Dirname(to) + "/" + Basename(to).Substring("st.".Length);
                    t = new Template(content);
                }
                else if (base_name.EndsWith(".st"))
                {
                    to = to.Substring(0, to.Length - ".st".Length);
                    t = new Template(content);
                }
                else
                {
                    // Copy as is.
                    File.WriteAllText(to, content);
                    continue;
                }

                string output_dir = to;
                for (; ; )
                {
                    var d = Dirname(output_dir);
                    if (d != null && d != "" && d != ".")
                    {
                        output_dir = d;
                    }
                    else break;
                }

                output_dir = output_dir + "/";
                //t.Add("test", test);
                var o = t.Render();
                File.WriteAllText(to, o);
            }
        }

        static string RemoveTrailingSlash(string str)
        {
            for (; ; )
            {
                if (str.EndsWith('/'))
                    str = str.Substring(0, str.Length - 1);
                else if (str.EndsWith('\\'))
                    str = str.Substring(0, str.Length - 1);
                else break;
            }
            return str;
        }

        static string Cap(string str)
        {
            if (str.Length == 0)
                return str;
            else if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            else
                return char.ToUpper(str[0]) + str.Substring(1);
        }

        public void CopyFile(string from, string to)
        {
            from = from.Replace('\\', '/');
            to = to.Replace('\\', '/');
            var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
            Directory.CreateDirectory(q);
            File.Copy(from, to, true);
        }

        public static string Localize(LineTranslationType encoding, string code)
        {
            var is_win = code.Contains("\r\n");
            var is_mac = code.Contains("\n\r");
            var is_uni = code.Contains("\n") && !(is_win || is_mac);
            if (encoding == LineTranslationType.CRLF)
            {
                if (is_win) return code;
                else if (is_mac) return code.Replace("\n\r", "\r\n");
                else if (is_uni) return code.Replace("\n", "\r\n");
                else return code;
            }
            if (encoding == LineTranslationType.CR)
            {
                if (is_win) return code.Replace("\r\n", "\n\r");
                else if (is_mac) return code;
                else if (is_uni) return code.Replace("\n", "\n\r");
                else return code;
            }
            if (encoding == LineTranslationType.LF)
            {
                if (is_win) return code.Replace("\r\n", "\n");
                else if (is_mac) return code.Replace("\n\r", "\n");
                else if (is_uni) return code;
                else return code;
            }
            return code;
        }

        string FixedName(string from, Config config)
        {
            string to = null;
            var cwd = System.Environment.CurrentDirectory.Replace("\\", "/");
            if (cwd != "" && !cwd.EndsWith("/")) cwd = cwd + "/";

            if (from.StartsWith(cwd))
                from = from.Substring(cwd.Length);

            if (from.StartsWith("CSharp" + "/"))
                from = from.Substring("CSharp".Length+1);

            // Split the dirname and basename of the path x.
            var dir = Dirname(from);
            if (dir == ".") dir = "";
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";
            if (dir.StartsWith(cwd)) dir = dir.Substring(cwd.Length);
            if (dir.StartsWith("CSharp" + "/")) dir = dir.Substring("CSharp".Length + 1);
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";

            var bn = Basename(from);

            to = dir + bn;

            to = "./"
                 + to;
            to = to.Replace('\\', '/');
            return to;
        }

        string FixedTemplatedFileName(string from, Config config)
        {
            string to = null;
            var cwd = System.Environment.CurrentDirectory.Replace("\\", "/");
            if (cwd != "" && !cwd.EndsWith("/")) cwd = cwd + "/";

            // Split the dirname and basename of the path x.
            var dir = Dirname(from);
            if (dir == ".") dir = "";
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";
            if (dir.StartsWith(cwd)) dir = dir.Substring(cwd.Length);
            if (dir != "" && !dir.EndsWith("/")) dir = dir + "/";

            var bn = Basename(from);

            to = dir + bn;

            to = "./"
                 + to;
            to = to.Replace('\\', '/');
            return to;
        }
    }

    public static class BinaryReaderExtensions
    {

        public static byte[] ReadBytesToEnd(this BinaryReader binaryReader)
        {

            var length = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
            return binaryReader.ReadBytes((int)length);
        }

        public static byte[] ReadAllBytes(this BinaryReader binaryReader)
        {

            binaryReader.BaseStream.Position = 0;
            return binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
        }

        public static byte[] ReadBytes(this BinaryReader binaryReader, Range range)
        {

            var (offset, length) = range.GetOffsetAndLength((int)binaryReader.BaseStream.Length);
            binaryReader.BaseStream.Position = offset;
            return binaryReader.ReadBytes(length);
        }
    }
}
