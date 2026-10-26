# SLL predicate-context merge benchmark

This copies `Nest.g4` and the `n6`, `n8`, `n10`, `n40`, and `n400` inputs from
the `foobar` reproducer. Each input is `F(F(...F(N)...)) WHEN` with *n* nested
calls; `n400.txt` intentionally has no final newline. The grammar and inputs
reproduce the problem in [ANTLR PR #4987](https://github.com/antlr/antlr4/pull/4987).

Build Trash 4.8.0 in Release mode, then run `bash test-example.sh` for a quick
AllStar tree check or `bash run-example.sh` for the C# comparison. The latter
also requires `antlr4`, Java, and NuGet access (or cached packages). It
generates a C# driver pinned to the ANTLR 4.13.1 tool and runtime. Generated
files go in the ignored `Generated-CSharp/` and `interp/` directories. The C#
SLL run of `n10.txt` is capped at five seconds so the example cannot hang.

## Why ANTLR 4.13.1 C# pure SLL grows exponentially

The two `expr` alternatives begin with distinct *semantic-predicate
instances*, even though both predicates are `{true}?` and both alternatives
otherwise match `FN LP arg RP`. In ANTLR's SLL conflict check,
`HasSLLConflictTerminatingPrediction` copies configurations while dropping
their predicate contexts. Configurations that then become equivalent require
their prediction contexts to be merged. In the 4.13.1 C# runtime this copy
calls `ATNConfigSet.Add` **without a merge cache**. Deep nesting makes the
contexts share subgraphs, but the same subgraphs are merged again for each
path. PR #4987 adds a merge cache at that call site, addressing the
exponential re-merging. Its PR description reports 291 ms at depth 6, 18.4 s
at depth 8, and over 60 s at depth 10 for the unfixed Java runtime; these
are *not* C# measurements. Locally, the unfixed C# 4.13.1 `n10.txt` pure-SLL
run exceeded this example's five-second cap.

## Why AllStar behaves differently

Trash's AllStar interpreter has its own ATN configuration sets and
prediction-context merge workspace. It does **not** perform ANTLR's
predicate-stripping copy with uncached context merges, so it does not hit the
specific exponential path in PR #4987. A comparison caveat: without XQuery
predicate hooks, AllStar treats target-language predicates such as `{true}?`
as unconditional transitions; it does not execute their C# code. Because
these predicates are literally true, that distinction does not change this
reproducer's intended alternatives, but it would matter for nonconstant
predicates. The automated test verifies that AllStar's SLL and default-LL
trees agree on `n10.txt`.

AllStar's **default** mode can still be much slower than `--sll`: SLL
conflicts cause full-context LL retries, which are needed in general to
resolve decisions using the caller context. For `n400.txt`, a local
`--parser-stats` run recorded 400 SLL conflicts and 400 full-context
fallbacks: approximately 16 million closure configurations by default,
versus 300,000 in pure SLL. The corresponding parse times were about 10.3 s
and 0.8–1.3 s on the test machine. That is repeated full-LL work, **not** the
4.13.1 predicate-copy defect; timings vary by machine. Pure SLL happened to
produce the same tree here, but that should not be assumed for other
grammars and inputs.

To see the AllStar fallback counters yourself:

```sh
dotnet ../../src/trparse/bin/Release/net10.0/trparse.dll Nest.g4 |
  dotnet ../../src/trinterp/bin/Release/net10.0/trinterp.dll |
  dotnet ../../src/trparse/bin/Release/net10.0/trparse.dll \
    --allstar --parser-stats --no-output examples/n400.txt
```
