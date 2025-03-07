# trdot

## Summary

Print a parse tree in Graphvis Dot format

## Description

Reads a tree from stdin and prints the tree as a Dot graph.

## Usage

    trdot

## Details

`trdot` reads parse tree data via stdin and outputs
a Dot graph specification. The stdout can be redirected to
save the output to a file. Or, you can copy the output and
use an online Dot graph visualizer to make a plot.
Any parse tree data can be converted to Dot, include a
parse of a grammar, the parse tree of a simple expression grammar,
or a list of parse tree nodes obtained via
[trquery](https://github.com/kaby76/Trash/tree/main/trquery).

## Examples

Consider the Expression grammar, obtained via

    mkdir foo; cd foo; trgen; cd Generated; dotnet build

Let's parse the expression "1+2" and print the parse tree as a Dot graph:

    trparse -i "1+2" | trdot

The output will be:

    digraph G {
    Node18643596 [label="file_"];
    Node33574638 [label="expression"];
    Node33736294 [label="expression"];
    Node35191196 [label="atom"];
    Node48285313 [label="scientific"];
    Node31914638 [label="1"];
    Node18796293 [label="+"];
    Node34948909 [label="expression"];
    Node46104728 [label="atom"];
    Node12289376 [label="scientific"];
    Node43495525 [label="2"];
    Node55915408 [label="EOF"];
    Node18643596 -> Node33574638;
    Node18643596 -> Node55915408;
    Node33574638 -> Node33736294;
    Node33574638 -> Node18796293;
    Node33574638 -> Node34948909;
    Node34948909 -> Node46104728;
    Node46104728 -> Node12289376;
    Node12289376 -> Node43495525;
    Node33736294 -> Node35191196;
    Node35191196 -> Node48285313;
    Node48285313 -> Node31914638;
    }

## Current version

0.23.17 Fix trgen Cpp target.

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
