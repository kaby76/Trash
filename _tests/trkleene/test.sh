#!/bin/bash
trparse t1.g4 | trkleene | trsponge -o Gold -c
trparse t2.g4 | trkleene | trsponge -o Gold -c
trparse t3.g4 | trkleene | trsponge -o Gold -c
trparse t4.g4 | trkleene | trsponge -o Gold -c
