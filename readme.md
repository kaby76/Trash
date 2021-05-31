# Trash

**Trash** is a toolkit for analyzing and editing grammars. While there are
[dozens of parser generators](https://en.wikipedia.org/wiki/Comparison_of_parser_generators),
there are few, if any, compiler-based tools to help edit those grammars.
In my opinion, grammars are just like code: they require debugging, extension,
porting, and conversion to work with new parser generators. It's been
50 years since Yacc was first developed, but we are still typing in grammars from books,
still informally specifying programming languages, still hand-writing parsers (insisting
that a hand-written parser is faster and better than a generated one--even if from a
hand-tweaked template!), modifying the hand-written parser even before defining
what the syntax is supposed to be, and still...still using Yacc!

Trash is a collection of command-line tools to support the analysis and editing
of grammars, currently and specifically for Antlr4. The toolkit can generate a parser
application for an Antlr4 grammar
with target source code in C#, Java, JavaScript, Python3, C++, Go, or Dart.
For the C# target, the generated parser can be used seemlessly with the
rest of the toolkit, such as with the display/grep/edit
of the parse trees.
All tools pass parse tree data through stdin and stdout so they
may be combined to create complex commands.

Each app in `Trash` is implemented as a [Dotnet Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) console application,
except 
[tragl](https://github.com/kaby76/Domemtech.Trash/tree/main/tragl)
because it uses WPF on Windows to display a parse tree.
Consequently, the toolkit can be used on Windows, Linux, or Mac.
No prerequisites are required other than installing the
[NET SDK](https://dotnet.microsoft.com/), and the toolchains
for any other targets you want to use.

The toolkit uses [Antlr](https://www.antlr.org/) and
[XPath2](https://en.wikipedia.org/wiki/XPath).
The code is implemented in C#.

## Install

Linux: Right-click, "save target as" of file
<a href="https://raw.githubusercontent.com/kaby76/Domemtech.Trash/master/_scripts/install.sh">install.sh</a>
, then run the script in the Bash shell (or `bash install.sh`).

Windows: Right-click, "save target as" of file
<a href="https://raw.githubusercontent.com/kaby76/Domemtech.Trash/master/_scripts/install.ps1">install.ps1</a>
, then run the script in Powershell (or `powershell install.ps1`).

## Examples

### Parse a grammar

    trparse ada.g4 | trtree | vim -

This command parses the Antlr4 grammar
[ada.g4](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/convert2/ada.g4)
using [trparse](https://github.com/kaby76/Domemtech.Trash/tree/main/trparse),
prints out the parse tree data as a simple
[text-oriented diagram](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/convert2/ada.g4.tree)
using [trtree](https://github.com/kaby76/Domemtech.Trash/tree/main/trtree),
then opens [vim](https://www.vim.org/) on the diagram. If you are not
familiar with `Vim`, then you can use [less](http://www.greenwoodsoftware.com/less/),
or save the output from `trtree` to a file
and open that with any other editor you would like. `trparse` can infer the type of
parse from the file name suffix.

`trtree` is only one of several ways to view parse tree data.
Other programs for different output are
[trjson](https://github.com/kaby76/Domemtech.Trash/tree/main/trjson) for [JSON output](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/convert2/ada.g4.json),
[trxml](https://github.com/kaby76/Domemtech.Trash/tree/main/trxml) for [XML output](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/convert2/ada.g4.xml),
[trst](https://github.com/kaby76/Domemtech.Trash/tree/main/trst) for [XML output](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/convert2/ada.g4.st),
[trdot](https://github.com/kaby76/Domemtech.Trash/tree/main/trdot),
[trprint](https://github.com/kaby76/Domemtech.Trash/tree/main/trprint) for input text for the parse,
and
[tragl](https://github.com/kaby76/Domemtech.Trash/tree/main/tragl).

### Convert grammars to Antlr4

    trparse ada.g2 | trconvert | trprint > ada.g4

This command parses an [old Antlr2 grammar](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/convert2/ada.g2)
using [trparse](https://github.com/kaby76/Domemtech.Trash/tree/main/trparse),
converts the parse tree data to Antlr4 syntax using
 [trconvert](https://github.com/kaby76/Domemtech.Trash/tree/main/trconvert)
 and
finally [prints out the converted parse tree data](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/convert2/ada.g4)
using
[trprint](https://github.com/kaby76/Domemtech.Trash/tree/main/trprint). Other
grammar that can be converted are Antlr3, Bison, and ISO EBNF. In order to
use the grammar to parse data, you will need to convert it to an Antlr4 grammar.

### Generate a parser application

    trgen

This command creates a parser application for the C# target.
If executed in an empty directory, [trgen](https://github.com/kaby76/Domemtech.Trash/tree/main/trgen)
creates an application using the Arithmetic grammar.
If executed in a directory containing
a Antlr Maven plugin (`pom.xml`), `trgen` will create a program according
to the information specified in the `pom.xml` file. Either way, it creates a directory
`Generated/`, and places the source code there.

`trgen` has many options to generate a parser from any Antlr4 grammar, for any target.
But, if a parser is generated for the C# target, built using the NET SDK, then `trparse`
can execute the generated parser, and can be used with all the other tools in Trash. _NB:
In order to use the generate parser application, you must first build it: **cd Generated;
dotnet restore; dotnet build; cd ..**._

### Run the generated parser application

    trparse -i "1+2+3" | trtree

After using `trgen` to generate a parser program in C#, and after building
the program, you can run the parser using `trparse`. This program 
looks for the generated parser in directory `Generated/`. If it exists,
it will run the parser application in the directory. You can pass
as command-line arguments an input string or input file. If no command-line
arguments are supplied, the program will read stdin. The output of `trparse`, as
with most tools of Trash, is parse tree data.

### Find nodes in the parse tree using XPath

    trparse -i "1+2+3" | trxgrep " //SCIENTIFIC_NUMBER" | trst

Using `trparse`, you can create a parse tree that can be searched
using `trxgrep`. The tool `trxgrep` uses XPath expressions to identify
exactly what node(s) in the tree you want. Those sub-trees can be
printed using any of the tools shown previously.

A major problem I noticed is a lack of a standardized way to identify
nodes in parse trees. XPath is a well-defined language that should be
used more often.

### Rename a symbol in a grammar, generate a parser for new grammar

    trparse Arithmetic.g4 | trrename "//parserRuleSpec//labeledAlt//RULE_REF[text() = 'expression']" "xxx" | trtext > new-source.g4
    trparse Arithmetic.g4 | trrename -r "expression,expression_;atom,atom_;scientific,scientific_" | trprint

### Count method declarations in a Java source file

    trgen --start-rule compilationUnit
    cd Generated/; dotnet build; cd ..
    trparse WindowsState.java | trxgrep "//methodDeclaration" | trst | wc

To count the number of methods in a Java source file, first generate a
parser, build it, and then run `trparse` to create a parse tree for the
input. Then, use `trxgrep` to pick off the methods, convert the nodes
found into a one-per-line tree, and use `wc` to count the number.

### Strip a grammar of all non-essential CFG

    trparse Java9.g4 | trstrip | trtext > new-grammar.g4

### Split a grammar

Since Antlr2, one can written a combined parser/lexer in one file,
or a split parser/lexer in two files.
While it's not hard to split or combine
a grammar, it's tedious. For automating transformations, it's
necessary because Antlr4 requires the grammars to be split
when super classes are needed for different targets.

    trcombine ArithmeticLexer.g4 ArithmeticParser.g4 | trprint > Arithmetic.g4

This command calls [trcombine](https://github.com/kaby76/Domemtech.Trash/tree/main/trcombine)
which parses two split grammar files
[ArithmeticLexer.g4](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/combine/ArithmeticLexer.g4)
and
[ArithmeticParser.g4](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/combine/ArithmeticParser.g4),
and creates a [combined grammar](https://github.com/kaby76/Domemtech.Trash/blob/main/_tests/combine/Arithmetic.g4)
for the two.

    trparse Arithmetic.g4 | trsplit | trsponge -o

This command calls [trsplit](https://github.com/kaby76/Domemtech.Trash/tree/main/trsplit)
which splits the grammar into two parse tree results, one that defines
ArithmeticLexer.g4 and the other that defines ArithmeticParser.g4.
The tool [trsponge](https://github.com/kaby76/Domemtech.Trash/tree/main/trsponge)
is similar to the [tee](https://en.wikipedia.org/wiki/Tee_(command)) in
Linux: the parse tree data is split and placed in files.

## Parsing Result Sets -- the data passed between commands

A *result set* is a JSON serialization of:

* A set of parse tree nodes.
* Parser information related to the parse tree nodes.
* Lexer information related to the parse tree nodes.
* The name of the input corresponding to the parse tree nodes.
* The input text corresponding to the parse tree nodes.

Most commands in Trash read and/or write parse result sets.

## Commands of Trash

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

# Current release

## 0.8.0 (27 May 2021)

* Preliminary release of the toolset.

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

    git clone https://github.com/kaby76/Domemtech.Trash
    cd Domemtech.Trash
    dotnet restore
    dotnet build

You must have the NET SDK installed to build and run.

# Prior Releases

(Incomplete.)

# Roadmap

## Planned for v1

* Place Trash in it's own repo, independent of Antlrvsix.
* Replace Trash shell and commands with Bash and independent programs.
* Provide full documentation.

If you have any questions, email me at ken.domino <at> gmail.com
