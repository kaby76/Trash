﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrashGlobbing
{
    public class Glob
    {
        private static readonly HashSet<char> RegexSpecialChars = new HashSet<char>(new[] { '[', '\\', '^', '$', '.', '|', '?', '*', '+', '(', ')' });
        private string _current_directory;

        public Glob()
        {
            _current_directory = Environment.CurrentDirectory.Replace('\\', '/');
        }

        public Glob(string dir)
        {
            _current_directory = dir;
        }

        public static string GlobToRegex(string glob)
        {
            var regex = new StringBuilder();
            var characterClass = false;
            regex.Append("^");
            int ptr = 0;
            for ( ; ptr < glob.Length; ++ptr)
            {
                var c = glob[ptr];
                if (characterClass)
                {
                    if (c == ']') characterClass = false;
                    regex.Append(c);
                    continue;
                }
                switch (c)
                {
                    case '*':
                        if (glob.Length > ptr + 1 && glob[ptr + 1] == '*')
                        {
                            if (glob.Length > ptr + 2 && glob[ptr + 2] == '/')
                            {
                                // For '**/*.lua', we allow also ./*.lua as well as
                                // ./..../*.lua.
                                // Skip past '**/'
                                ptr += 2;
                                regex.Append(@".*");
                            }
                            else
                            {
                                // Skip past '**'
                                ptr += 1;
                                regex.Append(@".*");
                            }
                        }
                        else
                            regex.Append(@"[^/\\]*");
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
                    var root = _current_directory;
                    var rest = expr;
                    return GetDirectory(root, rest);
                }
            }
        }

        private List<FileSystemInfo> Closure(bool recursive)
        {
            var cwd = _current_directory;
            DirectoryInfo di = new DirectoryInfo(cwd);
            if (!di.Exists)
                throw new Exception("Directory " + cwd + " does not exist.");
            var p = System.IO.Path.GetFullPath(di.FullName);
            return Closure(p, recursive);
        }

        private List<FileSystemInfo> Closure(string expr, bool recursive)
        {
            var result = new List<FileSystemInfo>();
            var stack = new Stack<FileSystemInfo>();
            try
            {
                FileInfo f2 = new FileInfo(expr);
                if (f2.Exists) stack.Push(f2);
            }
            catch { }
            try
            {
                DirectoryInfo d2 = new DirectoryInfo(expr);
                if (d2.Exists) stack.Push(d2);
            }
            catch { }
            while (stack.Any())
            {
                var fsi = stack.Pop();
                result.Add(fsi);
                if (fsi is FileInfo fi)
                {
                }
                else if (fsi is DirectoryInfo di)
                {
                    if (recursive)
                    {
                        foreach (var i in di.GetDirectories())
                        {
                            stack.Push(i);
                        }
                    }
                    foreach (var i in di.GetFiles())
                    {
                        stack.Push(i);
                    }
                }
            }
            return result;
        }

        // Whole new Regex pattern matching of files and directories.
        public List<FileSystemInfo> RegexContents(string expr = ".*", bool recursive = true)
        {
            var result = new List<FileSystemInfo>();
            if (expr == null)
                throw new Exception("Regex expression cannot be null.");
            // The expr must be a pattern for an absolute file name.
            var closure = Closure(recursive);
            foreach (FileSystemInfo i in closure)
            {
                var regex = new PathRegex(expr);
                if (regex.IsMatch(i))
                    result.Add(i);
            }
            return result;
        }

        public List<FileSystemInfo> GlobContents(DirectoryInfo cd, string glob, bool recursive = false)
        {
            var result = new List<FileSystemInfo>();
            // Get current files and directories for directory "cd".
            // Peel off pattern to next slash.
            var index_slash = glob.IndexOf('/');
            string pattern;
            string rest;
            if (index_slash >= 0)
            {
                var expr = glob.Substring(0, index_slash);
                rest = glob.Substring(glob.IndexOf('/') + 1);
                if (expr == "..")
                {
                    var where_to = cd.ToString().Replace('\\', '/');
                    if (!where_to.EndsWith('/')) where_to = where_to + '/';
                    where_to = where_to + expr;
                    var new_cd = new DirectoryInfo(where_to).FullName.Replace('\\', '/');
                    var new_cd_str = new DirectoryInfo(new_cd);
                    return this.GlobContents(new_cd_str, rest, recursive);
                } else if (expr == ".")
                {
                    var where_to = cd.ToString().Replace('\\', '/');
                    if (!where_to.EndsWith('/')) where_to = where_to + '/';
                    var new_cd = new DirectoryInfo(where_to).FullName.Replace('\\', '/');
                    var new_cd_str = new DirectoryInfo(new_cd);
                    return this.GlobContents(new_cd_str, rest, recursive);
                } else if (expr == "**")
                {
                    foreach (DirectoryInfo i in cd.GetDirectories())
                    {
                        try
                        {
                            if (!i.Exists) continue;
                            if (rest == "")
                            {
                                result.Add(i);
                            }
                            else
                            {
                                var more = GlobContents(i, glob, recursive);
                                foreach (var m in more) result.Add(m);
                            }
                        }
                        catch { }
                    }
                    var regex = new PathRegex(Glob.GlobToRegex(rest));
                    foreach (FileInfo i in cd.GetFiles())
                    {
                        try
                        {
                            if (!i.Exists) continue;
                            if (!regex.IsMatch(i.Name)) continue;
                            result.Add(i);
                        }
                        catch
                        {
                        }
                    }
                    return result;
                }
                else
                {
                    pattern = Glob.GlobToRegex(expr);
                }
            }
            else /* index_slash < 0 */
            {
                rest = "";
                var expr = glob;
                if (expr == "..")
                {
                    var where_to = cd.ToString().Replace('\\', '/');
                    if (!where_to.EndsWith('/')) where_to = where_to + '/';
                    where_to = where_to + expr;
                    var new_cd = new DirectoryInfo(where_to).FullName.Replace('\\', '/');
                    var new_cd_str = new DirectoryInfo(new_cd);
                    return this.GlobContents(new_cd_str, rest, recursive);
                }
                else if (expr == ".")
                {
                    var where_to = cd.ToString().Replace('\\', '/');
                    if (!where_to.EndsWith('/')) where_to = where_to + '/';
                    var new_cd = new DirectoryInfo(where_to).FullName.Replace('\\','/');
                    var new_cd_str = new DirectoryInfo(new_cd);
                    return this.GlobContents(new_cd_str, rest, recursive);
                }
                else if (expr == "**")
                {
                    foreach (DirectoryInfo i in cd.GetDirectories())
                    {
                        try
                        {
                            if (!i.Exists) continue; 
                            result.Add(i);
                            var more = GlobContents(i, glob, recursive);
                            foreach (var m in more) result.Add(m);
                        }
                        catch { }
                    }
                    foreach (FileInfo i in cd.GetFiles())
                    {
                        try
                        {
                            if (!i.Exists) continue;
                            result.Add(i);
                        }
                        catch
                        {
                        }
                    }
                    return result;
                } else if (expr == "")
                {
                    pattern = ".*";
                    result.Add(cd);
                }
                else
                {
                    pattern = Glob.GlobToRegex(glob);
                }
            }
            {
                var regex = new PathRegex(pattern);
                foreach (DirectoryInfo i in cd.GetDirectories())
                {
                    try
                    {
                        if (!i.Exists) continue;
                        if (!regex.IsMatch(i.Name)) continue;
                        if (rest == "")
                        {
                            result.Add(i);
                        }
                        if (recursive)
                        {
                            var more = GlobContents(i, rest, recursive);
                            foreach (var m in more) result.Add(m);
                        }
                    }
                    catch
                    {
                    }
                }
                foreach (FileInfo i in cd.GetFiles())
                {
                    try
                    {
                        if (!i.Exists) continue;
                        if (!regex.IsMatch(i.Name)) continue;
                        result.Add(i);
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }

        private bool match(FileSystemInfo d2, string pattern)
        {
            throw new NotImplementedException();
        }
    }

    public static class HelperGlobbing
    {
        public static List<FileSystemInfo> RegexAgain(this List<FileSystemInfo> old, string expr)
        {
            var result = new List<FileSystemInfo>();
            foreach (var i in old)
            {
                var regex = new PathRegex(expr);
                if (regex.IsMatch(i))
                    result.Add(i);
            }
            return result;
        }
    }

    class PathRegex
    {
        string expr;
        Regex re;

        public PathRegex(string e)
        {
            expr = e;
            re = new Regex(expr);
        }

        public bool IsMatch(FileSystemInfo fsi)
        {
            if (fsi is DirectoryInfo)
            {
                // There are two ways to test this, one
                // with trailing slash, the other without.
                // "prefix" has a trailing slash.
                var fp = fsi.FullName.Replace('\\', '/');
                if (re.IsMatch(fp)) return true;
                if (!fp.EndsWith("/")) fp = fp + "/";
                return re.IsMatch(fp);
            }
            else
            {
                var fp = fsi.FullName.Replace('\\', '/');
                return re.IsMatch(fp);
            }
        }
        public bool IsMatch(string str)
        {
            return re.IsMatch(str);
        }
    }
}
