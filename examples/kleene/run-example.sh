#!/usr/bin/env bash
# run-example.sh
# Applies left-recursive and right-recursive Kleene transforms to Kleene.g4.
# Output is written to Kleene-out.g4.
set -x
set -e
cd "$(dirname "$0")"

dotnet trash parse Kleene.g4 > Kleene.pt
dotnet trash tree -f Kleene.pt > Kleene.tree

dotnet trash xquery -f Kleene.pt --query kleene-lr.xq > Kleene-lr.pt
dotnet trash tree -f Kleene-lr.pt > Kleene-lr.tree
dotnet trash sponge -f Kleene-lr.pt -o nolr -c

dotnet trash xquery -f Kleene.pt --query kleene-rr.xq > Kleene-rr.pt
dotnet trash tree -f Kleene-rr.pt > Kleene-rr.tree
dotnet trash sponge -f Kleene-rr.pt -o norr -c
