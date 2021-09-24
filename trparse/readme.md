# Trparse

Parse a file, command-line string argument, or stdin
using a built-in or generated parser, and output to stdout the
parse tree data. If the current directory contains a parser in
the `Generated/` sub-directory, then the tool will use the
parser in `Generated/`. Otherwise, the parse type will depend
on other inputs.

With the positional args,
a file is parse. If not using the `Generated/` parser,
the type of parse will be based on the file suffix:

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

# Usage
    
    trparse _options_
    -i, --input String input.
    -t, --type  Specifies type of parse, antlr4, antlr3, antlr2, bison, ebnf, gen 

# Examples

    trparse Java.g2
    trparse -i "1+2+3"
    trparse Foobar.g -t antlr2
    echo "1+2+3" | trparse | trtree

# Current version

0.11.0 -- Updated trkleen. Added trreplace.
