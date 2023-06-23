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

0.20.28 Update run.sh in templates. NB: not all Trash tools supported yet.
