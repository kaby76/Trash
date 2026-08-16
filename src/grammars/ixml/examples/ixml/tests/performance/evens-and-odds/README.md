# Sizes 1

This directory contains one grammar and inputs which vary in size.

## Grammar

The grammar is a straightforward case of requiring lookahead to the
end of the input:

    S = evens, eflag; odds, oflag.
    evens = (); LE, evens, RE.
    odds = 'a'; LO, odds, RO.
    LE = 'a'.
    RE = 'a'.
    LO = 'a'.
    RO = 'a'.
    eflag = 'e'.
    oflag = 'o'.

L(G) is the same as for the regular expression "(aa)*e|a(aa)*o".

The goal is to require the parser to generate a lot of Earley items
but remain unambiguous.



## Inputs

for every length L in 0 and 2**n for n in 1 to 13, we have files named

  'P' || L || 'e.txt'
  'P' || L + 1 || 'o.txt'
  'N' || L || 'o.txt'
  'N' || L + 1 || 'e.txt'

Each file contains L or L+1 occurrences of 'a', followed by an 'e' or
an 'o' as indicated in the filename.  So there are two positive and
two negative test cases for each length.


