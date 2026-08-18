# Oberon Performance Tests

This directory contains a set of input files in Oberon. The goal is to
test performance on inputs of realistic size (the larger test cases
are modules for the Oberon 2013 compiler) on a grammar constructed for
real-world purposes rather than for testing.

## Grammar

The grammar is the Oberon.ixml grammar from the ixml samples directory.

## Inputs

There are several input files, graduated in size; each is roughly
twice the size of the preceding.  They were created by successively
removing about half of the module ORP.Mod.txt while keeping the result
syntactically legal.

