# trperf

## Summary

Perform performance analysis of an Antlr grammar parse

## Description

Parse files and output to stdout parse tree data using
performance analysis turned on.
The tool requires a pre-built parser via trgen for a grammar
for anything other than the standard parser grammars that
are supported. To specify the grammar, you can either
be in a trgen-generated parser directory, or use the -p option.

## Usage
    
    trperf (<string> | <options>)*
    -i, --input      String to parse.
    -s, --start-rule Start rule name.
    -p, --parser     Location of pre-built parser (aka the trgen Generated/ directory)

## Examples

    # print out performance data for a parse, ignore the header line, sort on "Max k", and output in a formatted table.
    trperf aggregate01.sql | tail -n +2 | sort -k6 -n -r | column -t

## Current version

0.18.0 Fix 134 for all tools. Fix 180, string-length() and math operations in XPath engine. Fix for crash in https://github.com/antlr/grammars-v4/issues/2818 where _grammar-test was removed, but pom.xml not completely cleaned up of the reference to the directory. Fix Globbing package because of conflict with Antlr4BuildTasks. Update Antlr4BuildTasks version. Rename TreeEdits.Delete() so there is no confuson that it modifies entire parse tree, token and char streams. Add -R option for rename map as file in trrename. Update trrename to use xpath and TreeEdits. Add methods to TreeEdits. Rewrote trrename to use xpath/treeedits packages.
