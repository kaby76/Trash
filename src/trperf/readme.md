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
    -c. -column      String of characters denoting columnns.
                     F = File name
                     d = Decision number
                     r = Rule name
                     i = Invocations
                     T = Time
                     k = Total k
                     m = Max k
                     f = Fallbacks
                     a = Ambiguities
                     e = Errors
                     t = Transitions
                     c = Input string at max-k
           Default is "FdriTkmfaetc".
    -i, --input      String to parse.
    -s, --start-rule Start rule name.
    -p, --parser     Location of pre-built parser (aka the trgen Generated/ directory)

## Examples

    # print out performance data for a parse, ignore the header line, sort on "Max k", and output in a formatted table.
    trperf aggregate01.sql | tail -n +2 | sort -k6 -g -r | column -t

    # print out performance with header, sorting on ambiguity.
    trperf x.go -h -c aFdriTkmfaet | ( head -n 1 && tail -n +2 | sort -k1 -g -r ) | head | column -t

    # print out performance with header, sorting on ambiguity,
    # compress text column.
    dotnet trperf ../examples/AllInOneNoPreprocessor.cs -h -c aFdriTkmfaetc \
      | ( head -n 1 && tail -n +2 | sort -k1 -g -r ) \
      | head | awk '
      {
        tmp = $0
        for(i=1; i<=12; i++) sub(/^[ \t]*[^ \t]+/, "", tmp)
        sub(/^[ \t]+/, "", tmp)
        gsub(/[ \t][ \t]+/, " ", tmp)
        lasts[NR] = tmp
        lines[NR] = $0
        for(i=1; i<=12; i++) if(length($i) > w[i]) w[i] = length($i)
      }
      END {
        for(r=1; r<=NR; r++) {
          split(lines[r], f)
          for(i=1; i<=12; i++) printf "%-*s  ", w[i], f[i]
          print lasts[r]
        }
      }
    '

## Current version

2.0 Unified dispatcher for the Trash toolkit. Fix broken Cpp target on Github. Add tokens per second perf measurement. Added more perf measurements to templates.

## License

The MIT License

Copyright (c) 2025 Ken Domino

Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to 
deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, 
merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom 
the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice 
shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
