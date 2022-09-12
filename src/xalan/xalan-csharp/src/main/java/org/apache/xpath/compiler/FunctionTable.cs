using System;
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
 * $Id: FunctionTable.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.compiler
{

	using Function = org.apache.xpath.functions.Function;

	/// <summary>
	/// The function table for XPath.
	/// </summary>
	public class FunctionTable
	{

	  /// <summary>
	  /// The 'current()' id. </summary>
	  public const int FUNC_CURRENT = 0;

	  /// <summary>
	  /// The 'last()' id. </summary>
	  public const int FUNC_LAST = 1;

	  /// <summary>
	  /// The 'position()' id. </summary>
	  public const int FUNC_POSITION = 2;

	  /// <summary>
	  /// The 'count()' id. </summary>
	  public const int FUNC_COUNT = 3;

	  /// <summary>
	  /// The 'id()' id. </summary>
	  public const int FUNC_ID = 4;

	  /// <summary>
	  /// The 'key()' id (XSLT). </summary>
	  public const int FUNC_KEY = 5;

	  /// <summary>
	  /// The 'local-name()' id. </summary>
	  public const int FUNC_LOCAL_PART = 7;

	  /// <summary>
	  /// The 'namespace-uri()' id. </summary>
	  public const int FUNC_NAMESPACE = 8;

	  /// <summary>
	  /// The 'name()' id. </summary>
	  public const int FUNC_QNAME = 9;

	  /// <summary>
	  /// The 'generate-id()' id. </summary>
	  public const int FUNC_GENERATE_ID = 10;

	  /// <summary>
	  /// The 'not()' id. </summary>
	  public const int FUNC_NOT = 11;

	  /// <summary>
	  /// The 'true()' id. </summary>
	  public const int FUNC_TRUE = 12;

	  /// <summary>
	  /// The 'false()' id. </summary>
	  public const int FUNC_FALSE = 13;

	  /// <summary>
	  /// The 'boolean()' id. </summary>
	  public const int FUNC_BOOLEAN = 14;

	  /// <summary>
	  /// The 'number()' id. </summary>
	  public const int FUNC_NUMBER = 15;

	  /// <summary>
	  /// The 'floor()' id. </summary>
	  public const int FUNC_FLOOR = 16;

	  /// <summary>
	  /// The 'ceiling()' id. </summary>
	  public const int FUNC_CEILING = 17;

	  /// <summary>
	  /// The 'round()' id. </summary>
	  public const int FUNC_ROUND = 18;

	  /// <summary>
	  /// The 'sum()' id. </summary>
	  public const int FUNC_SUM = 19;

	  /// <summary>
	  /// The 'string()' id. </summary>
	  public const int FUNC_STRING = 20;

	  /// <summary>
	  /// The 'starts-with()' id. </summary>
	  public const int FUNC_STARTS_WITH = 21;

	  /// <summary>
	  /// The 'contains()' id. </summary>
	  public const int FUNC_CONTAINS = 22;

	  /// <summary>
	  /// The 'substring-before()' id. </summary>
	  public const int FUNC_SUBSTRING_BEFORE = 23;

	  /// <summary>
	  /// The 'substring-after()' id. </summary>
	  public const int FUNC_SUBSTRING_AFTER = 24;

	  /// <summary>
	  /// The 'normalize-space()' id. </summary>
	  public const int FUNC_NORMALIZE_SPACE = 25;

	  /// <summary>
	  /// The 'translate()' id. </summary>
	  public const int FUNC_TRANSLATE = 26;

	  /// <summary>
	  /// The 'concat()' id. </summary>
	  public const int FUNC_CONCAT = 27;

	  /// <summary>
	  /// The 'substring()' id. </summary>
	  public const int FUNC_SUBSTRING = 29;

	  /// <summary>
	  /// The 'string-length()' id. </summary>
	  public const int FUNC_STRING_LENGTH = 30;

	  /// <summary>
	  /// The 'system-property()' id. </summary>
	  public const int FUNC_SYSTEM_PROPERTY = 31;

	  /// <summary>
	  /// The 'lang()' id. </summary>
	  public const int FUNC_LANG = 32;

	  /// <summary>
	  /// The 'function-available()' id (XSLT). </summary>
	  public const int FUNC_EXT_FUNCTION_AVAILABLE = 33;

	  /// <summary>
	  /// The 'element-available()' id (XSLT). </summary>
	  public const int FUNC_EXT_ELEM_AVAILABLE = 34;

	  /// <summary>
	  /// The 'unparsed-entity-uri()' id (XSLT). </summary>
	  public const int FUNC_UNPARSED_ENTITY_URI = 36;

	  // Proprietary

	  /// <summary>
	  /// The 'document-location()' id (Proprietary). </summary>
	  public const int FUNC_DOCLOCATION = 35;

	  /// <summary>
	  /// The function table.
	  /// </summary>
	  private static Type[] m_functions;

	  /// <summary>
	  /// Table of function name to function ID associations. </summary>
	  private static Hashtable m_functionID = new Hashtable();

	  /// <summary>
	  /// The function table contains customized functions
	  /// </summary>
	  private Type[] m_functions_customer = new Type[NUM_ALLOWABLE_ADDINS];

	  /// <summary>
	  /// Table of function name to function ID associations for customized functions
	  /// </summary>
	  private Hashtable m_functionID_customer = new Hashtable();

	  /// <summary>
	  /// Number of built in functions.  Be sure to update this as
	  /// built-in functions are added.
	  /// </summary>
	  private const int NUM_BUILT_IN_FUNCS = 37;

	  /// <summary>
	  /// Number of built-in functions that may be added.
	  /// </summary>
	  private const int NUM_ALLOWABLE_ADDINS = 30;

	  /// <summary>
	  /// The index to the next free function index.
	  /// </summary>
	  private int m_funcNextFreeIndex = NUM_BUILT_IN_FUNCS;

	  static FunctionTable()
	  {
		m_functions = new Type[NUM_BUILT_IN_FUNCS];
		m_functions[FUNC_CURRENT] = typeof(org.apache.xpath.functions.FuncCurrent);
		m_functions[FUNC_LAST] = typeof(org.apache.xpath.functions.FuncLast);
		m_functions[FUNC_POSITION] = typeof(org.apache.xpath.functions.FuncPosition);
		m_functions[FUNC_COUNT] = typeof(org.apache.xpath.functions.FuncCount);
		m_functions[FUNC_ID] = typeof(org.apache.xpath.functions.FuncId);
		m_functions[FUNC_KEY] = typeof(org.apache.xalan.templates.FuncKey);
		m_functions[FUNC_LOCAL_PART] = typeof(org.apache.xpath.functions.FuncLocalPart);
		m_functions[FUNC_NAMESPACE] = typeof(org.apache.xpath.functions.FuncNamespace);
		m_functions[FUNC_QNAME] = typeof(org.apache.xpath.functions.FuncQname);
		m_functions[FUNC_GENERATE_ID] = typeof(org.apache.xpath.functions.FuncGenerateId);
		m_functions[FUNC_NOT] = typeof(org.apache.xpath.functions.FuncNot);
		m_functions[FUNC_TRUE] = typeof(org.apache.xpath.functions.FuncTrue);
		m_functions[FUNC_FALSE] = typeof(org.apache.xpath.functions.FuncFalse);
		m_functions[FUNC_BOOLEAN] = typeof(org.apache.xpath.functions.FuncBoolean);
		m_functions[FUNC_LANG] = typeof(org.apache.xpath.functions.FuncLang);
		m_functions[FUNC_NUMBER] = typeof(org.apache.xpath.functions.FuncNumber);
		m_functions[FUNC_FLOOR] = typeof(org.apache.xpath.functions.FuncFloor);
		m_functions[FUNC_CEILING] = typeof(org.apache.xpath.functions.FuncCeiling);
		m_functions[FUNC_ROUND] = typeof(org.apache.xpath.functions.FuncRound);
		m_functions[FUNC_SUM] = typeof(org.apache.xpath.functions.FuncSum);
		m_functions[FUNC_STRING] = typeof(org.apache.xpath.functions.FuncString);
		m_functions[FUNC_STARTS_WITH] = typeof(org.apache.xpath.functions.FuncStartsWith);
		m_functions[FUNC_CONTAINS] = typeof(org.apache.xpath.functions.FuncContains);
		m_functions[FUNC_SUBSTRING_BEFORE] = typeof(org.apache.xpath.functions.FuncSubstringBefore);
		m_functions[FUNC_SUBSTRING_AFTER] = typeof(org.apache.xpath.functions.FuncSubstringAfter);
		m_functions[FUNC_NORMALIZE_SPACE] = typeof(org.apache.xpath.functions.FuncNormalizeSpace);
		m_functions[FUNC_TRANSLATE] = typeof(org.apache.xpath.functions.FuncTranslate);
		m_functions[FUNC_CONCAT] = typeof(org.apache.xpath.functions.FuncConcat);
		m_functions[FUNC_SYSTEM_PROPERTY] = typeof(org.apache.xpath.functions.FuncSystemProperty);
		m_functions[FUNC_EXT_FUNCTION_AVAILABLE] = typeof(org.apache.xpath.functions.FuncExtFunctionAvailable);
		m_functions[FUNC_EXT_ELEM_AVAILABLE] = typeof(org.apache.xpath.functions.FuncExtElementAvailable);
		m_functions[FUNC_SUBSTRING] = typeof(org.apache.xpath.functions.FuncSubstring);
		m_functions[FUNC_STRING_LENGTH] = typeof(org.apache.xpath.functions.FuncStringLength);
		m_functions[FUNC_DOCLOCATION] = typeof(org.apache.xpath.functions.FuncDoclocation);
		m_functions[FUNC_UNPARSED_ENTITY_URI] = typeof(org.apache.xpath.functions.FuncUnparsedEntityURI);
			  m_functionID[Keywords.FUNC_CURRENT_STRING] = new int?(FunctionTable.FUNC_CURRENT);
			  m_functionID[Keywords.FUNC_LAST_STRING] = new int?(FunctionTable.FUNC_LAST);
			  m_functionID[Keywords.FUNC_POSITION_STRING] = new int?(FunctionTable.FUNC_POSITION);
			  m_functionID[Keywords.FUNC_COUNT_STRING] = new int?(FunctionTable.FUNC_COUNT);
			  m_functionID[Keywords.FUNC_ID_STRING] = new int?(FunctionTable.FUNC_ID);
			  m_functionID[Keywords.FUNC_KEY_STRING] = new int?(FunctionTable.FUNC_KEY);
			  m_functionID[Keywords.FUNC_LOCAL_PART_STRING] = new int?(FunctionTable.FUNC_LOCAL_PART);
			  m_functionID[Keywords.FUNC_NAMESPACE_STRING] = new int?(FunctionTable.FUNC_NAMESPACE);
			  m_functionID[Keywords.FUNC_NAME_STRING] = new int?(FunctionTable.FUNC_QNAME);
			  m_functionID[Keywords.FUNC_GENERATE_ID_STRING] = new int?(FunctionTable.FUNC_GENERATE_ID);
			  m_functionID[Keywords.FUNC_NOT_STRING] = new int?(FunctionTable.FUNC_NOT);
			  m_functionID[Keywords.FUNC_TRUE_STRING] = new int?(FunctionTable.FUNC_TRUE);
			  m_functionID[Keywords.FUNC_FALSE_STRING] = new int?(FunctionTable.FUNC_FALSE);
			  m_functionID[Keywords.FUNC_BOOLEAN_STRING] = new int?(FunctionTable.FUNC_BOOLEAN);
			  m_functionID[Keywords.FUNC_LANG_STRING] = new int?(FunctionTable.FUNC_LANG);
			  m_functionID[Keywords.FUNC_NUMBER_STRING] = new int?(FunctionTable.FUNC_NUMBER);
			  m_functionID[Keywords.FUNC_FLOOR_STRING] = new int?(FunctionTable.FUNC_FLOOR);
			  m_functionID[Keywords.FUNC_CEILING_STRING] = new int?(FunctionTable.FUNC_CEILING);
			  m_functionID[Keywords.FUNC_ROUND_STRING] = new int?(FunctionTable.FUNC_ROUND);
			  m_functionID[Keywords.FUNC_SUM_STRING] = new int?(FunctionTable.FUNC_SUM);
			  m_functionID[Keywords.FUNC_STRING_STRING] = new int?(FunctionTable.FUNC_STRING);
			  m_functionID[Keywords.FUNC_STARTS_WITH_STRING] = new int?(FunctionTable.FUNC_STARTS_WITH);
			  m_functionID[Keywords.FUNC_CONTAINS_STRING] = new int?(FunctionTable.FUNC_CONTAINS);
			  m_functionID[Keywords.FUNC_SUBSTRING_BEFORE_STRING] = new int?(FunctionTable.FUNC_SUBSTRING_BEFORE);
			  m_functionID[Keywords.FUNC_SUBSTRING_AFTER_STRING] = new int?(FunctionTable.FUNC_SUBSTRING_AFTER);
			  m_functionID[Keywords.FUNC_NORMALIZE_SPACE_STRING] = new int?(FunctionTable.FUNC_NORMALIZE_SPACE);
			  m_functionID[Keywords.FUNC_TRANSLATE_STRING] = new int?(FunctionTable.FUNC_TRANSLATE);
			  m_functionID[Keywords.FUNC_CONCAT_STRING] = new int?(FunctionTable.FUNC_CONCAT);
			  m_functionID[Keywords.FUNC_SYSTEM_PROPERTY_STRING] = new int?(FunctionTable.FUNC_SYSTEM_PROPERTY);
			  m_functionID[Keywords.FUNC_EXT_FUNCTION_AVAILABLE_STRING] = new int?(FunctionTable.FUNC_EXT_FUNCTION_AVAILABLE);
			  m_functionID[Keywords.FUNC_EXT_ELEM_AVAILABLE_STRING] = new int?(FunctionTable.FUNC_EXT_ELEM_AVAILABLE);
			  m_functionID[Keywords.FUNC_SUBSTRING_STRING] = new int?(FunctionTable.FUNC_SUBSTRING);
			  m_functionID[Keywords.FUNC_STRING_LENGTH_STRING] = new int?(FunctionTable.FUNC_STRING_LENGTH);
			  m_functionID[Keywords.FUNC_UNPARSED_ENTITY_URI_STRING] = new int?(FunctionTable.FUNC_UNPARSED_ENTITY_URI);
			  m_functionID[Keywords.FUNC_DOCLOCATION_STRING] = new int?(FunctionTable.FUNC_DOCLOCATION);
	  }


	  public FunctionTable()
	  {
	  }

	  /// <summary>
	  /// Return the name of the a function in the static table. Needed to avoid
	  /// making the table publicly available.
	  /// </summary>
	  internal virtual string getFunctionName(int funcID)
	  {
		  if (funcID < NUM_BUILT_IN_FUNCS)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  return m_functions[funcID].FullName;
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  return m_functions_customer[funcID - NUM_BUILT_IN_FUNCS].FullName;
		  }
	  }

	  /// <summary>
	  /// Obtain a new Function object from a function ID.
	  /// </summary>
	  /// <param name="which">  The function ID, which may correspond to one of the FUNC_XXX 
	  ///    values found in <seealso cref="org.apache.xpath.compiler.FunctionTable"/>, but may 
	  ///    be a value installed by an external module. 
	  /// </param>
	  /// <returns> a a new Function instance.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if ClassNotFoundException, 
	  ///    IllegalAccessException, or InstantiationException is thrown. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: org.apache.xpath.functions.Function getFunction(int which) throws javax.xml.transform.TransformerException
	  internal virtual Function getFunction(int which)
	  {
			  try
			  {
				  if (which < NUM_BUILT_IN_FUNCS)
				  {
					  return (Function) m_functions[which].newInstance();
				  }
				  else
				  {
					  return (Function) m_functions_customer[which - NUM_BUILT_IN_FUNCS].newInstance();
				  }
			  }
			  catch (IllegalAccessException ex)
			  {
					  throw new TransformerException(ex.Message);
			  }
			  catch (InstantiationException ex)
			  {
					  throw new TransformerException(ex.Message);
			  }
	  }

	  /// <summary>
	  /// Obtain a function ID from a given function name </summary>
	  /// <param name="key"> the function name in a java.lang.String format. </param>
	  /// <returns> a function ID, which may correspond to one of the FUNC_XXX values
	  /// found in <seealso cref="org.apache.xpath.compiler.FunctionTable"/>, but may be a 
	  /// value installed by an external module. </returns>
	  internal virtual object getFunctionID(string key)
	  {
			  object id = m_functionID_customer[key];
			  if (null == id)
			  {
				  id = m_functionID[key];
			  }
			  return id;
	  }

	  /// <summary>
	  /// Install a built-in function. </summary>
	  /// <param name="name"> The unqualified name of the function, must not be null </param>
	  /// <param name="func"> A Implementation of an XPath Function object. </param>
	  /// <returns> the position of the function in the internal index. </returns>
	  public virtual int installFunction(string name, Type func)
	  {

		int funcIndex;
		object funcIndexObj = getFunctionID(name);

		if (null != funcIndexObj)
		{
		  funcIndex = ((int?) funcIndexObj).Value;

		  if (funcIndex < NUM_BUILT_IN_FUNCS)
		  {
				  funcIndex = m_funcNextFreeIndex++;
				  m_functionID_customer[name] = new int?(funcIndex);
		  }
		  m_functions_customer[funcIndex - NUM_BUILT_IN_FUNCS] = func;
		}
		else
		{
				funcIndex = m_funcNextFreeIndex++;

				m_functions_customer[funcIndex - NUM_BUILT_IN_FUNCS] = func;

				m_functionID_customer[name] = new int?(funcIndex);
		}
		return funcIndex;
	  }

	  /// <summary>
	  /// Tell if a built-in, non-namespaced function is available.
	  /// </summary>
	  /// <param name="methName"> The local name of the function.
	  /// </param>
	  /// <returns> True if the function can be executed. </returns>
	  public virtual bool functionAvailable(string methName)
	  {
		  object tblEntry = m_functionID[methName];
		  if (null != tblEntry)
		  {
			  return true;
		  }
		  else
		  {
				  tblEntry = m_functionID_customer[methName];
				  return (null != tblEntry)? true : false;
		  }
	  }
	}

}