using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Trash;

// Match archive member names, not filesystem paths. A single * stays within a
// path component; ** crosses directories, including zero directories in **/.
internal sealed class BundleMemberGlob
{
    private readonly Regex _regex;

    public BundleMemberGlob(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("--bundle-glob requires a nonempty pattern.");

        pattern = pattern.Replace('\\', '/');
        var expression = new StringBuilder("\\A");
        for (var index = 0; index < pattern.Length; index++)
        {
            switch (pattern[index])
            {
                case '*' when index + 1 < pattern.Length && pattern[index + 1] == '*':
                    index++;
                    if (index + 1 < pattern.Length && pattern[index + 1] == '/')
                    {
                        expression.Append("(?:[^/]+/)*");
                        index++;
                    }
                    else
                        expression.Append(".*");
                    break;
                case '*':
                    expression.Append("[^/]*");
                    break;
                case '?':
                    expression.Append("[^/]");
                    break;
                default:
                    expression.Append(Regex.Escape(pattern[index].ToString()));
                    break;
            }
        }
        expression.Append("\\z");
        _regex = new Regex(expression.ToString(), RegexOptions.CultureInvariant);
    }

    public bool IsMatch(string memberName) => _regex.IsMatch(memberName);
}
