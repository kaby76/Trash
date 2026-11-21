# trgen

## Summary

Generate an Antlr4 parser for a given target language

## Description

Generate a parser application using the Antlr tool and application templates.
The generated parser is placed in the directory <current-directory>/Generated-<target>/.
If there is a `desc.xml` file in the current directory, `trgen` will read
it for information on testing the grammar. If there is no `desc.xml` file,
the start rule must be provided. If the current directory is empty, `trgen` will
create a parser for the Arithmetic.g4 grammar.

## Usage

    dotnet trash gen <options>*

## Examples

    dotnet trash gen

### CongoCC target

Use `dotnet trash gen -t CongoCC` for a first-pass Java parser application
built with [CongoCC](https://github.com/congo-cc/congo-parser-generator).
The generated `build.sh` converts a combined `.g4` grammar to `.ccc` with
`dotnet trash parse | dotnet trash xquery`, runs CongoCC, and compiles its
Java output. Set `CONGOCC_JAR` if the generator JAR is not at
`$HOME/congo-parser-generator/bin/congocc.jar`. The generated driver accepts
`--input`/`-i`, file names, `-x`, `--per-file`, and `--no-output`/`-q`.

The initial converter supports named tokens, string literals, character sets
and ranges, lexer fragments, `-> skip`, parser references, grouped
alternatives, repetition, and simple direct left recursion. The left-recursion rewrite does not preserve ANTLR4
precedence or associativity, and alternative prediction is not yet translated
to CongoCC `SCAN`. See [`examples/words-congocc`](../../examples/words-congocc/)
and [`examples/arithmetic-congocc`](../../examples/arithmetic-congocc/).
If a grammar has a `desc.xml` that excludes CongoCC, pass `--force` with
`-t CongoCC`.

### Galop target

Use `dotnet trash gen -t Galop` to generate a native Go parser application
with [Galop](https://github.com/martianoff/galop). Run `bash build.sh` or
`pwsh ./build.ps1` in `Generated-Galop` after installing `galop` and Go.
Set `GALOP` to the path of the Galop executable if it is not on `PATH`.
The driver accepts `-x` for newline-separated filenames on stdin,
`-input` for literal input, `-tree`, `-tokens`, `-tee`, `-o`, `-q`,
`--per-file`, and `--perf` like the Go target. Its parse time includes
lexing. Galop currently requires a combined grammar for this driver;
imports and `tokenVocab` are not yet supported by Galop.

## Specification of desc.xml ([schema](https://github.com/kaby76/Trash/blob/main/desc.xsd))

The desc.xml is an XML file that specifies how to generate a driver and test
a grammar. The desc.xml is similar to the Antlr Maven Tester pom.xml file,
but with a few extension. trgen analyzes the grammar to determine how to test
the grammar; this analysis simplifies the information needed in the desc.xml
that would be required in the pom.xml.

A typical desc.xml simply specifies all targets to test.

```
<?xml version="1.0" encoding="UTF-8" ?>
<desc xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="../scripts/desc.xsd">
   <targets>Antlr4cs;Antlr4ng;Cpp;CSharp;Dart;Go;Java;JavaScript;PHP;Python3;TypeScript</targets>
</desc>
```
`dotnet trash gen` will read all .g4's contained in the directory with the desc.xml file.
It computes a dependency graph of the grammars used via `tokenVocab` options,
and `import` statements. The tool will then perform a topological sort, determine
the first "top-level" parser or combined grammar, and use that grammar for testing.
The start rule for the driver for testing will be determined by looking for the
first EOF-terminated rule. With these two analyses, the desc.xml eliminates the
need to specify information that is required in the pom.xml, and helps to greatly
simplify and eliminate bugs created when adding new grammars.


## Output measurements

The generated driver writes only total elapsed time (`TT`) to stderr by default.
Use `--perf` for the complete aggregate summary and `--per-file` for one
performance line per input file. The options are independent and may be combined.

### Per-file output

With `--per-file`, each parsed file produces one line in the form:

    <target> <index> <filename> <result> <time> s <tokens> tokens <pr> pr

| Field | Description |
|---|---|
| `<target>` | Target language (e.g. `CSharp`, `Java`, `Go`, …) |
| `<index>` | Zero-based index of this file in the input list |
| `<filename>` | Path to the input file |
| `<result>` | `success` or `fail` |
| `<time>` | Parse-only wall-clock time in seconds (excludes lexer/parser construction and I/O) |
| `<tokens>` | Number of tokens in the token stream |
| `<pr>` | Tokens per second for this file (`tokens / time`) |

### Summary output

With `--perf`, six summary lines are printed after all files are parsed:

| Label | Description |
|---|---|
| `PT` | **Parse time** — sum of the parse-only time across all input files |
| `OT` | **Overhead time** — `TT − PT`; time spent on file I/O, lexer/parser construction, and post-parse work |
| `TT` | **Total time** — overall wall-clock time for the entire run |
| `PR` | **Parse rate** in tokens per second — `total tokens / PT`; pure parse throughput across all files |
| `Post-warmup PR` | Parse rate computed from files 2…N only, excluding the first (warm-up) run; `n.a.` if only one file was parsed |
| `Post-warmup speed up` | Ratio of Post-warmup PR to first-file PR, showing the JIT/runtime warm-up benefit; `n.a.` if only one file was parsed |

## Current version

Release 5.0.0.

## Ignore file

`trgen` looks for `.trgen-ignore` in the current directory and then in each
parent directory up to the filesystem root. The closest file is used. Patterns
specified with `--ignore` are added to the patterns read from that file.

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
