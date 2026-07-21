# trinterp

## Summary

Generate ANTLR4 `.interp` files from a grammar parse tree

## Description

Reads an Antlr4 grammar parse tree from stdin (as produced by `trparse`) and
writes `.interp` and `.tokens` files to the output directory. Supports both
lexer and parser grammars, as well as combined grammars (which produce a lexer
and parser `.interp` pair).

When `--actions-in-interp` is specified, grammar actions and semantic predicates
are appended as strings to the `.interp` file so that interpreter drivers can
consume them without needing the generated target-language source.

## Usage

    trinterp [options]

## Options

    -o, --output-directory  Output directory (default: current directory)
    -f, --file              Read parse tree from file instead of stdin
    --actions-in-interp     Append actions and predicates as strings to .interp
    -v, --verbose           Verbose output

## Examples

    trparse CLexer.g4 CParser.g4 | trinterp -o out/
    trparse Heavy.g4 | trinterp --actions-in-interp -o out/

## Current version

1.0.0 Initial implementation.

## License

The MIT License

Copyright (c) 2025 Ken Domino

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
