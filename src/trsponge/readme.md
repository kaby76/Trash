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

0.20.0 Complete rewrite of parse tree representation. NB: not all Trash tools supported yet.
