# trsort

## Summary

Sort rules in a grammar

## Description

Reads a parse tree from stdin, move rules according to the named
operation, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

## Usage

    dotnet trash sort [-a]
    dotnet trash sort --bfs [<start-rule>]
    dotnet trash sort --dfs [<start-rule>]

## Details

Reorder the parser rules according to the specified mode.

`-a` / `--alphabetic` (default when no mode flag is given): sort all parser
rules alphabetically. All rules are retained.

`--bfs [<start-rule>]`: sort reachable parser rules in breadth-first order
from the named start rule. Rules not reachable from the start rule are dropped.
If `<start-rule>` is omitted, the start rule is auto-detected by finding the
parser rule whose alternative contains an `EOF` token; an error is reported if
zero or more than one such rule exists.

`--dfs [<start-rule>]`: same as `--bfs` but uses depth-first (preorder)
traversal.

Only one mode flag may be specified at a time.

## Example

    dotnet trash parse Java.g4 | dotnet trash sort -a | dotnet trash sponge -c
    dotnet trash parse Java.g4 | dotnet trash sort --dfs compilationUnit | dotnet trash sponge -c
    dotnet trash parse Java.g4 | dotnet trash sort --bfs | dotnet trash sponge -c

## Current version

Release 2.4.0.

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
