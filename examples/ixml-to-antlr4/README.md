# iXML to Antlr4 Example

Demonstrates converting an [iXML](https://invisiblexml.org/) grammar
to Antlr4 syntax using a multi-pass XQuery Update pipeline.

The example converts `ixml.ixml` — the iXML self-describing grammar from
the iXML 1.0 specification — into an Antlr4-flavoured grammar.

## Prerequisites

- `dotnet trash` installed (`dotnet tool install -g trash`)

## Run

```bash
bash run-example.sh
```

The final grammar is written to `xxx/ixml.ixml`.

## How it works

1. `dotnet trash parse -t ixml ixml.ixml` parses the iXML grammar file
   using the built-in ixml parser and emits a JSON parse tree (`ixml.pt`).

2. **Pass 1** (`ixml-to-antlr4.xq`) — structural syntax transforms:
   - Removes rule-head marks (`@`, `^`, `-`) — these control iXML output
     serialisation and have no Antlr4 equivalent.
   - Removes marks from nonterminal references and tmarks from terminals.
   - Replaces the iXML sequence separator `,` with nothing (Antlr4
     juxtaposes items).
   - Replaces iXML alternative separators `;`/`|` with Antlr4 `|`
     (only between alternatives, not inside character sets).
   - Normalises the assignment operator: iXML allows `=` as well as `:`;
     Antlr4 uses `:` only.
   - Replaces the rule-terminating `.` with Antlr4 `;`.

3. **Pass 2** (`encoded-to-antlr4.xq`) — encoded character references:
   - Converts iXML `#hex` references to Antlr4 `'\uXXXX'` escapes in
     rule bodies and to `\uXXXX` (unquoted) inside character-class
     brackets.  Hex digits are zero-padded to four places.

4. **Pass 3** (`charset-to-antlr4.xq`) — character-set member syntax:
   - Strips the surrounding quotes from single-character string members
     and range bounds inside `[...]`: e.g. `["a"-"z"]` → `[a-z]`.
   - Removes the iXML `|`/`;` separator between charset members — Antlr4
     juxtaposes them.
   - Rewrites Unicode category codes to Antlr4 `\p{XX}` syntax:
     e.g. `[L]` → `[\p{L}]`, `[Zs]` → `[\p{Zs}]`.

5. **Pass 4** (`sep-repeat-to-antlr4.xq`) — separator quantifiers:
   - `f ++ sep` (one-or-more with separator) → `f (sep f)*`
   - `f ** sep` (zero-or-more with separator) → `(f (sep f)*)?`
   - Uses `string()` to capture factor and separator text after prior
     passes.  Works reliably when the separator is a simple nonterminal
     (e.g. `rule++RS`).  Complex separators such as `(-",", s)` produce
     approximate output that needs manual review.

## Limitations

The output is a structural approximation of an Antlr4 grammar, not a
drop-in replacement.  Several aspects require manual follow-up:

- **Parser/lexer split**: iXML does not distinguish parser and lexer
  rules.  The output must be manually divided into a parser grammar
  (lowercase rule names) and a lexer grammar (uppercase rule names, or
  rules that match single characters/tokens directly).
- **Whitespace and comments**: iXML treats whitespace and comments as
  optional spacing inside rules.  In Antlr4 these are typically handled
  by a `WS`/`COMMENT` lexer rule that routes tokens to a hidden channel.
- **Complex separator expressions**: Separator quantifiers whose
  separator is a grouped expression (e.g. `term**(-",",s)`) produce
  approximate text that needs manual cleanup.
- **iXML insertion syntax** (`+`): The `insertion` rule (e.g. `+"text"`)
  has no Antlr4 equivalent and must be removed or rewritten by hand.
