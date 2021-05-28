# Trash

**Trash** is a collection of command-line tools to support the analysis and editing
of Antlr4 grammars, and the conversion of Antlr2/3, Bison, and EBNF grammars
to Antlr4. The toolkit can generate a parser for an Antlr4 grammar with target source code
in C#, Java, JavaScript, Python3, C++, Go, or Dart.
For the C# target, the toolkit can run the generated parser and display/grep/edit
parse trees.
All tools pass parse tree data through stdin and stdout so they
may be combined to create complex commands.

The tools are implemented as Dotnet Tools, except tragl because it uses WPF on Windows to display a parse tree.
Consequently, the toolkit can be used on Windows, Linux, and Mac.

The tool uses [Antlr](https://www.antlr.org/),
[Antlr4BuildTasks](https://github.com/kaby76/Antlr4BuildTasks),
[XPath2](https://en.wikipedia.org/wiki/XPath), 
[S-expressions](https://en.wikipedia.org/wiki/S-expression)
for Antlr parse trees,
and a number of other tools.
The code is implemented in C#.

## Example usages of Trash

### Examine an Antlr grammar

    trparse -f ada.g4 | trtree | vim -

This command, executed from Bash, parses the Antlr4 grammar
[ada.g4](https://github.com/kaby76/Domemtech.Trash/blob/main/_test/convert/ada.g4)
using [trparse](https://github.com/kaby76/Domemtech.Trash/tree/main/trparse),
prints out the parse tree data as a simple
[text-oriented diagram](https://github.com/kaby76/Domemtech.Trash/blob/main/_test/convert/ada.g4.tree)
using [trtree](https://github.com/kaby76/Domemtech.Trash/tree/main/trtree),
then opens [vim](https://www.vim.org/) on the diagram. If you are not
familiar with `Vim`, then you can save the output from `trtree` to a file
and open that.

### Convert grammars to Antlr4

    trparse -f OldGrammar.g2 -t antlr2 | trconvert | trprint > OldGrammar.g4

This command parses an old Antlr2 grammar
using [trparse](https://github.com/kaby76/Domemtech.Trash/tree/main/trparse),
then converts the parse tree data to Antlr4 syntax and
finally prints out the converted parse tree data
using [trconvert](https://github.com/kaby76/Domemtech.Trash/tree/main/trconvert).

### Generate a parser

    trgen

This command creates a parser application for the C# target.
If executed in an empty directory, [trgen](https://github.com/kaby76/Domemtech.Trash/tree/main/trgen)
creates an application using the Arithmetic grammar.
If executed in a directory containing
a Antlr Maven plugin (`pom.xml`), `trgen` will create a program according
to the information specified in the `pom.xml` file.

`trgen` has many options to generate a parser from any Antlr4 grammar, for any target.
But, if a parser is generated for the C# target, built using the NET SDK, then `trparse`
can execute the generated parser, and can be used with all the other tools in Trash.

### Run the generated parser and print out a parse tree, as JSON, XML, or s-expressions

	$ trparse -i "1+2+3" | trtree
	$ trparse -i "1+2+3" | trjson
	$ trparse -i "1+2+3" | trxml
	$ trparse -i "1+2+3" | trst
	$ trparse -i "1+2+3" | trdot

After using `trgen` to generate a parser program in C#, and after building
the program, you can run the parser using `trparse`, which takes arguments
for input strings, files, or reading stdin. The output of `trparse`, like
almost all programs in Trash, is parse tree data, which you can then
print out in a number of different formats.

### Find nodes in the parse tree using XPath

	$ trparse -i "1+2+3" | trxgrep "//INT" | trst

Using `trparse`, you can create a parse tree that can be searched
using `trxgrep`. The tool `trxgrep` uses XPath expressions to identify
exactly what node(s) in the tree you want. Those sub-trees can be
printed using any of the tools shown previously.

### Rename a symbol in a grammar, generate a parser for new grammar

_Not yet ported from Antlrvsix._

	cat previous-parse.data | trrename "//parserRuleSpec//labeledAlt//RULE_REF[text() = 'e']" "xxx" | trtext > new-source.g4

### Count method declarations in a Java source file

	$ trgen --file Java9.g4 --start-rule compilationUnit
	$ cd Generated/; dotnet build; cd ..
	$ trparse --file WindowsState.java | trxgrep "//methodDeclaration" | trst | wc

To count the number of methods in a Java source file, first generate a
parser, build it, and then run `trparse` to create a parse tree for the
input. Then, use `trxgrep` to pick off the methods, convert the nodes
found into a one-per-line tree, and use `wc` to count the number.

### Strip a grammar of all non-essential CFG

	$ trparse --file Java9.g4 | trstrip | trtext > new-grammar.g4

### Diff grammars

_Not yet ported from Antlrvsix._

	# From a Bash shell
	$ alias trash='dotnet ...\\trash.dll'
	$ echo version | trash

	$ cat << HERE1 | dotnet ...\\trash.dll > v1.temp
	read v1.g4
	parse
	strip
	print
	HERE1

	$ cat << HERE2 | dotnet ...\\trash.dll > v2.temp
	read v2.g4
	parse
	strip
	print
	HERE2

	$ diff v1.temp v2.temp

*There is a built-in diff for grammars, but it is not
practical except for small grammars in this release.*

## Usage

Each command in Trash has a set of options and required arguments.
The list of currently available commands is:

	tranalyze
	trconvert
	trfold
	trfoldlit
	trgen
	trgroup
	trjson
	trkleene
	trparse
	trprint
	trrename
	trst
	trstrip
	trtext
	trtokens
	trtree
	trunfold
	trungroup
	trwdog
	trxgrep
	trxml
	trxml2


## Parsing Result Sets -- the data passed between commands

A *result set* is a JSON serialization of:

* A set of parse tree nodes.
* Parser information related to the parse tree nodes.
* Lexer information related to the parse tree nodes.
* The name of the input corresponding to the parse tree nodes.
* The input text corresponding to the parse tree nodes.

Most commands in Trash, e.g., "." or "xgrep", read and/or write result sets and
perform an operation on the result set. Other commands in Trash,
e.g., "wc", "cat", or "echo", are standard character-orient data
passed. At the end of executing the command, either is just printed
to stdout.

## Commands of Trash

See [this list](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/commands.md) of commands available in Trash.

## Supported grammars

| Grammars | File suffix |
| ---- | ---- |
| Antlr4 | .g4 |
| Antlr3 | .g3 |
| Antlr2 | .g2 |
| Bison | .y |
| LBNF | .cf |
| W3C EBNF | .ebnf |
| ISO 14977 | .iso14977, .iso |

# Installation

## Install 

Just execute the following lines:

	dotnet tool install -g trfold
	dotnet tool install -g trfoldlit
	dotnet tool install -g trgen
	dotnet tool install -g trgroup
	dotnet tool install -g trjson
	dotnet tool install -g trkleene
	dotnet tool install -g trparse
	dotnet tool install -g trprint
	dotnet tool install -g trst
	dotnet tool install -g trstrip
	dotnet tool install -g trtext
	dotnet tool install -g trtokens
	dotnet tool install -g trtree
	dotnet tool install -g trunfold
	dotnet tool install -g trungroup
	dotnet tool install -g trxgrep
	dotnet tool install -g trxml
	dotnet tool install -g trxml2


(Or you can download the [install.sh](https://github.com/kaby76/Domemtech.Trash/blob/main/install.sh)
file and execute it in a shell.

# Current release

## 0.8.0 (27 May 2021)

* Preliminary release of the toolset.

# Documentation

_NB: The following documentation is obsolete, carried over from the
old Antlrvsix Trash shell._

Please refer to
the [documentation](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/commands.md).

## Analysis

### Recursion

* [Has direct/indirect recursion](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/analysis.md#has-directindirect-recursion)

## Refactoring

Antlrvsix provides a number of transformations that can help to make grammars cleaner (reformatting),
more readable (reducing the length of the RHS of a rule),
and more efficient (reducing the number of non-terminals) for Antlr.

Some of these refactorings are very specific for Antlr due to the way
the parser works, e.g., converting a prioritized chain of productions recognizing
an arithmetic expression to a recursive alternate form.
The refactorings implemented are:

### Raw tree editing

* [Delete parse tree node](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#delete-parse-tree-node)

### Reordering

* [Move start rule to top](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#move-start-rule)
* [Reorder parser rules](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#reorder-parser-rules)
* [Sort modes](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#sort-modes)

### Changing rules

* [Remove useless parentheses](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#remove-useless-parentheses)
* [Remove useless parser rules](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#remove-useless-productions)
* [Rename lexer or parser symbol](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#rename)
* [Unfold](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#Unfold)
* [Group alts](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#group-alts)
* [Ungroup alts](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#ungroup-alts)
* [Upper and lower case string literals](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#upper-and-lower-case-string-literals)
* [Fold](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#Fold)
* Replace direct left recursion with right recursion
* [Replace direct left/right recursion with Kleene operator](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#Kleene)
* Replace indirect left recursion with right recursion
* Replace parser rule symbols that conflict with Antlr keywords
* [Replace string literals in parser with lexer symbols](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#replace-literals-in-parser-with-lexer-token-symbols)
* Replace string literals in parser with lexer symbols, with lexer rule create
* [Delabel removes the annoying and mostly useless labeling in an Antlr grammar](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#delabel)

### Splitting and combining

* [Split combined grammars](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#splitting-and-combining-grammars)
* [Combine splitted grammars](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/refactoring.md#splitting-and-combining-grammars)

## Conversion

* [Antlr3 import](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/Import.md#antlr3)
* [Antlr2 import](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/Import.md#antlr2)
* [Bison import](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/doc/Import.md#bison)

---------

The source code for the extension is open source, free of charge, and free of ads. For the latest developments on the extension,
check out my [blog](http://codinggorilla.com).

# Building

1) git clone https://github.com/kaby76/AntlrVSIX
2) cd AntlrVSIX
3) # From a `Developer Command Prompt for VS2019`
4) msbuild /t:restore
5) msbuild

Trash.dll is at ./Trash/bin/Debug/net5-windows
after building successfully. You must have Net5 SDK installed
to build and run.

# Prior Releases

(Incomplete.)

# Roadmap

## Planned for v9

* Place Trash in it's own repo, independent of Antlrvsix.
* Replace Trash shell and commands with Bash and independent programs.


Any questions, email me at ken.domino <at> gmail.com
