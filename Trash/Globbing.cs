namespace Trash
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Globbing
    {
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

        private List<DirectoryInfo> GetDirectory(string cwd, string expr)
        {
            DirectoryInfo di = new DirectoryInfo(cwd);
            if (!di.Exists)
                throw new Exception($"directory {cwd} does not exist.");
            var results = new List<DirectoryInfo>();
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
            if (first == ".")
            {
                return GetDirectory(cwd, rest);
            }
            else if (first == "..")
            {
                return GetDirectory(cwd + "/..", rest);
            }
            else
            {
                List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                if (first != "")
                {
                    var ex = GlobToRegex(first);
                    var regex = new Regex(ex);
                    dirs = di.GetDirectories().Where(t => regex.IsMatch(t.Name)).ToList();
                }
                else
                {
                    dirs = new List<DirectoryInfo>() { di };
                }
                if (rest != "")
                {
                    foreach (var m in dirs)
                    {
                        var res = GetDirectory(m.FullName, rest);
                        foreach (var r in res) results.Add(r);
                    }
                }
                else
                {
                    foreach (var m in dirs)
                    {
                        results.Add(m);
                    }
                }
                return results;
            }
        }

        public List<DirectoryInfo> GetDirectory(string expr)
        {
            if (expr == null)
            {
                var result = new List<FileSystemInfo>();
                var cwd = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                DirectoryInfo di = new DirectoryInfo(cwd);
                if (!di.Exists)
                    throw new Exception("directory or file does not exist.");
                return new List<DirectoryInfo>() { di };
            }
            else
            {
                if (Path.IsPathRooted(expr))
                {
                    var full_path = Path.GetFullPath(expr);
                    var root = Path.GetPathRoot(full_path);
                    var rest = full_path.Substring(root.Length);
                    return GetDirectory(root, rest);
                }
                else
                {
                    var root = Directory.GetCurrentDirectory();
                    var rest = expr;
                    return GetDirectory(root, rest);
                }
            }
        }

        private List<FileSystemInfo> Contents(string cwd, string expr)
        {
            DirectoryInfo di = new DirectoryInfo(cwd);
            if (!di.Exists)
                throw new Exception($"directory {cwd} does not exist.");
            var results = new List<FileSystemInfo>();
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
            if (first == ".")
            {
                return Contents(cwd, rest);
            }
            else if (first == "..")
            {
                return Contents(cwd + "/..", rest);
            }
            else
            {
                List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                List<FileInfo> files = new List<FileInfo>();
                if (first != "")
                {
                    var ex = GlobToRegex(first);
                    var regex = new Regex(ex);
                    dirs = di.GetDirectories().Where(t => regex.IsMatch(t.Name)).ToList();
                    files = di.GetFiles().ToList().Where(t => regex.IsMatch(t.Name)).ToList();
                }
                else
                {
                    dirs = di.GetDirectories().ToList();
                    files = di.GetFiles().ToList();
                }
                if (rest != "")
                {
                    foreach (var m in dirs)
                    {
                        var res = Contents(m.FullName, rest);
                        foreach (var r in res) results.Add(r);
                    }
                }
                else
                {
                    foreach (var m in dirs)
                    {
                        results.Add(m);
                    }
                    foreach (var f in files)
                    {
                        results.Add(f);
                    }
                }
                return results;
            }
        }

        public List<FileSystemInfo> Contents(string expr)
        {
            if (expr == null)
            {
                var result = new List<FileSystemInfo>();
                var cwd = Directory.GetCurrentDirectory();
                DirectoryInfo di = new DirectoryInfo(cwd);
                if (!di.Exists)
                    throw new Exception("directory or file does not exist.");
                var dirs = di.GetDirectories().ToList();
                foreach (var dir in dirs)
                {
                    result.Add(dir);
                }
                var files = di.GetFiles().ToList();
                foreach (var f in files)
                {
                    result.Add(f);
                }
                return result;
            }
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

        public List<FileSystemInfo> Contents(List<string> exprs)
        {
            if (exprs == null)
                return Contents((string)null);
            else
            {
                var result = new List<FileSystemInfo>();
                foreach (var expr in exprs)
                {
                    var intermediate = Contents(expr);
                    result.AddRange(intermediate);
                }
                return result;
            }
        }
    }
}
