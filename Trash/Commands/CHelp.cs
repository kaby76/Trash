namespace Trash.Commands
{
    public class CHelp
    {
        public void Help()
        {
            System.Console.WriteLine(@"help <id>
Give help for command <id>. If <id> is an alias, the help command indicates what it
is aliased to, and gives help for that command.

Example:
    help help
");
        }

        public void Execute(Repl repl, ReplParser.HelpContext tree, bool piped)
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
delabel - remove all labels from an Antlr4 grammar.
delete - delete nodes in the parse tree for a grammar and reform the grammar.
. (aka 'dot') - print out the parse tree for the file at the top of stack.
find - find nodes in the parse tree for a grammar.
fold - replace a sequence of symbols on the RHS of a rule with a LHS symbol of another rule.
foldlit - replace a literal on the RHS of a rule with a LHS symbol of another rule.
group - group common sub-sequences of symbols for alts.
has - analyze a grammar for left/right/direct/indirect recursion.
help - gives help for commands.
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
set - set a value.
split - split a combined grammar.
st - print the tree using 'toStringTree()'.
stack - print the stack.
text - print the tree source.
ulliteral - convert a lexer rule for a simple string literal to accept a string in any case.
unalias - unalias a command.
unfold - unfold a grammar rule symbol.
ungroup - ungroup a parenthesized alt.
unulliteral - inverse transform of ulliteral.
version - print out the version of Trash.
workspace - create a new workspace for the run command.
write - write a file to disk.
");
            }
            else
            {
                var id = tree.NonWsArgMode();
                var cmd = id.GetText();
                if (cmd == "alias")
                    new CAlias().Help();
                else if (cmd == "analyze")
                    new CAnalyze().Help();
                else if (cmd == "cd")
                    new CCd().Help();
                else if (cmd == "combine")
                    new CCombine().Help();
                else if (cmd == "delabel")
                    new CDelabel().Help();
                else if (cmd == "delete")
                    new CDelete().Help();
                else if (cmd == "dot")
                    new CDot().Help();
                else if (cmd == "find")
                    new CFind().Help();
                else if (cmd == "fold")
                    new CFold().Help();
                else if (cmd == "foldlit")
                    new CFoldlit().Help();
                else if (cmd == "group")
                    new CGroup().Help();
                else if (cmd == "has")
                    new CHas().Help();
                else if (cmd == "history")
                    new CHistory().Help();
                else if (cmd == "kleene")
                    new CKleene().Help();
                else if (cmd == "ls")
                    new CLs().Help();
                else if (cmd == "mvsr")
                    new CMvsr().Help();
                else if (cmd == "parse")
                    new CParse().Help();
                else if (cmd == "pop")
                    new CPop().Help();
                else if (cmd == "print")
                    new CPrint().Help();
                else if (cmd == "pwd")
                    new CPwd().Help();
                else if (cmd == "quit")
                    new CQuit().Help();
                else if (cmd == "read")
                    new CRead().Help();
                else if (cmd == "rename")
                    new CRename().Help();
                else if (cmd == "reorder")
                    new CReorder().Help();
                else if (cmd == "rotate")
                    new CRotate().Help();
                else if (cmd == "rr")
                    new CRr().Help();
                else if (cmd == "run")
                    new CRun().Help();
                else if (cmd == "rup")
                    new CRup().Help();
                else if (cmd == "set")
                    new CSet().Help();
                else if (cmd == "split")
                    new CSplit().Help();
                else if (cmd == "st")
                    new CSt().Help();
                else if (cmd == "stack")
                    new CStack().Help();
                else if (cmd == "text")
                    new CText().Help();
                else if (cmd == "ulliteral")
                    new CUlliteral().Help();
                else if (cmd == "unalias")
                    new CUnalias().Help();
                else if (cmd == "unfold")
                    new CUnfold().Help();
                else if (cmd == "ungroup")
                    new CUngroup().Help();
                else if (cmd == "unulliteral")
                    new CUnulliteral().Help();
                else if (cmd == "version")
                    new CVersion().Help();
                else if (cmd == "workspace")
                    new CWorkspace().Help();
                else if (cmd == "write")
                    new CWrite().Help();
                else if (repl.Aliases.ContainsKey(cmd))
                {
                    var aliased_cmd = repl.Aliases[cmd];
                    System.Console.WriteLine($"'{cmd}' is aliased to '{aliased_cmd}'");
                    repl.Execute("help " + aliased_cmd);
                }
                else
                {
                    System.Console.WriteLine("Help for unknown command.");
                }
            }
        }
    }
}
