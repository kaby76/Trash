# ungroup

Demonstrates replacing a grouped alternative `(X | Y)` inside a parser rule
with fully distributed top-level alternatives, using an XQuery Update script
with an external variable parameter.

## Grammar

`A.g4` contains a rule with a plain grouped alternative:

```antlr
grammar A;
a : (a | 'X') 'B' 'Z' | 'C' ;
```

## Transform

### ungroup-rule.xq

Takes a single external variable `$ruleName` — the name of the rule to
transform — and expands the first plain group (no `*`, `+`, or `?` suffix)
found in that rule.

Each inner alternative of the group becomes a separate top-level alternative,
with the elements before and after the group prepended / appended:

```
a : (X | Y) B C | D ;
          ↓
a : X B C | Y B C | D ;
```

Run the query multiple times to expand further groups in the same rule or
other rules.

## Expected output

```antlr
grammar A;
a : a 'B' 'Z' | 'X' 'B' 'Z' | 'C' ;
```

## Running

```sh
bash run-example.sh
```

Or step by step:

```sh
dotnet trash parse A.g4 \
  | dotnet trash xquery --query ungroup-rule.xq 'a' \
  | dotnet trash sponge -c -o ungrouped
```

## Passing parameters

The `--query` file uses the standard XQuery declaration:

```xquery
declare variable $ruleName external;
```

Positional arguments after `--query <file>` are bound to external variables in
declaration order.  For a query with two external variables:

```sh
dotnet trash xquery --query transform.xq 'first' 'second'
```
