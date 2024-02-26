#!/bin/sh

pushd VsCode
rm -rf node_modules
rm -f package-lock.json
rm -rf out
# rm -rf 'c:/Users/kenne/AppData/Roaming/Code'
rm -rf server/net8.0
rm -rf Trash
rm -rf *.vsix
# npm install
popd
