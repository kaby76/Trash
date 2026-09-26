#!/bin/bash
# Convert one iXML grammar to an ANTLR4 combined grammar on stdout.
#
# Usage:
#   convert-ixml-to-antlr4.sh path/to/grammar.ixml > grammar.g4

set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Usage: $(basename "$0") path/to/grammar.ixml" >&2
    exit 2
fi

input=$1
if [[ ! -f "$input" ]]; then
    echo "$(basename "$0"): input file not found: $input" >&2
    exit 2
fi

# Resolve every query relative to this script, not the caller's working
# directory. BASH_SOURCE is used instead of $0 so this also works through a
# symlink or when invoked by another Bash script.
script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
input_dir=$(cd -- "$(dirname -- "$input")" && pwd)
input="$input_dir/$(basename -- "$input")"
input_name=$(basename -- "$input")
grammar_name=${input_name%.ixml}
grammar_name=$(printf '%s' "$grammar_name" | sed 's/[^A-Za-z0-9_]/_/g')
if [[ ! "$grammar_name" =~ ^[A-Za-z_] ]]; then
    grammar_name="G_$grammar_name"
fi

work_dir=$(mktemp -d "${TMPDIR:-/tmp}/ixml-to-antlr4.XXXXXX")
trap 'rm -rf -- "$work_dir"' EXIT

native_path() {
    if command -v cygpath >/dev/null 2>&1; then
        cygpath -am "$1"
    else
        printf '%s\n' "$1"
    fi
}

native_input=$(native_path "$input")
native_script_dir=$(native_path "$script_dir")
native_work_dir=$(native_path "$work_dir")

# Prevent MSYS2 from rewriting XPath/XQuery-looking arguments passed to .NET.
export MSYS2_ARG_CONV_EXCL="*"

# Run the local tool from the repository root so dotnet can discover
# .config/dotnet-tools.json even when this script was launched elsewhere.
tool_root=$(cd -- "$script_dir/../.." && pwd)
cd "$tool_root"

dotnet trash parse -t ixml "$native_input" > "$work_dir/input.pt"
dotnet trash xquery -q "$native_script_dir/ixml-to-antlr4.xq" \
    -f "$native_work_dir/input.pt" > "$work_dir/structural.pt"
dotnet trash xquery -q "$native_script_dir/encoded-to-antlr4.xq" \
    -f "$native_work_dir/structural.pt" > "$work_dir/encoded.pt"
dotnet trash xquery -q "$native_script_dir/charset-to-antlr4.xq" \
    -f "$native_work_dir/encoded.pt" > "$work_dir/charsets.pt"
dotnet trash xquery -q "$native_script_dir/sets-to-lexer.xq" \
    -f "$native_work_dir/charsets.pt" > "$work_dir/sets.pt"
dotnet trash xquery -q "$native_script_dir/sep-repeat-to-antlr4.xq" \
    -f "$native_work_dir/sets.pt" > "$work_dir/final.pt"

printf 'grammar %s;\n\n' "$grammar_name"
dotnet trash text -f "$native_work_dir/final.pt"
