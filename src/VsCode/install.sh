#!/bin/sh
set -e
npm i vscode-jsonrpc@8.1.0
npm i vscode-languageclient@8.1.0
npm i vscode-languageserver@8.1.0
npm i vscode-languageserver-protocol@3.17.3
npm i vscode-languageserver-types@3.17.3
rm -rf Server
mkdir Server
cp -r ../Server/bin ./Server
npm install
npm run compile
vsce package
code .
