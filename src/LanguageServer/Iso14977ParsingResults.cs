namespace LanguageServer
{
    using AltAntlr;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Domemtech.Symtab;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Workspaces;

    internal class Iso14977ParsingResults : ParsingResults, IParserDescription
    {
        public Iso14977ParsingResults(Document item) : base(item)
        {
            Passes.Add(() =>
            {
                int before_count = 0;
                if (!ParsingResults.InverseImports.ContainsKey(this.FullFileName))
                {
                    ParsingResults.InverseImports.Add(this.FullFileName, new HashSet<string>());
                }
                foreach (KeyValuePair<string, HashSet<string>> x in ParsingResults.InverseImports)
                {
                    before_count++;
                    before_count = before_count + x.Value.Count;
                }
                if (ParseTree == null) return false;
                int after_count = 0;
                foreach (KeyValuePair<string, HashSet<string>> dep in ParsingResults.InverseImports)
                {
                    string name = dep.Key;
                    Workspaces.Document x = item.Workspace.FindDocument(name);
                    if (x == null)
                    {
                        // Add document.
                        Workspaces.Container proj = Item.Parent;
                        Workspaces.Document new_doc = new Workspaces.Document(name);
                        proj.AddChild(new_doc);
                        after_count++;
                    }
                    after_count++;
                    after_count = after_count + dep.Value.Count;
                }
                return before_count != after_count;
            });
            Passes.Add(() =>
            {
                // The workspace is completely loaded. Create scopes for all files in workspace
                // if they don't already exist.
                foreach (KeyValuePair<string, HashSet<string>> dep in InverseImports)
                {
                    string name = dep.Key;
                    _scopes.TryGetValue(name, out IScope file_scope);
                    if (file_scope != null)
                    {
                        continue;
                    }

                    _scopes[name] = new FileScope(name, null);
                }

                // Set up search path scopes for Imports relationship.
                IScope root = _scopes[FullFileName];
                foreach (string dep in Imports)
                {
                    // Don't add if already have this search path.
                    IScope dep_scope = _scopes[dep];
                    bool found = false;
                    foreach (IScope scope in root.NestedScopes)
                    {
                        if (scope is SearchPathScope)
                        {
                            SearchPathScope spc = scope as SearchPathScope;
                            if (spc.NestedScopes.First() == dep_scope)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        SearchPathScope import = new SearchPathScope(root);
                        import.nest(dep_scope);
                        root.nest(import);
                    }
                }
                root.empty();
                RootScope = root;
                return false;
            });
            Passes.Add(() =>
            {
                if (ParseTree == null) return false;
                ParseTreeWalker.Default.Walk(new Pass2Listener(this), ParseTree);
                return false;
            });
            Passes.Add(() =>
            {
                if (ParseTree == null) return false;
                ParseTreeWalker.Default.Walk(new Pass3Listener(this), ParseTree);
                return false;
            });
        }

        public override List<bool> CanFindAllRefs { get; } = new List<bool>()
        {
            true, // nonterminal
            true, // nonterminal
            true, // Terminal
            true, // Terminal
            false, // comment
            false, // keyword
			false, // mode
			false, // mode
			false, // channel
			false, // channel
            false, // literal
            false, // punctuation
            false, // operator
        };
        public override List<bool> CanGotodef { get; } = new List<bool>()
        {
            true, // nonterminal
            true, // nonterminal
            true, // Terminal
            true, // Terminal
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
        public override List<bool> CanGotovisitor { get; } = new List<bool>()
        {
            false, // nonterminal
            false, // nonterminal
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
        public override bool CanNextRule
        {
            get
            {
                return true;
            }
        }
        public override bool CanReformat
        {
            get
            {
                return true;
            }
        }
        public override List<bool> CanRename { get; } = new List<bool>()
        {
            true, // nonterminal
            true, // nonterminal
            true, // Terminal
            true, // Terminal
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
        public override Func<IParserDescription, Dictionary<IParseTree, IList<CombinedScopeSymbol>>, IParseTree, int> Classify { get; } =
           (IParserDescription gd, Dictionary<IParseTree, IList<CombinedScopeSymbol>> st, IParseTree t) =>
           {
               TerminalNodeImpl term = t as TerminalNodeImpl;
               Antlr4.Runtime.Tree.IParseTree p = term;
               st.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
               if (list_value != null)
               {
                   // There's a symbol table entry for the leaf node.
                   // So, it is either a terminal, nonterminal.
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
                                   return (int)AntlrClassifications.ClassificationNonterminalRef;
                               }
                               else if (d is TerminalSymbol)
                               {
                                   return (int)AntlrClassifications.ClassificationNonterminalRef;
                               }
                               else if (d is ModeSymbol)
                               {
                                   return (int)AntlrClassifications.ClassificationModeRef;
                               }
                               else if (d is ChannelSymbol)
                               {
                                   return (int)AntlrClassifications.ClassificationChannelRef;
                               }
                           }
                       }
                       else if (value is NonterminalSymbol)
                       {
                           return (int)AntlrClassifications.ClassificationNonterminalDef;
                       }
                       else if (value is TerminalSymbol)
                       {
                           return (int)AntlrClassifications.ClassificationTerminalDef;
                       }
                       else if (value is ModeSymbol)
                       {
                           return (int)AntlrClassifications.ClassificationModeDef;
                       }
                       else if (value is ChannelSymbol)
                       {
                           return (int)AntlrClassifications.ClassificationChannelDef;
                       }
                   }
               }
               else
               {
                        // It is either a keyword, literal, comment.
                        string text = term.GetText();
                   if (_antlr_keywords.Contains(text))
                   {
                       return (int)AntlrClassifications.ClassificationKeyword;
                   }
                   //if ((term.Symbol.Type == Iso14977Parser.STRING
                   //      || term.Symbol.Type == Iso14977Parser.SET
                   //      || term.Symbol.Type == Iso14977Parser.CONSTRAINT))
                   //{
                   //    return (int)AntlrClassifications.ClassificationLiteral;
                   //}
                   //if (
                   //    term.Symbol.Type == Iso14977Parser.COMMENT)
                   //{
                   //    return (int)AntlrClassifications.ClassificationComment;
                   //}
               }
               return -1;
           };
        public override bool DoErrorSquiggles
        {
            get
            {
                return true;
            }
        }
        public override string FileExtension { get; } = ".iso14977";
        public override string[] Map { get; } = new string[]
          {
                 "Antlr - nonterminal def",
                 "Antlr - nonterminal ref",
                 "Antlr - terminal def",
                 "Antlr - terminal ref",
                 "Antlr - comment",
                 "Antlr - keyword",
                 "Antlr - literal",
                 "Antlr - mode def",
                 "Antlr - mode ref",
                 "Antlr - channel def",
                 "Antlr - channel ref",
                 "Antlr - punctuation",
                 "Antlr - operator",
          };
        public override string Name { get; } = "Iso14977";
        public override List<Func<ParsingResults, IParseTree, string>> PopUpDefinition { get; } =
            new List<Func<ParsingResults, IParseTree, string>>()
            {
                        (ParsingResults pd, IParseTree t) => // nonterminal
                        {
                            TerminalNodeImpl term = t as TerminalNodeImpl;
                            if (term == null)
                            {
                                return null;
                            }
                            Antlr4.Runtime.Tree.IParseTree p = term;
                            string dir = System.IO.Path.GetDirectoryName(pd.Item.FullPath);
                            pd.Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
                            if (list_value == null)
                            {
                                return null;
                            }
                            bool first = true;
                            StringBuilder sb = new StringBuilder();
                            foreach (CombinedScopeSymbol value in list_value)
                            {
                                if (value == null)
                                {
                                    continue;
                                }
                                ISymbol sym = value as ISymbol;
                                if (sym == null)
                                {
                                    continue;
                                }
                                List<ISymbol> list_of_syms = new List<ISymbol>() { sym };
                                if (sym is RefSymbol)
                                {
                                    list_of_syms = sym.resolve();
                                }
                                foreach (ISymbol s in list_of_syms)
                                {
                                    if (! first)
                                    {
                                        sb.AppendLine();
                                    }
                                    first = false;
                                    if (s is TerminalSymbol)
                                    {
                                        sb.Append("Terminal ");
                                    }
                                    else if (s is NonterminalSymbol)
                                    {
                                        sb.Append("Nonterminal ");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    string def_file = s.file;
                                    if (def_file == null)
                                    {
                                        continue;
                                    }
                                    Workspaces.Document def_document = pd.Item.Workspace.FindDocument(def_file);
                                    if (def_document == null)
                                    {
                                        continue;
                                    }
                                    ParsingResults def_pd = ParsingResultsFactory.Create(def_document);
                                    if (def_pd == null)
                                    {
                                        continue;
                                    }
                                    IParseTree fod = def_pd.Attributes.Where(
                                            kvp =>
                                            {
                                                IParseTree key = kvp.Key;
                                                if (!(key is TerminalNodeImpl))
                                                    return false;
                                                TerminalNodeImpl t1 = key as TerminalNodeImpl;
                                                IToken s1 = t1.Symbol;
                                                if (s.Token.Contains(s1))
                                                    return true;
                                                return false;
                                            })
                                        .Select(kvp => kvp.Key).FirstOrDefault();
                                    if (fod == null)
                                    {
                                        continue;
                                    }
                                    sb.Append("defined in ");
                                    sb.Append(s.file);
                                    sb.AppendLine();
                                    IParseTree node = fod;
                                    for (; node != null; node = node.Parent)
                                    {
                                        if (node is Iso14977Parser.Syntax_ruleContext)
                                        {
                                            break;
                                        }
                                    }
                                    if (node == null)
                                    {
                                        continue;
                                    }
                                    Reconstruct.Doit(sb, node);
                                }
                            }
                            return sb.ToString();
                        },
                        (ParsingResults pd, IParseTree t) => // nonterminal
                        {
                            TerminalNodeImpl term = t as TerminalNodeImpl;
                            if (term == null)
                            {
                                return null;
                            }
                            Antlr4.Runtime.Tree.IParseTree p = term;
                            string dir = System.IO.Path.GetDirectoryName(pd.Item.FullPath);
                            pd.Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
                            if (list_value == null)
                            {
                                return null;
                            }
                            bool first = true;
                            StringBuilder sb = new StringBuilder();
                            foreach (CombinedScopeSymbol value in list_value)
                            {
                                if (value == null)
                                {
                                    continue;
                                }
                                ISymbol sym = value as ISymbol;
                                if (sym == null)
                                {
                                    continue;
                                }
                                List<ISymbol> list_of_syms = new List<ISymbol>() { sym };
                                if (sym is RefSymbol)
                                {
                                    list_of_syms = sym.resolve();
                                }
                                foreach (ISymbol s in list_of_syms)
                                {
                                    if (! first)
                                    {
                                        sb.AppendLine();
                                    }
                                    first = false;
                                    if (s is TerminalSymbol)
                                    {
                                        sb.Append("Terminal ");
                                    }
                                    else if (s is NonterminalSymbol)
                                    {
                                        sb.Append("Nonterminal ");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    string def_file = s.file;
                                    if (def_file == null)
                                    {
                                        continue;
                                    }
                                    Workspaces.Document def_document = pd.Item.Workspace.FindDocument(def_file);
                                    if (def_document == null)
                                    {
                                        continue;
                                    }
                                    ParsingResults def_pd = ParsingResultsFactory.Create(def_document);
                                    if (def_pd == null)
                                    {
                                        continue;
                                    }
                                    IParseTree fod = def_pd.Attributes.Where(
                                            kvp =>
                                            {
                                                IParseTree key = kvp.Key;
                                                if (!(key is TerminalNodeImpl))
                                                    return false;
                                                TerminalNodeImpl t1 = key as TerminalNodeImpl;
                                                IToken s1 = t1.Symbol;
                                                if (s.Token.Contains(s1))
                                                    return true;
                                                return false;
                                            })
                                        .Select(kvp => kvp.Key).FirstOrDefault();
                                    if (fod == null)
                                    {
                                        continue;
                                    }
                                    sb.Append("defined in ");
                                    sb.Append(s.file);
                                    sb.AppendLine();
                                    IParseTree node = fod;
                                    for (; node != null; node = node.Parent)
                                    {
                                        if (node is Iso14977Parser.Syntax_ruleContext)
                                        {
                                            break;
                                        }
                                    }
                                    if (node == null)
                                    {
                                        continue;
                                    }
                                    Reconstruct.Doit(sb, node);
                                }
                            }
                            return sb.ToString();
                        },
                        (ParsingResults pd, IParseTree t) => // terminal
                        {
                            TerminalNodeImpl term = t as TerminalNodeImpl;
                            if (term == null)
                            {
                                return null;
                            }
                            Antlr4.Runtime.Tree.IParseTree p = term;
                            string dir = System.IO.Path.GetDirectoryName(pd.Item.FullPath);
                            pd.Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
                            if (list_value == null)
                            {
                                return null;
                            }
                            bool first = true;
                            StringBuilder sb = new StringBuilder();
                            foreach (CombinedScopeSymbol value in list_value)
                            {
                                if (value == null)
                                {
                                    continue;
                                }
                                ISymbol sym = value as ISymbol;
                                if (sym == null)
                                {
                                    continue;
                                }
                                List<ISymbol> list_of_syms = new List<ISymbol>() { sym };
                                if (sym is RefSymbol)
                                {
                                    list_of_syms = sym.resolve();
                                }
                                foreach (ISymbol s in list_of_syms)
                                {
                                    if (! first)
                                    {
                                        sb.AppendLine();
                                    }
                                    first = false;
                                    if (s is TerminalSymbol)
                                    {
                                        sb.Append("Terminal ");
                                    }
                                    else if (s is NonterminalSymbol)
                                    {
                                        sb.Append("Nonterminal ");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    string def_file = s.file;
                                    if (def_file == null)
                                    {
                                        continue;
                                    }
                                    Workspaces.Document def_document = pd.Item.Workspace.FindDocument(def_file);
                                    if (def_document == null)
                                    {
                                        continue;
                                    }
                                    ParsingResults def_pd = ParsingResultsFactory.Create(def_document);
                                    if (def_pd == null)
                                    {
                                        continue;
                                    }
                                    IParseTree fod = def_pd.Attributes.Where(
                                            kvp =>
                                            {
                                                IParseTree key = kvp.Key;
                                                if (!(key is TerminalNodeImpl))
                                                    return false;
                                                TerminalNodeImpl t1 = key as TerminalNodeImpl;
                                                IToken s1 = t1.Symbol;
                                                if (s.Token.Contains(s1))
                                                    return true;
                                                return false;
                                            })
                                        .Select(kvp => kvp.Key).FirstOrDefault();
                                    if (fod == null)
                                    {
                                        continue;
                                    }
                                    sb.Append("defined in ");
                                    sb.Append(s.file);
                                    sb.AppendLine();
                                    IParseTree node = fod;
                                    for (; node != null; node = node.Parent)
                                    {
                                        if (node is Iso14977Parser.Syntax_ruleContext)
                                        {
                                            break;
                                        }
                                    }
                                    if (node == null)
                                    {
                                        continue;
                                    }
                                    Reconstruct.Doit(sb, node);
                                }
                            }

                            return sb.ToString();
                        },
                        (ParsingResults pd, IParseTree t) => // terminal
                        {
                            TerminalNodeImpl term = t as TerminalNodeImpl;
                            if (term == null)
                            {
                                return null;
                            }
                            Antlr4.Runtime.Tree.IParseTree p = term;
                            string dir = System.IO.Path.GetDirectoryName(pd.Item.FullPath);
                            pd.Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
                            if (list_value == null)
                            {
                                return null;
                            }
                            bool first = true;
                            StringBuilder sb = new StringBuilder();
                            foreach (CombinedScopeSymbol value in list_value)
                            {
                                if (value == null)
                                {
                                    continue;
                                }
                                ISymbol sym = value as ISymbol;
                                if (sym == null)
                                {
                                    continue;
                                }
                                List<ISymbol> list_of_syms = new List<ISymbol>() { sym };
                                if (sym is RefSymbol)
                                {
                                    list_of_syms = sym.resolve();
                                }
                                foreach (ISymbol s in list_of_syms)
                                {
                                    if (! first)
                                    {
                                        sb.AppendLine();
                                    }
                                    first = false;
                                    if (s is TerminalSymbol)
                                    {
                                        sb.Append("Terminal ");
                                    }
                                    else if (s is NonterminalSymbol)
                                    {
                                        sb.Append("Nonterminal ");
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    string def_file = s.file;
                                    if (def_file == null)
                                    {
                                        continue;
                                    }
                                    Workspaces.Document def_document = pd.Item.Workspace.FindDocument(def_file);
                                    if (def_document == null)
                                    {
                                        continue;
                                    }
                                    ParsingResults def_pd = ParsingResultsFactory.Create(def_document);
                                    if (def_pd == null)
                                    {
                                        continue;
                                    }
                                    IParseTree fod = def_pd.Attributes.Where(
                                            kvp =>
                                            {
                                                IParseTree key = kvp.Key;
                                                if (!(key is TerminalNodeImpl))
                                                    return false;
                                                TerminalNodeImpl t1 = key as TerminalNodeImpl;
                                                IToken s1 = t1.Symbol;
                                                if (s.Token.Contains(s1))
                                                    return true;
                                                return false;
                                            })
                                        .Select(kvp => kvp.Key).FirstOrDefault();
                                    if (fod == null)
                                    {
                                        continue;
                                    }
                                    sb.Append("defined in ");
                                    sb.Append(s.file);
                                    sb.AppendLine();
                                    IParseTree node = fod;
                                    for (; node != null; node = node.Parent)
                                    {
                                        if (node is Iso14977Parser.Syntax_ruleContext)
                                        {
                                            break;
                                        }
                                    }
                                    if (node == null)
                                    {
                                        continue;
                                    }
                                    Reconstruct.Doit(sb, node);
                                }
                            }

                            return sb.ToString();
                        },
                        null, // comment
                        null, // keyword
                        null, // literal
                        null, // Mode
                        null, // Mode
                        null, // Channel
                        null, // Channel
                        null, // Punctuation
                        null, // Operator
            };
        public override int QuietAfter { get; set; } = 0;
        public override string StartRule { get; } = "grammar_";

        private static readonly List<string> _antlr_keywords = new List<string>()
        {
        };


        /* Tagging and classification types. */
        public enum AntlrClassifications : int
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

        public override Dictionary<IToken, int> ExtractComments(string code)
        {
            if (code == null) return null;
            var cts = this.TokStream;
            int type = (int)AntlrClassifications.ClassificationComment;
            Dictionary<IToken, int> new_list = new Dictionary<IToken, int>();
            for (int i = 0; i < cts.Index; ++i)
            {
                IList<IToken> inter = cts.GetHiddenTokensToLeft(i);
                if (inter != null)
                    foreach (IToken token in inter)
                    {
                        if (token.Type == Iso14977Lexer.COMMENT)
                        {
                            new_list[token] = type;
                        }
                    }
            }
            return new_list;
        }

        public Dictionary<IParseTree, ISymbol> GetSymbolTable()
        {
            return new Dictionary<IParseTree, ISymbol>();
        }

        public override bool IsFileType(string ffn)
        {
            if (ffn == null)
            {
                return false;
            }

            List<string> allowable_suffices = FileExtension.Split(';').ToList<string>();
            string suffix = Path.GetExtension(ffn).ToLower();
            foreach (string s in allowable_suffices)
            {
                if (suffix == s)
                {
                    return true;
                }
            }

            return false;
        }

        public override void Parse(ParsingResults pd, bool bail)
        {
            string ffn = pd.FullFileName;
            string code = pd.Code;
            if (ffn == null) return;
            if (code == null) return;
            this.QuietAfter = pd.QuietAfter;
            IParseTree pt = null;
            string newcode = code;
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(newcode);
                AntlrInputStream ais = new AntlrInputStream(
                    new StreamReader(new MemoryStream(byteArray))
                    .ReadToEnd())
                { name = ffn };
                var lexer = new Iso14977Lexer(ais);
                CommonTokenStream cts = new CommonTokenStream(lexer);
                var parser = new Iso14977Parser(cts);
                lexer.RemoveErrorListeners();
                var lexer_error_listener = new ErrorListener<int>(parser, lexer, pd.QuietAfter);
                lexer.AddErrorListener(lexer_error_listener);
                parser.RemoveErrorListeners();
                var parser_error_listener = new ErrorListener<IToken>(parser, lexer, pd.QuietAfter);
                parser.AddErrorListener(parser_error_listener);
                BailErrorHandler bail_error_handler = null;
                if (bail)
                {
                    bail_error_handler = new BailErrorHandler();
                    parser.ErrorHandler = bail_error_handler;
                }
                try
                {
                    pt = parser.syntax1();
                }
                catch (Exception)
                {
                }
                if (parser_error_listener.had_error || lexer_error_listener.had_error || (bail_error_handler != null && bail_error_handler.had_error))
                {
                    System.Console.Error.WriteLine("Error in parse of " + ffn);
                }
                else
                {
                    //System.Console.Error.WriteLine("Parse completed of " + ffn);
                }
                MyTokenStream out_token_stream2 = new MyTokenStream();
                out_token_stream2.Text = code;
                MyCharStream fake_char_stream2 = new MyCharStream();
                fake_char_stream2.Text = out_token_stream2.Text;
                fake_char_stream2.SourceName = ffn;
                MyLexer lexer2 = new MyLexer(null);
                lexer2._inputstream = fake_char_stream2;
                lexer2._vocabulary = lexer.Vocabulary as Vocabulary;
                lexer2._channelNames = lexer.ChannelNames;
                out_token_stream2._tokenSource = lexer2;
                var s2 = new Stack<IParseTree>();
                s2.Push(pt);
                while (s2.Any())
                {
                    var n = s2.Pop();
                    if (n is Iso14977Parser.Gap_free_symbolContext gfs)
                    {
                        var start_token_index = gfs.SourceInterval.a;
                        var stop_token_index = gfs.SourceInterval.b;
                        for (int i = start_token_index; i <= stop_token_index; ++i)
                        {
                            cts.Seek(0);
                            var t = cts.Get(i);
                            var token = new MyToken();
                            token.Type = t.Type;
                            token.StartIndex = t.StartIndex;
                            token.StopIndex = t.StopIndex;
                            token.Line = t.Line;
                            token.Column = t.Column;
                            token.Channel = t.Channel;
                            token.InputStream = lexer2.InputStream;
                            token.TokenSource = lexer2;
                            token.TokenIndex = t.TokenIndex;
                            token.Text =
                                out_token_stream2.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            out_token_stream2.Add(token);
                        }
                    }
                    else if (n is TerminalNodeImpl term && term.Symbol.Type == Antlr4.Runtime.TokenConstants.EOF)
                    {
                        var start_token_index = term.SourceInterval.a;
                        var stop_token_index = term.SourceInterval.b;
                        for (int i = start_token_index; i <= stop_token_index; ++i)
                        {
                            cts.Seek(0);
                            var t = cts.Get(i);
                            var token = new MyToken();
                            token.Type = t.Type;
                            token.StartIndex = t.StartIndex;
                            token.StopIndex = t.StopIndex;
                            token.Line = t.Line;
                            token.Column = t.Column;
                            token.Channel = t.Channel;
                            token.InputStream = lexer2.InputStream;
                            token.TokenSource = lexer2;
                            token.TokenIndex = t.TokenIndex;
                            token.Text =
                                out_token_stream2.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            out_token_stream2.Add(token);
                        }
                    }
                    else if (n is ParserRuleContext prc)
                    {
                        for (int i = n.ChildCount - 1; i >= 0; i--)
                        {
                            s2.Push(n.GetChild(i));
                        }
                    }
                }
                var parser2 = new Iso14977Parser(out_token_stream2);
                parser2.RemoveErrorListeners();
                var parser_error_listener2 = new ErrorListener<IToken>(parser2, lexer2, pd.QuietAfter);
                parser2.AddErrorListener(parser_error_listener);
                if (bail)
                {
                    bail_error_handler = new BailErrorHandler();
                    parser2.ErrorHandler = bail_error_handler;
                }
                try
                {
                    pt = parser2.syntax2();
                }
                catch (Exception)
                {
                    // Parsing error.
                }
                if (parser_error_listener.had_error || lexer_error_listener.had_error || (bail_error_handler != null && bail_error_handler.had_error))
                {
                    System.Console.Error.WriteLine("Error in parse of " + ffn);
                }
                else
                {
                    //System.Console.Error.WriteLine("Parse completed of " + ffn);
                }
                MyTokenStream out_token_stream3 = new MyTokenStream();
                out_token_stream3.Text = code;
                MyCharStream fake_char_stream3 = new MyCharStream();
                fake_char_stream3.Text = out_token_stream3.Text;
                fake_char_stream3.SourceName = ffn;
                MyLexer lexer3 = new MyLexer(null);
                lexer3._inputstream = fake_char_stream3;
                out_token_stream3._tokenSource = lexer3;
                var s3 = new Stack<IParseTree>();
                s3.Push(pt);
                while (s3.Any())
                {
                    var n = s3.Pop();
                    if (n is Iso14977Parser.Commentless_symbolContext gfs && !(gfs.Parent is Iso14977Parser.Comment_symbolContext))
                    {
                        var start_token_index = gfs.SourceInterval.a;
                        var stop_token_index = gfs.SourceInterval.b;
                        for (int i = start_token_index; i <= stop_token_index; ++i)
                        {
                            cts.Seek(0);
                            var t = cts.Get(i);
                            var token = new MyToken();
                            token.Type = t.Type;
                            token.StartIndex = t.StartIndex;
                            token.StopIndex = t.StopIndex;
                            token.Line = t.Line;
                            token.Column = t.Column;
                            token.Channel = t.Channel;
                            token.InputStream = lexer2.InputStream;
                            token.TokenSource = lexer2;
                            token.TokenIndex = t.TokenIndex;
                            token.Text =
                                out_token_stream3.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            out_token_stream3.Add(token);
                        }
                    }
                    else if (n is Iso14977Parser.Commentless_symbolContext gfs2 && (gfs2.Parent is Iso14977Parser.Comment_symbolContext))
                    {
                        var start_token_index = gfs2.SourceInterval.a;
                        var stop_token_index = gfs2.SourceInterval.b;
                        for (int i = start_token_index; i <= stop_token_index; ++i)
                        {
                            cts.Seek(0);
                            var t = cts.Get(i);
                            var token = new MyToken();
                            token.Type = t.Type;
                            token.StartIndex = t.StartIndex;
                            token.StopIndex = t.StopIndex;
                            token.Line = t.Line;
                            token.Column = t.Column;
                            token.Channel = Iso14977Lexer.OFF_CHANNEL;
                            token.InputStream = lexer2.InputStream;
                            token.TokenSource = lexer2;
                            token.TokenIndex = t.TokenIndex;
                            token.Text =
                                out_token_stream3.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            out_token_stream3.Add(token);
                        }
                    }
                    else if (n is TerminalNodeImpl opstar && opstar.Symbol.Type == Iso14977Lexer.OPSTAR)
                    {
                        var start_token_index = opstar.SourceInterval.a;
                        var stop_token_index = opstar.SourceInterval.b;
                        for (int i = start_token_index; i <= stop_token_index; ++i)
                        {
                            cts.Seek(0);
                            var t = cts.Get(i);
                            var token = new MyToken();
                            token.Type = t.Type;
                            token.StartIndex = t.StartIndex;
                            token.StopIndex = t.StopIndex;
                            token.Line = t.Line;
                            token.Column = t.Column;
                            token.Channel = Iso14977Lexer.COMMENT;
                            token.InputStream = lexer2.InputStream;
                            token.TokenSource = lexer2;
                            token.TokenIndex = t.TokenIndex;
                            token.Text =
                                out_token_stream3.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            out_token_stream3.Add(token);
                        }
                    }
                    else if (n is TerminalNodeImpl starcp && starcp.Symbol.Type == Iso14977Lexer.STARCP)
                    {
                        var start_token_index = starcp.SourceInterval.a;
                        var stop_token_index = starcp.SourceInterval.b;
                        for (int i = start_token_index; i <= stop_token_index; ++i)
                        {
                            cts.Seek(0);
                            var t = cts.Get(i);
                            var token = new MyToken();
                            token.Type = t.Type;
                            token.StartIndex = t.StartIndex;
                            token.StopIndex = t.StopIndex;
                            token.Line = t.Line;
                            token.Column = t.Column;
                            token.Channel = Iso14977Lexer.COMMENT;
                            token.InputStream = lexer2.InputStream;
                            token.TokenSource = lexer2;
                            token.TokenIndex = t.TokenIndex;
                            token.Text =
                                out_token_stream3.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            out_token_stream3.Add(token);
                        }
                    }
                    else if (n is TerminalNodeImpl term && term.Symbol.Type == Antlr4.Runtime.TokenConstants.EOF)
                    {
                        var start_token_index = term.SourceInterval.a;
                        var stop_token_index = term.SourceInterval.b;
                        for (int i = start_token_index; i <= stop_token_index; ++i)
                        {
                            cts.Seek(0);
                            var t = cts.Get(i);
                            var token = new MyToken();
                            token.Type = t.Type;
                            token.StartIndex = t.StartIndex;
                            token.StopIndex = t.StopIndex;
                            token.Line = t.Line;
                            token.Column = t.Column;
                            token.Channel = t.Channel;
                            token.InputStream = lexer2.InputStream;
                            token.TokenSource = lexer2;
                            token.TokenIndex = t.TokenIndex;
                            token.Text =
                                out_token_stream3.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                            out_token_stream3.Add(token);
                        }
                    }
                    else if (n is ParserRuleContext prc)
                    {
                        for (int i = n.ChildCount - 1; i >= 0; i--)
                        {
                            s3.Push(n.GetChild(i));
                        }
                    }
                }

                // Set up Antlr to parse input grammar.
                var parser3 = new Iso14977Parser(out_token_stream3);
                parser2.RemoveErrorListeners();
                var parser_error_listener3 = new ErrorListener<IToken>(parser3, lexer3, pd.QuietAfter);
                parser3.AddErrorListener(parser_error_listener3);
                if (bail)
                {
                    bail_error_handler = new BailErrorHandler();
                    parser2.ErrorHandler = bail_error_handler;
                }
                try
                {
                    pt = parser3.syntax3();
                }
                catch (Exception)
                {
                    // Parsing error.
                }
                if (parser_error_listener3.had_error || (bail_error_handler != null && bail_error_handler.had_error))
                {
                    System.Console.Error.WriteLine("Error in parse of " + ffn);
                }
                else
                {
                    //System.Console.Error.WriteLine("Parse completed of " + ffn);
                }

                pd.TokStream = cts;
                pd.Parser = parser;
                pd.Lexer = lexer;
                pd.ParseTree = pt;
            }
        }

        public override void Parse(string code,
            out CommonTokenStream TokStream,
            out Parser Parser,
            out Lexer Lexer,
            out IParseTree ParseTree)
        {
            IParseTree pt = null;

            // Set up Antlr to parse input grammar.
            byte[] byteArray = Encoding.UTF8.GetBytes(code);
            AntlrInputStream ais = new AntlrInputStream(
            new StreamReader(
                new MemoryStream(byteArray)).ReadToEnd());
            var lexer = new Iso14977Lexer(ais);
            CommonTokenStream cts = new CommonTokenStream(lexer);
            var parser = new Iso14977Parser(cts);
            lexer.RemoveErrorListeners();
            var lexer_error_listener = new ErrorListener<int>(parser, lexer, this.QuietAfter);
            lexer.AddErrorListener(lexer_error_listener);
            parser.RemoveErrorListeners();
            var parser_error_listener = new ErrorListener<IToken>(parser, lexer, this.QuietAfter);
            parser.AddErrorListener(parser_error_listener);
            try
            {
                pt = parser.syntax1();
            }
            catch (Exception)
            {
                // Parsing error.
            }

            TokStream = cts;
            Parser = parser;
            Lexer = lexer;
            ParseTree = pt;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void GetGrammarBasics()
        {
            // Gather Imports from grammars.
            // Gather InverseImports map.
            if (!ParsingResults.InverseImports.ContainsKey(this.FullFileName))
            {
                ParsingResults.InverseImports.Add(this.FullFileName, new HashSet<string>());
            }
        }

        public class Pass2Listener : Iso14977ParserBaseListener
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

            public override void EnterMeta_identifier([NotNull] Iso14977Parser.Meta_identifierContext context)
            {
                var chars_in_symbol = context.meta_identifier_character();
                string id = String.Join("", chars_in_symbol.Select(t => t.Payload.GetText()));
                if (context.Parent is Iso14977Parser.Syntax_ruleContext)
                {
                    var frontier = TreeEdits.Frontier(context);
                    var list = frontier.Select(t => t.Symbol).ToList();
                    ISymbol sym = new NonterminalSymbol(id, list);
                    _pd.RootScope.define(ref sym);
                    CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
                    _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
                    foreach (var tr in frontier)
                        _pd.Attributes[tr] = new List<CombinedScopeSymbol>() { s };
                }
            }
        }


        public class Pass3Listener : Iso14977ParserBaseListener
        {
            private readonly ParsingResults _pd;

            public Pass3Listener(ParsingResults pd)
            {
                _pd = pd;
            }
            public override void EnterMeta_identifier([NotNull] Iso14977Parser.Meta_identifierContext context)
            {
                var chars_in_symbol = context.meta_identifier_character();
                string id = String.Join("", chars_in_symbol.Select(t => t.Payload.GetText()));

                if (context.Parent is Iso14977Parser.Syntax_ruleContext)
                    return;

                var frontier = TreeEdits.Frontier(context);
                List<ISymbol> prior = _pd.RootScope.LookupType(id).ToList();
                if (!prior.Any())
                {
                    var listp = frontier.Select(t => t.Symbol).ToList();
                    ISymbol symp = new NonterminalSymbol(id, listp);
                    _pd.RootScope.define(ref symp);
                    prior = _pd.RootScope.LookupType(id).ToList();
                }
                var list = frontier.Select(t => t.Symbol).ToList();
                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                CombinedScopeSymbol s = new RefSymbol(list, prior);
                new_attrs.Add(s);
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
                foreach (var tr in frontier)
                    _pd.Attributes[tr] = new List<CombinedScopeSymbol>() { s };
            }
        }
    }
}
