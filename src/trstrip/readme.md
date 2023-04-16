# trstrip

## Summary

Strip a grammar of all actions, labels, etc.

## Description

Read the parse tree data from stdin and strip the grammar
of all comments, labels, and action blocks.

## Usage

    trstrip

## Examples

    trparse A.g4 | trstrip

## Current version

0.20.14 Add performance testing to templates. Add -force option to trgen to generate a target. Dotnet 7.0 dependency; desc.xml replacement for pom.xml. Add trfoldlit. NB: not all Trash tools supported yet.
