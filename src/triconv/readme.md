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

0.21.3 Fix trcover.
