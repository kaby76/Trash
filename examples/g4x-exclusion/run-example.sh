#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export MSYS2_ARG_CONV_EXCL='*'

# Parse the G4X grammar and show the two exclusion clauses in its tree.
dotnet trash parse JlsIdentifiers.g4x \
  | dotnet trash xpath '//exclusion' \
  | dotnet trash tree
