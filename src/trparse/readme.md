# trparse

## Summary

Parse a grammar or use generated parse to parse input

## Description

Parse files and output to stdout parse tree data.
The tool requires a pre-built parser via trgen for a grammar
for anything other than the standard parser grammars that
are supported. To specify the grammar, you can either
be in a trgen-generated parser directory, or use the -p option.

If using positional args on the command line, a file is parsed
depending on the extension of the file name:

* `.g2` for an ANTLRv2 grammar
* `.g3` for an ANTLRv3 grammar
* `.g4` for an ANTLRv4 grammar
* `.y` for a Bison grammar
* `.rex` for a Rex grammar
* `.gram` for a pegen grammar

You can force the type of parse with
the `--type` command-line option:

* `ANTLRv4` for ANTLRv2
* `ANTLRv3` for ANTLRv3
* `ANTLRv2` for ANTLRv4
* `Bison` for Bison
* `rex` for Rex
* `pegen_v3_10` for the `Generated/` parser

## Usage
    
    trparse (<string> | <options>)*
    -i, --input      Parse the given string as input.
    -t, --type       Specifies type of grammar: ANTLRv4, ANTLRv3, ANTLRv2, Bison, rex, pegen_v3_10
    -s, --start-rule Start rule name.
    -p, --parser     Location of pre-built parser (aka the trgen Generated/ directory)

## Examples

    trparse Java.g2
    trparse -i "1+2+3"
    trparse Foobar.g -t ANTLRv2
    echo "1+2+3" | trparse | trtree
    mkdir out; trparse MyParser.g4 MyLexer.g4 | trkleene | trsponge -o out

## Current version

0.23.9 Fixed #494, #496, #497.

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
