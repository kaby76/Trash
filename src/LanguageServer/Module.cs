namespace LanguageServer
{
    using Algorithms;
    using Antlr4.Runtime.Tree;
    using LoggerNs;
    using Microsoft.CodeAnalysis;
    using Domemtech.Symtab;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Workspaces;

    static class Foobar
    {
        public static HashSet<ParsingResults> Subtract(this HashSet<ParsingResults> a, HashSet<ParsingResults> b)
        {
            HashSet<ParsingResults> result = new HashSet<ParsingResults>();
            foreach (var t in a)
            {
                if (b.Contains(t)) continue;
                result.Add(t);
            }
            return result;
        }
    }

    public class Module
    {

        public enum RecoveryStrategy { Bail, Standard };

        private void ComputeIndexes(Document doc)
        {
            List<int> indices = new List<int>();
            int cur_index = 0;
            int cur_line = 0; // zero based LSP.
            int cur_col = 0; // zero based LSP.
            indices.Add(cur_index);
            string buffer = doc.Code;
            int length = doc.Code.Length;
            // Go through file and record index of start of each line.
            for (int i = 0; i < length; ++i)
            {
                if (cur_index >= length)
                {
                    break;
                }

                char ch = buffer[cur_index];
                if (ch == '\r')
                {
                    if (cur_index + 1 >= length)
                    {
                        break;
                    }
                    else if (buffer[cur_index + 1] == '\n')
                    {
                        cur_line++;
                        cur_col = 0;
                        cur_index += 2;
                        indices.Add(cur_index);
                    }
                    else
                    {
                        // Error in code.
                        cur_line++;
                        cur_col = 0;
                        cur_index += 1;
                        indices.Add(cur_index);
                    }
                }
                else if (ch == '\n')
                {
                    cur_line++;
                    cur_col = 0;
                    cur_index += 1;
                    indices.Add(cur_index);
                }
                else
                {
                    cur_col += 1;
                    cur_index += 1;
                }
                if (cur_index >= length)
                {
                    break;
                }
            }
            doc.Indices = indices.ToArray();
        }

        public int GetIndex(int line, int column, Document doc)
        {
            string buffer = doc.Code;
            if (buffer == null)
            {
                return 0;
            }

            if (doc.Indices == null) ComputeIndexes(doc);

            int low = doc.Indices[line];
            var index = low + column;
            return index;
        }

        public (int, int) GetLineColumn(int index, Document doc)
        {
            string buffer = doc.Code;
            if (buffer == null)
            {
                return (0, 0);
            }

            if (doc.Indices == null) ComputeIndexes(doc);

            // Binary search.
            int low = 0;
            int high = doc.Indices.Length - 1;
            int i = 0;
            while (low <= high)
            {
                i = (low + high) / 2;
                var v = doc.Indices[i];
                if (v < index) low = i + 1;
                else if (v > index) high = i - 1;
                else break;
            }
            var min = low <= high ? i : high;
            var myindex = (min, index - doc.Indices[min]);
            return myindex;
        }

        public QuickInfo GetQuickInfo(int index, Document doc)
        {
            ParsingResults pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (pd.ParseTree == null)
            {
                Compile(workspace);
            }

            Antlr4.Runtime.Tree.IParseTree pt = LanguageServer.Util.Find(index, doc);
            var gd = ParserDescriptionFactory.Create(doc);
            if (pt == null)
            {
                return null;
            }

            Antlr4.Runtime.Tree.IParseTree p = pt;
            pd.Attributes.TryGetValue(p, out IList<CombinedScopeSymbol> list_value);
            if (list_value == null)
            {
                return null;
            }

            TerminalNodeImpl q = p as Antlr4.Runtime.Tree.TerminalNodeImpl;
            var range = new Workspaces.Range(new Workspaces.Index(q.Symbol.StartIndex), new Workspaces.Index(q.Symbol.StopIndex + 1));
            bool found = pd.PopupList.TryGetValue(q, out int tag_type);
            if (!found)
            {
                return null;
            }

            if (list_value == null || list_value.Count == 0)
            {
                return new QuickInfo() { Display = gd.Map[tag_type], Range = range };
            }
            if (list_value.Count == 1)
            {
                CombinedScopeSymbol value = list_value.First();
                Domemtech.Symtab.ISymbol name = value as Domemtech.Symtab.ISymbol;
                string show = name?.Name;
                if (value is Domemtech.Symtab.Literal)
                {
                    show = ((Domemtech.Symtab.Literal)value).Cleaned;
                }
                if (gd.PopUpDefinition[tag_type] != null)
                {
                    Func<ParsingResults, IParseTree, string> fun = gd.PopUpDefinition[tag_type];
                    string mess = fun(pd, p);
                    if (mess != null)
                    {
                        return new QuickInfo() { Display = mess, Range = range };
                    }
                }
                string display = gd.Map[tag_type]
                    + "\n"
                    + show;
                return new QuickInfo() { Display = display, Range = range };
            }
            {
                string display = "Ambiguous -- ";
                foreach (CombinedScopeSymbol value in list_value)
                {
                    Domemtech.Symtab.ISymbol name = value as Domemtech.Symtab.ISymbol;
                    string show = name?.Name;
                    if (value is Domemtech.Symtab.Literal)
                    {
                        show = ((Domemtech.Symtab.Literal)value).Cleaned;
                    }
                    if (gd.PopUpDefinition[tag_type] != null)
                    {
                        Func<ParsingResults, IParseTree, string> fun = gd.PopUpDefinition[tag_type];
                        string mess = fun(pd, p);
                        if (mess != null)
                        {
                            display = display + mess;
                        }
                    }
                    else
                    {
                        display = display + gd.Map[tag_type]
                            + "\n"
                            + show;
                    }
                }
                return new QuickInfo() { Display = display, Range = range };
            }
        }

        public int GetTag(int index, Document doc)
        {
            ParsingResults pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (pd.ParseTree == null)
            {
                Compile(workspace);
            }

            Antlr4.Runtime.Tree.IParseTree pt = LanguageServer.Util.Find(index, doc);
            var gd = ParserDescriptionFactory.Create(doc);
            if (pt == null)
            {
                return -1;
            }

            Antlr4.Runtime.Tree.IParseTree p = pt;
            TerminalNodeImpl q = p as Antlr4.Runtime.Tree.TerminalNodeImpl;
            bool found = pd.PopupList.TryGetValue(q, out int tag_type);
            if (found)
            {
                return tag_type;
            }

            if (q.Symbol == null)
            {
                return -1;
            }

            bool found2 = pd.Comments.TryGetValue(q.Symbol, out int tag2);
            if (found2)
            {
                return tag2;
            }

            return -1;
        }

        public DocumentSymbol GetDocumentSymbol(int index, Document doc)
        {
            ParsingResults pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (pd.ParseTree == null)
            {
                Compile(workspace);
            }

            Antlr4.Runtime.Tree.IParseTree pt = LanguageServer.Util.Find(index, doc);
            var gd = ParserDescriptionFactory.Create(doc);
            if (pt == null)
            {
                return default(DocumentSymbol);
            }

            Antlr4.Runtime.Tree.IParseTree p = pt;
            TerminalNodeImpl q = p as Antlr4.Runtime.Tree.TerminalNodeImpl;
            bool found = pd.PopupList.TryGetValue(q, out int tag_type);
            if (!found)
            {
                return null;
            }

            if (q.Symbol == null)
            {
                return null;
            }

            return new DocumentSymbol()
            {
                name = q.Symbol.Text,
                range = new Workspaces.Range(q.Symbol.StartIndex, q.Symbol.StopIndex),
                kind = tag_type
            };
        }

        public IEnumerable<DocumentSymbol> GetSymbols(Document doc)
        {
            ParsingResults pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (pd.ParseTree == null)
            {
                Compile(workspace);
            }

            List<DocumentSymbol> combined = new List<DocumentSymbol>();
            foreach (KeyValuePair<TerminalNodeImpl, int> p in pd.Defs)
            {
                if (p.Key == null)
                {
                    continue;
                }

                combined.Add(
                    new DocumentSymbol()
                    {
                        name = p.Key.GetText(),
                        range = new Workspaces.Range(p.Key.Payload.StartIndex, p.Key.Payload.StopIndex),
                        kind = p.Value
                    });
            }
            foreach (KeyValuePair<TerminalNodeImpl, int> p in pd.Refs)
            {
                if (p.Key == null)
                {
                    continue;
                }

                combined.Add(
                    new DocumentSymbol()
                    {
                        name = p.Key.GetText(),
                        range = new Workspaces.Range(p.Key.Payload.StartIndex, p.Key.Payload.StopIndex),
                        kind = p.Value
                    });
            }

            // Sort the list.
            IOrderedEnumerable<DocumentSymbol> sorted_combined_tokens = combined.OrderBy(t => t.range.Start.Value).ThenBy(t => t.range.End.Value);
            return sorted_combined_tokens;
        }

        public class Info
        {
            public int start;
            public int end;
            public int kind; 
        }

        public IEnumerable<Info> Get(int start, int end, Document doc)
        {
            try
            {
                ParsingResults pd = ParsingResultsFactory.Create(doc);
                var workspace = doc.Workspace;
                if (pd.ParseTree == null)
                {
                    Compile(workspace);
                }

                List<Info> combined = new List<Info>();
                foreach (KeyValuePair<Antlr4.Runtime.IToken, int> p in pd.ColorizedList)
                {
                    if (p.Key == null)
                    {
                        continue;
                    }
                    var sym = p.Key;
                    var st = sym.StartIndex;
                    var en = sym.StopIndex + 1;
                    if (end < st) continue;
                    if (en < start) continue;
                    int s1 = st > start ? st : start;
                    int s2 = en < end ? en : end;
                    combined.Add(
                         new Info()
                         {
                             start = s1,
                             end = s2,
                             kind = p.Value
                         });
                    ;
                }
                // Sort the list.
                IOrderedEnumerable<Info> sorted_combined_tokens = combined.OrderBy(t => t.start).ThenBy(t => t.end);
                return sorted_combined_tokens;
            }
            catch (Exception)
            {
            }
            return new List<Info>();
        }

        public IEnumerable<Info> Get(Document doc)
        {
            try
            {
                ParsingResults pd = ParsingResultsFactory.Create(doc);
                var workspace = doc.Workspace;
                if (pd.ParseTree == null)
                {
                    Compile(workspace);
                }

                List<Info> combined = new List<Info>();
                foreach (KeyValuePair<Antlr4.Runtime.IToken, int> p in pd.ColorizedList)
                {
                    if (p.Key == null)
                    {
                        continue;
                    }
                    var sym = p.Key;
                    var st = sym.StartIndex;
                    var en = sym.StopIndex + 1;
                    int s1 = st;
                    int s2 = en;
                    // Create multiple "info" for multiline tokens.
                    var (ls, cs) = new LanguageServer.Module().GetLineColumn(st, doc);
                    var (le, ce) = new LanguageServer.Module().GetLineColumn(en, doc);
                    if (ls == le)
                    {
                         combined.Add(
                             new Info()
                             {
                                 start = s1,
                                 end = s2 - 1,
                                 kind = p.Value
                             });
                    }
                    else
                    {
                        var text = sym.Text;
                        int start_region = st;
                        for (int cur_index = 0; cur_index < text.Length; )
                        {
                            if (text[cur_index] == '\n' || text[cur_index] == '\r')
                            {
                                // Emit Info().
                                if (text[cur_index] == '\r' && (cur_index + 1 < text.Length) && text[cur_index + 1] == '\n')
                                    cur_index++;
                                cur_index++;
                                combined.Add(
                                    new Info()
                                    {
                                        start = start_region,
                                        end = st + cur_index - 1,
                                        kind = p.Value
                                    });
                                start_region = st + cur_index;
                            }
                            else
                                cur_index++;
                        }
                        if (start_region != en)
                        {
                            combined.Add(
                                new Info()
                                {
                                    start = start_region,
                                    end = en - 1,
                                    kind = p.Value
                                });
                        }
                    }
                }
                // Sort the list.
                IOrderedEnumerable<Info> sorted_combined_tokens = combined.OrderBy(t => t.start).ThenBy(t => t.end);
                return sorted_combined_tokens;
            }
            catch (Exception)
            {
            }
            return new List<Info>();
        }

        public IEnumerable<Workspaces.Range> GetErrors(Workspaces.Range range, Document doc)
        {
            ParsingResults pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (pd.ParseTree == null)
            {
                Compile(workspace);
            }

            var result = new List<Workspaces.Range>();
            foreach (IParseTree p in pd.Errors)
            {
                ErrorNodeImpl q = p as Antlr4.Runtime.Tree.ErrorNodeImpl;
                if (q == null)
                {
                    continue;
                }

                if (q.Payload == null)
                {
                    continue;
                }

                int y = q.Payload.StartIndex;
                int z = q.Payload.StopIndex;
                if (y < 0)
                {
                    y = 0;
                }

                if (z < 0)
                {
                    z = 0;
                }

                int a = y;
                int b = z + 1;
                int start_token_start = a;
                int end_token_end = b;
                if (start_token_start > range.End.Value)
                {
                    continue;
                }

                if (end_token_end < range.Start.Value)
                {
                    continue;
                }

                var r = new Workspaces.Range(new Workspaces.Index(a), new Workspaces.Index(b));
                result.Add(r);
            }
            return result;
        }

        public IList<Location> FindDefs(int index, Document doc)
        {
            List<Location> result = new List<Location>();
            if (doc == null)
            {
                return result;
            }
            var workspace = doc.Workspace;

            IParseTree ref_pt = Util.Find(index, doc);
            if (ref_pt == null)
            {
                return result;
            }

            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            if (ref_pd.ParseTree == null)
            {
                Compile(workspace);
            }

            ref_pd.Attributes.TryGetValue(ref_pt, out IList<Domemtech.Symtab.CombinedScopeSymbol> list_values);
            if (list_values == null)
            {
                return result;
            }

            foreach (CombinedScopeSymbol value in list_values)
            {
                if (value == null)
                {
                    continue;
                }

                Domemtech.Symtab.ISymbol @ref = value as Domemtech.Symtab.ISymbol;
                if (@ref == null)
                {
                    continue;
                }

                List<Domemtech.Symtab.ISymbol> defs = @ref.resolve();
                if (defs == null)
                {
                    continue;
                }

                foreach (var def in defs)
                {
                    string def_file = def.file;
                    if (def_file == null)
                    {
                        continue;
                    }

                    Document def_item = workspace.FindDocument(def_file);
                    if (def_item == null)
                    {
                        continue;
                    }

                    Location new_loc = new Location()
                    {
                        Range = new Workspaces.Range(def.Token.First().StartIndex, def.Token.First().StopIndex),
                        Uri = def_item
                    };
                    result.Add(new_loc);
                }
            }
            return result;
        }

        public IEnumerable<Location> FindRefsAndDefs(int index, Document doc)
        {
            List<Location> result = new List<Location>();
            IParseTree ref_pt = Util.Find(index, doc);
            if (ref_pt == null)
            {
                return result;
            }
            var workspace = doc.Workspace;

            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            if (ref_pd.ParseTree == null)
            {
                Compile(workspace);
            }

            ref_pd.Attributes.TryGetValue(ref_pt, out IList<Domemtech.Symtab.CombinedScopeSymbol> list_value);
            if (list_value == null)
            {
                return result;
            }

            List<Domemtech.Symtab.ISymbol> found_defs = null;
            Domemtech.Symtab.ISymbol found_ref = null;
            foreach (Domemtech.Symtab.CombinedScopeSymbol value in list_value)
            {
                if (value == null)
                {
                    continue;
                }

                Domemtech.Symtab.ISymbol @ref = value as Domemtech.Symtab.ISymbol;
                if (@ref == null)
                {
                    continue;
                }

                if (@ref.Token == null)
                {
                    continue;
                }

                found_ref = @ref;
                List<Domemtech.Symtab.ISymbol> defs = @ref.resolve();
                if (defs == null)
                {
                    continue;
                }
                found_defs = defs;
                break;
            }
            if (found_defs != null)
            {
                foreach (var def in found_defs)
                {
                    result.Add(
                        new Location()
                        {
                            Range = new Workspaces.Range(def.Token.First().StartIndex, def.Token.First().StopIndex),
                            Uri = workspace.FindDocument(def.file)
                        });
                    var dd = def as BaseSymbol;
                    foreach (var r in dd.Refs)
                    {
                        result.Add(
                            new Location()
                            {
                                Range = new Workspaces.Range(r.Token.First().StartIndex, r.Token.First().StopIndex),
                                Uri = workspace.FindDocument(r.file)
                            });
                    }
                }
            }
            return result;
        }

        public IEnumerable<Location> FindRefsAndDefs(string name, Document doc)
        {
            List<Location> result = new List<Location>();
            var workspace = doc.Workspace;

            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            if (ref_pd.ParseTree == null)
            {
                Compile(workspace);
            }

            IList<CombinedScopeSymbol> list_value =
                ref_pd.Attributes.Where(p =>
                {
                    IParseTree x = p.Key;
                    IList<CombinedScopeSymbol> y = p.Value;
                    if (x is TerminalNodeImpl term)
                    {
                        return term.Symbol != null && term.Symbol.Text == name;
                    }
                    else return false;
                }).Select(p => p.Value).SelectMany(p => p).ToList();

            List<Domemtech.Symtab.ISymbol> found_defs = null;
            Domemtech.Symtab.ISymbol found_ref = null;
            foreach (Domemtech.Symtab.CombinedScopeSymbol value in list_value)
            {
                if (value == null)
                {
                    continue;
                }

                Domemtech.Symtab.ISymbol @ref = value as Domemtech.Symtab.ISymbol;
                if (@ref == null)
                {
                    continue;
                }

                if (@ref.Token == null)
                {
                    continue;
                }

                found_ref = @ref;
                List<Domemtech.Symtab.ISymbol> defs = @ref.resolve();
                if (defs == null)
                {
                    continue;
                }
                found_defs = defs;
                break;
            }
            if (found_defs != null)
            {
                foreach (var def in found_defs)
                {
                    result.Add(
                        new Location()
                        {
                            Range = new Workspaces.Range(def.Token.First().StartIndex, def.Token.First().StopIndex),
                            Uri = workspace.FindDocument(def.file)
                        });
                    var dd = def as BaseSymbol;
                    foreach (var r in dd.Refs)
                    {
                        result.Add(
                            new Location()
                            {
                                Range = new Workspaces.Range(r.Token.First().StartIndex, r.Token.First().StopIndex),
                                Uri = workspace.FindDocument(r.file)
                            });
                    }
                }
            }
            return result;
        }

        public IEnumerable<Location> GetDefs(Document doc)
        {
            List<Location> result = new List<Location>();
            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (ref_pd.ParseTree == null)
            {
                Compile(workspace);
            }

            foreach (KeyValuePair<TerminalNodeImpl, int> value in ref_pd.Defs)
            {
                TerminalNodeImpl key = value.Key;
                Antlr4.Runtime.IToken sym = key.Payload;
                result.Add(
                    new Location()
                    {
                        Range = new Workspaces.Range(sym.StartIndex, sym.StopIndex),
                        Uri = workspace.FindDocument(sym.InputStream.SourceName)
                    });
            }
            return result;
        }

        public IEnumerable<TerminalNodeImpl> GetRefsLeaf(Document doc)
        {
            List<TerminalNodeImpl> result = new List<TerminalNodeImpl>();
            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (ref_pd.ParseTree == null)
            {
                Compile(workspace);
            }

            foreach (KeyValuePair<TerminalNodeImpl, int> value in ref_pd.Refs)
            {
                TerminalNodeImpl key = value.Key;
                result.Add(key);
            }
            return result;
        }

        public IEnumerable<TerminalNodeImpl> GetDefsLeaf(Document doc)
        {
            List<TerminalNodeImpl> result = new List<TerminalNodeImpl>();
            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            var workspace = doc.Workspace;
            if (ref_pd.ParseTree == null)
            {
                Compile(workspace);
            }

            foreach (KeyValuePair<TerminalNodeImpl, int> value in ref_pd.Defs)
            {
                TerminalNodeImpl key = value.Key;
                result.Add(key);
            }
            return result;
        }

        private Digraph<ParsingResults> ConstructGraph(IEnumerable<ParsingResults> to_do)
        {
            Digraph<ParsingResults> g = new Digraph<ParsingResults>();
            HashSet<ParsingResults> done = new HashSet<ParsingResults>();
            Stack<ParsingResults> stack = new Stack<ParsingResults>();
            foreach (ParsingResults f in to_do)
            {
                stack.Push(f);
            }

            while (stack.Count > 0)
            {
                ParsingResults f = stack.Pop();
                var workspace = f.Item.Workspace;
                g.AddVertex(f);
                done.Add(f);
                foreach (string d in f.PropagateChangesTo)
                {
                    Document d_doc = workspace.FindDocument(d);
                    ParsingResults d_pd = ParsingResultsFactory.Create(d_doc);
                    if (done.Contains(d_pd))
                    {
                        continue;
                    }

                    stack.Push(d_pd);
                }
            }

            foreach (ParsingResults v in g.Vertices)
            {
                HashSet<string> deps = v.PropagateChangesTo;
                var workspace = v.Item.Workspace;
                Document doc = workspace.FindDocument(v.FullFileName);
                ParsingResults pd = ParsingResultsFactory.Create(doc);
                foreach (string d in deps)
                {
                    Document d_doc = workspace.FindDocument(d);
                    ParsingResults d_pd = ParsingResultsFactory.Create(d_doc);
                    g.AddEdge(new DirectedEdge<ParsingResults>() { From = pd, To = d_pd });
                }
            }

            return g;
        }

        public List<ParsingResults> Compile(Workspace workspace, bool reparse = true)
        {
            try
            {
                // Parse changed files. If it's a combined grammar, no need to read
                // other files. If it's a parser grammar, get the lexer grammar and read
                // that. If it's a lexer grammar, read the parser grammar.
                // This has to be done intelligently. We don't want to parse every damn
                // file here. It's not going to be useful, and some files may not even work.

                // Let's find all changed files. Assume that the "workspace" contains
                // changed files. If it's not in the workspace already, then the file
                // has not changed, so we are not interested in it--unless it is a
                // depedency.
            DoAgain:

                HashSet<ParsingResults> to_do = new HashSet<ParsingResults>();
                HashSet<ParsingResults> new_to_do = new HashSet<ParsingResults>();
                foreach (Document document in Workspaces.DFSContainer.DFS(workspace))
                {
                    if (document.ParseTree != null && !document.Changed) continue;
                    string file_name = document.FullPath;
                    if (file_name == null) continue;
                    Container parent = document.Parent;
                    ParsingResults v = LanguageServer.ParserDescriptionFactory.Create(document);
                    if (v == null && document.ParseAs == null) continue;
                    if (new_to_do.Contains(v)) continue;
                    new_to_do.Add(v);
                }

                for (; ;)
                {
                    HashSet<ParsingResults> diff = new_to_do.Subtract(to_do);
                    to_do = new_to_do;
                    if (diff.Count == 0) break;

                    // Do a quick bail-mode parse to determine Imports, InverseImports, and 
                    // workspace changes.
                    foreach (ParsingResults v in diff)
                    {
                        if (reparse) v.Parse();
                        v.GetGrammarBasics();
                    }

                    // Add all files to workspace.
                    new_to_do = new HashSet<ParsingResults>();
                    foreach (KeyValuePair<string, HashSet<string>> dep in ParsingResults.InverseImports)
                    {
                        string name = dep.Key;
                        Workspaces.Document x = workspace.FindDocument(name);
                        ParsingResults v = LanguageServer.ParserDescriptionFactory.Create(x);
                        new_to_do.Add(v);
                    }
                }

                // Sort the grammars files based on includes to derive the symbol table.
                Digraph<ParsingResults> g = ConstructGraph(to_do);
                bool changed = true;
                for (int pass = 0; changed; pass++)
                {
                    changed = false;
                    foreach (ParsingResults v in g.Vertices)
                    {
                        int number_of_passes = v.Passes.Count;
                        if (pass < number_of_passes)
                        {
                            try
                            {
                                bool reset = v.Pass(pass);
                                if (reset)
                                {
                                    goto DoAgain;
                                }
                            }
#pragma warning disable 0168
                            catch (Exception eeks) { }
#pragma warning restore 0168
                            changed = true;
                        }
                    }
                }
                foreach (ParsingResults v in g.Vertices)
                {
                    v.GatherRefsDefsAndOthers();
                }
                foreach (ParsingResults v in g.Vertices)
                {
                    v.GatherErrors();
                }
                return g.Vertices.ToList();
            }
            catch (Exception e)
            {
                Logger.Log.Notify(e.ToString());
            }
            return new List<ParsingResults>();
        }

        public static Dictionary<string, string> Format(Document doc)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            string code = doc.Code;
            string corpus_location = Options.Option.GetString("CorpusLocation");
            if (corpus_location == null)
            {
                return result;
            }

            string ffn = doc.FullPath;
            if (ffn == null)
            {
                return result;
            }
            var grammar_description = LanguageServer.ParserDescriptionFactory.Create(doc);
            if (grammar_description == null)
            {
                return result;
            }
            org.antlr.codebuff.Tool.unformatted_input = code;
            try
            {
                string result2 = org.antlr.codebuff.Tool.Main(
                    new object[]
                    {
                "-g", grammar_description.Name,
                "-lexer", grammar_description.Lexer.GetType(),
                "-parser", grammar_description.Parser.GetType(),
                "-rule", grammar_description.StartRule,
                "-files", grammar_description.FileExtension,
                "-corpus", corpus_location,
                "-inoutstring",
                ""
                    });
                result.Add(ffn, result2);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public TextEdit[] Reformat(Document doc)
        {
            TextEdit[] result = new TextEdit[] { };
            ParsingResults ref_pd = ParsingResultsFactory.Create(doc);
            string code = doc.Code;
            string corpus_location = Options.Option.GetString("CorpusLocation");
            if (corpus_location == null)
            {
                return result;
            }

            string ffn = doc.FullPath;
            if (ffn == null)
            {
                return result;
            }
            var grammar_description = LanguageServer.ParserDescriptionFactory.Create(doc);
            if (grammar_description == null)
            {
                return result;
            }
            org.antlr.codebuff.Tool.unformatted_input = code;
            try
            {
                string result2 = org.antlr.codebuff.Tool.Main(
                    new object[]
                    {
                "-g", grammar_description.Name,
                "-lexer", grammar_description.Lexer,
                "-parser", grammar_description.Parser,
                "-rule", grammar_description.StartRule,
                "-files", grammar_description.FileExtension,
                "-corpus", corpus_location,
                "-inoutstring",
                ""
                    });
                List<TextEdit> edits = new List<TextEdit>();
                Diff_match_patch diff = new Diff_match_patch();
                List<Diff> diffs = diff.Diff_main(code, result2);
                List<Patch> patch = diff.Patch_make(diffs);
                //patch.Reverse();

                // Start edit session.
                int times = 0;
                int delta = 0;
                foreach (Patch p in patch)
                {
                    times++;
                    int start = p.start1 - delta;

                    int offset = 0;
                    foreach (Diff ed in p.diffs)
                    {
                        if (ed.operation == Operation.EQUAL)
                        {
                            //// Let's verify that.
                            int len = ed.text.Length;
                            //var tokenSpan = new SnapshotSpan(buffer.CurrentSnapshot,
                            //  new Span(start + offset, len));
                            //var tt = tokenSpan.GetText();
                            //if (ed.text != tt)
                            //{ }
                            offset = offset + len;
                        }
                        else if (ed.operation == Operation.DELETE)
                        {
                            int len = ed.text.Length;
                            //var tokenSpan = new SnapshotSpan(buffer.CurrentSnapshot,
                            //  new Span(start + offset, len));
                            //var tt = tokenSpan.GetText();
                            //if (ed.text != tt)
                            //{ }
                            TextEdit edit = new TextEdit()
                            {
                                range = new Workspaces.Range(
                                    new Workspaces.Index(start + offset),
                                    new Workspaces.Index(start + offset + len)),
                                NewText = ""
                            };
                            offset = offset + len;
                            edits.Add(edit);
                        }
                        else if (ed.operation == Operation.INSERT)
                        {
                            int len = ed.text.Length;
                            TextEdit edit = new TextEdit()
                            {
                                range = new Workspaces.Range(
                                    new Workspaces.Index(start + offset),
                                    new Workspaces.Index(start + offset)),
                                NewText = ed.text
                            };
                            edits.Add(edit);
                        }
                    }
                    delta = delta + (p.length2 - p.length1);
                }
                return edits.ToArray();
            }
            catch (Exception)
            {
                return result;
            }
        }

        public Dictionary<string, TextEdit[]> Rename(int index, string new_text, Document doc)
        {
            IEnumerable<Location> locations = FindRefsAndDefs(index, doc);
            Dictionary<string, TextEdit[]> result = new Dictionary<string, TextEdit[]>();
            var x1 = locations.Select(r => r.Uri).ToList();
            var documents = x1.Distinct().ToList();
            foreach (Document f in documents)
            {
                string fn = f.FullPath;
                IOrderedEnumerable<Location> per_file_changes = locations.Where(z => z.Uri == f).OrderBy(q => q.Range.Start.Value);
                StringBuilder sb = new StringBuilder();
                int previous = 0;
                string code = f.Code;
                foreach (Location l in per_file_changes)
                {
                    Document d = l.Uri;
                    string xx = d.FullPath;
                    var r = l.Range;
                    string pre = code.Substring(previous, r.Start.Value - previous);
                    sb.Append(pre);
                    sb.Append(new_text);
                    previous = r.End.Value + 1;
                }
                string rest = code.Substring(previous);
                sb.Append(rest);
                string new_code = sb.ToString();
                List<TextEdit> edits = new List<TextEdit>();
                Diff_match_patch diff = new Diff_match_patch();
                List<Diff> diffs = diff.Diff_main(code, new_code);
                List<Patch> patch = diff.Patch_make(diffs);
                int times = 0;
                int delta = 0;
                foreach (Patch p in patch)
                {
                    times++;
                    int start = p.start1 - delta;
                    int offset = 0;
                    foreach (Diff ed in p.diffs)
                    {
                        if (ed.operation == Operation.EQUAL)
                        {
                            //// Let's verify that.
                            int len = ed.text.Length;
                            //var tokenSpan = new SnapshotSpan(buffer.CurrentSnapshot,
                            //  new Span(start + offset, len));
                            //var tt = tokenSpan.GetText();
                            //if (ed.text != tt)
                            //{ }
                            offset = offset + len;
                        }
                        else if (ed.operation == Operation.DELETE)
                        {
                            int len = ed.text.Length;
                            //var tokenSpan = new SnapshotSpan(buffer.CurrentSnapshot,
                            //  new Span(start + offset, len));
                            //var tt = tokenSpan.GetText();
                            //if (ed.text != tt)
                            //{ }
                            TextEdit edit = new TextEdit()
                            {
                                range = new Workspaces.Range(
                                    new Workspaces.Index(start + offset),
                                    new Workspaces.Index(start + offset + len)),
                                NewText = ""
                            };
                            offset = offset + len;
                            edits.Add(edit);
                        }
                        else if (ed.operation == Operation.INSERT)
                        {
                            int len = ed.text.Length;
                            TextEdit edit = new TextEdit()
                            {
                                range = new Workspaces.Range(
                                    new Workspaces.Index(start + offset),
                                    new Workspaces.Index(start + offset)),
                                NewText = ed.text
                            };
                            edits.Add(edit);
                        }
                    }
                    delta = delta + (p.length2 - p.length1);
                }
                TextEdit[] e = edits.ToArray();
                result.Add(fn, e);
            }
            return result;
        }

        public List<string> Completion(int char_index, Document document)
        {
            ParsingResults ref_pd = ParsingResultsFactory.Create(document);
            var workspace = document.Workspace;
            if (ref_pd.ParseTree == null)
            {
                Compile(workspace);
            }

            List<string> result = ref_pd.Candidates(char_index);
            return result;
        }

        public string AntlrVersion()
        {
            Type parser_type = typeof(ANTLRv4Parser);
            System.CodeDom.Compiler.GeneratedCodeAttribute MyAttribute =
                (System.CodeDom.Compiler.GeneratedCodeAttribute)Attribute.GetCustomAttribute(parser_type, typeof(System.CodeDom.Compiler.GeneratedCodeAttribute));
            return MyAttribute?.Version;
        }

        public static Document OpenAndParse(string code)
        {
            Workspace _workspace = new Workspace();
            Document document = Document.CreateStringDocument(code);
            _ = ParsingResultsFactory.Create(document);
            var workspace = document.Workspace;
            _ = new LanguageServer.Module().Compile(workspace);
            Project project = _workspace.FindProject("Misc");
            if (project == null)
            {
                project = new Project("Misc", "Misc", "Misc");
                _workspace.AddChild(project);
            }
            project.AddDocument(document);
            var pr = LanguageServer.ParsingResultsFactory.Create(document);
            if (document.ParseTree == null)
            {
                new LanguageServer.Module().Compile(_workspace);
            }
            return document;
        }
    }
}

