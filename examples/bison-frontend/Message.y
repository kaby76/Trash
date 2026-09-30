%token NAME "name"
%start message
%%
unused: NAME ;
message: "name" '!' ;
%%
