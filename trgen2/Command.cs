namespace Trash
{
    using Antlr4.StringTemplate;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    class Command
    {
        Config _config;

        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trgen2.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public int Execute(Config config)
        {
            _config = config;
            string lines = null;
            if (!(_config.File != null && _config.File != ""))
            {
                if (_config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
                }
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
                if (_config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + _config.File + "<<<");
                }
                lines = File.ReadAllText(_config.File);
            }
            var xml = string.Join("", lines);
            // Convert xml to tree.
            dynamic root = Antlr4.StringTemplate.Misc.DynamicXml.Parse(xml);
            if (_config.templates != null)
                _config.templates = Path.GetFullPath(_config.templates);
            var path = Environment.CurrentDirectory;
            var cd = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            root_directory = cd;
            GenerateSingle(root);
            return 0;
        }

        public static string version = "0.13.6";
        public List<string> all_source_files = null;
        public List<string> all_target_files = null;
        public string root_directory;
        public string suffix;
        public string ignore_string = null;
        public string source_directory;


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

        public static EnvType GetEnvType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return EnvType.Unix;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return EnvType.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return EnvType.Mac;
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


        public void GenerateSingle(object root)
        {
            source_directory = Environment.CurrentDirectory.Replace('\\', '/') + "/";
            if (source_directory != "" && !source_directory.EndsWith("/"))
            {
                source_directory = source_directory + "/";
            }
            try
            {
                // Create a directory containing target build files.
                Directory.CreateDirectory((string)_config.output_directory);
            }
            catch (Exception)
            {
                throw;
            }
            // Find all source files.
            this.all_target_files = new List<string>();
            this.all_source_files = new Domemtech.Globbing.Glob()
                    .RegexContents(this._config.all_source_pattern)
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
            GenFromTemplates(root);
            AddSource();
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
            using (Stream stream = a.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void AddSource()
        {
            var cd = Environment.CurrentDirectory + "/";
            cd = cd.Replace('\\', '/');
            var set = new HashSet<string>();
            foreach (var path in this.all_source_files)
            {
                // Construct proper starting directory based on namespace.
                var from = path;
                var f = from.Substring(cd.Length);
                // First, remove source_directory.
                f = (
                        f.StartsWith(source_directory)
                        ? f.Substring((source_directory).Length)
                        : f
                        );

                string to = null;
                if (to == null)
                {
                    to = this._config.output_directory
                        + f;
                }
                System.Console.Error.WriteLine("Copying source file from "
                  + from
                  + " to "
                  + to);
                this.all_target_files.Add(to);
                this.CopyFile(from, to);
            }
        }

        public class Params
        {
            public string AttrName { get; set; }
            public string AttrValue { get; set; }
        }

        private void GenFromTemplates(object root)
        {
            if (_config.templates == null)
            {
                System.Reflection.Assembly a = this.GetType().Assembly;
                // Load resource file that contains the names of all files in templates/ directory,
                // which were obtained by doing "cd templates/; find . -type f > files" at a Bash
                // shell.
                var orig_file_names = ReadAllResourceLines(a, "trgen2.templates.files");
                var regex_string = "^(?=.*" + _config.template + "/).*$";
                var regex = new Regex(regex_string);
                var files_to_copy = orig_file_names.Where(f =>
                {
                    if (f == "./files") return false;
                    var v = regex.IsMatch(f);
                    return v;
                }).ToList();
                var prefix_to_remove = "trgen2.templates.";
                System.Console.Error.WriteLine("Prefix to remove " + prefix_to_remove);
                var resources = a.GetManifestResourceNames();
                string param_str = ReadAllResource(a, prefix_to_remove
                    + _config.template + ".parameters.json");
                var parameters = JsonSerializer.Deserialize<List<Params>>(param_str);
                var cd = Path.GetFileName(Environment.CurrentDirectory);
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    // copy the file straight up if it doesn't begin
                    // with target directory name. Otherwise,
                    // remove the target dir name.
                    if (file.EndsWith("parameters.json"))
                    {
                        continue;
                    }
                    var to = from.StartsWith("./" + _config.template)
                        ? from.Substring(("./" + _config.template).Length + 1)
                        : from.Substring(2);
                    to = ((string)_config.output_directory).Replace('\\', '/') + to;
                    from = prefix_to_remove + from.Replace('/', '.').Substring(2);
                    to = to.Replace('\\', '/');
                    to = to.Replace("#dir_name#", cd);
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = ReadAllResource(a, from);
                    System.Console.Error.WriteLine("Rendering template file from "
                        + from
                        + " to "
                        + to);
                    Template t = new Template(content);
                    t.Add("additional_sources", all_target_files.Where(t =>
                    {
                        var ext = Path.GetExtension(t);
                        return suffix.Contains(ext);
                    })
                        .Select(t => t.Substring(_config.output_directory.Length))
                        .ToList());
                    foreach (var pair in parameters)
                    {
                        t.Add(pair.AttrName, pair.AttrValue);
                    }
                    t.Add("dir_name", cd);
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
            }
            else
            {
                var regex_string = "^(.*(files|" + _config.template + "/)).*$";
                var files_to_copy = new Domemtech.Globbing.Glob(_config.templates)
                    .RegexContents(regex_string)
                    .Where(f =>
                    {
                        if (f.Attributes.HasFlag(FileAttributes.Directory)) return false;
                        if (f is DirectoryInfo) return false;
                        return true;
                    })
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
                var prefix_to_remove = _config.templates + '/';
                prefix_to_remove = prefix_to_remove.Replace("\\", "/");
                prefix_to_remove = prefix_to_remove.Replace("//", "/");
                System.Console.Error.WriteLine("Prefix to remove " + prefix_to_remove);
                var set = new HashSet<string>();
                foreach (var file in files_to_copy)
                {
                    var from = file;
                    var e = file.Substring(prefix_to_remove.Length);
                    var to = e.StartsWith(_config.template)
                         ? e.Substring(_config.template.Length + 1)
                         : e;
                    if (e == "files") continue;
                    e = e.Substring(_config.template.Length + 1);
                    if (e == "parameters.json") continue;
                    to = ((string)_config.output_directory).Replace('\\', '/') + to;
                    var q = Path.GetDirectoryName(to).ToString().Replace('\\', '/');
                    Directory.CreateDirectory(q);
                    string content = File.ReadAllText(from);
                    System.Console.Error.WriteLine("Rendering template file from "
                        + from
                        + " to "
                        + to);
                    Template t = new Template(content);
                    t.Add("additional_sources", all_target_files.Where(t =>
                    {
                        var ext = Path.GetExtension(t);
                        return suffix.Contains(ext);
                    })
                        .Select(t => t.Substring(_config.output_directory.Length))
                        .ToList());
                    t.Add("root", root);
                    //t.Add("is_combined_grammar", p.tool_grammar_files.Count() == 1);
                    var o = t.Render();
                    File.WriteAllText(to, o);
                }
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
    }


    public class Goof : GetMemberBinder
    {
        public Goof(string name, bool ignoreCase) : base(name, ignoreCase)
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            return null;
        }
    }
}
