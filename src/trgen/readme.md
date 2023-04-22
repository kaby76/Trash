# trgen

## Summary

Generate an Antlr4 parser for a given target language

## Description

Generate a parser application using the Antlr tool and application templates.
The generated parser is placed in the directory <current-directory>/Generated/.
If there is a `pom.xml` file in the current directory, `trgen` will read
it for information on the grammar. If there is no `pom.xml` file, the start
rule must be provided. If the current directory is empty, `trgen` will
create a parser for the Arithmetic.g4 grammar.

## Usage

    trgen <options>* 

## Examples

    trgen

## Current version

0.20.15 Add performance testing to templates. Add -force option to trgen to generate a target. Dotnet 7.0 dependency; desc.xml replacement for pom.xml. Add trfoldlit. NB: not all Trash tools supported yet.
