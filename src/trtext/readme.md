# trtext

## Summary

Reconstruct source text from parse trees in a PAX/tar artifact bundle.

## Description

By default, reads a PAX/tar artifact bundle from stdin and writes a PAX/tar
bundle to stdout. Each `.pt` member becomes reconstructed source text under
the original source filename and extension, preserving its relative directory.
Other members, including `.errors`, pass through unchanged. Reconstructed
source members receive no display newline. `-f FILE` reads the input from a
file but leaves the default bundle output unchanged. Legacy parsing-result
JSON input is still auto-detected and converted to a source bundle.

Use `--text` for the previous plain-text stdout behavior. With multiple parse
results, that mode prefixes each reconstructed line with its filename and adds
a display newline after each result. `--bundle` remains accepted as a redundant
compatibility alias for the default; `--bundle --text` is an error.

The display-only options `-l`, `-L`, `-c`, and `-n` require `--text`; they are
rejected in bundle mode rather than being written into source members. `-l`
and `-L` list filenames according to whether a result has selected nodes;
`-c` counts selected nodes. The legacy `-n` option is accepted but currently
has no effect.

## Usage

    dotnet trash text [-f FILE] [--text | --bundle] [-n] [-l] [-L] [-c]

    -f, --file                Read the input from FILE instead of stdin.
        --text                Write plain text instead of a PAX/tar bundle.
        --bundle              Compatibility alias for default bundle output.
    -n, --line-number         Legacy text-mode option (currently no effect).
    -l, --files-with-matches  In --text mode, list files with selected nodes.
    -L, --files-without-match In --text mode, list files without selected nodes.
    -c, --count               In --text mode, count selected nodes per file.

## Examples

    dotnet trash parse input.g4 | dotnet trash text | tar -xvf - -C output
    dotnet trash parse input.g4 | dotnet trash text --text

## Current version

Release 4.2.0.

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
