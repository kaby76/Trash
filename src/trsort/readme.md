# trsort

## Summary

Sort rules in a grammar

## Description

Reads a parse tree from stdin, move rules according to the named
operation, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

## Usage

    trsort bfs <string>
    trsort dfs <string>

## Details

Reorder the parser rules according to the specified type and start rule.
For BFS and DFS, an XPath expression must be supplied to specify all the start
rule symbols. For alphabetic reordering, all parser rules are retained, and
simply reordered alphabetically. For BFS and DFS, if the rule is unreachable
from a start node set that is specified via <string>, then the rule is dropped
from the grammar.

## Example

    trparse Java.g4 | trsort alpha | trtext
    trparse Java.g4 | trsort dfs ""//parserRuleSpec/RULE_REF[text()='libraryDefinition']"" | trtext

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.21.5 Fix trquery.
