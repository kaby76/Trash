The tests in this directory use a single grammar with tests of varying
sizes, to measure the cost of increased input.

The grammar recognizes whitespace-delimited sequences of decimal
numerals denoting positive integers divisible by 3, 5, or 7.  If any
integer is divisible by more than one of these, the sentence is
ambiguous.

The three sub-grammars are each regular and deterministic; each
requires one parse-tree node per digit of the number.  So both the
size of the raw parse tree and the size of the output XML should grow
linearly with linear increases in the number of input characters.
    
The inputs grow by powers of two.  For individual processors it may be
helpful to add additional tests in particular size ranges to get more
detailed information. It's also likely to be helpful to omit tests at
the end of the seris that take too long to run (for whatever meaning
of "too long" applies in a particular situation).

    
