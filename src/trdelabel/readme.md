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

0.20.22 Updates to trfoldlit, trconvert (rex, pegjs, kocmanllk but incomplete, trparse, trstrip. Add trcover. Fix trgen for Java with Java/ files. Parse tree attributes added. NB: not all Trash tools supported yet.
