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

0.23.0 Synchronized Antlr4 grammar with grammars-v4. Fix Typescript and Antlr4ng templates. Updates to trperf. Add xpath addressing of off-channel tokens. Update trfoldlit. Fix testing, build scripts. Remove trlabel, trstrip.

## License

The MIT License

Copyright (c) 2024 Ken Domino

Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to 
deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, 
merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom 
the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice 
shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
