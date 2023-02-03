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

0.19.3 Complete rewrite of parse tree representation. NB: not all Trash tools supported yet.
