grammar t2;
assignment : ( ( ( ( '=>' ) ) | ( ( '->' ) ) ) ? ( ( validID ) ) ( ( ( '+=' | '=' | '?=' ) ) ) ( ( assignableTerminal ) ) ) ;
// It should be assignment : ( ( '=>' | '->' ) ? validID ( '+=' | '=' | '?=' ) assignableTerminal ) ;
