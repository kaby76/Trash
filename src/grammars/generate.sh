#!/usr/bin/bash

for i in antlr4
do
	cd $i
	rm -rf Generated
	trgen -p $i
	cd Generated
	make
	cd ../..
done
