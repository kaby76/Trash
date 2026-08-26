#!/usr/bin/bash
dotnet build-server shutdown
rm -rf */bin */obj
rm -rf src/*/bin src/*/obj
