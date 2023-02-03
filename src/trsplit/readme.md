# trsplit

## Summary

Split a combined Antlr4 grammar

## Description

The split command splits one grammar into two. The input grammar
must be a combined lexer/parser grammar implemented in one file.
The transformation creates a lexer grammar and a parser grammar,
outputed as parse tree data with the two grammars.
[trsponge](https://github.com/kaby76/Domemtech.Trash/tree/main/trsponge)
is used to instantiate the two files in the file system.

## Usage

    trsplit

## Details

The `trsplit` application splits a combined grammar into two files.
It does this as follows:

* Partition the rules in the grammar into parser and lexer rules. This
is done by examining the LHS symbol: parser rules start with a lowercase
LHS symbol name; lexer rules start with an uppercase LHS symbol name.
* In the parser grammar, insert an `optionsSpec` declaration that
contains a `tokenVocab` specification for the name of the vocabulary
generated for the lexer grammar.
* Add `grammarDecl` statements to the top of the new files to declare
the parser and lexer grammars.

After splitting, use `trsponge` to output the files. The resulting files
may require hand-tweaking due to various constraints that split grammars
must follow, including:

* String literals that do not have a corresponding lexer rule must be
modified.
* Parser options do not apply to lexer grammars. Remove or replace.

## Example

    trparse Arithmetic.g4 | trsplit | trsponge

## Current version

0.19.3 Complete rewrite of parse tree representation. NB: not all Trash tools supported yet.
