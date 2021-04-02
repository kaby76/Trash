grammar Academic;

e : fe  ( ( '+' | '-' | '*' | '/' ) fe )*;
fe : id | num | ( '+' | '-' ) fe | e;
id : 'id';
num : 'num';
