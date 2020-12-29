# Trash, a shell for transforming grammars

## Commands

### Agl

<pre>
agl
</pre>

Read stdin and display the graph using an
Automatic Graph Layout library window.

Example:

    . | agl

### Alias

Aliases allow a string to be substituted for a word when it is used as the
first word of a simple command. The shell maintains a list of aliases that
may be set and unset with the alias and unalias builtin commands.

The first word of each simple command is checked to see if it has an alias.
If so, that word is replaced by the text of the alias.
Only [a-zA-Z_] appear in an alias name. The replacement text may contain
any valid shell input, including shell metacharacters. The first word of
 the replacement text is tested for aliases, but a word that is identical
 to an alias being expanded is not expanded a second time. This means that
 one may alias ls to "ls -F", for instance, and Bash does not try to recursively
 expand the replacement text. If the last character of the alias value is a
 blank, then the next command word following the alias is also checked
 for alias expansion.

Aliases are created and listed with the alias command, and removed with the
 unalias command.

<pre>
alias <em>id</em> = <em>string</em>
</pre>

Set up an alias that assigns <em>string</em> to <em>id</em>. The command <em>string</em> 
is executed with <em>id</em>.

Example:

    alias h=history
    h

### Analyze

_Trash_ can perform an analysis of a grammar. The analysis includes a count of symbol
type, cycles, and unused symbols.

<pre>
analyze
</pre>

Example:

    analyze

### History expansion

_Trash_ keeps a persistent history of prior commands in ~/.trash.rc that is read
when the program starts. 
History expansions introduce words from the history list into the input stream,
making it easy to repeat commands. Currently there is no editing
capability.

<pre>
!!
</pre>

Execute the previous command.

<pre>
!<em>int</em>
</pre>

Execute the command line <em>int</em>.

<pre>
!<em>id</em>
</pre>

Execute the command that begins with <em>string</em>.

### Cat

<pre>
cat
</pre>

"Cat" the contents of a file to stdout.

Example:

    cat input.txt | run | tree

### Cd

<pre>
cd <em>string</em>?
</pre>

Change current directory. If string is not given, change to the user's home directory.
`cd` accepts wildcards.

Example:

    cd
    cd *foo

### Combine

<pre>
combine
</pre>

Combine two grammars on top of stack into one grammar.
One grammar must be a lexer grammar, the other a parser grammar,
order is irrelevant.

Example:

    (top of stack contains a lexer file and a parser file, both parsed.)
    combine

### Convert

<pre>
convert (antlr2 | antlr3 | antlr4 | bison | ebnf)?
</pre>

Convert the parsed grammar file at the top of stack into Antlr4 syntax. If the type of
grammar cannot be inferred from the file suffix, a type can be supplied with the command.
The resulting Antlr4 grammar replaces the top of stack.

Example:

    (top of stack contains a grammar file that is Antlr2 or 3, Bison, or EBNF syntax.)
    combine antlr3

### "."

<pre>
.
</pre>

Print out the parse tree for the file at the top of stack.

Example:

    .

### Delabel

<pre>
delabel
</pre>

Remove all labels from an Antlr4 grammar that is on the top of stack, e.g.,
"expr : lhs=expr (PLUS | MINUS) rhs=expr # foobar1 ....." => "expr : expr (PLUS | MINUS) expr ....."

Example:

    delabel

### Delete

<pre>
delete <em>string</em>
</pre>

Delete nodes in the parsed file at the top of stack specified by the XPath expression string.

Example:

    delete "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

