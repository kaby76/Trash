#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

check_tree() {
    local grammar=$1 input=$2 expected=$3 actual
    actual=$(dotnet trash parse "$grammar" | dotnet trash interp |
        dotnet trash parse --allstar -i "$input" | dotnet trash tree --text -a)
    if [[ "$actual" != "$expected" ]]; then
        printf 'Unexpected tree for %s:\n%s\n' "$input" "$actual" >&2
        exit 1
    fi
    printf '%s\n' "$actual"
}

check_tree Arithmetic.rex '1+2*3' '(Start (Expr (Expr (Term 1)) + (Term 2 * 3)) <EOF>)'
check_tree List.rex '[red,blue]' '(Start [ red , blue ] <EOF>)'
check_tree List.rex '[]' '(Start [ ] <EOF>)'

if (dotnet trash parse Arithmetic.rex | dotnet trash interp |
    dotnet trash parse --allstar --no-output -i '1+') >/dev/null 2>&1; then
    echo 'Expected incomplete arithmetic expression to fail.' >&2
    exit 1
fi
