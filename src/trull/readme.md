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

0.21.4 Fix trcover.
