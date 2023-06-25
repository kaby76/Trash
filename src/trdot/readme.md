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
[trxgrep](https://github.com/kaby76/Domemtech.Trash/tree/main/trxgrep).

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

0.21.0 Fixes to trgen for Cpp target. Update run.sh in templates.
