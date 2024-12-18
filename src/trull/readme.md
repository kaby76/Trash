# trull

## Summary

Transform a grammar with upper- and lowercase string literals

## Description

The ulliteral command applies the upper- and lowercase string literal transform
to a collection of terminal nodes in the parse tree, which is identified with the supplied
xpath expression. If the xpath expression is not given, the transform is applied to the
whole file.

## Usage

    trull <xpath>?

## Examples

Before:

    grammar KeywordFun;
    a : 'abc';
    b : 'def';
    A : 'abc';
    B : 'def';
    C : 'uvw' 'xyz'?;
    D : 'uvw' 'xyz'+;

Command:

    trparse KeywordFun.g4 | trull "//lexerRuleSpec[TOKEN_REF/text() = 'A']//STRING_LITERAL" | trprint

After:

    grammar KeywordFun;
    a : 'abc';
    b : 'def';
    A :  [aA] [bB] [cC];
    B : 'def';
    C : 'uvw' 'xyz'?;
    D : 'uvw' 'xyz'+;

Command:

    trparse KeywordFun.g4 | trull | trprint

After:

    grammar KeywordFun;
    a : 'abc';
    b : 'def';
    A :  [aA] [bB] [cC];
    B :  [dD] [eE] [fF];
    C :  [uU] [vV] [wW] ( [xX] [yY] [zZ] )?;
    D :  [uU] [vV] [wW] ( [xX] [yY] [zZ] )+;

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.23.11 Fix trgen for st*.g4 grammars. Add template processing for target-specific files in a grammmar.

## License

The MIT License

Copyright (c) 2024 Ken Domino

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
