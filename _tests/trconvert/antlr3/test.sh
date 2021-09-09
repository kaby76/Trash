#!/bin/sh

for i in *.g3
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse $i -t antlr3 | trconvert | trsponge --clobber true
done
