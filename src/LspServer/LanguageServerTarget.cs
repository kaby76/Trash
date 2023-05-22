using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using LoggerNs;
using LspTypes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using org.eclipse.wst.xml.xpath2.processor.@internal.ast;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.AntlrDOM;
using StreamJsonRpc;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Threading;
using System;

namespace Server
{

    public class LanguageServerTarget
    {
        private readonly LSPServer server;
        private readonly bool trace = true;
        //private readonly Workspaces.Workspace _workspace;
        private Dictionary<string, string> workspace = new Dictionary<string, string>();
        private static readonly object _object = new object();
        private readonly Dictionary<string, bool> ignore_next_change = new Dictionary<string, bool>();
        private Dictionary<string, ParsingResultSet> data = new Dictionary<string, ParsingResultSet>();
        
        public LanguageServerTarget(LSPServer server)
        {
            this.server = server;
            //_workspace = Workspaces.Workspace.Instance;
        }


        void ApplyChanges(string transaction_name, Dictionary<string, string> ch)
        {
            //if (!ch.Any())
            //{
            //    throw new Exception("No changes were needed, none made.");
            //}
            //var a = new Dictionary<string, TextEdit[]>();
            //Workspace workspace = null;
            //foreach (var pair in ch)
            //{
            //    var fn = pair.Key;
            //    var new_code = pair.Value;
            //    Document document = CheckDoc(fn);
            //    //workspace = document.Workspace;
            //   // var code = document.Code;
            //    List<TextEdit> edits = new List<TextEdit>();
            //    Diff_match_patch diff = new Diff_match_patch();
            //    List<Diff> diffs = diff.Diff_main(code, new_code);
            //    List<Patch> patch = diff.Patch_make(diffs);
            //    {
            //        // Start edit session.
            //        int times = 0;
            //        int delta = 0;
            //        foreach (Patch p in patch)
            //        {
            //            times++;
            //            int start = p.start1 - delta;

            //            int offset = 0;
            //            foreach (Diff ed in p.diffs)
            //            {
            //                if (ed.operation == Operation.EQUAL)
            //                {
            //                    //// Let's verify that.
            //                    int len = ed.text.Length;
            //                    //var tokenSpan = new SnapshotSpan(buffer.CurrentSnapshot,
            //                    //  new Span(start + offset, len));
            //                    //var tt = tokenSpan.GetText();
            //                    //if (ed.text != tt)
            //                    //{ }
            //                    offset += len;
            //                }
            //                else if (ed.operation == Operation.DELETE)
            //                {
            //                    int len = ed.text.Length;
            //                    //var tokenSpan = new SnapshotSpan(buffer.CurrentSnapshot,
            //                    //  new Span(start + offset, len));
            //                    //var tt = tokenSpan.GetText();
            //                    //if (ed.text != tt)
            //                    //{ }
            //                    LanguageServer.TextEdit edit = new LanguageServer.TextEdit()
            //                    {
            //                        range = new Workspaces.Range(
            //                            new Workspaces.Index(start + offset),
            //                            new Workspaces.Index(start + offset + len)),
            //                        NewText = ""
            //                    };
            //                    offset += len;
            //                    edits.Add(edit);
            //                }
            //                else if (ed.operation == Operation.INSERT)
            //                {
            //                    int len = ed.text.Length;
            //                    LanguageServer.TextEdit edit = new LanguageServer.TextEdit()
            //                    {
            //                        range = new Workspaces.Range(
            //                            new Workspaces.Index(start + offset),
            //                            new Workspaces.Index(start + offset)),
            //                        NewText = ed.text
            //                    };
            //                    edits.Add(edit);
            //                }
            //            }
            //            delta += (p.length2 - p.length1);
            //        }
            //    }
            //    var changes = edits.ToArray();

            //    List<LspTypes.TextEdit> new_list = new List<LspTypes.TextEdit>();
            //    int count = 0;
            //    foreach (LanguageServer.TextEdit delta in changes)
            //    {
            //        var new_edit = new LspTypes.TextEdit
            //        {
            //            Range = new LspTypes.Range()
            //        };
            //        (int, int) lcs = new LanguageServer.Module().GetLineColumn(delta.range.Start.Value, document);
            //        (int, int) lce = new LanguageServer.Module().GetLineColumn(delta.range.End.Value, document);
            //        new_edit.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
            //        new_edit.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
            //        new_edit.NewText = delta.NewText;
            //        new_list.Add(new_edit);
            //        count++;
            //    }
            //    var result = new_list.ToArray();
            //    a[fn] = result;
            //    ignore_next_change[fn] = true;
            //    // This must be done after computing changes since offsets/line/column computations
            //    // depend on what is currently the source.
            //    document.Code = new_code;
            //}
            //// Recompile only after every single change everywhere is in.
            //if (workspace != null)
            //    _ = new LanguageServer.Module().Compile(workspace);
            //server.ApplyEdit(transaction_name, a);
        }

        [JsonRpcMethod(Methods.InitializeName)]
        public object Initialize(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- Initialize");
                    Logger.Log.WriteLine(arg.ToString());
                }

                var init_params = arg.ToObject<InitializeParams>();

                ServerCapabilities capabilities = new LspTypes.ServerCapabilities
                {
                    TextDocumentSync = new TextDocumentSyncOptions
                    {
                        OpenClose = true,
                        Change = TextDocumentSyncKind.Incremental,
                        Save = new SaveOptions
                        {
                            IncludeText = true
                        },
                        WillSave = false,
                        WillSaveWaitUntil = false,
                    },

                    CompletionProvider =
                        (Options.Option.GetBoolean("EnableCompletion")
                            ? new CompletionOptions
                            {
                                ResolveProvider = true,
                                TriggerCharacters = new string[] { ",", "." }
                            }
                            : null),

                    //HoverProvider = true,
                    HoverProvider = false,

                    SignatureHelpProvider = null,

                    // DeclarationProvider not supported.

                    DefinitionProvider = true,

                    TypeDefinitionProvider = false, // Does not make sense for Antlr.

                    ImplementationProvider = false, // Does not make sense for Antlr.

                    ReferencesProvider = true,

                    //DocumentHighlightProvider = true,
                    DocumentHighlightProvider = false,

                    //DocumentSymbolProvider = true,
                    DocumentSymbolProvider = false,

                    CodeLensProvider = null,

                    DocumentLinkProvider = null,

                    // ColorProvider not supported.

                    //DocumentFormattingProvider = true,
                    DocumentFormattingProvider = false,

                    DocumentRangeFormattingProvider = false,

                    RenameProvider = true,
                    //RenameProvider = false,

                    //FoldingRangeProvider = new SumType<bool, FoldingRangeOptions>(false),
                    FoldingRangeProvider = new SumType<bool, FoldingRangeOptions, FoldingRangeRegistrationOptions>(false),

                    ExecuteCommandProvider = null,

                    // SelectionRangeProvider not supported.

                    WorkspaceSymbolProvider = false,
                    //SemanticTokensProvider = false

                    SemanticTokensProvider = 
                    new SemanticTokensOptions()
                    {
                        Full = true,
                        Range = false,
                        Legend = new SemanticTokensLegend()
                        {
                            tokenTypes =
                            //TokenTypes = 
                            new string[] {
                                "class",
                                "variable",
                                "enum",
                                "comment",
                                "string",
                                "keyword",
                            },
                            tokenModifiers =
                            //TokenModifiers =
                            new string[] {
                                "declaration",
                                "documentation",
                            }
                        }
                    },
                };

