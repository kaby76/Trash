// This code from https://begriffs.com/posts/2021-11-28-practical-parsing.html
/* irc.l  - complete file */

%option noyywrap nounput noinput

%{
#include "irc.tab.h"

#define _XOPEN_SOURCE 600

#include <limits.h>
#include <stdlib.h>
#include <string.h>
%}

re_space    [ ]+
re_host     [[:alnum:]][[:alnum:]\.\-]*
re_nick     [[:alpha:]][[:alnum:]\-\[\]\\`^{}_]*
re_user     [~[:alpha:]][[:alnum:]]*
re_keyname  [[:alnum:]\-]+
re_keyval   [^ ;\r\n]*
re_command  [[:alpha:]]+|[[:digit:]]{3}
re_middle   [^: \r\n][^ \r\n]*
re_trailing [^\r\n]*

%x IN_TAGS IN_PREFIX IN_PARAMS

%%

@ { BEGIN IN_TAGS; return *yytext; }
: { BEGIN IN_PREFIX; return *yytext; }

{re_space} { return SPACE; }
{re_command} {
	yylval.str = strdup(yytext);
	BEGIN IN_PARAMS;
	return COMMAND;
}


<IN_TAGS>\+?({re_host}\/)?{re_keyname}(={re_keyval})?  {
	struct map_pair *p = malloc(sizeof *p);
	char *split = strchr(yytext, '=');
	if (split)
		*split = '\0';
	*p = (struct map_pair){
		.k = strdup(yytext),
		.v = split ? strdup(split+1) : calloc(1,1)
	};
	yylval.pair = p;
	return TAG;
}
<IN_TAGS>{re_space} {
	BEGIN INITIAL;
	return SPACE;
}
<IN_TAGS>; { return ';'; }


<IN_PREFIX>({re_host})|({re_nick})(!{re_user})?(@{re_host})? {
	struct prefix *p = malloc(sizeof *p);
	if (!p)
		goto done;
	*p = (struct prefix){0};
	char *bang = strchr(yytext, '!'),
	     *at   = strchr(yytext, '@');
	if (!bang && !at)
	{
		p->host = strdup(yytext);
		goto done;
	}
	if (bang) *bang = '\0';
	if (at) *at = '\0';
	p->nick = strdup(yytext);
	if (bang)
		p->user = strdup(bang+1);
	if (at)
		p->host = strdup(at+1);
done:
	yylval.prefix = p;
	BEGIN INITIAL;
	return PREFIX;
}


<IN_PARAMS>{re_space} { return SPACE; }
<IN_PARAMS>{re_middle} {
	yylval.str = strdup(yytext);
	return MIDDLE;
}
<IN_PARAMS>:{re_trailing} {
	yylval.str = strdup(yytext+1); /* trim : */
	BEGIN INITIAL;
	return TRAILING;
}

<*>\n|\r\n  ; /* ignore */