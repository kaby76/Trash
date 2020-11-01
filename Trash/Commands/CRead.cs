namespace Trash.Commands
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    class CRead
    {
        public void Help()
        {
            System.Console.WriteLine(@"read <string>
Read the text file file-name and place it on the top of the stack. read accepts wildcards.

Example:
    read ""c:/users/kenne/documents/antlrvsix/corpus-for-codebuff/a.g4""
") ;
        }

        public void Execute(Repl repl, ReplParser.ReadContext tree, bool piped)
        {
            var f = repl.GetArg(tree.arg());
            string here = null;
            if (!f.Contains("."))
            {
                here = f;
            }
            if (f != null)
            {
                var files = new Globbing().Contents(f);
                if (files.Count() != 1)
                    throw new Exception("Ambiguous match for '" + f + "'.");
                var doc = repl._docs.ReadDoc(files.First().FullName);
                repl.stack.Push(doc);
            }
            else if (here != null)
            {
                StringBuilder sb = new StringBuilder();
                for (; ; )
                {
                    string cl;
                    if (repl.script_file != null)
                    {
                        cl = repl.lines[repl.current_line_index++];
                    }
                    else
                    {
                        cl = Console.ReadLine();
                    }
                    if (cl == here)
                        break;
                    sb.AppendLine(cl);
                }
                var s = sb.ToString();
                var doc = repl._docs.CreateDoc("temp" + repl.current_line_index + ".g4", s);
                repl.stack.Push(doc);
            }
            else throw new Exception("not sure what to read.");
        }
    }
}
