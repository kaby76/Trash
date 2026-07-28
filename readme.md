# Trash

[![Build](https://github.com/kaby76/Trash/workflows/CI/badge.svg)](https://github.com/kaby76/Trash/actions?query=workflow%3ACI)

Trash is a collection of command-line tools to analyze and transform
Antlr4 grammars and parse trees. The toolkit can: generate a parser
application for an Antlr4 grammar for any target and any OS; analyze the
grammar for common problems; automate changes applied to a grammar scraped
from a specification; transform parse trees for transpiling
and preprocessing source code. With the [collection of Antlr grammars](https://github.com/antlr/grammars-v4),
one can write applications that parse popular programming languages quickly and easily.

Unlike tools that center around Antlr4 parsing, visitors and listeners, this toolkit does not support visitor and
listener applications because it is too primitive and target-language dependent. Instead, the toolkit operates around
XQuery scripts that operate directly on parse trees. Also, Antlr4 parse trees do not contain attributes line line/column information,
intertoken content (e.g., comments), and so on. Trash parse trees contain a multitude of information.

In addition, instead of Antlr and all other parser generators, this toolkit works around
composable commands with Bash, Powershell, Python, Lua, etc. as the glue. Complex refactorings can be
achieved by chaining different commands together.

Each app in `Trash` is implemented as a sub-packaged [Dotnet Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) console application, and can be used on Windows, Linux, or Mac.
No prerequisites are required other than installing the
[NET SDK](https://dotnet.microsoft.com/), and the toolchains
for any other targets you want to use. All commands are executed through the top-level Dotnet application
"trash", e.g., "dotnet trash parse --help".

The toolkit uses [Antlr](https://www.antlr.org/) and
[XPath2](https://en.wikipedia.org/wiki/XPath).
The code is implemented in C#.

An application of the toolkit was used to scrape and refactor the Dart2
grammar from spec. See [this script](https://github.com/kaby76/ScrapeDartSpec/blob/master/refactor.sh).

## Installation
### Requirements
[Install Dotnet 10.0.x](https://dotnet.microsoft.com/en-us/download)

### Install Globally

    dotnet tool install -g trash

### Uninstall

    dotnet tool uninstall -g trash

### Install Locally

    dotnet new tool-manifest
    dotnet tool install trash

## List of commands
1) <a href="src/tranalyze/readme.md">dotnet trash analyze</a> -- Analyze a grammar
1) <a href="src/trcaret/readme.md">dotnet trash caret</a> -- Caret operations on a parse tree
1) <a href="src/trclonereplace/readme.md">dotnet trash clonereplace</a> -- Clone and replace in a grammar
1) <a href="src/trcombine/readme.md">dotnet trash combine</a> -- Combine a split Antlr4 grammar
1) <a href="src/trconvert/readme.md">dotnet trash convert</a> -- Convert a grammar from one form to another
1) <a href="src/trcover/readme.md">dotnet trash cover</a> -- Code coverage analysis
1) <a href="src/trdot/readme.md">dotnet trash dot</a> -- Print a parse tree in Graphviz Dot format
1) <a href="src/trextract/readme.md">dotnet trash extract</a> -- Extract from a parse tree
1) <a href="src/trff/readme.md">dotnet trash ff</a> -- Outputs FIRST and FOLLOW sets of a grammar
1) <a href="src/trfoldlit/readme.md">dotnet trash foldlit</a> -- Perform fold transform on grammar with literals
1) <a href="src/trgen/readme.md">dotnet trash gen</a> -- Generate an Antlr4 parser for a given target language
1) <a href="src/trgenvsc/readme.md">dotnet trash genvsc</a> -- Generate VS Code extension files
1) <a href="src/trglob/readme.md">dotnet trash glob</a> -- Glob file patterns
1) <a href="src/triconv/readme.md">dotnet trash iconv</a> -- Convert file encoding
1) <a href="src/tritext/readme.md">dotnet trash itext</a> -- Get strings from a PDF file
1) <a href="src/trjson/readme.md">dotnet trash json</a> -- Print a parse tree in JSON structured format
1) <a href="src/trnullable/readme.md">dotnet trash nullable</a> -- Nullable analysis of a grammar
1) <a href="src/trparse/readme.md">dotnet trash parse</a> -- Parse a grammar or use a generated parser to parse input
1) <a href="src/trperf/readme.md">dotnet trash perf</a> -- Perform performance analysis of an Antlr grammar parse
1) <a href="src/trquery/readme.md">dotnet trash query</a> -- Query parse trees using XPath
1) <a href="src/trrename/readme.md">dotnet trash rename</a> -- Rename symbols in a grammar
1) <a href="src/trsort/readme.md">dotnet trash sort</a> -- Sort rules in a grammar
1) <a href="src/trsplit/readme.md">dotnet trash split</a> -- Split a combined Antlr4 grammar
1) <a href="src/trsponge/readme.md">dotnet trash sponge</a> -- Extract parsing results of a Trash command into files
1) <a href="src/trtext/readme.md">dotnet trash text</a> -- Print a parse tree with a specific interval
1) <a href="src/trtokens/readme.md">dotnet trash tokens</a> -- Print tokens in a parse tree
1) <a href="src/trtree/readme.md">dotnet trash tree</a> -- Print a parse tree in a human-readable format
1) <a href="src/trunfold/readme.md">dotnet trash unfold</a> -- Perform an unfold transform on a grammar
1) <a href="src/trunfoldlit/readme.md">dotnet trash unfoldlit</a> -- Perform unfold transform with literals on a grammar
1) <a href="src/trungroup/readme.md">dotnet trash ungroup</a> -- Perform an ungroup transform on a grammar
1) <a href="src/trwdog/readme.md">dotnet trash wdog</a> -- Kill a program that runs too long
1) <a href="src/trxgrep/readme.md">dotnet trash xgrep</a> -- Search using XPath in parse trees
1) <a href="src/trxml/readme.md">dotnet trash xml</a> -- Print a parse tree in XML structured format
1) <a href="src/trxml2/readme.md">dotnet trash xml2</a> -- Print an enumeration of all paths in a parse tree to leaves

