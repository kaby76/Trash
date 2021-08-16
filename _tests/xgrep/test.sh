rm -rf bin obj
dotnet restore
dotnet build
echo "1 + 2 + 3" | ../../trparse/bin/Debug/net5.0/trparse.exe | trxgrep ' //SCIENTIFIC_NUMBER' | trtree > gold.txt

