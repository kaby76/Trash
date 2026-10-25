#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

dotnet build ../../src/trparse/trparse.csproj -c Release --nologo >&2
dotnet build ../../src/trinterp/trinterp.csproj -c Release --nologo >&2
temp_root=$(realpath -e "${TMPDIR:-/tmp}")
scratch=$(realpath -e "$(mktemp -d "$temp_root/ast-during-parse.XXXXXX")")
cleanup() {
  if [[ "$scratch" == "$temp_root"/ast-during-parse.* && -d "$scratch" ]]; then
    rm -rf -- "$scratch"
  fi
}
trap cleanup EXIT

parse=../../src/trparse/bin/Release/net10.0/trparse.dll
interp=../../src/trinterp/bin/Release/net10.0/trinterp.dll
dotnet "$parse" Arithmetic.g4 arithmetic.ast | dotnet "$interp" -o "$scratch/interp"
grep -q '^ast-mapping:$' "$scratch/interp/Arithmetic.interp"
if grep -q '^ast-mapping:$' "$scratch/interp/ArithmeticLexer.interp"; then
  echo 'AST table must be in the parser interp only.' >&2
  exit 1
fi
check_ast() {
  local expression=$1 expected=$2 actual
  actual=$(dotnet "$parse" --allstar -L "$scratch/interp" \
    -i "$expression" |
    dotnet trash tree --text -a)
  if [[ "$actual" != "$(< "$expected")" ]]; then
    printf 'Unexpected AST for %s:\n%s\n' "$expression" "$actual" >&2
    exit 1
  fi
  printf '%s\n' "$actual"
}

check_ast '1+2*3' expected.tree
check_ast '(1+2)*3' expected-parentheses.tree

# The same parse-result bundle can be consumed as a tree by XQuery4.
count=$(dotnet "$parse" --allstar -L "$scratch/interp" \
  -i '1+2*3' |
  dotnet trash xquery 'count(//Number)')
if [[ "$count" != 3 ]]; then
  printf 'Expected three Number AST nodes, got: %s\n' "$count" >&2
  exit 1
fi

if dotnet "$parse" --allstar -L "$scratch/interp" \
    -i '1+' > "$scratch/invalid.pt" 2> "$scratch/invalid.err"; then
  echo 'An incomplete expression unexpectedly parsed.' >&2
  exit 1
fi
grep -q 'input rejected by grammar' "$scratch/invalid.err"

# An embedded mapping must never silently produce a CST through Earley.
if dotnet "$parse" --earley -L "$scratch/interp" -i '1+2' \
    > "$scratch/earley.pt" 2> "$scratch/earley.err"; then
  echo 'Earley unexpectedly accepted an embedded AST mapping.' >&2
  exit 1
fi
grep -q 'use --allstar' "$scratch/earley.err"
