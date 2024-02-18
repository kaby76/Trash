#!/bin/sh

set -x
set -e

pushd LspServer
dotnet build
popd

pushd VsCode
if [[ "$OSTYPE" == "darwin"* ]]; then
        brew install vsce
fi
rm -rf server
npm i vscode-jsonrpc@6.0.0-next.5
npm i vscode-languageclient@7.0.0-next.9
npm i vscode-languageserver@7.0.0-next.7
npm i vscode-languageserver-protocol@3.16.0-next.7
npm i vscode-languageserver-types@3.16.0-next.3
cp -r ../LspServer/bin/Debug/net8.0 ./server
npm install
npm run compile
vsce package
popd
