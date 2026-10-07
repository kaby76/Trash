# trrename

## Summary

Rename symbols in a grammar

## Description

Rename symbols in an ANTLR4 grammar using the XQuery4 update engine.

## Usage

    dotnet trash parse Expression.g4 | dotnet trash rename -r 'e,exp;a,atom' | dotnet trash text --text

## Details

`trrename` selects grammar symbol nodes with an XQuery4 expression and replaces
their values using XQuery4 updates. By default it selects `RULE_REF` and
`TOKEN_REF` nodes under parser and lexer rules, including definitions and
references. `-e EXPR` supplies a different selection expression.

`-r` accepts semicolon-delimited `oldName,newName` pairs, for example
`'id,identifier;name,name_'`. Quote the value in Bash because it contains
semicolons. Positional pairs remain supported for compatibility. Alternatively,
use `-R FILE` to read comma-separated pairs from lines in a file. `-r` takes
precedence over positional pairs, which take precedence over `-R`.
All renames use original symbol spellings, so `a,b;b,c` does not rename an
original `a` twice. With no map entries, the input bundle passes through.
Unrelated bundle members such as `.errors` are preserved.

## Examples

    dotnet trash parse Foobar.g4 | dotnet trash rename -r 'a,b;c,d' | dotnet trash text --text > new-grammar.g4

Run the regression cases with `bash tests/trrename/test.sh` from the repository
root after building `trparse`, `trrename`, and `trtext` in Release configuration.

## Current version

Release 4.1.0.

## License

The MIT License

Copyright (c) 2026 Ken Domino

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
