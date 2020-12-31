# Trash

**Trash** is a command-line shell to support the editing, analyzing,
refactoring, and converting from one format to anther, of
CFG parsing tool grammars.
Trash is analogous to
[Bash](https://en.wikipedia.org/wiki/Bash_(Unix_shell))
but for parsing, in the lingua-franca of parsing: parse trees and XPath.
___Trash currently only runs on Windows due to AGL being dependent on
the Windows UI. The CLI tool will be replaced in the next release with
Bash, which will allow all commands except AGL to run on Linux.___

The tool uses [Antlr](https://www.antlr.org/),
[Antlr4BuildTasks](https://github.com/kaby76/Antlr4BuildTasks),
[XPath2](https://en.wikipedia.org/wiki/XPath), 
[S-expressions](https://en.wikipedia.org/wiki/S-expression)
for Antlr parse trees,
and a number of other tools.
The code is implemented in C#.

## What can you do with Trash?

### Generate a parser

	read Expr.g4
	parse
	generate s

### Parse and print out a parse tree, as JSON, XML, or s-expressions

	echo "1+2+3" | run | tree
	echo "1+2+3" | run | json
	echo "1+2+3" | run | xml
	echo "1+2+3" | run | st
	echo "1+2+3" | run | dot

### Print out a parse tree using AGL (on Windows)

	echo "1+2+3" | run | agl

### Find tokens

	echo "1+2+3" | run | xgrep "//INT" | st

### Rename a symbol in a grammar, save

	rename "//parserRuleSpec//labeledAlt//RULE_REF[text() = 'e']" "xxx"
	print
	generate s

### Count method declarations in a Java source file

	read Java9.g4
	parse
	generate compilationUnit
	cat WindowState.java | run > save-tree
	cat save-tree | xgrep "//methodDeclaration" | st | wc

### Strip a grammar of all non-essential CFG

	read Java9.g4
	parse
	strip
	print

### Create a "here" doc.

    read HERE
    grammar A;
    s : e ;
    e : e '*' e | INT ;
    INT : [0-9]+ ;
    WS : [ \t\n]+ -> skip ;
    HERE
    parse
    rename "//parserRuleSpec//labeledAlt//RULE_REF[text() = 'e']" "xxx"
    print
    quit

## Usage

<pre>
trash [-script <em>string</em>]
</pre>

Trash reads stdin or a specified script file commands, one line at a time.
Start a `Cmd` or `Powershell` or `Bash` shell, then

(1) `dotnet ...\Trash.dll`

where `...` is the path to the `Trash.dll`. This command
starts an interactive shell, reading from stdin one line at a time
until `quit` command is executed.

Once in the interactive shell, type `help` to get a list of the
commands.

(2) `cat ex1.tr | dotnet ...\Trash.dll`

where `...` is the path to the `Trash.dll`. This command
reads commands from stdin one line at a time
until `quit` or the end-of-file is read.

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

## Install via Antlrvsix or Antlrvs-vscode

Install the Antlrvsix for VS2019 or Antlrvsix-vscode for VSCode
extension. The Trash program
will be installed in the extensions area for VS2019/VSCode.
Use Find or similar
command to find the Trash.dll.

## Nightly build

# Current release

## 8.3 (30 Dec 2020)

* Added a grammar diff program.
* Added an ISO 14977 parser.
* Added AGL (Automatic Graph Layout), XML, JSON output.
* Added Cat, Echo, Wc, Strip commands.
* Adding BNFC’s LBNF and other associated grammars. NB: This work is not yet complete, and only works for the simplest of grammars.
* [Trash history should be limited; Alias setup should be separated from history. #113](https://github.com/kaby76/AntlrVSIX/issues/113)
* [Include fix of Antlrv4parser.g4 grammar](https://github.com/antlr/grammars-v4/issues/1956)
* [Fix Update to NET 5! #110](https://github.com/kaby76/AntlrVSIX/issues/110)
* [Fix There should be a graph output command in Trash #108](https://github.com/kaby76/AntlrVSIX/issues/108)
* [Fix TreeOutput.OutputTree should output tokens in similar format to parse tree nodes #107](https://github.com/kaby76/AntlrVSIX/issues/107)
* [Fix "Find" should be renamed "XGrep" in Trash #106](https://github.com/kaby76/AntlrVSIX/issues/106)


# Documentation

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
