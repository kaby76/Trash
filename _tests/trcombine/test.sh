#!/bin/bash

trparse Arithmetic.g4 | trsplit | trsponge -c true
trcombine ArithmeticLexer.g4 ArithmeticParser.g4 | trprint > back.g4
diff Arithmetic.g4 back.g4
