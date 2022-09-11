# trparse

## Summary

Parse a grammar or use generated parse to parse input

## Description

Parse files and output to stdout parse tree data.
The tool requires a pre-built parser via trgen for a grammar
for anything other than the standard parser grammars that
are supported. To specify the grammar, you can either
be in a trgen-generated parser directory, or use the -p option.

If using positional args on the command line, a file is parse
depending on the extension of the file name:

* `.g2` for an Antlr2
* `.g3` for an Antlr3
* `.g4` for an Antlr4
* `.y` for a Bison
* `.ebnf` for ISO EBNF

You can force the type of parse with
the `--type` command-line option:

* `antlr2` for Antlr2
* `antlr3` for Antlr3
* `antlr4` for Antlr4
* `bison` for Bison
* `ebnf` for ISO EBNF
* `gen` for the `Generated/` parser

## Usage
    
    trparse (<string> | <options>)*
    -i, --input      Parse the given string as input.
    -t, --type       Specifies type of parse, antlr4, antlr3, antlr2, bison, ebnf, gen 
    -s, --start-rule Start rule name.
    -p, --parser     Location of pre-built parser (aka the trgen Generated/ directory)

## Examples

    trparse Java.g2
    trparse -i "1+2+3"
    trparse Foobar.g -t antlr2
    echo "1+2+3" | trparse | trtree
    mkdir out; trparse MyParser.g4 MyLexer.g4 | trkleene | trsponge -o out

## Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
