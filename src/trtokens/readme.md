# trtokens

## Summary

Print tokens in a parse tree

## Description

The trtokens command reads standard in for a parsing result set and prints out
the tokens for each result. For each tree in a set, the first and last tokens
for the tree are computed and printed, with a blank line separator.

## Usage

    trtokens

## Examples

Input:

    Assume the Arithmetic.g4 parser has been built.

Command:

    trparse -i "1 * 2 + 3" | trxgrep " //expression" | trtokens

Output:

    Time to parse: 00:00:00.0778212
    # tokens per sec = 128.49968903075256
    [@4,4:4='2',<2>,1:4]

    [@0,0:0='1',<2>,1:0]

    [@8,8:8='3',<2>,1:8]

    [@0,0:0='1',<2>,1:0]
    [@1,1:1=' ',<15>,channel=1,1:1]
    [@2,2:2='*',<7>,1:2]
    [@3,3:3=' ',<15>,channel=1,1:3]
    [@4,4:4='2',<2>,1:4]

    [@0,0:0='1',<2>,1:0]
    [@1,1:1=' ',<15>,channel=1,1:1]
    [@2,2:2='*',<7>,1:2]
    [@3,3:3=' ',<15>,channel=1,1:3]
    [@4,4:4='2',<2>,1:4]
    [@5,5:5=' ',<15>,channel=1,1:5]
    [@6,6:6='+',<5>,1:6]
    [@7,7:7=' ',<15>,channel=1,1:7]
    [@8,8:8='3',<2>,1:8]

## Current version

0.20.20 Updates to trfoldlit, trconvert (rex, pegjs, kocmanllk but incomplete, trparse, trstrip. Add trcover. Fix trgen for Java with Java/ files. Parse tree attributes added. NB: not all Trash tools supported yet.
