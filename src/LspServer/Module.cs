//namespace Server
//{
//    using Antlr4.Runtime;
//    using Antlr4.Runtime.Tree;
//    using LoggerNs;
//    using org.eclipse.wst.xml.xpath2.processor.util;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using Workspaces;

//    public class Module
//    {
//        public TerminalNodeImpl Find(int index, Document doc)
//        {
//            if (!_all_parses.ContainsKey(doc))
//            {
//                Compile(doc.Workspace);
//            }

//            foreach (IParseTree node in DFSVisitor.DFS(_all_parses[doc] as ParserRuleContext))
//            {
//                if (node as TerminalNodeImpl == null)
//                {
//                    continue;
//                }

//                TerminalNodeImpl leaf = node as TerminalNodeImpl;
//                if (leaf.Symbol.StartIndex <= index && index <= leaf.Symbol.StopIndex)
//                {
//                    return leaf;
//                }
//            }
//            return null;
//        }

//        public enum RecoveryStrategy { Bail, Standard };

//        private void ComputeIndexes(Document doc)
//        {
//            List<int> indices = new List<int>();
//            int cur_index = 0;
//            int cur_line = 0; // zero based LSP.
//            int cur_col = 0; // zero based LSP.
//            indices.Add(cur_index);
//            string buffer = doc.Code;
//            int length = doc.Code.Length;
//            // Go through file and record index of start of each line.
//            for (int i = 0; i < length; ++i)
//            {
//                if (cur_index >= length)
//                {
//                    break;
//                }

//                char ch = buffer[cur_index];
//                if (ch == '\r')
//                {
//                    if (cur_index + 1 >= length)
//                    {
//                        break;
//                    }
//                    else if (buffer[cur_index + 1] == '\n')
//                    {
//                        cur_line++;
//                        cur_col = 0;
//                        cur_index += 2;
//                        indices.Add(cur_index);
//                    }
//                    else
//                    {
//                        // Error in code.
//                        cur_line++;
//                        cur_col = 0;
//                        cur_index += 1;
//                        indices.Add(cur_index);
//                    }
//                }
//                else if (ch == '\n')
//                {
//                    cur_line++;
//                    cur_col = 0;
//                    cur_index += 1;
//                    indices.Add(cur_index);
//                }
//                else
//                {
//                    cur_col += 1;
//                    cur_index += 1;
//                }
//                if (cur_index >= length)
//                {
//                    break;
//                }
//            }
//            doc.Indices = indices.ToArray();
//        }

//        public int GetIndex(int line, int column, Document doc)
//        {
//            string buffer = doc.Code;
//            if (buffer == null)
//            {
//                return 0;
//            }

//            if (doc.Indices == null) ComputeIndexes(doc);

//            int low = doc.Indices[line];
//            var index = low + column;
//            return index;
//        }

//        public (int, int) GetLineColumn(int index, Document doc)
//        {
//            string buffer = doc.Code;
//            if (buffer == null)
//            {
//                return (0, 0);
//            }

//            if (doc.Indices == null) ComputeIndexes(doc);

//            // Binary search.
//            int low = 0;
//            int high = doc.Indices.Length - 1;
//            int i = 0;
//            while (low <= high)
//            {
//                i = (low + high) / 2;
//                var v = doc.Indices[i];
//                if (v < index) low = i + 1;
//                else if (v > index) high = i - 1;
//                else break;
//            }
//            var min = low <= high ? i : high;
//            var myindex = (min, index - doc.Indices[min]);
//            return myindex;
//        }

//        public List<string> GetClasses(string suffix)
//        {
//            return Grammar.Classes(suffix);
//        }

