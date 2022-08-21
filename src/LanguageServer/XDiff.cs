using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using Algorithms;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ZhangShashaCSharp;
using static LanguageServer.Transform;

// See http://pages.cs.wisc.edu/~yuanwang/xdiff.html

namespace LanguageServer
{
    public class XDiffMain
    {
        private Parser _parser;
        private Lexer _lexer;
        List<IParseTree> n1 = new List<IParseTree>();
        List<IParseTree> n2 = new List<IParseTree>();
        Dictionary<IParseTree, string> s1 = new Dictionary<IParseTree, string>();
        Dictionary<IParseTree, string> s2 = new Dictionary<IParseTree, string>();

        public XDiffMain(Parser parser, Lexer lexer)
        {
            _parser = parser;
            _lexer = lexer;
        }

        public List<ZhangShashaCSharp.Operation> Main(IParseTree t1, IParseTree t2)
        {
            // Convert the parse tree to a graph of rule symbols, then create a tree of uses from start rule.
            var table1 = new Transform.TableOfRules();
            table1.ReadRules(t1);
            table1.FindPartitions();
            table1.FindStartRules();

            var table2 = new Transform.TableOfRules();
            table2.ReadRules(t1);
            table2.FindPartitions();
            table2.FindStartRules();

            Digraph<string> g1 = new Digraph<string>();
            foreach (TableOfRules.Row r in table1.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                g1.AddVertex(r.LHS);
            }
            foreach (TableOfRules.Row r in table1.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                List<string> j = r.RHS;
                //j.Reverse();
                foreach (string rhs in j)
                {
                    TableOfRules.Row sym = table1.rules.Where(t => t.LHS == rhs).FirstOrDefault();
                    if (!sym.is_parser_rule)
                    {
                        continue;
                    }

                    DirectedEdge<string> e = new DirectedEdge<string>() { From = r.LHS, To = rhs };
                    g1.AddEdge(e);
                }
            }
            List<string> starts1 = new List<string>();
            foreach (TableOfRules.Row r in table1.rules)
            {
                if (r.is_parser_rule && r.is_start)
                {
                    starts1.Add(r.LHS);
                }
            }

            Digraph<string> g2 = new Digraph<string>();
            foreach (TableOfRules.Row r in table2.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                g2.AddVertex(r.LHS);
            }
            foreach (TableOfRules.Row r in table2.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                List<string> j = r.RHS;
                //j.Reverse();
                foreach (string rhs in j)
                {
                    TableOfRules.Row sym = table2.rules.Where(t => t.LHS == rhs).FirstOrDefault();
                    if (!sym.is_parser_rule)
                    {
                        continue;
                    }

                    DirectedEdge<string> e = new DirectedEdge<string>() { From = r.LHS, To = rhs };
                    g2.AddEdge(e);
                }
            }
            List<string> starts2 = new List<string>();
            foreach (TableOfRules.Row r in table2.rules)
            {
                if (r.is_parser_rule && r.is_start)
                {
                    starts2.Add(r.LHS);
                }
            }

            var map1 = new Dictionary<string, Node>();
            var po1 = Algorithms.Postorder.Sort(g1, starts1).ToList();

            throw new NotImplementedException();
        }

        private List<ZhangShashaCSharp.Operation> EditScript(int match)
        {
            throw new NotImplementedException();
        }
    }
}
