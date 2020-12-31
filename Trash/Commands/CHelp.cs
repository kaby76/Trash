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
agl - Read a tree from stdin and draw a Windows Form graph.
alias - Allow a string to be substituted for a word of a simple command.
analyze - Perform an analysis of a grammar.
!! (""bang bang"") - Repeat the previous command.
!int - Repeat a previous command.
cat - Type the contents of a file to stdout.
cd - Change directory.
combine - Create a combined grammar from the lexer and parser grammars on the top of stack.
convert - Convert the parsed grammar file at the top of stack into Antlr4 syntax.
delabel - Remove all labels from an Antlr4 grammar.
delete - Delete nodes in the parse tree for a grammar and reform the grammar.
dot - Print a Dot graph.
echo - Echo a string to stdout.
fold - Replace a sequence of symbols on the RHS of a rule with a LHS symbol of another rule.
foldlit - Replace a literal on the RHS of a rule with a LHS symbol of another rule.
generate - Generate a parser and compile it.
group - Group common sub-sequences of symbols for alts.
has - Analyze a grammar for left/right/direct/indirect recursion.
help - Give help for commands.
history - Show the history of commands executed.
json - Read a tree from stdin and produce a json representation.
kleene - Convert a rule in BNF to EBNF.
ls - Show contents of a directory.
mvsr - Move a rule to the top of the grammar.
parse - Parse a grammar.
period (aka '.') - Print out the parse tree for the file at the top of stack.
pop - Pop the stack of files.
print - Print the file at the top of stack.
pwd - Print out the current working directory.
quit - Exit Trash.
read - Read a file and place it on the stack.
rename - Rename a symbol in the grammar.
reorder - Reorder the rules of a grammar.
rotate - Rotate the stack.
rr - Replace left recursion with right recursion.
run - Run a parser and output a parse tree.
rup - Remove useless parentheses in a grammar rule.
set - Set a value.
split - Split a combined grammar.
st - Print the tree using 'toStringTree()'.
stack - Print the stack.
strip - Remove all comments, labels, etc., in order to derive a plain CFG.
text - Print the tree source.
tokens - Print the tokens associated with a parse tree.
ulliteral - Convert a lexer rule for a simple string literal to accept a string in any case.
unalias - Unalias a command.
unfold - Unfold a grammar rule symbol.
ungroup - Ungroup a parenthesized alt.
unulliteral - Apply inverse transform of ulliteral.
version - Print out the version of Trash.
wc - Read stdin and output the number of lines.
workspace - Create a new workspace for the run command.
write - Write a file to disk.
xgrep - Find nodes in the parse tree for a grammar.
xml - Read a tree from stdin and convert it to xml.
");
            }
            else
            {
                var id = tree.NonWsArgMode();
                var cmd = id.GetText();
                if (cmd == "agl")
                    new CAgl().Help();
                else if (cmd == "alias")
                    new CAlias().Help();
                else if (cmd == "analyze")
                    new CAnalyze().Help();
                else if (cmd == "bang")
                    new CBang().Help();
                else if (cmd == "cat")
                    new CCat().Help();
                else if (cmd == "cd")
                    new CCd().Help();
                else if (cmd == "combine")
                    new CCombine().Help();
                else if (cmd == "convert")
                    new CConvert().Help();
                else if (cmd == "delabel")
                    new CDelabel().Help();
                else if (cmd == "delete")
                    new CDelete().Help();
                else if (cmd == "dot")
                    new CDot().Help();
                else if (cmd == "echo")
                    new CEcho().Help();
                else if (cmd == "fold")
                    new CFold().Help();
                else if (cmd == "foldlit")
                    new CFoldlit().Help();
                else if (cmd == "generate")
                    new CGenerate().Help();
                else if (cmd == "group")
                    new CGroup().Help();
                else if (cmd == "has")
                    new CHas().Help();
                else if (cmd == "help")
                    new CHelp().Help();
                else if (cmd == "history")
                    new CHistory().Help();
                else if (cmd == "json")
                    new CJson().Help();
                else if (cmd == "kleene")
                    new CKleene().Help();
                else if (cmd == "ls")
                    new CLs().Help();
                else if (cmd == "mvsr")
                    new CMvsr().Help();
                else if (cmd == "parse")
                    new CParse().Help();
                else if (cmd == "period")
                    new CPeriod().Help();
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
                else if (cmd == "xgrep")
                    new CXGrep().Help();
                else if (cmd == "xml")
                    new CXml().Help();
                else if (repl.Aliases.ContainsKey(cmd))
                {
                    var aliased_cmd = repl.Aliases[cmd];
                    System.Console.WriteLine($"'{cmd}' is aliased to '{aliased_cmd}'");
                    repl.Execute("help " + aliased_cmd);
                }
                else
                {
                    System.Console.WriteLine("Help unavailable -- unknown command '" + cmd + "'.");
                }
            }
        }
    }
}
