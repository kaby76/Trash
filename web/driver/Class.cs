using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithms;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LanguageServer;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;

namespace driver
{
    public class SymbolEdge : DirectedEdge<string>
    {
        public SymbolEdge() { }

        public string _symbol { get; set; }

        public override string ToString()
        {
            return From + "->" + To + (_symbol == null ? " [ label=\"&#1013;\" ]" : " [label=\"" + _symbol + "\" ]") + ";";
        }
    }

    public class AntlrGraph : ANTLRv4ParserBaseVisitor<Digraph<string, SymbolEdge>>
    {
        private int gen = 0;

        public Dictionary<string, Digraph<string, SymbolEdge>> Rules { get; set; } = new Dictionary<string, Digraph<string, SymbolEdge>>();

        public override Digraph<string, SymbolEdge> VisitActionBlock(ANTLRv4Parser.ActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitActionScopeName(ANTLRv4Parser.ActionScopeNameContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitAction_(ANTLRv4Parser.Action_Context context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitAlternative(ANTLRv4Parser.AlternativeContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            var last = new List<string>() { g.AddStart(g.AddVertex(f)) };
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.element())
            {
                var cg = this.VisitElement(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices)
                    foreach (var l in last)
                        g.AddEdge(new SymbolEdge() { From = l, To = v, _symbol = null });
                last = new List<string>(cg.EndVertices);
            }
            foreach (var l in last) g.AddEdge(new SymbolEdge() { From = l, To = t, _symbol = null });
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitAltList(ANTLRv4Parser.AltListContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.alternative())
            {
                var cg = this.VisitAlternative(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitArgActionBlock(ANTLRv4Parser.ArgActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitAtom(ANTLRv4Parser.AtomContext context)
        {
            var ct1 = context.terminal();
            if (ct1 != null) return this.Visit(ct1);
            var ct2 = context.ruleref();
            if (ct2 != null) return this.Visit(ct2);
            var ct3 = context.notSet();
            if (ct3 != null) return this.Visit(ct3);
            var ct4 = context.DOT();
            if (ct4 == null) throw new Exception();
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = "." });
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitBlock(ANTLRv4Parser.BlockContext context)
        {
            var cg = this.VisitAltList(context.altList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitBlockSet(ANTLRv4Parser.BlockSetContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.setElement())
            {
                var cg = this.VisitSetElement(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitBlockSuffix(ANTLRv4Parser.BlockSuffixContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitChannelsSpec(ANTLRv4Parser.ChannelsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitCharacterRange(ANTLRv4Parser.CharacterRangeContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = context.GetText() });
            return g;
        }

        //public override Digraph<string, SymbolEdge> VisitChildren(IRuleNode node)
        //{
        //}

        public override Digraph<string, SymbolEdge> VisitDelegateGrammar(ANTLRv4Parser.DelegateGrammarContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitDelegateGrammars(ANTLRv4Parser.DelegateGrammarsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitEbnf(ANTLRv4Parser.EbnfContext context)
        {
            var cg = this.VisitBlock(context.block());
            var g = new Digraph<string, SymbolEdge>();
            var suffix = context.blockSuffix()?.GetText();
            switch (suffix)
            {
                case "+":
                case "+?":
                    {
                        var f = "s" + gen++;
                        var t = "s" + gen++;
                        g.AddStart(g.AddVertex(f));
                        g.AddEnd(g.AddVertex(t));
                        foreach (var v in cg.Vertices) g.AddVertex(v);
                        foreach (var e in cg.Edges) g.AddEdge(e);
                        foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                        g.AddEdge(new SymbolEdge() { From = t, To = f, _symbol = null });
                        break;
                    }
                case "*":
                case "*?":
                    {
                        var f = "s" + gen++;
                        g.AddStart(g.AddVertex(f));
                        g.AddEnd(g.AddVertex(f));
                        foreach (var v in cg.Vertices) g.AddVertex(v);
                        foreach (var e in cg.Edges) g.AddEdge(e);
                        foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = f, _symbol = null });
                        break;
                    }
                case "?":
                case "??":
                    {
                        var f = "s" + gen++;
                        var t = "s" + gen++;
                        g.AddStart(g.AddVertex(f));
                        g.AddEnd(g.AddVertex(t));
                        foreach (var v in cg.Vertices) g.AddVertex(v);
                        foreach (var e in cg.Edges) g.AddEdge(e);
                        foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                        foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                        g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
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

        public override Digraph<string, SymbolEdge> VisitEbnfSuffix(ANTLRv4Parser.EbnfSuffixContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitElement(ANTLRv4Parser.ElementContext context)
        {
            if (context.labeledElement() != null)
            {
                var cg = this.VisitLabeledElement(context.labeledElement());
                var g = new Digraph<string, SymbolEdge>();
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
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.atom() != null)
            {
                var cg = this.VisitAtom(context.atom());
                var g = new Digraph<string, SymbolEdge>();
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
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
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
                var f = "s" + gen++;
                var t = "s" + gen++;
                var g = new Digraph<string, SymbolEdge>();
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge> VisitElementOption(ANTLRv4Parser.ElementOptionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitElementOptions(ANTLRv4Parser.ElementOptionsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitErrorNode(IErrorNode node)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitExceptionGroup(ANTLRv4Parser.ExceptionGroupContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitExceptionHandler(ANTLRv4Parser.ExceptionHandlerContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitFinallyClause(ANTLRv4Parser.FinallyClauseContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitGrammarDecl(ANTLRv4Parser.GrammarDeclContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitGrammarSpec(ANTLRv4Parser.GrammarSpecContext context)
        {
            return this.Visit(context.rules());
        }

        public override Digraph<string, SymbolEdge> VisitGrammarType(ANTLRv4Parser.GrammarTypeContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitIdentifier(ANTLRv4Parser.IdentifierContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitIdList(ANTLRv4Parser.IdListContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLabeledAlt(ANTLRv4Parser.LabeledAltContext context)
        {
            var cg = this.VisitAlternative(context.alternative());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLabeledElement(ANTLRv4Parser.LabeledElementContext context)
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

        public override Digraph<string, SymbolEdge> VisitLabeledLexerElement(ANTLRv4Parser.LabeledLexerElementContext context)
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

        public override Digraph<string, SymbolEdge> VisitLexerAlt(ANTLRv4Parser.LexerAltContext context)
        {
            var cg = this.VisitLexerElements(context.lexerElements());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLexerAltList(ANTLRv4Parser.LexerAltListContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.lexerAlt())
            {
                var cg = this.VisitLexerAlt(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitLexerAtom(ANTLRv4Parser.LexerAtomContext context)
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
                var g = new Digraph<string, SymbolEdge>();
                var f = "s" + gen++;
                var t = "s" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = ct5.GetText() });
                return g;
            }
            var ct4 = context.DOT();
            if (ct4 == null) throw new Exception();
            {
                var g = new Digraph<string, SymbolEdge>();
                var f = "s" + gen++;
                var t = "s" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = "." });
                return g;
            }
        }

        public override Digraph<string, SymbolEdge> VisitLexerBlock(ANTLRv4Parser.LexerBlockContext context)
        {
            var cg = this.VisitLexerAltList(context.lexerAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommand(ANTLRv4Parser.LexerCommandContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommandExpr(ANTLRv4Parser.LexerCommandExprContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommandName(ANTLRv4Parser.LexerCommandNameContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommands(ANTLRv4Parser.LexerCommandsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerElement(ANTLRv4Parser.LexerElementContext context)
        {
            if (context.labeledLexerElement() != null)
            {
                var cg = this.VisitLabeledLexerElement(context.labeledLexerElement());
                var g = new Digraph<string, SymbolEdge>();
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
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.lexerAtom() != null)
            {
                var cg = this.VisitLexerAtom(context.lexerAtom());
                var g = new Digraph<string, SymbolEdge>();
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
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.lexerBlock() != null)
            {
                var cg = this.VisitLexerBlock(context.lexerBlock());
                var g = new Digraph<string, SymbolEdge>();
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
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = t, To = f, _symbol = null });
                            return g;
                        }
                    case "*":
                    case "*?":
                        {
                            var f = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(f));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = f, _symbol = null });
                            return g;
                        }
                    case "?":
                    case "??":
                        {
                            var f = "s" + gen++;
                            var t = "s" + gen++;
                            g.AddStart(g.AddVertex(f));
                            g.AddEnd(g.AddVertex(t));
                            foreach (var v in cg.Vertices) g.AddVertex(v);
                            foreach (var e in cg.Edges) g.AddEdge(e);
                            foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                            foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
                            g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
                            return g;
                        }
                    default:
                        throw new Exception();
                }
            }
            else if (context.actionBlock() != null)
            {
                var f = "s" + gen++;
                var t = "s" + gen++;
                var g = new Digraph<string, SymbolEdge>();
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = null });
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge> VisitLexerElements(ANTLRv4Parser.LexerElementsContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            var last = new List<string>() { g.AddStart(g.AddVertex(f)) };
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.lexerElement())
            {
                var cg = this.VisitLexerElement(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices)
                    foreach (var l in last)
                        g.AddEdge(new SymbolEdge() { From = l, To = v, _symbol = null });
                last = new List<string>(cg.EndVertices);
            }
            foreach (var l in last) g.AddEdge(new SymbolEdge() { From = l, To = t, _symbol = null });
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitLexerRuleBlock(ANTLRv4Parser.LexerRuleBlockContext context)
        {
            var cg = this.VisitLexerAltList(context.lexerAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLexerRuleSpec(ANTLRv4Parser.LexerRuleSpecContext context)
        {
            var cg = this.VisitLexerRuleBlock(context.lexerRuleBlock());
            Rules[context.TOKEN_REF().GetText()] = cg;
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLocalsSpec(ANTLRv4Parser.LocalsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitModeSpec(ANTLRv4Parser.ModeSpecContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
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

        public override Digraph<string, SymbolEdge> VisitNotSet(ANTLRv4Parser.NotSetContext context)
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

        public override Digraph<string, SymbolEdge> VisitOption(ANTLRv4Parser.OptionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitOptionsSpec(ANTLRv4Parser.OptionsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitOptionValue(ANTLRv4Parser.OptionValueContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitParserRuleSpec(ANTLRv4Parser.ParserRuleSpecContext context)
        {
            var cg = this.VisitRuleBlock(context.ruleBlock());
            Rules[context.RULE_REF().GetText()] = cg;
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitPrequelConstruct(ANTLRv4Parser.PrequelConstructContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleAction(ANTLRv4Parser.RuleActionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleAltList(ANTLRv4Parser.RuleAltListContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            foreach (var c in context.labeledAlt())
            {
                var cg = this.VisitLabeledAlt(c);
                foreach (var v in cg.Vertices) g.AddVertex(v);
                foreach (var e in cg.Edges) g.AddEdge(e);
                foreach (var v in cg.StartVertices) g.AddEdge(new SymbolEdge() { From = f, To = v, _symbol = null });
                foreach (var v in cg.EndVertices) g.AddEdge(new SymbolEdge() { From = v, To = t, _symbol = null });
            }
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitRuleBlock(ANTLRv4Parser.RuleBlockContext context)
        {
            var cg = this.VisitRuleAltList(context.ruleAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitRuleModifier(ANTLRv4Parser.RuleModifierContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleModifiers(ANTLRv4Parser.RuleModifiersContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRulePrequel(ANTLRv4Parser.RulePrequelContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleref(ANTLRv4Parser.RulerefContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            if (context.RULE_REF() != null)
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = context.RULE_REF().GetText() });
            var n = g.AddVertex(context.GetText());
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitRuleReturns(ANTLRv4Parser.RuleReturnsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRules(ANTLRv4Parser.RulesContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
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

        public override Digraph<string, SymbolEdge> VisitRuleSpec(ANTLRv4Parser.RuleSpecContext context)
        {
            var cg = this.Visit(context.GetChild(0));
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitSetElement(ANTLRv4Parser.SetElementContext context)
        {
            if (context.TOKEN_REF() != null)
            {
                var g = new Digraph<string, SymbolEdge>();
                var f = "s" + gen++;
                var t = "s" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = context.TOKEN_REF().GetText() });
                return g;
            }
            else if (context.STRING_LITERAL() != null)
            {
                var g = new Digraph<string, SymbolEdge>();
                var f = "s" + gen++;
                var t = "s" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = context.STRING_LITERAL().GetText() });
                return g;
            }
            else if (context.characterRange() != null)
            {
                var cg = this.VisitCharacterRange(context.characterRange());
                return cg;
            }
            else if (context.LEXER_CHAR_SET() != null)
            {
                var g = new Digraph<string, SymbolEdge>();
                var f = "s" + gen++;
                var t = "s" + gen++;
                g.AddStart(g.AddVertex(f));
                g.AddEnd(g.AddVertex(t));
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = context.LEXER_CHAR_SET().GetText() });
                return g;
            }
            else throw new Exception();
        }

        public override Digraph<string, SymbolEdge> VisitTerminal(ANTLRv4Parser.TerminalContext context)
        {
            var g = new Digraph<string, SymbolEdge>();
            var f = "s" + gen++;
            var t = "s" + gen++;
            g.AddStart(g.AddVertex(f));
            g.AddEnd(g.AddVertex(t));
            if (context.TOKEN_REF() != null)
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = context.TOKEN_REF().GetText() });
            else
                g.AddEdge(new SymbolEdge() { From = f, To = t, _symbol = context.STRING_LITERAL().GetText() });
            return g;
        }

        //public override Digraph<string, SymbolEdge> VisitTerminal(ITerminalNode node)
        //{
        //}

        public override Digraph<string, SymbolEdge> VisitThrowsSpec(ANTLRv4Parser.ThrowsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitTokensSpec(ANTLRv4Parser.TokensSpecContext context)
        {
            return null;
        }
    }

    public class Class
    {
        public static Graph Parse(string input, string type)
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
                        var res = ag.Visit(pt);
                        return CreateGraphThompson(res);
                    }
                    break;
            }
            return null;
        }
        private static Graph CreateGraphThompson(Digraph<string, SymbolEdge> g)
        {
            var graph = new Graph();
            foreach (var v in g.Vertices)
            {
                graph.AddNode(v);
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



        public static Graph CreateGraph(IParseTree[] trees, IList<string> parserRules, IList<string> lexerRules)
        {
            var graph = new Graph();
            foreach (var tree in trees)
            {
                if (tree != null)
                {
                    if (tree.ChildCount == 0)
                        graph.AddNode(tree.GetHashCode().ToString());
                    else
                        GraphEdges(graph, tree, tree.GetHashCode());
                    FormatNodes(graph, tree, parserRules, lexerRules, tree.GetHashCode());
                }
            }
            return graph;
        }

        private static void GraphEdges(Graph graph, ITree tree, int base_hash_code)
        {
            for (var i = tree.ChildCount - 1; i > -1; i--)
            {
                var child = tree.GetChild(i);
                graph.AddEdge((base_hash_code + tree.GetHashCode()).ToString(),
                    (base_hash_code + child.GetHashCode()).ToString());

                GraphEdges(graph, child, base_hash_code);
            }
        }

        private static void FormatNodes(Graph graph, ITree tree, IList<string> parserRules, IList<string> lexerRules, int base_hash_code)
        {
            var node = graph.FindNode((base_hash_code + tree.GetHashCode()).ToString());
            if (node != null)
            {
                node.LabelText = Trees.GetNodeText(tree, parserRules);
                var ruleFailedAndMatchedNothing = false;
                if (tree is ParserRuleContext context)
                {
                    ruleFailedAndMatchedNothing =
                        // ReSharper disable once ComplexConditionExpression
                        context.exception != null &&
                        context.Stop != null
                        && context.Stop.TokenIndex < context.Start.TokenIndex;
                }
                else if (tree is TerminalNodeImpl term)
                {
                    var token = term.Symbol.Type;
                    var token_value = term.Symbol.Text;
                    node.LabelText = token > 0 ?
                    (lexerRules[token - 1] + "/" + token_value) : "EOF";
                }
                if (tree is IErrorNode || ruleFailedAndMatchedNothing)
                    node.Label.FontColor = Color.Red;
                else
                    node.Label.FontColor = Color.Black;
                node.Attr.Color = Color.Black;
                node.UserData = tree;
                //if (BackgroundColor.HasValue)
                //    node.Attr.FillColor = BackgroundColor.Value;
            }

            for (int i = 0; i < tree.ChildCount; i++)
                FormatNodes(graph, tree.GetChild(i), parserRules, lexerRules, base_hash_code);
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
            return result;
        }
    }
}
