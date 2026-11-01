# Proposal: describe assignment syntax beside a grammar

## Purpose

PLDB's [Assignment feature](https://pldb.info/features/hasAssignment) shows
examples of assignment across languages. A `.g4` grammar can tell us which
strings are accepted and how they are parsed. It cannot, by itself, establish
that a construct changes a variable's value. Rule names and `=` tokens are
useful search hints, but neither is a reliable definition of assignment.

Add a small, optional **semantic mapping file** beside a grammar. Its first
feature would identify the grammar alternatives that represent assignment,
name the target and value subtrees, and classify the syntax. The grammar
remains unchanged. The mapping would describe *syntactic roles*, not execute
the language or prove that a target is writable.

## Categories

PLDB's [feature list](https://pldb.info/lists/features) is a useful source of
candidate features beyond assignment. The groups below are **proposed for
Trash**; they are not PLDB categories. They separate features by the kind of
evidence a grammar and a mapping could supply.

| Proposed category | Examples from PLDB | Evidence Trash could provide |
| --- | --- | --- |
| Lexical forms | Comments, Strings, Integers, Hexadecimals, Multiline Strings | Lexer rules and positive/negative token examples often suffice. |
| Expressions and operators | Assignment, Bitwise Operators, Ternary Operators, Increment and Decrement Operators | Map a particular parser alternative, its operator, and operand roles. Assignment belongs here. |
| Statements and control flow | Conditionals, While Loops, Switch Statements, Exceptions | Map the construct's rule alternative and roles such as condition, body, and branches. This identifies syntax, not control-flow behavior. |
| Declarations and bindings | Functions, Constants, Classes, Structs, Enums, File Imports | Map declared names, initializers, parameters, and bodies. Keep declarations with initializers distinct from updates. |
| Type syntax | Generics, Type Aliases, Union Types | Identify the syntactic forms, while leaving type checking and inference to semantic analysis. |
| Runtime or ecosystem properties | Garbage Collection, Static Typing, Type Inference, Standard Library, Disk Output | Grammar evidence is insufficient; these require specifications, implementations, or other external evidence. |

This categorization should guide the meta language, not turn every PLDB
feature into a parse-tree pattern. A feature result should distinguish
**declared** (a reviewed mapping exists), **observed** (a positive input
matched it), **unsupported** (supported by explicit external evidence), and
**unknown** (no sufficient evidence). Missing mappings must not be reported
as proof that a language lacks a feature. An assignment mapping is the first
prototype for the expressions-and-operators category; its pattern/capture
machinery should be reusable for the other syntax-visible categories.

## Why rule-name and token searches are insufficient

- In C, `assignmentExpression` has a `conditionalExpression` alternative
  that is not an assignment. The assignment alternative contains a target,
  an operator, and another `assignmentExpression`.
- In the grammars-v4 Java grammar, `expression` contains assignment, equality,
  arithmetic, and many other alternatives. Selecting that whole rule would
  misclassify most expressions.
- Python permits annotated, augmented, unpacking, and chained assignments;
  these do not all have the same number of target or value subtrees.
- A token such as `=` may appear in a default argument, named argument,
  type alias, configuration entry, or comparison in another language.

The mapping must therefore select a **specific parse-tree shape**, not merely
a rule or an operator spelling.

## Proposed sidecar format

Use a human-readable `.sem` file. Reuse the structural patterns and captures
from Trash's existing `.ast` rewrite files, but keep the semantic mapping
separate from AST construction. The following is **proposed syntax**; no
current Trash command accepts it:

```text
semantics C {
    assignment simple
        assignmentExpression(target:unaryExpression, '=', value:assignmentExpression)
        targets(target) values(value) operator('=')
        counts-for-has-assignment;

    assignment augmented
        assignmentExpression(target:unaryExpression, op:_, value:assignmentExpression)
        where text(op) in ('*=', '/=', '%=', '+=', '-=', '<<=', '>>=', '&=', '^=', '|=')
        targets(target) values(value) operator(op)
        counts-for-has-assignment;
}
```

Each declaration has four parts:

1. **Pattern:** a rule and the ordered children of one alternative. A quoted
   token matches its spelling; `_` matches one child; captures bind children.
2. **Guard:** an optional, restricted test on captured token text or type.
   This is needed when one alternative admits several operators. It must be
   side-effect-free and must not be target-language code.
