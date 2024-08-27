#!/bin/sh

set -x
set -e

pushd parser/Generated-CSharp
make
popd

pushd server/LspServer
dotnet build
popd

if [[ "$OSTYPE" == "darwin"* ]]; then
        brew install vsce
fi
npm i vscode-jsonrpc@6.0.0-next.5
npm i vscode-languageclient@7.0.0-next.9
npm i vscode-languageserver@7.0.0-next.7
npm i vscode-languageserver-protocol@3.16.0-next.7
npm i vscode-languageserver-types@3.16.0-next.3
npm install
npm run compile
./node_modules/.bin/vsce package
