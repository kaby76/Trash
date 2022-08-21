using AltAntlr;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageServer
{
    class Class1
    {
        public static IEnumerable<IParseTree> ConvertAltAntlrToAntlr4(IEnumerable<IParseTree> nodes)
        {
            List<IParseTree> result = new List<IParseTree>();
            foreach (var root in nodes)
            {
                var stack = new Stack<Tuple<IParseTree, IParseTree>>();
                stack.Push(new Tuple<IParseTree, IParseTree>(root, null));
                bool first = true;
                while (stack.Any())
                {
                    var t = stack.Pop();
                    var v = t.Item1;
                    var p = t.Item2;
                    if (!(v is TerminalNodeImpl))
                    {
                        IParseTree converted = ConvertSingle(v, p);
                        if (first)
                        {
                            result.Add(converted);
                            first = false;
                        }
                        for (var i = v.ChildCount-1; i >= 0; --i)
                        {
                            var oo = v.GetChild(i);
                            stack.Push(new Tuple<IParseTree, IParseTree>(oo, converted));
                        }
                    }
                    else
                    {
                        IParseTree converted = ConvertSingleTerminal(v, p);
                        if (first)
                        {
                            result.Add(converted);
                            first = false;
                        }
                    }
                }
            }
            return result;
        }

        private static IParseTree ConvertSingleTerminal(IParseTree node, IParseTree parent)
        {
            TerminalNodeImpl n = node as TerminalNodeImpl;
            var p = parent as ParserRuleContext;
            var token = new CommonToken(n.Symbol);
            var c = new TerminalNodeImpl(token);
            if (p == null) return c;
            if (p.children == null) p.children = new List<IParseTree>() { c };
            else p.children.Add(c);
            return c;
            //new_ap_rule.AddChild(new_semi);
            //new_semi.Parent = new_ap_rule;
        }

        private static IParseTree ConvertSingle(IParseTree node, IParseTree parent)
        {
            var r = node as MyParserRuleContext;
            var p = parent as Antlr4.Runtime.ParserRuleContext;
            IParseTree c = null;
            switch (r.RuleIndex)
            {
                case ANTLRv4Parser.RULE_actionBlock:
                    c = new ANTLRv4Parser.ActionBlockContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_actionScopeName:
                    c = new ANTLRv4Parser.ActionScopeNameContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_action_:
                    c = new ANTLRv4Parser.Action_Context(p, 0);
                    break;
                case ANTLRv4Parser.RULE_alternative:
                    c = new ANTLRv4Parser.AlternativeContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_altList:
                    c = new ANTLRv4Parser.AltListContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_argActionBlock:
                    c = new ANTLRv4Parser.ArgActionBlockContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_atom:
                    c = new ANTLRv4Parser.AtomContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_block:
                    c = new ANTLRv4Parser.BlockContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_blockSet:
                    c = new ANTLRv4Parser.BlockSetContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_blockSuffix:
                    c = new ANTLRv4Parser.BlockSuffixContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_channelsSpec:
                    c = new ANTLRv4Parser.ChannelsSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_characterRange:
                    c = new ANTLRv4Parser.CharacterRangeContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_delegateGrammar:
                    c = new ANTLRv4Parser.DelegateGrammarContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_delegateGrammars:
                    c = new ANTLRv4Parser.DelegateGrammarsContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ebnf:
                    c = new ANTLRv4Parser.EbnfContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ebnfSuffix:
                    c = new ANTLRv4Parser.EbnfSuffixContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_element:
                    c = new ANTLRv4Parser.ElementContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_elementOption:
                    c = new ANTLRv4Parser.ElementOptionContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_elementOptions:
                    c = new ANTLRv4Parser.ElementOptionsContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_exceptionGroup:
                    c = new ANTLRv4Parser.ExceptionGroupContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_exceptionHandler:
                    c = new ANTLRv4Parser.ExceptionHandlerContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_finallyClause:
                    c = new ANTLRv4Parser.FinallyClauseContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_grammarDecl:
                    c = new ANTLRv4Parser.GrammarDeclContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_grammarSpec:
                    c = new ANTLRv4Parser.GrammarSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_grammarType:
                    c = new ANTLRv4Parser.GrammarTypeContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_identifier:
                    c = new ANTLRv4Parser.IdentifierContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_idList:
                    c = new ANTLRv4Parser.IdListContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_labeledAlt:
                    c = new ANTLRv4Parser.LabeledAltContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_labeledElement:
                    c = new ANTLRv4Parser.LabeledElementContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_labeledLexerElement:
                    c = new ANTLRv4Parser.LabeledLexerElementContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerAlt:
                    c = new ANTLRv4Parser.LexerAltContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerAltList:
                    c = new ANTLRv4Parser.LexerAltListContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerAtom:
                    c = new ANTLRv4Parser.LexerAtomContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerBlock:
                    c = new ANTLRv4Parser.LexerBlockContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerCommand:
                    c = new ANTLRv4Parser.LexerCommandContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerCommandExpr:
                    c = new ANTLRv4Parser.LexerCommandExprContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerCommandName:
                    c = new ANTLRv4Parser.LexerCommandNameContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerCommands:
                    c = new ANTLRv4Parser.LexerCommandsContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerElement:
                    c = new ANTLRv4Parser.LexerElementContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerElements:
                    c = new ANTLRv4Parser.LexerElementsContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerRuleBlock:
                    c = new ANTLRv4Parser.LexerRuleBlockContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_lexerRuleSpec:
                    c = new ANTLRv4Parser.LexerRuleSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_localsSpec:
                    c = new ANTLRv4Parser.LocalsSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_modeSpec:
                    c = new ANTLRv4Parser.ModeSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_notSet:
                    c = new ANTLRv4Parser.NotSetContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_option:
                    c = new ANTLRv4Parser.OptionContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_optionsSpec:
                    c = new ANTLRv4Parser.OptionsSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_optionValue:
                    c = new ANTLRv4Parser.OptionValueContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_parserRuleSpec:
                    c = new ANTLRv4Parser.ParserRuleSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_prequelConstruct:
                    c = new ANTLRv4Parser.PrequelConstructContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleAction:
                    c = new ANTLRv4Parser.RuleActionContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleAltList:
                    c = new ANTLRv4Parser.RuleAltListContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleBlock:
                    c = new ANTLRv4Parser.RuleBlockContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleModifier:
                    c = new ANTLRv4Parser.RuleModifierContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleModifiers:
                    c = new ANTLRv4Parser.RuleModifiersContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_rulePrequel:
                    c = new ANTLRv4Parser.RulePrequelContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleref:
                    c = new ANTLRv4Parser.RulerefContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleReturns:
                    c = new ANTLRv4Parser.RuleReturnsContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_rules:
                    c = new ANTLRv4Parser.RulesContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_ruleSpec:
                    c = new ANTLRv4Parser.RuleSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_setElement:
                    c = new ANTLRv4Parser.SetElementContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_terminal:
                    c = new ANTLRv4Parser.TerminalContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_throwsSpec:
                    c = new ANTLRv4Parser.ThrowsSpecContext(p, 0);
                    break;
                case ANTLRv4Parser.RULE_tokensSpec:
                    c = new ANTLRv4Parser.TokensSpecContext(p, 0);
                    break;
                default:
                    throw new Exception("unknown case");
            }
            if (p == null) return c;
            if (p.children == null) p.children = new List<IParseTree>() { c };
            else p.children.Add(c);
            return c;
        }
    }
}
