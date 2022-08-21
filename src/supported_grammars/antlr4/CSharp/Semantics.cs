using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Domemtech.Symtab;
using System;
using System.Collections.Generic;
using System.Linq;

public class Semantics
{
	Dictionary<IParseTree, IList<CombinedScopeSymbol>> st = new Dictionary<IParseTree, IList<CombinedScopeSymbol>>();

	public string GrammarName { get; } = "Antlr4";

	public string FileExtensions { get; } = ".g4;.g";

	public List<string> Classes = new List<string>() {
			"nonterminal def",
			"nonterminal ref",
			"terminal def",
			"terminal ref",
			"comment",
			"keyword",
			"literal",
			"mode def",
			"mode ref",
			"channel def",
			"channel ref",
			"punctuation",
			"operator",
		};

	private enum ClassesEnum : int
	{
		ClassificationNonterminalDef = 0,
		ClassificationNonterminalRef,
		ClassificationTerminalDef,
		ClassificationTerminalRef,
		ClassificationComment,
		ClassificationKeyword,
		ClassificationLiteral,
		ClassificationModeDef,
		ClassificationModeRef,
		ClassificationChannelDef,
		ClassificationChannelRef,
		ClassificationPunctuation,
		ClassificationOperator,
	}

	public List<bool> CanFindAllRefs { get; } = new List<bool>()
	{
		true, // nonterminal
		true, // nonterminal
		true, // Terminal
		true, // Terminal
		false, // comment
		false, // keyword
		true, // literal
		true, // mode
		true, // mode
		true, // channel
		true, // channel
		false, // punctuation
		false, // operator
	};

	public List<bool> CanGotodef { get; } = new List<bool>()
	{
		true, // nonterminal
		true, // nonterminal
		true, // Terminal
		true, // Terminal
		false, // comment
		false, // keyword
		false, // literal
		true, // mode
		true, // mode
		true, // channel
		true, // channel
		false, // punctuation
		false, // operator
	};

	public List<bool> CanGotovisitor { get; } = new List<bool>()
	{
		true, // nonterminal
		true, // nonterminal
		false, // Terminal
		false, // Terminal
		false, // comment
		false, // keyword
		false, // literal
		false, // mode
		false, // mode
		false, // channel
		false, // channel
		false, // punctuation
		false, // operator
	};

	public bool CanNextRule
	{
		get
		{
			return true;
		}
	}

	public List<bool> CanRename { get; } = new List<bool>()
	{
		true, // nonterminal
		true, // nonterminal
		true, // Terminal
		true, // Terminal
		false, // comment
		false, // keyword
		false, // literal
		true, // mode
		true, // mode
		true, // channel
		true, // channel
		false, // punctuation
		false, // operator
	};

	public bool CanReformat
	{
		get
		{
			return true;
		}
	}

