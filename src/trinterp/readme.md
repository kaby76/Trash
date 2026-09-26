# trinterp

## Summary

Generate ANTLR4 `.interp` files from a grammar parse tree

## Description

Reads ANTLRv4 or G4Plus grammar parse trees from stdin (as produced by `dotnet trash parse`) and
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

    dotnet trash parse CLexer.g4 CParser.g4 | dotnet trash interp -o out/
    dotnet trash parse Heavy.g4 | dotnet trash interp --actions-in-interp -o out/

## G4Plus interpretation

    dotnet trash parse Lexer.g4p Parser.g4p | dotnet trash interp -o interp
    dotnet trash parse --allstar -L interp input.txt

G4Plus `.g4p` and `.g4+` grammars use the same ATN serialization as ANTLRv4.
Rule names are case-neutral: lexer grammar declarations define lexer rules;
parser and combined grammar declarations define parser rules. Combined grammars
generate an implicit lexer for string literals, without reclassifying uppercase
rule names. For whitespace handling or other named tokens, supply a separate
lexer grammar and select it with `options { tokenVocab=Lexer; }`. A combined
G4Plus grammar with `tokenVocab` uses that lexer instead of an implicit lexer.

Vocabularies are bound to the selected lexer in the input batch. If that lexer
is absent, a `.tokens` file beside the grammar source is used. A parser without
`tokenVocab` may use the sole lexer in a batch; multiple lexers require explicit
selection. Undefined and ambiguous G4Plus symbols are errors. `--start-rule`
overrides discovery of the single rule explicitly referencing EOF.

This implementation supports alternatives, references, literals, lexer character
sets/ranges, repetitions (including nongreedy repetitions), modes, lexer commands,
and the existing left-recursion transformation. It preserves actions/predicates
for the existing serialization machinery; it does not add target-language action
or predicate execution. The `more` command is serialized, but the existing
interpreter lexer still rejects execution of that action. Named rule references
inside lexer character sets also require future set expansion. Set difference,
imports, and character sets/ranges in
parser rules (which require scannerless compilation) currently produce explicit
errors. Parsing those constructs as grammar syntax does not imply they can yet
be compiled into an ANTLR ATN.

The backend owns `GrammarNode` rule bodies instead of DOM nodes. Input front ends
lower to a common block/alternative/element representation before vocabulary
binding and ATN construction. Future EBNF front ends can feed that representation
without changing the serializer or duplicating the ATN builders.

## Current version

Release 3.7.0.

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
