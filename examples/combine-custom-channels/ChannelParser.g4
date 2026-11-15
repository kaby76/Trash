parser grammar ChannelParser;

options { tokenVocab = ChannelLexer; }

start : WORD EOF ;
