# trxquery

Apply XQuery 4.0 Update expressions to a Trash parse tree.

## Synopsis

    ... | dotnet trash xquery '<xquery-expression>'
    ... | dotnet trash xquery -q <query-file>

## Description

`trxquery` reads a JSON parse tree (`ParsingResultSet[]`) from stdin (or a file
via `-f`), evaluates the given XQuery 4.0 expression with the parse tree as the
context document, and emits the (possibly mutated) parse tree on stdout.

Mutations are applied using the XQuery Update Facility (insert/delete/replace/rename)
directly on the underlying parse tree nodes via `TreeEdits`, so the result can be
piped to other Trash tools (e.g. `dotnet trash text`).

## XQuery Update syntax

Delete a node:

    delete node //ruleName

Insert a text node before a matched node:

    insert node "text" before //ruleName

Insert an element after a matched node:

    insert node <foo/> after //ruleName

Replace the text value of a node:

    replace value of node //ruleName with "new-text"

Replace a node:

    replace node //ruleName with <bar/>

Rename an element:

    rename node //ruleName as "newName"

## External variables (query parameters)

XQuery files can declare external variables using standard XQuery syntax:

    declare variable $name external;

Positional arguments after `--query <file>` are bound to declared external
variables in declaration order:

    dotnet trash parse A.g4 | dotnet trash xquery --query transform.xq 'value1' 'value2'

Given a query file with:

    declare variable $first external;
    declare variable $second external;

the call above binds `$first = "value1"` and `$second = "value2"`.  Any number
of positional arguments can be passed; each maps to the next external variable
in declaration order.  Extra arguments beyond the number of declared external
variables are ignored, and extra declarations beyond the number of supplied
arguments are left unbound (which will produce an error if the query uses them).

## Options

    -q, --query   File containing the XQuery expression.
    -f, --file    Read parse tree data from file instead of stdin.
    --fmt         Output formatted (indented) JSON.
    -v, --verbose Verbose stderr output.
    --version     Print version.

## Examples

Delete all `WS` tokens from a parse tree:

    dotnet trash parse Foo.g4 | dotnet trash xquery 'delete node //WS'

Replace the text of the first `ID` token:

    dotnet trash parse Foo.g4 | dotnet trash xquery 'replace value of node (//ID)[1] with "myNewName"'

Expand grouped alternatives in rule `a` using an external variable:

    dotnet trash parse A.g4 | dotnet trash xquery --query ungroup-rule.xq 'a'
