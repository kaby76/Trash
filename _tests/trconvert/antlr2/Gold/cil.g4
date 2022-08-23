



/* PARSER */

grammar CIL;

options
{
	importVocab=CILLexer;
}

start: declarations
	;


//=== DECLARATIONS ===

declarations: (declaration)* EOF
	;

declaration: classDeclaration
	| namespaceDeclaration
	| methodDeclaration
	| fieldDeclaration
	| dataDeclaration
	| vtableDeclaration
	| vtfixupDeclaration
	| extSourceSpec
	| fileDeclaration
	| assemblyDeclaration
	| assemblyRefDeclaration
	| comtypeDeclaration
	| mresourceDeclaration
	| moduleDeclaration
	| securityDeclaration
	| customAttributDeclaration
	| languageDeclaration
	| ignoredDeclaration
	;


//=== LANGUAGE ===

languageDeclaration: DOT_LANGUAGE SQSTRING COMMA SQSTRING COMMA SQSTRING
	| DOT_LANGUAGE SQSTRING COMMA SQSTRING
	| DOT_LANGUAGE SQSTRING
	;


//=== MODULE ===

moduleDeclaration: DOT_MODULE EXTERNname
	| DOT_MODULEname
	| DOT_MODULE
	;

//=== IGNORED DECLARATION ===

ignoredDeclaration: DOT_SUBSYSTEM int32
	| DOT_CORFLAGS int32
	| DOT_FILE ALIGNMENT int32
	| DOT_IMAGEBASE int64
	;

//=== FILE ===

fileDeclaration
	: DOT_FILE fileAttributsname fileEntry DOT_HASH EQUAL LPAREN bytes RPAREN fileEntry					
	| DOT_FILE fileAttributsname fileEntry
	;

fileEntry: (DOT_ENTRYPOINT)?
	;

fileAttributs: (fileAttribut)*
	;

fileAttribut: NOMETADATA
	;


//=== VTFIXUP ===

vtfixupDeclaration: DOT_VTFIXUP LBRACK int32 RBRACK vtfixupAttributs AT id
	;

vtfixupAttributs: (vtfixupAttribut)*
	;

vtfixupAttribut: (INT32 | INT64 | FROMUNMANAGED | CALLMOSTDERIVED)
	;


//=== VTABLE ===

vtableDeclaration
	: DOT_VTABLE EQUAL LPAREN bytes RPAREN
	;

//=== SECURITY ===

securityDeclaration
	: DOT_PERMISSION securityAction typeSpec LPAREN nameValPairs RPAREN
	| DOT_PERMISSION securityAction typeSpec
	| DOT_PERMISSIONSET securityAction EQUAL LPAREN bytes RPAREN
	| DOT_CAPABILITY securityAction EQUAL LPAREN bytes RPAREN
	;

securityAction: REQUEST
	| DEMAND
	| ASSERT
	| DENY
	| PERMITONLY
	| LINKCHECK
	| INHERITCHECK
	| REQMIN
	| REQOPT
	| REQREFUSE
	| PREJITGRANT
	| PREJITDENY
	| NONCASDEMAND
	| NONCASLINKDEMAND
	;

nameValPairs: nameValPair (COMMA nameValPair)*
	;

nameValPair: compQString EQUAL caValue
	;

caValue: truefalse
	| int32
	| INT32 LPAREN int32 RPAREN
	| compQString
	| className LPAREN INT8  COLON int32 RPAREN
	| className LPAREN INT16 COLON int32 RPAREN
	| className LPAREN INT32 COLON int32 RPAREN
	| className LPAREN int32 RPAREN
	;

//=== EXT SOURCE SPEC === (IGNORED)
extSourceSpec: DOT_LINE int32 (COLON int32 (SQSTRING)?
								| SQSTRING)?

	//| P_LINE int32 DQSTRING				// #line (not supported)
	;

//=== CUSTOM ATTRIBUT ===

customAttributDeclaration
	: DOT_CUSTOM customType EQUAL LPAREN bytes RPAREN
	| DOT_CUSTOM customType EQUAL compQString
	| DOT_CUSTOM customType
	| DOT_CUSTOM LPAREN ownerType RPAREN customType EQUAL LPAREN bytes RPAREN
	| DOT_CUSTOM LPAREN ownerType RPAREN customType
	;

customType: callConv VOID /*type*/ typeSpec DOUBLE_COLON DOT_CTOR LPAREN signatureArguments RPAREN
	;


//=== ASSEMBLY ===

assemblyDeclaration:assemblyHead LCURLYassemblyBodyDeclarations RCURLY
	;

assemblyHead
	: DOT_ASSEMBLYassemblyAttributsname
	;

assemblyAttributs: (assemblyAttribut)*
	;

assemblyAttribut: NOAPPDOMAIN
	| NOPROCESS
	| NOMACHINE
	;

assemblyBodyDeclarations: (assemblyBodyDeclaration)*
	;

assemblyBodyDeclaration: DOT_HASH ALGORITHM int32
	| securityDeclaration
	| asmOrRefDeclaration
	;

asmOrRefDeclaration
	: DOT_PUBLICKEY EQUAL LPAREN bytes RPAREN
	| DOT_VERint32 COLONint32 COLONint32 COLONint32
	| DOT_LOCALEcompQString
	| DOT_LOCALE EQUAL LPAREN bytes RPAREN
	| customAttributDeclaration
	;

assemblyRefDeclaration:assemblyRefHead LCURLYassemblyRefBodyDeclarations RCURLY
	;

assemblyRefHead: DOT_ASSEMBLY EXTERN name (AS name)?
	;

assemblyRefBodyDeclarations: (assemblyRefBodyDeclaration)*
	;

assemblyRefBodyDeclaration
	: ! DOT_HASH EQUAL LPAREN bytes RPAREN
	| asmOrRefDeclaration
	| ! DOT_PUBLICKEYTOKEN EQUAL LPAREN bytes RPAREN
	;


//=== COMTYPES === (IGNORED)

comtypeDeclaration: comtypeHead LCURLY comtypeBodyDeclarations RCURLY
	;

comtypeHead: DOT_CLASS EXTERN comtAttributs name
	;

comtAttributs: (comtAttribut)*
	;

comtAttribut: PRIVATE
	|  PUBLIC
	| NESTED PUBLIC
	| NESTED PRIVATE
	| NESTED FAMILY
	| NESTED ASSEMBLY
	| NESTED FAMANDASSEM
	| NESTED FAMORASSEM
	;

