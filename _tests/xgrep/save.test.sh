rm -rf Generated
trgen
cd Generated
make
echo "1 + 2 + 3" | ../../../trparse/bin/Debug/net5.0/trparse.exe | ../../../trxgrep/bin/Debug/net5.0/trxgrep.exe ' //SCIENTIFIC_NUMBER' | ../../../trtree/bin/Debug/net5.0/trtree.exe > gold.txt

