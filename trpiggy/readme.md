# Trpiggy

Read from stdin a parsing result set, modify the trees using a template engine, then
output the modified parsing result set. The command also reads a template to follow
as the first argument to the command. The template extends the well-known visitor
pattern used by Antlr with a template that contains strings and xpath expressions
that defines children.

# Usage

    trpiggy template-spec

# Examples

    trparse "1+2+3" | trpiggy modify.txt | trtext

# Current version

0.16.5 -- Add trperf/update templates.
