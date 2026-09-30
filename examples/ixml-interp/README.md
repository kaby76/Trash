# Interpret basic iXML directly

Run `bash run-example.sh` with the current Trash tools installed. The script
generates `.interp` tables, checks exact AllStar parse trees for arithmetic and
lists, and verifies rejection of incomplete expressions and trailing input.

```sh
dotnet trash parse Arithmetic.ixml | dotnet trash interp -o interp-arithmetic
dotnet trash parse --allstar -L interp-arithmetic -i '1+23' | dotnet trash tree -a
```

Expected tree:

```text
(ixml_start (expr (expr (number 1)) + (number 2 3)) <EOF>)
```

Unlike the separate `ixml-to-antlr4` conversion example, this compiles the built-in
iXML parser's tree directly into the shared ATN model. A disjoint character
alphabet produces one lexer token per character, preserving scannerless matching
even when literals and character sets overlap. Whitespace is significant: include
it in the grammar where required.

The first declared rule is the entry rule. A synthetic `ixml_start` wrapper
(uniquely renamed if necessary) enforces EOF without changing recursive calls to
that rule. Keep the default start selection to use this full-input check; an
explicit `--start-rule` selects the named rule directly, as for other front ends.

Supported: rule references, literals (including doubled quote escapes), hexadecimal
characters, character sets/ranges and complements, alternatives, empty alternatives,
groups, `?`, `*`, `+`, and separated repetition `**` / `++`. Direct left recursion
uses the existing interpreter transformation; indirect left recursion requires the
interpreter's opt-in option.

This is a basic subset of [iXML 1.0](https://invisiblexml.org/1.0/), not a conforming
iXML XML serializer. It produces ordinary Trash rule trees. XML output marks,
insertions, empty character sets, Unicode category classes, and characters outside the non-surrogate BMP
are not supported. Unsupported grammar constructs produce diagnostics. Ambiguous
grammars follow AllStar's alternative selection, rather than enumerating parses.
