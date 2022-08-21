// Template generated code from trgen 0.8.7

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Domemtech.Symtab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public partial class FrontEnd
{
    public Parser Parser { get; set; }
    public Lexer Lexer { get; set; }
    public ITokenStream TokenStream { get; set; }
    public IParseTree Tree { get; set; }
    public string Code { get; set; }
    public IParseTree Parse(string input)
    {
        var str = new AntlrInputStream(input);
        var lexer = new ANTLRv2Lexer(str);
        Lexer = lexer;
        var tokens = new CommonTokenStream(lexer);
        TokenStream = tokens;
        var parser = new ANTLRv2Parser(tokens);
        Parser = parser;
        var tree = parser.grammar_();
        Tree = tree;
        return tree;
    }

    static void Main(string[] args)
    {
        bool show_tree = false;
        bool show_tokens = false;
        bool old = false;
        bool two_byte = false;
        string file_name = null;
        string input = null;
        System.Text.Encoding encoding = null;
        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i].Equals("-tokens"))
            {
                show_tokens = true;
                continue;
            }
            else if (args[i].Equals("-two-byte"))
            {
                two_byte = true;
                continue;
            }
            else if (args[i].Equals("-old"))
            {
                old = true;
                continue;
            }
            else if (args[i].Equals("-tree"))
            {
                show_tree = true;
                continue;
            }
            else if (args[i].Equals("-input"))
                input = args[++i];
            else if (args[i].Equals("-file"))
                file_name = args[++i];
            else if (args[i].Equals("-encoding"))
            {
                ++i;
                encoding = Encoding.GetEncoding(
                    args[i],
                    new EncoderReplacementFallback("(unknown)"),
                    new DecoderReplacementFallback("(error)"));
                if (encoding == null)
                    throw new Exception(@"Unknown encoding. Must be an Internet Assigned Numbers Authority (IANA) code page name. https://www.iana.org/assignments/character-sets/character-sets.xhtml");
            }
        }
        ICharStream str = null;
        if (input == null && file_name == null)
        {
            str = CharStreams.fromStream(System.Console.OpenStandardInput());
        } else if (input != null)
        {
            str = CharStreams.fromString(input);
        } else if (file_name != null)
        {
            if (old)
            {
                FileStream fs = new FileStream(file_name, FileMode.Open);
                str = new Antlr4.Runtime.AntlrInputStream(fs);
            }
            else if (encoding == null)
                str = CharStreams.fromPath(file_name);
            else
                str = CharStreams.fromPath(file_name, encoding);
        }
        var lexer = new ANTLRv2Lexer(str);
        if (show_tokens)
        {
            StringBuilder new_s = new StringBuilder();
            for (int i = 0; ; ++i)
            {
                var ro_token = lexer.NextToken();
                var token = (CommonToken)ro_token;
                token.TokenIndex = i;
                new_s.AppendLine(token.ToString());
                if (token.Type == Antlr4.Runtime.TokenConstants.EOF)
                    break;
            }
            System.Console.Error.WriteLine(new_s.ToString());
            lexer.Reset();
        }
        var tokens = new CommonTokenStream(lexer);
        var parser = new ANTLRv2Parser(tokens);
        var listener_lexer = new ErrorListener<int>();
        var listener_parser = new ErrorListener<IToken>();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        DateTime before = DateTime.Now;
        var tree = parser.grammar_();
        DateTime after = DateTime.Now;
        System.Console.Error.WriteLine("Time: " + (after - before));
        if (listener_lexer.had_error || listener_parser.had_error)
        {
            System.Console.Error.WriteLine("Parse failed.");
        }
        else
        {
            System.Console.Error.WriteLine("Parse succeeded.");
        }
        if (show_tree)
        {
            System.Console.Error.WriteLine(tree.ToStringTree(parser));
        }
        System.Environment.Exit(listener_lexer.had_error || listener_parser.had_error ? 1 : 0);
    }

    public List<Func<bool>> Passes { get; } = new List<Func<bool>>();
    public Dictionary<IParseTree, IList<CombinedScopeSymbol>> Attributes { get; set; } = new Dictionary<IParseTree, IList<CombinedScopeSymbol>>();
    protected readonly Dictionary<string, IScope> _scopes = new Dictionary<string, IScope>();
    public Dictionary<string, HashSet<string>> InverseImports = new Dictionary<string, HashSet<string>>();
    public HashSet<string> Imports { get; set; } = new HashSet<string>();
    public string FullFileName
    {
        get
        {
            return Item?.FullPath;
        }
    }
    public Workspaces.Document Item { get; set; }
    public IScope RootScope { get; set; }

    public void ComputeAttributes(Workspaces.Document doc)
    {
        // Passes executed in order for all files.
        Passes.Add(() =>
        {
            // Gather Imports from grammars.
            // Gather _dependent_grammars map.
            int before_count = 0;
            if (!InverseImports.ContainsKey(this.FullFileName))
            {
                InverseImports.Add(this.FullFileName, new HashSet<string>());
            }
            foreach (KeyValuePair<string, HashSet<string>> x in InverseImports)
            {
                before_count++;
                before_count = before_count + x.Value.Count;
            }
            if (Tree == null) return false;
            ParseTreeWalker.Default.Walk(new Pass0Listener(this), Tree);
            int after_count = 0;
            foreach (KeyValuePair<string, HashSet<string>> dep in InverseImports)
            {
                string name = dep.Key;
                var workspace = doc.Workspace;
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
            foreach (KeyValuePair<string, HashSet<string>> dep in InverseImports)
            {
                string name = dep.Key;
                var workspace = doc.Workspace;
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
            if (Tree == null) return false;
            ParseTreeWalker.Default.Walk(new Pass2Listener(this), Tree);
            return false;
        });
        Passes.Add(() =>
        {
            if (Tree == null) return false;
            ParseTreeWalker.Default.Walk(new Pass3Listener(this), Tree);
            return false;
        });
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
    public bool CanReformat
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
    public int Classify(IParseTree t)
    {
        TerminalNodeImpl term = t as TerminalNodeImpl;
        Antlr4.Runtime.Tree.IParseTree p = term;
        Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
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
            if ((term.Symbol.Type == ANTLRv2Parser.STRING_LITERAL
                    || term.Symbol.Type == ANTLRv2Parser.INT
                    || term.Symbol.Type == ANTLRv2Parser.LEXER_CHAR_SET))
            {
                return (int)AntlrClassifications.ClassificationLiteral;
            }
            if (
                term.Symbol.Type == ANTLRv2Parser.DOC_COMMENT
                || term.Symbol.Type == ANTLRv2Parser.ML_COMMENT
                || term.Symbol.Type == ANTLRv2Parser.SL_COMMENT)
            {
                return (int)AntlrClassifications.ClassificationComment;
            }
        }
        return -1;
    }
    public bool DoErrorSquiggles
    {
        get
        {
            return true;
        }
    }
    public string FileExtension { get; } = ".g2;.g";
    public string[] Map { get; } = new string[]
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
    public string Name { get; } = "Antlr2";
    public string PopUpDefinition(IParseTree t, AntlrClassifications ty)
    {
        switch (ty)
        {
            case AntlrClassifications.ClassificationNonterminalDef:
                {
                    TerminalNodeImpl term = t as TerminalNodeImpl;
                    if (term == null)
                    {
                        return null;
                    }
                    Antlr4.Runtime.Tree.IParseTree p = term;
                    string dir = System.IO.Path.GetDirectoryName(Item.FullPath);
                    Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
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
                            if (!first)
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

                            var workspace = Item.Workspace;
                            Workspaces.Document def_document = workspace.FindDocument(def_file);
                            if (def_document == null)
                            {
                                continue;
                            }
                            FrontEnd def_pd = Create(def_document);
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
                                if (node is ANTLRv2Parser.Rule_Context ||
                                    node is ANTLRv2Parser.TokensSpecContext)
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
                }
                break;
            case AntlrClassifications.ClassificationNonterminalRef:
                {
                    TerminalNodeImpl term = t as TerminalNodeImpl;
                    if (term == null)
                    {
                        return null;
                    }
                    Antlr4.Runtime.Tree.IParseTree p = term;
                    string dir = System.IO.Path.GetDirectoryName(Item.FullPath);
                    Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
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
                            if (!first)
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

                            var workspace = Item.Workspace;
                            Workspaces.Document def_document = workspace.FindDocument(def_file);
                            if (def_document == null)
                            {
                                continue;
                            }
                            FrontEnd def_pd = Create(def_document);
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
                                if (node is ANTLRv2Parser.Rule_Context ||
                                    node is ANTLRv2Parser.TokensSpecContext)
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
                }
                break;
            case AntlrClassifications.ClassificationTerminalDef:
                {
                    TerminalNodeImpl term = t as TerminalNodeImpl;
                    if (term == null)
                    {
                        return null;
                    }
                    Antlr4.Runtime.Tree.IParseTree p = term;
                    string dir = System.IO.Path.GetDirectoryName(Item.FullPath);
                    Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
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
                            if (!first)
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

                            var workspace = Item.Workspace;
                            Workspaces.Document def_document = workspace.FindDocument(def_file);
                            if (def_document == null)
                            {
                                continue;
                            }
                            FrontEnd def_pd = Create(def_document);
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
                                if (node is ANTLRv2Parser.Rule_Context ||
                                    node is ANTLRv2Parser.TokensSpecContext)
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
                }
                break;
            case AntlrClassifications.ClassificationTerminalRef:
                {
                    TerminalNodeImpl term = t as TerminalNodeImpl;
                    if (term == null)
                    {
                        return null;
                    }
                    Antlr4.Runtime.Tree.IParseTree p = term;
                    string dir = System.IO.Path.GetDirectoryName(Item.FullPath);
                    Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
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
                            if (!first)
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

                            var workspace = Item.Workspace;
                            Workspaces.Document def_document = workspace.FindDocument(def_file);
                            if (def_document == null)
                            {
                                continue;
                            }
                            FrontEnd def_pd = Create(def_document);
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
                                if (node is ANTLRv2Parser.Rule_Context ||
                                    node is ANTLRv2Parser.TokensSpecContext)
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
                }
                break;
            default:
                return null;
        }
    }
    public int QuietAfter { get; set; } = 0;
    public string StartRule { get; } = "grammar_";

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

    private static readonly Dictionary<string, FrontEnd> _per_file_parser_details = new Dictionary<string, FrontEnd>();

    public static IEnumerable<KeyValuePair<string, FrontEnd>> AllParserDetails => _per_file_parser_details;

    public static FrontEnd Create(Workspaces.Document document)
    {
        if (document == null)
        {
            return null;
        }

        string ffn = document.FullPath;
        foreach (KeyValuePair<string, FrontEnd> pd in _per_file_parser_details)
        {
            if (pd.Key == ffn)
            {
                return pd.Value;
            }
        }
        var result = new FrontEnd();
        result.Item = document;
        _per_file_parser_details[ffn] = result;
        return result;
    }

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

    public Dictionary<IToken, int> ExtractComments(string code)
    {
        if (code == null) return null;
        byte[] byteArray = Encoding.UTF8.GetBytes(code);
        var ais = new AntlrInputStream(
                    new StreamReader(
                        new MemoryStream(byteArray)).ReadToEnd());
        ANTLRv2Lexer lexer = new ANTLRv2Lexer(ais);
        CommonTokenStream cts_off_channel = new CommonTokenStream(lexer, ANTLRv2Lexer.OFF_CHANNEL);
        lexer.RemoveErrorListeners();
        var lexer_error_listener = new ErrorListener<int>(this.QuietAfter);
        lexer.AddErrorListener(lexer_error_listener);
        Dictionary<IToken, int> new_list = new Dictionary<IToken, int>();
        int type = (int)AntlrClassifications.ClassificationComment;
        while (cts_off_channel.LA(1) != ANTLRv2Parser.Eof)
        {
            IToken token = cts_off_channel.LT(1);
            if (token.Type == ANTLRv2Lexer.ML_COMMENT
                || token.Type == ANTLRv2Lexer.SL_COMMENT
                || token.Type == ANTLRv2Lexer.DOC_COMMENT)
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

    public bool IsFileType(string ffn)
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
    public class BailErrorHandler : BailErrorStrategy
    {
        public bool had_error;

        public override void ReportError(Parser recognizer, RecognitionException e)
        {
            had_error = true;
            base.ReportError(recognizer, e);
        }

        public override void Recover(Parser recognizer, RecognitionException e)
        {
            had_error = true;
            base.Recover(recognizer, e);
        }

        public override IToken RecoverInline(Parser recognizer)
        {
            had_error = true;
            return base.RecoverInline(recognizer);
        }
    }

    public void Parse(FrontEnd pd, bool bail)
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
        ANTLRv2Lexer lexer = new ANTLRv2Lexer(ais);
        CommonTokenStream cts = new CommonTokenStream(lexer);
        ANTLRv2Parser parser = new ANTLRv2Parser(cts);
        lexer.RemoveErrorListeners();
        var lexer_error_listener = new ErrorListener<int>(pd.QuietAfter);
        lexer.AddErrorListener(lexer_error_listener);
        parser.RemoveErrorListeners();
        var parser_error_listener = new ErrorListener<IToken>(pd.QuietAfter);
        parser.AddErrorListener(parser_error_listener);
        BailErrorHandler bail_error_handler = null;
        if (bail)
        {
            bail_error_handler = new BailErrorHandler();
            parser.ErrorHandler = bail_error_handler;
        }

        try
        {
            pt = parser.grammar_();
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

        pd.TokenStream = cts;
        pd.Parser = parser;
        pd.Lexer = lexer;
        pd.Tree = pt;
        //Stack<IParseTree> stack = new Stack<IParseTree>();
        //stack.Push(pt);
        //while (stack.Any())
        //{
        //    var x = stack.Pop();
        //    if (x is TerminalNodeImpl leaf)
        //    {
        //    }
        //    else
        //    {
        //        var y = x as AttributedParseTreeNode;
        //        if (y != null) y.ParserDetails = pd;
        //        for (int i = 0; i < x.ChildCount; ++i)
        //        {
        //            var c = x.GetChild(i);
        //            if (c != null) stack.Push(c);
        //        }
        //    }
        //}
    }

    public void Parse(string code,
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
        ANTLRv2Lexer lexer = new ANTLRv2Lexer(ais);
        CommonTokenStream cts = new CommonTokenStream(lexer);
        ANTLRv2Parser parser = new ANTLRv2Parser(cts);
        lexer.RemoveErrorListeners();
        var lexer_error_listener = new ErrorListener<int>(this.QuietAfter);
        lexer.AddErrorListener(lexer_error_listener);
        parser.RemoveErrorListeners();
        var parser_error_listener = new ErrorListener<IToken>(this.QuietAfter);
        parser.AddErrorListener(parser_error_listener);
        try
        {
            pt = parser.grammar_();
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

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public void GetGrammarBasics()
    {
        // Gather Imports from grammars.
        // Gather InverseImports map.
        if (!InverseImports.ContainsKey(this.FullFileName))
        {
            InverseImports.Add(this.FullFileName, new HashSet<string>());
        }
        if (Tree == null) return;
        ParseTreeWalker.Default.Walk(new Pass0Listener(this), Tree);
        var workspace = this.Item.Workspace;
        foreach (KeyValuePair<string, HashSet<string>> dep in InverseImports)
        {
            string name = dep.Key;
            Workspaces.Document x = workspace.FindDocument(name);
            if (x == null)
            {
                // Add document.
                Workspaces.Container proj = Item.Parent;
                Workspaces.Document new_doc = new Workspaces.Document(name);
                proj.AddChild(new_doc);
            }
        }
    }

    public class Pass0Listener : ANTLRv2ParserBaseListener
    {
        private readonly FrontEnd _pd;
        private bool saw_tokenVocab_option = false;
        private enum GrammarType
        {
            Combined,
            Parser,
            Lexer
        }

        private GrammarType Type;

        public Pass0Listener(FrontEnd pd)
        {
            _pd = pd;
            if (!_pd.InverseImports.ContainsKey(_pd.FullFileName))
            {
                _pd.InverseImports.Add(_pd.FullFileName, new HashSet<string>());
            }
        }

        public override void EnterLexerSpec([NotNull] ANTLRv2Parser.LexerSpecContext context)
        {
            if (Type == GrammarType.Parser)
                Type = GrammarType.Combined;
            else
                Type = GrammarType.Lexer;
        }

        public override void EnterTreeParserSpec([NotNull] ANTLRv2Parser.TreeParserSpecContext context)
        {
        }

        public override void EnterParserSpec([NotNull] ANTLRv2Parser.ParserSpecContext context)
        {
            if (Type == GrammarType.Lexer)
                Type = GrammarType.Combined;
            else
                Type = GrammarType.Parser;
        }

        public override void EnterRules(ANTLRv2Parser.RulesContext context)
        {
            if (saw_tokenVocab_option)
            {
                return;
            }

            // We didn't see an option to include lexer grammar.

            if (Type != GrammarType.Lexer)
            {
                // It's a parser grammar, but we didn't see the tokenVocab option for the lexer.
                // We must assume a lexer grammar in this directory.
                // BUT!!!! There could be many things wrong here, so just don't do this willy nilly.

                string file = _pd.Item.FullPath;
                string dep = file.Replace("Lexer.g2", "Parser.g2");
                if (dep == file)
                {
                    // If the file is not named correctly so that it ends in Parser.g4,
                    // then it's probably a mistake. I don't know where to get the lexer
                    // grammar.
                    return;
                }

                string dir = System.IO.Path.GetDirectoryName(file);
                _pd.Imports.Add(dep);
                if (!_pd.InverseImports.ContainsKey(dep))
                {
                    _pd.InverseImports.Add(dep, new HashSet<string>());
                }

                bool found = false;
                foreach (string f in _pd.InverseImports[dep])
                {
                    if (f == file)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    _pd.InverseImports[dep].Add(file);
                }
            }
            if (Type != GrammarType.Parser)
            {
                // It's a parser grammar, but we didn't see the tokenVocab option for the lexer.
                // We must assume a lexer grammar in this directory.
                // BUT!!!! There could be many things wrong here, so just don't do this willy nilly.

                string file = _pd.Item.FullPath;
                string dep = file.Replace("Parser.g2", "Lexer.g2");
                if (dep == file)
                {
                    // If the file is not named correctly so that it ends in Parser.g4,
                    // then it's probably a mistake. I don't know where to get the lexer
                    // grammar.
                    return;
                }

                string dir = System.IO.Path.GetDirectoryName(file);
                _pd.Imports.Add(dep);
                if (!_pd.InverseImports.ContainsKey(dep))
                {
                    _pd.InverseImports.Add(dep, new HashSet<string>());
                }

                bool found = false;
                foreach (string f in _pd.InverseImports[dep])
                {
                    if (f == file)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    _pd.InverseImports[dep].Add(file);
                }
            }
        }
    }

    public class Pass2Listener : ANTLRv2ParserBaseListener
    {
        private readonly FrontEnd _pd;

        public Pass2Listener(FrontEnd pd)
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

        public override void EnterGrammar_(ANTLRv2Parser.Grammar_Context context)
        {
            _pd.Attributes[context] = new List<CombinedScopeSymbol>() { (CombinedScopeSymbol)_pd.RootScope };
        }

        public override void EnterRule_(ANTLRv2Parser.Rule_Context context)
        {
            var id = context.id();
            var token_ref = id?.TOKEN_REF();
            var rule_ref = id?.RULE_REF();
            var grammar = id?.GRAMMAR();
            var tree = id?.TREE();
            string name = id.GetText();
            ISymbol sym = null;
            if (token_ref != null)
            {
                sym = new TerminalSymbol(name, new List<IToken>() { token_ref.Symbol }) as ISymbol;
            }
            else if (rule_ref != null)
            {
                sym = new NonterminalSymbol(name, new List<IToken>() { rule_ref.Symbol }) as ISymbol;
            }
            else if (grammar != null)
            {
                sym = new NonterminalSymbol(name, new List<IToken>() { grammar.Symbol }) as ISymbol;
            }
            else if (tree != null)
            {
                sym = new NonterminalSymbol(name, new List<IToken>() { tree.Symbol }) as ISymbol;
            }
            _pd.RootScope.define(ref sym);
            CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
            _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
            _pd.Attributes[id] = new List<CombinedScopeSymbol>() { s };
            _pd.Attributes[id.GetChild(0)] = new List<CombinedScopeSymbol>() { s };
        }

        public override void EnterId(ANTLRv2Parser.IdContext context)
        {
            var p = context.Parent;
            var add_def = false;
            for (; p != null; p = p.Parent)
            {
                if (p is ANTLRv2Parser.TokensSpecContext)
                {
                    add_def = true;
                    break;
                }
                if (p is ANTLRv2Parser.Rule_Context)
                    break;
                if (p is ANTLRv2Parser.ElementNoOptionSpecContext)
                    break;
                if (p is ANTLRv2Parser.OptionContext)
                    break;
            }

            if (!add_def)
                return;
            TerminalNodeImpl term = context.GetChild(0) as TerminalNodeImpl;
            string id = term.GetText();
            ISymbol sym = new TerminalSymbol(id, new List<IToken>() { term.Symbol });
            _pd.RootScope.define(ref sym);
            CombinedScopeSymbol s = (CombinedScopeSymbol)sym;
            _pd.Attributes[context] = new List<CombinedScopeSymbol>() { s };
            _pd.Attributes[term] = new List<CombinedScopeSymbol>() { s };
        }
    }

    public class Pass3Listener : ANTLRv2ParserBaseListener
    {
        private readonly FrontEnd _pd;

        public Pass3Listener(FrontEnd pd)
        {
            _pd = pd;
        }

        public override void EnterRule_ref_or_keyword_as([NotNull] ANTLRv2Parser.Rule_ref_or_keyword_asContext context)
        {
            TerminalNodeImpl first = context.GetChild(0) as TerminalNodeImpl;
            if (first?.Symbol.Type == ANTLRv2Parser.RULE_REF)
            {
                string id = context.GetChild(0).GetText();
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
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context.GetChild(0)] = new_attrs;
            }
        }

        public override void EnterId([NotNull] ANTLRv2Parser.IdContext context)
        {
            var p = context.Parent;
            var add_ref = false;
            for (; p != null; p = p.Parent)
            {
                if (p is ANTLRv2Parser.Rule_Context)
                    break;
                if (p is ANTLRv2Parser.OptionContext)
                    break;
            }

            if (!add_ref)
                return;

            TerminalNodeImpl first = context.GetChild(0) as TerminalNodeImpl;
            if (first?.Symbol.Type == ANTLRv2Parser.TOKEN_REF)
            {
                string id = first.GetText();
                List<ISymbol> list = _pd.RootScope.LookupType(id).ToList();
                if (!list.Any())
                {
                    ISymbol sym = new TerminalSymbol(id, new List<IToken>() { first.Symbol });
                    _pd.RootScope.define(ref sym);
                    list = _pd.RootScope.LookupType(id).ToList();
                }
                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { first.Symbol }, list);
                new_attrs.Add(s);
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context.GetChild(0)] = new_attrs;
            }
            if (first?.Symbol.Type == ANTLRv2Parser.RULE_REF
                || first?.Symbol.Type == ANTLRv2Parser.GRAMMAR
                || first?.Symbol.Type == ANTLRv2Parser.TREE
                )
            {
                string id = context.GetChild(0).GetText();
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
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context.GetChild(0)] = new_attrs;
            }
        }

        public override void EnterElementNoOptionSpec([NotNull] ANTLRv2Parser.ElementNoOptionSpecContext context)
        {
            var token_ref = context.TOKEN_REF();
            if (token_ref != null)
            {
                string id = token_ref.GetText();
                List<ISymbol> list = _pd.RootScope.LookupType(id).ToList();
                if (!list.Any())
                {
                    ISymbol sym = new TerminalSymbol(id, new List<IToken>() { token_ref.Symbol });
                    _pd.RootScope.define(ref sym);
                    list = _pd.RootScope.LookupType(id).ToList();
                }
                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { token_ref.Symbol }, list);
                new_attrs.Add(s);
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context.GetChild(0)] = new_attrs;
            }
        }

        public override void EnterRange([NotNull] ANTLRv2Parser.RangeContext context)
        {
            var token_ref = context.TOKEN_REF();
            if (token_ref != null)
            {
                string id = token_ref.GetText();
                List<ISymbol> list = _pd.RootScope.LookupType(id).ToList();
                if (!list.Any())
                {
                    ISymbol sym = new TerminalSymbol(id, new List<IToken>() { token_ref.Symbol });
                    _pd.RootScope.define(ref sym);
                    list = _pd.RootScope.LookupType(id).ToList();
                }
                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { token_ref.Symbol }, list);
                new_attrs.Add(s);
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context.GetChild(0)] = new_attrs;
            }
        }

        public override void EnterNotTerminal([NotNull] ANTLRv2Parser.NotTerminalContext context)
        {
            var token_ref = context.TOKEN_REF();
            if (token_ref != null)
            {
                string id = token_ref.GetText();
                List<ISymbol> list = _pd.RootScope.LookupType(id).ToList();
                if (!list.Any())
                {
                    ISymbol sym = new TerminalSymbol(id, new List<IToken>() { token_ref.Symbol });
                    _pd.RootScope.define(ref sym);
                    list = _pd.RootScope.LookupType(id).ToList();
                }
                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { token_ref.Symbol }, list);
                new_attrs.Add(s);
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context.GetChild(0)] = new_attrs;
            }
        }

        public override void EnterTerminal_([NotNull] ANTLRv2Parser.Terminal_Context context)
        {
            var token_ref = context.TOKEN_REF();
            if (token_ref != null)
            {
                string id = token_ref.GetText();
                List<ISymbol> list = _pd.RootScope.LookupType(id).ToList();
                if (!list.Any())
                {
                    ISymbol sym = new TerminalSymbol(id, new List<IToken>() { token_ref.Symbol });
                    _pd.RootScope.define(ref sym);
                    list = _pd.RootScope.LookupType(id).ToList();
                }
                List<CombinedScopeSymbol> new_attrs = new List<CombinedScopeSymbol>();
                CombinedScopeSymbol s = new RefSymbol(new List<IToken>() { token_ref.Symbol }, list);
                new_attrs.Add(s);
                _pd.Attributes[context] = new_attrs;
                _pd.Attributes[context.GetChild(0)] = new_attrs;
            }
        }
    }
    internal class Reconstruct
    {
        public static void Doit(StringBuilder sb, IParseTree node)
        {
            //if (node is ANTLRv4Parser.ActionBlockContext)
            //{
            //    sb.Append(" { ... }");
            //}
            //else
            if (node is TerminalNodeImpl)
            {
                sb.Append(" " + node.GetText());
            }
            else
            {
                for (int i = 0; i < node.ChildCount; ++i)
                {
                    IParseTree c = node.GetChild(i);
                    Doit(sb, c);
                }
            }
        }
    }

}