//        public QuickInfo GetQuickInfo(int index, Document doc)
//        {
//            if (!_all_parses.ContainsKey(doc))
//            {
//                Compile(doc.Workspace);
//            }
//            var ffn = doc.FullPath;
//            var suffix = System.IO.Path.GetExtension(ffn);
//            Antlr4.Runtime.Tree.IParseTree pt = Find(index, doc);
//            Antlr4.Runtime.Tree.IParseTree p = _all_parses[doc];
//            if (pt == null) return null;
//            var term = pt as TerminalNodeImpl;
//            int start = term.Symbol.StartIndex;
//            int stop = term.Symbol.StopIndex + 1;
//            StringBuilder sb = new StringBuilder();
//            for (int i = 0; i < _all_classified_nodes.Count; ++i)
//            {
//                var c = _all_classified_nodes[i];
//                if (c.Contains(pt))
//                {
//                    var clas = Grammar.Classes(suffix)[i];
//                    sb.Append(" " + clas);
//                }
//            }
//            var display = sb.ToString();
//            var range = new Workspaces.Range(new Workspaces.Index(start), new Workspaces.Index(stop));
//            return new QuickInfo() { Display = display, Range = range };
//        }

//        public int GetTag(int index, Document doc)
//        {
//            if (!_all_parses.ContainsKey(doc))
//            {
//                Compile(doc.Workspace);
//            }

//            Antlr4.Runtime.Tree.IParseTree pt = Find(index, doc);
//            Antlr4.Runtime.Tree.IParseTree p = pt;
//            TerminalNodeImpl q = p as Antlr4.Runtime.Tree.TerminalNodeImpl;
//            if (q == null) return -1;
//            for (int i = 0; i < _all_classified_nodes.Count; ++i)
//            {
//                if (_all_classified_nodes[i].Contains(q)) return i;
//            }
//            return -1;
//        }

//        public DocumentSymbol GetDocumentSymbol(int index, Document doc)
//        {
//            if (!_all_parses.ContainsKey(doc))
//            {
//                Compile(doc.Workspace);
//            }
//            Antlr4.Runtime.Tree.IParseTree pt = Find(index, doc);
//            Antlr4.Runtime.Tree.IParseTree p = pt;
//            TerminalNodeImpl q = p as Antlr4.Runtime.Tree.TerminalNodeImpl;
//            if (q == null) return null;
//            for (int i = 0; i < _all_classified_nodes.Count; ++i)
//            {
//                if (_all_classified_nodes[i].Contains(q))
//                {
//                    return new DocumentSymbol()
//                    {
//                        name = q.Symbol.Text,
//                        range = new Workspaces.Range(q.Symbol.StartIndex, q.Symbol.StopIndex),
//                        kind = i
//                    };
//                }
//            }
//            return null;
//        }

//        public IEnumerable<DocumentSymbol> GetSymbols(Document doc)
//        {
//            if (!_all_parses.ContainsKey(doc))
//            {
//                Compile(doc.Workspace);
//            }

//            List<DocumentSymbol> combined = new List<DocumentSymbol>();
//            for (int i = 0; i < _all_classified_nodes.Count; ++i)
//            {
//                foreach (var p in _all_classified_nodes[i])
//                {
//                    var q = p as TerminalNodeImpl;
//                    if (q == null) continue;
//                    combined.Add(
//                        new DocumentSymbol()
//                        {
//                            name = q.GetText(),
//                            range = new Workspaces.Range(q.Payload.StartIndex, q.Payload.StopIndex),
//                            kind = i
//                        });
//                }
//            }
//            // Sort the list.
//            IOrderedEnumerable<DocumentSymbol> sorted_combined_tokens = combined.OrderBy(t => t.range.Start.Value).ThenBy(t => t.range.End.Value);
//            return sorted_combined_tokens;
//        }

//        public class Info
//        {
//            public int start;
//            public int end;
//            public int kind; 
//        }

