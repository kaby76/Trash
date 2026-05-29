#!/usr/bin/bash
# pack-trash.sh
# Publishes every sub-tool as a framework-dependent DLL into src/trash/staging/,
# then builds and packs the trash dispatcher nupkg.
#
# Usage: bash _scripts/pack-trash.sh [Release|Debug]
#
set -e
FAILED_TOOLS=()

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SRC="$REPO_ROOT/src"
CONFIG="${1:-Release}"
TFM=net10.0
STAGING="$SRC/trash/staging"

# All PackAsTool tools except tragl (Windows-only WPF).
TOOLS=(
    tranalyze
    trcaret
    trclonereplace
    trcombine
    trconvert
    trcover
    trdistill
    trdot
    trenum
    trextract
    trff
    trfold
    trfoldlit
    trformat
    trgen
    trgenvsc
    trglob
    trgroup
    triconv
    tritext
    trjson
    trkleene
    trnullable
    trparse
    trperf
    trpiggy
    trquery
    trrename
    trrr
    trrup
    trsem
    trsort
    trsplit
    trsponge
    trtext
    trthompson
    trtokens
    trtree
    trull
    trunfold
    trunfoldlit
    trungroup
    trwdog
    trxgrep
    trxml
    trxml2
)

echo "=== Cleaning staging directory ==="
rm -rf "$STAGING"
mkdir -p "$STAGING"

echo "=== Publishing sub-tools (framework-dependent, no AppHost) ==="
for tool in "${TOOLS[@]}"; do
    csproj="$SRC/$tool/$tool.csproj"
    if [ ! -f "$csproj" ]; then
        echo "  SKIP $tool (no csproj found)"
        continue
    fi
    echo "  Publishing $tool ..."
    if ! dotnet publish "$csproj" \
        -c "$CONFIG" \
        --no-self-contained \
        -p:UseAppHost=false \
        -o "$STAGING/$tool" \
        --nologo -v q 2>&1; then
        echo "  FAILED $tool"
        FAILED_TOOLS+=("$tool")
        rm -rf "$STAGING/$tool"
    fi
done

echo "=== Clearing NuGet cache for trash (prevents stale cached nupkg on reinstall) ==="
rm -rf "$HOME/.nuget/packages/trash"

echo "=== Building and packing trash dispatcher ==="
dotnet pack "$SRC/trash/trash.csproj" -c "$CONFIG" --nologo

NUPKG=$(find "$SRC/trash/bin/$CONFIG" -name "trash.*.nupkg" | sort | tail -1)
echo ""
echo "=== Done ==="
echo "Package: $NUPKG"
if [ ${#FAILED_TOOLS[@]} -gt 0 ]; then
    echo ""
    echo "WARNING: the following tools failed to publish and are NOT in the package:"
    for t in "${FAILED_TOOLS[@]}"; do echo "  $t"; done
fi
