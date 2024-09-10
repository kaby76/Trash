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

    trparse -i "1 * 2 + 3" | trquery "grep //expression" | trtokens

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

0.23.5 Add scripts for syntactic highlighting as default for trgenvsc.

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
