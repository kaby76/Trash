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

0.18.2 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
