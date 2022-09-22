##
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements. See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership. The ASF licenses this file
# to you under the Apache License, Version 2.0 (the  "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
##
#
# $Id: Makefile.inc 468650 2006-10-28 07:03:30Z minchau $
#
####################################################################
#                                                                  #
# Makefile.inc for XSLT compiler                                   #
#                                                                  #
####################################################################

####################################################################
# Useful macros                                                    #
####################################################################

JAVAC = javac
#JAVAC_FLAGS = -g -d $(XSLT)/src/classes
JAVAC_FLAGS = -sourcepath $(XSLT)/src
JAVACC = javacc
JAVACC_FLAGS = -static=FALSE 
JAVACUP = javacup
JAVACUP_FLAGS =
JAVALEX = jlex
JAVALEX_FLAGS =

####################################################################
#  Explicit rules                                                  #
####################################################################

.SUFFIXES: .java .class .jj .lex .cup

.java.class:
	$(JAVAC) $(JAVAC_FLAGS) $<

.jj.java:
	$(JAVACC) $(JAVACC_FLAGS) $<

.cup.java:
	$(JAVACUP) $(JAVACUP_FLAGS) $<

.lex.java:
	$(JAVALEX) $(JAVALEX_FLAGS) $<

