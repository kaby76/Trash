#!/usr/bin/env bash
# Compute FIRST sets for the parser rules reachable from First.g4's start rule.
set -e
cd "$(dirname "$0")"

dotnet trash parse First.g4 \
  | dotnet trash xquery --query first.xq start
