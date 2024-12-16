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

0.23.11 Fix trgen star.g4 problem.

## License

The MIT License

Copyright (c) 2024 Ken Domino

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