	public int Classify(TerminalNodeImpl term)
	{
		Antlr4.Runtime.Tree.IParseTree p = term;
		st.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
		if (list_value != null)
		{
			// There's a symbol table entry for the leaf node.
			// So, it is either a terminal, nonterminal,
			// channel, mode.
			// We don't care if it's a defining occurrence or
			// applied occurrence, just what type of symbol it
			// is.
			foreach (CombinedScopeSymbol value in list_value)
			{
				if (value is RefSymbol)
				{
					List<ISymbol> defs = ((RefSymbol)value).Def;
					foreach (var d in defs)
					{
						if (d is NonterminalSymbol)
						{
							return (int)ClassesEnum.ClassificationNonterminalRef;
						}
						else if (d is TerminalSymbol)
						{
							return (int)ClassesEnum.ClassificationNonterminalRef;
						}
						else if (d is ModeSymbol)
						{
							return (int)ClassesEnum.ClassificationModeRef; ;
						}
						else if (d is ChannelSymbol)
						{
							return (int)ClassesEnum.ClassificationChannelRef; ;
						}
					}
				}
				else if (value is NonterminalSymbol)
				{
					return (int)ClassesEnum.ClassificationNonterminalDef;
				}
				else if (value is TerminalSymbol)
				{
					return (int)ClassesEnum.ClassificationTerminalDef;
				}
				else if (value is ModeSymbol)
				{
					return (int)ClassesEnum.ClassificationModeDef;
				}
				else if (value is ChannelSymbol)
				{
					return (int)ClassesEnum.ClassificationChannelDef;
				}
			}
		}
		else
		{
			// It is either a keyword, literal, comment.
			string text = term.GetText();
			if (_antlr_keywords.Contains(text))
			{
				return (int)ClassesEnum.ClassificationKeyword;
			}
			if ((term.Symbol.Type == ANTLRv4Parser.STRING_LITERAL
				 || term.Symbol.Type == ANTLRv4Parser.INT
				 || term.Symbol.Type == ANTLRv4Parser.LEXER_CHAR_SET))
			{
				return (int)ClassesEnum.ClassificationLiteral;
			}
			// The token could be part of parserRuleSpec context.
			//for (IRuleNode r = term.Parent; r != null; r = r.Parent)
			//{
			//    if (r is ANTLRv4Parser.ParserRuleSpecContext ||
			//          r is ANTLRv4Parser.LexerRuleSpecContext)
			//    {
			//        return 4;
			//    }
			//}
			if (term.Payload.Channel == ANTLRv4Lexer.OFF_CHANNEL
				|| term.Symbol.Type == ANTLRv4Lexer.DOC_COMMENT
				|| term.Symbol.Type == ANTLRv4Lexer.BLOCK_COMMENT
				|| term.Symbol.Type == ANTLRv4Lexer.LINE_COMMENT)
			{
				return (int)ClassesEnum.ClassificationComment;
			}
		}
		return -1;
	}

	private static readonly List<string> _antlr_keywords = new List<string>() {
			"options",
			"tokens",
			"channels",
			"import",
			"fragment",
			"lexer",
			"parser",
			"grammar",
			"protected",
			"public",
			"returns",
			"locals",
			"throws",
			"catch",
			"finally",
			"mode",
			"pushMode",
			"popMode",
			"type",
			"skip",
			"channel"
		};



    public class Pass0Listener : ANTLRv4ParserBaseListener
    {
        private readonly ParsingResults _pd;
        private bool saw_tokenVocab_option = false;
        private enum GrammarType
        {
            Combined,
            Parser,
            Lexer
        }

        private GrammarType Type;

        public Pass0Listener(ParsingResults pd)
        {
            _pd = pd;
            if (!ParsingResults.InverseImports.ContainsKey(_pd.FullFileName))
            {
                ParsingResults.InverseImports.Add(_pd.FullFileName, new HashSet<string>());
            }
        }

        public override void EnterGrammarType([NotNull] ANTLRv4Parser.GrammarTypeContext context)
        {
            if (context.GetChild(0).GetText() == "parser")
            {
                Type = GrammarType.Parser;
            }
            else if (context.GetChild(0).GetText() == "lexer")
            {
                Type = GrammarType.Lexer;
            }
            else
            {
                Type = GrammarType.Combined;
            }
        }

        public override void EnterOption([NotNull] ANTLRv4Parser.OptionContext context)
        {
            if (context.ChildCount < 3)
            {
                return;
            }

            if (context.GetChild(0) == null)
            {
                return;
            }

            if (context.GetChild(0).GetText() != "tokenVocab")
            {
                return;
            }

            string dep_grammar = context.GetChild(2).GetText();
            string file = _pd.Item.FullPath;
            string dir = System.IO.Path.GetDirectoryName(file);
            string dep = dir + System.IO.Path.DirectorySeparatorChar + dep_grammar + ".g4";
            dep = Workspaces.Util.GetProperFilePathCapitalization(dep);
            if (dep == null)
            {
                return;
            }

            _pd.Imports.Add(dep);
            if (!ParsingResults.InverseImports.ContainsKey(dep))
            {
                ParsingResults.InverseImports.Add(dep, new HashSet<string>());
            }

            bool found = false;
            foreach (string f in ParsingResults.InverseImports[dep])
            {
                if (f == file)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                ParsingResults.InverseImports[dep].Add(file);
            }
            saw_tokenVocab_option = true;
        }

