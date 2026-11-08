# trglob

## Summary

Expand one or more glob patterns into file names.

## Description

## Usage

    dotnet trash glob [--full] <pattern>+

Each argument may contain one pattern or a semicolon-separated list of
patterns. Quote patterns in the shell so `*`, `**`, `!`, and `;` reach `trglob`
unchanged. Patterns are relative to the current directory unless absolute.
Positive patterns are combined; a pattern prefixed with `!` excludes matches
from the combined result. Duplicate paths are removed and output is sorted.
By default paths are relative to the current directory; `--full` prints
absolute paths.

`*` matches within one path segment, `?` matches one character, and `**`
matches across directories (including zero directories in `**/`).
Braces select alternatives, including alternatives with directory separators:
`{*.cpp,cpp20/**/*.cpp}`. Brace alternatives can be nested.

## Examples

    dotnet trash glob '../examples/**/*.dart'
    dotnet trash glob '../examples/**'
    dotnet trash glob '../examples/*.cpp;../examples/cpp20/**/*.cpp'
    dotnet trash glob '../examples/{*.cpp,cpp20/**/*.cpp}'
    dotnet trash glob '../examples/**/*.cpp' '!../examples/generated/**'

In `desc.xml`, `<inputs>` also accepts semicolon-separated patterns, such as
`<inputs>examples/*.cpp;examples/cpp20/**/*.cpp</inputs>`. `trgen` emits each
pattern separately in generated test scripts, prefixing each with `../`
because those scripts run from a `Generated-*` directory.

## Current version

Release 4.9.0.

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
