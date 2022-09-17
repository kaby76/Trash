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

0.17.1 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
