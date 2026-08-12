#!/usr/bin/bash
while [[ $# -gt 0 ]]
do
	key="$1"
	shift
	dotnet tool uninstall -g $key
	dotnet tool install -g $key
done

