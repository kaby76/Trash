# trsponge

## Summary

Extract parsing results output of Trash command into files

## Description

Read the parse tree data from stdin and write the
results to file(s).

## Usage

    trsponge <options>

## Example

    trparse Arithmetic.g4 | trsplit | trsponge

## Current version

0.21.15 Add trglob for platform-independent "find" for testing. Add Antlr4ng target, a new Antlr4 tool and TypeScript runtime.
