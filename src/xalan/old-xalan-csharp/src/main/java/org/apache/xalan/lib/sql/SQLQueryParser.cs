using System;
using System.Collections;
using System.Text;

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
 * $Id: SQLQueryParser.java 468638 2006-10-28 06:52:06Z minchau $
 */

/// <summary>
/// This is used by the SQLDocumentHandler for processing JDBC queries.
/// This prepares JDBC PreparedStatement or CallableStatements and the
/// input/output of parameters from/to variables.
///  
/// </summary>

namespace org.apache.xalan.lib.sql
{

	using org.apache.xpath.objects;
	using ExpressionContext = org.apache.xalan.extensions.ExpressionContext;
	using QName = org.apache.xml.utils.QName;



	public class SQLQueryParser
	{
	  /// <summary>
	  /// If the parser used inline parser to pull out variables then
	  /// this will be true. The default is not to use the Inline Parser.
	  /// </summary>
	  private bool m_InlineVariables = false;

	  /// 
	  private bool m_IsCallable = false;

	  /// 
	  private string m_OrigQuery = null;

	  /// 
	  private StringBuilder m_ParsedQuery = null;

	  /// 
	  private ArrayList m_Parameters = null;

	  /// 
	  private bool m_hasOutput = false;

	  /// 
	  private bool m_HasParameters;

	  public const int NO_OVERRIDE = 0;
	  public const int NO_INLINE_PARSER = 1;
	  public const int INLINE_PARSER = 2;

	  /// <summary>
	  /// The SQLStatement Parser will be created as a psuedo SINGLETON per
	  /// XConnection. Since we are only caching the Query and its parsed results
	  /// we may be able to use this as a real SINGLETON. It all depends on how
	  /// Statement Caching will play out.
	  /// </summary>
	  public SQLQueryParser()
	  {
		init();
	  }

	  /// <summary>
	  /// Constructor, used to create a new parser entry
	  /// </summary>
	  private SQLQueryParser(string query)
	  {
		m_OrigQuery = query;
	  }

	  /// <summary>
	  /// On a per Xconnection basis, we will create a SQLStatemenetParser, from
	  /// this parser, individual parsers will be created. The Init method is defined
	  /// to initialize all the internal structures that maintains the pool of parsers.
	  /// </summary>
	  private void init()
	  {
		// Do nothing for now.
	  }

	  /// <summary>
	  /// Produce an SQL Statement Parser based on the incomming query.
	  /// 
	  /// For now we will just create a new object, in the future we may have this
	  /// interface cache the queries so that we can take advantage of a preparsed
	  /// String.
	  /// 
	  /// If the Inline Parser is not enabled in the Options, no action will be
	  /// taken on the parser. This option can be set by the Stylesheet. If the
	  /// option is not set or cleared, a default value will be set determined
	  /// by the way variables were passed into the system.
	  /// </summary>
	  public virtual SQLQueryParser parse(XConnection xconn, string query, int @override)
	  {
		SQLQueryParser parser = new SQLQueryParser(query);

		// Try to implement caching here, if we found a parser in the cache
		// then just return the instance otherwise
		parser.parse(xconn, @override);

		return parser;
	  }



	  /// <summary>
	  /// Produce an SQL Statement Parser based on the incomming query.
	  /// 
	  /// For now we will just create a new object, in the future we may have this
	  /// interface cache the queries so that we can take advantage of a preparsed
	  /// String.
	  /// 
	  /// If the Inline Parser is not enabled in the Options, no action will be
	  /// taken on the parser. This option can be set by the Stylesheet. If the
	  /// option is not set or cleared, a default value will be set determined
	  /// by the way variables were passed into the system.
	  /// </summary>
	  private void parse(XConnection xconn, int @override)
	  {
		// Grab the Feature here. We could maintain it from the Parent Parser
		// but that may cause problems if a single XConnection wants to maintain
		// both Inline Variable Statemens along with NON inline variable statements.

		m_InlineVariables = "true".Equals(xconn.getFeature("inline-variables"));
		if (@override == NO_INLINE_PARSER)
		{
			m_InlineVariables = false;
		}
		else if (@override == INLINE_PARSER)
		{
			m_InlineVariables = true;
		}

		if (m_InlineVariables)
		{
			inlineParser();
		}

	  }

