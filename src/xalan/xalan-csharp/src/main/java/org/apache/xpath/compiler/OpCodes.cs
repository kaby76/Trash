/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: OpCodes.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.compiler
{

	/// <summary>
	/// Operations codes for XPath.
	/// 
	/// Code for the descriptions of the operations codes:
	/// [UPPER CASE] indicates a literal value,
	/// [lower case] is a description of a value,
	///      ([length] always indicates the length of the operation,
	///       including the operations code and the length integer.)
	/// {UPPER CASE} indicates the given production,
	/// {description} is the description of a new production,
	///      (For instance, {boolean expression} means some expression
	///       that should be resolved to a boolean.)
	///  * means that it occurs zero or more times,
	///  + means that it occurs one or more times,
	///  ? means that it is optional.
	/// 
	/// returns: indicates what the production should return.
	/// </summary>
	public class OpCodes
	{

	  /// <summary>
	  /// [ENDOP]
	  /// Some operators may like to have a terminator.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int ENDOP = -1;

	  /// <summary>
	  /// [EMPTY]
	  /// Empty slot to indicate NULL.
	  /// </summary>
	  public const int EMPTY = -2;

	  /// <summary>
	  /// [ELEMWILDCARD]
	  /// Means ELEMWILDCARD ("*"), used instead
	  /// of string index in some places.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int ELEMWILDCARD = -3;

	  /// <summary>
	  /// [OP_XPATH]
	  /// [length]
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XNodeSet
	  ///  XNumber
	  ///  XString
	  ///  XBoolean
	  ///  XRTree
	  ///  XObject
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_XPATH = 1;

	  /// <summary>
	  /// [OP_OR]
	  /// [length]
	  ///  {boolean expression}
	  ///  {boolean expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_OR = 2;

	  /// <summary>
	  /// [OP_AND]
	  /// [length]
	  ///  {boolean expression}
	  ///  {boolean expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_AND = 3;

	  /// <summary>
	  /// [OP_NOTEQUALS]
	  /// [length]
	  ///  {expression}
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_NOTEQUALS = 4;

	  /// <summary>
	  /// [OP_EQUALS]
	  /// [length]
	  ///  {expression}
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_EQUALS = 5;

	  /// <summary>
	  /// [OP_LTE] (less-than-or-equals)
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_LTE = 6;

	  /// <summary>
	  /// [OP_LT] (less-than)
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_LT = 7;

	  /// <summary>
	  /// [OP_GTE] (greater-than-or-equals)
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_GTE = 8;

	  /// <summary>
	  /// [OP_GT] (greater-than)
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_GT = 9;

	  /// <summary>
	  /// [OP_PLUS]
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_PLUS = 10;

	  /// <summary>
	  /// [OP_MINUS]
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_MINUS = 11;

	  /// <summary>
	  /// [OP_MULT]
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_MULT = 12;

	  /// <summary>
	  /// [OP_DIV]
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_DIV = 13;

	  /// <summary>
	  /// [OP_MOD]
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_MOD = 14;

	  /// <summary>
	  /// [OP_QUO]
	  /// [length]
	  ///  {number expression}
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_QUO = 15;

	  /// <summary>
	  /// [OP_NEG]
	  /// [length]
	  ///  {number expression}
	  /// 
	  /// returns:
	  ///  XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_NEG = 16;

	  /// <summary>
	  /// [OP_STRING] (cast operation)
	  /// [length]
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XString
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_STRING = 17;

	  /// <summary>
	  /// [OP_BOOL] (cast operation)
	  /// [length]
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_BOOL = 18;

	  /// <summary>
	  /// [OP_NUMBER] (cast operation)
	  /// [length]
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_NUMBER = 19;

	  /// <summary>
	  /// [OP_UNION]
	  /// [length]
	  ///  {PathExpr}+
	  /// 
	  /// returns:
	  ///  XNodeSet
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_UNION = 20;

	  /// <summary>
	  /// [OP_LITERAL]
	  /// [3]
	  /// [index to token]
	  /// 
	  /// returns:
	  ///  XString
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_LITERAL = 21;

	  /// <summary>
	  /// The low opcode for nodesets, needed by getFirstPredicateOpPos and 
	  ///  getNextStepPos.          
	  /// </summary>
	  internal const int FIRST_NODESET_OP = 22;

	  /// <summary>
	  /// [OP_VARIABLE]
	  /// [4]
	  /// [index to namespace token, or EMPTY]
	  /// [index to function name token]
	  /// 
	  /// returns:
	  ///  XString
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_VARIABLE = 22;

	  /// <summary>
	  /// [OP_GROUP]
	  /// [length]
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XNodeSet
	  ///  XNumber
	  ///  XString
	  ///  XBoolean
	  ///  XRTree
	  ///  XObject
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_GROUP = 23;

	  /// <summary>
	  /// [OP_EXTFUNCTION] (Extension function.)
	  /// [length]
	  /// [index to namespace token]
	  /// [index to function name token]
	  ///  {OP_ARGUMENT}
	  /// 
	  /// returns:
	  ///  XNodeSet
	  ///  XNumber
	  ///  XString
	  ///  XBoolean
	  ///  XRTree
	  ///  XObject
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_EXTFUNCTION = 24;

	  /// <summary>
	  /// [OP_FUNCTION]
	  /// [length]
	  /// [FUNC_name]
	  ///  {OP_ARGUMENT}
	  /// [ENDOP]
	  /// 
	  /// returns:
	  ///  XNodeSet
	  ///  XNumber
	  ///  XString
	  ///  XBoolean
	  ///  XRTree
	  ///  XObject
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_FUNCTION = 25;

	  /// <summary>
	  /// The last opcode for stuff that can be a nodeset. </summary>
	  internal const int LAST_NODESET_OP = 25;

	  /// <summary>
	  /// [OP_ARGUMENT] (Function argument.)
	  /// [length]
	  ///  {expression}
	  /// 
	  /// returns:
	  ///  XNodeSet
	  ///  XNumber
	  ///  XString
	  ///  XBoolean
	  ///  XRTree
	  ///  XObject
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_ARGUMENT = 26;

	  /// <summary>
	  /// [OP_NUMBERLIT] (Number literal.)
	  /// [3]
	  /// [index to token]
	  /// 
	  /// returns:
	  ///  XString
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_NUMBERLIT = 27;

	  /// <summary>
	  /// [OP_LOCATIONPATH]
	  /// [length]
	  ///   {FROM_stepType}
	  /// | {function}
	  /// {predicate}
	  /// [ENDOP]
	  /// 
	  /// (Note that element and attribute namespaces and
	  /// names can be wildcarded '*'.)
	  /// 
	  /// returns:
	  ///  XNodeSet
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_LOCATIONPATH = 28;

	  // public static final int LOCATIONPATHEX_MASK = 0x0000FFFF;
	  // public static final int LOCATIONPATHEX_ISSIMPLE = 0x00010000;
	  // public static final int OP_LOCATIONPATH_EX = (28 | 0x00010000);

	  /// <summary>
	  /// [OP_PREDICATE]
	  /// [length]
	  ///  {expression}
	  /// [ENDOP] (For safety)
	  /// 
	  /// returns:
	  ///  XBoolean or XNumber
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_PREDICATE = 29;

	  /// <summary>
	  /// [OP_MATCHPATTERN]
	  /// [length]
	  ///  {PathExpr}+
	  /// 
	  /// returns:
	  ///  XNodeSet
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_MATCHPATTERN = 30;

	  /// <summary>
	  /// [OP_LOCATIONPATHPATTERN]
	  /// [length]
	  ///   {FROM_stepType}
	  /// | {function}{predicate}
	  /// [ENDOP]
	  /// returns:
	  ///  XNodeSet
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int OP_LOCATIONPATHPATTERN = 31;

	  /// <summary>
	  /// [NODETYPE_COMMENT]
	  /// No size or arguments.
	  /// Note: must not overlap function OP number!
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODETYPE_COMMENT = 1030;

	  /// <summary>
	  /// [NODETYPE_TEXT]
	  /// No size or arguments.
	  /// Note: must not overlap function OP number!
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODETYPE_TEXT = 1031;

	  /// <summary>
	  /// [NODETYPE_PI]
	  /// [index to token]
	  /// Note: must not overlap function OP number!
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODETYPE_PI = 1032;

	  /// <summary>
	  /// [NODETYPE_NODE]
	  /// No size or arguments.
	  /// Note: must not overlap function OP number!
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODETYPE_NODE = 1033;

	  /// <summary>
	  /// [NODENAME]
	  /// [index to ns token or EMPTY]
	  /// [index to name token]
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODENAME = 34;

	  /// <summary>
	  /// [NODETYPE_ROOT]
	  /// No size or arguments.
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODETYPE_ROOT = 35;

	  /// <summary>
	  /// [NODETYPE_ANY]
	  /// No size or arguments.
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODETYPE_ANYELEMENT = 36;

	  /// <summary>
	  /// [NODETYPE_ANY]
	  /// No size or arguments.
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int NODETYPE_FUNCTEST = 1034;

	  /// <summary>
	  /// [FROM_stepType]
	  /// [length, including predicates]
	  /// [length of just the step, without the predicates]
	  /// {node test}
	  /// {predicates}?
	  /// 
	  /// returns:
	  ///  XBoolean
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int AXES_START_TYPES = 37;

	  /// <summary>
	  /// ancestor axes opcode. </summary>
	  public const int FROM_ANCESTORS = 37;

	  /// <summary>
	  /// ancestor-or-self axes opcode. </summary>
	  public const int FROM_ANCESTORS_OR_SELF = 38;

	  /// <summary>
	  /// attribute axes opcode. </summary>
	  public const int FROM_ATTRIBUTES = 39;

	  /// <summary>
	  /// children axes opcode. </summary>
	  public const int FROM_CHILDREN = 40;

	  /// <summary>
	  /// descendants axes opcode. </summary>
	  public const int FROM_DESCENDANTS = 41;

	  /// <summary>
	  /// descendants-of-self axes opcode. </summary>
	  public const int FROM_DESCENDANTS_OR_SELF = 42;

	  /// <summary>
	  /// following axes opcode. </summary>
	  public const int FROM_FOLLOWING = 43;

	  /// <summary>
	  /// following-siblings axes opcode. </summary>
	  public const int FROM_FOLLOWING_SIBLINGS = 44;

	  /// <summary>
	  /// parent axes opcode. </summary>
	  public const int FROM_PARENT = 45;

	  /// <summary>
	  /// preceding axes opcode. </summary>
	  public const int FROM_PRECEDING = 46;

	  /// <summary>
	  /// preceding-sibling axes opcode. </summary>
	  public const int FROM_PRECEDING_SIBLINGS = 47;

	  /// <summary>
	  /// self axes opcode. </summary>
	  public const int FROM_SELF = 48;

	  /// <summary>
	  /// namespace axes opcode. </summary>
	  public const int FROM_NAMESPACE = 49;

	  /// <summary>
	  /// '/' axes opcode. </summary>
	  public const int FROM_ROOT = 50;

	  /// <summary>
	  /// For match patterns.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int MATCH_ATTRIBUTE = 51;

	  /// <summary>
	  /// For match patterns.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int MATCH_ANY_ANCESTOR = 52;

	  /// <summary>
	  /// For match patterns.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const int MATCH_IMMEDIATE_ANCESTOR = 53;

	  /// <summary>
	  /// The end of the axes types. </summary>
	  public const int AXES_END_TYPES = 53;

	  /// <summary>
	  /// The next free ID.  Please keep this up to date. </summary>
	  private const int NEXT_FREE_ID = 99;
	}

}