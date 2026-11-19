#!/usr/bin/env bash
set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

dotnet ../../src/trparse/bin/Release/net11.0/trparse.dll ChannelLexer.g4 ChannelParser.g4 \
  | dotnet ../../src/trcombine/bin/Release/net11.0/trcombine.dll \
  | dotnet ../../src/trxquery/bin/Release/net11.0/trxquery.dll -q channel-to-hidden.xq \
  | dotnet ../../src/trtext/bin/Release/net11.0/trtext.dll --text
