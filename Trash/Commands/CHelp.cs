namespace Trash.Commands
{
    public class CHelp
    {
        public void Help()
        {
        }

        public void Execute(Repl repl, ReplParser.HelpContext tree)
        {
            if (tree.NonWsArgMode() == null)
            {
                System.Console.WriteLine(@"Commands:
alias - Allow a string to be substituted for a word of a simple command.
analyze - perform an analysis of a grammar.
!! (""bang bang"") - repeat the previous command.
!int - repeat a previous command.
cd - change directory.
combine - create a combined grammar from the lexer and parser grammars on the top of stack.
delete - delete nodes in the parse tree for a grammar and reform the grammar.
find - find nodes in the parse tree for a grammar.
fold - replace a sequence of symbols on the RHS of a rule with a LHS symbol of another rule.
foldlit - replace a literal on the RHS of a rule with a LHS symbol of another rule.
group - group common sub-sequences of symbols for alts.
has - analyze a grammar for left/right/direct/indirect recursion.
history - show the history of commands executed.
kleene - convert a rule in BNF to EBNF.
ls - show contents of a directory.
mvsr - move a rule to the top of the grammar.
parse - parse a grammar.
pop - pop the stack of files.
print - print the file at the top of stack.
pwd - print out the current working directory.
quit - exit Trash.
read - read a file and place it on the stack.
rename - rename a symbol in the grammar.
reorder - reorder the rules of a grammar.
rotate - rotate the stack.
rr - replace left recursion with right recursion.
run - generate a parser, compile it, and run it on input.
rup - remove useless parentheses in a grammar rule.
split - split a combined grammar.
stack - print the stack.
ulliteral - convert a lexer rule for a simple string literal to accept a string in any case.
unalias - unalias a command.
unfold - unfold a grammar rule symbol.
ungroup - ungroup a parenthesized alt.
workspace - create a new workspace for the run command.
write - write a file to disk.
");
            }
            else
            {
                var id = tree.NonWsArgMode();
                if (id.GetText() == "alias")
                    new CAlias().Help();
                else if (id.GetText() == "analyze")
                    new CAnalyze().Help();
                else if (id.GetText() == "cd")
                    new CCd().Help();
                else if (id.GetText() == "combine")
                    new CCombine().Help();
                else if (id.GetText() == "delete")
                    new CDelete().Help();
                else if (id.GetText() == "find")
                    new CFind().Help();
                else if (id.GetText() == "fold")
                    new CFold().Help();
                else if (id.GetText() == "foldlit")
                    new CFoldlit().Help();
                else if (id.GetText() == "group")
                    new CGroup().Help();
                else if (id.GetText() == "has")
                    new CHas().Help();
                else if (id.GetText() == "history")
                    new CHistory().Help();
                else if (id.GetText() == "kleene")
                    new CKleene().Help();
                else if (id.GetText() == "ls")
                    new CLs().Help();
                else if (id.GetText() == "mvsr")
                    new CMvsr().Help();
                else if (id.GetText() == "parse")
                    new CParse().Help();
                else if (id.GetText() == "pop")
                    new CPop().Help();
                else if (id.GetText() == "print")
                    new CPrint().Help();
                else if (id.GetText() == "pwd")
                    new CPwd().Help();
                else if (id.GetText() == "quit")
                    new CQuit().Help();
                else if (id.GetText() == "read")
                    new CRead().Help();
                else if (id.GetText() == "rename")
                    new CRename().Help();
                else if (id.GetText() == "reorder")
                    new CReorder().Help();
                else if (id.GetText() == "rotate")
                    new CRotate().Help();
                else if (id.GetText() == "rr")
                    new CRr().Help();
                else if (id.GetText() == "run")
                    new CRun().Help();
                else if (id.GetText() == "rup")
                    new CRup().Help();
                else if (id.GetText() == "split")
                    new CSplit().Help();
                else if (id.GetText() == "stack")
                    new CStack().Help();
                else if (id.GetText() == "ulliteral")
                    new CUlliteral().Help();
                else if (id.GetText() == "unalias")
                    new CUnalias().Help();
                else if (id.GetText() == "unfold")
                    new CUnfold().Help();
                else if (id.GetText() == "ungroup")
                    new CUngroup().Help();
                else if (id.GetText() == "workspace")
                    new CWorkspace().Help();
                else if (id.GetText() == "write")
                    new CWrite().Help();
                else
                {
                    System.Console.WriteLine("Help for unknown command.");
                }
            }
        }
    }
}
