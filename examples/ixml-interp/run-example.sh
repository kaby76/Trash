#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

dotnet trash parse Arithmetic.ixml | dotnet trash interp -o interp-arithmetic
dotnet trash parse List.ixml | dotnet trash interp -o interp-list

check_tree() {
    local directory=$1 input=$2 expected=$3 actual
    actual=$(dotnet trash parse --allstar -L "$directory" -i "$input" | dotnet trash tree -a)
    if [[ "$actual" != "$expected" ]]; then
        printf 'Unexpected tree for %s:\n%s\n' "$input" "$actual" >&2
        exit 1
    fi
    printf '%s\n' "$actual"
}

check_tree interp-arithmetic '1+23' '(ixml_start (expr (expr (number 1)) + (number 2 3)) <EOF>)'
check_tree interp-list '[red,blue]' '(ixml_start (list [ (word r e d) , (word b l u e) ]) <EOF>)'
check_tree interp-list '[]' '(ixml_start (list [ ]) <EOF>)'

for input in '1+' '1x'; do
    if dotnet trash parse --allstar --no-output -L interp-arithmetic -i "$input" 2> rejected.err; then
        echo "Expected invalid input '$input' to fail." >&2
        exit 1
    fi
done