        public override void EnterDelegateGrammar([NotNull] ANTLRv4Parser.DelegateGrammarContext context)
        {
            if (context.ChildCount < 1)
            {
                return;
            }

            if (context.GetChild(0) == null)
            {
                return;
            }

            string dep_grammar = context.GetChild(0).GetText();
            string file = _pd.Item.FullPath;
            string dir = System.IO.Path.GetDirectoryName(file);
            string dep = dir + System.IO.Path.DirectorySeparatorChar + dep_grammar + ".g4";
            dep = Workspaces.Util.GetProperFilePathCapitalization(dep);
            if (dep == null)
            {
                return;
            }

            _pd.Imports.Add(dep);
            if (!ParsingResults.InverseImports.ContainsKey(dep))
            {
                ParsingResults.InverseImports.Add(dep, new HashSet<string>());
            }

            bool found = false;
            foreach (string f in ParsingResults.InverseImports[dep])
            {
                if (f == file)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                ParsingResults.InverseImports[dep].Add(file);
            }
        }

        public override void EnterRules([NotNull] ANTLRv4Parser.RulesContext context)
        {
            if (saw_tokenVocab_option)
            {
                return;
            }

            if (Type == GrammarType.Lexer)
            {
                string file = _pd.Item.FullPath;
                string dep = file.Replace("Lexer.g4", "Parser.g4");
                if (dep == file)
                {
                    // If the file is not named correctly so that it ends in Parser.g4,
                    // then it's probably a mistake. I don't know where to get the lexer
                    // grammar.
                    return;
                }
                if (!ParsingResults.InverseImports.ContainsKey(dep))
                {
                    ParsingResults.InverseImports.Add(dep, new HashSet<string>());
                }

                bool found = false;
                foreach (string f in ParsingResults.InverseImports[dep])
                {
                    if (f == file)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    ParsingResults.InverseImports[file].Add(dep);
                }
            }
            if (Type == GrammarType.Parser)
            {
                // It's a parser grammar, but we didn't see the tokenVocab option for the lexer.
                // We must assume a lexer grammar in this directory.
                // BUT!!!! There could be many things wrong here, so just don't do this willy nilly.

                string file = _pd.Item.FullPath;
                string dep = file.Replace("Parser.g4", "Lexer.g4");
                if (dep == file)
                {
                    // If the file is not named correctly so that it ends in Parser.g4,
                    // then it's probably a mistake. I don't know where to get the lexer
                    // grammar.
                    return;
                }

                string dir = System.IO.Path.GetDirectoryName(file);
                _pd.Imports.Add(dep);
                if (!ParsingResults.InverseImports.ContainsKey(dep))
                {
                    ParsingResults.InverseImports.Add(dep, new HashSet<string>());
                }

                bool found = false;
                foreach (string f in ParsingResults.InverseImports[dep])
                {
                    if (f == file)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    ParsingResults.InverseImports[dep].Add(file);
                }
            }
        }
    }

    public class Pass2Listener : ANTLRv4ParserBaseListener
    {
        private readonly ParsingResults _pd;

        public Pass2Listener(ParsingResults pd)
        {
            _pd = pd;
        }

