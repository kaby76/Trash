#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

dotnet build AstRewrite.csproj -c Release --nologo >&2
actual=$(mktemp)
trap 'rm -f "$actual"' EXIT
check_ast() {
  local expression=$1 expected=$2
  dotnet trash parse Arithmetic.g4 |
    dotnet trash interp |
    dotnet trash parse --allstar -i "$expression" |
    dotnet run --project AstRewrite.csproj -c Release --no-build -- arithmetic.ast > "$actual"
  diff -u <(tr -d '\r' < "$expected") <(tr -d '\r' < "$actual")
  cat "$actual"
}

check_ast '1+2*3' expected.json
check_ast '(1+2)*3' expected-parentheses.json

# Without a matching multi-child rule, the rewriter must reject the CST.
if (dotnet trash parse Arithmetic.g4 |
    dotnet trash interp |
    dotnet trash parse --allstar -i '1+2' |
    dotnet run --project AstRewrite.csproj -c Release --no-build -- incomplete.ast) > "$actual" 2>&1; then
  echo 'An incomplete mapping unexpectedly accepted the CST.' >&2
  exit 1
fi
grep -q 'No AST rewrite for expr' "$actual"
