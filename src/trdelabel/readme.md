# trdelabel

## Summary

Remove labels from an Antlr4 grammar

## Description

Remove all labels from an Antlr4 grammar.

    "expr : lhs=expr (PLUS | MINUS) rhs=expr # foobar1 ....." => "expr : expr (PLUS | MINUS) expr ....."

## Usage

    trdelabel

## Example

    trparse A.g4 | delabel | trprint

## Current version

0.20.12 Add -force option to trgen to generate a target. Dotnet 7.0 dependency; desc.xml replacement for pom.xml. NB: not all Trash tools supported yet.