        public IParseTree NearestScope(IParseTree node)
        {
            for (; node != null; node = node.Parent)
            {
                _pd.Attributes.TryGetValue(node, out IList<CombinedScopeSymbol> list);
                if (list != null)
                {
                    if (list.Count == 1 && list[0] is IScope)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public IScope GetScope(IParseTree node)
        {
            if (node == null)
            {
                return null;
            }

            _pd.Attributes.TryGetValue(node, out IList<CombinedScopeSymbol> list);
            if (list != null)
            {
                if (list.Count == 1 && list[0] is IScope)
                {
                    return list[0] as IScope;
                }
            }
            return null;
        }

        public override void EnterGrammarSpec([NotNull] ANTLRv4Parser.GrammarSpecContext context)
        {
            _pd.Attributes[context] = new List<CombinedScopeSymbol>() { (CombinedScopeSymbol)_pd.RootScope };
        }

        public override void EnterParserRuleSpec([NotNull] ANTLRv4Parser.ParserRuleSpecContext context)
        {
            int i;
            for (i = 0; i < context.ChildCount; ++i)
            {
                if (!(context.GetChild(i) is TerminalNodeImpl))
                {
                    continue;
                }

                TerminalNodeImpl c = context.GetChild(i) as TerminalNodeImpl;
                if (c.Symbol.Type == ANTLRv4Lexer.RULE_REF)
                {
                    break;
                }
            }
            if (i == context.ChildCount)
            {
                return;
            }

            TerminalNodeImpl rule_ref = context.GetChild(i) as TerminalNodeImpl;
            string id = rule_ref.GetText();
            ISymbol sym = new NonterminalSymbol(id, new List<IToken>() { rule_ref.Symbol });
            _pd.RootScope.define(ref sym);
            CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
            _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
            _pd.Attributes[context.GetChild(i)] = new List<CombinedScopeSymbol>() { s };
        }

        public override void EnterLexerRuleSpec([NotNull] ANTLRv4Parser.LexerRuleSpecContext context)
        {
            int i;
            for (i = 0; i < context.ChildCount; ++i)
            {
                if (!(context.GetChild(i) is TerminalNodeImpl))
                {
                    continue;
                }

                TerminalNodeImpl c = context.GetChild(i) as TerminalNodeImpl;
                if (c.Symbol.Type == ANTLRv4Lexer.TOKEN_REF)
                {
                    break;
                }
            }
            if (i == context.ChildCount)
            {
                return;
            }

            TerminalNodeImpl token_ref = context.GetChild(i) as TerminalNodeImpl;
            string id = token_ref.GetText();
            ISymbol sym = new TerminalSymbol(id, new List<IToken>() { token_ref.Symbol });
            _pd.RootScope.define(ref sym);
            CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
            _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
            _pd.Attributes[context.GetChild(i)] = new List<CombinedScopeSymbol>() { s };
        }

        public override void EnterIdentifier([NotNull] ANTLRv4Parser.IdentifierContext context)
        {
            if (context.Parent is ANTLRv4Parser.ModeSpecContext)
            {
                TerminalNodeImpl term = context.GetChild(0) as TerminalNodeImpl;
                string id = term.GetText();
                ISymbol sym = new ModeSymbol(id, new List<IToken>() { term.Symbol });
                _pd.RootScope.define(ref sym);
                CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
                _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
                _pd.Attributes[context.GetChild(0)] = new List<CombinedScopeSymbol>() { s };
            }
            else if (context.Parent is ANTLRv4Parser.IdListContext && context.Parent?.Parent is ANTLRv4Parser.ChannelsSpecContext)
            {
                TerminalNodeImpl term = context.GetChild(0) as TerminalNodeImpl;
                string id = term.GetText();
                ISymbol sym = new ChannelSymbol(id, new List<IToken>() { term.Symbol });
                _pd.RootScope.define(ref sym);
                CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
                _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
                _pd.Attributes[term] = new List<CombinedScopeSymbol>() { s };
            }
            else
            {
                var p = context.Parent;
                var add_def = false;
                for (; p != null; p = p.Parent)
                {
                    if (p is ANTLRv4Parser.TokensSpecContext)
                    {
                        add_def = true;
                        break;
                    }
                }
                if (add_def)
                {
                    TerminalNodeImpl term = context.GetChild(0) as TerminalNodeImpl;
                    string id = term.GetText();
                    ISymbol sym = new TerminalSymbol(id, new List<IToken>() { term.Symbol });
                    _pd.RootScope.define(ref sym);
                    CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
                    _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
                    _pd.Attributes[term] = new List<CombinedScopeSymbol>() { s };
                }
            }
        }
    }

    public class Pass3Listener : ANTLRv4ParserBaseListener
    {
        private readonly ParsingResults _pd;

        public Pass3Listener(ParsingResults pd)
        {
            _pd = pd;
        }

        public override void EnterTerminal([NotNull] ANTLRv4Parser.TerminalContext context)
        {
            if (context.TOKEN_REF() != null)
            {
                string id = context.TOKEN_REF().GetText();
                List<ISymbol> list = _pd.RootScope.LookupType(id).ToList();
                if (!list.Any())
                {
                    ISymbol sym = new TerminalSymbol(id, new List<IToken>() { context.TOKEN_REF().Symbol });
                    _pd.RootScope.define(ref sym);
                    list = _pd.RootScope.LookupType(id).ToList();
                }
                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { context.TOKEN_REF().Symbol }, list);
                new_attrs.Add(s);
                _pd.Attributes[context.TOKEN_REF()] = new_attrs;
            }
        }

        public override void EnterRuleref([NotNull] ANTLRv4Parser.RulerefContext context)
        {
            TerminalNodeImpl first = context.RULE_REF() as TerminalNodeImpl;
            string id = first.GetText();
            List<ISymbol> list = _pd.RootScope.LookupType(id).ToList();
            if (!list.Any())
            {
                ISymbol sym = new NonterminalSymbol(id, new List<IToken>() { first.Symbol });
                _pd.RootScope.define(ref sym);
                list = _pd.RootScope.LookupType(id).ToList();
            }
            List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
            CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { first.Symbol }, list);
            new_attrs.Add(s);
            _pd.Attributes[first] = new_attrs;
        }

        public override void EnterIdentifier([NotNull] ANTLRv4Parser.IdentifierContext context)
        {
            if (context.Parent is ANTLRv4Parser.LexerCommandExprContext && context.Parent.Parent is ANTLRv4Parser.LexerCommandContext)
            {
                ANTLRv4Parser.LexerCommandContext lc = context.Parent.Parent as ANTLRv4Parser.LexerCommandContext;
                if (lc.GetChild(0)?.GetChild(0)?.GetText() == "pushMode")
                {
                    TerminalNodeImpl term = context.GetChild(0) as TerminalNodeImpl;
                    string id = term.GetText();
                    List<ISymbol> sym_list = _pd.RootScope.LookupType(id).ToList();
                    if (!sym_list.Any())
                    {
                        ISymbol sym = new ModeSymbol(id, null);
                        _pd.RootScope.define(ref sym);
                        sym_list = _pd.RootScope.LookupType(id).ToList();
                    }
                    List<CombinedScopeSymbol> ref_list = new List<CombinedScopeSymbol>();
                    CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { term.Symbol }, sym_list);
                    ref_list.Add(s);
                    _pd.Attributes[context] = ref_list;
                    _pd.Attributes[context.GetChild(0)] = ref_list;
                }
                else if (lc.GetChild(0)?.GetChild(0)?.GetText() == "channel")
                {
                    TerminalNodeImpl term = context.GetChild(0) as TerminalNodeImpl;
                    string id = term.GetText();
                    List<ISymbol> sym_list = _pd.RootScope.LookupType(id).ToList();
                    if (!sym_list.Any())
                    {
                        ISymbol sym = new ChannelSymbol(id, null);
                        _pd.RootScope.define(ref sym);
                        sym_list = _pd.RootScope.LookupType(id).ToList();
                    }
                    List<CombinedScopeSymbol> ref_list = new List<CombinedScopeSymbol>();
                    CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { term.Symbol }, sym_list);
                    ref_list.Add(s);
                    _pd.Attributes[context] = ref_list;
                    _pd.Attributes[context.GetChild(0)] = ref_list;
                }
                else if (lc.GetChild(0)?.GetChild(0)?.GetText() == "type")
                {
                    TerminalNodeImpl term = context.GetChild(0) as TerminalNodeImpl;
                    string id = term.GetText();
                    List<ISymbol> sym_list = _pd.RootScope.LookupType(id).ToList();
                    if (!sym_list.Any())
                    {
                        ISymbol sym = new TerminalSymbol(id, null);
                        _pd.RootScope.define(ref sym);
                        sym_list = _pd.RootScope.LookupType(id).ToList();
                    }
                    List<CombinedScopeSymbol> ref_list = new List<CombinedScopeSymbol>();
                    CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { term.Symbol }, sym_list);
                    ref_list.Add(s);
                    _pd.Attributes[context] = ref_list;
                    _pd.Attributes[context.GetChild(0)] = ref_list;
                }
            }
        }
    }
}

