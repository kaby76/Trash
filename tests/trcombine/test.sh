#!/usr/bin/env bash

set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

parse='../../src/trparse/bin/Release/net11.0/trparse.dll'
combine='../../src/trcombine/bin/Release/net11.0/trcombine.dll'
text='../../src/trtext/bin/Release/net11.0/trtext.dll'
sponge='../../src/trsponge/bin/Release/net11.0/trsponge.dll'
xquery='../../src/trxquery/bin/Release/net11.0/trxquery.dll'

scratch="$(mktemp -d ./trcombine-test-XXXXXXXX)"
trap 'case "$scratch" in ./trcombine-test-*) rm -rf -- "$scratch" ;; esac' EXIT

dotnet "$parse" ArithmeticLexer.g4 ArithmeticParser.g4 |
	dotnet "$combine" > "$scratch/arithmetic.tar"
test "$(tar -tf "$scratch/arithmetic.tar")" = 'Arithmetic.g4.pt'
dotnet "$sponge" -f "$scratch/arithmetic.tar" -o "$scratch/arithmetic"
test "$(find "$scratch/arithmetic" -type f | wc -l)" -eq 1
diff -u Gold/Arithmetic.g4 "$scratch/arithmetic/Arithmetic.g4"

# Leading comments must not obscure the grammar kind, and compatible lexer
# options must survive the merge.
dotnet "$parse" CommentedLexer.g4 CommentedParser.g4 |
	dotnet "$combine" > "$scratch/commented.tar"
test "$(tar -tf "$scratch/commented.tar")" = 'Commented.g4.pt'
dotnet "$sponge" -f "$scratch/commented.tar" -o "$scratch/commented"
test "$(find "$scratch/commented" -type f | wc -l)" -eq 1
grep -q 'Parser license header' "$scratch/commented/Commented.g4"
grep -q '^grammar Commented;' "$scratch/commented/Commented.g4"
grep -q 'caseInsensitive = true' "$scratch/commented/Commented.g4"
if grep -q tokenVocab "$scratch/commented/Commented.g4"; then exit 1; fi
dotnet "$parse" --no-output "$scratch/commented/Commented.g4"

# The combined result is a real grammar tree, so XQuery can update the
# lexer-command nodes and remove the channels declaration.
dotnet "$parse" ChannelLexer.g4 ChannelParser.g4 |
    dotnet "$combine" > "$scratch/channels.tar"
test "$(tar -tf "$scratch/channels.tar")" = 'Channel.g4.pt'
dotnet "$xquery" -q ../../examples/combine-custom-channels/channel-to-hidden.xq \
    -f "$scratch/channels.tar" > "$scratch/channels-rewritten.tar"
dotnet "$sponge" -f "$scratch/channels-rewritten.tar" -o "$scratch/channels"
grep -q 'channel(HIDDEN)' "$scratch/channels/Channel.g4"
if grep -Eq 'channels[[:space:]]*\{|channel\(SPECIAL\)' "$scratch/channels/Channel.g4"; then exit 1; fi
dotnet "$parse" --no-output "$scratch/channels/Channel.g4"

# Select the two top-level grammars, then inline lexer and parser imports.
dotnet "$parse" CharacterLexer.g4 LetterLexer.g4 WordLexer.g4 StatementParser.g4 RootParser.g4 |
    dotnet "$combine" > "$scratch/imports.tar"
test "$(tar -tf "$scratch/imports.tar")" = 'Root.g4.pt'
dotnet "$sponge" -f "$scratch/imports.tar" -o "$scratch/imports"
grep -q '^grammar Root;' "$scratch/imports/Root.g4"
grep -q 'fragment LETTER' "$scratch/imports/Root.g4"
grep -q 'fragment LOWER' "$scratch/imports/Root.g4"
grep -q 'root : start' "$scratch/imports/Root.g4"
grep -q 'start : WORD EOF' "$scratch/imports/Root.g4"
if grep -Eq '(^import |tokenVocab|caseInsensitive)' "$scratch/imports/Root.g4"; then exit 1; fi
dotnet "$parse" --no-output "$scratch/imports/Root.g4"

# Direct source arguments are rejected immediately, without reading stdin.
if dotnet "$combine" CommentedLexer.g4 CommentedParser.g4 </dev/null \
	>"$scratch/misuse.out" 2>"$scratch/misuse.err"; then
	echo 'trcombine unexpectedly accepted source-file arguments.' >&2
	exit 1
fi
grep -q 'reads parsed grammars from stdin' "$scratch/misuse.err"
test ! -s "$scratch/misuse.out"

echo 'trcombine regression tests passed.'
