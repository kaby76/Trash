using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Server
{

    public class Grammar
    {
        public Grammar()
        {
        }

        public Parser Parser { get; set; }
        public Lexer Lexer { get; set; }
        public Opt Options { get; set; }
        public string LanguageId { get; internal set; }

        public List<string> Classifiers()
        {
            return Options.ClassesAndClassifiers.Select(c => c.Item2.Trim()).Where(c => c != "").ToList();
        }

        public List<string> AllClasses()
        {
            IEnumerable<Tuple<string, string>> o1 = Options.ClassesAndClassifiers;
            var o2 = o1.Select(p => p.Item1).ToList();
            return o2;
        }

        public List<string> Classes()
        {
            return Options.ClassesAndClassifiers.Select(c => c.Item1.Trim()).Where(c => c != "").ToList();
        }

        public (IParseTree, Lexer, Parser, ITokenStream, ICharStream) Parse(Workspaces.Document document)
        {
            string input = document.Code;
            var dll = Options.ParserLocation;
            var full_path = Path.GetDirectoryName(dll);
            Assembly asm1 = Assembly.LoadFile(full_path + Path.DirectorySeparatorChar + "Antlr4.Runtime.Standard.dll");
            Assembly asm = Assembly.LoadFile(dll);
            //var xxxxxx = asm1.GetTypes();
            Type[] types = asm.GetTypes();
            Type type = asm.GetType("Program");
            var methods = type.GetMethods();
            MethodInfo methodInfo = type.GetMethod("Parse");
            object[] parm = new object[] { input };
            DateTime before = DateTime.Now;
            var res = methodInfo.Invoke(null, parm);
            var tree = res as IParseTree;
            var t2 = tree as ParserRuleContext;
            var m2 = type.GetProperty("Parser");
            object[] p2 = new object[0];
            var r2 = m2.GetValue(null, p2);
            var parser = r2 as Parser;
            var m3 = type.GetProperty("Lexer");
            object[] p3 = new object[0];
            var r3 = m3.GetValue(null, p3);
            var lexer = r3 as Lexer;
            var m4 = type.GetProperty("TokenStream");
            object[] p4 = new object[0];
            var r4 = m4.GetValue(null, p4);
            var token_stream = r4 as ITokenStream;
            var m5 = type.GetProperty("CharStream");
            object[] p5 = new object[0];
            var r5 = m5.GetValue(null, p5);
            var char_stream = r5 as ICharStream;

            return (tree, lexer, parser, token_stream, char_stream);
        }
    }
}