using System;
using System.Collections.Generic;
using Algorithms;
using System.Linq;
using Workspaces;
using LanguageServer;
using System.IO;
using org.eclipse.wst.xml.xpath2.processor.util;
using Antlr4.Runtime.Tree;

namespace ConsoleApp1
{
    public class SymbolEdge<T> : DirectedEdge<T>
    {
        public SymbolEdge() { }

        public string _symbol { get; set; }

        public override string ToString()
        {
            return From + "->" + To + (_symbol == null ? " [ label=\"&#1013;\" ]" : " [label=\"" + _symbol + "\" ]") + ";";
        }
    }
    public class MyHashSet<T> : HashSet<T>
    {
        public MyHashSet(IEnumerable<T> o) : base(o) { }
        //public MyHashSet(T o) : base(o) { }
        public MyHashSet() : base() { }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            var o = obj as MyHashSet<T>;
            if (o == null) return false;
            if (o.Count != this.Count) return false;
            foreach (var c in this)
            {
                if (!o.Contains(c)) return false;
            }
            foreach (var c in o)
            {
                if (!this.Contains(c)) return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return this.Count;
        }
    }
    class Program
    {
        private static MyHashSet<string> EpsilonClosureOf(Digraph<string, SymbolEdge<string>> graph, string theState)
        {
            MyHashSet<string> result = new MyHashSet<string>();
            Stack<string> s = new Stack<string>();
            MyHashSet<string> visited = new MyHashSet<string>();
            s.Push(theState);
            while (s.Any())
            {
                var v = s.Pop();
                if (visited.Contains(v)) continue;
                visited.Add(v);
                result.Add(v);
                foreach (var o in graph.SuccessorEdges(v))
                {
                    if (!(o._symbol == null || o._symbol == "")) continue;
                    s.Push(o.To);
                }
            }
            return result;
        }

        public static Document CheckDoc(string path)
        {
            string file_name = path;
            Document document = Workspaces.Workspace.Instance.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                try
                {   // Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(file_name))
                    {
                        // Read the stream to a string, and write the string to the console.
                        string str = sr.ReadToEnd();
                        document.Code = str;
                    }
                }
                catch (IOException)
                {
                    throw new Exception("File does not exist.");
                }
                Project project = Workspaces.Workspace.Instance.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    Workspaces.Workspace.Instance.AddChild(project);
                }
                project.AddDocument(document);
            }
            document.Changed = true;
            _ = ParsingResultsFactory.Create(document);
            var workspace = document.Workspace;
            var results = new LanguageServer.Module().Compile(workspace);
            foreach (var result in results)
            {
                if (result.FullFileName == document.FullPath)
                    document.ParseTree = result.ParseTree;
            }
            return document;
        }

        public static Document CheckStringDoc(string str)
        {
            Document document = Document.CreateStringDocument(str);
            Project project = Workspaces.Workspace.Instance.FindProject("Misc");
            if (project == null)
            {
                project = new Project("Misc", "Misc", "Misc");
                Workspaces.Workspace.Instance.AddChild(project);
            }
            project.AddDocument(document);
            document.Changed = true;
            _ = ParsingResultsFactory.Create(document);
            var workspace = document.Workspace;
            var results = new LanguageServer.Module().Compile(workspace);
            return document;
        }


        static void Main(string[] args)
        {

            {
                //            Document doc1 = CheckStringDoc(@"
                //grammar t1;
                //a : 'b' b | 'c' c | 'd' d | 'd' a | ('b' | 'c') ;
                //b : 'b' | 'c' b? ;
                //c : 'b' | 'c' | 'd' | c 'd' ;
                //d : 'b' | d? 'c' ;
                //");
                Document doc3 = CheckDoc(@"c:/users/kenne/documents/github/grammars-upstream/antlr/antlr4/LexBasic.g4");
                Document doc2 = CheckDoc(@"c:/users/kenne/documents/github/grammars-upstream/antlr/antlr4/ANTLRv4Lexer.g4");
                Document doc1 = CheckDoc(@"c:/users/kenne/documents/github/grammars-upstream/antlr/antlr4/ANTLRv4Parser.g4");
                new LanguageServer.Module().Compile(Workspaces.Workspace.Instance);
                var enumeration = new LanguageServer.EnumerateDerivation(doc1, doc2, "grammarSpec");
                enumeration.Enumerate();
                return;
            }
            {
                Workspace _workspace = new Workspace();
                {
                    Document document = Document.CreateStringDocument(@"
grammar t1;
a : 'b' | 'c' | 'd' | 'd' a ;
// a : 'd'* ( 'b' | 'c' | 'd' ) ; << Should get this.
b : 'b' | 'c' b? ;
// a : 'c'* ('b' | 'c') ; << Should get this.
c : 'b' | 'c' | 'd' | c 'd' ;
//
d : 'b' | d? 'c' ;
//
");
                    _ = ParsingResultsFactory.Create(document);
                    var workspace = document.Workspace;
                    _ = new LanguageServer.Module().Compile(workspace);
                    Project project = _workspace.FindProject("Misc");
                    if (project == null)
                    {
                        project = new Project("Misc", "Misc", "Misc");
                        _workspace.AddChild(project);
                    }
                    project.AddDocument(document);
                    var pr = LanguageServer.ParsingResultsFactory.Create(document);
                    if (document.ParseTree == null)
                    {
                        new LanguageServer.Module().Compile(_workspace);
                    }
                    var result = LanguageServer.Transform.ConvertRecursionToKleeneOperator(document);
                }
            }
        }
    }
}
