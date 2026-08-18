#!/usr/bin/env bash
# run-example.sh
# Ungroup the first plain grouped alternative in rule 'a' of A.g4.
# Input:   grammar A;  a : d(a | 'X') 'B' 'Z' | 'C' ;
# Output:  grammar A;  a : d a 'B' 'Z' | d 'X' 'B' 'Z' | 'C' ;
set -x
set -e
cd "$(dirname "$0")"

dotnet trash parse A.g4 > A.pt
dotnet trash tree -f A.pt > A.tree

dotnet trash xquery -f A.pt --query ungroup-rule.xq 'a' > A-ungrouped.pt
dotnet trash tree -f A-ungrouped.pt > A-ungrouped.tree
dotnet trash sponge -f A-ungrouped.pt -c -o ungrouped