comtypeBodyDeclarations: (comtypeBodyDeclaration)*
	;

comtypeBodyDeclaration:	DOT_FILE name
	| DOT_CLASS EXTERN name
	| DOT_CLASS int32
	| customAttributDeclaration
	;

exportDeclaration: exportHead LCURLY comtypeBodyDeclarations RCURLY
	;

exportHead: DOT_EXPORT comtAttributs name
	;


//=== Manifest Resources (IGNORED)

mresourceDeclaration: mresourceHead LCURLY mresourceBodyDeclarations RCURLY
	;

mresourceHead: DOT_MRESOURCE mresourceAttributs name
	;

mresourceAttributs: (mresourceAttribut)*
	;

mresourceAttribut: (PUBLIC | PRIVATE)
	;

mresourceBodyDeclarations: (mresourceBodyDeclaration)*
	;

mresourceBodyDeclaration: DOT_FILE name AT int32
	| DOT_ASSEMBLY EXTERN name
	| customAttributDeclaration
	;


//=== NAMESPACE ===

namespaceDeclaration:namespaceHead LCURLYnamespaceBodyDeclarations RCURLY
	;

namespaceHead
	: DOT_NAMESPACEname
	;

namespaceBodyDeclarations: (declaration)*
	;

//=== CLASS ===

classDeclaration:classHead LCURLYclassBodyDeclarations RCURLY
	;

classHead
	: DOT_CLASSclassAttributsidextendsClauseimplementsClause
	;

classAttributs: (classAttribut)*
	;

classAttribut: (PUBLIC
	| PRIVATE
	| VALUE
	| ENUM
	| INTERFACE
	| SEALED
	| ABSTRACT
	| AUTO
	| SEQUENTIAL
	| EXPLICIT
	| ANSI
	| UNICODE
	| AUTOCHAR
	| IMPORT
	| SERIALIZABLE
	| NESTED PUBLIC
	| NESTED PRIVATE
	| NESTED FAMILY
	| NESTED ASSEMBLY
	| NESTED FAMANDASSEM
	| NESTED FAMORASSEM
	| BEFOREFIELDINIT
	| SPECIALNAME
	| RTSPECIALNAME)
	;

extendsClause: (EXTENDSclassName )?
	;

implementsClause: (IMPLEMENTSclassNames )?
	;

classBodyDeclarations: (classBodyDeclaration)*
	;

classBodyDeclaration: methodDeclaration
	| classDeclaration
	| eventDeclaration
	| propertyDeclaration
	| fieldDeclaration
	| dataDeclaration
	| securityDeclaration
	| extSourceSpec
	| customAttributDeclaration
	| DOT_SIZE int32
	| DOT_PACK int32
	| exportDeclaration
	| languageDeclaration
	| overrideDeclaration
	;

overrideDeclaration: DOT_OVERRIDE typeSpec DOUBLE_COLON methodName WITH callConv type typeSpec DOUBLE_COLON methodName LPAREN signatureArguments RPAREN
	;


//=== FIELD ===

fieldDeclaration: DOT_FIELDoffsetOptfieldAttributstypeidatOptinitOpt
	;

offsetOpt: (LBRACK int32 RBRACK)?
	;

atOpt: (AT id)?
	;

initOpt: (EQUAL fieldInit)?
	;

fieldInit
 	: FLOAT32 LPAREN (float64 | int64) RPAREN
	| FLOAT64 LPAREN (float64 | int64) RPAREN
	| INT64 LPAREN int64 RPAREN
	| INT32 LPAREN int64 RPAREN
	| INT16 LPAREN int64 RPAREN
	| CHAR LPAREN int64 RPAREN
	| INT8 LPAREN int64 RPAREN
	| BOOL LPAREN truefalse RPAREN
	| compQString
	| BYTEARRAY LPAREN bytes RPAREN
	| NULLREF
	;

fieldAttributs: (fieldAttribut)*
	;

fieldAttribut: (PUBLIC
	| PRIVATE
	| SPECIALNAME
	| RTSPECIALNAME
	| LITERAL
	| STATIC
	| FAMILY
	| INITONLY
	| ASSEMBLY
	| FAMANDASSEM
	| FAMORASSEM
	| PRIVATESCOPE
	| NOTSERIALIZED
	| MARSHAL LPAREN nativeType RPAREN
	)
	;


//=== DATA ===

dataDeclaration:dataHead LCURLYdataItemList RCURLY
	|dataHeaddataItem
	;

dataHead: DOT_DATA id EQUAL
	| DOT_DATA TLS id EQUAL
	
	// should be: DOT_DATA (TLS)? (id)? EQUAL
	;

dataItemList: dataItem (COMMA dataItem)*
	;

dataItemCount: (LBRACK int32 RBRACK)?
	;

dataItem
	: CHAR STAR LPAREN compQString RPAREN
	| REF LPAREN id RPAREN
	| BYTEARRAY LPAREN bytes RPAREN
	| (FLOAT32|FLOAT64) LPAREN float64 RPAREN dataItemCount
	| INT64 LPAREN int64 RPAREN dataItemCount
	| INT32 LPAREN int32 RPAREN dataItemCount
	| INT16 LPAREN int32 RPAREN dataItemCount
	| INT8 LPAREN int32 RPAREN dataItemCount
	| FLOAT64 dataItemCount
	| FLOAT32 dataItemCount
	| INT64 dataItemCount
	| INT32 dataItemCount
	| INT16 dataItemCount
	| INT8 dataItemCount
	;


//=== EVENT ===

eventDeclaration:eventHead LCURLYeventBodyDeclarations RCURLY
	;

eventHead
	: DOT_EVENTeventAttributs (typeSpec)?id
	;

eventAttributs: (eventAttribut)*
	;

eventAttribut: SPECIALNAME
	| RTSPECIALNAME
	;

eventBodyDeclarations: (eventBodyDeclaration)*
	;

eventBodyDeclaration: eventAddOn
	| eventRemoveOn
	| eventFire
	| eventOther
	| customAttributDeclaration
	| languageDeclaration
	| extSourceSpec
	;

eventAddOn: DOT_ADDON eventMethod
	;

eventRemoveOn: DOT_REMOVEON eventMethod
	;

eventFire: DOT_FIRE eventMethod
	;

eventOther: DOT_OTHER eventMethod
	;

eventMethod: callConv type typeSpec DOUBLE_COLON methodName LPAREN signatureArguments RPAREN
	| callConv type methodName LPAREN signatureArguments RPAREN
	;


//=== PROPERTY ===