//        public IEnumerable<Info> Get(int start, int end, Document doc)
//        {
//            try
//            {
//                if (!_all_parses.ContainsKey(doc))
//                {
//                    Compile(doc.Workspace);
//                }
//                List<Info> combined = new List<Info>();
//                for (int i = 0; i < _all_classified_nodes.Count; ++i)
//                {
//                    foreach (var p in _all_classified_nodes[i])
//                    {
//                        var q = p as TerminalNodeImpl;
//                        var sym = q.Symbol;
//                        var st = sym.StartIndex;
//                        var en = sym.StopIndex + 1;
//                        if (end < st) continue;
//                        if (en < start) continue;
//                        int s1 = st > start ? st : start;
//                        int s2 = en < end ? en : end;
//                        combined.Add(
//                             new Info()
//                             {
//                                 start = s1,
//                                 end = s2,
//                                 kind = i
//                             });
//                        ;
//                    }
//                }
//                // Sort the list.
//                IOrderedEnumerable<Info> sorted_combined_tokens = combined.OrderBy(t => t.start).ThenBy(t => t.end);
//                return sorted_combined_tokens;
//            }
//            catch (Exception)
//            {
//            }
//            return new List<Info>();
//        }

//        public IEnumerable<Info> Get(Document doc)
//        {
//            try
//            {
//                if (!_all_parses.ContainsKey(doc))
//                {
//                    Compile(doc.Workspace);
//                }
//                List<Info> combined = new List<Info>();
//                for (int i = 0; i < _all_classified_nodes.Count; ++i)
//                {
//                    foreach (var p in _all_classified_nodes[i])
//                    {
//                        var q = p as TerminalNodeImpl;
//                        var sym = q.Symbol;
//                        var st = sym.StartIndex;
//                        var en = sym.StopIndex + 1;
//                        int s1 = st;
//                        int s2 = en;
//                        // Create multiple "info" for multiline tokens.
//                        var (ls, cs) = new Module().GetLineColumn(st, doc);
//                        var (le, ce) = new Module().GetLineColumn(en, doc);
//                        if (ls == le)
//                        {
//                            combined.Add(
//                                new Info()
//                                {
//                                    start = s1,
//                                    end = s2 - 1,
//                                    kind = i
//                                });
//                        }
//                        else
//                        {
//                            var text = sym.Text;
//                            int start_region = st;
//                            for (int cur_index = 0; cur_index < text.Length;)
//                            {
//                                if (text[cur_index] == '\n' || text[cur_index] == '\r')
//                                {
//                                    // Emit Info().
//                                    if (text[cur_index] == '\r' && (cur_index + 1 < text.Length) && text[cur_index + 1] == '\n')
//                                        cur_index++;
//                                    cur_index++;
//                                    combined.Add(
//                                        new Info()
//                                        {
//                                            start = start_region,
//                                            end = st + cur_index - 1,
//                                            kind = i
//                                        });
//                                    start_region = st + cur_index;
//                                }
//                                else
//                                    cur_index++;
//                            }
//                            if (start_region != en)
//                            {
//                                combined.Add(
//                                    new Info()
//                                    {
//                                        start = start_region,
//                                        end = en - 1,
//                                        kind = i
//                                    });
//                            }
//                        }
//                    }
//                }
//                // Sort the list.
//                IOrderedEnumerable<Info> sorted_combined_tokens = combined.OrderBy(t => t.start).ThenBy(t => t.end);
//                return sorted_combined_tokens;
//            }
//            catch (Exception)
//            {
//            }
//            return new List<Info>();
//        }

//        public IEnumerable<Workspaces.Range> GetErrors(Workspaces.Range range, Document doc)
//        {
//            if (!_all_parses.ContainsKey(doc))
//            {
//                Compile(doc.Workspace);
//            }
//            var result = new List<Workspaces.Range>();
//            var p = _all_parses[doc];
//            var (tree, parser, lexer) = (p, Grammar.Parser, Grammar.Lexer);
            
            
//            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
//            {
//                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
//                var nodes = engine.parseExpression("//*",
//                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
//                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
//                foreach (var n in nodes)
//                {
//                    ErrorNodeImpl q = p as Antlr4.Runtime.Tree.ErrorNodeImpl;
//                    if (q == null)
//                    {
//                        continue;
//                    }
//                    if (q.Payload == null)
//                    {
//                        continue;
//                    }

