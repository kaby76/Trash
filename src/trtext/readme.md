# trtext

## Summary

Print a parse tree with a specific interval

## Description

Reads a tree from stdin and prints the source text. If 'line-number' is
specified, the line number range for the tree is printed.

## Usage

    trtext line-number?

## Examples

    trxgrep //lexerRuleSpec | trtext

## Current version

0.20.15 Add performance testing to templates. Add -force option to trgen to generate a target. Dotnet 7.0 dependency; desc.xml replacement for pom.xml. Add trfoldlit. NB: not all Trash tools supported yet.