propertyDeclaration:propertyHead LCURLYpropertyBodyDeclarations RCURLY
	;

propertyHead
	: DOT_PROPERTYpropAttributs callConvtypeid LPARENsignatureArguments RPARENinitOpt
	;

propAttributs: (propAttribut)*
	;

propAttribut: SPECIALNAME
	| RTSPECIALNAME
	;

propertyBodyDeclarations: (propertyBodyDeclaration)*
	;

propertyBodyDeclaration: propSet
	| propGet
	| propOther
	| customAttributDeclaration
	| languageDeclaration
	| extSourceSpec
	;

propGet: DOT_GET propMethod
	;

propSet: DOT_SET propMethod
	;

propOther: DOT_OTHER propMethod
	;

propMethod: callConv type typeSpec DOUBLE_COLON methodName LPAREN signatureArguments RPAREN
	| callConv type methodName LPAREN signatureArguments RPAREN
	;


//=== METHOD ===

methodDeclaration:methodHead LCURLYmethodBodyDeclarations RCURLY
	;

methodHead
	: DOT_METHODmethodAttributs callConv paramAttributstype (MARSHAL LPAREN nativeType RPAREN)?methodName
	LPARENsignatureArguments RPARENimplAttributs
	;

methodAttributs: (methodAttribut)*
	;

methodAttribut: (STATIC
	| PUBLIC
	| PRIVATE
	| FAMILY
	| FINAL
	| SPECIALNAME
	| VIRTUAL
	| ABSTRACT
	| ASSEMBLY
	| FAMANDASSEM
	| FAMORASSEM
	| PRIVATESCOPE
	| HIDEBYSIG
	| NEWSLOT
	| RTSPECIALNAME
	| UNMANAGEDEXP
	| REQSECOBJ
	| STRICT
	| pinvokeMethodAttribut)
	;

pinvokeMethodAttribut: PINVOKEIMPL LPAREN compQString AS compQString pinvokeAttributs RPAREN
	| PINVOKEIMPL LPAREN compQString pinvokeAttributs RPAREN
	| PINVOKEIMPL LPAREN pinvokeAttributs RPAREN
	;

pinvokeAttributs: (pinvokeAttribut)*
	;

pinvokeAttribut: NOMANGLE
	| ANSI
	| UNICODE
	| AUTOCHAR
	| LASTERR
	| WINAPI
	| CDECL
	| STDCALL
	| THISCALL
	| FASTCALL
	;

implAttributs: (implAttribut)*
	;

implAttribut: (NATIVE
	| CIL
	| OPTIL
	| MANAGED
	| UNMANAGED
	| FORWARDREF
	| PRESERVESIG
	| RUNTIME
	| INTERNALCALL
	| SYNCHRONIZED
	| NOINLINING)
	;

paramAttributs: (paramAttribut)*
	;

paramAttribut: (LBRACK IN RBRACK
	| LBRACK OUT RBRACK
	| LBRACK OPT RBRACK
	| LBRACK int32 RBRACK)
	;

signatureArguments: (signatureArgumentList)?
	;

signatureArgumentList: signatureArgument (COMMA signatureArgument)*
	;

signatureArgument: (TRIPLE_DOT
	| paramAttributs type MARSHAL LPAREN nativeType RPAREN (id)?
	| paramAttributs type id
	| paramAttributs type)
	;

methodName: name
	| DOT_CTOR
	| DOT_CCTOR
	;

methodBodyDeclarations: (methodBodyDeclaration)*
	;

methodBodyDeclaration: DOT_EMITBYTE int32
	| sehBlock
	| maxStackDeclaration
	| localsDeclaration
	| DOT_ENTRYPOINT
	| languageDeclaration
	| dataDeclaration
	| codeLabelDeclaration
	| customAttributDeclaration
	| instr
	| DOT_ZEROINIT
	| securityDeclaration
	| extSourceSpec
	| DOT_VTENTRY int32 COLON int32
	| DOT_OVERRIDE typeSpec DOUBLE_COLON methodName
	| scopeBlock
	| DOT_PARAM LBRACK int32 RBRACK initOpt
	| DOT_EXPORT LBRACK int32 RBRACK (AS id)?
	;

maxStackDeclaration: DOT_MAXSTACK int32
	;

localsDeclaration: DOT_LOCALS (INIT)? LPAREN signatureArguments RPAREN
	;

codeLabelDeclaration: ID COLON
	;

scopeBlock: LCURLYmethodBodyDeclarations RCURLY
	;

sehBlock:tryBlocksehClauses
	;

sehClauses: (sehClause)+
	;

tryBlock: DOT_TRY scopeBlock
	| DOT_TRY id TO id
	| DOT_TRY int32 TO int32
	;

sehClause: catchClause handlerBlock
	| filterClause handlerBlock
	| finallyClause handlerBlock
	| faultClause handlerBlock
	;

filterClause: FILTER scopeBlock
	| FILTER id
	| FILTER int32
	;

catchClause: CATCH className
	;

finallyClause: FINALLY
	;

faultClause: FAULT
	;

handlerBlock: scopeBlock
	| HANDLER id TO id
	| HANDLER int32 TO int32
	;

// type declaration
type: typeRoot typePostFix
	;

typeRoot: CLASS className
	| OBJECT
	| STRING
	| VALUE CLASS className
	| VALUETYPE className
	| EXCLAM int32
	//| METHOD callConv type STAR LPAREN signatureArguments RPAREN   // TODO
	| TYPEDREF
	| CHAR
	| VOID
	| BOOL
	| INT8
	| INT16
	| INT32
	| INT64
	| FLOAT32
	| FLOAT64
	| UNSIGNED INT8
	| UNSIGNED INT16
	| UNSIGNED INT32
	| UNSIGNED INT64
	| NATIVE INT
	| NATIVE UNSIGNED INT
	| NATIVE FLOAT
	;

typePostFix: (REF
	| STAR
	| PINNED
	| MODREQ LPAREN className RPAREN
	| MODOPT LPAREN className RPAREN
	| arrayPostFix)*
	;

arrayPostFix: LBRACK bounds RBRACK
	;

bounds: // EMPTY
	| bound (COMMA bound)*
	;

bound:  TRIPLE_DOT
	| int32
	| (int32 TRIPLE_DOT int32)
	| int32 TRIPLE_DOT
	;

ownerType : typeSpec
	| memberRef
	;

memberRef : METHOD callConv type typeSpec DOUBLE_COLON methodName LPAREN signatureArguments RPAREN
	| METHOD callConv type methodName LPAREN signatureArguments RPAREN
	| FIELD type typeSpec DOUBLE_COLON id
	| FIELD type id
	;

