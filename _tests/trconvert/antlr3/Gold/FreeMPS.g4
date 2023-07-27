
grammar FreeMPS;

options {
    	language=CSharp2;

}

/*------------------------------------------------------------------
 * PARSER RULES
 *------------------------------------------------------------------*/
//genereller Aufbau einer MPS-Datei
modell			: firstrow rows columns rhs ranges? bounds? endata EOF; //Aufbau eines MPS Dokuments

//Aufbau der einzelnen Sections
firstrow		: NAMEINDICATORCARD BEZEICHNER? KEYWORDFREE?;	//1. Zeile: Indicatorcard "NAME" und "Name des Modells" und evtl Keyword "FREE" das es sich um FreeMPS handelt

rows			: ROWINDICATORCARD (rowdatacard)+;				//ROW Sektor: Indicatorcard und mindestens eine rowdatacard

columns			: COLUMNINDICATORCARD columndatacards;			//COLUMNS Sektor: Indicatorcard und zugeh�rige columndatacards

rhs				: RHSINDICATORCARD rhsdatacards;				//RHS Sektor: Indicatorcard und zugeh�rige rhsdatacards

ranges			: RANGESINDICATORCARD rangesdatacards;			//optionaler RANGES Sektor: Indicatorcard und zugeh�rige rangesdatacards

bounds			: BOUNDSINDICATORCARD boundsdatacards;			//optionaler BOUNDS Sektor: Indicatorcard und zugeh�rige boundsdatacards

endata			: ENDATAINDICATORCARD;							//letzte ZEILE: Indicatorcard "ENDATA"

//Aufbau der Datacards der einzelnen Sections
rowdatacard		: ROWTYPE BEZEICHNER;								//Rowdatacard: Zeilentyp und Zeilenname

columndatacards		: (columndatacard | intblock)+;					//columndatacards: beliebeige "normale" columndatacards oder ein Integer-Bereiche

rhsdatacards		: rhsdatacard+;									//rhsdatacards bestehen aus mindestens einer rhsdatacard

rangesdatacards		: rangesdatacard+;								//rangedatacards bestehen aus mindestens einer rangesdatacard

boundsdatacards		: boundsdatacard+;								//boundsdatacards bestehen aus mindestens einer boundsdatacard

//Aufbaue einer einzelnen Datacard einer Section
columndatacard		: BEZEICHNER BEZEICHNER NUMERICALVALUE (BEZEICHNER NUMERICALVALUE)?;						//Eine Datacard f�r Column bestehend aus einer Nichtnullvariablen und ein oder zwei Zeilennamen samt Wert

rhsdatacard			: (BEZEICHNER | RHSINDICATORCARD) BEZEICHNER NUMERICALVALUE (BEZEICHNER NUMERICALVALUE)?;	//Eine Datacard f�r RHS bestehend aus einem Namen f�r die RHS-Spalte und einem Zeilennamen samt Wert

rangesdatacard		: (BEZEICHNER | RANGESINDICATORCARD) BEZEICHNER NUMERICALVALUE (BEZEICHNER NUMERICALVALUE)?;//Eine Datacard f�r Ranges bestehend aus einem Namen f�r die RANGES-Spalte und einer oder zwei Zeilennamen samt Wert

boundsdatacard		: BOUNDKEY (BEZEICHNER | BOUNDSINDICATORCARD) BEZEICHNER NUMERICALVALUE?;					//Eine Datacard f�r Bounds bestehend aus einem Boundskey, einem Namen f�r die BOUNDS-Spalte und einer Nichtnullvariablen samt Grnze/Wert (kann z.B. bei FR entfallen)

//Aufbau eines INT-Block der COLUMNS-Section
intblock		: startmarker columndatacard+  endmarker; 		//Integer-Bereich f�r Int Variablen: StartZeile, mind. eine columndatacard und die Endzeile

startmarker		: BEZEICHNER KEYWORDMARKER STARTMARKER;			//Startmarkierung f�r Integer

endmarker		: BEZEICHNER KEYWORDMARKER ENDMARKER;			//Endmarkierung f�r Integer


/*------------------------------------------------------------------
 * LEXER RULES
 *------------------------------------------------------------------*/

//�berschriften der einzelnen Sections

NAMEINDICATORCARD	: 'NAME';

ROWINDICATORCARD 	: 'ROWS';

COLUMNINDICATORCARD : 'COLUMNS';

RHSINDICATORCARD	: 'RHS';

RANGESINDICATORCARD	: 'RANGES';

BOUNDSINDICATORCARD	: 'BOUNDS';

ENDATAINDICATORCARD: 'ENDATA';

//Markierungsw�rter f�r einen Int-Bereicch der COLUMNS-Section

KEYWORDMARKER	: '\'MARKER\'';

STARTMARKER		: '\'INTORG\'';

ENDMARKER		: '\'INTEND\'';

//Sonstige Keywords und Bezeichner und Werte

KEYWORDFREE		: 'FREE';

BOUNDKEY		: ('UP' | 'LO' | 'FX' | 'LI' | 'UI' | 'SC'| 'FR' | 'BV' | 'MI' | 'PL');

ROWTYPE			: ('E' | 'L' | 'G' | 'N');


BEZEICHNER 		: LETTER ZEICHEN*;

NUMERICALVALUE	: DIGIT DIGITS*;




/*------------------------------------------------------------------
 * Diese Zeichen allein sind noch keine Token
 *------------------------------------------------------------------*/

WS 				: (' ' | '\t' | '\n' | '\r' | '\f')+;
LINE_COMMENT	: ('*' | '$') ~('\n'|'\r')* '\r'? '\n';
fragment ZEICHEN: (LETTER | DIGIT) ;			//Ein Zeichen ist noch kein Token, besteht aber aus ein einem Buchstaben oder einer Zahl
fragment LETTER	: ('a'..'z' | 'A'..'Z' | '_' | '/' | '#' | '@' | '(' | ')');//Ein Buchstabe ist noch kein Token
fragment DIGIT	: '0'..'9' | '-' | '+' | '.' | ',' ;	//Eine Ziffer ist noch kein Token
fragment DIGITS	: DIGIT | 'D' | 'E' | 'e' | 'd';	//Eine Ziffer ist noch kein Token




