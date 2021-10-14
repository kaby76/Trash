#!/bin/bash
trparse t1.g4 | trrup | trsponge -o Gold -c
trparse t2.g4 | trrup | trsponge -o Gold -c
trparse t3.g4 | trrup | trsponge -o Gold -c
