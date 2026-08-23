# Codex handoff: Trash 2.8.0

Updated: 2026-08-23

## Working scope

- Repository: `C:\msys64\home\Kenne\issues\trash-release-2.8.0`
- Branch at handoff: `trash-release-2.8.0`
- All new work must be done in this repository. Do not modify the 2.6.0 or
  2.7.0 working directories unless explicitly requested.
- The working tree was clean before this handoff file was added.

## Release 2.7.0 work completed

Release 2.7.0 is published at:

- Pull request: <https://github.com/kaby76/Trash/pull/680>
- Release: <https://github.com/kaby76/Trash/releases/tag/2.7.0>
- Tag target: `cc46513b013e037c1b93a630ebc5349433400322`

Important commits from the 2.7.0 work:

- `7545255d1` — Optimize XQuery4 node-set ordering.
- `2fe42fad4` — Test and improve ALL(*) interpreter compatibility.
- `1d9f21c93` — Handle non-greedy lexer decisions.
- `cc46513b0` — Preserve pending update order for tied targets.

### XQuery4

The 2.7.0 branch added:

- a reusable, identity-based `NodeSet` abstraction;
- a lazily built, structurally versioned document-order index;
- optimized union, intersect, except, path deduplication, and ordering;
- structural-index invalidation for tree mutations;
- stable ordering for pending updates whose targets compare equal;
- XQuery4 scaling benchmarks and expanded tests.

Issues #676 through #679 are functionally addressed by Release 2.7.0, although
they were still open on GitHub at the time of this handoff:

- <https://github.com/kaby76/Trash/issues/676>
- <https://github.com/kaby76/Trash/issues/677>
- <https://github.com/kaby76/Trash/issues/678>
- <https://github.com/kaby76/Trash/issues/679>

### Interpretive ALL(*) parser

The 2.7.0 work included:

- DFA-based SLL state caching;
- more precise SLL conflict detection;
- prediction-context merging during full-LL closure;
- corrected precedence prediction and left-recursive tree construction;
- lexer commands `pushMode`, `popMode`, `mode`, `skip`, `type`, and `channel`;
- correct handling of actions inside referenced lexer rules;
- non-greedy lexer-decision support;
- narrowly scoped parser/lexer literal-token reconciliation;
- native ANTLR C# tree comparison and performance coverage using SysML v2 and
  SystemVerilog grammars from grammars-v4.

Last focused verification performed during the 2.7.0 work:

- XQuery4: 505 tests passed.
- SystemVerilog native-tree/performance tests: 121 passed.
- SysML native-tree/performance tests: 4 passed.
- MySQL ALL(*) parsing corpus: 24 passed.

## Current 2.8.0 design topic

GitHub issue #681 was created for per-input output from a grouped parsing
pipeline:

<https://github.com/kaby76/Trash/issues/681>

The motivating generated Java command is line 51 of:

`C:\msys64\home\Kenne\issues\g4-compare-antlr-trash\abb\Generated-Java\test.sh`

```sh
echo "${files[*]}" | dotnet trash wdog java -classpath "$CLASSPATH" Test -q -x -tee -tree > parse.txt 2>&1
```

The desired Trash workflow starts from a group of filenames, parses every
file, renders one ANTLR-style tree per input, writes one error file per input,
and preserves the relative input hierarchy. For example, from the ABNF grammar
directory:

```sh
find examples -name '*.*bnf' |
  dotnet trash parse --allstar -L interp/ -x |
  dotnet trash tree -o pt-allstar
```

Given these inputs:

```text
examples/abnf.abnf
examples/iri.abnf
examples/apg-java/ABNFforSABNF.abnf
```

the intended outputs include:

```text
pt-allstar/abnf.tree
pt-allstar/iri.tree
pt-allstar/apg-java/ABNFforSABNF.tree
```

The generated testing workflow also produces corresponding `.errors` files.

## Artifact-stream direction under discussion

No implementation for #681 has started. The latest discussion favors a
general typed multi-file stream rather than independently adding output
directories to `trparse` and `trtree`, or overloading `ParsingResultSet` with
unrelated file contents.

The closest established model is a streaming POSIX tar/PAX archive. A proposed
Trash artifact bundle could contain entries such as:

```text
abnf.pt
abnf.errors
iri.pt
iri.errors
apg-java/ABNFforSABNF.pt
apg-java/ABNFforSABNF.errors
```

The conceptual pipeline is:

```sh
find examples -name '*.*bnf' |
  dotnet trash parse --allstar -L interp/ -x --bundle |
  dotnet trash tree --bundle |
  dotnet trash sponge -o pt-allstar -c
```

Responsibilities would be:

1. `trparse` produces a `.pt` artifact and an `.errors` artifact for each
   input.
2. `trtree` consumes `.pt` artifacts, replaces them with `.tree` artifacts,
   and passes `.errors` and all unknown artifact types through unchanged.
3. `trsponge` extracts all final artifacts beneath its output directory while
   preserving relative paths.

Potential format direction:

- Use an actual tar/PAX stream rather than inventing framing.
- Store parsing-result JSON as the bytes of each `.pt` member.
- Dispatch primarily by extension, with an optional PAX content-type field.
- Use `System.Formats.Tar` in .NET.
- Keep the existing raw `ParsingResultSet` JSON pipeline for compatibility;
  introduce bundle behavior explicitly at first.

Advantages of this design:

- arbitrary text and binary artifacts can coexist;
- tools transform only the artifact types they understand;
- unknown artifacts can pass through unchanged;
- relative paths travel with the data;
- the stream remains composable through stdin/stdout;
- only `trsponge` needs filesystem-output semantics.

## Decisions still required before implementation

1. Whether issue #681 should be rewritten around the general artifact-bundle
   design or whether a separate architectural issue should be created.
2. Whether to use literal POSIX tar/PAX or a Trash-specific framed format.
3. Whether bundle mode is selected with `--bundle`, detected by a stream magic
   value, or eventually becomes the default pipeline representation.
4. The exact `.pt` payload: one `ParsingResultSet`, a one-element array for
   compatibility, or another versioned representation.
5. How diagnostics are represented inside `.errors` artifacts and whether an
   empty file is always emitted for successful inputs.
6. How the common input root is selected and removed from archive member paths.
7. Collision, clobber, absolute-path, `..`, Windows separator, and extraction
   safety rules.

Do not implement #681 until these design choices are confirmed with the user.
