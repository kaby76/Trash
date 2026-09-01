#!/usr/bin/env bash
# Construct XML containing the name of every parser rule in RuleNames.g4.
set -euo pipefail
cd "$(dirname "$0")"

dotnet trash parse RuleNames.g4 \
  | dotnet trash xquery --query rule-names.xq \
  | dotnet trash tree
