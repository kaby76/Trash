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

0.20.18 Updates to trfoldlit, trconvert (rex, pegjs, kocmanllk but incomplete, trparse, trstrip. Add trcover. NB: not all Trash tools supported yet.
