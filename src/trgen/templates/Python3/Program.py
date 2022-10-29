# Template generated code from trgen <version>

import sys
from antlr4 import *
from antlr4.error.ErrorListener import ErrorListener
from readchar import readchar
from <lexer_name> import <lexer_name>;
from <parser_name> import <parser_name>;
from datetime import datetime

def getChar():
    xx = readchar()
    if (xx == 0):
        return '';
    return xx

class MyErrorListener(ErrorListener):
    __slots__ = 'num_errors'

    def __init__(self):
        super().__init__()
        self.num_errors = 0

    def syntaxError(self, recognizer, offendingSymbol, line, column, msg, e):
        self.num_errors = self.num_errors + 1
        super().syntaxError(recognizer, offendingSymbol, line, column, msg, e)

show_tokens = False
show_tree = False
inputs = []
is_fns = []
encoding = "utf-8"
error_code = 0

def main(argv):
    i = 1
    while i \< len(argv):
        arg = argv[i]
        if arg in ("-tokens"):
            show_tokens = True
        elif arg in ("-tree"):
            show_tree = True
        elif arg in ("-encoding"):
            i = i + 1
            encoding = argv[i]
        elif arg in ("-input"):
            i = i + 1
            inputs.append(argv[i])
            is_fns.append(false)
        else:
            inputs.append(argv[i])
            is_fns.append(True)
        i = i + 1
    if len(inputs) == 0:
        ParseStdin()
    else:
        start_time = datetime.now()
        for f in range(0, len(inputs)):
            if is_fns[f]:
                ParseFilename(inputs[f])
            else:
                ParseString(inputs[f])
        end_time = datetime.now()
        diff = end_time - start_time
        diff_time = diff.total_seconds()
        print(f'Total Time: {diff_time}', file=sys.stderr);
    sys.exit(error_code)

def ParseStdin():
        sb = ""
        ch = getChar()
        while (ch != ''):
            sb = sb + ch
            ch = getChar()
        input = sb
        str = InputStream(input);
        DoParse(str)

def ParseString(input):
    print('Input: ' + input, file=sys.stderr);
    str = InputStream(input);
    DoParse(str);

def ParseFilename(input):
    print('File: ' + input, file=sys.stderr);
    str = FileStream(input, encoding);
    DoParse(str);

def DoParse(str):
    lexer = <lexer_name>(str);
    lexer.removeErrorListeners()
    l_listener = MyErrorListener()
    lexer.addErrorListener(l_listener)
    # lexer.strictMode = false
    tokens = CommonTokenStream(lexer)
    parser = <parser_name>(tokens)
    parser.removeErrorListeners()
    p_listener = MyErrorListener()
    parser.addErrorListener(p_listener)
    if (show_tokens):
        i = 0
        while True:
            ro_token = lexer.nextToken()
            token = ro_token
            # token.TokenIndex = i
            i = i + 1
            print(token)
            if (token.type == -1):
                break
        lexer.reset()
    start_time = datetime.now()
    tree = parser.<start_symbol>()
    end_time = datetime.now()
    diff = end_time - start_time
    diff_time = diff.total_seconds()
    if p_listener.num_errors > 0 or l_listener.num_errors > 0:
        print('Parse failed.', file=sys.stderr);
        error_code = 1;
    else:
        print('Parse succeeded.', file=sys.stderr);
    print(f'Time: {diff_time}', file=sys.stderr);
    if (show_tree):
        print(tree.toStringTree(recog=parser))

if __name__ == '__main__':
    main(sys.argv)
