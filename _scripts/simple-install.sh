#
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trformat trgen trgroup trinsert tritext trjson trkleene trmove trparse trperf trpiggy trprint trrename trreplace trrr trrup trsort trsplit trsponge trst trstrip trtext trtokens trtree trull trunfold trungroup trwdog trxgrep trxml trxml2; do dotnet tool install -g $i; done
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac
if [[ "$machine" == "MinGw" || "$machine" == "Msys" ]]
then
    git clone https://github.com/kaby76/Domemtech.Trash.git; cd Domemtech.Trash/tragl; dotnet build; cwd=`pwd`; a tragl="$cwd/bin/Debug/net6.0/tragl.exe"
fi