## Examples

### Parse a grammar, create a parser for the grammar, build, and test
```
git clone https://github.com/antlr/grammars-v4
cd grammars-v4/python/python
dotnet trash parse *.g4 | dotnet trash query 'grep //grammarDecl' | dotnet trash text
# Output:
# PythonLexer.g4:lexer grammar PythonLexer;
# PythonParser.g4:parser grammar PythonParser;
dotnet trash gen
cd Generated
dotnet build
cat - <<EOF | dotnet trash parse | dotnet trash query 'grep //test' | dotnet trash text
x == y
x == y if z == b else a == u
lambda: a
lambda x, y: a
EOF
# Output:
# a
# lambda x, y: a
# a
# lambda: a
# a == u
# x == y if z == b else a == u
# x == y
```
### Display parse tree
```
dotnet trash parse -i "a == b" | dotnet trash tree
```
`trtree` is only one of several ways to view parse tree data.
Other programs for different output are
[trjson](https://github.com/kaby76/Trash/tree/main/src/trjson) for [JSON output](https://github.com/kaby76/Trash/blob/main/_tests/trconvert/antlr2/ada.g4.json),
[trxml](https://github.com/kaby76/Trash/tree/main/src/trxml) for [XML output](https://github.com/kaby76/Trash/blob/main/_tests/trconvert/antlr2/ada.g4.xml),
[trst](https://github.com/kaby76/Trash/tree/main/src/trst) for [Antlr runtime ToStringTree output](https://github.com/kaby76/Trash/blob/main/_tests/trconvert/antlr2/ada.g4.st),
[trdot](https://github.com/kaby76/Trash/tree/main/src/trdot),
[trprint](https://github.com/kaby76/Trash/tree/main/src/trprint) for input text for the parse,
and
[tragl](https://github.com/kaby76/Trash/tree/main/src/tragl).

### Convert grammars to Antlr4
```
dotnet trash parse ada.g2 | dotnet trash convert | trprint | less
```
This command parses an [old Antlr2 grammar](https://github.com/kaby76/Trash/blob/main/_tests/trconvert/antlr2/ada.g2)
using [trparse](https://github.com/kaby76/Trash/tree/main/src/trparse),
converts the parse tree data to Antlr4 syntax using
 [trconvert](https://github.com/kaby76/Trash/tree/main/src/trconvert)
 and
finally [prints out the converted parse tree data, ada.g4](https://github.com/kaby76/Trash/blob/main/_tests/trconvert/antlr2/ada.g4)
using
[trprint](https://github.com/kaby76/Trash/tree/main/src/trprint). Other
grammar that can be converted are Antlr3, Bison, and ISO EBNF. In order to
use the grammar to parse data, you will need to convert it to an Antlr4 grammar.

### Generate an Arithmetic parser application
```
mkdir foobar; cd foobar; dotnet trash gen
```
This command creates a parser application for the C# target.
If executed in an empty directory, which is done in the example
shown above, [trgen](https://github.com/kaby76/Trash/tree/main/src/trgen)
creates an application using the Arithmetic grammar.
If executed in a directory containing
a Antlr Maven plugin (`pom.xml`), `trgen` will create a program according
to the information specified in the `pom.xml` file. Either way, it creates a directory
`Generated/`, and places the source code there.

`trgen` has many options to generate a parser from any Antlr4 grammar, for any target.
But, if a parser is generated for the C# target, built using the NET SDK, then `trparse`
can execute the generated parser, and can be used with all the other tools in Trash. _NB:
In order to use the generate parser application, you must first build it:

    dotnet restore Generated/Test.csproj
    dotnet build Generated/Test.csproj

### Run the generated parser application

    trash parse -i "1+2+3" | trash tree

After using `trgen` to generate a parser program in C#, shown previously,
and after building the program, you can run the parser using `trparse`. This program 
looks for the generated parser in directory `Generated/`. If it exists,
it will run the parser application in the directory. You can pass
as command-line arguments an input string or input file. If no command-line
arguments are supplied, the program will read stdin. The output of `trparse`, as
with most tools of Trash, is parse tree data.

### Find nodes in the parse tree using XPath

    mkdir empty; cd empty; trash gen; dotnet build Generated/Test.csproj; \
        trash parse -i "1+2+3" | trash query "grep //SCIENTIFIC_NUMBER"

With this command, a directory is created, the Arithmetic grammar generated, build,
and then run using [parse](https://github.com/kaby76/Trash/tree/main/src/trparse).
The `trash parse` tool unifies all parsing, whether it's parsing a grammar or parsing input
using a generated parser application. The output from the `trparse` tool is a parse
tree which you can search. [query](https://github.com/kaby76/Trash/tree/main/src/trquery)
is the generalized search program for parse trees. `Trquery` uses XPath expressions to
precisely identify nodes in the parse tree.

XPath was added to Antlr4, but `Trash` takes the idea
further with the addition of an XPath2 engine ported from the
[Eclipse Web toolkit](https://git.eclipse.org/r/admin/repos/sourceediting%2Fwebtools.sourceediting).
XPath is a well-defined language that should be
used more often in compiler construction.

### Rename a symbol in a grammar, generate a parser for new grammar

    trash parse Arithmetic.g4 | trash rename "//parserRuleSpec//labeledAlt//RULE_REF[text() = 'expression']" "xxx" | dotnet trash text > new-source.g4
    trash parse Arithmetic.g4 | trash rename -r "expression,expression_;atom,atom_;scientific,scientific_" | trprint

In these two examples, the Arithmetic grammar is parsed.
[trrename](https://github.com/kaby76/Trash/tree/main/src/trrename) reads the parse tree data and
modifies it by renaming the `expression` symbol two ways: first by XPath expression identifying the LHS terminal
symbol of the `expression` symbol, and the second by assumption that the tree is an Antlr4 parse tree,
then renaming a semi-colon-separated list of paired renames. The resulting code is reconstructed and saved.
`trrename` does not rename symbols in actions, nor does it rename identifiers corresponding to the
grammar symbols in any support source code (but it could if the tool is extended).

### Count method declarations in a Java source file

    git clone https://github.com/antlr/grammars-v4.git; \
        cd grammars-v4/java/java9; \
        trash gen; dotnet build Generated/Test.csproj;\
        trash parse examples/AllInOne8.java | trash query "greap //methodDeclaration" | trst | wc

This command clones the Antlr4 grammars-v4 repo, generates a parser for the Java9 grammar,
then runs the parser on [examples/AllInOne8.java](https://github.com/antlr/grammars-v4/blob/master/java/java9/examples/AllInOne8.java).
The parse tree is then piped to `trquery` to find all parse tree nodes that are
a `methodDeclaration` type, converts it to a simple string, and counts the result using
`wc`.

### Strip a grammar of all non-essential CFG

    trash parse Java9.g4 | trash strip | trash text > Essential-Java9.g4

### Split a grammar

Since Antlr2, one can written a combined parser/lexer in one file,
or a split parser/lexer in two files.
While it's not hard to split or combine
a grammar, it's tedious. For automating transformations, it's
necessary because Antlr4 requires the grammars to be split
when super classes are needed for different targets.

    trash combine ArithmeticLexer.g4 ArithmeticParser.g4 | trash text > Arithmetic.g4

This command calls [trcombine](https://github.com/kaby76/Trash/tree/main/src/trcombine)
which parses two split grammar files
[ArithmeticLexer.g4](https://github.com/kaby76/Trash/blob/main/_tests/combine/ArithmeticLexer.g4)
and
[ArithmeticParser.g4](https://github.com/kaby76/Trash/blob/main/_tests/combine/ArithmeticParser.g4),
and creates a [combined grammar](https://github.com/kaby76/Trash/blob/main/_tests/combine/Arithmetic.g4)
for the two.

    trash parse Arithmetic.g4 | trash split | trash sponge -o true

This command calls [trsplit](https://github.com/kaby76/Trash/tree/main/src/trsplit)
which splits the grammar into two parse tree results, one that defines
ArithmeticLexer.g4 and the other that defines ArithmeticParser.g4.
The tool [trsponge](https://github.com/kaby76/Trash/tree/main/src/trsponge)
is similar to the [tee](https://en.wikipedia.org/wiki/Tee_(command)) in
Linux: the parse tree data is split and placed in files.

## Parsing Result Sets -- the data passed between commands

A *parsing result set* is a JSON serialization of an array of:

* A set of parse tree nodes.
* Parser information related to the parse tree nodes.
* Lexer information related to the parse tree nodes.
* The name of the input corresponding to the parse tree nodes.
* The input text corresponding to the parse tree nodes.

Most commands in Trash read and/or write parsing result sets.

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

## Analysis

### Recursion

* [Has direct/indirect recursion](https://github.com/kaby76/Trash/blob/main/doc/analysis.md#has-directindirect-recursion)

## Refactoring

Trash provides a number of transformations that can help to make grammars cleaner (reformatting),
more readable (reducing the length of the RHS of a rule),
and more efficient (reducing the number of non-terminals) for Antlr.

Some of these refactorings are very specific for Antlr due to the way
the parser works, e.g., converting a prioritized chain of productions recognizing
an arithmetic expression to a recursive alternate form.
The refactorings implemented are:

### Raw tree editing

* [Delete parse tree node](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#delete-parse-tree-node)

### Reordering

* [Move start rule to top](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#move-start-rule)
* [Reorder parser rules](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#reorder-parser-rules)
* [Sort modes](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#sort-modes)

### Changing rules

* [Remove useless parentheses](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#remove-useless-parentheses)
* [Remove useless parser rules](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#remove-useless-productions)
* [Rename lexer or parser symbol](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#rename)
* [Unfold](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#Unfold)
* [Group alts](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#group-alts)
* [Ungroup alts](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#ungroup-alts)
* [Upper and lower case string literals](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#upper-and-lower-case-string-literals)
* [Fold](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#Fold)
* Replace direct left recursion with right recursion
* [Replace direct left/right recursion with Kleene operator](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#Kleene)
* Replace indirect left recursion with right recursion
* Replace parser rule symbols that conflict with Antlr keywords
* [Replace string literals in parser with lexer symbols](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#replace-literals-in-parser-with-lexer-token-symbols)
* Replace string literals in parser with lexer symbols, with lexer rule create
* [Delabel removes the annoying and mostly useless labeling in an Antlr grammar](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#delabel)

### Splitting and combining

* [Split combined grammars](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#splitting-and-combining-grammars)
* [Combine splitted grammars](https://github.com/kaby76/Trash/blob/main/doc/refactoring.md#splitting-and-combining-grammars)

## Conversion

* [Antlr3 import](https://github.com/kaby76/Trash/blob/main/doc/Import.md#antlr3)
* [Antlr2 import](https://github.com/kaby76/Trash/blob/main/doc/Import.md#antlr2)
* [Bison import](https://github.com/kaby76/Trash/blob/main/doc/Import.md#bison)

---------

The source code for the extension is open source, free of charge, and free of ads. For the latest developments on the extension,
check out my [blog](http://codinggorilla.com).

# Building

    git clone https://github.com/kaby76/Trash
    cd Trash
    make clean; make; make install
    
You must have the NET SDK version 10 installed to build and run.

# Releases

See https://github.com/kaby76/Trash/releases.

If you have any questions, email me at ken.domino <at> gmail.com
