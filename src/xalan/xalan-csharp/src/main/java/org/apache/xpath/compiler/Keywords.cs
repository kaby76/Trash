using System.Collections;

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
 * $Id: Keywords.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.compiler
{

	/// <summary>
	/// Table of strings to operation code lookups.
	/// @xsl.usage internal
	/// </summary>
	public class Keywords
	{

	  /// <summary>
	  /// Table of keywords to opcode associations. </summary>
	  private static Hashtable m_keywords = new Hashtable();

	  /// <summary>
	  /// Table of axes names to opcode associations. </summary>
	  private static Hashtable m_axisnames = new Hashtable();

	  /// <summary>
	  /// Table of function name to function ID associations. </summary>
	  private static Hashtable m_nodetests = new Hashtable();

	  /// <summary>
	  /// Table of node type strings to opcode associations. </summary>
	  private static Hashtable m_nodetypes = new Hashtable();

	  /// <summary>
	  /// ancestor axes string. </summary>
	  private const string FROM_ANCESTORS_STRING = "ancestor";

	  /// <summary>
	  /// ancestor-or-self axes string. </summary>
	  private const string FROM_ANCESTORS_OR_SELF_STRING = "ancestor-or-self";

	  /// <summary>
	  /// attribute axes string. </summary>
	  private const string FROM_ATTRIBUTES_STRING = "attribute";

	  /// <summary>
	  /// child axes string. </summary>
	  private const string FROM_CHILDREN_STRING = "child";

	  /// <summary>
	  /// descendant-or-self axes string. </summary>
	  private const string FROM_DESCENDANTS_STRING = "descendant";

	  /// <summary>
	  /// ancestor axes string. </summary>
	  private const string FROM_DESCENDANTS_OR_SELF_STRING = "descendant-or-self";

	  /// <summary>
	  /// following axes string. </summary>
	  private const string FROM_FOLLOWING_STRING = "following";

	  /// <summary>
	  /// following-sibling axes string. </summary>
	  private const string FROM_FOLLOWING_SIBLINGS_STRING = "following-sibling";

	  /// <summary>
	  /// parent axes string. </summary>
	  private const string FROM_PARENT_STRING = "parent";

	  /// <summary>
	  /// preceding axes string. </summary>
	  private const string FROM_PRECEDING_STRING = "preceding";

	  /// <summary>
	  /// preceding-sibling axes string. </summary>
	  private const string FROM_PRECEDING_SIBLINGS_STRING = "preceding-sibling";

	  /// <summary>
	  /// self axes string. </summary>
	  private const string FROM_SELF_STRING = "self";

	  /// <summary>
	  /// namespace axes string. </summary>
	  private const string FROM_NAMESPACE_STRING = "namespace";

	  /// <summary>
	  /// self axes abreviated string. </summary>
	  private const string FROM_SELF_ABBREVIATED_STRING = ".";

	  /// <summary>
	  /// comment node test string. </summary>
	  private const string NODETYPE_COMMENT_STRING = "comment";

	  /// <summary>
	  /// text node test string. </summary>
	  private const string NODETYPE_TEXT_STRING = "text";

	  /// <summary>
	  /// processing-instruction node test string. </summary>
	  private const string NODETYPE_PI_STRING = "processing-instruction";

	  /// <summary>
	  /// Any node test string. </summary>
	  private const string NODETYPE_NODE_STRING = "node";

	  /// <summary>
	  /// Wildcard element string. </summary>
	  private const string NODETYPE_ANYELEMENT_STRING = "*";

	  /// <summary>
	  /// current function string. </summary>
	  public const string FUNC_CURRENT_STRING = "current";

	  /// <summary>
	  /// last function string. </summary>
	  public const string FUNC_LAST_STRING = "last";

	  /// <summary>
	  /// position function string. </summary>
	  public const string FUNC_POSITION_STRING = "position";

	  /// <summary>
	  /// count function string. </summary>
	  public const string FUNC_COUNT_STRING = "count";

	  /// <summary>
	  /// id function string. </summary>
	  internal const string FUNC_ID_STRING = "id";

	  /// <summary>
	  /// key function string (XSLT). </summary>
	  public const string FUNC_KEY_STRING = "key";

	  /// <summary>
	  /// local-name function string. </summary>
	  public const string FUNC_LOCAL_PART_STRING = "local-name";

	  /// <summary>
	  /// namespace-uri function string. </summary>
	  public const string FUNC_NAMESPACE_STRING = "namespace-uri";

	  /// <summary>
	  /// name function string. </summary>
	  public const string FUNC_NAME_STRING = "name";

	  /// <summary>
	  /// generate-id function string (XSLT). </summary>
	  public const string FUNC_GENERATE_ID_STRING = "generate-id";

	  /// <summary>
	  /// not function string. </summary>
	  public const string FUNC_NOT_STRING = "not";

	  /// <summary>
	  /// true function string. </summary>
	  public const string FUNC_TRUE_STRING = "true";

	  /// <summary>
	  /// false function string. </summary>
	  public const string FUNC_FALSE_STRING = "false";

	  /// <summary>
	  /// boolean function string. </summary>
	  public const string FUNC_BOOLEAN_STRING = "boolean";

	  /// <summary>
	  /// lang function string. </summary>
	  public const string FUNC_LANG_STRING = "lang";

	  /// <summary>
	  /// number function string. </summary>
	  public const string FUNC_NUMBER_STRING = "number";

	  /// <summary>
	  /// floor function string. </summary>
	  public const string FUNC_FLOOR_STRING = "floor";

	  /// <summary>
	  /// ceiling function string. </summary>
	  public const string FUNC_CEILING_STRING = "ceiling";

	  /// <summary>
	  /// round function string. </summary>
	  public const string FUNC_ROUND_STRING = "round";

	  /// <summary>
	  /// sum function string. </summary>
	  public const string FUNC_SUM_STRING = "sum";

	  /// <summary>
	  /// string function string. </summary>
	  public const string FUNC_STRING_STRING = "string";

	  /// <summary>
	  /// starts-with function string. </summary>
	  public const string FUNC_STARTS_WITH_STRING = "starts-with";

	  /// <summary>
	  /// contains function string. </summary>
	  public const string FUNC_CONTAINS_STRING = "contains";

	  /// <summary>
	  /// substring-before function string. </summary>
	  public const string FUNC_SUBSTRING_BEFORE_STRING = "substring-before";

	  /// <summary>
	  /// substring-after function string. </summary>
	  public const string FUNC_SUBSTRING_AFTER_STRING = "substring-after";

	  /// <summary>
	  /// normalize-space function string. </summary>
	  public const string FUNC_NORMALIZE_SPACE_STRING = "normalize-space";

	  /// <summary>
	  /// translate function string. </summary>
	  public const string FUNC_TRANSLATE_STRING = "translate";

	  /// <summary>
	  /// concat function string. </summary>
	  public const string FUNC_CONCAT_STRING = "concat";

	  /// <summary>
	  /// system-property function string. </summary>
	  public const string FUNC_SYSTEM_PROPERTY_STRING = "system-property";

	  /// <summary>
	  /// function-available function string (XSLT). </summary>
	  public const string FUNC_EXT_FUNCTION_AVAILABLE_STRING = "function-available";

	  /// <summary>
	  /// element-available function string (XSLT). </summary>
	  public const string FUNC_EXT_ELEM_AVAILABLE_STRING = "element-available";

	  /// <summary>
	  /// substring function string. </summary>
	  public const string FUNC_SUBSTRING_STRING = "substring";

	  /// <summary>
	  /// string-length function string. </summary>
	  public const string FUNC_STRING_LENGTH_STRING = "string-length";

	  /// <summary>
	  /// unparsed-entity-uri function string (XSLT). </summary>
	  public const string FUNC_UNPARSED_ENTITY_URI_STRING = "unparsed-entity-uri";

	  // Proprietary, built in functions

	  /// <summary>
	  /// current function string (Proprietary). </summary>
	  public const string FUNC_DOCLOCATION_STRING = "document-location";

	  static Keywords()
	  {
		m_axisnames[FROM_ANCESTORS_STRING] = new int?(OpCodes.FROM_ANCESTORS);
		m_axisnames[FROM_ANCESTORS_OR_SELF_STRING] = new int?(OpCodes.FROM_ANCESTORS_OR_SELF);
		m_axisnames[FROM_ATTRIBUTES_STRING] = new int?(OpCodes.FROM_ATTRIBUTES);
		m_axisnames[FROM_CHILDREN_STRING] = new int?(OpCodes.FROM_CHILDREN);
		m_axisnames[FROM_DESCENDANTS_STRING] = new int?(OpCodes.FROM_DESCENDANTS);
		m_axisnames[FROM_DESCENDANTS_OR_SELF_STRING] = new int?(OpCodes.FROM_DESCENDANTS_OR_SELF);
		m_axisnames[FROM_FOLLOWING_STRING] = new int?(OpCodes.FROM_FOLLOWING);
		m_axisnames[FROM_FOLLOWING_SIBLINGS_STRING] = new int?(OpCodes.FROM_FOLLOWING_SIBLINGS);
		m_axisnames[FROM_PARENT_STRING] = new int?(OpCodes.FROM_PARENT);
		m_axisnames[FROM_PRECEDING_STRING] = new int?(OpCodes.FROM_PRECEDING);
		m_axisnames[FROM_PRECEDING_SIBLINGS_STRING] = new int?(OpCodes.FROM_PRECEDING_SIBLINGS);
		m_axisnames[FROM_SELF_STRING] = new int?(OpCodes.FROM_SELF);
		m_axisnames[FROM_NAMESPACE_STRING] = new int?(OpCodes.FROM_NAMESPACE);
		m_nodetypes[NODETYPE_COMMENT_STRING] = new int?(OpCodes.NODETYPE_COMMENT);
		m_nodetypes[NODETYPE_TEXT_STRING] = new int?(OpCodes.NODETYPE_TEXT);
		m_nodetypes[NODETYPE_PI_STRING] = new int?(OpCodes.NODETYPE_PI);
		m_nodetypes[NODETYPE_NODE_STRING] = new int?(OpCodes.NODETYPE_NODE);
		m_nodetypes[NODETYPE_ANYELEMENT_STRING] = new int?(OpCodes.NODETYPE_ANYELEMENT);
		m_keywords[FROM_SELF_ABBREVIATED_STRING] = new int?(OpCodes.FROM_SELF);
		m_keywords[FUNC_ID_STRING] = new int?(FunctionTable.FUNC_ID);
		m_keywords[FUNC_KEY_STRING] = new int?(FunctionTable.FUNC_KEY);

		m_nodetests[NODETYPE_COMMENT_STRING] = new int?(OpCodes.NODETYPE_COMMENT);
		m_nodetests[NODETYPE_TEXT_STRING] = new int?(OpCodes.NODETYPE_TEXT);
		m_nodetests[NODETYPE_PI_STRING] = new int?(OpCodes.NODETYPE_PI);
		m_nodetests[NODETYPE_NODE_STRING] = new int?(OpCodes.NODETYPE_NODE);
	  }

	  internal static object getAxisName(string key)
	  {
			  return m_axisnames[key];
	  }

	  internal static object lookupNodeTest(string key)
	  {
			  return m_nodetests[key];
	  }

	  internal static object getKeyWord(string key)
	  {
			  return m_keywords[key];
	  }

	  internal static object getNodeType(string key)
	  {
			  return m_nodetypes[key];
	  }
	}

}