# AntlrVSIX

AntlrVSIX is a tool to support editing, analysis, refactoring,
and conversion of context-free grammars, including Antlr,
Bison, ISO 14977, LBNF, and W3C EBNF. It contains
extensions for Visual Studio 2019, Visual Studio Code, Gnu Emacs,
a [Language Server Protocol (LSP)](https://langserver.org/) server,
and a stand-alone command-line tool known as [Trash](https://github.com/kaby76/AntlrVSIX/blob/master/Trash/readme.md).
Trash is analogous to
[Bash](https://en.wikipedia.org/wiki/Bash_(Unix_shell))
but for parsing, in the lingua-franca of parsing: parse trees and XPath.
All clients use the LSP server implemented in this repository. The
clients each are implemented elsewhere, but a thin shim is provided in this
repository to get the client to work with the server.
For Visual Studio 2019, the client is implemented using Microsoft's 
[client](https://www.nuget.org/packages/Microsoft.VisualStudio.LanguageServer.Client/).
The server is implemented using
[Antlr](https://www.antlr.org/), [Antlr4BuildTasks](https://github.com/kaby76/Antlr4BuildTasks),
[XPath2](https://en.wikipedia.org/wiki/XPath), 
[S-expressions](https://en.wikipedia.org/wiki/S-expression)
for Antlr parse trees,
and a number of other tools.
Most of the code is implemented in C#.
The extension for VS Code is written in Typescript.
Supported are colorized tagging, hover, go to def, find all refs,
replace, and reformat. Beyond the basic LSP commands, the tool supports
analysis, refactoring, and conversion.
__NB: Antlrvsix for Visual Studio 2019 v8.3 will not offer semantic highlighting
by default because the [MS LSP client in VS2019](https://www.nuget.org/packages/Microsoft.VisualStudio.LanguageServer.Client/)
does not support semantic highlighting. In fact,
it is 3 years behind the protocol. In order to support it for VSCode,
I had to rewrite the entire [language server protocol library](https://www.nuget.org/packages/Microsoft.VisualStudio.LanguageServer.Protocol/)
(replacement [here](https://www.nuget.org/packages/LspTypes/))--in a week--in order to bring
it up to date. MS is too slow. If you want semantic highlighting, you can turn on the feature
in the Antlrvsix options. Or, better, you could edit your grammar file using VSCode with Antlrvsix-vscode
or Mike's vscode-antlr extension for VSCode. This problem will be addressed in
release v8.4 or v9.0.__

## Supported grammars and features

| Grammars | File suffix | Features supported |
| ---- | ---- | ---- |
| Antlr4 | .g4 | Basic LSP services (tagging, go to def, find all refs, rename, etc.); transformations; analysis |
| Antlr3 | .g3 | Basic LSP services (tagging, go to def, find all refs, rename, etc.); conversion to Antlr4 |
| Antlr2 | .g2 | Basic LSP services (tagging, go to def, find all refs, rename, etc.); conversion to Antlr4 |
| Bison | .y | Basic LSP services (tagging, go to def, find all refs, rename, etc.); conversion to Antlr4 |
| ISO 14977 | .iso .iso14977 | Basic LSP services (tagging, go to def, find all refs, rename, etc.) |
| LBNF | .cf | Basic LSP services (tagging, go to def, find all refs, rename, etc.) |
| W3C EBNF | .ebnf | Basic LSP services (tagging, go to def, find all refs, rename, etc.); conversion to Antlr4 |

# Installation

## Visual Studio 2019

For Visual Studio 2019, I recommend that you use the latest, upgraded version since it has the latest bug fixes.
To install, visit the [Visual Studio Marketplace for VS2019 for Antlrvsix](https://marketplace.visualstudio.com/items?itemName=KenDomino.AntlrVSIX). Download and install the .vsix file. You can also install it through the `Manage Extensions`
menu item in Visual Studio 2019.

If you find this extension not to your liking, you can try to use the extension for Visual Studio Code.
I found that it is sometimes easier to install [Open in Visual Studio Code](https://github.com/madskristensen/OpenInVsCode/)
and edit the grammar in VSCode due to VS2019 not directly supporting LSP semantic highlighting.

## Visual Studio Code

For Visual Studio Code, visit the [Visual Studio Marketplace for VSCode for Antlrvsix](https://marketplace.visualstudio.com/items?itemName=KenDomino.Antlrvsix-vscode). Download and install the the .vsix file. NB: the vsix file between VS2019 and VSCode are
not the same files.

Note, to get semantic highlighting of grammar symbols, you need to make sure to pick a color theme that has them,
such as "Light+", "Dark+", etc., and make sure to adjust the settings "File > Settings > Text Editor > Semantic Highlighting: Enabled, true". If you just start out with VSCode, you get nothing.

## GNU Emacs

For Emacs, you will need to follow the instructions [here](https://github.com/kaby76/AntlrVSIX/tree/master/Emacs).

## Nightly build

[![Build status](https://ci.appveyor.com/api/projects/status/ekyhff3c28p5qgox?svg=true)](https://ci.appveyor.com/project/kaby76/antlrvsix)

Each night at 0h 0m UTC, Appveyor builds the latest source. The output of the build can be downloaded
from Appveyor.

* [Antlrvsix for VS2019](https://ci.appveyor.com/api/projects/kaby76/antlrvsix/artifacts/VsIde/bin/Debug/AntlrVSIX.vsix)
* [Antlrvsix for VSCode](https://ci.appveyor.com/api/projects/kaby76/antlrvsix/artifacts/VsCode/Antlrvsix-vscode-1.2.0.vsix)

# Current release

## VsIde v8.3, VsCode 1.2 (30 Dec 2020)

After two months of work, release 8.3 for Antlrvsix has been completed.
Most of the changes pertain to Trash, the command-line interpreter
shell contained within Antlrvsix, but there are a few other important
changes to the extension itself.

Release 8.3 features an all-new input/output pipe implementation between
commands in Trash. It uses a JSON serialization of parse trees between
commands. The JSON serialization also contains the parser, lexer, token
stream, file contents, and file location. Basically, it includes everything
that one would need for a command to manipulate a parse tree. The purpose
of the JSON file is so that each command can now be implemented
“out-of-process”, meaning that Trash can now be replaced by Bash and
each command implemented as an independent program in any language of
choice. Nobody wants yet another monolithic program to implement
programming language tools. This release works towards an open system.
With this release, all the commands and Trash still are implemented in
one program, but it will be switched over in a month or two.

I’ve also included a few new goodies:

* Added a grammar diff program.
* Added an ISO 14977 parser.
* Added AGL (Automatic Graph Layout), XML, JSON output.
* Added Cat, Echo, Wc, Strip commands.
* Adding BNFC’s LBNF and other associated grammars. NB: This work is not yet complete, and only works for the simplest of grammars.
* For ISO 14977, it was something that I decided to implement a while ago. But I didn’t know what I was getting into, and really should have read what D. Wheeler wrote about the spec. While it is now almost done, I learned along the way that the spec has several problems. One error is that the symbol meta identifier cannot contain spaces (meta identifier = letter, (letter | decimal digit);), yet throughout the spec–and meta identifier itself–meta identifier should allow spaces! And, as Wheeler pointed out, there are many other problems. Yes, grammars in Iso 14977 are very verbose…”a sea of commas”. But, it does have some interesting features, and so worth adding a parser for it.

The “diff” program I implemented with this release is interesting. I
used the Zhang-Shasha tree-edit distance algorithm, extending it to
record the actual tree edits that correspond to the minimum tree-edit
distance. This algorithm is, unfortunately, for an ordered tree, so it
works best for small differences. And, for large grammars, it is too slow.
I will be trying to implement other algorithms in the next month or two.
There is certainly a lot that could be done here. One important type of
difference is to include no only simple single-node inserts and deletes,
but more complex operations like fold and unfold.

In addition, with this release, I’m disabling semantic highlighting for
VS2019 (but not for VSCode). This is because it’s buggy and slow, and
despite my warning people, they complain about it being buggy and slow!
Use the extension for VSCode. It works fine. In the next release,
I will try to fix Antlrvsix for VS2019, but you never know.

* [Turn off semantic highlighting for VS2019 Antlrvsix #114](https://github.com/kaby76/AntlrVSIX/issues/114)
* [Trash history should be limited; Alias setup should be separated from history. #113](https://github.com/kaby76/AntlrVSIX/issues/113)
* [Include fix of Antlrv4parser.g4 grammar](https://github.com/antlr/grammars-v4/issues/1956)
* [Fix Update to NET 5! #110](https://github.com/kaby76/AntlrVSIX/issues/110)
* [Fix There should be a graph output command in Trash #108](https://github.com/kaby76/AntlrVSIX/issues/108)
* [Fix TreeOutput.OutputTree should output tokens in similar format to parse tree nodes #107](https://github.com/kaby76/AntlrVSIX/issues/107)
* [Fix "Find" should be renamed "XGrep" in Trash #106](https://github.com/kaby76/AntlrVSIX/issues/106)

# Documentation

## Guide to basic features

For information on how basic use, see the [User Guide](https://github.com/kaby76/AntlrVSIX/blob/master/doc/readme.md).

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

# Building Antlrvsix

* From a clean "git" root directory, open a "Developer command prompt", and execute:
  * msbuild /t:restore
  * msbuild

The extension is at ./Client/bin/Debug/AntlrVSIX.vsix after building successfully.

* If you want to make modifications for yourself, you should [reset your
Experimental Hive for Visual Studio](https://docs.microsoft.com/en-us/visualstudio/extensibility/the-experimental-instance?view=vs-2017). To do that,
Microsoft recommends using CreateExpInstance.exe.
Unfortunately, I've found CreateExpInstance doesn't always work because it copies from
previous hives stored under the AppData directory. It is often easier to
just recursively delete all directories ...\AppData\Local\Microsoft\VisualStudio\16.0_*. I've included a
Bash script "clean.sh" in the Antlrvsix source to clean out the build files so one can build from scratch.


---------

# Prior Releases

See [this guide](https://github.com/kaby76/AntlrVSIX/blob/master/PriorReleases.md).

# Roadmap

## Planned for v9 (end Dec 2020)

* Part two of replacing in-process Trash with out-process tools: replace commands with out-of-process programs.
* Add Intellij plugin.
* Add existing transforms not in Trash to Trash. Make sure they work.
* Add expression rule optimization. Verify that this works with Java.
* Add left factoring and inverse? Not sure, as Unify is a superset of left factoring.
* Add in empty string alternative hoist transforms.
* Add website-based Trash.
* Add AST for grammars. Rewrite all with ASTs for syntax-independent code.
* Add flexible parser to read any grammar.
* Partition Trash from Antlrvsix completely.
* 3-way grammar merge.

## Alternatives

Mike Lischke's
[vscode-antlr4](https://github.com/mike-lischke/vscode-antlr4) is a good tool for VSCode for Antlr grammars, which
you may prefer. However, Antlrvsix contains
a command-line tool that can be used to automate transformations on an
Antlr grammar, or want to quickly run a parser over a grammar file and
use XPath to query items in the parse tree. There is no tool like it, and it is quite powerful.

If you are just building and running an Antlr application and don't care about
editing features (i.e., you don't care for highlighting the grammar, go to def, etc.),
then you just use [Antlr4BuildTasks](https://github.com/kaby76/Antlr4BuildTasks). Antlr4BuildTasks
is a replacement of [Antlr4cs](https://github.com/tunnelvisionlabs/antlr4cs) and the
build tool contained in that library. The Antlr4cs is several versions behind the latest Antlr release.


Any questions, email me at ken.domino <at> gmail.com
