namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
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
        public string script_file = null;
        public int current_line_index = 0;
        public string[] lines = null;
        public int QuietAfter = 10;
        public readonly Docs _docs;
        public Workspace _workspace { get; set; } = new Workspace();

        public Repl(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "-script":
                        script_file = args[i+1];
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
                if (llistener.had_error) throw new Exception("command syntax error");
                if (listener.had_error) throw new Exception("command syntax error");
                var tree = btree.cmd();
                {
                    if (false) ;
                    else if (tree.alias() is ReplParser.AliasContext x_alias)
                    {
                        new CAlias().Execute(this, x_alias);
                    }
                    else if (tree.analyze() is ReplParser.AnalyzeContext x_analyze)
                    {
                        new CAnalyze().Execute(this, x_analyze);
                    }
                    else if (tree.anything() != null)
                    {
                        var anything = tree.anything();
                        if (Aliases.ContainsKey(anything.id().GetText()))
                        {
                            var cmd = Aliases[anything.id().GetText()];
                            var stuff = anything.children.ToList();
                            stuff.RemoveAt(0);
                            var rest = stuff.Select(s => s.GetText()).ToList();
                            var rs = rest != null ? String.Join(" ", rest) : "";
                            cmd = cmd + " " + rs;
                            Execute(cmd);
                            return;
                        }
                        else
                        {
                            System.Console.Error.WriteLine("Unknown command");
                        }
                    }
                    else if (tree.bang() is ReplParser.BangContext x_bang)
                    {
                        new CBang().Execute(this, x_bang);
                        return;
                    }
                    else if (tree.cd() is ReplParser.CdContext x_cd)
                    {
                        new CCd().Execute(this, x_cd);
                    }
                    else if (tree.combine() is ReplParser.CombineContext x_combine)
                    {
                        new CCombine().Execute(this, x_combine);
                    }
                    else if (tree.convert() is ReplParser.ConvertContext x_convert)
                    {
                        new CConvert().Execute(this, x_convert);
                    }
                    else if (tree.delabel() is ReplParser.DelabelContext x_delabel)
                    {
                        new CDelabel().Execute(this, x_delabel);
                    }
                    else if (tree.delete() is ReplParser.DeleteContext x_delete)
                    {
                        new CDelete().Execute(this, x_delete);
                    }
                    else if (tree.dot() is ReplParser.DotContext x_dot)
                    {
                        new CDot().Execute(this, x_dot);
                    }
                    else if (tree.empty() != null)
                    {
                        throw new Repl.DoNotAddToHistory(); // Don't add to history.
                    }
                    else if (tree.find() is ReplParser.FindContext x_find)
                    {
                        new CFind().Execute(this, x_find);
                    }
                    else if (tree.fold() is ReplParser.FoldContext x_fold)
                    {
                        new CFold().Execute(this, x_fold);
                    }
                    else if (tree.foldlit() is ReplParser.FoldlitContext x_foldlit)
                    {
                        new CFoldlit().Execute(this, x_foldlit);
                    }
                    else if (tree.group() is ReplParser.GroupContext x_group)
                    {
                        new CGroup().Execute(this, x_group);
                    }
                    else if (tree.has() is ReplParser.HasContext x_has)
                    {
                        new CHas().Execute(this, x_has);
                    }
                    else if (tree.help() is ReplParser.HelpContext x_help)
                    {
                        new CHelp().Execute(this, x_help);
                    }
                    else if (tree.history() is ReplParser.HistoryContext x_history)
                    {
                        new CHistory().Execute(this, x_history);
                    }
                    else if (tree.kleene() is ReplParser.KleeneContext x_kleene)
                    {
                        new CKleene().Execute(this, x_kleene);
                    }
                    else if (tree.ls() is ReplParser.LsContext x_ls)
                    {
                        new CLs().Execute(this, x_ls);
                    }
                    else if (tree.mvsr() is ReplParser.MvsrContext x_mvsr)
                    {
                        new CMvsr().Execute(this, x_mvsr);
                    }
                    else if (tree.parse() is ReplParser.ParseContext x_parse)
                    {
                        new CParse().Execute(this, x_parse);
                    }
                    else if (tree.pop() is ReplParser.PopContext x_pop)
                    {
                        new CPop().Execute(this, x_pop);
                    }
                    else if (tree.print() is ReplParser.PrintContext x_print)
                    {
                        new CPrint().Execute(this, x_print);
                    }
                    else if (tree.pwd() is ReplParser.PwdContext x_pwd)
                    {
                        new CPwd().Execute(this, x_pwd);
                    }
                    else if (tree.quit() != null)
                    {
                        HistoryAdd(input);
                        throw new Repl.Quit();
                    }
                    else if (tree.read() is ReplParser.ReadContext x_read)
                    {
                        new CRead().Execute(this, x_read);
                    }
                    else if (tree.rename() is ReplParser.RenameContext x_rename)
                    {
                        new CRename().Execute(this, x_rename);
                    }
                    else if (tree.reorder() is ReplParser.ReorderContext x_reorder)
                    {
                        new CReorder().Execute(this, x_reorder);
                    }
                    else if (tree.rotate() is ReplParser.RotateContext x_rotate)
                    {
                        new CRotate().Execute(this, x_rotate);
                    }
                    else if (tree.rr() is ReplParser.RrContext x_rr)
                    {
                        new CRr().Execute(this, x_rr);
                    }
                    else if (tree.run() is ReplParser.RunContext x_run)
                    {
                        new CRun().Execute(this, x_run);
                    }
                    else if (tree.rup() is ReplParser.RupContext x_rup)
                    {
                        new CRup().Execute(this, x_rup);
                    }
                    else if (tree.set() is ReplParser.SetContext x_set)
                    {
                        new CSet().Execute(this, x_set);
                    }
                    else if (tree.split() is ReplParser.SplitContext x_split)
                    {
                        new CSplit().Execute(this, x_split);
                    }
                    else if (tree.stack() is ReplParser.StackContext x_stack)
                    {
                        new CStack().Execute(this, x_stack);
                    }
                    else if (tree.ulliteral() is ReplParser.UlliteralContext x_ulliteral)
                    {
                        new CUlliteral().Execute(this, x_ulliteral);
                    }
                    else if (tree.unalias() is ReplParser.UnaliasContext x_unalias)
                    {
                        new CUnalias().Execute(this, x_unalias);
                    }
                    else if (tree.unfold() is ReplParser.UnfoldContext x_unfold)
                    {
                        new CUnfold().Execute(this, x_unfold);
                    }
                    else if (tree.ungroup() is ReplParser.UngroupContext x_ungroup)
                    {
                        new CUngroup().Execute(this, x_ungroup);
                    }
                    else if (tree.unulliteral() is ReplParser.UnulliteralContext x_unulliteral)
                    {
                        new CUnulliteral().Execute(this, x_unulliteral);
                    }
                    else if (tree.workspace() is ReplParser.WorkspaceContext x_workspace)
                    {
                        new CWorkspace().Execute(this, x_workspace);
                    }
                    else if (tree.write() is ReplParser.WriteContext x_write)
                    {
                        new CWrite().Execute(this, x_write);
                    }
                }
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

                    Console.Error.Write("> ");
                    // We're going to do this in two shots. First scan for an end of line or end of file.
                    // Then, create a normal Antlr stream with the line or lines. We then parse this as usual.
                    // The only thing that we allow multi-lines are explicit '\' continuations, or strings.
                    StringBuilder sb = new StringBuilder();
                    for (; ; )
                    {
                        int i = reader.Read();
                        if (i < 0) break;
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
                    str = new AntlrInputStream(inp);
                    var lexer = new ReplLexer(str);
                    lexer.Mode(ReplLexer.CommandMode);
                    lexer.RemoveErrorListeners();
                    var llistener = new ErrorListener<int>(0);
                    lexer.AddErrorListener(llistener);
                    var tokens = new CommonTokenStream(lexer);
                    int last = str.Index;


                    {
                        var parser = new ReplParser(tokens);
                        var listener = new ErrorListener<IToken>(2);
                        parser.AddErrorListener(listener);
                        //parser.ErrorHandler = new MyBailErrorStrategy(tokens);
                        var btree = parser.cmd_all();
                        if (llistener.had_error || listener.had_error)
                        {
                            System.Console.Error.WriteLine("command syntax error");
                            continue;
                        }
                        var tree = btree.cmd();
                        try
                        {
                            if (false) ;
                            else if (tree.alias() is ReplParser.AliasContext x_alias)
                            {
                                new CAlias().Execute(this, x_alias);
                            }
                            else if (tree.analyze() is ReplParser.AnalyzeContext x_analyze)
                            {
                                new CAnalyze().Execute(this, x_analyze);
                            }
                            else if (tree.anything() != null)
                            {
                                var anything = tree.anything();
                                if (Aliases.ContainsKey(anything.id().GetText()))
                                {
                                    var cmd = Aliases[anything.id().GetText()];
                                    var stuff = anything.children.ToList().Skip(1);
                                    var rest = stuff.Select(s => s.GetText()).ToList();
                                    var rs = rest != null ? String.Join(" ", rest) : "";
                                    cmd = cmd + " " + rs;
                                    Execute(cmd);
                                    last = str.Index;
                                }
                                else
                                {
                                    System.Console.Error.WriteLine("Unknown command");
                                }
                            }
                            else if (tree.bang() is ReplParser.BangContext x_bang)
                            {
                                new CBang().Execute(this, x_bang);
                            }
                            else if (tree.cd() is ReplParser.CdContext x_cd)
                            {
                                new CCd().Execute(this, x_cd);
                            }
                            else if (tree.combine() is ReplParser.CombineContext x_combine)
                            {
                                new CCombine().Execute(this, x_combine);
                            }
                            else if (tree.convert() is ReplParser.ConvertContext x_convert)
                            {
                                new CConvert().Execute(this, x_convert);
                            }
                            else if (tree.delabel() is ReplParser.DelabelContext x_delabel)
                            {
                                new CDelabel().Execute(this, x_delabel);
                            }
                            else if (tree.delete() is ReplParser.DeleteContext x_delete)
                            {
                                new CDelete().Execute(this, x_delete);
                            }
                            else if (tree.dot() is ReplParser.DotContext x_dot)
                            {
                                new CDot().Execute(this, x_dot);
                            }
                            else if (tree.empty() != null)
                            {
                                throw new Repl.DoNotAddToHistory(); // Don't add to history.
                            }
                            else if (tree.find() is ReplParser.FindContext x_find)
                            {
                                new CFind().Execute(this, x_find);
                            }
                            else if (tree.fold() is ReplParser.FoldContext x_fold)
                            {
                                new CFold().Execute(this, x_fold);
                            }
                            else if (tree.foldlit() is ReplParser.FoldlitContext x_foldlit)
                            {
                                new CFoldlit().Execute(this, x_foldlit);
                            }
                            else if (tree.group() is ReplParser.GroupContext x_group)
                            {
                                new CGroup().Execute(this, x_group);
                            }
                            else if (tree.has() is ReplParser.HasContext x_has)
                            {
                                new CHas().Execute(this, x_has);
                            }
                            else if (tree.help() is ReplParser.HelpContext x_help)
                            {
                                new CHelp().Execute(this, x_help);
                            }
                            else if (tree.history() is ReplParser.HistoryContext x_history)
                            {
                                new CHistory().Execute(this, x_history);
                            }
                            else if (tree.kleene() is ReplParser.KleeneContext x_kleene)
                            {
                                new CKleene().Execute(this, x_kleene);
                            }
                            else if (tree.ls() is ReplParser.LsContext x_ls)
                            {
                                new CLs().Execute(this, x_ls);
                            }
                            else if (tree.mvsr() is ReplParser.MvsrContext x_mvsr)
                            {
                                new CMvsr().Execute(this, x_mvsr);
                            }
                            else if (tree.parse() is ReplParser.ParseContext x_parse)
                            {
                                new CParse().Execute(this, x_parse);
                            }
                            else if (tree.pop() is ReplParser.PopContext x_pop)
                            {
                                new CPop().Execute(this, x_pop);
                            }
                            else if (tree.print() is ReplParser.PrintContext x_print)
                            {
                                new CPrint().Execute(this, x_print);
                            }
                            else if (tree.pwd() is ReplParser.PwdContext x_pwd)
                            {
                                new CPwd().Execute(this, x_pwd);
                            }
                            else if (tree.quit() != null)
                            {
                                return;
                            }
                            else if (tree.read() is ReplParser.ReadContext x_read)
                            {
                                new CRead().Execute(this, x_read);
                            }
                            else if (tree.rename() is ReplParser.RenameContext x_rename)
                            {
                                new CRename().Execute(this, x_rename);
                            }
                            else if (tree.reorder() is ReplParser.ReorderContext x_reorder)
                            {
                                new CReorder().Execute(this, x_reorder);
                            }
                            else if (tree.rotate() is ReplParser.RotateContext x_rotate)
                            {
                                new CRotate().Execute(this, x_rotate);
                            }
                            else if (tree.rr() is ReplParser.RrContext x_rr)
                            {
                                new CRr().Execute(this, x_rr);
                            }
                            else if (tree.run() is ReplParser.RunContext x_run)
                            {
                                new CRun().Execute(this, x_run);
                            }
                            else if (tree.rup() is ReplParser.RupContext x_rup)
                            {
                                new CRup().Execute(this, x_rup);
                            }
                            else if (tree.set() is ReplParser.SetContext x_set)
                            {
                                new CSet().Execute(this, x_set);
                            }
                            else if (tree.split() is ReplParser.SplitContext x_split)
                            {
                                new CSplit().Execute(this, x_split);
                            }
                            else if (tree.stack() is ReplParser.StackContext x_stack)
                            {
                                new CStack().Execute(this, x_stack);
                            }
                            else if (tree.ulliteral() is ReplParser.UlliteralContext x_ulliteral)
                            {
                                new CUlliteral().Execute(this, x_ulliteral);
                            }
                            else if (tree.unalias() is ReplParser.UnaliasContext x_unalias)
                            {
                                new CUnalias().Execute(this, x_unalias);
                            }
                            else if (tree.unfold() is ReplParser.UnfoldContext x_unfold)
                            {
                                new CUnfold().Execute(this, x_unfold);
                            }
                            else if (tree.ungroup() is ReplParser.UngroupContext x_ungroup)
                            {
                                new CUngroup().Execute(this, x_ungroup);
                            }
                            else if (tree.unulliteral() is ReplParser.UnulliteralContext x_unulliteral)
                            {
                                new CUnulliteral().Execute(this, x_unulliteral);
                            }
                            else if (tree.workspace() is ReplParser.WorkspaceContext x_workspace)
                            {
                                new CWorkspace().Execute(this, x_workspace);
                            }
                            else if (tree.write() is ReplParser.WriteContext x_write)
                            {
                                new CWrite().Execute(this, x_write);
                            }
                            HistoryAdd(inp);
                            HistoryWrite();
                        }
                        catch (Repl.DoNotAddToHistory)
                        {
                        }
                        catch (Repl.Quit)
                        {
                            return;
                        }
                        catch (Exception eeks)
                        {
                            System.Console.Error.WriteLine(eeks.Message);
                        }
                        finally
                        {
                            //// Reset the input buffer and start all over from
                            //// scratch.
                            //while (str.Index < str.Size && !(str.LA(1) == '\n' || str.LA(1) == '\r'))
                            //    str.Consume();
                            //while (str.Index < str.Size && (str.LA(1) == '\n' || str.LA(1) == '\r'))
                            //    str.Consume();
                            //lexer = new ReplLexer(str);
                            //lexer.RemoveErrorListeners();
                            //lexer.AddErrorListener(new ErrorListener<int>(0));
                            //tokens = new MyUnbufferedTokenStream(lexer);
                        }
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

        public void RecallAliases()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!System.IO.File.Exists(home + Path.DirectorySeparatorChar + PreviousHistoryFfn)) return;
            var history = System.IO.File.ReadAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn);
            History = history.ToList();

            for (int i = 0; i < history.Length; ++i)
            {
                var inp = history[i];
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
                    var tree = btree.cmd();
                    if (tree.alias() is ReplParser.AliasContext x_alias)
                    {
                        new CAlias().Execute(this, x_alias, true);
                    }
                    else if (tree.unalias() is ReplParser.UnaliasContext x_unalias)
                    {
                        new CUnalias().Execute(this, x_unalias);
                    }
                    else if (tree.anything() != null)
                    {
                        var anything = tree.anything();
                        if (Aliases.ContainsKey(anything.id().GetText()))
                        {
                            //var cmd = Aliases[anything.id().GetText()];
                            //var stuff = anything.stuff();
                            //var rest = stuff.Select(s => s.GetText()).ToList();
                            //var rs = rest != null ? String.Join(" ", rest) : "";
                            //cmd = cmd + rs;
                            //RecallAliases(cmd);
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
        }
    }
}
