# Trash

**Trash** is a tool to support editing, analysis, refactoring,
and conversion of CFG grammars, including Antlr2, Antlr3, Antlr4,
Bison, W3C EBNF, and ISO 14977.
Trash is analogous to
[Bash](https://en.wikipedia.org/wiki/Bash_(Unix_shell))
but for parsing, in the lingua-franca of parsing: parse trees and XPath.

The tool uses [Antlr](https://antlr.org),
[Antlr4BuildTasks](https://github.com/kaby76/Antlr4BuildTasks),
[XPath2](https://en.wikipedia.org/wiki/XPath), 
[S-expressions](https://en.wikipedia.org/wiki/S-expression)
for Antlr parse trees,
and a number of other tools.
The code is implemented in C#.

## Supported grammars

| Grammars | File suffix |
| ---- | ---- | ---- |
| Antlr4 | .g4 |
| Antlr3 | .g3 |
| Antlr2 | .g2 |
| Bison | .y |
| W3C EBNF | .ebnf |
| ISO 14977 | .iso14977 |

# Installation

## (Incomplete)

## Nightly build

## Current release (Incomplete)

# Documentation

## Running the **Trash** command-line engine

The **Trash** tool is a command-line interpreter for executing
operations on grammar files. With it, you can read Antlr2, Antlr3,
Antrl4, and Bison grammars, convert to Antlr4, find sub-trees
in the parse tree for the grammar using XPath expressions, analyze,
and refactor the grammar.

To run the tool, you will need to find the **Trash.exe** executable
after installing the extension in Visual Studio 2019, or unpacking
it directly from the downloaded .vsix--a ZIP file.
It should be under
"C:/Users/userid/AppData/Local/Microsoft/VisualStudio/.../AntlrVSIX/8.0/Trash/netcoreapp3.1/Trash.exe".
Once you find the executable, please refer to
the [documentation](https://github.com/kaby76/AntlrVSIX/blob/master/doc/Trash.md).

## Analysis

### Recursion

* [Has direct/indirect recursion](https://github.com/kaby76/AntlrVSIX/blob/master/doc/analysis.md#has-directindirect-recursion)

## Refactoring

Antlrvsix provides a number of transformations that can help to make grammars cleaner (reformatting),
more readable (reducing the length of the RHS of a rule),
and more efficient (reducing the number of non-terminals) for Antlr.

Some of these refactorings are very specific for Antlr due to the way
the parser works, e.g., converting a prioritized chain of productions recognizing
an arithmetic expression to a recursive alternate form.
The refactorings implemented are:

### Raw tree editing

* [Delete parse tree node](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#delete-parse-tree-node)

### Reordering

* [Move start rule to top](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#move-start-rule)
* [Reorder parser rules](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#reorder-parser-rules)
* [Sort modes](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#sort-modes)

### Changing rules

* [Remove useless parentheses](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#remove-useless-parentheses)
* [Remove useless parser rules](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#remove-useless-productions)
* [Rename lexer or parser symbol](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#rename)
* [Unfold](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#Unfold)
* [Group alts](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#group-alts)
* [Ungroup alts](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#ungroup-alts)
* [Upper and lower case string literals](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#upper-and-lower-case-string-literals)
* [Fold](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#Fold)
* Replace direct left recursion with right recursion
* [Replace direct left/right recursion with Kleene operator](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#Kleene)
* Replace indirect left recursion with right recursion
* Replace parser rule symbols that conflict with Antlr keywords
* [Replace string literals in parser with lexer symbols](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#replace-literals-in-parser-with-lexer-token-symbols)
* Replace string literals in parser with lexer symbols, with lexer rule create
* [Delabel removes the annoying and mostly useless labeling in an Antlr grammar](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#delabel)

### Splitting and combining

* [Split combined grammars](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#splitting-and-combining-grammars)
* [Combine splitted grammars](https://github.com/kaby76/AntlrVSIX/blob/master/doc/refactoring.md#splitting-and-combining-grammars)

## Conversion

* [Antlr3 import](https://github.com/kaby76/AntlrVSIX/blob/master/doc/Import.md#antlr3)
* [Antlr2 import](https://github.com/kaby76/AntlrVSIX/blob/master/doc/Import.md#antlr2)
* [Bison import](https://github.com/kaby76/AntlrVSIX/blob/master/doc/Import.md#bison)

---------

The source code for the extension is open source, free of charge, and free of ads. For the latest developments on the extension,
check out my [blog](http://codinggorilla.com).

# Building (Incomplete)

---------

# Prior Releases

# Roadmap

## Planned for vx.x 

## Alternatives (Incomplete)

Any questions, email me at ken.domino <at> gmail.com
