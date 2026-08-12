# ABNF Interp Example

Demonstrates generating Antlr `.interp` files from the
[ABNF grammar](https://github.com/antlr/grammars-v4/tree/master/abnf)
and using them to parse ABNF/BNF files without a code-generation step.

Grammar and example files sourced from
[antlr/grammars-v4](https://github.com/antlr/grammars-v4).

## Prerequisites

- `dotnet trash` installed (`dotnet tool install -g trash`)

## Run

```bash
bash run.sh
```

## How it works

1. `dotnet trash parse Abnf.g4` parses the grammar and emits a JSON
   representation of its ATN/parse-tree data.
2. `dotnet trash interp -o interp/` reads that output and writes `.interp`
   and `.tokens` files into `interp/`.
3. `dotnet trash parse --lib interp/` uses those files to parse each example
   input with an Earley ATN-based interpreter — no generated code required.
