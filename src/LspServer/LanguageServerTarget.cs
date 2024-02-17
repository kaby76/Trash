namespace Server
{
    using LoggerNs;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using LspTypes;
    using StreamJsonRpc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Workspaces;

    public class LanguageServerTarget
    {
        private readonly LSPServer server;
        private readonly bool trace = true;
        private readonly Workspaces.Workspace _workspace;
        private static readonly object _object = new object();
        private readonly Dictionary<string, bool> ignore_next_change = new Dictionary<string, bool>();
        private int current_version;

        public LanguageServerTarget(LSPServer server)
        {
            this.server = server;
            _workspace = Workspaces.Workspace.Instance;
        }


        void ApplyChanges(string transaction_name, Dictionary<string, string> ch)
        {
            if (!ch.Any())
            {
                throw new LanguageServerException("No changes were needed, none made.");
            }
            var a = new Dictionary<string, LspTypes.TextEdit[]>();
            Workspace workspace = null;
            foreach (var pair in ch)
            {
                var fn = pair.Key;
                var new_code = pair.Value;
                Document document = CheckDoc(fn);
                workspace = document.Workspace;
                var code = document.Code;
                List<TextEdit> edits = new List<TextEdit>();
                Diff_match_patch diff = new Diff_match_patch();
                List<Diff> diffs = diff.Diff_main(code, new_code);
                List<Patch> patch = diff.Patch_make(diffs);
                {
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
                                offset += len;
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
                                offset += len;
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
                        delta += (p.length2 - p.length1);
                    }
                }
                var changes = edits.ToArray();

                List<LspTypes.TextEdit> new_list = new List<LspTypes.TextEdit>();
                int count = 0;
                foreach (TextEdit delta in changes)
                {
                    var new_edit = new LspTypes.TextEdit
                    {
                        Range = new LspTypes.Range()
                    };
                    (int, int) lcs = new Module().GetLineColumn(delta.range.Start.Value, document);
                    (int, int) lce = new Module().GetLineColumn(delta.range.End.Value, document);
                    new_edit.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                    new_edit.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                    new_edit.NewText = delta.NewText;
                    new_list.Add(new_edit);
                    count++;
                }
                var result = new_list.ToArray();
                a[fn] = result;
                ignore_next_change[fn] = true;
                // This must be done after computing changes since offsets/line/column computations
                // depend on what is currently the source.
                document.Code = new_code;
            }
            // Recompile only after every single change everywhere is in.
            if (workspace != null)
                Module.Compile(workspace);
            server.ApplyEdit(transaction_name, a);
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

                ServerCapabilities capabilities = new ServerCapabilities
                {
                    TextDocumentSync = new TextDocumentSyncOptions
                    {
                        OpenClose = true,
                        Change = TextDocumentSyncKind.Incremental,
                        Save = new SaveOptions
                        {
                            IncludeText = true
                        }
                    },

                    CompletionProvider = null,

                    HoverProvider = true,

                    SignatureHelpProvider = null,

                    // DeclarationProvider not supported.

                    DefinitionProvider = true,

                    TypeDefinitionProvider = false, // Does not make sense for Antlr.

                    ImplementationProvider = false, // Does not make sense for Antlr.

                    ReferencesProvider = true,

                    DocumentHighlightProvider = true,

                    DocumentSymbolProvider = true,

                    CodeLensProvider = null,

                    DocumentLinkProvider = null,

                    // ColorProvider not supported.

                    DocumentFormattingProvider = true,

                    DocumentRangeFormattingProvider = false,

                    RenameProvider = true,

                    FoldingRangeProvider = new SumType<bool, FoldingRangeOptions, FoldingRangeRegistrationOptions>(false),

                    ExecuteCommandProvider = null,

                    // SelectionRangeProvider not supported.

                    WorkspaceSymbolProvider = false,

/*		    SemanticTokensProvider = new SemanticTokensOptions()
                    {
                        Full = true,
                        Range = false,
                        Legend = new SemanticTokensLegend()
                        {
                            tokenTypes = Grammar.AllClasses().ToArray(),
                            tokenModifiers = new string[] {
                                "declaration",
                                "documentation",
                            }
                        }
                    },
                    */
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
                var document = CheckDoc(request.TextDocument.Uri);
                document.LanguageId = request.TextDocument.LanguageId;
                document.Code = request.TextDocument.Text;
                var workspace = document.Workspace;
                Module.Compile(workspace);
            }
        }

        [JsonRpcMethod(Methods.TextDocumentDidChangeName)]
        public void TextDocumentDidChangeName(JToken arg)
        {
            lock (_object)
            {
                if (trace)
                {
                    Logger.Log.WriteLine("<-- TextDocumentDidChange");
                    Logger.Log.WriteLine(arg.ToString());
                }
                DidChangeTextDocumentParams request = arg.ToObject<DidChangeTextDocumentParams>();
                Document document = CheckDoc(request.TextDocument.Uri);

                // Create a thread to handle this update in the correct order.
                //Thread thread = new Thread(() =>
                {
                    try
                    {
                        //       if (request.TextDocument?.Version != null)
                        {
                            int version = (int)request.TextDocument.Version;
                            // spin until this version is current_version.
                            // for (; ; )
                            {
                                //  if (first_change || version == current_version)
                                {
                                    if (!ignore_next_change.ContainsKey(document.FullPath))
                                    {
                                        string code = document.Code;
                                        if (trace)
                                        {
                                            Logger.Log.WriteLine("making change, code before");
                                            Logger.Log.WriteLine(code);
                                            Logger.Log.WriteLine("----------------------");
                                        }
                                        int start_index = 0;
                                        int end_index = 0;
                                        foreach (TextDocumentContentChangeEvent change in request.ContentChanges)
                                        {
                                            var range = change.Range;
                                            int length = (int)(change.RangeLength ?? 0); // Why? range encodes start and end => length!
                                            string text = change.Text;
                                            {
                                                int line = (int)range.Start.Line;
                                                int character = (int)range.Start.Character;
                                                start_index = new Module().GetIndex(line, character, document);
                                            }
                                            {
                                                int line = (int)range.End.Line;
                                                int character = (int)range.End.Character;
                                                end_index = new Module().GetIndex(line, character, document);
                                            }
                                            (int, int) bs = new Module().GetLineColumn(start_index, document);
                                            (int, int) be = new Module().GetLineColumn(end_index, document);
                                            string original = code.Substring(start_index, end_index - start_index);
                                            string n = code.Substring(0, start_index)
                                                    + text
                                                    + code.Substring(0 + start_index + end_index - start_index);
                                            code = n;
                                        }
                                        if (trace)
                                        {
                                            Logger.Log.WriteLine("making change, code after");
                                            Logger.Log.WriteLine(code);
                                            Logger.Log.WriteLine("----------------------");
                                        }
                                        document.Code = code;
                                        var workspace = document.Workspace;
                                        Module.Compile(workspace);
                                    }
                                    else
                                    {
                                        ignore_next_change.Remove(document.FullPath);
                                    }
                                    current_version = version + 1;
                                    //first_change = false;

                                    //          break;
                                }
                                //        Thread.Sleep(100);
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        Thread.ResetAbort();
                    }
                    // })
                    //      {
                    //          IsBackground = true
                    //      };

                    //       thread.Start();
                }
            }
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
                // Nothing to do--who cares.
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
                    if (trace)
                    {
                        Logger.Log.WriteLine("<-- TextDocumentCompletion");
                        Logger.Log.WriteLine(arg.ToString());
                    }
                    CompletionParams request = arg.ToObject<CompletionParams>();
                    Document document = CheckDoc(request.TextDocument.Uri);
                    CompletionContext context = request.Context;
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;
                    int char_index = new Module().GetIndex(line, character, document);
                    if (trace)
                    {
                        Logger.Log.WriteLine("position index = " + char_index);
                        (int, int) back = new Module().GetLineColumn(char_index, document);
                        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    }
                    return null;
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

        public Document CheckDoc(string uri)
        {
            var decoded_string = System.Uri.UnescapeDataString(uri);
            var uri_decoded_string = new Uri(decoded_string);
            var file_name = uri_decoded_string.LocalPath;
            Document document = this._workspace.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                try
                {   // Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(file_name))
                    {
                        // Read the stream to a string, and write the string to the console.
                        string str = sr.ReadToEnd();
                        document.Code = str;
                    }
                }
                catch (IOException)
                {
                }
                Project project = this._workspace.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    Workspaces.Workspace.Instance.AddChild(project);
                }
                project.AddDocument(document);
                document.Changed = true;
                Module.Compile(this._workspace);
            }
            return document;
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;
                    int index = new Module().GetIndex(line, character, document);
                    if (trace)
                    {
                        Logger.Log.WriteLine("position index = " + index);
                        (int, int) back = new Module().GetLineColumn(index, document);
                        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    }
                    var quick_info = new Module().GetQuickInfo(index, document);
                    if (quick_info == null)
                    {
                        return null;
                    }

                    hover = new Hover
                    {
                        Contents = new SumType<string, MarkedString, MarkedString[], MarkupContent>(
                            new MarkupContent()
                            {
                                Kind = MarkupKind.PlainText,
                                Value = quick_info.Display
                            }
                            )
                    };
                    int index_start = quick_info.Range.Start.Value;
                    int index_end = quick_info.Range.End.Value;
                    (int, int) lcs = new Module().GetLineColumn(index_start, document);
                    (int, int) lce = new Module().GetLineColumn(index_end, document);
                    hover.Range = new LspTypes.Range
                    {
                        Start = new Position((uint)lcs.Item1, (uint)lcs.Item2),
                        End = new Position((uint)lce.Item1, (uint)lce.Item2)
                    };
                    Logger.Log.WriteLine("returning " + quick_info.Display);
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;
                    int index = new Module().GetIndex(line, character, document);
                    if (trace)
                    {
                        Logger.Log.WriteLine("position index = " + index);
                        (int, int) back = new Module().GetLineColumn(index, document);
                        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    }
                    var found = new Module().FindDefs(index, document);
                    if (found == null) return result;
                    List<object> locations = new List<object>();
                    foreach (var f in found)
                    {
                        LspTypes.Location location = new LspTypes.Location
                        {
                            Uri = new Uri(f.Uri.FullPath).ToString()
                        };
                        Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                        location.Range = new LspTypes.Range();
                        (int, int) lcs = new Module().GetLineColumn(f.Range.Start.Value, def_document);
                        (int, int) lce = new Module().GetLineColumn(f.Range.End.Value, def_document);
                        location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                        location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                        locations.Add(location);
                    }
                    result = locations.ToArray();
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;
                    int index = new Module().GetIndex(line, character, document);
                    if (trace)
                    {
                        Logger.Log.WriteLine("position index = " + index);
                        (int, int) back = new Module().GetLineColumn(index, document);
                        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    }
                    var found = new Module().FindDefs(index, document);
                    List<object> locations = new List<object>();
                    foreach (var f in found)
                    {
                        LspTypes.Location location = new LspTypes.Location
                        {
                            Uri = new Uri(f.Uri.FullPath).ToString()
                        };
                        Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                        location.Range = new LspTypes.Range();
                        (int, int) lcs = new Module().GetLineColumn(f.Range.Start.Value, def_document);
                        (int, int) lce = new Module().GetLineColumn(f.Range.End.Value, def_document);
                        location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                        location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                        locations.Add(location);
                    }
                    result = locations.ToArray();
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;
                    int index = new Module().GetIndex(line, character, document);
                    if (trace)
                    {
                        Logger.Log.WriteLine("position index = " + index);
                        (int, int) back = new Module().GetLineColumn(index, document);
                        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    }
                    var found = new Module().FindDefs(index, document);
                    List<object> locations = new List<object>();
                    foreach (var f in found)
                    {
                        var location = new LspTypes.Location
                        {
                            Uri = new Uri(f.Uri.FullPath).ToString()
                        };
                        Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                        location.Range = new LspTypes.Range();
                        (int, int) lcs = new Module().GetLineColumn(f.Range.Start.Value, def_document);
                        (int, int) lce = new Module().GetLineColumn(f.Range.End.Value, def_document);
                        location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                        location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                        locations.Add(location);
                    }
                    result = locations.ToArray();
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;
                    int index = new Module().GetIndex(line, character, document);
                    if (trace)
                    {
                        Logger.Log.WriteLine("position index = " + index);
                        (int, int) back = new Module().GetLineColumn(index, document);
                        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    }
                    var found = new Module().FindRefsAndDefs(index, document);
                    List<object> locations = new List<object>();
                    foreach (var f in found)
                    {
                        var location = new LspTypes.Location
                        {
                            Uri = new Uri(f.Uri.FullPath).ToString()
                        };
                        Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                        location.Range = new LspTypes.Range();
                        (int, int) lcs = new Module().GetLineColumn(f.Range.Start.Value, def_document);
                        (int, int) lce = new Module().GetLineColumn(f.Range.End.Value + 1, def_document);
                        location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                        location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                        locations.Add(location);
                    }
                    result = locations.ToArray();
                    if (trace)
                    {
                        Logger.Log.Write("returning ");
                        Logger.Log.WriteLine(string.Join(
                            System.Environment.NewLine, result.Select(s =>
                        {
                            var v = (LspTypes.Location)s;
                            var dd = CheckDoc(v.Uri);
                            return "<" + v.Uri +
                                ",[" + new Module().GetIndex(
                                    (int)v.Range.Start.Line,
                                    (int)v.Range.Start.Character,
                                    dd)
                                + ".."
                                + new Module().GetIndex(
                                    (int)v.Range.End.Line,
                                    (int)v.Range.End.Character,
                                    dd)
                                + "]>";
                        })));
                    }
                    server.ShowMessage("" + result.Length + " results.", MessageType.Info);
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    Position position = request.Position;
                    int line = (int)position.Line;
                    int character = (int)position.Character;
                    int index = new Module().GetIndex(line, character, document);
                    if (trace)
                    {
                        Logger.Log.WriteLine("position index = " + index);
                        (int, int) back = new Module().GetLineColumn(index, document);
                        Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                    }
                    var found = new Module().FindRefsAndDefs(index, document);
                    List<object> locations = new List<object>();
                    foreach (var f in found)
                    {
                        if (f.Uri.FullPath != document.FullPath)
                        {
                            continue;
                        }
                        DocumentHighlight location = new DocumentHighlight();
                        Document def_document = _workspace.FindDocument(f.Uri.FullPath);
                        location.Range = new LspTypes.Range();
                        (int, int) lcs = new Module().GetLineColumn(f.Range.Start.Value, def_document);
                        (int, int) lce = new Module().GetLineColumn(f.Range.End.Value + 1, def_document);
                        location.Range.Start = new Position((uint)lcs.Item1, (uint)lcs.Item2);
                        location.Range.End = new Position((uint)lce.Item1, (uint)lce.Item2);
                        locations.Add(location);
                    }
                    result = locations.ToArray();
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    Logger.Log.WriteLine("B4 GetSymbols " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    var r = new Module().GetSymbols(document);
                    Logger.Log.WriteLine("Af GetSymbols " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    List<object> symbols = new List<object>();
                    foreach (var s in r)
                    {
                        SymbolInformation si = new SymbolInformation();
                        if (s.kind == 0)
                        {
                            si.Kind = SymbolKind.Variable; // Nonterminal
                        }
                        else if (s.kind == 1)
                        {
                            si.Kind = SymbolKind.Enum; // Terminal
                        }
                        else if (s.kind == 2)
                        {
                            continue;
                            // si.Kind = SymbolKind.String; // Comment
                        }
                        else if (s.kind == 3)
                        {
                            continue;
                            // si.Kind = SymbolKind.Key; // Keyword
                        }
                        else if (s.kind == 4)
                        {
                            continue;
                            // si.Kind = SymbolKind.Constant; // Literal
                        }
                        else if (s.kind == 5)
                        {
                            si.Kind = SymbolKind.Event; // Mode
                        }
                        else if (s.kind == 6)
                        {
                            si.Kind = SymbolKind.Object; // Channel
                        }
                        else
                        {
                            // si.Kind = 0; // Default.
                            continue;
                        }

                        si.Name = s.name;
                        si.Location = new LspTypes.Location
                        {
                            Uri = request.TextDocument.Uri
                        };
                        (int, int) lcs = new Module().GetLineColumn(s.range.Start.Value, document);
                        (int, int) lce = new Module().GetLineColumn(s.range.End.Value, document);
                        si.Location.Range = new LspTypes.Range
                        {
                            Start = new Position((uint)lcs.Item1, (uint)lcs.Item2),
                            End = new Position((uint)lce.Item1, (uint)lce.Item2)
                        };
                        symbols.Add(si);
                    }
                    Logger.Log.WriteLine("Af list " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                    if (trace)
                    {
                        Logger.Log.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
                        Logger.Log.Write("returning ");
                        Logger.Log.WriteLine(string.Join(" ", symbols.Select(s =>
                        {
                            SymbolInformation v = (SymbolInformation)s;
                            return "<" + v.Name + "," + v.Kind
                                + ",[" + new Module().GetIndex(
                                    (int)v.Location.Range.Start.Line,
                                    (int)v.Location.Range.Start.Character,
                                    document)
                                + ".."
                                + new Module().GetIndex(
                                    (int)v.Location.Range.End.Line,
                                    (int)v.Location.Range.End.Character,
                                    document)
                                + "]>";
                        })));
                    }
                    result = symbols.ToArray();
                    Logger.Log.WriteLine("B4 return " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    var new_list = new List<LspTypes.TextEdit>();
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    if (!ignore_next_change.ContainsKey(document.FullPath))
                    {
                        int index = new Module().GetIndex(line, character, document);
                        if (trace)
                        {
                            Logger.Log.WriteLine("position index = " + index);
                            (int, int) back = new Module().GetLineColumn(index, document);
                            Logger.Log.WriteLine("back to l,c = " + back.Item1 + "," + back.Item2);
                        }
                        string new_name = request.NewName;
                    }
                    else
                    {
                        ignore_next_change.Remove(document.FullPath);
                    }
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
                    Document document = CheckDoc(request.TextDocument.Uri);
                    var r = new Module().Get(document);
                    List<object> symbols = new List<object>();
                    List<uint> data = new List<uint>();
                    // Let us fill up temp values to figure out.
                    int start = 0;
                    var new_r = r.ToList();
                    foreach (var s in new_r)
                    {
                        int kind;
                        kind = s.kind;

                        (int, int) lc_start = new Module().GetLineColumn(start, document);
                        (int, int) lcs = new Module().GetLineColumn(s.start, document);
                        (int, int) lce = new Module().GetLineColumn(s.end, document);

                        var diff_l = lcs.Item1 - lc_start.Item1;
                        var diff_c = diff_l != 0 ? lcs.Item2 : lcs.Item2 - lc_start.Item2;
                        // line
                        data.Add((uint)diff_l);
                        // startChar
                        data.Add((uint)diff_c);
                        // length
                        data.Add((uint)(s.end - s.start + 1));
                        // tokenType
                        data.Add((uint)kind);
                        // tokenModifiers
                        data.Add((uint)0);

                        start = s.start;
                    }
                    result = new SemanticTokens();
                    result.Data = data.ToArray();
                    if (trace)
                    {
                        Logger.Log.Write("returning semantictokens");
                        for (int i = 0; i < data.Count; ++i)
                        {
                            if (i % 5 == 0) Logger.Log.WriteLine();
                            Logger.Log.Write(" " + data[i]);
                        }
                        Logger.Log.WriteLine();
                    }
                }
                catch (LanguageServerException e)
                {
                    server.ShowMessage(e.Message, MessageType.Info);
                }
                catch (Exception e)
                {
                    server.ShowMessage(e.Message, MessageType.Info);
                }
                return result;
            }
        }
    }
}
