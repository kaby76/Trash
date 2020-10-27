namespace Trash.Commands
{
    using GlobExpressions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.Extensions.FileSystemGlobbing;
    using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design.Serialization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Text.RegularExpressions;

    class CLs
    {
        public void Help()
        {
            System.Console.WriteLine(@"ls <string>?
List directory contents. If string is not given, list the current directory contents.
ls accepts wildcards.

Example:
    ls
    ls foobar*
");
        }

        private List<string> Dirs(string cwd, string pat)
        {
            throw new Exception();
        }

        private (string, string) CarCdr(string path)
        {
            throw new Exception();
        }

        private static readonly HashSet<char> RegexSpecialChars = new HashSet<char>(new[] { '[', '\\', '^', '$', '.', '|', '?', '*', '+', '(', ')' });

        private static string GlobToRegex(string glob)
        {
            var regex = new StringBuilder();
            var characterClass = false;
            regex.Append("^");
            foreach (var c in glob)
            {
                if (characterClass)
                {
                    if (c == ']') characterClass = false;
                    regex.Append(c);
                    continue;
                }
                switch (c)
                {
                    case '*':
                        regex.Append(".*");
                        break;
                    case '?':
                        regex.Append(".");
                        break;
                    case '[':
                        characterClass = true;
                        regex.Append(c);
                        break;
                    default:
                        if (RegexSpecialChars.Contains(c)) regex.Append('\\');
                        regex.Append(c);
                        break;
                }
            }
            regex.Append("$");
            return regex.ToString();
        }

        private List<string> Contents(string cwd, string expr)
        {
            DirectoryInfo di = new DirectoryInfo(cwd);
            if (!di.Exists)
                throw new Exception($"directory {cwd} does not exist.");
            var results = new List<string>();
            // Find first non-embedded file sep char.
            int j;
            for (j = 0; j < expr.Length; ++j)
            {
                if (expr[j] == '[')
                {
                    ++j;
                    for (; j < expr.Length; ++j)
                        if (expr[j] == '\\') ++j;
                        else if (expr[j] == ']') break;
                }
                else if (expr[j] == '/') break;
                else if (expr[j] == '\\') break;
            }
            string first = "";
            string rest = "";
            if (expr != "")
            {
                first = expr.Substring(0, j);
                if (j == expr.Length) rest = "";
                else rest = expr.Substring(j + 1, expr.Length - j - 1);
            }
            if (rest == "")
            {
                var ex = GlobToRegex(first);
                var regex = new Regex(ex);
                var dirs = di.GetDirectories().ToList().Select(i => i.Name).ToList();
                foreach (var m in dirs.Where(t => regex.IsMatch(t)).ToList())
                {
                    results.Add(m);
                }
                var files = di.GetFiles().ToList().Select(i => i.Name).ToList();
                foreach (var f in files.Where(t => regex.IsMatch(t)).ToList())
                {
                    results.Add(f);
                }
            }
            else
            {
                var ex = GlobToRegex(first);
                var regex = new Regex(ex);
                var dirs = di.GetDirectories().ToList().Select(i => i.Name).ToList();
                var matches = dirs.Where(t => regex.IsMatch(t)).ToList();
                foreach (var m in matches)
                {
                    var res = Contents(m, rest);
                    foreach (var r in res) results.Add(r);
                }
            }
            return results;
        }

        public List<string> Contents(string expr)
        {
            if (Path.IsPathRooted(expr))
            {
                var full_path = Path.GetFullPath(expr);
                var root = Path.GetPathRoot(full_path);
                var rest = full_path.Substring(root.Length);
                return Contents(root, rest);
            }
            else
            {
                var root = Directory.GetCurrentDirectory();
                var rest = expr;
                return Contents(root, rest);
            }
        }

        public void Execute(Repl repl, ReplParser.LsContext tree)
        {
            var expr = repl.GetArg(tree.arg());
            if (expr != null)
            {
                var list = Contents(expr);
                //DirectoryInfoWrapper patternSearchFolder;
                //bool is_absolute = Path.IsPathRooted(expr);
                //if (is_absolute)
                //{
                //    var root = Path.GetPathRoot(expr);
                //    patternSearchFolder = new DirectoryInfoWrapper(new DirectoryInfo(root));
                //}
                //else
                //{
                //    var cwd = Directory.GetCurrentDirectory();
                //    patternSearchFolder = new DirectoryInfoWrapper(new DirectoryInfo(cwd));
                //}

                //var matcher = new Microsoft.Extensions.FileSystemGlobbing.Matcher(StringComparison.OrdinalIgnoreCase);
                //matcher.AddInclude(expr);
                //var results = matcher.Execute(patternSearchFolder);
                //if (!results.HasMatches)
                //    throw new Exception("directory or file does not exist.");

                //var files = results.Files;
                //foreach (var f in files)
                //{
                //    var e = f.Path;
                //    Console.WriteLine($"{e}");
                //}
                foreach (var f in list)
                {
                    Console.WriteLine($"{f}");
                }

            }
            else
            {
                var cwd = Directory.GetCurrentDirectory();
                DirectoryInfo di = new DirectoryInfo(cwd);
                if (!di.Exists)
                    throw new Exception("directory or file does not exist.");

                var dirs = di.GetDirectories().ToList().Select(i => i.Name).ToList();
                foreach (var dir in dirs)
                {
                    Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}/");
                }
                if (expr == null) expr = "*";
                var files = di.GetFiles(expr).ToList().Select(i => i.Name).ToList();
                foreach (var f in files)
                {
                    Console.WriteLine($"{f.Substring(f.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                }
            }
        }
    }
}
