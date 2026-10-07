# trjson

## Summary

Render parse trees as JSON artifacts in a PAX/tar bundle.

## Description

By default, read a PAX/tar parse-result bundle from stdin and write a PAX/tar
bundle to stdout. Each `.pt` member is replaced by a `.json` member at the
same path (for example, `dir/example.st.pt` becomes `dir/example.st.json`).
Other members pass through unchanged. Legacy parsing-result JSON input is
also accepted. Use `--text` for plain JSON output; `--bundle` is a compatibility
alias for the default and cannot be combined with `--text`.

## Usage

    dotnet trash json [-f FILE] [--text | --bundle]

## Examples

    dotnet trash parse A.g4 | dotnet trash json | tar -tf -
    dotnet trash parse A.g4 | dotnet trash json --text | less

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
