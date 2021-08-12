#!/bin/sh

for i in *.g2
do
	echo $i
	trparse $i -t antlr2 | trprint > o
	diff o $i
	rm -f o
done
for i in *.g2
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse $i -t antlr2 | trconvert | trsponge -c true
done
for i in *.g4
do
	echo $i
	java -jar ~/Downloads/antlr-4.9.2-complete.jar $i
	rm -f *.java *.interp *.tokens
done
