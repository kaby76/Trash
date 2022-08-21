namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using Domemtech.Symtab;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class BisonParsingResults : ParsingResults, IParserDescription
    {
        public BisonParsingResults(Workspaces.Document doc) : base(doc)
        {
            // Passes executed in order for all files.
            Passes.Add(() =>
            {
                // Gather Imports from grammars.
                // Gather _dependent_grammars map.
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
                ParseTreeWalker.Default.Walk(new Pass0Listener(this), ParseTree);
                int after_count = 0;
                var workspace = doc.Workspace;
                foreach (KeyValuePair<string, HashSet<string>> dep in ParsingResults.InverseImports)
                {
                    string name = dep.Key;
                    Workspaces.Document x = workspace.FindDocument(name);
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
                // For all imported grammars across the entire universe,
                // make sure all are loaded in the workspace,
                // then restart.
                var workspace = doc.Workspace;
                foreach (KeyValuePair<string, HashSet<string>> dep in ParsingResults.InverseImports)
                {
                    string name = dep.Key;
                    Workspaces.Document x = workspace.FindDocument(name);
                    if (x == null)
                    {
                        // Add document.
                        Workspaces.Container proj = Item.Parent;
                        Workspaces.Document new_doc = new Workspaces.Document(name);
                        proj.AddChild(new_doc);
                        return true;
                    }
                    foreach (string y in dep.Value)
                    {
                        Workspaces.Document z = workspace.FindDocument(y);
                        if (z == null)
                        {
                            // Add document.
                            Workspaces.Container proj = Item.Parent;
                            Workspaces.Document new_doc = new Workspaces.Document(y);
                            proj.AddChild(new_doc);
                            return true;
                        }
                    }
                }

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
                this.Attributes[ParseTree] = new List<CombinedScopeSymbol>() { (CombinedScopeSymbol)this.RootScope };
                // Collect def lexer symbols.
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                        new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(this.ParseTree, this.Parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                        new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var nodes = engine.parseExpression(
                            @"//token_decls//token_decl/id[position() = 1]",
                            new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as BisonParser.IdContext);
                    Antlr4.Runtime.Tree.IParseTree parent;
                    foreach (var id in nodes)
                    {
                        var token = id.ID();
                        if (id == null) continue;
                        for (parent = id; parent != null; parent = parent.Parent)
                        {
                            if (parent is BisonParser.Token_declsContext)
                            {
                                BisonParser.Token_declsContext token_decls = parent as BisonParser.Token_declsContext;
                                int count = token_decls.ChildCount;
                                if (count == 1)
                                {
                                    string tok = id.GetText();
                                    ISymbol sym = new NonterminalSymbol(tok, new List<IToken>() { token.Symbol });
                                    this.RootScope.define(ref sym);
                                    CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
                                    this.Attributes[id] = new List<CombinedScopeSymbol>() { s };
                                    this.Attributes[id.GetChild(0)] = new List<CombinedScopeSymbol>() { s };
                                }
                                else if (count == 2)
                                {
                                    string tag = parent.GetChild(0).GetText();
                                    tag = tag.Replace("<", "").Replace(">", "");
                                    string tok = id.GetText();
                                    ISymbol sym = new NonterminalSymbol(tok, new List<IToken>() { token.Symbol });
                                    this.RootScope.define(ref sym);
                                    CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
                                    this.Attributes[id] = new List<CombinedScopeSymbol>() { s };
                                    this.Attributes[id.GetChild(0)] = new List<CombinedScopeSymbol>() { s };
                                }
                                else if (count == 3)
                                {
                                    string tag = parent.GetChild(1).GetText();
                                    tag = tag.Replace("<", "").Replace(">", "");
                                    string tok = id.GetText();
                                    ISymbol sym = new NonterminalSymbol(tok, new List<IToken>() { token.Symbol });
                                    this.RootScope.define(ref sym);
                                    CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
                                    this.Attributes[id] = new List<CombinedScopeSymbol>() { s };
                                    this.Attributes[id.GetChild(0)] = new List<CombinedScopeSymbol>() { s };
                                }
                            }
                        }
                    }
                }

                // Collect def parser symbols.
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                        new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(this.ParseTree, this.Parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                        new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var nodes = engine.parseExpression(
                            @"//rules",
                            new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement));
                    foreach (var rule in nodes)
                    {
                        var r = rule.AntlrIParseTree;
                        var lhs_id = r.GetChild(0) as BisonParser.IdContext;
                        ITerminalNode lhs = lhs_id.ID();
                        TerminalNodeImpl rule_ref = lhs as TerminalNodeImpl;
                        string id = rule_ref.GetText();
                        ISymbol s = new NonterminalSymbol(id, new List<IToken>() { rule_ref.Symbol });
                        this.RootScope.define(ref s);
                        CombinedScopeSymbol s2 = (CombinedScopeSymbol)s;
                        this.Attributes[lhs_id] = new List<CombinedScopeSymbol>() { s2 };
                        this.Attributes[rule_ref] = new List<CombinedScopeSymbol>() { s2 };
                    }
                }

                return false;
            });
            Passes.Add(() =>
            {
                if (ParseTree == null) return false;
                // Collect ref symbols.
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                        new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(this.ParseTree, this.Parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine =
                        new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var nodes = engine.parseExpression(
                            @"//rules",
                            new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement));
                    foreach (var rule in nodes)
                    {
                        var r = rule.AntlrIParseTree;
                        var rhses = engine.parseExpression(
                                @".//rhses_1/rhs",
                                new StaticContextBuilder()).evaluate(
                                dynamicContext, new object[] { rule })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement));
                        foreach (var r1 in rhses)
                        {
                            var sym = engine.parseExpression(
                                    @".//symbol",
                                    new StaticContextBuilder()).evaluate(
                                    dynamicContext, new object[] { r1 })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as BisonParser.SymbolContext);
                            foreach (var s3 in sym)
                            {
                                var id = s3.id();
                                var id_term = id.ID();
                                if (id_term == null) continue;
                                var id_str = id_term.GetText();
                                List<ISymbol> list = this.RootScope.LookupType(id_str).ToList();
                                if (!list.Any())
                                {
                                    ISymbol s4 = new NonterminalSymbol(id_str, new List<IToken>() { id_term.Symbol });
                                    this.RootScope.define(ref s4);
                                    list = this.RootScope.LookupType(id_str).ToList();
                                }
                                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                                CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { id_term.Symbol }, list);
                                new_attrs.Add(s);
                                this.Attributes[s3] = new_attrs;
                                this.Attributes[id] = new_attrs;
                                this.Attributes[id_term] = new_attrs;
                            }
                        }
                    }
                }
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
            true, // literal
            true, // mode
            true, // mode
            true, // channel
            true, // channel
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
            true, // mode
            true, // mode
            true, // channel
            true, // channel
            false, // punctuation
            false, // operator
        };
        public override List<bool> CanGotovisitor { get; } = new List<bool>()
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
        public override bool CanNextRule
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
            true, // mode
            true, // mode
            true, // channel
            true, // channel
            false, // punctuation
            false, // operator
        };
        public override bool CanReformat
        {
            get
            {
                return true;
            }
        }
        public override Func<IParserDescription, Dictionary<IParseTree, IList<CombinedScopeSymbol>>, IParseTree, int> Classify { get; } =
         (IParserDescription gd, Dictionary<IParseTree, IList<CombinedScopeSymbol>> st, IParseTree t) =>
         {
             TerminalNodeImpl term = t as TerminalNodeImpl;
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
                                 return (int)AntlrClassifications.ClassificationNonterminalRef;
                             }
                             else if (d is TerminalSymbol)
                             {
                                 return (int)AntlrClassifications.ClassificationNonterminalRef;
                             }
                             else if (d is ModeSymbol)
                             {
                                 return (int)AntlrClassifications.ClassificationModeRef; ;
                             }
                             else if (d is ChannelSymbol)
                             {
                                 return (int)AntlrClassifications.ClassificationChannelRef; ;
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
                 if ((term.Symbol.Type == BisonParser.STRING
                       || term.Symbol.Type == BisonParser.INT
                       ))
                 {
                     return (int)AntlrClassifications.ClassificationLiteral;
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
                 if (term.Payload.Channel == BisonLexer.OFF_CHANNEL
                  || term.Symbol.Type == BisonLexer.BLOCK_COMMENT
                  || term.Symbol.Type == BisonLexer.LINE_COMMENT)
                 {
                     return (int)AntlrClassifications.ClassificationComment;
                 }
             }
             return -1;
         };
        public override bool DoErrorSquiggles => true;
        public override string FileExtension { get; } = ".y";
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
        public override string Name { get; } = "Bison";
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

                var workspace = pd.Item.Workspace;
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
                        Workspaces.Document def_document = workspace.FindDocument(def_file);
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
                            if (node is BisonParser.RulesContext)
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

                var workspace = pd.Item.Workspace;
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
                        Workspaces.Document def_document = workspace.FindDocument(def_file);
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
                            if (node is BisonParser.RulesContext)
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

                var workspace = pd.Item.Workspace;
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
                        Workspaces.Document def_document = workspace.FindDocument(def_file);
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
                            if (node is BisonParser.RulesContext)
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

                var workspace = pd.Item.Workspace;
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
                        Workspaces.Document def_document = workspace.FindDocument(def_file);
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
                            if (node is BisonParser.RulesContext)
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
        public override string StartRule { get; } = "input";



        private static readonly List<string> _antlr_keywords = new List<string>()
        {
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
            byte[] byteArray = Encoding.UTF8.GetBytes(code);
            var ais = new AntlrInputStream(
                        new StreamReader(
                            new MemoryStream(byteArray)).ReadToEnd());
            var lexer = new BisonLexer(ais);
            CommonTokenStream cts_off_channel = new CommonTokenStream(lexer, BisonLexer.OFF_CHANNEL);
            lexer.RemoveErrorListeners();
            var lexer_error_listener = new ErrorListener<int>(null, lexer, this.QuietAfter);
            lexer.AddErrorListener(lexer_error_listener);
            Dictionary<IToken, int> new_list = new Dictionary<IToken, int>();
            int type = (int)AntlrClassifications.ClassificationComment;
            while (cts_off_channel.LA(1) != BisonParser.Eof)
            {
                IToken token = cts_off_channel.LT(1);
                if (token.Type == BisonLexer.BLOCK_COMMENT
                    || token.Type == BisonLexer.LINE_COMMENT)
                {
                    new_list[token] = type;
                }
                cts_off_channel.Consume();
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

            // Set up Antlr to parse input grammar.
            byte[] byteArray = Encoding.UTF8.GetBytes(code);
            AntlrInputStream ais = new AntlrInputStream(
            new StreamReader(
                new MemoryStream(byteArray)).ReadToEnd())
            {
                name = ffn
            };
            var lexer = new BisonLexer(ais);
            CommonTokenStream cts = new CommonTokenStream(lexer);
            var parser = new BisonParser(cts);
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
                pt = parser.input();
            }
            catch (Exception)
            {
                // Parsing error.
            }

            //StringBuilder sb = new StringBuilder();
            //TreeSerializer.ParenthesizedAST(pt, sb, "", cts);
            //string fn = System.IO.Path.GetFileName(ffn);
            //fn = "c:\\temp\\" + fn;
            //System.IO.File.WriteAllText(fn, sb.ToString());
            if (parser_error_listener.had_error || lexer_error_listener.had_error || (bail_error_handler != null && bail_error_handler.had_error))
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
            var lexer = new BisonLexer(ais);
            CommonTokenStream cts = new CommonTokenStream(lexer);
            var parser = new BisonParser(cts);
            lexer.RemoveErrorListeners();
            var lexer_error_listener = new ErrorListener<int>(parser, lexer, this.QuietAfter);
            lexer.AddErrorListener(lexer_error_listener);
            parser.RemoveErrorListeners();
            var parser_error_listener = new ErrorListener<IToken>(parser, lexer, this.QuietAfter);
            parser.AddErrorListener(parser_error_listener);
            try
            {
                pt = parser.input();
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


        public class Pass0Listener : BisonParserBaseListener
        {
            private readonly ParsingResults _pd;

            public Pass0Listener(ParsingResults pd)
            {
                _pd = pd;
            }
        }


        public IParseTree NearestScope(IParseTree node)
        {
            for (; node != null; node = node.Parent)
            {
                this.Attributes.TryGetValue(node, out IList<CombinedScopeSymbol> list);
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

            this.Attributes.TryGetValue(node, out IList<CombinedScopeSymbol> list);
            if (list != null)
            {
                if (list.Count == 1 && list[0] is IScope)
                {
                    return list[0] as IScope;
                }
            }
            return null;
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
    }
}

