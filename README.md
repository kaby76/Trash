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

Runnable examples with step-by-step instructions are in the **[examples/](examples/)** directory.

## PAX/tar bundled data passing between commands

Trash commands pass an ordinary POSIX PAX/tar *artifact bundle* through
standard input and standard output. Because it is a regular tar stream, a
bundle can also be inspected with standard tools such as `tar -tf` and
`tar -xvf`.

Each input parsed by `dotnet trash parse` normally contributes two regular-file
members:

* `<name>.pt` contains one JSON-serialized `ParsingResultSet` object. It holds
  the parse-tree nodes, parser and lexer metadata, source name, and source text
  for that input.
* `<name>.errors` contains the parser diagnostics as UTF-8 text. It is empty
  when the input parsed without errors.

For example, parsing `examples/a.g4` and `examples/nested/b.g4` can produce:

```text
a.pt
a.errors
nested/b.pt
nested/b.errors
```

Input hierarchy below the inputs' common directory is preserved. The
`parse --base-directory DIR` option selects that root explicitly. Artifact
names always use `/` separators and must be relative, traversal-free paths.
When removing source extensions would make two names collide, Trash retains
enough of the original names to keep every artifact distinct.

Parse-tree commands such as `xpath`, `xquery`, `foldlit`, `rename`, `sort`,
`unfold`, `unfoldlit`, and `combine` replace the contents of the corresponding
`.pt` members and pass `.errors` and other bundle members through unchanged.
Their output is another PAX/tar bundle, so commands remain composable with `|`.

Commands whose normal output is human-readable text—such as `text`, `tree`,
`xml`, `dot`, and `tokens`—still write text by default. Give one of these
commands `--bundle` to use it as a bundle filter: every `.pt` member is replaced
by its rendered artifact (for example, `.tree`), while unrelated members pass
through unchanged.

```sh
find examples -type f \
  | dotnet trash parse --allstar -L interp -x \
  | dotnet trash tree --bundle \
  | dotnet trash sponge -o pt-allstar -c
```

`sponge` detects the PAX/tar stream automatically. Under the directory supplied
with `-o`, it reconstructs every `.pt` parse tree as source text using its
recorded source filename and copies `.errors` and other artifacts unchanged. It
rejects absolute paths, `.` or `..` path components, links, duplicate
destinations, and overwrites unless explicitly allowed.

For backward compatibility, commands also auto-detect and read the former JSON
array representation of parsing result sets. Newly produced pipeline output is
PAX/tar by default.

## About

The source code for the extension is open source, free of charge, and free of ads. For the latest developments on the extension,
check out my [blog](http://codinggorilla.com).

## Building

    git clone https://github.com/kaby76/Trash
    cd Trash
    make clean; make; make install
    
You must have the NET SDK version 10 installed to build and run.

## Releases

See https://github.com/kaby76/Trash/releases.

If you have any questions, email me at ken.domino <at> gmail.com
