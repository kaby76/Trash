#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
dotnet_cmd=${DOTNET:-dotnet}
repo=$(cd ../.. && pwd)

parse=$repo/src/trparse/bin/Release/net10.0/trparse.dll
interp=$repo/src/trinterp/bin/Release/net10.0/trinterp.dll
gen=$repo/src/trgen/bin/Release/net10.0/trgen.dll
wdog=$repo/src/trwdog/bin/Release/net10.0/trwdog.dll

bash test-example.sh

# Build the tables once so their generation time is not charged to either parse.
"$dotnet_cmd" "$parse" Nest.g4 | "$dotnet_cmd" "$interp" -o interp

echo '--- ALL(*) SLL, n10 ---'
"$dotnet_cmd" "$parse" --allstar --sll -L interp --no-output --per-file examples/n10.txt
echo '--- ALL(*) default LL, n10 ---'
"$dotnet_cmd" "$parse" --allstar -L interp --no-output --per-file examples/n10.txt

# The generated project pins both the ANTLR tool and C# runtime to 4.13.1.
"$dotnet_cmd" "$gen" -t CSharp --antlr-version 4.13.1
(cd Generated-CSharp && bash build.sh)

echo '--- ANTLR C# 4.13.1 default LL, n10 ---'
(cd Generated-CSharp && "$dotnet_cmd" bin/Debug/net10.0/Test.dll --per-file ../examples/n10.txt)

echo '--- ANTLR C# 4.13.1 pure SLL, n10 (five-second cap) ---'
if output=$(cd Generated-CSharp &&
    "$dotnet_cmd" "$wdog" -t 5 "$dotnet_cmd" bin/Debug/net10.0/Test.dll \
        --sll --per-file ../examples/n10.txt 2>&1); then
    printf '%s\n' "$output"
elif [[ "$output" == *'Process is taking longer than 5 seconds'* ]]; then
    printf '%s\n' "$output"
    echo 'The unfixed C# runtime did not finish within five seconds.'
else
    printf 'C# SLL driver failed unexpectedly:\n%s\n' "$output" >&2
    exit 1
fi
