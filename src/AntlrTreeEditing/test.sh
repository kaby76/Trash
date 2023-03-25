#

./clean.sh
dotnet restore
dotnet build
./Driver/bin/Debug/netcoreapp3.1/Driver.exe ParseTreeDOM/ANTLRv4Parser.g4 > output
diff gold output
