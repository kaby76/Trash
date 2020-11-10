namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Trash.Commands;
    using Workspaces;

    public class Repl
    {
        public List<string> History { get; set; } = new List<string>();
        const string PreviousHistoryFfn = ".trash.rc";
        public Dictionary<string, string> Aliases { get; set; } = new Dictionary<string, string>();
        public Utils.StackQueue<Document> stack = new Utils.StackQueue<Document>();
        public Utils.StackQueue<Tuple<IParseTree[], Parser, Document, string[]>> tree_stack = new Utils.StackQueue<Tuple<IParseTree[], Parser, Document, string[]>>();
        public string script_file = null;
        public int current_line_index = 0;
        public string[] lines = null;
        public int QuietAfter = 10;
        public readonly Docs _docs;
        public Workspace _workspace { get; set; } = new Workspace();
        public static string Version { get; internal set; } = "8.3";
        public string Prompt { get; set; } = "> ";
        public bool Echo { get; set; } = false;

        public Repl(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "-script":
                        script_file = args[i + 1];
                        break;
                    case "-prompt":
                        Prompt = args[i + 1];
                        break;
                    case "-noprompt":
                        Prompt = null;
                        break;
                    case "-echo":
                        Echo = true;
                        break;
                    default:
                        break;
                }
            }

            // Clean up left overs from "run" commands.
            var path = Path.GetTempPath();
            var regex = new Regex("Antlrvsix[0-9]+$");
            foreach (string dir in Directory.GetDirectories(path))
            {
                if (regex.IsMatch(dir))
                {
                    Directory.Delete(dir, true);
                }
            }

            _docs = new Docs(this);
        }

        void HistoryAdd(string input)
        {
            History.Add(input);
            HistoryWrite();
        }

        void HistoryRead()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!System.IO.File.Exists(home + Path.DirectorySeparatorChar + PreviousHistoryFfn)) return;
            var history = System.IO.File.ReadAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn);
            History = history.ToList();

            RecallAliases();
        }

        void HistoryWrite()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            System.IO.File.WriteAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn, History);
        }

        public string GetArg(ReplParser.ArgContext arg)
        {
            if (arg == null)
                return null;
            if (arg.StringLiteral() != null)
            {
                var sl = arg.StringLiteral().GetText();
                sl = sl.Substring(1, sl.Length - 2);
                return sl;
            }
            else if (arg.NonWsArgMode() != null)
            {
                var a = arg.NonWsArgMode().GetText();
                return a;
            }
            else
                return null;
        }

        public string GetString(ITerminalNode arg)
        {
            if (arg == null)
                return null;
            var sl = arg.GetText();
            sl = sl.Substring(1, sl.Length - 2);
            return sl;
        }

        public void Execute(string input)
        {
            try
            {
                var str = new AntlrInputStream(input);
                var lexer = new ReplLexer(str);
	            lexer.Mode(ReplLexer.CommandMode);
	            lexer.RemoveErrorListeners();
                var llistener = new ErrorListener<int>(0);
                lexer.AddErrorListener(llistener);
                var tokens = new CommonTokenStream(lexer);
                var parser = new ReplParser(tokens);
                var listener = new ErrorListener<IToken>(2);
                parser.AddErrorListener(listener);
                var btree = parser.cmd_all();
                if (llistener.had_error || listener.had_error)
                {
                    throw new Exception("Syntax error for command '" + str + "'");
                }
                var trees = btree.cmd();
                for (int c = 0; c < trees.Length; ++c)
                {
                    try
                    {
                        var tree = trees[c];
                        var is_piped = c > 0;
                        if (false) ;
                        else if (tree.agl() is ReplParser.AglContext x_agl)
                        {
                            new CAgl().Execute(this, x_agl, is_piped);
                        }
                        else if (tree.alias() is ReplParser.AliasContext x_alias)
                        {
                            new CAlias().Execute(this, x_alias, is_piped);
                        }
                        else if (tree.analyze() is ReplParser.AnalyzeContext x_analyze)
                        {
                            new CAnalyze().Execute(this, x_analyze, is_piped);
                        }
                        else if (tree.anything() is ReplParser.AnythingContext x_anything)
                        {
                            new CAnything().Execute(this, x_anything, is_piped);
                        }
                        else if (tree.bang() is ReplParser.BangContext x_bang)
                        {
                            new CBang().Execute(this, x_bang, is_piped);
                            return;
                        }
                        else if (tree.cd() is ReplParser.CdContext x_cd)
                        {
                            new CCd().Execute(this, x_cd, is_piped);
                        }
                        else if (tree.combine() is ReplParser.CombineContext x_combine)
                        {
                            new CCombine().Execute(this, x_combine, is_piped);
                        }
                        else if (tree.convert() is ReplParser.ConvertContext x_convert)
                        {
                            new CConvert().Execute(this, x_convert, is_piped);
                        }
                        else if (tree.delabel() is ReplParser.DelabelContext x_delabel)
                        {
                            new CDelabel().Execute(this, x_delabel, is_piped);
                        }
                        else if (tree.delete() is ReplParser.DeleteContext x_delete)
                        {
                            new CDelete().Execute(this, x_delete, is_piped);
                        }
                        else if (tree.dot() is ReplParser.DotContext x_dot)
                        {
                            new CDot().Execute(this, x_dot, is_piped);
                        }
                        else if (tree.empty() != null)
                        {
                            throw new Repl.DoNotAddToHistory(); // Don't add to history.
                        }
                        else if (tree.fold() is ReplParser.FoldContext x_fold)
                        {
                            new CFold().Execute(this, x_fold, is_piped);
                        }
                        else if (tree.foldlit() is ReplParser.FoldlitContext x_foldlit)
                        {
                            new CFoldlit().Execute(this, x_foldlit, is_piped);
                        }
                        else if (tree.group() is ReplParser.GroupContext x_group)
                        {
                            new CGroup().Execute(this, x_group, is_piped);
                        }
                        else if (tree.has() is ReplParser.HasContext x_has)
                        {
                            new CHas().Execute(this, x_has, is_piped);
                        }
                        else if (tree.help() is ReplParser.HelpContext x_help)
                        {
                            new CHelp().Execute(this, x_help, is_piped);
                        }
                        else if (tree.history() is ReplParser.HistoryContext x_history)
                        {
                            new CHistory().Execute(this, x_history, is_piped);
                        }
                        else if (tree.json() is ReplParser.JsonContext x_json)
                        {
                            new CJson().Execute(this, x_json, is_piped);
                        }
                        else if (tree.kleene() is ReplParser.KleeneContext x_kleene)
                        {
                            new CKleene().Execute(this, x_kleene, is_piped);
                        }
                        else if (tree.ls() is ReplParser.LsContext x_ls)
                        {
                            new CLs().Execute(this, x_ls, is_piped);
                        }
                        else if (tree.mvsr() is ReplParser.MvsrContext x_mvsr)
                        {
                            new CMvsr().Execute(this, x_mvsr, is_piped);
                        }
                        else if (tree.parse() is ReplParser.ParseContext x_parse)
                        {
                            new CParse().Execute(this, x_parse, is_piped);
                        }
                        else if (tree.pop() is ReplParser.PopContext x_pop)
                        {
                            new CPop().Execute(this, x_pop, is_piped);
                        }
                        else if (tree.print() is ReplParser.PrintContext x_print)
                        {
                            new CPrint().Execute(this, x_print, is_piped);
                        }
                        else if (tree.pwd() is ReplParser.PwdContext x_pwd)
                        {
                            new CPwd().Execute(this, x_pwd, is_piped);
                        }
                        else if (tree.quit() != null)
                        {
                            HistoryAdd(input);
                            throw new Repl.Quit();
                        }
                        else if (tree.read() is ReplParser.ReadContext x_read)
                        {
                            new CRead().Execute(this, x_read, is_piped);
                        }
                        else if (tree.rename() is ReplParser.RenameContext x_rename)
                        {
                            new CRename().Execute(this, x_rename, is_piped);
                        }
                        else if (tree.reorder() is ReplParser.ReorderContext x_reorder)
                        {
                            new CReorder().Execute(this, x_reorder, is_piped);
                        }
                        else if (tree.rotate() is ReplParser.RotateContext x_rotate)
                        {
                            new CRotate().Execute(this, x_rotate, is_piped);
                        }
                        else if (tree.rr() is ReplParser.RrContext x_rr)
                        {
                            new CRr().Execute(this, x_rr, is_piped);
                        }
                        else if (tree.run() is ReplParser.RunContext x_run)
                        {
                            new CRun().Execute(this, x_run, is_piped);
                        }
                        else if (tree.rup() is ReplParser.RupContext x_rup)
                        {
                            new CRup().Execute(this, x_rup, is_piped);
                        }
                        else if (tree.set() is ReplParser.SetContext x_set)
                        {
                            new CSet().Execute(this, x_set, is_piped);
                        }
                        else if (tree.split() is ReplParser.SplitContext x_split)
                        {
                            new CSplit().Execute(this, x_split, is_piped);
                        }
                        else if (tree.st() is ReplParser.StContext x_st)
                        {
                            new CSt().Execute(this, x_st, is_piped);
                        }
                        else if (tree.stack() is ReplParser.StackContext x_stack)
                        {
                            new CStack().Execute(this, x_stack, is_piped);
                        }
                        else if (tree.svg() is ReplParser.SvgContext x_svg)
                        {
                            new CSvg().Execute(this, x_svg, is_piped);
                        }
                        else if (tree.text() is ReplParser.TextContext x_text)
                        {
                            new CText().Execute(this, x_text, is_piped);
                        }
                        else if (tree.ulliteral() is ReplParser.UlliteralContext x_ulliteral)
                        {
                            new CUlliteral().Execute(this, x_ulliteral, is_piped);
                        }
                        else if (tree.unalias() is ReplParser.UnaliasContext x_unalias)
                        {
                            new CUnalias().Execute(this, x_unalias, is_piped);
                        }
                        else if (tree.unfold() is ReplParser.UnfoldContext x_unfold)
                        {
                            new CUnfold().Execute(this, x_unfold, is_piped);
                        }
                        else if (tree.ungroup() is ReplParser.UngroupContext x_ungroup)
                        {
                            new CUngroup().Execute(this, x_ungroup, is_piped);
                        }
                        else if (tree.unulliteral() is ReplParser.UnulliteralContext x_unulliteral)
                        {
                            new CUnulliteral().Execute(this, x_unulliteral, is_piped);
                        }
                        else if (tree.version() is ReplParser.VersionContext x_version)
                        {
                            new CVersion().Execute(this, x_version, is_piped);
                        }
                        else if (tree.workspace() is ReplParser.WorkspaceContext x_workspace)
                        {
                            new CWorkspace().Execute(this, x_workspace, is_piped);
                        }
                        else if (tree.write() is ReplParser.WriteContext x_write)
                        {
                            new CWrite().Execute(this, x_write, is_piped);
                        }
                        else if (tree.xgrep() is ReplParser.XgrepContext x_xgrep)
                        {
                            new CXGrep().Execute(this, x_xgrep, is_piped);
                        }
                        else if (tree.xml() is ReplParser.XmlContext x_xml)
                        {
                            new CXml().Execute(this, x_xml, is_piped);
                        }
                        else throw new Exception("Unhandled command!");
                        HistoryAdd(input);
                    }
                    catch (Repl.DoNotAddToHistory)
                    {
                    }
                    catch (Repl.Quit)
                    {
                        throw;
                    }
                    catch (Exception eeks)
                    {
                        System.Console.Error.WriteLine(eeks.Message);
                    }
                }
            }
            catch (Repl.Quit quit)
            {
                throw;
            }
            catch (Exception eeks)
            {
            }
        }

        public void Execute()
        {
            HistoryRead();
            StreamReader reader;
            string input;
            ICharStream str;
            if (script_file != null)
            {
                // Read commands from script, and read a grammar file (Antlr4) from stdin.
                var st = System.IO.File.ReadAllText(script_file);
                reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(st ?? "")));
            }
            else
            {
                reader = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
            }
            { 
                for (; ; )
                {
                    if (Prompt != null)
                    {
                        Console.Error.Write(Prompt);
                    }
                    // We're going to do this in two shots. First scan for an end of line or end of file.
                    // Then, create a normal Antlr stream with the line or lines. We then parse this as usual.
                    // The only thing that we allow multi-lines are explicit '\' continuations, or strings.
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        for (; ; )
                        {
                            int i = reader.Read();
                            if (i < 0) throw new Repl.Quit();
                            char c = (char)i;
                            if (c == '\n') break;
                            else if (c == '\r') continue;
                            else if (c == '"' || c == '\'')
                            {
                                // Read string.
                            }
                            else if (c == '\\')
                            {
                                // escape.
                            }
                            else if (c == ';')
                            {
                                break;
                            }
                            sb.Append(c);
                        }
                        var inp = sb.ToString();
                        if (Echo) System.Console.WriteLine("> " + inp);
                        Execute(inp);
                        if (tree_stack.Any())
                        {
                            var nodes = tree_stack.Pop();
                            foreach (var node in nodes.Item1)
                            {
                                TerminalNodeImpl x = TreeEdits.LeftMostToken(node);
                                var ts = x.Payload.TokenSource;
                                System.Console.WriteLine();
                                System.Console.WriteLine(
                                    TreeOutput.OutputTree(
                                        node,
                                        ts as Lexer,
                                        null).ToString());
                            }
                        }
                    }
                    catch (Repl.Quit quit)
                    {
                        System.Console.WriteLine("Bye!");
                        System.Environment.Exit(0);
                    }
                    catch (Exception eeks)
                    {
                    }
                }
            }
        }

        private class Quit : Exception
        {
            public Quit() { }
        }

        private class DoNotAddToHistory : Exception
        {
            public DoNotAddToHistory() { }
        }

        public void EnactEdits(Dictionary<string, string> results)
        {
            if (results.Count > 0)
            {
                stack.Pop();
                foreach (var res in results)
                {
                    if (res.Value == null) continue;
                    var new_doc = _docs.CreateDoc(res.Key, res.Value);
                    stack.Push(new_doc);
                    _docs.ParseDoc(new_doc, QuietAfter);
                }
            }
            else
            {
                System.Console.Error.WriteLine("no changes");
            }
        }

        public void ExecuteAlias(string inp)
        {
            var str = new AntlrInputStream(inp);
            var lexer = new ReplLexer(str);
            lexer.Mode(ReplLexer.CommandMode);
            lexer.RemoveErrorListeners();
            var llistener = new ErrorListener<int>(0);
            lexer.AddErrorListener(llistener);
            var tokens = new CommonTokenStream(lexer);
            int last = str.Index;
            try
            {
                var parser = new ReplParser(tokens);
                var listener = new ErrorListener<IToken>(2);
                parser.AddErrorListener(listener);
                //parser.ErrorHandler = new MyBailErrorStrategy(tokens);
                var btree = parser.cmd_all();
                if (llistener.had_error) throw new Exception("command syntax error");
                if (listener.had_error) throw new Exception("command syntax error");
                var trees = btree.cmd();
                for (int c = 0; c < trees.Length; ++c)
                {
                    var tree = trees[c];
                    var is_piped = c > 0;
                    if (tree.alias() is ReplParser.AliasContext x_alias)
                    {
                        new CAlias().Execute(this, x_alias, is_piped, true);
                    }
                    else if (tree.unalias() is ReplParser.UnaliasContext x_unalias)
                    {
                        new CUnalias().Execute(this, x_unalias, is_piped);
                    }
                    else if (tree.anything() is ReplParser.AnythingContext x_anything)
                    {
                        new CAnything().Execute(this, x_anything, is_piped, true);
                    }
                }
            }
            catch (Repl.DoNotAddToHistory)
            {
            }
            catch (Exception eeks)
            {
            }
            finally
            {
            }
        }

        public void RecallAliases()
        {
            for (int i = 0; i < History.Count; ++i)
            {
                var inp = History[i];
                ExecuteAlias(inp);
            }
        }
    }
}
