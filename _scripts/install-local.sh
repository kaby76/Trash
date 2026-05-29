#!/usr/bin/bash
set -x
dotnet new tool-manifest --force
cd src
dotnet tool install trash
dotnet trash --version
