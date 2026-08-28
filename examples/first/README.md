# FIRST sets with XQuery4

Demonstrates computing grammar-theoretic FIRST sets for the parser rules in an
ANTLR4 grammar using XQuery4.

The query first computes nullable rules and then computes FIRST facts to a
fixed point. This supports alternatives, sequences, optional and repeated
elements, grouped blocks, direct or mutual rule cycles, and nullable rules.

## Grammar

`First.g4` deliberately includes:

- an optional prefix and a nullable rule;
- mutually recursive `recursiveA` and `recursiveB` rules;
- a grouped terminal alternative;
- a negated token set and a wildcard; and
- a repeated terminator after the initial choice.

Negated sets and wildcards are reported symbolically because expanding them
requires the complete lexer vocabulary.

## Running

```sh
bash run-example.sh
```

Or run the pipeline directly:

```sh
dotnet trash parse First.g4 \
  | dotnet trash xquery --query first.xq start
```

The external argument `start` selects the start rule. The output contains the
FIRST set for that rule and every parser rule reachable from it. Nullable rules
include `ε`.

The output is:

```text
choice -> {'(', 'after', 'blue', 'maybe', 'red', ., ID, NUMBER, ~('x' | 'y')}; nullablePrefix -> {'maybe', ε}; prefix -> {'prefix'}; recursiveA -> {'(', ID, NUMBER}; recursiveB -> {'(', NUMBER}; start -> {'(', 'after', 'blue', 'maybe', 'prefix', 'red', ., ID, NUMBER, ~('x' | 'y')}; terminator -> {';'}
```
