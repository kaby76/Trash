using Algorithms;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace LanguageServer
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

        //public override Digraph<string, SymbolEdge> Visit(IParseTree tree)
        //{
        //    return this.Accept((IRuleNode)tree);  // Use substituted "Accept" here.
        //}

//        public virtual Digraph<string, SymbolEdge> Accept(IRuleNode tree)
//        {
//            // Case on tree type and call visitor
//            if (tree.GetType().Assembly.GetName().Name == "LanguageServer")
//            {
//                return tree.Accept(this);
//            }
//            else
//            {
//                var name = tree.GetType().Name;
//                var rule_index = tree.RuleContext.RuleIndex;
//                var antlr4_rule_name = ANTLRv4Parser.ruleNames[rule_index];
//                var type = typeof(ANTLRv4Parser).Assembly.GetType(antlr4_rule_name);
//                var new_tree = Activator.CreateInstance(type);


//return this.VisitGrammarSpec(tree as ANTLRv4Parser.GrammarSpecContext);

//                // Otherwise, call default.
//                return this.VisitChildren(tree);
//            }
  //      }


        public override Digraph<string, SymbolEdge> VisitActionBlock([NotNull] ANTLRv4Parser.ActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitActionScopeName([NotNull] ANTLRv4Parser.ActionScopeNameContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitAction_([NotNull] ANTLRv4Parser.Action_Context context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitAlternative([NotNull] ANTLRv4Parser.AlternativeContext context)
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

        public override Digraph<string, SymbolEdge> VisitAltList([NotNull] ANTLRv4Parser.AltListContext context)
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

        public override Digraph<string, SymbolEdge> VisitArgActionBlock([NotNull] ANTLRv4Parser.ArgActionBlockContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitAtom([NotNull] ANTLRv4Parser.AtomContext context)
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

        public override Digraph<string, SymbolEdge> VisitBlock([NotNull] ANTLRv4Parser.BlockContext context)
        {
            var cg = this.VisitAltList(context.altList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitBlockSet([NotNull] ANTLRv4Parser.BlockSetContext context)
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

        public override Digraph<string, SymbolEdge> VisitBlockSuffix([NotNull] ANTLRv4Parser.BlockSuffixContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitChannelsSpec([NotNull] ANTLRv4Parser.ChannelsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitCharacterRange([NotNull] ANTLRv4Parser.CharacterRangeContext context)
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

        public override Digraph<string, SymbolEdge> VisitDelegateGrammar([NotNull] ANTLRv4Parser.DelegateGrammarContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitDelegateGrammars([NotNull] ANTLRv4Parser.DelegateGrammarsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitEbnf([NotNull] ANTLRv4Parser.EbnfContext context)
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
            }
            return g;
        }

        public override Digraph<string, SymbolEdge> VisitEbnfSuffix([NotNull] ANTLRv4Parser.EbnfSuffixContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitElement([NotNull] ANTLRv4Parser.ElementContext context)
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

        public override Digraph<string, SymbolEdge> VisitElementOption([NotNull] ANTLRv4Parser.ElementOptionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitElementOptions([NotNull] ANTLRv4Parser.ElementOptionsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitErrorNode(IErrorNode node)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitExceptionGroup([NotNull] ANTLRv4Parser.ExceptionGroupContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitExceptionHandler([NotNull] ANTLRv4Parser.ExceptionHandlerContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitFinallyClause([NotNull] ANTLRv4Parser.FinallyClauseContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitGrammarDecl([NotNull] ANTLRv4Parser.GrammarDeclContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitGrammarSpec([NotNull] ANTLRv4Parser.GrammarSpecContext context)
        {
            return this.Visit(context.rules());
        }

        public override Digraph<string, SymbolEdge> VisitGrammarType([NotNull] ANTLRv4Parser.GrammarTypeContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitIdentifier([NotNull] ANTLRv4Parser.IdentifierContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitIdList([NotNull] ANTLRv4Parser.IdListContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLabeledAlt([NotNull] ANTLRv4Parser.LabeledAltContext context)
        {
            var cg = this.VisitAlternative(context.alternative());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLabeledElement([NotNull] ANTLRv4Parser.LabeledElementContext context)
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

        public override Digraph<string, SymbolEdge> VisitLabeledLexerElement([NotNull] ANTLRv4Parser.LabeledLexerElementContext context)
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

        public override Digraph<string, SymbolEdge> VisitLexerAlt([NotNull] ANTLRv4Parser.LexerAltContext context)
        {
            var cg = this.VisitLexerElements(context.lexerElements());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLexerAltList([NotNull] ANTLRv4Parser.LexerAltListContext context)
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

        public override Digraph<string, SymbolEdge> VisitLexerAtom([NotNull] ANTLRv4Parser.LexerAtomContext context)
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

        public override Digraph<string, SymbolEdge> VisitLexerBlock([NotNull] ANTLRv4Parser.LexerBlockContext context)
        {
            var cg = this.VisitLexerAltList(context.lexerAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommand([NotNull] ANTLRv4Parser.LexerCommandContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommandExpr([NotNull] ANTLRv4Parser.LexerCommandExprContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommandName([NotNull] ANTLRv4Parser.LexerCommandNameContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerCommands([NotNull] ANTLRv4Parser.LexerCommandsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitLexerElement([NotNull] ANTLRv4Parser.LexerElementContext context)
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

        public override Digraph<string, SymbolEdge> VisitLexerElements([NotNull] ANTLRv4Parser.LexerElementsContext context)
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

        public override Digraph<string, SymbolEdge> VisitLexerRuleBlock([NotNull] ANTLRv4Parser.LexerRuleBlockContext context)
        {
            var cg = this.VisitLexerAltList(context.lexerAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLexerRuleSpec([NotNull] ANTLRv4Parser.LexerRuleSpecContext context)
        {
            var cg = this.VisitLexerRuleBlock(context.lexerRuleBlock());
            Rules[context.TOKEN_REF().GetText()] = cg;
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitLocalsSpec([NotNull] ANTLRv4Parser.LocalsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitModeSpec([NotNull] ANTLRv4Parser.ModeSpecContext context)
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

        public override Digraph<string, SymbolEdge> VisitNotSet([NotNull] ANTLRv4Parser.NotSetContext context)
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

        public override Digraph<string, SymbolEdge> VisitOption([NotNull] ANTLRv4Parser.OptionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitOptionsSpec([NotNull] ANTLRv4Parser.OptionsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitOptionValue([NotNull] ANTLRv4Parser.OptionValueContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitParserRuleSpec([NotNull] ANTLRv4Parser.ParserRuleSpecContext context)
        {
            var cg = this.VisitRuleBlock(context.ruleBlock());
            Rules[context.RULE_REF().GetText()] = cg;
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitPrequelConstruct([NotNull] ANTLRv4Parser.PrequelConstructContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleAction([NotNull] ANTLRv4Parser.RuleActionContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleAltList([NotNull] ANTLRv4Parser.RuleAltListContext context)
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

        public override Digraph<string, SymbolEdge> VisitRuleBlock([NotNull] ANTLRv4Parser.RuleBlockContext context)
        {
            var cg = this.VisitRuleAltList(context.ruleAltList());
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitRuleModifier([NotNull] ANTLRv4Parser.RuleModifierContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleModifiers([NotNull] ANTLRv4Parser.RuleModifiersContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRulePrequel([NotNull] ANTLRv4Parser.RulePrequelContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRuleref([NotNull] ANTLRv4Parser.RulerefContext context)
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

        public override Digraph<string, SymbolEdge> VisitRuleReturns([NotNull] ANTLRv4Parser.RuleReturnsContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitRules([NotNull] ANTLRv4Parser.RulesContext context)
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

        public override Digraph<string, SymbolEdge> VisitRuleSpec([NotNull] ANTLRv4Parser.RuleSpecContext context)
        {
            var cg = this.Visit(context.GetChild(0));
            return cg;
        }

        public override Digraph<string, SymbolEdge> VisitSetElement([NotNull] ANTLRv4Parser.SetElementContext context)
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

        public override Digraph<string, SymbolEdge> VisitTerminal([NotNull] ANTLRv4Parser.TerminalContext context)
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

        public override Digraph<string, SymbolEdge> VisitThrowsSpec([NotNull] ANTLRv4Parser.ThrowsSpecContext context)
        {
            return null;
        }

        public override Digraph<string, SymbolEdge> VisitTokensSpec([NotNull] ANTLRv4Parser.TokensSpecContext context)
        {
            return null;
        }
    }

    public class Thompson
    {
        IEnumerable<IParseTree> _nodes;
        public Thompson(IEnumerable<IParseTree> nodes)
        {
            _nodes = nodes;
        }

        public string Construct(List<string> lhs_symbols = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var node in _nodes)
            {
                var ag = new AntlrGraph();
                var tree = Class1.ConvertAltAntlrToAntlr4(new List<IParseTree>() { node });
                var res = ag.Visit(tree.First());
                sb.AppendLine(res.ToDotString());
            }
            return sb.ToString();
        }
    }
}
