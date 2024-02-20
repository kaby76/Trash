using Antlr4.Runtime;
using LoggerNs;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.Linq;
using Workspaces;

namespace Server
{

    public class Module
    {
        public UnvParseTreeElement Find(int index, Document doc)
        {
            if (!_all_parses.ContainsKey(doc))
            {
                Compile(doc.Workspace);
            }

            foreach (UnvParseTreeNode node in DFSVisitor.DFS(_all_parses[doc]))
            {
                if (node is UnvParseTreeElement e)
                {
                    if (e.IsTerminal())
                    {
                        int line = e.GetLine() - 1;
                        int column = e.GetColumn();
                        int i = GetIndex(line, column, doc);
                        if (i <= index && index <= i+e.GetText().Length)
                        {
                            return e;
                        }
                    }
                    else
                        for (int j = 0; j < e.ChildNodes.Length; ++j)
                        {
                            var c = e.ChildNodes.item(j);
                        }
                }
            }
            return null;
        }

        public enum RecoveryStrategy
        {
            Bail,
            Standard
        };

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

        public List<string> GetClasses(string language_id)
        {
            return Module._all_grammars.Where(g => g.Options.LanguageId == language_id).First().Classes();
        }

        public QuickInfo GetQuickInfo(int index, Document doc)
        {
            if (!_all_parses.ContainsKey(doc))
            {
                Compile(doc.Workspace);
            }

            var ffn = doc.FullPath;
            var grammar = _all_grammars.Where(g => g.LanguageId == doc.LanguageId).FirstOrDefault();
            if (grammar == null) return null;
            var pt = Find(index, doc);
            return null;
            //var p = _all_parses[doc];
            //if (pt == null) return null;
            //var term = pt;
            //int start = term.Symbol.StartIndex;
            //int stop = term.Symbol.StopIndex + 1;
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < _all_classified_nodes.Count; ++i)
            //{
            //    var c = _all_classified_nodes[i];
            //    if (c.Contains(pt))
            //    {
            //        var clas = grammar.Classes()[i];
            //        sb.Append(" " + clas);
            //    }
            //}

            //var display = sb.ToString();
            //var range = new Workspaces.Range(new Workspaces.Index(start), new Workspaces.Index(stop));
            //return new QuickInfo() { Display = display, Range = range };
        }

        public int GetTag(int index, Document doc)
        {
            if (!_all_parses.ContainsKey(doc))
            {
                Compile(doc.Workspace);
            }

            var pt = Find(index, doc);
            var p = pt;
            //TerminalNodeImpl q = p as Antlr4.Runtime.Tree.TerminalNodeImpl;
            //if (q == null) return -1;
            //for (int i = 0; i < _all_classified_nodes.Count; ++i)
            //{
            //    if (_all_classified_nodes[i].Contains(q)) return i;
            //}

            return -1;
        }

        public DocumentSymbol GetDocumentSymbol(int index, Document doc)
        {
            if (!_all_parses.ContainsKey(doc))
            {
                Compile(doc.Workspace);
            }

            var pt = Find(index, doc);
            var p = pt;
            for (int i = 0; i < _all_classified_nodes.Count; ++i)
            {
                if (_all_classified_nodes[i].Contains(p))
                {
                    return new DocumentSymbol()
                    {
                        name = p.GetText(),
                        range = new Workspaces.Range(GetIndex(p.GetLine() - 1, p.GetColumn(), doc),
                            p.GetText().Length),
                        kind = i
                    };
                }
            }

            return null;
        }

