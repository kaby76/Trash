#!/bin/bash
alias trparse=../../trparse/bin/Debug/net5.0/trparse.exe
alias trkleene=../../trkleene/bin/Debug/net5.0/trkleene.exe
alias trsponge=../../trsponge/bin/Debug/net5.0/trsponge.exe
trparse t1.g4 | trkleene | trsponge -o Gold -c
trparse t2.g4 | trkleene | trsponge -o Gold -c
trparse t3.g4 | trkleene | trsponge -o Gold -c
trparse t4.g4 | trkleene | trsponge -o Gold -c
trparse t5.g4 | trkleene | trsponge -o Gold -c
trparse PostgreSQLParser.g4 PostgreSQLLexer.g4 | trkleene | trsponge -o Gold -c
