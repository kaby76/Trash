parser grammar TemplateParser;

options { tokenVocab = TemplateLexer; }

file_ : (Any | Code)+ EOF ;