//                    int y = q.Payload.StartIndex;
//                    int z = q.Payload.StopIndex;
//                    if (y < 0)
//                    {
//                        y = 0;
//                    }

//                    if (z < 0)
//                    {
//                        z = 0;
//                    }

//                    int a = y;
//                    int b = z + 1;
//                    int start_token_start = a;
//                    int end_token_end = b;
//                    if (start_token_start > range.End.Value)
//                    {
//                        continue;
//                    }

//                    if (end_token_end < range.Start.Value)
//                    {
//                        continue;
//                    }

//                    var r = new Workspaces.Range(new Workspaces.Index(a), new Workspaces.Index(b));
//                    result.Add(r);

//                }
//            }
//            return result;
//        }

//        public IList<Location> FindDefs(int index, Document doc)
//        {
//            List<Location> result = new List<Location>();
//            if (doc == null)
//            {
//                return result;
//            }
//            var workspace = doc.Workspace;

//            IParseTree ref_pt = Find(index, doc);
//            if (ref_pt == null)
//            {
//                return result;
//            }
//            var term = ref_pt as TerminalNodeImpl;
//            result.Add(
//                new Location()
//                {
//                    Range = new Workspaces.Range(term.Payload.StartIndex, term.Payload.StopIndex),
//                    Uri = doc
//                });
//            return result;
//        }

//        public IEnumerable<Location> FindRefsAndDefs(int index, Document doc)
//        {
//            List<Location> result = new List<Location>();
//            IParseTree ref_pt = Find(index, doc);
//            if (ref_pt == null)
//            {
//                return result;
//            }
//            var term = ref_pt as TerminalNodeImpl;
//            result.Add(
//                new Location()
//                {
//                    Range = new Workspaces.Range(term.Payload.StartIndex, term.Payload.StopIndex),
//                    Uri = doc
//                });
//            return result;
//        }

//        public IEnumerable<Location> GetDefs(Document doc)
//        {
//            List<Location> result = new List<Location>();
//            if (!_all_parses.ContainsKey(doc))
//            {
//                Compile(doc.Workspace);
//            }
//            for (int i = 0; i < _all_classified_nodes.Count; ++i)
//            {
//                foreach (var p in _all_classified_nodes[i])
//                {
//                    var q = p as TerminalNodeImpl;
//                    if (q == null) continue;
//                    var sym = q.Symbol;
//                    result.Add(
//                    new Location()
//                    {
//                        Range = new Workspaces.Range(sym.StartIndex, sym.StopIndex),
//                        Uri = doc
//                    });
//                }
//            }
//            return result;
//        }

//        static Dictionary<Document, IParseTree> _all_parses = new Dictionary<Document, IParseTree>();
//        static List<List<IParseTree>> _all_classified_nodes = new List<List<IParseTree>>();

//        public Dictionary<Document, IParseTree> Compile(Workspace workspace)
//        {
//            try
//            {
//                foreach (Document document in Workspaces.DFSContainer.DFS(workspace))
//                {
//                    if (!document.Changed) continue;
//                    string file_name = document.FullPath;
//                    string suffix = System.IO.Path.GetExtension(file_name);
//                    if (file_name == null) continue;
//                    var p = Grammar.Parse(document);
//                    _all_parses[document] = p;
//                    document.Changed = false;
//                    _all_classified_nodes = new List<List<IParseTree>>();
//                    foreach (var c in Grammar.Classifiers(suffix))
//                    {
//                        if (c == "")
//                        {
//                            _all_classified_nodes.Add(new List<IParseTree>());
//                            continue;
//                        }
//                        var (tree, parser, lexer) = (p, Grammar.Parser, Grammar.Lexer);
//                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
//                        {
//                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
//                            var nodes = engine.parseExpression(c,
//                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
//                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
//                            _all_classified_nodes.Add(nodes);
//                        }
//                    }
//                }
//                return _all_parses;
//            }
//            catch (Exception e)
//            {
//                Logger.Log.Notify(e.ToString());
//            }
//            return new Dictionary<Document, IParseTree>();
//        }
//    }
//}

