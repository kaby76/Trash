# Trparse

Parse a file, command-line string argument, or stdin
using a built-in or generated parser, and output to stdout the
parse tree data. If the current directory contains a parser in
the `Generated/` sub-directory, then the tool will use the
parser in `Generated/`. With the `--file` command-line option,
you can parse the grammar, based on suffices (`.g1` for an Antlr2
grammar, `.g3` for an Antlr3 grammar, `.g4` for an Antlr4 grammar,
`.y` for a Bison grammar, `.ebnf` for an ISO EBNF grammar).
If the suffix is not standard, you can for the type of parse with
the `--type` command-line option (`antlr2` for an Antlr2
grammar, `antlr3` for an Antlr3 grammar, `antlr4` for an Antlr4
grammar, `bison` for a Bison grammar, `ebnf` for an ISO EBNF grammar,
or `gen` for the generated parser in `Generated/`, if there is any
confusion.

# Usage
    
    trparse _options_
    -i, --input String input.
    -f, --file  File input.
    -t, --type  Specifies type of parse, antlr4, antlr3, antlr2, bison, ebnf, gen 

# Examples

    trparse -f Java.g2
    trparse -i "1+2+3"
    trparse -f Foobar.g -t antlr2
    echo "1+2+3" | trparse | trtree

# Current version

0.8.1 -- Improved help and documentation.
