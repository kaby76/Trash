using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;

namespace Trash.Commands
{
    public class CHelp
    {
        public void Help()
        {
        }

        public void Execute(Repl repl, ReplParser.HelpContext tree)
        {
            if (tree.id_keyword() == null)
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
                var id = tree.id_keyword();
                if (id.GetText() == "alias")
                    new CAlias().Help();
            }
        }
    }
}
