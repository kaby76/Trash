# Sizes 0

This directory contains a set of related grammars and inputs which
vary in size.

## Grammars

The base grammar is the straightforward

    S = 'a'* .

which is 10 (1E1) characters long.

By adding whitespace and comments, we make several alternative
versions of this grammar which are approximately 1E2, 1E3, and 1E4
characters long.  For the times when we want smaller jumps in size
than a factor of ten, we also have grammar files with sizes of 10 time
1 * 2**n for n in 1 to 10 (so: 10, 20, 40, ... 10240).

For each length, we make one in which all the whitespace and comments
are added in one place and one in which they are evenly divided among
several places where they can occur:

    {1} S {2} = {3} 'a'* {4} . {5}

Whitespace is also allowed before the asterisk, but I'd like five
blocks of whitespace, not six, to simplify the arithmetic.

For each length and division among blocks, we make one grammar in
which the whitesapce is all blanks, tabs, or newlines, one in which it
is essentially one long comment, and one in which it is a series of
smaller comments of size 25 or so (separated by newlines for
legibility).

That's four lengths, single-block or multi-block, whitespace-only or
long-comment or short-comment for 24 grammars.  The names of the
grammars are formed on the pattern

  'G' + base + + L + '.' + B + '.' + S, where
  base is 'b' for binary or 'd' for decimal
  L is the exponent for the length: 1 .. 4
  B is the block style 's' or 'm'
  S is the whitespace choice 'ws', or 'lc', or 'sc'.


## Inputs

The positive inputs are sequences of the letter 'a', of lengths 0,
1e0, 1e1, 1e2, 1e3, 1e4.

The negative inputs (designed to take the same resources as the
positive ones) are formed by adding a single 'b' at the end of each
positive input.

The  input files are named on the pattern

    polarity + length + '.txt', where
    polarity is 'P' or 'N' for positive and negative
    length is 0, 1e0, 1e1, 1e2, 1e3, 1e4

A second set of inputs was later added to provide a ratio of 1:2
between successive (groups of) test cases.  For these:

    length is b00, b01, b02, ... b13

