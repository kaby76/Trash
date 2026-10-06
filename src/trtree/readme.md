# trtree

## Summary

Render parse trees into `.tree` artifacts in a PAX/tar bundle.

## Description

Reads a PAX/tar parse-result bundle from stdin and writes a PAX/tar bundle to
stdout by default. Each `.pt` member becomes a `.tree` member in the same
relative directory; `.errors` and other artifacts pass through unchanged.
`-f FILE` reads the input from a file without changing the output mode. Legacy
parsing-result JSON input is still auto-detected.

Use `--text` for the previous human-readable stdout behavior, including
filename prefixes when multiple results are present. `--bundle` remains a
redundant compatibility alias for the default and cannot be combined with
`--text`. The display-only `-d` option requires `--text`; tree style options
(`-a`, `-A`, `-i`, `-b`, and `--paren-indent-style`) work in either mode.

## Usage

    dotnet trash tree [-f FILE] [--text | --bundle] [-a | -A | -i | -b]

## Examples

    dotnet trash parse A.g4 | dotnet trash tree | tar -xvf - -C output
    dotnet trash parse A.g4 | dotnet trash tree --text
    dotnet trash parse A.g4 | dotnet trash tree --text -a
    dotnet trash parse A.g4 | dotnet trash tree --text -A

`-a` matches ANTLR's `ToStringTree()` output and prints terminal token text
without terminal token-type nodes or a trailing line terminator. `-A` retains
the previous ANTLR-like form, which includes terminal token-type nodes. With
no style option, the block-style tree is used.

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
# Artifact bundles

PAX/tar and legacy JSON inputs are detected automatically. The default output
is a PAX/tar bundle. Every `.pt` member is rendered with the selected tree
style and replaced by a `.tree` member; error and unknown regular-file
artifacts pass through unchanged. `--bundle` remains available for callers
that previously specified it explicitly.
