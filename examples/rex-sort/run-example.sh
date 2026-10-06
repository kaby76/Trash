#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

scratch="$(mktemp -d ./rex-sort-test-XXXXXXXX)"
trap 'case "$scratch" in ./rex-sort-test-*) rm -rf -- "$scratch" ;; esac' EXIT

dotnet trash parse ../rex-interp/Arithmetic.rex \
  | dotnet trash xquery -q sort-rex.xq \
  | dotnet trash text >"$scratch/result.tar"
tar -xOf "$scratch/result.tar" Arithmetic.rex >"$scratch/sorted.rex"

awk '
  /^Expr ::=/{expr=NR}
  /^Start ::=/{start=NR}
  /^Term ::=/{term=NR}
  /^<\?TOKENS\?>/{tokens=NR}
  /^Digit ::=/{digit=NR}
  /^End ::=/{end=NR}
  /^Number ::=/{number=NR}
  END {exit !(expr && expr < start && start < term && term < tokens &&
              tokens < digit && digit < end && end < number)}
' "$scratch/sorted.rex"

dotnet trash parse "$scratch/sorted.rex" >/dev/null
echo 'REx rules sorted and the resulting grammar reparsed successfully.'

cat "$scratch/sorted.rex"
