# triconv

## Summary

Test whether a file is proper Unicode.

## Description

Read a file and use the System.Text package to convert and test
unicode.

## Usage

    triconv [-f ENCODING] [-t ENCODING] [INPUTFILE...]

## Examples

    triconf -f utf-8 file.txt > /dev/null
    echo $?

## Current version

0.20.16 Add performance testing to templates. Add -force option to trgen to generate a target. Dotnet 7.0 dependency; desc.xml replacement for pom.xml. Add trfoldlit. NB: not all Trash tools supported yet.
