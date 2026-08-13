# Trash

[![Build](https://github.com/kaby76/Trash/workflows/CI/badge.svg)](https://github.com/kaby76/Trash/actions?query=workflow%3ACI)

Trash is a collection of command-line tools to analyze and transform
Antlr4 grammars and parse trees. The toolkit can:
* Generate a parser application for an Antlr4 grammar for any target and any OS;
* Generate and parse input for an Antlr4 grammar that is independent of Antlr4 and runtime;
* Use XPath4, XQuery4, and the XQuery Update Facility languages to search and modify parse trees, including "off-channel" content such as comments.
* Chain together output from different applications to form complex queries and refactorings.

With the [grammars-v4 collection of Antlr4 grammars](https://github.com/antlr/grammars-v4),
one can write applications that parse popular programming languages quickly and easily.

Each app in `Trash` is implemented as a sub-packaged [Dotnet Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) console application, and can be used on Windows, Linux, or Mac.
No prerequisites are required other than installing the
[NET SDK](https://dotnet.microsoft.com/), and the toolchains
for any other targets you want to use. All commands are executed through the top-level Dotnet application
"trash", e.g., "dotnet trash parse --help".

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

Every command is invoked as `dotnet trash <name> [options] [args]`.
Most commands read a *parse result set* from stdin and write one to stdout,
making them composable with `|`.
Full invocation rules (argument order, `--` separator, MSYS2 quoting) and a
sortable reference table are in
**[commands.html](https://htmlpreview.github.io/?https://github.com/kaby76/Trash/blob/main/commands.html)**.

| Command | Description |
|---------|-------------|
| [analyze](src/tranalyze/readme.md) | Analyze a grammar |
| [caret](src/trcaret/readme.md) | Caret operations on a parse tree |
| [clonereplace](src/trclonereplace/readme.md) | Clone and replace in a grammar |
| [combine](src/trcombine/readme.md) | Combine a split Antlr4 grammar |
| [convert](src/trconvert/readme.md) | Convert a grammar from one form to another |
| [cover](src/trcover/readme.md) | Code coverage analysis |
| [dot](src/trdot/readme.md) | Print a parse tree in Graphviz Dot format |
| [extract](src/trextract/readme.md) | Extract target-specific code from a grammar |
| [ff](src/trff/readme.md) | Output FIRST and FOLLOW sets of a grammar |
| [foldlit](src/trfoldlit/readme.md) | Fold transform on grammar with literals |
| [gen](src/trgen/readme.md) | Generate an Antlr4 parser for a given target language |
| [genvsc](src/trgenvsc/readme.md) | Generate VS Code extension files |
| [glob](src/trglob/readme.md) | Expand glob file patterns |
| [iconv](src/triconv/readme.md) | Convert file encoding |
| [interp](src/trinterp/readme.md) | Generate Antlr4 `.interp` files from a grammar parse tree |
| [itext](src/tritext/readme.md) | Get strings from a PDF file |
| [json](src/trjson/readme.md) | Print a parse tree in JSON structured format |
| [nullable](src/trnullable/readme.md) | Nullable analysis of a grammar |
| [parse](src/trparse/readme.md) | Parse a grammar or use a generated parser to parse input |
| [perf](src/trperf/readme.md) | Performance analysis of an Antlr grammar parse |
| [query](src/trquery/readme.md) | Query parse trees using XPath |
| [rename](src/trrename/readme.md) | Rename symbols in a grammar |
| [sort](src/trsort/readme.md) | Sort rules in a grammar |
| [split](src/trsplit/readme.md) | Split a combined Antlr4 grammar |
| [sponge](src/trsponge/readme.md) | Write parse result set back to files on disk |
| [text](src/trtext/readme.md) | Print source text for parse tree nodes |
| [tokens](src/trtokens/readme.md) | Print tokens in a parse tree |
| [tree](src/trtree/readme.md) | Print a parse tree in a human-readable format |
| [unfold](src/trunfold/readme.md) | Unfold transform on a grammar |
| [unfoldlit](src/trunfoldlit/readme.md) | Unfold transform with literals on a grammar |
| [ungroup](src/trungroup/readme.md) | Ungroup transform on a grammar |
| [wdog](src/trwdog/readme.md) | Kill a program that runs too long |
| [xpath](src/trxpath/readme.md) | Search parse trees using XPath 4.0 |
| [xml](src/trxml/readme.md) | Print a parse tree in XML format |
| [xml2](src/trxml2/readme.md) | Enumerate all XPath paths in a parse tree to leaves |
| [xquery](src/trxquery/readme.md) | Apply XQuery Update expressions to a parse tree |

## Examples

### Parse a grammar, create a parser for the grammar, build, and test
```
git clone https://github.com/antlr/grammars-v4
cd grammars-v4/python/python
dotnet trash parse *.g4 | dotnet trash xpath ' //grammarDecl' | dotnet trash text
# Output:
# lexer grammar PythonLexer;
# parser grammar PythonParser;
dotnet trash gen -t CSharp
cd Generated-CSharp
dotnet build
cat - <<EOF | dotnet trash parse | dotnet trash xpath ' //test' | dotnet trash text
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
`dotnet trash tree` is only one of several ways to view parse tree data.
Other commands for different output are
[dotnet trash json](https://github.com/kaby76/Trash/tree/main/src/trjson) for [JSON output](https://github.com/kaby76/Trash/blob/main/tests/trconvert/antlr2/ada.g4.json),
[dotnet trash xml](https://github.com/kaby76/Trash/tree/main/src/trxml) for [XML output](https://github.com/kaby76/Trash/blob/main/tests/trconvert/antlr2/ada.g4.xml),
[dotnet trash dot](https://github.com/kaby76/Trash/tree/main/src/trdot) for Graphviz Dot output,
and
[dotnet trash text](https://github.com/kaby76/Trash/tree/main/src/trtext) for the source text of a parse tree interval.

### Generate an Arithmetic parser application
```
mkdir foobar; cd foobar; dotnet trash gen
```
This command creates a parser application for the C# target.
If executed in an empty directory, which is done in the example
shown above, [dotnet trash gen](https://github.com/kaby76/Trash/tree/main/src/trgen)
creates an application using the Arithmetic grammar.
If executed in a directory containing
an Antlr Maven plugin (`pom.xml`), `dotnet trash gen` will create a program according
to the information specified in the `pom.xml` file. Either way, it creates a directory
`Generated-<target>/` (e.g. `Generated-CSharp/`), and places the source code there.

`dotnet trash gen` has many options to generate a parser from any Antlr4 grammar, for any target.
But, if a parser is generated for the C# target, built using the NET SDK, then `dotnet trash parse`
can execute the generated parser, and can be used with all the other tools in Trash. _NB:
In order to use the generated parser application, you must first build it:

    dotnet restore Generated-CSharp/Test.csproj
    dotnet build Generated-CSharp/Test.csproj

### Run the generated parser application

    dotnet trash parse -i "1+2+3" | dotnet trash tree

After using `dotnet trash gen` to generate a parser program in C#, shown previously,
and after building the program, you can run the parser using `dotnet trash parse`. This program
looks for the generated parser in the `Generated-CSharp/` directory. If it exists,
it will run the parser application in the directory. You can pass
as command-line arguments an input string or input file. If no command-line
arguments are supplied, the program will read stdin. The output of `dotnet trash parse`, as
with most tools of Trash, is parse tree data.

### Find nodes in the parse tree using XPath

    mkdir empty; cd empty; dotnet trash gen; dotnet build Generated-CSharp/Test.csproj; \
        dotnet trash parse -i "1+2+3" | dotnet trash query "grep //SCIENTIFIC_NUMBER"

With this command, a directory is created, the Arithmetic grammar generated, built,
and then run using [dotnet trash parse](https://github.com/kaby76/Trash/tree/main/src/trparse).
The `dotnet trash parse` tool unifies all parsing, whether it's parsing a grammar or parsing input
using a generated parser application. The output from `dotnet trash parse` is a parse
tree which you can search. [dotnet trash query](https://github.com/kaby76/Trash/tree/main/src/trquery)
is the generalized search program for parse trees. `dotnet trash query` uses XPath expressions to
precisely identify nodes in the parse tree.

XPath was added to Antlr4, but `Trash` takes the idea
further with the addition of an XPath2 engine ported from the
[Eclipse Web toolkit](https://git.eclipse.org/r/admin/repos/sourceediting%2Fwebtools.sourceediting).
XPath is a well-defined language that should be
used more often in compiler construction.

### Rename a symbol in a grammar, generate a parser for new grammar

    dotnet trash parse Arithmetic.g4 | dotnet trash rename "//parserRuleSpec//labeledAlt//RULE_REF[text() = 'expression']" "xxx" | dotnet trash text > new-source.g4
    dotnet trash parse Arithmetic.g4 | dotnet trash rename -r "expression,expression_;atom,atom_;scientific,scientific_" | dotnet trash text

In these two examples, the Arithmetic grammar is parsed.
[dotnet trash rename](https://github.com/kaby76/Trash/tree/main/src/trrename) reads the parse tree data and
modifies it by renaming the `expression` symbol two ways: first by XPath expression identifying the LHS terminal
symbol of the `expression` symbol, and the second by assumption that the tree is an Antlr4 parse tree,
then renaming a semi-colon-separated list of paired renames. The resulting code is reconstructed and saved.
`dotnet trash rename` does not rename symbols in actions, nor does it rename identifiers corresponding to the
grammar symbols in any support source code (but it could if the tool is extended).

### Count method declarations in a Java source file

    git clone https://github.com/antlr/grammars-v4.git; \
        cd grammars-v4/java/java9; \
        dotnet trash gen; dotnet build Generated-CSharp/Test.csproj;\
        dotnet trash parse examples/AllInOne8.java | dotnet trash xpath ' //methodDeclaration' | dotnet trash text | wc

This command clones the Antlr4 grammars-v4 repo, generates a parser for the Java9 grammar,
then runs the parser on [examples/AllInOne8.java](https://github.com/antlr/grammars-v4/blob/master/java/java9/examples/AllInOne8.java).
The parse tree is then piped to `dotnet trash query` to find all parse tree nodes that are
a `methodDeclaration` type, prints the source text of each, and counts the result using
`wc`.

### Combine or Split a grammar

    dotnet trash combine ArithmeticLexer.g4 ArithmeticParser.g4 | dotnet trash text > Arithmetic.g4

    dotnet trash parse Arithmetic.g4 | dotnet trash split | dotnet trash sponge -o split-grammar

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
