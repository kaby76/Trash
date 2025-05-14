﻿using Algorithms;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Trash
{
    public class SymbolEdge<T> : DirectedEdge<T>
    {
        public SymbolEdge() { }

        public ITerminalNode _symbol { get; set; }

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

    public class Model : ANTLRv4ParserBaseVisitor<Digraph<string, SymbolEdge<string>>>
    {
        private int gen = 0;
        private string _grammar;
        public Parser _parser;
        public Lexer _lexer;
        public ITokenStream _input;

        public class Rule
        {
            public Rule()
            {
            }

            public string grammar;
            public string lhs;
            public int lhs_rule_number;
            public Digraph<string, SymbolEdge<string>> rhs;
        }

        public Rule[] Rules = new Rule[0];

        public Model()
        {
        }

        public override Digraph<string, SymbolEdge<string>> VisitActionBlock(ANTLRv4Parser.ActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitActionScopeName(
            ANTLRv4Parser.ActionScopeNameContext context)
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
                foreach (var v in cg.StartVertices)
                {
                    var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                    g.AddEdge(e);
                }

                foreach (var v in cg.EndVertices)
                {
                    var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                    g.AddEdge(e);
                }
            }

            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitArgActionBlock(
            ANTLRv4Parser.ArgActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitAtom(ANTLRv4Parser.AtomContext context)
        {
            var ct1 = context.terminalDef();
            if (ct1 != null) return this.Visit(ct1);
            var ct2 = context.ruleref();
            if (ct2 != null) return this.Visit(ct2);
            var ct3 = context.notSet();
            if (ct3 != null) return this.Visit(ct3);
            var ct4 = context.wildcard();
            if (ct4 == null) return this.Visit(ct4);
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            var e = new SymbolEdge<string>() { From = f, To = t, _symbol = ct4.DOT() };
            g.AddEdge(e);
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
                foreach (var v in cg.StartVertices)
                {
                    var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                    g.AddEdge(e);
                }

                foreach (var v in cg.EndVertices)
                {
                    var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                    g.AddEdge(e);
                }
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

        public override Digraph<string, SymbolEdge<string>> VisitCharacterRange(
            ANTLRv4Parser.CharacterRangeContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            var e = new SymbolEdge<string>() { From = f, To = t, _symbol = /*context.GetText()*/ null };
            g.AddEdge(e);
            return g;
        }

        //public override Digraph<string, SymbolEdge> VisitChildren(IRuleNode node)
        //{
        //}

        public override Digraph<string, SymbolEdge<string>> VisitDelegateGrammar(
            ANTLRv4Parser.DelegateGrammarContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitDelegateGrammars(
            ANTLRv4Parser.DelegateGrammarsContext context)
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
                    foreach (var v in cg.StartVertices)
                    {
                        var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                        g.AddEdge(e);
                    }

                    foreach (var v in cg.EndVertices)
                    {
                        var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                        g.AddEdge(e);
                    }

                    {
                        var e = new SymbolEdge<string>() { From = t, To = f, _symbol = null };
                        g.AddEdge(e);
                    }
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
                    foreach (var v in cg.StartVertices)
                    {
                        var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                        g.AddEdge(e);
                    }

                    foreach (var v in cg.EndVertices)
                    {
                        var e = new SymbolEdge<string>() { From = v, To = f, _symbol = null };
                        g.AddEdge(e);
                    }

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
                    foreach (var v in cg.StartVertices)
                    {
                        var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                        g.AddEdge(e);
                    }

                    foreach (var v in cg.EndVertices)
                    {
                        var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                        g.AddEdge(e);
                    }

                    {
                        var e = new SymbolEdge<string>() { From = f, To = t, _symbol = null };
                        g.AddEdge(e);
                    }
                    break;
                }
                case null:
                {
                    return cg;
                }
                default:
                    throw new Exception();
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
                        foreach (var v in cg.StartVertices)
                        {
                            var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                            g.AddEdge(e);
                        }

                        foreach (var v in cg.EndVertices)
                        {
                            var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                            g.AddEdge(e);
                        }

                        {
                            var e = new SymbolEdge<string>() { From = t, To = f, _symbol = null };
                            g.AddEdge(e);
                        }
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
                        foreach (var v in cg.StartVertices)
                        {
                            var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                            g.AddEdge(e);
                        }

                        foreach (var v in cg.EndVertices)
                        {
                            var e = new SymbolEdge<string>() { From = v, To = f, _symbol = null };
                            g.AddEdge(e);
                        }

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
                        foreach (var v in cg.StartVertices)
                        {
                            var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                            g.AddEdge(e);
                        }

                        foreach (var v in cg.EndVertices)
                        {
                            var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                            g.AddEdge(e);
                        }

                        {
                            var e = new SymbolEdge<string>() { From = f, To = t, _symbol = null };
                            g.AddEdge(e);
                        }
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
                        foreach (var v in cg.StartVertices)
                        {
                            var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                            g.AddEdge(e);
                        }

                        foreach (var v in cg.EndVertices)
                        {
                            var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                            g.AddEdge(e);
                        }

                        {
                            var e = new SymbolEdge<string>() { From = t, To = f, _symbol = null };
                            g.AddEdge(e);
                        }
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
                        foreach (var v in cg.StartVertices)
                        {
                            var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                            g.AddEdge(e);
                        }

                        foreach (var v in cg.EndVertices)
                        {
                            var e = new SymbolEdge<string>() { From = v, To = f, _symbol = null };
                            g.AddEdge(e);
                        }

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
                        foreach (var v in cg.StartVertices)
                        {
                            var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                            g.AddEdge(e);
                        }

                        foreach (var v in cg.EndVertices)
                        {
                            var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                            g.AddEdge(e);
                        }

                        {
                            var e = new SymbolEdge<string>() { From = f, To = t, _symbol = null };
                            g.AddEdge(e);
                        }
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
                var e = new SymbolEdge<string>() { From = f, To = t, _symbol = null };
                g.AddEdge(e);
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitElementOption(
            ANTLRv4Parser.ElementOptionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitElementOptions(
            ANTLRv4Parser.ElementOptionsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitErrorNode(IErrorNode node)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitExceptionGroup(
            ANTLRv4Parser.ExceptionGroupContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitExceptionHandler(
            ANTLRv4Parser.ExceptionHandlerContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitFinallyClause(
            ANTLRv4Parser.FinallyClauseContext context)
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

        public override Digraph<string, SymbolEdge<string>> VisitLabeledElement(
            ANTLRv4Parser.LabeledElementContext context)
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
                foreach (var v in cg.StartVertices)
                {
                    var e = new SymbolEdge<string>() { From = f, To = v, _symbol = null };
                    g.AddEdge(e);
                }

                foreach (var v in cg.EndVertices)
                {
                    var e = new SymbolEdge<string>() { From = v, To = t, _symbol = null };
                    g.AddEdge(e);
                }
            }

            return g;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerAtom(ANTLRv4Parser.LexerAtomContext context)
        {
            var ct1 = context.terminalDef();
            if (ct1 != null) return this.VisitTerminalDef(ct1);
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
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = ct5 });
                return g;
            }

            var ct4 = context.wildcard();
            if (ct4 == null) throw new Exception();
            {
                var g = new Digraph<string, SymbolEdge<string>>();
                var f = "" + gen++;
                var t = "" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = ct4.DOT() });
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

        public override Digraph<string, SymbolEdge<string>> VisitLexerCommandExpr(
            ANTLRv4Parser.LexerCommandExprContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerCommandName(
            ANTLRv4Parser.LexerCommandNameContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerCommands(
            ANTLRv4Parser.LexerCommandsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerElement(ANTLRv4Parser.LexerElementContext context)
        {
            if (context.lexerAtom() != null)
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
                        foreach (var v in cg.StartVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
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
                        foreach (var v in cg.StartVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
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
                        foreach (var v in cg.StartVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
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
                        foreach (var v in cg.StartVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
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
                        foreach (var v in cg.StartVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = v, To = f, _symbol = null });
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
                        foreach (var v in cg.StartVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices)
                            g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
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

        public override Digraph<string, SymbolEdge<string>> VisitLexerElements(
            ANTLRv4Parser.LexerElementsContext context)
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

        public override Digraph<string, SymbolEdge<string>> VisitLexerRuleBlock(
            ANTLRv4Parser.LexerRuleBlockContext context)
        {
            var cg = this.VisitLexerAltList(context.lexerAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitLexerRuleSpec(
            ANTLRv4Parser.LexerRuleSpecContext context)
        {
            var cg = this.VisitLexerRuleBlock(context.lexerRuleBlock());
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

        public override Digraph<string, SymbolEdge<string>> VisitParserRuleSpec(
            ANTLRv4Parser.ParserRuleSpecContext context)
        {
            var rule_name = context.RULE_REF().GetText();
            var cg = this.VisitRuleBlock(context.ruleBlock());
            Digraph<string, SymbolEdge<string>> m = ToPowerSet(cg);
            //Digraph<string, SymbolEdge<string>> m2 = FlattenStates(m);
            var rule = new Rule()
            {
                grammar = _grammar, lhs = rule_name,
                lhs_rule_number = _parser.GetRuleIndex(rule_name), rhs = m
            };

            if (Rules.Length <= rule.lhs_rule_number)
            {
                Array.Resize(ref Rules, rule.lhs_rule_number + 1);
            }
            Rules[rule.lhs_rule_number] = rule;
            return cg;
        }

        public override Digraph<string, SymbolEdge<string>> VisitPrequelConstruct(
            ANTLRv4Parser.PrequelConstructContext context)
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
                foreach (var v in cg.StartVertices)
                    g.AddEdge(new SymbolEdge<string>() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices)
                    g.AddEdge(new SymbolEdge<string>() { From = v, To = t, _symbol = null });
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

        public override Digraph<string, SymbolEdge<string>> VisitRuleModifiers(
            ANTLRv4Parser.RuleModifiersContext context)
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
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.RULE_REF() });
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
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.TOKEN_REF() });
                return g;
            }
            else if (context.STRING_LITERAL() != null)
            {
                var g = new Digraph<string, SymbolEdge<string>>();
                var f = "" + gen++;
                var t = "" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.STRING_LITERAL() });
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
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.LEXER_CHAR_SET() });
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge<string>> VisitTerminalDef(ANTLRv4Parser.TerminalDefContext context)
        {
            var g = new Digraph<string, SymbolEdge<string>>();
            var f = "" + gen++;
            var t = "" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            if (context.TOKEN_REF() != null)
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.TOKEN_REF() });
            else
                g.AddEdge(new SymbolEdge<string>() { From = f, To = t, _symbol = context.STRING_LITERAL() });
            return g;
        }


        public override Digraph<string, SymbolEdge<string>> VisitThrowsSpec(ANTLRv4Parser.ThrowsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge<string>> VisitTokensSpec(ANTLRv4Parser.TokensSpecContext context)
        {
            return null;
        }

        public void ComputeModel(string dll_path, Antlr4.Runtime.Parser pp, Antlr4.Runtime.Lexer ll, ITokenStream tt)
        {
            // Repeatedly, search directory for .g4's.
            // If there are aren't any, go up one directory and try again.
            // Once .g4's found, parse them and return.
            if (!dll_path.EndsWith("/")) dll_path += "/";
            var path = dll_path;
            path = Path.GetFullPath(path);
            List<string> grammar_files;
            for (;;)
            {
                var old_path = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(path);
                grammar_files = new TrashGlobbing.Glob(path)
                    .RegexContents(".*[.]g4")
                    .Where(f => f is FileInfo && !f.Attributes.HasFlag(FileAttributes.Directory))
                    .Select(f => f.FullName.Replace('\\', '/'))
                    .ToList();
                Directory.SetCurrentDirectory(old_path);
                if (grammar_files.Any()) break;
                path = path + "../";
            }
            foreach (var gfile in grammar_files)
            {
                ParseGrammar(gfile, pp, ll, tt);
            }
        }

        public static Digraph<string, SymbolEdge<string>> ToPowerSet(Digraph<string, SymbolEdge<string>> NFA)
        {
            Digraph<string, SymbolEdge<string>> DFA = new Digraph<string, SymbolEdge<string>>();
            Dictionary<MyHashSet<string>, string> nfas_to_dfa_state = new Dictionary<MyHashSet<string>, string>();
            Dictionary<string, MyHashSet<string>> dfa_state_to_nfas = new Dictionary<string, MyHashSet<string>>();
            Dictionary<string, MyHashSet<string>> nfa_closure = new Dictionary<string, MyHashSet<string>>();
            int number = 0;
            Stack<string> stack = new Stack<string>();
            // Precompute closures and dfa states for start.
            foreach (var s in NFA.Vertices)
            {
                MyHashSet<string> closure = EpsilonClosureOf(NFA, s);
                nfa_closure[s] = closure;
                if (NFA.StartVertices.Contains(s))
                {
                    if (!nfas_to_dfa_state.TryGetValue(closure, out string dfa_name))
                    {
                        // Create a new state name
                        dfa_name = "d" + number++;
                        nfas_to_dfa_state.Add(closure, dfa_name);
                        dfa_state_to_nfas.Add(dfa_name, closure);
                        DFA.AddVertex(dfa_name);
                        DFA.AddStart(dfa_name);
                        stack.Push(dfa_name);
                    }
                }
            }
            while (stack.Count() > 0)
            {
                var dfa_state = stack.Pop();
                var x = new Dictionary<ITerminalNode, MyHashSet<string>>();
                var nfas = dfa_state_to_nfas[dfa_state];
                foreach (var nfa_state in nfas)
                {
                    foreach (var e in NFA.SuccessorEdges(nfa_state))
                    {
                        if (e._symbol == null) continue;
                        var v = e.To;
                        if (!x.TryGetValue(e._symbol, out MyHashSet<string> to_state))
                        {
                            to_state = new MyHashSet<string>();
                            x[e._symbol] = to_state;
                        }
                        var u = nfa_closure[v];
                        to_state.UnionWith(u);
                    }
                }
                foreach (var pair in x)
                {
                    ITerminalNode k = pair.Key;
                    MyHashSet<string> v = pair.Value;
                    nfas_to_dfa_state.TryGetValue(v, out string new_dfa_state);
                    if (new_dfa_state == null)
                    {
                        // Create a new state name
                        new_dfa_state = "d" + number++;
                        nfas_to_dfa_state.Add(v, new_dfa_state);
                        dfa_state_to_nfas.Add(new_dfa_state, v);
                        DFA.AddVertex(new_dfa_state);
                        stack.Push(new_dfa_state);
                    }
                    if (!(DFA.Edges.Where(prev =>
                        {
                            var test1 = prev.From.Equals(dfa_state);
                            var test2 = prev.To.Equals(new_dfa_state);
                            var test3 = prev._symbol.Equals(k);
                            return test1 && test2 && test3;
                        }).Any()))
                        DFA.AddEdge(new SymbolEdge<string>() { From = dfa_state, To = new_dfa_state, _symbol = k });
                }
            }
            return DFA;
        }

        private static void Add(MyHashSet<string> unmarked, Digraph<string, SymbolEdge<string>> NFA,
            Digraph<string, SymbolEdge<string>> DFA, string u)
        {
            List<string> list = DFA.Vertices.ToList();

            //     if (!FancyContains(list, u))
            if (!DFA.Vertices.Contains(u))
            {
                unmarked.Add(u);
                DFA.AddVertex(u);
                if (NFA.EndVertices.Contains(u))
                {
                    DFA.AddEnd(u);
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
                    if (o._symbol != null) continue;
                    s.Push(o.To);
                }
            }
            return result;
        }

        public void ParseGrammar(string grammar, Antlr4.Runtime.Parser pp, Antlr4.Runtime.Lexer ll, ITokenStream tt)
        {
            System.Console.Error.WriteLine("Parsing grammar " + grammar);
            var lines = File.ReadAllText(grammar);
            var lexer = new ANTLRv4Lexer(new AntlrInputStream(lines));
            var common_token_stream = new CommonTokenStream(lexer);
            var parser = new ANTLRv4Parser(common_token_stream);
            ANTLRv4Parser.GrammarSpecContext pt = parser.grammarSpec();
            if (pt.grammarDecl().grammarType().LEXER() == null)
            {
                _grammar = grammar;
                _parser = pp;
                _lexer = ll;
                _input = common_token_stream;
                this.VisitGrammarSpec(pt);
            }
        }
    }
}
