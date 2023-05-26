using Algorithms;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace driver
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

    public class AntlrGraph : ANTLRv4ParserBaseVisitor<Digraph<string, SymbolEdge<string>>>
    {
        private int gen = 0;

        public Dictionary<string, Digraph<string, SymbolEdge<string>>> Rules { get; set; } = new Dictionary<string, Digraph<string, SymbolEdge<string>>>();

        public override Digraph<string, SymbolEdge<string>> VisitActionBlock(ANTLRv4Parser.ActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitActionScopeName(ANTLRv4Parser.ActionScopeNameContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitAction_(ANTLRv4Parser.Action_Context context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitAlternative(ANTLRv4Parser.AlternativeContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            var last = new List<string>() { g.AddStart(g.AddVertex(f)) };
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.element())
            {
                var cg = this.VisitElement(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices)
                    foreach (var l in last)
                        g.AddEdge(new SymbolEdge<string>() { From = l, To = v, _symbol = null });
                last = new List<string>(cg.EndVertices);
            }
            foreach (var l in last) g.AddEdge(new SymbolEdge<string>() { From = l, To = t, _symbol = null });
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitAltList(ANTLRv4Parser.AltListContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.alternative())
            {
                var cg = this.VisitAlternative(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitArgActionBlock(ANTLRv4Parser.ArgActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitAtom(ANTLRv4Parser.AtomContext context)
        {
            var ct1 = context.terminal();
            if (ct1 != null) return this.Visit(ct1);
            var ct2 = context.ruleref();
            if (ct2 != null) return this.Visit(ct2);
            var ct3 = context.notSet();
            if (ct3 != null) return this.Visit(ct3);
            var ct4 = context.DOT();
            if (ct4 == null) throw new Exception();
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = "." });
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitBlock(ANTLRv4Parser.BlockContext context)
        {
            var cg = this.VisitAltList(context.altList());
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitBlockSet(ANTLRv4Parser.BlockSetContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.setElement())
            {
                var cg = this.VisitSetElement(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitBlockSuffix(ANTLRv4Parser.BlockSuffixContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitChannelsSpec(ANTLRv4Parser.ChannelsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitCharacterRange(ANTLRv4Parser.CharacterRangeContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.GetText() });
            return g;
        }

        //public override Digraph<string, SymbolEdge> VisitChildren(IRuleNode node)
        //{
        //}

        public override Digraph<string, SymbolEdge<string>> VisitDelegateGrammar(ANTLRv4Parser.DelegateGrammarContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitDelegateGrammars(ANTLRv4Parser.DelegateGrammarsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitEbnf(ANTLRv4Parser.EbnfContext context)
        {
            var cg = this.VisitBlock(context.block());
            var g = new Digraph<string, SymbolEdge<string>>();
            var suffix = context.blockSuffix()?.GetText();
            switch (suffix)
            {
                case "+":
                case "+?":
                    {
                        var f = "" + gen++;
                        var t = "" + gen++;
                        g.AddStart(g.AddVertex(f));
                        g.AddEnd(g.AddVertex(t));
                        foreach (var v in cg.Vertices) g.AddVertex(v);
                        foreach (var e in cg.Edges) g.AddEdge(e);
                        foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                        g.AddEdge(new SymbolEdge<string>() { From = t, To = f, _symbol = null });
                        break;
                    }
                case "*":
                case "*?":
                    {
                        var f = "" + gen++;
                        g.AddStart(g.AddVertex(f));
                        g.AddEnd(g.AddVertex(f));
                        foreach (var v in cg.Vertices) g.AddVertex(v);
                        foreach (var e in cg.Edges) g.AddEdge(e);
                        foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
                        break;
                    }
                case "?":
                case "??":
                    {
                        var f = "" + gen++;
                        var t = "" + gen++;
                        g.AddStart(g.AddVertex(f));
                        g.AddEnd(g.AddVertex(t));
                        foreach (var v in cg.Vertices) g.AddVertex(v);
                        foreach (var e in cg.Edges) g.AddEdge(e);
                        foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                        g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                        break;
                    }
                case null:
                    {
                        return cg;
                    }
                default:
                    throw new Exception();
                    break;
            }
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitEbnfSuffix(ANTLRv4Parser.EbnfSuffixContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitElement(ANTLRv4Parser.ElementContext context)
        {
            if (context.labeledElement() != null)
            {
                var cg = this.VisitLabeledElement(context.labeledElement());
                var g = new Digraph<string, SymbolEdge<string>>();
                var suffix = context.ebnfSuffix()?.GetText();
                switch (suffix)
                {
                    case null:
                        {
                            return cg;
                        }
                    case "+":
                    case "+?":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.atom() != null)
            {
                var cg = this.VisitAtom(context.atom());
                var g = new Digraph<string, SymbolEdge<string>>();
                var suffix = context.ebnfSuffix()?.GetText();
                switch (suffix)
                {
                    case null:
                        {
                            return cg;
                        }
                    case "+":
                    case "+?":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.ebnf() != null)
            {
                var cg = this.VisitEbnf(context.ebnf());
                return cg;
            }
            else if (context.actionBlock() != null)
            {
                var f = "" + gen++;
                var t = "" + gen++;
                var g = new Digraph<string, SymbolEdge<string>>();
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitElementOption(ANTLRv4Parser.ElementOptionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitElementOptions(ANTLRv4Parser.ElementOptionsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitErrorNode(IErrorNode node)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitExceptionGroup(ANTLRv4Parser.ExceptionGroupContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitExceptionHandler(ANTLRv4Parser.ExceptionHandlerContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitFinallyClause(ANTLRv4Parser.FinallyClauseContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitGrammarDecl(ANTLRv4Parser.GrammarDeclContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitGrammarSpec(ANTLRv4Parser.GrammarSpecContext context)
        {
            return this.Visit(context.rules());
        }

        public override Digraph<string, SymbolEdge<string>> VisitGrammarType(ANTLRv4Parser.GrammarTypeContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitIdentifier(ANTLRv4Parser.IdentifierContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitIdList(ANTLRv4Parser.IdListContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLabeledAlt(ANTLRv4Parser.LabeledAltContext context)
        {
            var cg = this.VisitAlternative(context.alternative());
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLabeledElement(ANTLRv4Parser.LabeledElementContext context)
        {
            if (context.block() != null)
            {
                var cg = this.VisitBlock(context.block());
                return cg;
            }
            else if (context.atom() != null)
            {
                var cg = this.VisitAtom(context.atom());
                return cg;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitLabeledLexerElement(ANTLRv4Parser.LabeledLexerElementContext context)
        {
            if (context.lexerBlock() != null)
            {
                var cg = this.VisitLexerBlock(context.lexerBlock());
                return cg;
            }
            else if (context.lexerAtom() != null)
            {
                var cg = this.VisitLexerAtom(context.lexerAtom());
                return cg;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerAlt(ANTLRv4Parser.LexerAltContext context)
        {
            var cg = this.VisitLexerElements(context.lexerElements());
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerAltList(ANTLRv4Parser.LexerAltListContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.lexerAlt())
            {
                var cg = this.VisitLexerAlt(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerAtom(ANTLRv4Parser.LexerAtomContext context)
        {
            var ct1 = context.terminal();
            if (ct1 != null) return this.VisitTerminal(ct1);
            var ct2 = context.characterRange();
            if (ct2 != null) return this.VisitCharacterRange(ct2);
            var ct3 = context.notSet();
            if (ct3 != null) return this.VisitNotSet(ct3);
            var ct5 = context.LEXER_CHAR_SET();
            if (ct5 != null)
            {
                var g = new Digraph<string, SymbolEdge<string>>();
                var f = "" + gen++;
                var t = "" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = ct5.GetText() });
                return g;
            }
            var ct4 = context.DOT();
            if (ct4 == null) throw new Exception();
            {
                var g = new Digraph<string, SymbolEdge<string>>();
                var f = "" + gen++;
                var t = "" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = "." });
                return g;
            }
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerBlock(ANTLRv4Parser.LexerBlockContext context)
        {
            var cg = this.VisitLexerAltList(context.lexerAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerCommand(ANTLRv4Parser.LexerCommandContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerCommandExpr(ANTLRv4Parser.LexerCommandExprContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerCommandName(ANTLRv4Parser.LexerCommandNameContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerCommands(ANTLRv4Parser.LexerCommandsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerElement(ANTLRv4Parser.LexerElementContext context)
        {
            if (context.labeledLexerElement() != null)
            {
                var cg = this.VisitLabeledLexerElement(context.labeledLexerElement());
                var g = new Digraph<string, SymbolEdge<string>>();
                var suffix = context.ebnfSuffix()?.GetText();
                switch (suffix)
                {
                    case null:
                        {
                            return cg;
                        }
                    case "+":
                    case "+?":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.lexerAtom() != null)
            {
                var cg = this.VisitLexerAtom(context.lexerAtom());
                var g = new Digraph<string, SymbolEdge<string>>();
                var suffix = context.ebnfSuffix()?.GetText();
                switch (suffix)
                {
                    case null:
                        {
                            return cg;
                        }
                    case "+":
                    case "+?":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.lexerBlock() != null)
            {
                var cg = this.VisitLexerBlock(context.lexerBlock());
                var g = new Digraph<string, SymbolEdge<string>>();
                var suffix = context.ebnfSuffix()?.GetText();
                switch (suffix)
                {
                    case null:
                        {
                            return cg;
                        }
                    case "+":
                    case "+?":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "" + gen++;
                            var t = "" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.actionBlock() != null)
            {
                var f = "" + gen++;
                var t = "" + gen++;
                var g = new Digraph<string, SymbolEdge<string>>();
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = null });
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerElements(ANTLRv4Parser.LexerElementsContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            var last = new List<string>() { g.AddStart(g.AddVertex(f)) };
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.lexerElement())
            {
                var cg = this.VisitLexerElement(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices)
                    foreach (var l in last)
                        g.AddEdge(new SymbolEdge<string>() { From = l, To = v, _symbol = null });
                last = new List<string>(cg.EndVertices);
            }
            foreach (var l in last) g.AddEdge(new SymbolEdge<string>() { From = l, To = t, _symbol = null });
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerRuleBlock(ANTLRv4Parser.LexerRuleBlockContext context)
        {
            var cg = this.VisitLexerAltList(context.lexerAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerRuleSpec(ANTLRv4Parser.LexerRuleSpecContext context)
        {
            var cg = this.VisitLexerRuleBlock(context.lexerRuleBlock());
            Rules[context.TOKEN_REF().GetText()] = cg;
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLocalsSpec(ANTLRv4Parser.LocalsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitModeSpec(ANTLRv4Parser.ModeSpecContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            foreach (var c in context.lexerRuleSpec())
            {
                var cg = this.VisitLexerRuleSpec(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddStart(v);
                foreach (var v in cg.EndVertices) g.AddEnd(v);
            }
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitNotSet(ANTLRv4Parser.NotSetContext context)
        {
            if (context.setElement() != null)
            {
                var cg = this.VisitSetElement(context.setElement());
                return cg;
            }
            else if (context.blockSet() != null)
            {
                var cg = this.VisitBlockSet(context.blockSet());
                return cg;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitOption(ANTLRv4Parser.OptionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitOptionsSpec(ANTLRv4Parser.OptionsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitOptionValue(ANTLRv4Parser.OptionValueContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitParserRuleSpec(ANTLRv4Parser.ParserRuleSpecContext context)
        {
            var cg = this.VisitRuleBlock(context.ruleBlock());
            Rules[context.RULE_REF().GetText()] = cg;
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitPrequelConstruct(ANTLRv4Parser.PrequelConstructContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleAction(ANTLRv4Parser.RuleActionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleAltList(ANTLRv4Parser.RuleAltListContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.labeledAlt())
            {
                var cg = this.VisitLabeledAlt(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleBlock(ANTLRv4Parser.RuleBlockContext context)
        {
            var cg = this.VisitRuleAltList(context.ruleAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleModifier(ANTLRv4Parser.RuleModifierContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleModifiers(ANTLRv4Parser.RuleModifiersContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRulePrequel(ANTLRv4Parser.RulePrequelContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleref(ANTLRv4Parser.RulerefContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            if (context.RULE_REF() != null)
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.RULE_REF().GetText() });
            var n = g.AddVertex(context.GetText());
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleReturns(ANTLRv4Parser.RuleReturnsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRules(ANTLRv4Parser.RulesContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            foreach (var c in context.ruleSpec())
            {
                var cg = this.VisitRuleSpec(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddStart(v);
                foreach (var v in cg.EndVertices) g.AddEnd(v);
            }
            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitRuleSpec(ANTLRv4Parser.RuleSpecContext context)
        {
            var cg = this.Visit(context.GetChild(0));
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitSetElement(ANTLRv4Parser.SetElementContext context)
        {
            if (context.TOKEN_REF() != null)
            {
                var g = new Digraph<string, SymbolEdge<string>>();
                var f = "" + gen++;
                var t = "" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.TOKEN_REF().GetText() });
                return g;
            }
            else if (context.STRING_LITERAL() != null)
            {
                var g = new Digraph<string, SymbolEdge<string>>();
                var f = "" + gen++;
                var t = "" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.STRING_LITERAL().GetText() });
                return g;
            }
            else if (context.characterRange() != null)
            {
                var cg = this.VisitCharacterRange(context.characterRange());
                return cg;
            }
            else if (context.LEXER_CHAR_SET() != null)
            {
                var g = new Digraph<string, SymbolEdge<string>>();
                var f = "" + gen++;
                var t = "" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.LEXER_CHAR_SET().GetText() });
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitTerminal(ANTLRv4Parser.TerminalContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            if (context.TOKEN_REF() != null)
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.TOKEN_REF().GetText() });
            else
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.STRING_LITERAL().GetText() });
            return g;
        }

        //public override Digraph<string, SymbolEdge> VisitTerminal(ITerminalNode node)
        //{
        //}

        public override Digraph<string, SymbolEdge<string>> VisitThrowsSpec(ANTLRv4Parser.ThrowsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitTokensSpec(ANTLRv4Parser.TokensSpecContext context)
        {
            return null;
        }
    }

    public class Model
    {
        public static Graph ThompsonsGraph { get; set; } = null;
        public static Graph PowerSetGraph { get; set; } = null;
        public static string ThompsonsSvg { get; set; } = null;
        public static string PowerSetSvg { get; set; } = null;

        public static void ComputeModel(string input)
        {
            Digraph<string, SymbolEdge<string>> t = Parse(input);
            ThompsonsGraph = ToAGL(t);
            ThompsonsSvg = Model.PrintSvgAsString(ThompsonsGraph);
            Digraph<MyHashSet<string>, SymbolEdge<MyHashSet<string>>> m = ToPowerSet(t);
            Digraph<string, SymbolEdge<string>> m2 = FlattenStates(m);
            PowerSetGraph = ToAGL(m2);
            PowerSetSvg = Model.PrintSvgAsString(PowerSetGraph);
        }

        private static Digraph<string, SymbolEdge<string>> FlattenStates(Digraph<MyHashSet<string>, SymbolEdge<MyHashSet<string>>> m)
        {
            var redo = new Digraph<string, SymbolEdge<string>>();
            var rename = new Dictionary<MyHashSet<string>, string>();
            int s = 0;
            foreach (var v in m.Vertices)
            {
                //string n = String.Join(',', v.Select(x => x)) ;
                string n = s++.ToString();
                rename[v] = n;
                redo.AddVertex(n);
                if (m.StartVertices.Contains(v))
                {
                    redo.AddStart(n);
                }
                else if (m.EndVertices.Contains(v))
                {
                    redo.AddEnd(n);
                }
            }
            foreach (var e in m.Edges)
            {
                redo.AddEdge(new SymbolEdge<string>() { From = rename[e.From], To = rename[e.To], _symbol = e._symbol });
            }
            return redo;
        }

        public static Digraph<MyHashSet<string>, SymbolEdge<MyHashSet<string>>> ToPowerSet(Digraph<string, SymbolEdge<string>> NFA)
        {
            Digraph<MyHashSet<string>, SymbolEdge<MyHashSet<string>>> DFA = new Digraph<MyHashSet<string>, SymbolEdge<MyHashSet<string>>>();
            MyHashSet<MyHashSet<string>> marked = new MyHashSet<MyHashSet<string>>();
            MyHashSet<MyHashSet<string>> unmarked = new MyHashSet<MyHashSet<string>>();
            foreach (var s in NFA.StartVertices)
            {
                var startingState = new MyHashSet<string>(new List<string> { s });
                var new_dfa_state = EpsilonClosureOf(NFA, s);
                DFA.AddVertex(new_dfa_state);
                var hs = new MyHashSet<string>();
                for (int i = new_dfa_state.Count - 1; i >= 0; --i) hs.Add(new_dfa_state.ElementAt(i));
                DFA.AddVertex(hs);
                unmarked.Add(new_dfa_state);
                DFA.AddStart(new_dfa_state);
            }
            while (unmarked.Any())
            {
                MyHashSet<string> tttt = unmarked.First();
                unmarked.Remove(tttt);
                marked.Add(tttt);
                HashSet<string> alphabetReach = new HashSet<string>();
                foreach (string x in tttt)
                {
                    foreach (var e in NFA.SuccessorEdges(x))
                    {
                        if (!(e._symbol != null && e._symbol != "")) continue;
                        var v = e.To;
                        var u = EpsilonClosureOf(NFA, v);
                        Add(unmarked, NFA, DFA, u);
                        if (!(DFA.Edges
                            .Where(prev =>
                            {
                                var test1 = prev.From.Equals(tttt);
                                var test2 = prev.To.Equals(u);
                                var test3 = prev._symbol.Equals(e._symbol);
                                return test1 && test2 && test3;
                            }).Any()))
                            DFA.AddEdge(new SymbolEdge<MyHashSet<string>>() { From = tttt, To = u, _symbol = e._symbol });
                    }
                }
            }
            return DFA;
        }

        private static void Add(MyHashSet<MyHashSet<string>> unmarked, Digraph<string, SymbolEdge<string>> NFA, Digraph<MyHashSet<string>, SymbolEdge<MyHashSet<string>>> DFA, MyHashSet<string> u)
        {
            List<MyHashSet<string>> list = DFA.Vertices.ToList();

       //     if (!FancyContains(list, u))
            if (!DFA.Vertices.Contains(u))
            {
                unmarked.Add(u);
                DFA.AddVertex(u);
                foreach (var x in u)
                {
                    if (NFA.EndVertices.Contains(x))
                    {
                        DFA.AddEnd(u);
                        break;
                    }
                }
            }
        }

        private static MyHashSet<string> EpsilonClosureOf(Digraph<string, SymbolEdge<string>> graph, string theState)
        {
            MyHashSet<string> result = new MyHashSet<string>();
            Stack<string> s = new Stack<string>();
            MyHashSet<string> visited = new MyHashSet<string>();
            s.Push(theState);
            while (s.Any())
            {
                string v = s.Pop();
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

        public static Digraph<string, SymbolEdge<string>> Parse(string input, string type = "antlr4")
        {
            Graph result = null;
            switch (type) {
                case "antlr4":
                    {
                        IParseTree pt = null;
                        byte[] byteArray = Encoding.UTF8.GetBytes(input);
                        AntlrInputStream ais = new AntlrInputStream(
                        new StreamReader(
                            new MemoryStream(byteArray)).ReadToEnd());
                        ANTLRv4Lexer lexer = new ANTLRv4Lexer(ais);
                        CommonTokenStream cts = new CommonTokenStream(lexer);
                        ANTLRv4Parser parser = new ANTLRv4Parser(cts);
                        lexer.RemoveErrorListeners();
                        var lexer_error_listener = new ErrorListener<int>(parser, lexer, 5);
                        lexer.AddErrorListener(lexer_error_listener);
                        parser.RemoveErrorListeners();
                        var parser_error_listener = new ErrorListener<IToken>(parser, lexer, 5);
                        parser.AddErrorListener(parser_error_listener);
                        BailErrorHandler bail_error_handler = null;
                        bail_error_handler = new BailErrorHandler();
                        parser.ErrorHandler = bail_error_handler;
                        try
                        {
                            pt = parser.grammarSpec();
                        }
                        catch (Exception)
                        {
                            // Parsing error.
                        }
                        if (parser_error_listener.had_error || lexer_error_listener.had_error || (bail_error_handler != null && bail_error_handler.had_error))
                            return null;

                        var ag = new AntlrGraph();
                        Digraph<string, SymbolEdge<string>> res = ag.Visit(pt);
                        return res;
                    }
                    break;
            }
            return null;
        }
        
        private static Graph ToAGL(Digraph<string, SymbolEdge<string>> g)
        {
            var graph = new Graph();
            foreach (var v in g.Vertices)
            {
                var n = graph.AddNode(v);
                n.LabelText = v;
                if (g.EndVertices.Contains(v))
                {
                    n.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleVioletRed;
                }
                else if (g.StartVertices.Contains(v))
                {
                    n.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGreen;
                }
            }
            foreach (var e in g.Edges)
            {
                var n = e._symbol != null ? e._symbol : "";
                graph.AddEdge(e.From, n, e.To);
            }
            graph.CreateGeometryGraph();
            foreach (var n in graph.Nodes)
            {
                n.GeometryNode.BoundaryCurve = CurveFactory.CreateRectangleWithRoundedCorners(60, 40, 3, 2, new Point(0, 0));
               // n.GeometryNode.BoundaryCurve = CurveFactory.CreateCircle(60, new Point(0, 0));
                n.Label.Width = n.Width * 0.6;
                n.Label.Height = 40;
            }
            foreach (var de in graph.Edges)
            {
                // again setting the dimensions, that should depend on Drawing.Label and the viewer, blindly
                if (de.Label != null)
                {
                    de.Label.GeometryLabel.Width = 55;
                    de.Label.GeometryLabel.Height = 33;
                }
            }
            //AssignLabelsDimensions(graph);
            LayoutHelpers.CalculateLayout(graph.GeometryGraph, new SugiyamaLayoutSettings(), null);
            return graph;
        }

        public static string PrintSvgAsString(Graph graph)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            var svgWriter = new SvgGraphWriter(writer.BaseStream, graph);
            svgWriter.Write();
            ms.Position = 0;
            var sr = new StreamReader(ms);
            var result = sr.ReadToEnd();
            var start = result.IndexOf("<svg");
            result = result.Substring(start);
            //result = "<svg viewBox=\"0 0 100 100\" width=\"100%\">" + result + "</svg>";
            Regex re1 = new Regex(@"width=""([^""]+)""");
            var vw = re1.Match(result);
            var v1 = vw.Groups[1].Value;
            Regex re2 = new Regex(@"height=""([^""]+)""");
            var vh = re2.Match(result);
            var v2 = vh.Groups[1].Value;
            // result = re1.Replace(result, @"viewBox=""0 0 400 400"" width=""100%""", 1);
            result = re1.Replace(result, "viewBox=\"0 0 " + v1 + " " + v2 + "\"", 1);
        //    result = re2.Replace(result, @"height=""100%""", 1);
           
            return result;
        }
    }
}
