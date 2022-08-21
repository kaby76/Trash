namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Domemtech.Symtab;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ParsingResults : IParserDescription, ICloneable
    {
        public virtual IEnumerable<IParseTree> AllNodes { get; set; } = null;
        public virtual Dictionary<IParseTree, IList<CombinedScopeSymbol>> Attributes { get; set; } = new Dictionary<IParseTree, IList<CombinedScopeSymbol>>();
        public virtual List<bool> CanFindAllRefs
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual List<bool> CanGotodef
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual List<bool> CanGotovisitor
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual bool CanNextRule
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual bool CanReformat
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual List<bool> CanRename
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual bool Changed
        {
            get
            {
                return Item == null ? true : Item.Changed;
            }
        }
        public virtual Func<IParserDescription, Dictionary<IParseTree, IList<CombinedScopeSymbol>>, IParseTree, int> Classify
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual string Code
        {
            get
            {
                return Item?.Code;
            }
        }
        public virtual Dictionary<IToken, int> ColorizedList { get; set; } = new Dictionary<IToken, int>();
        public virtual Dictionary<IToken, int> Comments { get; set; } = new Dictionary<IToken, int>();
        public virtual Dictionary<TerminalNodeImpl, int> Defs { get; set; } = new Dictionary<TerminalNodeImpl, int>();
        public virtual bool DoErrorSquiggles
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual HashSet<IParseTree> Errors { get; set; } = new HashSet<IParseTree>();
        public virtual string FileExtension
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual string FullFileName
        {
            get
            {
                return Item?.FullPath;
            }
        }
        public virtual HashSet<string> Imports { get; set; } = new HashSet<string>();
        public virtual Workspaces.Document Item { get; set; }
        public virtual Antlr4.Runtime.Lexer Lexer { get; set; } = null;
        public virtual string[] Map
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual Antlr4.Runtime.Parser Parser { get; set; } = null;
        public virtual IParseTree ParseTree { get; set; } = null;
        public virtual List<Func<bool>> Passes { get; } = new List<Func<bool>>();
        public virtual List<Func<ParsingResults, IParseTree, string>> PopUpDefinition
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual Dictionary<TerminalNodeImpl, int> PopupList { get; set; } = new Dictionary<TerminalNodeImpl, int>();
        public virtual HashSet<string> PropagateChangesTo { get; set; } = new HashSet<string>();
        public virtual int QuietAfter { get; set; }
        public virtual Dictionary<TerminalNodeImpl, int> Refs { get; set; } = new Dictionary<TerminalNodeImpl, int>();
        public virtual IScope RootScope { get; set; }
        public virtual string StartRule
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public virtual BufferedTokenStream TokStream { get; set; } = null;


        protected static readonly Dictionary<string, IScope> _scopes = new Dictionary<string, IScope>();
        public static Dictionary<string, HashSet<string>> InverseImports = new Dictionary<string, HashSet<string>>();


        public virtual List<string> Candidates(int char_index)
        {
            Workspaces.Document document = Item;
            string ffn = document.FullPath;
            var gd = ParserDescriptionFactory.Create(document);
            if (gd == null)
            {
                throw new Exception();
            }

            string code = Code.Substring(0, char_index);
            gd.Parse(code, out CommonTokenStream tok_stream, out Parser parser, out Lexer lexer, out IParseTree pt);
            LASets la_sets = new LASets();
            IntervalSet int_set = la_sets.Compute(parser, tok_stream);
            List<string> result = new List<string>();
            foreach (int r in int_set.ToList())
            {
                string rule_name = Lexer.RuleNames[r];
                result.Add(rule_name);
            }
            return result;
        }
        
        public virtual void Cleanup()
        {
            string dir = Item.FullPath;
            dir = System.IO.Path.GetDirectoryName(dir);
            _scopes.TryGetValue(dir, out IScope value);
            if (value == null)
            {
                return;
            }

            Nuke(value);
            foreach (IScope s in value.NestedScopes)
            {
                // Nuke all symbols in this scope.
                Nuke(s);
            }
        }

        public abstract object Clone();

        public virtual Dictionary<IToken, int> ExtractComments(string code)
        {
            throw new NotImplementedException();
        }

        public virtual void GatherErrors()
        {
            Workspaces.Document document = Item;
            string ffn = document.FullPath;
            var gd = ParserDescriptionFactory.Create(document);
            if (gd == null)
            {
                throw new Exception();
            }

            {
                IEnumerable<IParseTree> it = AllNodes.Where(t => t as Antlr4.Runtime.Tree.ErrorNodeImpl != null);
                foreach (IParseTree t in it)
                {
                    Errors.Add(t);
                }
            }
        }

        public virtual void GatherRefsDefsAndOthers()
        {
            try
            {
                Workspaces.Document document = Item;
                string ffn = document.FullPath;
                var gd = ParserDescriptionFactory.Create(document);
                if (gd == null)
                {
                    throw new Exception();
                }

                if (AllNodes != null)
                {
                    ColorizedList = new Dictionary<IToken, int>();
                    Refs = new Dictionary<TerminalNodeImpl, int>();
                    Defs = new Dictionary<TerminalNodeImpl, int>();
                    PopupList = new Dictionary<TerminalNodeImpl, int>();
                    Func<IParserDescription, Dictionary<IParseTree, IList<CombinedScopeSymbol>>, IParseTree, int> fun = gd.Classify;
                    IEnumerable<IParseTree> it = AllNodes.Where(n => n is TerminalNodeImpl);
                    foreach (var n in it)
                    {
                        var t = n as TerminalNodeImpl;
                        int i = -1;
                        try
                        {
                            i = gd.Classify(gd, Attributes, t);
                            if (i >= 0)
                            {
                                ColorizedList.Add(t.Symbol, i);
                            }
                        }
                        catch (Exception) { }
                        try
                        {
                            if (i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationNonterminalRef
                                || i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationTerminalRef
                                || i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationModeRef
                                || i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationChannelRef
                                )
                            {
                                Refs.Add(t, i);
                                PopupList.Add(t, i);
                            }
                        }
                        catch (Exception) { }
                        try
                        {
                            if (i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationNonterminalDef
                                || i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationTerminalDef
                                || i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationModeDef
                                || i == (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationChannelDef
                                )
                            {
                                Defs.Add(t, i);
                                PopupList.Add(t, i);
                            }
                        }
                        catch (Exception) { }
                    }
                }

                if (Comments != null)
                {
                    foreach (KeyValuePair<Antlr4.Runtime.IToken, int> p in Comments)
                    {
                        IToken t = p.Key;
                        ColorizedList.Add(t, (int)LanguageServer.Antlr4ParsingResults.AntlrClassifications.ClassificationComment);
                    }
                }
            }
#pragma warning disable 0168
            catch (Exception eeks) { }
#pragma warning restore 0168
        }

        public abstract bool IsFileType(string ffn);

        private void Nuke(IScope scope)
        {
            if (scope == null)
            {
                return;
            }

            foreach (IScope c in scope.NestedScopes)
            {
                Nuke(c);
            }
            IList<ISymbol> copy = scope.Symbols;
            foreach (ISymbol s in copy)
            {
                if (s.file == Item.FullPath)
                {
                    scope.remove(s);
                }
            }
        }

        public ParsingResults(Workspaces.Document item)
        {
            Item = item;
            Item.Changed = true;
        }

        public virtual void Parse()
        {
            Workspaces.Document document = Item;
            string code = document.Code;
            string ffn = document.FullPath;
            bool has_changed = document.Changed;
            document.Changed = false;
            if (!has_changed)
            {
                return;
            }

            var gd = ParserDescriptionFactory.Create(document);
            if (gd == null)
            {
                throw new Exception();
            }

            bool bail = QuietAfter == 0;
            gd.Parse(this, bail);

            document.ParseTree = ParseTree;
            document.Parser = gd.Parser;
            document.Lexer = gd.Lexer;
            AllNodes = DFSVisitor.DFS(ParseTree as ParserRuleContext);
            Comments = gd.ExtractComments(code);
            Defs = new Dictionary<TerminalNodeImpl, int>();
            Refs = new Dictionary<TerminalNodeImpl, int>();
            PopupList = new Dictionary<TerminalNodeImpl, int>();
            Errors = new HashSet<IParseTree>();
            Imports = new HashSet<string>();
            Attributes = new Dictionary<IParseTree, IList<CombinedScopeSymbol>>();
            ColorizedList = new Dictionary<Antlr4.Runtime.IToken, int>();
            Cleanup();
        }

        public abstract void Parse(ParsingResults pd, bool bail);

        public abstract void Parse(string code, out CommonTokenStream TokStream, out Parser Parser, out Lexer Lexer, out IParseTree ParseTree);

        public bool Pass(int pass_number)
        {
            return Passes[pass_number]();
        }

        public abstract void GetGrammarBasics();

    }
}
