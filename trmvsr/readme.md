# Trmvsr

Move the rule, whose symbol is identified by the xpath string, to the top of the grammar.

# Usage

    trmvsr <string>

# Example

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.11.3 -- Updated trtree, trinsert.
