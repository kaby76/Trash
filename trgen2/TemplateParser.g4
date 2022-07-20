parser grammar TemplateParser;

options { lexerVocab = TemplateLexer; }

file_ : (Any | Code)+ EOF ;
