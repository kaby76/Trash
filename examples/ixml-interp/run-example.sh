#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

check_tree() {
    local grammar=$1 input=$2 expected=$3 actual
    actual=$(dotnet trash parse "$grammar" | dotnet trash interp |
        dotnet trash parse --allstar -i "$input" | dotnet trash tree -a)
    if [[ "$actual" != "$expected" ]]; then
        printf 'Unexpected tree for %s:\n%s\n' "$input" "$actual" >&2
        exit 1
    fi
    printf '%s\n' "$actual"
}

check_tree Arithmetic.ixml '1+23' '(ixml_start (expr (expr (number 1)) + (number 2 3)) <EOF>)'
check_tree List.ixml '[red,blue]' '(ixml_start (list [ (word r e d) , (word b l u e) ]) <EOF>)'
check_tree List.ixml '[]' '(ixml_start (list [ ]) <EOF>)'

for input in '1+' '1x'; do
    if (dotnet trash parse Arithmetic.ixml | dotnet trash interp |
        dotnet trash parse --allstar --no-output -i "$input") >/dev/null 2>&1; then
        echo "Expected invalid input '$input' to fail." >&2
        exit 1
    fi
done