nativeType: nativeTypeRoot nativeTypePostFix
	;

nativeTypeRoot: // EMPTY
	| CUSTOM LPAREN ( compQString (COMMA compQString)* ) RPAREN
	| FIXED SYSSTRING LBRACK int32 RBRACK
	| FIXED ARRAY LBRACK int32 RBRACK
	| VARIANT
	| CURRENCY
	| SYSCHAR
	| VOID
	| BOOL
	| INT8
	| INT16
	| INT32
	| INT64
	| FLOAT32
	| FLOAT64
	| ERROR
	| UNSIGNED INT8
	| UNSIGNED INT16
	| UNSIGNED INT32
	| UNSIGNED INT64
	| DECIMAL
	| DATE
	| BSTR
	| LPSTR
	| LPWSTR
	| LPTSTR
	| OBJECTREF
	| IUNKNOWN
	| IDISPATCH
	| STRUCT
	| INTERFACE
	| SAFEARRAY variantType COMMA compQString
	| SAFEARRAY variantType
	| INT
	| UNSIGNED INT
	| NESTED STRUCT
	| BYVALSTR
	| ANSI BSTR
	| TBSTR
	| VARIANT BOOL
	| METHOD
	| AS ANY
	| LPSTRUCT
	;

nativeTypePostFix: (STAR
	| LBRACK RBRACK
	| LBRACK int32 PLUS int32 RBRACK
	| LBRACK int32 RBRACK
	| LBRACK PLUS int32 RBRACK)*
	;

variantType: variantTypeRoot variantTypePostFix
	;

variantTypeRoot: NULL
	| VARIANT
	| CURRENCY
	| VOID
	| BOOL
	| INT8
	| INT16
	| INT32
	| INT64
	| FLOAT32
	| FLOAT64
	| UNSIGNED INT8
	| UNSIGNED INT16
	| UNSIGNED INT32
	| UNSIGNED INT64
	| STAR
	| DECIMAL
	| DATE
	| BSTR
	| LPSTR
	| LPWSTR
	| IUNKNOWN
	| IDISPATCH
	| SAFEARRAY
	| INT
	| UNSIGNED INT
	| ERROR
	| HRESULT
	| CARRAY
	| USERDEFINED
	| RECORD
	| FILETIME
	| BLOB
	| STREAM
	| STORAGE
	| STREAMED_OBJECT
	| BLOB_OBJECT
	| CF
	| CLSID
	;

variantTypePostFix: (LBRACK RBRACK
	| REF
	| VECTOR)*
	;

typeSpec : className
	| type
	;

callConv: (INSTANCE (EXPLICIT)? callKind)?
	;

callKind : (DEFAULT
	| VARARG
	| UNMANAGED (	CDECL
			| STDCALL
			| THISCALL
			| FASTCALL)
	)?
	;

// instruction
instr
	: (instr_none
	| instr_var (int32 | id)
	| instr_i int32
	| instr_i8 int64
	| instr_r float64
	| instr_r int64
	| instr_r LPAREN bytes RPAREN
	| instr_brtarget int32
	| instr_brtarget id
	| instr_method callConv type typeSpec DOUBLE_COLON methodName LPAREN signatureArguments RPAREN
	| instr_method callConv type methodName LPAREN signatureArguments RPAREN
	| instr_field type typeSpec DOUBLE_COLON id
	| instr_field type id
	| instr_type typeSpec
	| instr_string compQString
	| instr_string BYTEARRAY LPAREN bytes RPAREN
	| instr_sig callConv type LPAREN signatureArguments RPAREN
	| instr_tok ownerType
	| instr_switch LPAREN labels RPAREN)
	;

// switch labels
labels : // EMPTY
	| (id|int32) (COMMA (id|int32))*
	;

// instruction list
instr_none : ADD | ADD_OVF | ADD_OVF_UN | AND
	| ARGLIST | BREAK | CEQ | CGT
	| CGT_UN | CKFINITE | CLT | CLT_UN
	| CONV_I | CONV_I1 | CONV_I2 | CONV_I4
	| CONV_I8 | CONV_OVF_I | CONV_OVF_I_UN | CONV_OVF_I1
	| CONV_OVF_I1_UN | CONV_OVF_I2 | CONV_OVF_I2_UN | CONV_OVF_I4
	| CONV_OVF_I4_UN | CONV_OVF_I8 | CONV_OVF_I8_UN | CONV_OVF_U
	| CONV_OVF_U_UN | CONV_OVF_U1 | CONV_OVF_U1_UN | CONV_OVF_U2
	| CONV_OVF_U2_UN | CONV_OVF_U4 | CONV_OVF_U4_UN | CONV_OVF_U8
	| CONV_OVF_U8_UN | CONV_R_UN | CONV_R4 | CONV_R8
	| CONV_U | CONV_U1 | CONV_U2 | CONV_U4
	| CONV_U8 | CPBLK | DIV | DIV_UN
	| DUP | ENDFAULT | ENDFILTER | ENDFINALLY
	| INITBLK | LDARG_0 | LDARG_1
	| LDARG_2 | LDARG_3 | LDC_I4_0 | LDC_I4_1
	| LDC_I4_2 | LDC_I4_3 | LDC_I4_4 | LDC_I4_5
	| LDC_I4_6 | LDC_I4_7 | LDC_I4_8 | LDC_I4_M1
	| LDELEM_I | LDELEM_I1 | LDELEM_I2 | LDELEM_I4
	| LDELEM_I8 | LDELEM_R4 | LDELEM_R8 | LDELEM_REF
	| LDELEM_U1 | LDELEM_U2 | LDELEM_U4 | LDIND_I
	| LDIND_I1 | LDIND_I2 | LDIND_I4 | LDIND_I8
	| LDIND_R4 | LDIND_R8 | LDIND_REF | LDIND_U1
	| LDIND_U2 | LDIND_U4 | LDLEN | LDLOC_0
	| LDLOC_1 | LDLOC_2 | LDLOC_3 | LDNULL
	| LOCALLOC | MUL | MUL_OVF | MUL_OVF_UN
	| NEG | NOP | NOT | OR
	| POP | REFANYTYPE | REM | REM_UN
	| RET | RETHROW | SHL | SHR
	| SHR_UN | STELEM_I | STELEM_I1 | STELEM_I2
	| STELEM_I4 | STELEM_I8 | STELEM_R4 | STELEM_R8
	| STELEM_REF | STIND_I | STIND_I1 | STIND_I2
	| STIND_I4 | STIND_I8 | STIND_R4 | STIND_R8
	| STIND_REF | STLOC_0 | STLOC_1 | STLOC_2
	| STLOC_3 | SUB | SUB_OVF | SUB_OVF_UN
	| TAIL_ | THROW | VOLATILE_ | XOR
	;

