#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

dotnet trash parse Arithmetic.rex | dotnet trash interp -o interp-arithmetic
dotnet trash parse List.rex | dotnet trash interp -o interp-list

check_tree() {
    local directory=$1 input=$2 expected=$3 actual
    actual=$(dotnet trash parse --allstar -L "$directory" -i "$input" | dotnet trash tree -a)
    if [[ "$actual" != "$expected" ]]; then
        printf 'Unexpected tree for %s:\n%s\n' "$input" "$actual" >&2
        exit 1
    fi
    printf '%s\n' "$actual"
}

check_tree interp-arithmetic '1+2*3' '(Start (Expr (Expr (Term 1)) + (Term 2 * 3)) <EOF>)'
check_tree interp-list '[red,blue]' '(Start [ red , blue ] <EOF>)'
check_tree interp-list '[]' '(Start [ ] <EOF>)'

if dotnet trash parse --allstar --no-output -L interp-arithmetic -i '1+' 2> rejected.err; then
    echo 'Expected incomplete arithmetic expression to fail.' >&2
    exit 1
fi
