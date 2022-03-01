# Trrename

Rename a symbol in a grammar.

# Usage

    trrename -r <string>

# Details

`trrename` renames rule symbols in a grammar.

The `-r` option is required. It
is a list of semi-colon delimited pairs of symbol names, which are separated
by a comma, e.g., `id,identifier;name,name_`. If you are using Bash,
make sure to enclose the argument as it contains semi-colons.

# Examples

    trparse Foobar.g4 | trrename -r "a,b;c,d" | trprint > new-grammar.g4

# Current version

0.15.0 -- updates to trgen.