For further details, see the [Delete parse tree node](refactoring.md#delete-parse-tree-node) refactoring details.

### Dot

<pre>
dot
</pre>

Read the result set from a command and convert it into a Dot representation.
You can then [visualize the graph online](https://dreampuf.github.io/GraphvizOnline/#digraph%20G%20%7B%0A%0A%20%20subgraph%20cluster_0%20%7B%0A%20%20%20%20style%3Dfilled%3B%0A%20%20%20%20color%3Dlightgrey%3B%0A%20%20%20%20node%20%5Bstyle%3Dfilled%2Ccolor%3Dwhite%5D%3B%0A%20%20%20%20a0%20-%3E%20a1%20-%3E%20a2%20-%3E%20a3%3B%0A%20%20%20%20label%20%3D%20%22process%20%231%22%3B%0A%20%20%7D%0A%0A%20%20subgraph%20cluster_1%20%7B%0A%20%20%20%20node%20%5Bstyle%3Dfilled%5D%3B%0A%20%20%20%20b0%20-%3E%20b1%20-%3E%20b2%20-%3E%20b3%3B%0A%20%20%20%20label%20%3D%20%22process%20%232%22%3B%0A%20%20%20%20color%3Dblue%0A%20%20%7D%0A%20%20start%20-%3E%20a0%3B%0A%20%20start%20-%3E%20b0%3B%0A%20%20a1%20-%3E%20b3%3B%0A%20%20b2%20-%3E%20a3%3B%0A%20%20a3%20-%3E%20a0%3B%0A%20%20a3%20-%3E%20end%3B%0A%20%20b3%20-%3E%20end%3B%0A%0A%20%20start%20%5Bshape%3DMdiamond%5D%3B%0A%20%20end%20%5Bshape%3DMsquare%5D%3B%0A%7D)
(or [here](http://graphviz.it/#/gallery/unix.gv)) or with Graphviz](https://graphviz.org/).

### Echo

<pre>
echo <em>string</em>
</pre>

Echo a string literal to stdout (e.g., to be used with `run`).

### Fold

<pre>
fold <em>string</em>
</pre>

Replace a sequence of symbols on the RHS of a rule
with the rule LHS symbol.

For further details, see the [fold](refactoring.md#Fold) refactoring details.

### Foldlit

<pre>
fold <em>string</em>
</pre>

Replace a literal on the RHS of a rule
with the lexer rule LHS symbol.

For further details, see the [fold literal](refactoring.md#replace-literals-in-parser-with-lexer-token-symbols) refactoring details.

### Generate

<pre>
generate <em>string</em>
</pre>

Generate a C# parser using the stack files. Make sure to `read` all
dependent source code if your grammar reference classes in different
source files. The command will then create a `generated/` directory
containing the code and compile it with NET Core `dotnet`. Afterward,
use `run` to run the parser.

### Group

<pre>
group <em>string</em>
</pre>

Perform a recursive left- and right- factorization of alternatives for rules.
The nodes specified must be for `ruleAltList`, `lexerAltList`, or `altList`.
A common prefix and suffix is performed on the alternatives, and
a new expression derived. The process repeats for alternatives nested. 

For further details, see the [Group alts](refactoring.md#group-alts) refactoring details.

### Has

<pre>
has (dr | ir) (left | right) <em>string</em>
</pre>

Print out whether the rule specified by the xpath expression pointing to the LHS symbol
of a parser or lexer rule has left or right recursion.

For further details, see the [analysis](analysis.md#has-directindirect-recursion) section.

### Help

<pre>
help
</pre>

Print out a list of commands.

### History

<pre>
history
</pre>

Print out the shell command history.

### Json

<pre>
json
</pre>

Read from stdin the result set and print out it as JSON.

### Kleene

<pre>
kleene <em>string</em>
</pre>

Replace a rule, whose symbol is identified by the xpath string,
of the grammar at the top of the grammar with an EBNF
form if it contains direct left or direct right recursion.

### Ls

<pre>
ls <em>string</em>
</pre>

List directory contents. If string is not given, list the current directory contents.
`ls` accepts wildcards.

### Mvsr

<pre>
mvsr <em>string</em>
</pre>

Move the rule, whose symbol is identified by the xpath string,
to the top of the grammar.

### Parse

<pre>
parse (antlr2 | antlr3 | antlr4 | bison | ebnf)?
</pre>

Parse the flie at the top of stack with the given parser type (_antlr2_, _antlr3, _antlr4_, _bison_, etc).

### Pop

<pre>
pop
</pre>

Pop the top document from the stack. If the stack is empty, nothing is further popped.
There is no check as to whether the document has been written to disk. If you want to write
the file, use `write`.

### Print

<pre>
print
</pre>

Print out text file at the top of stack.

### Pwd

<pre>
pwd
</pre>

Print out the current working directory.

### Quit

<pre>
(quit | exit)
</pre>

Exit the shell program.

### Read

<pre>
read <em>string</em>
</pre>

Read the text file _file-name_ and place it on the top of the stack.
`read` accepts wildcards.

### Rename

<pre>
rename <em>string</em> <em>string</em>
</pre>

Rename a symbol, the first parameter as specified by the xpath expression string,
to a new name, the second parameter as a string. The result may
place all changed grammars that use the symbol on the stack.

For further details, see the [Rename](refactoring.md#rename) refactoring details.

### Reorder

<pre>
reorder alpha
reorder bfs <em>string</em>
reorder dfs <em>string</em>
</pre>

Reorder the parser rules according to the specified type. For
BFS and DFS, an XPath expression must be supplied to specify
all the start rule symbols. For alphabetic reordering, all parser
rules are retained, and simply reordered alphabetically. 
For BFS and DFS, if the rule is unreachable from a start node,
then the rule is dropped from the grammar.

### Rotate

<pre>
rotate
</pre>

Rotate the stack once.

### RR

<pre>
rr
</pre>

Replace left indirect or direct recursion with right recursion.

### Run

<pre>
run arg1 (arg2 arg3? )?
</pre>

Generate a parser using the Antlr tool on 
the grammar specified by the current workspace
run the generated parser, output a tree or 
find elements in the tree.

### Rup

<pre>
rup <em>string</em>?
</pre>

Find all altLists as specified by the xpath expression in the parsed file at the top of stack.
If the xpath expression is not given, the transform is applied to the whole file.
Rewrite the node with the parentheses removed, if the altList satifies three constraints:
(1) the expression must be a `altList` type in the Antlr4 grammar;
(2) the `altList` node doesn't contain more than one child, or if it does, then the containing altList/labeledAlt/alterative each does not contain more than one child;
(3) the `ebnf` parent of `block` must not contain a `blockSuffix`.

For further details, see the [remove useless parentheses](refactoring.md#remove-useless-parentheses) refactoring details.

### Split

<pre>
split
</pre>

The `split` command attempts to split a grammar at the top of the stack.
The grammar must be a combined lexer/parser grammar for the transformation to
proceed. The transformation creates a lexer grammar and a parser grammar and places them
on the stack. The original grammar is popped off the stack.

For further details, see the [split grammar](refactoring.md#splitting-and-combining-grammars) refactoring details.

### Stack

<pre>
stack
</pre>

Print the stack of files.

### Strip

<pre>
strip
</pre>

Strip the top-of-stack grammar completely of all non-essential CFG
rules, including labels, comments, actions, and format one rule per line.

### Text

<pre>
text
</pre>

Read stdin the result set and output the corresponding text for it.

### Tokens

<pre>
tokens
</pre>

Read stdin the result set and output the corresponding tokens for it.

### ULLiteral

<pre>
ulliteral <em>string</em>?
</pre>

The ulliteral command applies the "upper- and lower-case string literal"
transform to a collection of terminal nodes in the parse tree,
which is identified with the supplied xpath expression.
If the xpath expression is not given, the transform is applied to the whole
file.
Prior to using this command,
the document must have been parsed.
The ulliteral operation substitutes a sequence of 
sets containing an upper and lower case characters
for a `STRING_LITERAL`.
The expression must point to the right-hand side `STRING_LITERAL` of
a parser or lexer rule.
The resulting code is parsed and placed
on the top of stack.

### Unalias

<pre>
unalias <em>id</em>
</pre>

Remove an aliased command.

### Unfold

<pre>
unfold <em>string</em>
</pre>

The unfold command applies the unfold transform to a collection of terminal nodes in the parse tree,
which is identified with the supplied xpath expression. Prior to using this command, you must have the file parsed.
An unfold operation substitutes the right-hand side of a parser or lexer rule
into a reference of the rule name that occurs at the specified node.
The resulting code is parsed and placed
on the top of stack.

### Ungroup

<pre>
ungroup <em>string</em>
</pre>

Perform an ungroup transformation of the `element` node(s) specified
by the string.

For further details, see the [Ungroup alts](refactoring.md#ungroup-alts) refactoring details.


### Version

<pre>
version
</pre>

Output to stdout the current version of Trash.

### Wc

<pre>
wc
</pre>

Read stdin and output the count of the number of lines.

### Workspace

<pre>
workspace
</pre>

Create a new workspace.

### Write

<pre>
write
</pre>

Pop the stack, and write out the file specified.

### XGrep

<pre>
xgrep <em>string</em>
</pre>

Find all sub-trees in the parsed file at the top of stack using the given XPath expression string.

Example:

    . | xgrep "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

### Xml

<pre>
xml
</pre>

Read from stdin the result set and print out it as XML.

