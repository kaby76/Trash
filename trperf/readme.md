# Trperf

Parse files and output to stdout parse tree data using
performance analysis turned on.
The tool requires a pre-built parser via trgen for a grammar
for anything other than the standard parser grammars that
are supported. To specify the grammar, you can either
be in a trgen-generated parser directory, or use the -p option.

# Usage
    
    trparse (<string> | <options>)*
    -s, --start-rule Start rule name.
    -p, --parser     Location of pre-built parser (aka the trgen Generated/ directory)

# Examples

    # print out performance data for a parse, ignore the header line, sort on "Max k", and output in a formatted table.
    trperf aggregate01.sql | tail -n +2 | sort -k6 -n -r | column -t

# Current version

0.16.5 -- Add trperf/update templates.
