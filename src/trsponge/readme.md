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

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
