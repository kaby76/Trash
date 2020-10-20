using System;
using System.Collections.Generic;
using System.Text;

namespace Trash.Commands
{
    class CConvert
    {
        public void Help()
        {
            System.Console.WriteLine(@"convert (antlr2 | antlr3 | antlr4 | bison | ebnf)?
Convert the parsed grammar file at the top of stack into Antlr4 syntax. If the type of
grammar cannot be inferred from the file suffix, a type can be supplied with the command.
The resulting Antlr4 grammar replaces the top of stack.

Example:
    (top of stack contains a grammar file that is Antlr2 or 3, Bison, or EBNF syntax.)
    combine antlr3
");
        }

        public void Execute(Repl repl, ReplParser.ConvertContext tree)
        {
            var type = tree.type()?.GetText();
            var doc = repl.stack.Peek();
            var f = doc.FullPath;
            if (type == null || type == "")
            {
                if (doc.FullPath.EndsWith(".g4")) type = "antlr4";
                if (doc.FullPath.EndsWith(".g2")) type = "antlr2";
                if (doc.FullPath.EndsWith(".g3")) type = "antlr3";
                if (doc.FullPath.EndsWith(".y")) type = "bison";
                if (doc.FullPath.EndsWith(".ebnf")) type = "ebnf";
            }
            if (type == "antlr3")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                var imp = new LanguageServer.Antlr3Import();
                imp.Try(doc.FullPath, doc.Code, ref res);
                foreach (var r in res)
                {
                    var new_doc = repl._docs.CreateDoc(r.Key, r.Value);
                    repl.stack.Push(new_doc);
                    if (new_doc.FullPath.EndsWith(".g4")) repl._docs.ParseDoc(repl.stack.Peek(), repl.QuietAfter);
                }
            }
            else if (type == "antlr2")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                var imp = new LanguageServer.Antlr2Import();
                imp.Try(doc.FullPath, doc.Code, ref res);
                foreach (var r in res)
                {
                    var new_doc = repl._docs.CreateDoc(r.Key, r.Value);
                    repl.stack.Push(new_doc);
                    if (new_doc.FullPath.EndsWith(".g4")) repl._docs.ParseDoc(repl.stack.Peek(), repl.QuietAfter);
                }
            }
            else if (type == "bison")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                var imp = new LanguageServer.BisonImport();
                imp.Try(doc.FullPath, doc.Code, ref res);
                foreach (var r in res)
                {
                    var new_doc = repl._docs.CreateDoc(r.Key, r.Value);
                    repl.stack.Push(new_doc);
                    if (new_doc.FullPath.EndsWith(".g4")) repl._docs.ParseDoc(repl.stack.Peek(), repl.QuietAfter);
                }
            }
            else if (type == "ebnf")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                var imp = new LanguageServer.W3CebnfImport();
                imp.Try(doc.FullPath, doc.Code, ref res);
                foreach (var r in res)
                {
                    var new_doc = repl._docs.CreateDoc(r.Key, r.Value);
                    repl.stack.Push(new_doc);
                    if (new_doc.FullPath.EndsWith(".g4")) repl._docs.ParseDoc(repl.stack.Peek(), repl.QuietAfter);
                }
            }
            else if (type == "antlr4")
            {
                System.Console.WriteLine("Cannot convert an Antlr4 file to Antlr4.");
            }
            else
            {
                System.Console.WriteLine("Unknown type for conversion.");
            }
        }
    }
}
