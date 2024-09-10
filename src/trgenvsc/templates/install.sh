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
npm i vscode-jsonrpc
npm i vscode-languageclient
npm i vscode-languageserver
npm i vscode-languageserver-protocol
npm i vscode-languageserver-types
npm i jsonc-parser
npm i @types/vscode
npm i @types/mocha
npm i @types/node
npm i @vscode/vsce

npm install
npm run compile
./node_modules/.bin/vsce package

echo "Creating settings.rc file to set up syntactic highlighting for inputs for the grammar."
echo "From all lexer rules, get all code points from single character string literals."
python generate-settings.py > settings.rc

echo "Running VSCode. Hit 'F5' in VSCode to start a debug session of VSCode with a LSP server and extension for the grammar."
echo "After opening a file, change type of file to 'any'."
echo "The file will have syntactic highlighting."
code .