	  /// <summary>
	  /// If a SQL Statement does not have any parameters, then it can be executed
	  /// directly. Most SQL Servers use this as a performance advantage since no
	  /// parameters need to be parsed then bound.
	  /// </summary>
	  public virtual bool hasParameters()
	  {
		return m_HasParameters;
	  }

	  /// <summary>
	  /// If the Inline Parser is used, the parser will note if this stastement is
	  /// a plain SQL Statement or a Called Procedure. Called Procudures generally
	  /// have output parameters and require special handling.
	  /// 
	  /// Called Procudures that are not processed with the Inline Parser will
	  /// still be executed but under the context of a PreparedStatement and
	  /// not a CallableStatement. Called Procudures that have output parameters
	  /// MUST be handled with the Inline Parser.
	  /// </summary>
	  public virtual bool Callable
	  {
		  get
		  {
			return m_IsCallable;
		  }
	  }


	  /// 
	  public virtual ArrayList Parameters
	  {
		  get
		  {
			return m_Parameters;
		  }
		  set
		  {
			m_HasParameters = true;
			m_Parameters = value;
		  }
	  }


	  /// <summary>
	  /// Return a copy of the parsed SQL query that will be set to the
	  /// Database system to execute. If the inline parser was not used,
	  /// then the original query will be returned.
	  /// </summary>
	  public virtual string SQLQuery
	  {
		  get
		  {
			if (m_InlineVariables)
			{
				return m_ParsedQuery.ToString();
			}
			else
			{
				return m_OrigQuery;
			}
		  }
	  }