instr_var: LDARG
	| LDARG_S
	| LDARGA
	| LDARGA_S
	| LDLOC
	| LDLOC_S
	| LDLOCA
	| LDLOCA_S
	| STARG
	| STARG_S
	| STLOC
	| STLOC_S
	;

instr_i : LDC_I4
	| LDC_I4_S
	| UNALIGNED_
	;

instr_i8 : LDC_I8
	;

instr_r : LDC_R4 | LDC_R8
	;

instr_brtarget : BEQ | BEQ_S | BGE | BGE_S
	| BGE_UN | BGE_UN_S | BGT | BGT_S | BGT_UN | BGT_UN_S
	| BLE | BLE_S | BLE_UN | BLE_UN_S | BLT | BLT_S
	| BLT_UN | BLT_UN_S | BNE_UN | BNE_UN_S | BR | BR_S
	| BRFALSE | BRFALSE_S | BRTRUE | BRTRUE_S | LEAVE | LEAVE_S
	;

instr_method : CALL | CALLVIRT | JMP | LDFTN | LDVIRTFTN | NEWOBJ
	;

instr_field : LDFLD | LDFLDA | LDSFLD | LDSFLDA | STFLD | STSFLD
	;

instr_type :
	BOX | CASTCLASS | CPOBJ | INITOBJ | ISINST |
	LDELEMA | LDOBJ | MKREFANY | NEWARR | REFANYVAL |
	SIZEOF | STOBJ | UNBOX
	;

instr_string : LDSTR ;

instr_sig : CALLI ;

instr_tok : LDTOKEN ;

instr_switch : SWITCH ;


// base types
id: ID
	| SQSTRING
	;

// dotted name (id . id . id...)
name
	:id (DOTid )*
	;

compQString
	:DQSTRING (PLUSDQSTRING )*
	;

className : LBRACK DOT_MODULE name RBRACK slashedName
	| LBRACK name RBRACK slashedName
	| slashedName
	;

classNames: className (COMMA className)*
	;

slashedName: name (SLASH name)*
	;

int32: INTEGER_LITERAL
	;

int64: INTEGER_LITERAL
	;

float64: FLOAT_LITERAL
	| FLOAT32 LPAREN int32 RPAREN
	| FLOAT64 LPAREN int64 RPAREN
	;

truefalse : TRUE
	| FALSE
	;

bytes
	: ( hex_byte )*
	;

hex_byte :
	(ID
	|HEX_BYTE
	|INTEGER_LITERAL )
	; 
DOT_ASSEMBLY : '.assembly';
 
DOT_ADDON : '.addon';
 
DOT_CCTOR : '.cctor';
 
DOT_CLASS : '.class';
 
DOT_CAPABILITY : '.capability';
 
DOT_CORFLAGS : '.corflags';
 
DOT_CTOR : '.ctor';
 
DOT_CUSTOM : '.custom';
 
DOT_DATA : '.data';
 
DOT_EMITBYTE : '.emitbyte';
 
DOT_ENTRYPOINT : '.entrypoint';
 
DOT_EVENT : '.event';
 
DOT_EXPORT : '.export';
 
DOT_FIELD : '.field';
 
DOT_FILE : '.file';
 
DOT_FIRE : '.fire';
 
DOT_GET : '.get';
 
DOT_HASH : '.hash';
 
DOT_IMAGEBASE : '.imagebase';
 
DOT_IMPORT : '.import';
 
DOT_LANGUAGE : '.language';
 
DOT_LINE : '.line';
 
DOT_LOCALE : '.locale';
 
DOT_LOCALIZED : '.localized';
 
DOT_LOCALS : '.locals';
 
DOT_MANIFESTRES : '.manifestres';
 
DOT_MAXSTACK : '.maxstack';
 
DOT_METHOD : '.method';
 
DOT_MODULE : '.module';
 
DOT_MRESOURCE : '.mresource';
 
DOT_NAMESPACE : '.namespace';
 
DOT_OTHER : '.other';
 
DOT_OVERRIDE : '.override';
 
DOT_PACK : '.pack';
 
DOT_PARAM : '.param';
 
DOT_PDIRECT : '.pdirect';
 
DOT_PERMISSION : '.permission';
 
DOT_PERMISSIONSET : '.permissionset';
 
DOT_PROPERTY : '.property';
 
DOT_PUBLICKEY : '.publickey';
 
DOT_PUBLICKEYTOKEN : '.publickeytoken';
 
DOT_REMOVEON : '.removeon';
 
DOT_SET : '.set';
 
DOT_SIZE : '.size';
 
DOT_SUBSYSTEM : '.subsystem';
 
DOT_TRY : '.try';
 
DOT_VER : '.ver';
 
DOT_VTABLE : '.vtable';
 
DOT_VTENTRY : '.vtentry';
 
DOT_VTFIXUP : '.vtfixup';
 
DOT_ZEROINIT : '.zeroinit';
 
ABSTRACT : 'abstract';
 
ADD : 'add';
 
ADD_OVF : 'add.ovf';
 
ADD_OVF_UN : 'add.ovf.un';
 
ALGORITHM : 'algorithm';
 
ALIGNMENT : 'alignment';
 
AND : 'and';
 
ANSI : 'ansi';
 
ANY : 'any';
 
ARGLIST : 'arglist';
 
ARRAY : 'array';
 
AS : 'as';
 
ASSEMBLY : 'assembly';
 
ASSERT : 'assert';
 
AT : 'at';
 
AUTO : 'auto';
 
AUTOCHAR : 'autochar';
 
BEFOREFIELDINIT : 'beforefieldinit';
 
BEQ : 'beq';
 
BEQ_S : 'beq.s';
 
BGE : 'bge';
 
BGE_S : 'bge.s';
 
BGE_UN : 'bge.un';
 
BGE_UN_S : 'bge.un.s';
 
BGT : 'bgt';
 
BGT_S : 'bgt.s';
 
BGT_UN : 'bgt.un';
 
BGT_UN_S : 'bgt.un.s';
 
BLE : 'ble';
 
BLE_S : 'ble.s';
 
BLE_UN : 'ble.un';
 
