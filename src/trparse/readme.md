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
depending on the extension of the file name.  The recognized extensions are
listed in the [Supported grammars](#supported-grammars) table below.

You can force the type of parse with the `--type` command-line option.
Accepted values:

* `ANTLRv4` — ANTLRv4 (`.g4`)
* `ANTLRv3` — ANTLRv3 (`.g3`)
* `ANTLRv2` — ANTLRv2 (`.g2`)
* `Bison` — Bison/Yacc (`.y`)
* `Lark` — Lark (`.lark`)
* `rex` — Rex (`.rex`)
* `LBNF` — LBNF/BNF Converter (`.cf`)
* `W3CEBNF` — W3C EBNF (`.ebnf`)
* `Iso14977` — ISO 14977 EBNF (`.iso14977`, `.iso`)
* `ABNF` — IETF ABNF (`.abnf`)
* `Javacc` — JavaCC (`.jj`, `.jjt`)
* `Pegjs` — PEG.js (`.pegjs`)
* `Pest` — Pest (`.pest`)
* `Xtext` — Xtext (`.xtext`)
* `Grammophone` — Grammophone
* `Princeton` — Princeton BNF
* `pegen_v3_10` — Python pegen (`.peg`)

## Supported grammars

| Grammar | File suffix | `--type` value |
|---------|-------------|----------------|
| ANTLRv4 | `.g4` | `ANTLRv4` |
| ANTLRv3 | `.g3` | `ANTLRv3` |
| ANTLRv2 | `.g2` | `ANTLRv2` |
| Bison/Yacc | `.y` | `Bison` |
| Lark | `.lark` | `Lark` |
| Rex | `.rex` | `rex` |
| LBNF (BNF Converter) | `.cf` | `LBNF` |
| W3C EBNF | `.ebnf` | `W3CEBNF` |
| ISO 14977 EBNF | `.iso14977`, `.iso` | `Iso14977` |
| IETF ABNF | `.abnf` | `ABNF` |
| JavaCC | `.jj`, `.jjt` | `Javacc` |
| PEG.js | `.pegjs` | `Pegjs` |
| Pest | `.pest` | `Pest` |
| Xtext | `.xtext` | `Xtext` |
| Grammophone | — | `Grammophone` |
| Princeton BNF | — | `Princeton` |
| Python pegen | `.peg` | `pegen_v3_10` |

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

Release 3.1.0.

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
# Artifact bundles

The default output is an ordinary POSIX PAX/tar stream. Each parsed input
produces one `.pt` member containing a single
`ParsingResultSet` JSON object and one UTF-8 `.errors` member. Successful inputs
have an empty `.errors` member. Relative hierarchy below the inputs' common
directory is preserved. Use `--base-directory DIR` to select the stripped root
explicitly; inputs outside it are rejected.