	  /// <summary>
	  /// The SQL Statement Parser, when an Inline Parser is used, tracks the XSL
	  /// variables used to populate a statement. The data use to popoulate a
	  /// can also be provided. If the data is provided, it will overide the
	  /// populastion using XSL variables. When the Inline PArser is not used, then
	  /// the Data will always be provided.
	  /// 
	  /// </summary>
	  public virtual void populateStatement(PreparedStatement stmt, ExpressionContext ctx)
	  {
		// Set input parameters from variables.
	//    for ( int indx = returnParm ? 1 : 0 ; indx < m_Parameters.size() ; indx++ )

		for (int indx = 0 ; indx < m_Parameters.Count ; indx++)
		{
		  QueryParameter parm = (QueryParameter) m_Parameters[indx];

		  try
		  {

			if (m_InlineVariables)
			{
			  XObject value = (XObject)ctx.getVariableOrParam(new QName(parm.Name));

			  if (value != null)
			  {
				stmt.setObject(indx + 1, value.@object(), parm.Type, 4); // Currently defaulting scale to 4 - should read this!
			  }
			  else
			  {
				stmt.setNull(indx + 1, parm.Type);
			  }
			}
			else
			{
			  string value = parm.Value;

			  if (!string.ReferenceEquals(value, null))
			  {
				stmt.setObject(indx + 1, value, parm.Type, 4); // Currently defaulting scale to 4 - should read this!
			  }
			  else
			  {
				stmt.setNull(indx + 1, parm.Type);
			  }
			}
		  }
		  catch (Exception)
		  {
	//        if ( ! parm.isOutput() ) throw new SQLException(tx.toString());
		  }
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerOutputParameters(CallableStatement cstmt) throws SQLException
	  public virtual void registerOutputParameters(CallableStatement cstmt)
	  {
		// Register output parameters if call.
		if (m_IsCallable && m_hasOutput)
		{
		  for (int indx = 0 ; indx < m_Parameters.Count ; indx++)
		  {
			QueryParameter parm = (QueryParameter) m_Parameters[indx];
			if (parm.Output)
			{
			  //System.out.println("chrysalisSQLStatement() Registering output parameter for parm " + indx);
			  cstmt.registerOutParameter(indx + 1, parm.Type);
			}
		  }
		}
	  }

	  /// 
	  protected internal virtual void inlineParser()
	  {
		QueryParameter curParm = null;
		int state = 0;
		StringBuilder tok = new StringBuilder();
		bool firstword = true;

		if (m_Parameters == null)
		{
			m_Parameters = new ArrayList();
		}

		if (m_ParsedQuery == null)
		{
			m_ParsedQuery = new StringBuilder();
		}

		for (int idx = 0 ; idx < m_OrigQuery.Length ; idx++)
		{
		  char ch = m_OrigQuery[idx];
		  switch (state)
		  {

			case 0: // Normal
			  if (ch == '\'')
			  {
				  state = 1;
			  }
			  else if (ch == '?')
			  {
				  state = 4;
			  }
			  else if (firstword && (char.IsLetterOrDigit(ch) || ch == '#'))
			  {
				tok.Append(ch);
				state = 3;
			  }
			  m_ParsedQuery.Append(ch);
			  break;

			case 1: // In String
			  if (ch == '\'')
			  {
				  state = 0;
			  }
			  else if (ch == '\\')
			  {
				  state = 2;
			  }
			  m_ParsedQuery.Append(ch);
			  break;

			case 2: // In escape
			  state = 1;
			  m_ParsedQuery.Append(ch);
			  break;

			case 3: // First word
			  if (char.IsLetterOrDigit(ch) || ch == '#' || ch == '_')
			  {
				  tok.Append(ch);
			  }
			  else
			  {
				if (tok.ToString().Equals("call", StringComparison.CurrentCultureIgnoreCase))
				{
				  m_IsCallable = true;
				  if (curParm != null)
				  {
					// returnParm = true;
					curParm.IsOutput = true;
					// hasOutput = true;
				  }
				}
				firstword = false;
				tok = new StringBuilder();
				if (ch == '\'')
				{
					state = 1;
				}
				else if (ch == '?')
				{
					state = 4;
				}
				else
				{
					state = 0;
				}
			  }

			  m_ParsedQuery.Append(ch);
			  break;

			case 4: // Get variable definition
			  if (ch == '[')
			  {
				  state = 5;
			  }
			  break;

			case 5: // Read variable type.
			  if (!char.IsWhiteSpace(ch) && ch != '=')
			  {
				tok.Append(char.ToUpper(ch));
			  }
			  else if (tok.Length > 0)
			  {
				// OK we have at least one parameter.
				m_HasParameters = true;

				curParm = new QueryParameter();

				curParm.TypeName = tok.ToString();
	//            curParm.type = map_type(curParm.typeName);
				m_Parameters.Add(curParm);
				tok = new StringBuilder();
				if (ch == '=')
				{
					state = 7;
				}
				else
				{
					state = 6;
				}
			  }
			  break;

			case 6: // Look for '='
			  if (ch == '=')
			  {
				  state = 7;
			  }
			  break;

			case 7: // Read variable name.
			  if (!char.IsWhiteSpace(ch) && ch != ']')
			  {
				  tok.Append(ch);
			  }
			  else if (tok.Length > 0)
			  {
				curParm.Name = tok.ToString();
				tok = new StringBuilder();
				if (ch == ']')
				{
				  //param_output.addElement(new Boolean(false));
				  state = 0;
				}
				else
				{
					state = 8;
				}
			  }
			  break;

			case 8: // Look for "OUTput.
			  if (!char.IsWhiteSpace(ch) && ch != ']')
			  {
				tok.Append(ch);
			  }
			  else if (tok.Length > 0)
			  {
				tok.Length = 3;
				if (tok.ToString().Equals("OUT", StringComparison.CurrentCultureIgnoreCase))
				{
				  curParm.IsOutput = true;
				  m_hasOutput = true;
				}

				tok = new StringBuilder();
				if (ch == ']')
				{
				  state = 0;
				}
			  }
			  break;
		  }
		}


		// Prepare statement or call.
		if (m_IsCallable)
		{
		  m_ParsedQuery.Insert(0, '{');
		  m_ParsedQuery.Append('}');
		}

	  }

	}


}