BLE_UN_S : 'ble.un.s';
 
BLOB : 'blob';
 
BLOB_OBJECT : 'blob_object';
 
BLT : 'blt';
 
BLT_S : 'blt.s';
 
BLT_UN : 'blt.un';
 
BLT_UN_S : 'blt.un.s';
 
BNE_UN : 'bne.un';
 
BNE_UN_S : 'bne.un.s';
 
BOOL : 'bool';
 
BOX : 'box';
 
BR : 'br';
 
BR_S : 'br.s';
 
BREAK : 'break';
 
BRFALSE : 'brfalse';
 
BRFALSE_S : 'brfalse.s';
 
BRINST : 'brinst';
 
BRINST_S : 'brinst.s';
 
BRNULL : 'brnull';
 
BRNULL_S : 'brnull.s';
 
BRTRUE : 'brtrue';
 
BRTRUE_S : 'brtrue.s';
 
BRZERO : 'brzero';
 
BRZERO_S : 'brzero.s';
 
BSTR : 'bstr';
 
BYTEARRAY : 'bytearray';
 
BYVALSTR : 'byvalstr';
 
CALL : 'call';
 
CALLI : 'calli';
 
CALLMOSTDERIVED : 'callmostderived';
 
CALLVIRT : 'callvirt';
 
CARRAY : 'carray';
 
CASTCLASS : 'castclass';
 
CATCH : 'catch';
 
CDECL : 'cdecl';
 
CEQ : 'ceq';
 
CF : 'cf';
 
CGT : 'cgt';
 
CGT_UN : 'cgt.un';
 
CHAR : 'char';
 
CIL : 'cil';
 
CKFINITE : 'ckfinite';
 
CLASS : 'class';
 
CLSID : 'clsid';
 
CLT : 'clt';
 
CLT_UN : 'clt.un';
 
CONST : 'const';
 
CONV_I : 'conv.i';
 
CONV_I1 : 'conv.i1';
 
CONV_I2 : 'conv.i2';
 
CONV_I4 : 'conv.i4';
 
CONV_I8 : 'conv.i8';
 
CONV_OVF_I : 'conv.ovf.i';
 
CONV_OVF_I_UN : 'conv.ovf.i.un';
 
CONV_OVF_I1 : 'conv.ovf.i1';
 
CONV_OVF_I1_UN : 'conv.ovf.i1.un';
 
CONV_OVF_I2 : 'conv.ovf.i2';
 
CONV_OVF_I2_UN : 'conv.ovf.i2.un';
 
CONV_OVF_I4 : 'conv.ovf.i4';
 
CONV_OVF_I4_UN : 'conv.ovf.i4.un';
 
CONV_OVF_I8 : 'conv.ovf.i8';
 
CONV_OVF_I8_UN : 'conv.ovf.i8.un';
 
CONV_OVF_U : 'conv.ovf.u';
 
CONV_OVF_U_UN : 'conv.ovf.u.un';
 
CONV_OVF_U1 : 'conv.ovf.u1';
 
CONV_OVF_U1_UN : 'conv.ovf.u1.un';
 
CONV_OVF_U2 : 'conv.ovf.u2';
 
CONV_OVF_U2_UN : 'conv.ovf.u2.un';
 
CONV_OVF_U4 : 'conv.ovf.u4';
 
CONV_OVF_U4_UN : 'conv.ovf.u4.un';
 
CONV_OVF_U8 : 'conv.ovf.u8';
 
CONV_OVF_U8_UN : 'conv.ovf.u8.un';
 
CONV_R_UN : 'conv.r.un';
 
CONV_R4 : 'conv.r4';
 
CONV_R8 : 'conv.r8';
 
CONV_U : 'conv.u';
 
CONV_U1 : 'conv.u1';
 
CONV_U2 : 'conv.u2';
 
CONV_U4 : 'conv.u4';
 
CONV_U8 : 'conv.u8';
 
CPBLK : 'cpblk';
 
CPOBJ : 'cpobj';
 
CURRENCY : 'currency';
 
CUSTOM : 'custom';
 
DATE : 'date';
 
DECIMAL : 'decimal';
 
DEFAULT : 'default';
 
DEMAND : 'demand';
 
DENY : 'deny';
 
DIV : 'div';
 
DIV_UN : 'div.un';
 
DUP : 'dup';
 
ENDFAULT : 'endfault';
 
ENDFILTER : 'endfilter';
 
ENDFINALLY : 'endfinally';
 
ENDMAC : 'endmac';
 
ENUM : 'enum';
 
ERROR : 'error';
 
EXPLICIT : 'explicit';
 
EXTENDS : 'extends';
 
EXTERN : 'extern';
 
FALSE : 'false';
 
FAMANDASSEM : 'famandassem';
 
FAMILY : 'family';
 
FAMORASSEM : 'famorassem';
 
FASTCALL : 'fastcall';
 
FAULT : 'fault';
 
FIELD : 'field';
 
FILETIME : 'filetime';
 
FILTER : 'filter';
 
FINAL : 'final';
 
FINALLY : 'finally';
 
FIXED : 'fixed';
 
FLOAT : 'float';
 
FLOAT32 : 'float32';
 
FLOAT64 : 'float64';
 
FORWARDREF : 'forwardref';
 
FROMUNMANAGED : 'fromunmanaged';
 
HANDLER : 'handler';
 
HIDEBYSIG : 'hidebysig';
 
HRESULT : 'hresult';
 
IDISPATCH : 'idispatch';
 
IL : 'il';
 
ILLEGAL : 'illegal';
 
IMPLEMENTS : 'implements';
 
IMPLICITCOM : 'implicitcom';
 
IMPLICITRES : 'implicitres';
 
IMPORT : 'import';
 
IN : 'in';
 
INHERITCHECK : 'inheritcheck';
 
INIT : 'init';
 
INITBLK : 'initblk';
 
INITOBJ : 'initobj';
 
INITONLY : 'initonly';
 
INSTANCE : 'instance';
 
INT : 'int';
 
INT16 : 'int16';
 
INT32 : 'int32';
 
INT64 : 'int64';
 
INT8 : 'int8';
 
INTERFACE : 'interface';
 
INTERNALCALL : 'internalcall';
 
ISINST : 'isinst';
 
IUNKNOWN : 'iunknown';
 
JMP : 'jmp';
 
LASTERR : 'lasterr';
 
LCID : 'lcid';
 
LDARG : 'ldarg';
 
LDARG_0 : 'ldarg.0';
 