3. **Roles:** one or more targets, one or more values, and an optional operator.
   Retain source spans and the matched rule/alternative as provenance.
4. **Feature policy:** whether this form counts toward a particular catalog
   feature. An initializer and an update should remain distinguishable even
   if a catalog counts both as “assignment.”

The initial vocabulary should include `simple`, `augmented`,
`initialization`, `multiple`, `destructuring`, and `chained`. These are
syntactic categories. The format must support repeated and grouped captures
before `multiple` and `chained` can be represented honestly.

For example, the existing `examples/linvast-ast/c/c.ast` maps the C
assignment alternative to `AssignExprNode`. A `.sem` declaration would use
the same captured children but would emit a reusable `Assignment` fact. It
would not require that particular AST node name or replace the CST/AST.

## Data produced

For a matched input, emit a record such as:

```json
{
  "feature": "assignment",
  "form": "augmented",
  "operator": "+=",
  "targets": [{ "start": 10, "end": 11 }],
  "values": [{ "start": 15, "end": 20 }],
  "sourceRule": "assignmentExpression",
  "mapping": "C.sem:8"
}
```

Offsets are illustrative; the implementation should use the same input
coordinate convention as the parse result and account for source transforms.
Facts should be separate from the parse tree, so an `.ast` mapping can still
construct an AST and ordinary Trash tree tools remain usable. If emitted in
the command pipeline, facts can be another named member of the PAX/tar
bundle. A grammar-level report can list declared assignment forms and whether
validated examples exercise each one. These are different claims: a mapping
declares a feature; a corpus demonstrates observed matches.

## Discovery and validation

Trash could suggest candidate alternatives by searching the grammar tree for
assignment-like rule names, operators, or keywords. Such suggestions must
never automatically become facts. The author reviews each candidate and
writes the mapping.

At compilation, check that referenced rules and token names exist, captures
are defined, and patterns can match the grammar's alternatives. At test time,
parse positive examples and negative controls, then compare emitted facts:

| Grammar | Positive cases | Negative controls |
| --- | --- | --- |
| C | `x = y`, `x += y` | `x == y`, a bare `x` expression |
| Java | assignment expression, field initializer | equality, annotation member |
| Python | `x = y`, `x += y`, `a = b = c`, unpacking | keyword argument, default parameter |
| Lua | one target and multiple targets | equality, a function-call statement |

The first implementation should require at least one positive and one
negative example per mapping. Report unmatched or multiply matched examples;
do not silently classify an unexpected tree. Include a test where a rule name
contains “assign” but its selected alternative is not an assignment.

## Integration plan

1. **Prototype the format and matcher.** Parse `.sem` files with a built-in
   grammar, reuse `.ast` pattern-matching concepts, and emit facts from an
   already produced CST. Start with simple and augmented assignments in C
   and Java. Keep `.sem` parsing and validation independent of `.ast` rewrites.
2. **Add corpus checks.** Match positive and negative examples from C, Java,
   Python, and Lua. Add grouped/repeated capture syntax only when the Python
   and Lua cases require it.
3. **Integrate with interpreted parsing.** Once the format is stable,
   `trinterp` may compile the mapping into an optional `.interp` section and
   `trparse --allstar` may emit facts as parse events complete. Ensure this
   agrees with the initial CST-based matcher, including after source
   transforms and with embedded `.ast` mappings.
4. **Export feature summaries.** Produce a machine-readable report with
   form counts, examples, source locations, and the explicit policy used to
   derive a PLDB-style `hasAssignment` value.

## Boundaries

The proposed metadata does not decide whether an assignment is legal, what
location it writes, whether an overloaded operator calls a function, or what
value the expression returns. Macros, preprocessing, semantic predicates,
and language-specific actions may change those answers. Those require
additional language semantics. The first goal is a verifiable, shared
description of *where assignment-shaped constructs occur in parsed syntax*.

## Open decisions

- Should declaration initializers count toward `hasAssignment` by default,
  or only when an explicit `counts-for-has-assignment` marker is present?
- Should chained assignment be one fact with several targets, or one fact
  per write with a common source span?
- Should the catalog consume facts about parsed examples, declared grammar
  forms, or both? The report should keep these separate either way.
