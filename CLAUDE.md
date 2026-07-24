# Trash project notes for Claude

## CLI invocation

The Trash toolchain is a single dotnet tool called `trash`. Subcommands are dispatched via `dotnet trash <subcommand>`.

- Parse grammars: `dotnet trash parse <file.g4> ...`  (NOT a separate `trparse` binary)
- Generate interp files: `dotnet trash interp [options]`
- Full pipeline example:
  ```
  dotnet trash parse grammar.g4 | dotnet trash interp --atn -o outdir
  ```

There is no standalone `trparse` executable.
