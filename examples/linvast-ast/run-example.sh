#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

dotnet build ../../src/trparse/trparse.csproj -c Release --nologo >&2
temp_root=$(realpath -e "${TMPDIR:-/tmp}")
scratch=$(realpath -e "$(mktemp -d "$temp_root/linvast-ast.XXXXXX")")
cleanup() {
  if [[ "$scratch" == "$temp_root"/linvast-ast.* && -d "$scratch" ]]; then
    rm -rf -- "$scratch"
  fi
}
trap cleanup EXIT
parse=../../../src/trparse/bin/Release/net10.0/trparse.dll

compile() {
  local language=$1
  shift
  (cd "$language" && dotnet trash parse "$@" |
    dotnet trash interp -o "$scratch/$language")
}

compile c C.g4
compile java JavaLexer.g4 Java.g4
# Kotlin has two EOF-terminated entry rules; select kotlinFile explicitly.
(cd kotlin && dotnet trash parse KotlinLexer.g4 KotlinParser.g4 |
  dotnet trash interp --start-rule kotlinFile -o "$scratch/kotlin")
compile lua Lua.g4

check() {
  local language=$1 rule=$2 input=$3 expected=$4 actual
  actual=$(cd "$language" && dotnet "$parse" --allstar \
    -L "$scratch/$language" --start-rule "$rule" \
    --ast-map "$language.ast" "../input/$input" |
    dotnet trash tree --text -a)
  if [[ "$actual" != "$expected" ]]; then
    printf '%s/%s: expected %s, got %s\n' "$language" "$rule" "$expected" "$actual" >&2
    exit 1
  fi
  printf '%s %s => %s\n' "$language" "$rule" "$actual"
}

check c primaryExpression identifier.txt '(IdNode x)'
check java primary identifier.txt '(IdNode x)'
check kotlin simpleIdentifier identifier.txt '(IdNode x)'
check lua var identifier.txt '(IdNode x)'
check c primaryExpression integer.txt '(LitExprNode 42)'
check java primary integer.txt '(LitExprNode 42)'
check kotlin literalConstant integer.txt '(LitExprNode 42)'
check lua number integer.txt '(LitExprNode 42)'

# The .pt AST is still a normal Trash bundle, not a text-only side channel.
count=$(cd java && dotnet "$parse" --allstar -L "$scratch/java" \
  --start-rule primary --ast-map java.ast ../input/identifier.txt |
  dotnet trash xquery 'count(//IdNode)')
[[ "$count" == 1 ]] || { echo "Expected one IdNode, got $count" >&2; exit 1; }
