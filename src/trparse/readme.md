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

* `ANTLRv4` for ANTLRv4
* `ANTLRv3` for ANTLRv3
* `ANTLRv2` for ANTLRv2
* `Bison` for Bison
* `rex` for Rex
* `pegen_v3_10` for the `Generated/` parser

## Earley ATN-based parsing (interp files)

As an alternative to a pre-built generated parser, `trparse` can parse input
directly from the `.interp` files produced by `trinterp`, using a from-scratch
Earley parser over the serialized ATN.  No generated C# code and no
`Antlr4.Runtime.Standard` deserialization are involved.

Provide both the parser and lexer interp files:

    dotnet trash parse --pinterp abbParser.interp --linterp abbLexer.interp input.abb

The output is the same `ParsingResultSet` JSON format as all other `trparse`
modes, so every downstream Trash Toolkit tool (`trtree`, `trxgrep`, etc.)
works without modification.

### How it works

1. **InterpFileReader** parses the `.interp` text into token names, rule names,
   channel/mode names, and the raw ATN integer array.
2. **AtnDeserializer** converts the integer array into `MyATN` / `MyATNState` /
   `MyTransition` structures (custom types in `EarleyAtnParser/`, no Antlr4
   runtime dependency).
3. **LexerAtnSimulator** runs a longest-match NFA simulation over the input
   characters to produce a full token stream (all channels).
4. **EarleyParser** runs the Earley algorithm directly over the parser ATN,
   producing a complete single-derivation parse tree
   (`ParserRuleContext`-compatible).
5. **InterpRunner** wraps the pipeline and converts the result to a
   `ParsingResultSet` compatible with the rest of the toolkit.

## Usage

    dotnet trash parse (<string> | <options>)*
    -i, --input        Parse the given string as input.
    -t, --type         Specifies type of grammar: ANTLRv4, ANTLRv3, ANTLRv2, Bison, rex, pegen_v3_10
    -p, --parser       Location of pre-built parser (aka the trgen Generated/ directory)
        --pinterp      Path to parser .interp file (Earley ATN-based parsing).
        --linterp      Path to lexer .interp file  (Earley ATN-based parsing).

## Examples

    dotnet trash parse Java.g2
    dotnet trash parse -i "1+2+3"
    dotnet trash parse Foobar.g -t ANTLRv2
    echo "1+2+3" | dotnet trash parse | dotnet trash tree
    mkdir out; dotnet trash parse MyParser.g4 MyLexer.g4 | dotnet trash sponge -o out

    # Earley interp-based parse (no generated code needed)
    dotnet trash parse abb.g4 | dotnet trash interp -o out/
    dotnet trash parse --pinterp out/abbParser.interp --linterp out/abbLexer.interp input.abb | dotnet trash tree

## Current version

2.0 Unified dispatcher for the Trash toolkit. Fix broken Cpp target on Github. Add tokens per second perf measurement. Added more perf measurements to templates. Added Earley ATN-based parsing via --pinterp / --linterp.

## License

The MIT License

Copyright (c) 2026 Ken Domino

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