        public IEnumerable<DocumentSymbol> GetSymbols(Document doc)
        {
            if (!_all_parses.ContainsKey(doc))
            {
                Compile(doc.Workspace);
            }

            List<DocumentSymbol> combined = new List<DocumentSymbol>();
            for (int i = 0; i < _all_classified_nodes.Count; ++i)
            {
                foreach (var p in _all_classified_nodes[i])
                {
                    var l = p.GetLine() - 1;
                    var c = p.GetColumn();
                    var ind = GetIndex(l, c, doc);
                    combined.Add(
                        new DocumentSymbol()
                        {
                            name = p.GetText(),
                            range = new Workspaces.Range(ind, ind+p.GetText().Length),
                            kind = i
                        });
                }
            }

            // Sort the list.
            IOrderedEnumerable<DocumentSymbol> sorted_combined_tokens =
                combined.OrderBy(t => t.range.Start.Value).ThenBy(t => t.range.End.Value);
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
                if (!_all_parses.ContainsKey(doc))
                {
                    Compile(doc.Workspace);
                }

                List<Info> combined = new List<Info>();
                for (int i = 0; i < _all_classified_nodes.Count; ++i)
                {
                    foreach (var p in _all_classified_nodes[i])
                    {
                        var l = p.GetLine() - 1;
                        var c = p.GetColumn();
                        var ind = GetIndex(l, c, doc);
                        var st = ind;
                        var en = ind + p.GetText().Length;
                        if (end < st) continue;
                        if (en < start) continue;
                        int s1 = st > start ? st : start;
                        int s2 = en < end ? en : end;
                        combined.Add(
                            new Info()
                            {
                                start = s1,
                                end = s2,
                                kind = i
                            });
                        ;
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

        public IEnumerable<Info> Get(Document doc)
        {
            try
            {
                if (!_all_parses.ContainsKey(doc))
                {
                    Compile(doc.Workspace);
                }

                List<Info> combined = new List<Info>();
                for (int i = 0; i < _all_classified_nodes.Count; ++i)
                {
                    foreach (var p in _all_classified_nodes[i])
                    {
                        var q = p;
                        var l = p.GetLine() - 1;
                        var c = p.GetColumn();
                        var ind = GetIndex(l, c, doc);
                        var st = ind;
                        var en = ind + p.GetText().Length;
                        int s1 = st;
                        int s2 = en;
                        // Create multiple "info" for multiline tokens.
                        var (ls, cs) = new Module().GetLineColumn(st, doc);
                        var (le, ce) = new Module().GetLineColumn(en, doc);
                        if (ls == le)
                        {
                            combined.Add(
                                new Info()
                                {
                                    start = s1,
                                    end = s2 - 1,
                                    kind = i
                                });
                        }
                        else
                        {
                            var text = q.GetText();
                            int start_region = st;
                            for (int cur_index = 0; cur_index < text.Length;)
                            {
                                if (text[cur_index] == '\n' || text[cur_index] == '\r')
                                {
                                    // Emit Info().
                                    if (text[cur_index] == '\r' && (cur_index + 1 < text.Length) &&
                                        text[cur_index + 1] == '\n')
                                        cur_index++;
                                    cur_index++;
                                    combined.Add(
                                        new Info()
                                        {
                                            start = start_region,
                                            end = st + cur_index - 1,
                                            kind = i
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
                                        kind = i
                                    });
                            }
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
            if (!_all_parses.ContainsKey(doc))
            {
                Compile(doc.Workspace);
            }

            var result = new List<Workspaces.Range>();
            var p = _all_parses[doc];
            var grammar = _all_grammars.Where(g => g.LanguageId == doc.LanguageId).FirstOrDefault();
            if (grammar == null) return null;
            var (tree, parser, lexer) = (p, grammar.Parser, grammar.Lexer);
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
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
            var ref_pt = Find(index, doc);
            if (ref_pt == null)
            {
                return result;
            }
            var term = ref_pt;
            var text = term.GetText();
            foreach (var defs in _all_classified_nodes)
            {
                foreach (var @def in defs)
                {
                    if (@def.GetText() == text)
                    {
                        var l = @def.GetLine() - 1;
                        var c = @def.GetColumn();
                        var ind = GetIndex(l, c, doc);
                        result.Add(
                            new Location()
                            {
                                Range = new Workspaces.Range(ind, ind + term.GetText().Length),
                                Uri = doc
                            });
                        return result;
                    }
                }
            }

            return null;
        }

        public IEnumerable<Location> FindRefsAndDefs(int index, Document doc)
        {
            List<Location> result = new List<Location>();
            var ref_pt = Find(index, doc);
            if (ref_pt == null)
            {
                return result;
            }

            var term = ref_pt;
            var l = term.GetLine() - 1;
            var c = term.GetColumn();
            var ind = GetIndex(l, c, doc);
            result.Add(
                new Location()
                {
                    Range = new Workspaces.Range(ind, ind + term.GetText().Length),
                    Uri = doc
                });
            return result;
        }

        public IEnumerable<Location> GetDefs(Document doc)
        {
            List<Location> result = new List<Location>();
            if (!_all_parses.ContainsKey(doc))
            {
                Compile(doc.Workspace);
            }

            for (int i = 0; i < _all_classified_nodes.Count; ++i)
            {
                foreach (var p in _all_classified_nodes[i])
                {
                    var q = p;
                    if (q == null) continue;
                    var sym = q;
                    var l = sym.GetLine() - 1;
                    var c = sym.GetColumn();
                    var ind = GetIndex(l, c, doc);
                    result.Add(
                        new Location()
                        {
                            Range = new Workspaces.Range(ind, ind + sym.GetText().Length),
                            Uri = doc
                        });
                }
            }

            return result;
        }

        static Dictionary<Document, UnvParseTreeElement> _all_parses = new Dictionary<Document, UnvParseTreeElement>();
        static List<List<UnvParseTreeElement>> _all_classified_nodes = new List<List<UnvParseTreeElement>>();
        public static List<Grammar> _all_grammars = new List<Grammar>();

        public static void Compile(Workspace workspace)
        {
            try
            {
                foreach (Document document in Workspaces.DFSContainer.DFS(workspace))
                {
                    if (!document.Changed) continue;
                    string file_name = document.FullPath;
                    string suffix = System.IO.Path.GetExtension(file_name);
                    if (file_name == null) continue;
                    Grammar grammar = _all_grammars.Where(g => g.LanguageId == document.LanguageId).FirstOrDefault();
                    if (grammar == null) continue;
                    var q = grammar.Parse(document);
                    UnvParseTreeElement p2 = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM(true).BottomUpConvert(q.Item1, null, q.Item3, q.Item2,
                        q.Item4 as CommonTokenStream, q.Item5);
                    _all_parses[document] = p2;
                    document.Changed = false;
                    _all_classified_nodes = new List<List<UnvParseTreeElement>>();
                    foreach (string c in grammar.Classifiers())
                    {
                        if (c == "")
                        {
                            _all_classified_nodes.Add(new List<UnvParseTreeElement>());
                            continue;
                        }

                        //                  (IParseTree tree, Parser parser, Lexer lexer) = (q.Item1, q.Item3, q.Item2);
                        org.eclipse.wst.xml.xpath2.processor.Engine engine =
                            new org.eclipse.wst.xml.xpath2.processor.Engine();
                        var ate = new ParseTreeEditing.AntlrDOM.ConvertToAntlrDOM();
                        ParseTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                            ate.MakeDynamicContext(p2);
                        var nodes = engine.parseExpression(c,
                                new StaticContextBuilder()).evaluate(dynamicContext,
                                new object[] { dynamicContext.Document })
                            .Select(x=>x.NativeValue as UnvParseTreeElement)
                            .ToList();
                        _all_classified_nodes.Add(nodes);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Notify(e.ToString());
            }
        }
    }
}