LDARG_1 : 'ldarg.1';
 
LDARG_2 : 'ldarg.2';
 
LDARG_3 : 'ldarg.3';
 
LDARG_S : 'ldarg.s';
 
LDARGA : 'ldarga';
 
LDARGA_S : 'ldarga.s';
 
LDC_I4 : 'ldc.i4';
 
LDC_I4_0 : 'ldc.i4.0';
 
LDC_I4_1 : 'ldc.i4.1';
 
LDC_I4_2 : 'ldc.i4.2';
 
LDC_I4_3 : 'ldc.i4.3';
 
LDC_I4_4 : 'ldc.i4.4';
 
LDC_I4_5 : 'ldc.i4.5';
 
LDC_I4_6 : 'ldc.i4.6';
 
LDC_I4_7 : 'ldc.i4.7';
 
LDC_I4_8 : 'ldc.i4.8';
 
LDC_I4_M1 : 'ldc.i4.m1';
 
LDC_I4_S : 'ldc.i4.s';
 
LDC_I8 : 'ldc.i8';
 
LDC_R4 : 'ldc.r4';
 
LDC_R8 : 'ldc.r8';
 
LDELEM_I : 'ldelem.i';
 
LDELEM_I1 : 'ldelem.i1';
 
LDELEM_I2 : 'ldelem.i2';
 
LDELEM_I4 : 'ldelem.i4';
 
LDELEM_I8 : 'ldelem.i8';
 
LDELEM_R4 : 'ldelem.r4';
 
LDELEM_R8 : 'ldelem.r8';
 
LDELEM_REF : 'ldelem.ref';
 
LDELEM_U1 : 'ldelem.u1';
 
LDELEM_U2 : 'ldelem.u2';
 
LDELEM_U4 : 'ldelem.u4';
 
LDELEM_U8 : 'ldelem.u8';
 
LDELEMA : 'ldelema';
 
LDFLD : 'ldfld';
 
LDFLDA : 'ldflda';
 
LDFTN : 'ldftn';
 
LDIND_I : 'ldind.i';
 
LDIND_I1 : 'ldind.i1';
 
LDIND_I2 : 'ldind.i2';
 
LDIND_I4 : 'ldind.i4';
 
LDIND_I8 : 'ldind.i8';
 
LDIND_R4 : 'ldind.r4';
 
LDIND_R8 : 'ldind.r8';
 
LDIND_REF : 'ldind.ref';
 
LDIND_U1 : 'ldind.u1';
 
LDIND_U2 : 'ldind.u2';
 
LDIND_U4 : 'ldind.u4';
 
LDIND_U8 : 'ldind.u8';
 
LDLEN : 'ldlen';
 
LDLOC : 'ldloc';
 
LDLOC_0 : 'ldloc.0';
 
LDLOC_1 : 'ldloc.1';
 
LDLOC_2 : 'ldloc.2';
 
LDLOC_3 : 'ldloc.3';
 
LDLOC_S : 'ldloc.s';
 
LDLOCA : 'ldloca';
 
LDLOCA_S : 'ldloca.s';
 
LDNULL : 'ldnull';
 
LDOBJ : 'ldobj';
 
LDSFLD : 'ldsfld';
 
LDSFLDA : 'ldsflda';
 
LDSTR : 'ldstr';
 
LDTOKEN : 'ldtoken';
 
LDVIRTFTN : 'ldvirtftn';
 
LEAVE : 'leave';
 
LEAVE_S : 'leave.s';
 
LINKCHECK : 'linkcheck';
 
LITERAL : 'literal';
 
LOCALLOC : 'localloc';
 
LPSTR : 'lpstr';
 
LPSTRUCT : 'lpstruct';
 
LPTSTR : 'lptstr';
 
LPVOID : 'lpvoid';
 
LPWSTR : 'lpwstr';
 
MANAGED : 'managed';
 
MARSHAL : 'marshal';
 
METHOD : 'method';
 
MKREFANY : 'mkrefany';
 
MODOPT : 'modopt';
 
MODREQ : 'modreq';
 
MUL : 'mul';
 
MUL_OVF : 'mul.ovf';
 
MUL_OVF_UN : 'mul.ovf.un';
 
NATIVE : 'native';
 
NEG : 'neg';
 
NESTED : 'nested';
 
NEWARR : 'newarr';
 
NEWOBJ : 'newobj';
 
NEWSLOT : 'newslot';
 
NOAPPDOMAIN : 'noappdomain';
 
NOINLINING : 'noinlining';
 
NOMACHINE : 'nomachine';
 
NOMANGLE : 'nomangle';
 
NOMETADATA : 'nometadata';
 
NONCASDEMAND : 'noncasdemand';
 
NONCASINHERITANCE : 'noncasinheritance';
 
NONCASLINKDEMAND : 'noncaslinkdemand';
 
NOP : 'nop';
 
NOPROCESS : 'noprocess';
 
NOT : 'not';
 
NOT_IN_GC_HEAP : 'not_in_gc_heap';
 
NOTREMOTABLE : 'notremotable';
 
NOTSERIALIZED : 'notserialized';
 
NULL : 'null';
 
NULLREF : 'nullref';
 
OBJECT : 'object';
 
OBJECTREF : 'objectref';
 
OPT : 'opt';
 
OPTIL : 'optil';
 
OR : 'or';
 
OUT : 'out';
 
PERMITONLY : 'permitonly';
 
PINNED : 'pinned';
 
PINVOKEIMPL : 'pinvokeimpl';
 
POP : 'pop';
 
PREFIX1 : 'prefix1';
 
PREFIX2 : 'prefix2';
 
PREFIX3 : 'prefix3';
 
PREFIX4 : 'prefix4';
 
PREFIX5 : 'prefix5';
 
PREFIX6 : 'prefix6';
 
PREFIX7 : 'prefix7';
 
PREFIXREF : 'prefixref';
 
PREJITDENY : 'prejitdeny';
 
PREJITGRANT : 'prejitgrant';
 
PRESERVESIG : 'preservesig';
 
PRIVATE : 'private';
 
PRIVATESCOPE : 'privatescope';
 
PROTECTED : 'protected';
 
PUBLIC : 'public';
 
READONLY : 'readonly';
 
RECORD : 'record';
 
REFANY : 'refany';
 
REFANYTYPE : 'refanytype';
 
REFANYVAL : 'refanyval';
 
REM : 'rem';
 
REM_UN : 'rem.un';
 
REQMIN : 'reqmin';
 
REQOPT : 'reqopt';
 