                InitializeResult result = new InitializeResult
                {
                    Capabilities = capabilities
                };
                string json = JsonConvert.SerializeObject(result);
                if (trace)
                {
                    Logger.Log.WriteLine("--> " + json);
                }
                return result;
            }
        }

        [JsonRpcMethod(Methods.InitializedName)]
        public void InitializedName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- Initialized");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
            }
        }

        [JsonRpcMethod(Methods.ShutdownName)]
        public JToken ShutdownName()
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- Shutdown");
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.ExitName)]
        public void ExitName()
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- Exit");
                    }
                    server.Exit();
                }
                catch (Exception)
                { }
            }
        }

        // ======= WINDOW ========

        [JsonRpcMethod(Methods.WorkspaceDidChangeConfigurationName)]
        public void WorkspaceDidChangeConfigurationName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- WorkspaceDidChangeConfiguration");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    //var parameter = arg.ToObject<DidChangeConfigurationParams>();
                    //this.server.SendSettings(parameter);
                }
                catch (Exception)
                { }
            }
        }

        [JsonRpcMethod(Methods.WorkspaceDidChangeWatchedFilesName)]
        public void WorkspaceDidChangeWatchedFilesName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- WorkspaceDidChangeWatchedFiles");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
            }
        }

        [JsonRpcMethod(Methods.WorkspaceSymbolName)]
        public JToken WorkspaceSymbolName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- WorkspaceSymbol");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.WorkspaceExecuteCommandName)]
        public JToken WorkspaceExecuteCommandName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- WorkspaceExecuteCommand");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.WorkspaceApplyEditName)]
        public JToken WorkspaceApplyEditName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- WorkspaceApplyEdit");
                    Logger.Log.WriteLine(arg.ToString());
                }
                return null;
            }
        }

        // ======= TEXT SYNCHRONIZATION ========

        [JsonRpcMethod(Methods.TextDocumentDidOpenName)]
        public void TextDocumentDidOpenName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentDidOpen");
                    Logger.Log.WriteLine(arg.ToString());
                }
                DidOpenTextDocumentParams request = arg.ToObject<DidOpenTextDocumentParams>();
                var language_id = request.TextDocument.LanguageId;
                if (language_id == "Antlr") language_id = "antlr4";
                var text = request.TextDocument.Text;
                string fn = request.TextDocument.Uri;
                //string fn = request.TextDocument.Uri.LocalPath;
                workspace[fn] = text;
                // We should use suffix to do the right parse.
                // Buf for now, it's "Antlr" when VS2022 is connecting.
                switch (language_id)
                {
                    case "antlr4":
                        var prs = DoParse(language_id, text, "", fn, 0);
                        data.Add(fn, prs);
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        var ate = new ParseTreeEditing.AntlrDOM.ConvertToAntlrDOM();
                        using (ParseTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                               ate.Try(prs.Nodes, prs.Parser))
                        {
                            {
                                var expr = "//parserRuleSpec/RULE_REF";
                                var nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext,
                                        new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrElement).AntlrIParseTree)
                                    .ToList();
                                foreach (var node in nodes)
                                {
                                    if (trace)
                                    {
                                        Logger.Log.WriteLine("Found " + node.ToString());
                                    }

                                    // Parser symbol
                                    prs.ParserDefs.Add(node);
                                }
                            }
                            {
                                var expr = "//lexerRuleSpec/TOKEN_REF";
                                var nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext,
                                        new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrElement).AntlrIParseTree)
                                    .ToList();
                                foreach (var node in nodes)
                                {
                                    if (trace)
                                    {
                                        Logger.Log.WriteLine("Found " + node.ToString());
                                    }

                                    // Lexer symbol
                                    prs.LexerDefs.Add(node);
                                }
                            }
                            {
                                var expr = "//(lexerRuleSpec/lexerRuleBlock//terminal/TOKEN_REF | parserRuleSpec/ruleBlock//(ruleref/RULE_REF | terminal/TOKEN_REF))";
                                var nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext,
                                        new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrElement).AntlrIParseTree)
                                    .ToList();
                                foreach (var node in nodes)
                                {
                                    if (trace)
                                    {
                                        Logger.Log.WriteLine("Found " + node.ToString());
                                    }

                                    // Lexer symbol
                                    prs.Refs.Add(node);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                //var document = CheckDoc(request.TextDocument.Uri);
                //var language_id = request.TextDocument.LanguageId;
                //document.Code = request.TextDocument.Text;
                //if (language_id == "antlr2")
                //    document.ParseAs = language_id;
                //else if (language_id == "antlr3")
                //    document.ParseAs = language_id;
                //else if (language_id == "antlr4")
                //    document.ParseAs = language_id;
                //else if (language_id == "ebnf")
                //    document.ParseAs = language_id;
                //else if (language_id == "bison")
                //    document.ParseAs = language_id;
                //var workspace = document.Workspace;
                //List<ParsingResults> to_do = new LanguageServer.Module().Compile(workspace);
            }
        }

        [JsonRpcMethod(Methods.TextDocumentDidChangeName)]
        public void TextDocumentDidChangeName(JToken arg)
        {
            //lock (_object)
            //{
            //    if (trace)
            //    {
            //        Logger.Log.WriteLine("<-- TextDocumentDidChange");
            //        Logger.Log.WriteLine(arg.ToString());
            //    }
            //    DidChangeTextDocumentParams request = arg.ToObject<DidChangeTextDocumentParams>();
            //    Document document = CheckDoc(request.TextDocument.Uri);

            //    // Create a thread to handle this update in the correct order.
            //    //Thread thread = new Thread(() =>
            //    {
            //        try
            //        {
            //            //       if (request.TextDocument?.Version != null)
            //            {
            //                int version = (int)request.TextDocument.Version;
            //                // spin until this version is current_version.
            //                // for (; ; )
            //                {
            //                    //  if (first_change || version == current_version)
            //                    {
            //                        if (!ignore_next_change.ContainsKey(document.FullPath))
            //                        {
            //                            ParsingResults pd = ParsingResultsFactory.Create(document);
            //                            string code = pd.Code;
            //                            if (trace)
            //                            {
            //                                Logger.Log.WriteLine("making change, code before");
            //                                Logger.Log.WriteLine(code);
            //                                Logger.Log.WriteLine("----------------------");
            //                            }
            //                            int start_index = 0;
            //                            int end_index = 0;
            //                            foreach (TextDocumentContentChangeEvent change in request.ContentChanges)
            //                            {
            //                                var range = change.Range;
            //                                int length = (int)(change.RangeLength ?? 0); // Why? range encodes start and end => length!
            //                                string text = change.Text;
            //                                {
            //                                    int line = (int)range.Start.Line;
            //                                    int character = (int)range.Start.Character;
            //                                    start_index = new LanguageServer.Module().GetIndex(line, character, document);
            //                                }
            //                                {
            //                                    int line = (int)range.End.Line;
            //                                    int character = (int)range.End.Character;
            //                                    end_index = new LanguageServer.Module().GetIndex(line, character, document);
            //                                }
            //                                (int, int) bs = new LanguageServer.Module().GetLineColumn(start_index, document);
            //                                (int, int) be = new LanguageServer.Module().GetLineColumn(end_index, document);
            //                                string original = code.Substring(start_index, end_index - start_index);
            //                                string n = code.Substring(0, start_index)
            //                                        + text
            //                                        + code.Substring(0 + start_index + end_index - start_index);
            //                                code = n;
            //                            }
            //                            if (trace)
            //                            {
            //                                Logger.Log.WriteLine("making change, code after");
            //                                Logger.Log.WriteLine(code);
            //                                Logger.Log.WriteLine("----------------------");
            //                            }
            //                            document.Code = code;
            //                            var workspace = document.Workspace;
            //                            List<ParsingResults> to_do = new LanguageServer.Module().Compile(workspace);
            //                        }
            //                        else
            //                        {
            //                            ignore_next_change.Remove(document.FullPath);
            //                        }
            //                        current_version = version + 1;
            //                        //first_change = false;

            //                        //          break;
            //                    }
            //                    //        Thread.Sleep(100);
            //                }
            //            }
            //        }
            //        catch (ThreadAbortException)
            //        {
            //            Thread.ResetAbort();
            //        }
            //        // })
            //        //      {
            //        //          IsBackground = true
            //        //      };

            //        //       thread.Start();
            //    }
            //}
        }

        [JsonRpcMethod(Methods.TextDocumentWillSaveName)]
        public void TextDocumentWillSaveName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentWillSave");
                    Logger.Log.WriteLine(arg.ToString());
                }
                // Nothing to do--who cares because the server does not perform a save.
            }
        }

        [JsonRpcMethod(Methods.TextDocumentWillSaveWaitUntilName)]
        public JToken TextDocumentWillSaveWaitUntilName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentWillSaveWaitUntil");
                    Logger.Log.WriteLine(arg.ToString());
                }
                // Nothing to do--who cares because the server does not perform a save, and
                // the server doesn't manufacture edit requests out of thin air.
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentDidSaveName)]
        public void TextDocumentDidSaveName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentDidSave");
                    Logger.Log.WriteLine(arg.ToString());
                }
                // Nothing to do--who cares because the server does not perform a save.
            }
        }

        [JsonRpcMethod(Methods.TextDocumentDidCloseName)]
        public void TextDocumentDidCloseName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentDidClose");
                    Logger.Log.WriteLine(arg.ToString());
                }
                DidCloseTextDocumentParams request = arg.ToObject<DidCloseTextDocumentParams>();
                string fn = request.TextDocument.Uri;
                workspace.Remove(fn);
                data.Remove(fn);
            }
        }

        // ======= DIAGNOSTICS ========

        [JsonRpcMethod(Methods.TextDocumentPublishDiagnosticsName)]
        public void TextDocumentPublishDiagnosticsName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentPublishDiagnostics");
                    Logger.Log.WriteLine(arg.ToString());
                }
            }
        }

        // ======= LANGUAGE FEATURES ========

        [JsonRpcMethod(Methods.TextDocumentCompletionName)]
        public object[] TextDocumentCompletionName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    List<CompletionItem> items = new List<CompletionItem>();
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentCompletion");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    CompletionParams request = arg.ToObject<CompletionParams>();
                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //CompletionContext context = request.Context;
                    //Position position = request.Position;
                    //int line = (int)position.Line;
                    //int character = (int)position.Character;
                    //int char_index = new LanguageServer.Module().GetIndex(line, character, document);
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine("position index = " + char_index);
                    //    (int, int) back = new LanguageServer.Module().GetLineColumn(char_index, document);
                    //    Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //}
                    //List<string> res = new LanguageServer.Module().Completion(char_index, document);
                    //foreach (string r in res)
                    //{
                    //    CompletionItem item = new CompletionItem
                    //    {
                    //        Label = r,
                    //        InsertText = r,
                    //        Kind = CompletionItemKind.Variable
                    //    };
                    //    items.Add(item);
                    //}
                    return items.ToArray();
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentCompletionResolveName)]
        public JToken TextDocumentCompletionResolveName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentCompletionResolve");
                    Logger.Log.WriteLine(arg.ToString());
                }
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentHoverName)]
        public object TextDocumentHoverName(JToken arg)
        {
            lock (_object)
            {
                Hover hover = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentHover");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    TextDocumentPositionParams request = arg.ToObject<TextDocumentPositionParams>();
                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //Position position = request.Position;
                    //int line = (int)position.Line;
                    //int character = (int)position.Character;
                    //int index = new LanguageServer.Module().GetIndex(line, character, document);
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine("position index = " + index);
                    //    (int, int) back = new LanguageServer.Module().GetLineColumn(index, document);
                    //    Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //}
                    //QuickInfo quick_info = new LanguageServer.Module().GetQuickInfo(index, document);
                    //if (quick_info == null)
                    //{
                    //    return null;
                    //}

                    //hover = new Hover
                    //{
                    //    Contents = new SumType<string, MarkedString, MarkedString[], MarkupContent>(
                    //        new MarkupContent()
                    //        {
                    //            Kind = MarkupKind.PlainText,
                    //            Value = quick_info.Display
                    //        }
                    //        )
                    //};
                    //int index_start = quick_info.Range.Start.Value;
                    //int index_end = quick_info.Range.End.Value;
                    //(int, int) lcs = new LanguageServer.Module().GetLineColumn(index_start, document);
                    //(int, int) lce = new LanguageServer.Module().GetLineColumn(index_end, document);
                    //hover.Range = new LspTypes.Range
                    //{
                    //    Start = new Position((uint)lcs.Item1, (uint)lcs.Item2),
                    //    End = new Position((uint)lce.Item1, (uint)lce.Item2)
                    //};
                    //Logger.Log.WriteLine("returning " + quick_info.Display);
                }
                catch (Exception)
                { }
                return hover;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentSignatureHelpName)]
        public JToken TextDocumentSignatureHelpName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentSignatureHelp");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        // TextDocumentDeclaration does not exist in Microsoft.VisualStudio.LanguageServer.Protocol 16.3.57
        // but does in version 3.14 of LSP.

        [JsonRpcMethod(Methods.TextDocumentDefinitionName)]
        public object[] TextDocumentDefinitionName(JToken arg)
        {
            lock (_object)
            {
                object[] result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentDefinition");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    TextDocumentPositionParams request = arg.ToObject<TextDocumentPositionParams>();
                    string fn = request.TextDocument.Uri;
                    Position position = request.Position;
                    int line = 1 + (int)position.Line; // LSP zero based, Antlr 1 based.
                    int column = (int)position.Character; // zero based, Antlr 0 based.
                    List<object> locations = new List<object>();

                    // Find def
                    var found = data.TryGetValue(fn, out ParsingResultSet prs);

                    var sym = prs.Refs.Where(r =>
                    {
                        var si = r.SourceInterval.a;
                        var ts = prs.Parser.TokenStream.Get(si);
                        return ts.Line == line && ts.Column <= column &&
                               column <= ts.Column + (ts.StopIndex - ts.StartIndex + 1);
                    }).FirstOrDefault();
                    if (sym != null)
                    {
                        foreach (var pair in data)
                        {
                            prs = pair.Value;
                            fn = pair.Key;
                            var text = sym.GetText();
                            if ('a' <= text[0] && text[0] <= 'z')
                            {
                                var l = prs.ParserDefs.Where(r => { return text == r.GetText(); }
                                ).FirstOrDefault();
                                if (l != null)
                                {
                                    LspTypes.Location location = new LspTypes.Location
                                    {
                                        Uri = new Uri(fn).ToString()
                                    };
                                    location.Range = new LspTypes.Range();
                                    var si = l.SourceInterval.a;
                                    var ts = prs.Parser.TokenStream.Get(si);
                                    location.Range.Start =
                                        new Position((uint)ts.Line - 1,
                                            (uint)ts.Column); // -1 because it's LSP zero based line numbers.
                                    location.Range.End = new Position((uint)ts.Line - 1,
                                        (uint)(ts.Column + ts.StopIndex - ts.StartIndex + 1));
                                    locations.Add(location);
                                }
                                result = locations.ToArray();
                            }
                            else if ('A' <= text[0] && text[0] <= 'Z')
                            {
                                var l = prs.LexerDefs.Where(r => { return text == r.GetText(); }
                                ).FirstOrDefault();
                                if (l != null)
                                {
                                    LspTypes.Location location = new LspTypes.Location
                                    {
                                        Uri = new Uri(fn).ToString()
                                    };
                                    location.Range = new LspTypes.Range();
                                    var si = l.SourceInterval.a;
                                    var ts = prs.Parser.TokenStream.Get(si);
                                    location.Range.Start =
                                        new Position((uint)ts.Line - 1,
                                            (uint)ts.Column); // -1 because it's LSP zero based line numbers.
                                    location.Range.End = new Position((uint)ts.Line - 1,
                                        (uint)(ts.Column + ts.StopIndex - ts.StartIndex + 1));
                                    locations.Add(location);
                                }
                                result = locations.ToArray();
                            }
                        }
                    }

                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //Position position = request.Position;
                    //int line = (int)position.Line;
                    //int character = (int)position.Character;
                    //int index = new LanguageServer.Module().GetIndex(line, character, document);
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine("position index = " + index);
                    //    (int, int) back = new LanguageServer.Module().GetLineColumn(index, document);
                    //    Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //}
                    //var found = new LanguageServer.Module().FindDefs(index, document);
                    //List<object> locations = new List<object>();
                    //foreach (var f in found)
                    //{
                    //    LspTypes.Location location = new LspTypes.Location
                    //    {
                    //        Uri = new Uri(f.Uri.FullPath).ToString()
                    //    };
                    //    Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                    //    location.Range = new LspTypes.Range();
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(f.Range.Start.Value, def_document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(f.Range.End.Value, def_document);
                    //    location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    //    location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    //    locations.Add(location);
                    //}
                    //result = locations.ToArray();
                }
                catch (Exception)
                { }
                return result;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentTypeDefinitionName)]
        public object[] TextDocumentTypeDefinitionName(JToken arg)
        {
            lock (_object)
            {
                object[] result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentTypeDefinitionName");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    TextDocumentPositionParams request = arg.ToObject<TextDocumentPositionParams>();
                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //Position position = request.Position;
                    //int line = (int)position.Line;
                    //int character = (int)position.Character;
                    //int index = new LanguageServer.Module().GetIndex(line, character, document);
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine("position index = " + index);
                    //    (int, int) back = new LanguageServer.Module().GetLineColumn(index, document);
                    //    Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //}
                    //var found = new LanguageServer.Module().FindDefs(index, document);
                    //List<object> locations = new List<object>();
                    //foreach (var f in found)
                    //{
                    //    LspTypes.Location location = new LspTypes.Location
                    //    {
                    //        Uri = new Uri(f.Uri.FullPath).ToString()
                    //    };
                    //    Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                    //    location.Range = new LspTypes.Range();
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(f.Range.Start.Value, def_document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(f.Range.End.Value, def_document);
                    //    location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    //    location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    //    locations.Add(location);
                    //}
                    //result = locations.ToArray();
                }
                catch (Exception)
                { }
                return result;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentImplementationName)]
        public object[] TextDocumentImplementationName(JToken arg)
        {
            lock (_object)
            {
                object[] result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentImplementation");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    TextDocumentPositionParams request = arg.ToObject<TextDocumentPositionParams>();
                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //Position position = request.Position;
                    //int line = (int)position.Line;
                    //int character = (int)position.Character;
                    //int index = new LanguageServer.Module().GetIndex(line, character, document);
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine("position index = " + index);
                    //    (int, int) back = new LanguageServer.Module().GetLineColumn(index, document);
                    //    Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //}
                    //var found = new LanguageServer.Module().FindDefs(index, document);
                    //List<object> locations = new List<object>();
                    //foreach (var f in found)
                    //{
                    //    var location = new LspTypes.Location
                    //    {
                    //        Uri = new Uri(f.Uri.FullPath).ToString()
                    //    };
                    //    Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                    //    location.Range = new LspTypes.Range();
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(f.Range.Start.Value, def_document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(f.Range.End.Value, def_document);
                    //    location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    //    location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    //    locations.Add(location);
                    //}
                    //result = locations.ToArray();
                }
                catch (Exception)
                { }
                return result;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentReferencesName)]
        public object[] TextDocumentReferencesName(JToken arg)
        {
            lock (_object)
            {
                object[] result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentReferences");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    TextDocumentPositionParams request = arg.ToObject<TextDocumentPositionParams>();
                    string fn = request.TextDocument.Uri;
                    Position position = request.Position;
                    int line = 1 + (int)position.Line; // LSP zero based, Antlr 1 based.
                    int column = (int)position.Character; // zero based, Antlr 0 based.
                    List<object> locations = new List<object>();

                    // Find whatever we are pointing to.
                    var found = data.TryGetValue(fn, out ParsingResultSet prs);

                    var sym = prs.Refs.Where(r =>
                    {
                        var si = r.SourceInterval.a;
                        var ts = prs.Parser.TokenStream.Get(si);
                        return ts.Line == line && ts.Column <= column &&
                               column <= ts.Column + (ts.StopIndex - ts.StartIndex + 1);
                    }).FirstOrDefault();
                    if (sym != null)
                    {
                        foreach (var pair in data)
                        {
                            prs = pair.Value;
                            fn = pair.Key;
                            var text = sym.GetText();
                            var all = prs.Refs.Where(r => { return text == r.GetText(); }
                                ).ToList();
                            foreach (var l in all)
                            {
                                LspTypes.Location location = new LspTypes.Location
                                {
                                    Uri = new Uri(fn).ToString()
                                };
                                location.Range = new LspTypes.Range();
                                var si = l.SourceInterval.a;
                                var ts = prs.Parser.TokenStream.Get(si);
                                location.Range.Start =
                                    new Position((uint)ts.Line - 1,
                                        (uint)ts.Column); // -1 because it's LSP zero based line numbers.
                                location.Range.End = new Position((uint)ts.Line - 1,
                                    (uint)(ts.Column + ts.StopIndex - ts.StartIndex + 1));
                                locations.Add(location);
                            }
                        }
                    }
                    result = locations.ToArray();


                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //Position position = request.Position;
                    //int line = (int)position.Line;
                    //int character = (int)position.Character;
                    //int index = new LanguageServer.Module().GetIndex(line, character, document);
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine("position index = " + index);
                    //    (int, int) back = new LanguageServer.Module().GetLineColumn(index, document);
                    //    Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //}
                    //var found = new LanguageServer.Module().FindRefsAndDefs(index, document);
                    //List<object> locations = new List<object>();
                    //foreach (var f in found)
                    //{
                    //    var location = new LspTypes.Location
                    //    {
                    //        Uri = new Uri(f.Uri.FullPath).ToString()
                    //    };
                    //    Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                    //    location.Range = new LspTypes.Range();
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(f.Range.Start.Value, def_document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(f.Range.End.Value + 1, def_document);
                    //    location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    //    location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    //    locations.Add(location);
                    //}
                    //result = locations.ToArray();
                    //if (trace)
                    //{
                    //    Logger.Log.Write("returning ");
                    //    Logger.Log.WriteLine(string.Join(
                    //        System.Environment.NewLine, result.Select(s =>
                    //    {
                    //        var v = (LspTypes.Location)s;
                    //        var dd = CheckDoc(v.Uri);
                    //        return "<" + v.Uri +
                    //            ",[" + new LanguageServer.Module().GetIndex(
                    //                (int)v.Range.Start.Line,
                    //                (int)v.Range.Start.Character,
                    //                dd)
                    //            + ".."
                    //            + new LanguageServer.Module().GetIndex(
                    //                (int)v.Range.End.Line,
                    //                (int)v.Range.End.Character,
                    //                dd)
                    //            + "]>";
                    //    })));
                    //}
                    //server.ShowMessage("" + result.Length + " results.", MessageType.Info);
                }
                catch (Exception eeks)
                {
                    Logger.Log.WriteLine("Exception: " + eeks.ToString());
                }
                return result;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentDocumentHighlightName)]
        public object[] TextDocumentDocumentHighlightName(JToken arg)
        {
            lock (_object)
            {
                object[] result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentDocumentHighlight");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    TextDocumentPositionParams request = arg.ToObject<TextDocumentPositionParams>();
                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //Position position = request.Position;
                    //int line = (int)position.Line;
                    //int character = (int)position.Character;
                    //int index = new LanguageServer.Module().GetIndex(line, character, document);
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine("position index = " + index);
                    //    (int, int) back = new LanguageServer.Module().GetLineColumn(index, document);
                    //    Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //}
                    //var found = new LanguageServer.Module().FindRefsAndDefs(index, document);
                    //List<object> locations = new List<object>();
                    //foreach (var f in found)
                    //{
                    //    if (f.Uri.FullPath != document.FullPath)
                    //    {
                    //        continue;
                    //    }
                    //    DocumentHighlight location = new DocumentHighlight();
                    //    Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                    //    location.Range = new LspTypes.Range();
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(f.Range.Start.Value, def_document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(f.Range.End.Value + 1, def_document);
                    //    location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    //    location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    //    locations.Add(location);
                    //}
                    //result = locations.ToArray();
                }
                catch (Exception)
                { }
                return result;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentDocumentSymbolName)]
        public object[] TextDocumentDocumentSymbolName(JToken arg)
        {
            lock (_object)
            {
                object[] result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentDocumentSymbol "
                            + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    DocumentSymbolParams request = arg.ToObject<DocumentSymbolParams>();
                    var fn = request.TextDocument.Uri;
                    var found = data.TryGetValue(fn, out ParsingResultSet prs);
                    if (!found)
                    {

                    }

                    List<object> symbols = new List<object>();
                    foreach (var s in prs.ParserDefs)
                    {
                        SymbolInformation si = new SymbolInformation();
                        si.Kind = SymbolKind.Variable; // Nonterminal
                        si.Name = s.GetText();
                        si.Location = new LspTypes.Location
                        {
                            Uri = request.TextDocument.Uri
                        };
                        var t = s as TerminalNodeImpl;
                        si.Location.Range = new LspTypes.Range
                        {
                            Start = new Position((uint)t.Symbol.Line, (uint)t.Symbol.Column),
                            End = new Position((uint)t.Symbol.Line, (uint)(t.Symbol.Column + t.Symbol.Text.Length))
                        };
                        symbols.Add(si);
                    }
                    foreach (var s in prs.LexerDefs)
                    {
                        SymbolInformation si = new SymbolInformation();
                        si.Kind = SymbolKind.Enum; // terminal
                        si.Name = s.GetText();
                        si.Location = new LspTypes.Location
                        {
                            Uri = request.TextDocument.Uri
                        };
                        var t = s as TerminalNodeImpl;
                        si.Location.Range = new LspTypes.Range
                        {
                            Start = new Position((uint)t.Symbol.Line, (uint)t.Symbol.Column),
                            End = new Position((uint)t.Symbol.Line, (uint)(t.Symbol.Column + t.Symbol.Text.Length))
                        };
                        symbols.Add(si);
                    }
                    foreach (var s in prs.Refs)
                    {
                        SymbolInformation si = new SymbolInformation();
                        si.Name = s.GetText();
                        var fchar = si.Name[0];
                        si.Kind = (fchar >= 'a' && fchar <= 'z') ? SymbolKind.Variable : SymbolKind.Enum;
                        si.Location = new LspTypes.Location
                        {
                            Uri = request.TextDocument.Uri
                        };
                        var t = s as TerminalNodeImpl;
                        si.Location.Range = new LspTypes.Range
                        {
                            Start = new Position((uint)t.Symbol.Line, (uint)t.Symbol.Column),
                            End = new Position((uint)t.Symbol.Line, (uint)(t.Symbol.Column + t.Symbol.Text.Length))
                        };
                        symbols.Add(si);
                    }
                    result = symbols.ToArray();
                    Logger.Log.WriteLine("return " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

                    //    else if (s.kind == 1)
                    //    {
                    //        si.Kind = SymbolKind.Enum; // Terminal
                    //    }
                    //    else if (s.kind == 2)
                    //    {
                    //        continue;
                    //        // si.Kind = SymbolKind.String; // Comment
                    //    }
                    //    else if (s.kind == 3)
                    //    {
                    //        continue;
                    //        // si.Kind = SymbolKind.Key; // Keyword
                    //    }
                    //    else if (s.kind == 4)
                    //    {
                    //        continue;
                    //        // si.Kind = SymbolKind.Constant; // Literal
                    //    }
                    //    else if (s.kind == 5)
                    //    {
                    //        si.Kind = SymbolKind.Event; // Mode
                    //    }
                    //    else if (s.kind == 6)
                    //    {
                    //        si.Kind = SymbolKind.Object; // Channel
                    //    }
                    //    else
                    //    {
                    //        // si.Kind = 0; // Default.
                    //        continue;
                    //    }

                    //    si.Name = s.name;
                    //    si.Location = new LspTypes.Location
                    //    {
                    //        Uri = request.TextDocument.Uri
                    //    };
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(s.range.Start.Value, document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(s.range.End.Value, document);
                    //    si.Location.Range = new LspTypes.Range
                    //    {
                    //        Start = new Position((uint)lcs.Item1, (uint)lcs.Item2),
                    //        End = new Position((uint)lce.Item1, (uint)lce.Item2)
                    //    };
                    //    symbols.Add(si);
                    //}
                    //Logger.Log.WriteLine("Af list " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    //if (trace)
                    //{
                    //    Logger.Log.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    //    Logger.Log.Write("returning ");
                    //    Logger.Log.WriteLine(string.Join(" ", symbols.Select(s =>
                    //    {
                    //        SymbolInformation v = (SymbolInformation)s;
                    //        return "<" + v.Name + "," + v.Kind
                    //            + ",[" + new LanguageServer.Module().GetIndex(
                    //                (int)v.Location.Range.Start.Line,
                    //                (int)v.Location.Range.Start.Character,
                    //                document)
                    //            + ".."
                    //            + new LanguageServer.Module().GetIndex(
                    //                (int)v.Location.Range.End.Line,
                    //                (int)v.Location.Range.End.Character,
                    //                document)
                    //            + "]>";
                    //    })));
                    //}
                    //result = symbols.ToArray();
                    //Logger.Log.WriteLine("B4 return " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                }
                catch (Exception)
                { }
                return result;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentCodeActionName)]
        public JToken TextDocumentCodeActionName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentCodeAction");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentCodeLensName)]
        public JToken TextDocumentCodeLensName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentCodeLens");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.CodeLensResolveName)]
        public JToken CodeLensResolveName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- CodeLensResolve");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentDocumentLinkName)]
        public JToken TextDocumentDocumentLinkName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentDocumentLink");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.DocumentLinkResolveName)]
        public JToken DocumentLinkResolveName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- DocumentLinkResolve");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentFormattingName)]
        public object[] TextDocumentFormattingName(JToken arg)
        {
            lock (_object)
            {
                object[] result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentFormatting");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    DocumentFormattingParams request = arg.ToObject<DocumentFormattingParams>();
                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //var new_list = new List<LspTypes.TextEdit>();
                    //LanguageServer.TextEdit[] changes = new LanguageServer.Module().Reformat(document);
                    //int count = 0;
                    //foreach (LanguageServer.TextEdit delta in changes)
                    //{
                    //    var new_edit = new LspTypes.TextEdit
                    //    {
                    //        Range = new LspTypes.Range()
                    //    };
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(delta.range.Start.Value, document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(delta.range.End.Value, document);
                    //    new_edit.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    //    new_edit.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    //    new_edit.NewText = delta.NewText;
                    //    new_list.Add(new_edit);
                    //    count++;
                    //}
                    //result = new_list.ToArray();
                }
                catch (Exception)
                { }
                return result;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentRangeFormattingName)]
        public JToken TextDocumentRangeFormattingName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentRangeFormatting");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentOnTypeFormattingName)]
        public JToken TextDocumentOnTypeFormattingName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentOnTypeFormatting");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentRenameName)]
        public WorkspaceEdit TextDocumentRenameName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentRename");
                    Logger.Log.WriteLine(arg.ToString());
                }
                WorkspaceEdit edit = null;
                try
                {
                    RenameParams request = arg.ToObject<RenameParams>();
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;


                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //if (!ignore_next_change.ContainsKey(document.FullPath))
                    //{
                    //    int index = new LanguageServer.Module().GetIndex(line, character, document);
                    //    if (trace)
                    //    {
                    //        Logger.Log.WriteLine("position index = " + index);
                    //        (int, int) back = new LanguageServer.Module().GetLineColumn(index, document);
                    //        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    //    }
                    //    string new_name = request.NewName;
                    //    Dictionary<string, LanguageServer.TextEdit[]> changes = new LanguageServer.Module().Rename(index, new_name, document);
                    //    edit = new WorkspaceEdit();
                    //    int count = 0;
                    //    var edit_changes_array = new Dictionary<string, LspTypes.TextEdit[]>();
                    //    foreach (KeyValuePair<string, LanguageServer.TextEdit[]> pair in changes)
                    //    {
                    //        string ffn = pair.Key;
                    //        Uri uri = new Uri(ffn);
                    //        Document doc = CheckDoc(ffn);
                    //        LanguageServer.TextEdit[] val = pair.Value;
                    //        var new_list = new List<LspTypes.TextEdit>();
                    //        foreach (LanguageServer.TextEdit v in val)
                    //        {
                    //            var new_edit = new LspTypes.TextEdit
                    //            {
                    //                Range = new LspTypes.Range()
                    //            };
                    //            (int, int) lcs = new LanguageServer.Module().GetLineColumn(v.range.Start.Value, doc);
                    //            (int, int) lce = new LanguageServer.Module().GetLineColumn(v.range.End.Value, doc);
                    //            new_edit.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    //            new_edit.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    //            new_edit.NewText = v.NewText;
                    //            new_list.Add(new_edit);
                    //            count++;
                    //        }
                    //        edit_changes_array.Add(uri.ToString(), new_list.ToArray());
                    //    }
                    //    edit.Changes = edit_changes_array;
                    //}
                    //else
                    //{
                    //    ignore_next_change.Remove(document.FullPath);
                    //}
                }
                catch (Exception)
                { }

                return edit;
            }
        }

        [JsonRpcMethod(Methods.TextDocumentFoldingRangeName)]
        public JToken TextDocumentFoldingRangeName(JToken arg)
        {
            lock (_object)
            {
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentFoldingRange");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                }
                catch (Exception)
                { }
                return null;
            }
        }

        //[JsonRpcMethod(Methods.TextDocumentSemanticTokensFullName)]
        [JsonRpcMethod(Methods.TextDocumentSemanticTokensFull)]
        public SemanticTokens SemanticTokens(JToken arg)
        {
            lock (_object)
            {
                SemanticTokens result = null;
                try
                {
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- SemanticTokens");
                        Logger.Log.WriteLine(arg.ToString());
                    }

                    DocumentSymbolParams request = arg.ToObject<DocumentSymbolParams>();
                    string fn = request.TextDocument.Uri;
                    //string fn = request.TextDocument.Uri.LocalPath;
                    var found = data.TryGetValue(fn, out ParsingResultSet prs);
                    if (!found)
                    {

                    }

                    List<object> symbols = new List<object>();
                    List<uint> d = new List<uint>();
                    SortedList<int, IParseTree> sorted_symbols = new SortedList<int, IParseTree>();
                    foreach (var n in prs.ParserDefs) sorted_symbols.Add(prs.Parser.TokenStream.Get(n.SourceInterval.a).StartIndex, n);
                    foreach (var n in prs.LexerDefs) sorted_symbols.Add(prs.Parser.TokenStream.Get(n.SourceInterval.a).StartIndex, n);
                    foreach (var n in prs.Refs) sorted_symbols.Add(prs.Parser.TokenStream.Get(n.SourceInterval.a).StartIndex, n);

                    (int,int) start = (1, 0);

                    foreach (var pair in sorted_symbols)
                    {
                        var node = pair.Value;
                        if (trace)
                        {
                            Logger.Log.WriteLine("Found " + node.ToString());
                        }

                        // Parser symbol
                        var kind = 0;
                        int ai = node.SourceInterval.a;
                        int bi = node.SourceInterval.b;
                        var tai = prs.Parser.TokenStream.Get(ai);
                        var tbi = prs.Parser.TokenStream.Get(bi);
                        // Stupid LSP works with line/columns rather than absolute index into buffer.
                        // Convert.
                        (int, int) lc_start = start;
                        (int, int) lcs = (tai.Line, tai.Column);
                        var len = tbi.StopIndex - tbi.StartIndex + 1;
                        //(int, int) lce = (tbi.Line, tbi.Column + len);

                        var diff_l = lcs.Item1 - lc_start.Item1;
                        var diff_c = diff_l != 0 ? lcs.Item2 : lcs.Item2 - lc_start.Item2;
                        // line
                        d.Add((uint)diff_l);
                        // startChar
                        d.Add((uint)diff_c);
                        // length
                        d.Add((uint)(tbi.StopIndex - tai.StartIndex + 1));
                        // tokenType
                        d.Add((uint)kind);
                        // tokenModifiers
                        d.Add(0);
                        start = (tai.Line, tai.Column);
                    }
                    result = new SemanticTokens();
                    result.Data = d.ToArray();
                    if (trace)
                    {
                        Logger.Log.Write("returning semantictokens");
                        Logger.Log.WriteLine(string.Join(" ", data));
                    }


                    //Document document = CheckDoc(request.TextDocument.Uri);
                    //var r = new LanguageServer.Module().Get(document);
                    //List<object> symbols = new List<object>();
                    //List<uint> data = new List<uint>();
                    //// Let us fill up temp values to figure out.
                    //int start = 0;
                    //var new_r = r.ToList();
                    //foreach (var s in new_r)
                    //{
                    //    int kind;
                    //    if (s.kind == 0)
                    //    {
                    //        // Parser symbol
                    //        kind = 0;
                    //    }
                    //    else if (s.kind == 1)
                    //    {
                    //        // Lexer symbol
                    //        kind = 1;
                    //    }
                    //    else if (s.kind == 2)
                    //    {
                    //        // Comment
                    //        kind = 3;
                    //    }
                    //    else if (s.kind == 3)
                    //    {
                    //        // Keyword
                    //        kind = 5;
                    //    }
                    //    else if (s.kind == 4)
                    //    {
                    //        // Literal
                    //        kind = 4;
                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }

                    //    (int, int) lc_start = new LanguageServer.Module().GetLineColumn(start, document);
                    //    (int, int) lcs = new LanguageServer.Module().GetLineColumn(s.start, document);
                    //    (int, int) lce = new LanguageServer.Module().GetLineColumn(s.end, document);

                    //    var diff_l = lcs.Item1 - lc_start.Item1;
                    //    var diff_c = diff_l != 0 ? lcs.Item2 : lcs.Item2 - lc_start.Item2;
                    //    // line
                    //    data.Add((uint)diff_l);
                    //    // startChar
                    //    data.Add((uint)diff_c);
                    //    // length
                    //    data.Add((uint)(s.end - s.start + 1));
                    //    // tokenType
                    //    data.Add((uint)kind);
                    //    // tokenModifiers
                    //    data.Add(0);

                    //    start = s.start;
                    //}
                    //result = new SemanticTokens();
                    //result.Data = data.ToArray();
                    //if (trace)
                    //{
                    //    Logger.Log.Write("returning semantictokens");
                    //    Logger.Log.WriteLine(string.Join(" ", data));
                    //}
                }
                catch (Exception e)
                {
                    //server.ShowMessage(e.Message, (Microsoft.VisualStudio.LanguageServer.Protocol.MessageType)MessageType.Info);
                    server.ShowMessage(e.Message, MessageType.Info);
                }

                return result;
            }
        }

        private (int, int) GetLineColumn(int start)
        {
            throw new NotImplementedException();
        }

        ParsingResultSet DoParse(string parser_type, string txt, string prefix, string input_name, int row_number)
        {
            Type type = null;
            if (parser_type == null || parser_type == "")
            {
                string path = server.config.ParserLocation != null ? server.config.ParserLocation
                    : Environment.CurrentDirectory + Path.DirectorySeparatorChar;
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path + "Generated-CSharp/bin/Debug/net7.0/";
                var exists = File.Exists(full_path + "Test.dll");
                if (!exists) full_path = path + "bin/Debug/net7.0/";
                full_path = Path.GetFullPath(full_path);
                Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
                Assembly asm = Assembly.LoadFile(full_path + server.config.Dll + ".dll");
                var xxxxxx = asm1.GetTypes();
                Type[] types = asm.GetTypes();
                type = asm.GetType("Program");
            }
            else
            {
                // Get this assembly.
                System.Reflection.Assembly a = this.GetType().Assembly;
                string path = a.Location;
                path = Path.GetDirectoryName(path);
                path = path.Replace("\\", "/");
                if (!path.EndsWith("/")) path = path + "/";
                var full_path = path;
                var exists = File.Exists(full_path + parser_type + ".dll");
                full_path = Path.GetFullPath(full_path);
                Assembly asm1 = Assembly.LoadFile(full_path + "Antlr4.Runtime.Standard.dll");
                Assembly asm = Assembly.LoadFile(full_path + parser_type + ".dll");
                var xxxxxx = asm1.GetTypes();
                Type[] types = asm.GetTypes();
                type = asm.GetType("Program");
            }

            MethodInfo methodInfo = type.GetMethod("SetupParse2");
            object[] parm1 = new object[] { txt, true };
            var res = methodInfo.Invoke(null, parm1);

            MethodInfo methodInfo2 = type.GetMethod("Parse2");
            object[] parm2 = new object[] { };
            DateTime before = DateTime.Now;
            var res2 = methodInfo2.Invoke(null, parm2);
            DateTime after = DateTime.Now;

            MethodInfo methodInfo3 = type.GetMethod("AnyErrors");
            object[] parm3 = new object[] { };
            var res3 = methodInfo3.Invoke(null, parm3);
            var result = "";
            if ((bool)res3)
            {
                result = "fail";
            }
            else
            {
                result = "success";
            }
            System.Console.Error.WriteLine(prefix + "CSharp " + row_number + " " + input_name + " " + result + " " + (after - before).TotalSeconds);
            var parser = type.GetProperty("Parser").GetValue(null, new object[0]) as Antlr4.Runtime.Parser;
            var lexer = type.GetProperty("Lexer").GetValue(null, new object[0]) as Antlr4.Runtime.Lexer;
            var tokstream = type.GetProperty("TokenStream").GetValue(null, new object[0]) as ITokenStream;
            var charstream = type.GetProperty("CharStream").GetValue(null, new object[0]) as ICharStream;
            var commontokstream = tokstream as CommonTokenStream;
            var r5 = type.GetProperty("Input").GetValue(null, new object[0]);
            var tree = res2 as IParseTree;
            var t2 = tree as ParserRuleContext;
            //if (!config.Quiet) System.Console.Error.WriteLine("Time to parse: " + (after - before));
            //if (!config.Quiet) System.Console.Error.WriteLine("# tokens per sec = " + tokstream.Size / (after - before).TotalSeconds);
            //if (!config.Quiet && config.Verbose) System.Console.Error.WriteLine(LanguageServer.TreeOutput.OutputTree(tree, lexer, parser, commontokstream));
            var tuple = new ParsingResultSet() { Text = (r5 as string), FileName = input_name, Nodes = new IParseTree[] { t2 }, Parser = parser, Lexer = lexer };
            return tuple;
            //return (bool)res3 ? 1 : 0;
        }

    }

    internal class ParsingResultSet
    {
        public ParsingResultSet()
        {
            ParserDefs = new List<IParseTree>();
            LexerDefs = new List<IParseTree>();
            Refs = new List<IParseTree>();
        }

        public string Text { get; set; }
        public string FileName { get; set; }
        public IParseTree[] Nodes { get; set; }
        public Parser Parser { get; set; }
        public Lexer Lexer { get; set; }
        public List<IParseTree> ParserDefs { get; set; }
        public List<IParseTree> LexerDefs { get; set; }
        public List<IParseTree> Refs { get; set; }
    }
}
