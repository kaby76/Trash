#!/bin/bash

trparse Arithmetic.g4 | trsplit | trsponge -o true
trcombine ArithmeticLexer.g4 ArithmeticParser.g4 | trprint > back.g4