REQREFUSE : 'reqrefuse';
 
REQSECOBJ : 'reqsecobj';
 
REQUEST : 'request';
 
RET : 'ret';
 
RETHROW : 'rethrow';
 
RETVAL : 'retval';
 
RTSPECIALNAME : 'rtspecialname';
 
RUNTIME : 'runtime';
 
SAFEARRAY : 'safearray';
 
SEALED : 'sealed';
 
SEQUENTIAL : 'sequential';
 
SERIALIZABLE : 'serializable';
 
SHL : 'shl';
 
SHR : 'shr';
 
SHR_UN : 'shr.un';
 
SIZEOF : 'sizeof';
 
SPECIAL : 'special';
 
SPECIALNAME : 'specialname';
 
STARG : 'starg';
 
STARG_S : 'starg.s';
 
STATIC : 'static';
 
STDCALL : 'stdcall';
 
STELEM_I : 'stelem.i';
 
STELEM_I1 : 'stelem.i1';
 
STELEM_I2 : 'stelem.i2';
 
STELEM_I4 : 'stelem.i4';
 
STELEM_I8 : 'stelem.i8';
 
STELEM_R4 : 'stelem.r4';
 
STELEM_R8 : 'stelem.r8';
 
STELEM_REF : 'stelem.ref';
 
STFLD : 'stfld';
 
STIND_I : 'stind.i';
 
STIND_I1 : 'stind.i1';
 
STIND_I2 : 'stind.i2';
 
STIND_I4 : 'stind.i4';
 
STIND_I8 : 'stind.i8';
 
STIND_R4 : 'stind.r4';
 
STIND_R8 : 'stind.r8';
 
STIND_REF : 'stind.ref';
 
STLOC : 'stloc';
 
STLOC_0 : 'stloc.0';
 
STLOC_1 : 'stloc.1';
 
STLOC_2 : 'stloc.2';
 
STLOC_3 : 'stloc.3';
 
STLOC_S : 'stloc.s';
 
STOBJ : 'stobj';
 
STORAGE : 'storage';
 
STORED_OBJECT : 'stored_object';
 
STREAM : 'stream';
 
STREAMED_OBJECT : 'streamed_object';
 
STRING : 'string';
 
STRUCT : 'struct';
 
STSFLD : 'stsfld';
 
SUB : 'sub';
 
SUB_OVF : 'sub.ovf';
 
SUB_OVF_UN : 'sub.ovf.un';
 
SWITCH : 'switch';
 
SYNCHRONIZED : 'synchronized';
 
SYSCHAR : 'syschar';
 
SYSSTRING : 'sysstring';
 
TAIL_ : 'tail.';
 
TBSTR : 'tbstr';
 
THISCALL : 'thiscall';
 
THROW : 'throw';
 
TLS : 'tls';
 
TO : 'to';
 
TRUE : 'true';
 
TYPEDREF : 'typedref';
 
UNALIGNED_ : 'unaligned.';
 
UNBOX : 'unbox';
 
UNICODE : 'unicode';
 
UNMANAGED : 'unmanaged';
 
UNMANAGEDEXP : 'unmanagedexp';
 
UNSIGNED : 'unsigned';
 
UNUSED : 'unused';
 
USERDEFINED : 'userdefined';
 
VALUE : 'value';
 
VALUETYPE : 'valuetype';
 
VARARG : 'vararg';
 
VARIANT : 'variant';
 
VECTOR : 'vector';
 
VIRTUAL : 'virtual';
 
VOID : 'void';
 
VOLATILE_ : 'volatile.';
 
WCHAR : 'wchar';
 
WINAPI : 'winapi';
 
WITH : 'with';
 
WRAPPER : 'wrapper';
 
XOR : 'xor';
 
STRICT : 'strict';


CHAR_SEQUENCE:
	DOT (
		(DIGIT)+ (EXPONENT)?
		| LETTER (LETTER|DIGIT|'$'|'_'|DOT)*
		| DOT DOT
		)
	| ('_'|'?'|LETTER) (LETTER|DIGIT|'$'|'_'|DOT)*
	| ('-')? DIGIT  
			( ('a'..'f'|'A'..'F')
			| (DIGIT)*
				(
					({LA(2) != '.'}? DOT (DIGIT)* (EXPONENT)? )?
					| EXPONENT
				)
			| ('x'|'X') (HEX_DIGIT)+
			)
	;
EXPONENT: ('e'|'E') ('+'|'-')? (DIGIT)+
	;
HEX_DIGIT: (DIGIT|'a'..'f'|'A'..'F') ;
DIGIT: '0'..'9' ;
LETTER: ('a'..'z'|'A'..'Z') ;

// Symbols
//QUESTION options { paraphrase = "?"; } :	'?'		;
LPAREN :		'('		;
RPAREN :		')'		;
LBRACK :	 	'['		;
RBRACK :		']'		;
LCURLY :		'{'		;
RCURLY :		'}'		;
COLON :		':'		;
COMMA :		','		;
EQUAL :		'='		;
PLUS :		'+'		;
MINUS :		'-'		;

STAR :		'*'		;
SEMI :		';'		;
LESS :		'<'		;
GREATER :		'>'		;
SLASH :		'/'		;
REF :		'&'		;
EXCLAM :		'!'		;
TRIPLE_DOT							  : '...'	;


DOUBLE_COLON	: '::' 	; 
DOT				:	'.'		;

// Simple string like "message".
DQSTRING: '"'! (ESC|~('"'|'\\'))* '"'! ;

SQSTRING: '\''! (ESC|~('\''|'\\'))* '\''! ;
VOCAB: '\3' .. '\377' ;
ESC
	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	'?'
		|	('u')+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT	// unicode
		|	('x'|'X') HEX_DIGIT HEX_DIGIT
		|	('0'..'3')
			(	('0'..'7')
				(	'0'..'7'
				)?
			)?
		|	('4'..'7')
			(	('0'..'9')
			)?
		)
	;

// Whitespace -- ignored
WS	:	(	' '
		|	'\t'
		|	'\f'
			// handle newlines
		|	(	'\r\n'  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
		)+
	;

// Single-line comments
SL_COMMENT
	:	'//'
		(~('\n'|'\r'))* ('\n'|'\r'('\n')?)?
	;

// multiple-line comments
ML_COMMENT
	:	'/*'
		(
			{ LA(2)!='/' }? '*'
		|	'\r' '\n'
		|	'\r'
		|	'\n'
		|	~('*'|'\n'|'\r')
		)*
		'*/'
	;
