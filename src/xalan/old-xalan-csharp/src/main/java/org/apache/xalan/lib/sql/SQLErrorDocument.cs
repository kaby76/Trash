using System;

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
 * $Id: SQLErrorDocument.java 468638 2006-10-28 06:52:06Z minchau $
 */

namespace org.apache.xalan.lib.sql
{


	using DTM = org.apache.xml.dtm.DTM;
	using DTMManager = org.apache.xml.dtm.DTMManager;

	/// 
	/// <summary>
	/// A base class that will convert an exception into an XML stream
	/// that can be returned in place of the standard result. The XML
	/// format returned is a follows.
	/// 
	/// <ext-error>
	///  <message> The Message for a generic error </message>
	///  <sql-error>
	///    <message> SQL Message from the Exception thrown </message>
	///    <code> SQL Error Code </stack>
	///  </sql-error>
	/// <ext-error>
	/// 
	/// </summary>

	/// <summary>
	/// The SQL Document is the main controlling class the executesa SQL Query
	/// </summary>
	public class SQLErrorDocument : DTMDocument
	{
	  private const string S_EXT_ERROR = "ext-error";
	  private const string S_SQL_ERROR = "sql-error";
	  private const string S_MESSAGE = "message";
	  private const string S_CODE = "code";

	  private const string S_STATE = "state";

	  private const string S_SQL_WARNING = "sql-warning";

	  private int m_ErrorExt_TypeID = org.apache.xml.dtm.DTM_Fields.NULL;
	  private int m_Message_TypeID = org.apache.xml.dtm.DTM_Fields.NULL;
	  private int m_Code_TypeID = org.apache.xml.dtm.DTM_Fields.NULL;

	  private int m_State_TypeID = org.apache.xml.dtm.DTM_Fields.NULL;

	  private int m_SQLWarning_TypeID = org.apache.xml.dtm.DTM_Fields.NULL;

	  private int m_SQLError_TypeID = org.apache.xml.dtm.DTM_Fields.NULL;

	  private int m_rootID = org.apache.xml.dtm.DTM_Fields.NULL;
	  private int m_extErrorID = org.apache.xml.dtm.DTM_Fields.NULL;
	  private int m_MainMessageID = org.apache.xml.dtm.DTM_Fields.NULL;

	  /// <summary>
	  /// Build up an SQLErrorDocument that includes the basic error information
	  /// along with the Extended SQL Error information. </summary>
	  /// <param name="mgr"> </param>
	  /// <param name="ident"> </param>
	  /// <param name="error"> </param>
	  public SQLErrorDocument(DTMManager mgr, int ident, SQLException error) : base(mgr, ident)
	  {

		createExpandedNameTable();
		buildBasicStructure(error);

		int sqlError = addElement(2, m_SQLError_TypeID, m_extErrorID, m_MainMessageID);
		int element = org.apache.xml.dtm.DTM_Fields.NULL;

		element = addElementWithData(new int?(error.ErrorCode), 3, m_Code_TypeID, sqlError, element);

		element = addElementWithData(error.LocalizedMessage, 3, m_Message_TypeID, sqlError, element);

	//    this.dumpDTM();
	  }


	  /// <summary>
	  /// Build up an Error Exception with just the Standard Error Information </summary>
	  /// <param name="mgr"> </param>
	  /// <param name="ident"> </param>
	  /// <param name="error"> </param>
	  public SQLErrorDocument(DTMManager mgr, int ident, Exception error) : base(mgr, ident)
	  {
		createExpandedNameTable();
		buildBasicStructure(error);
	  }

	  /// <summary>
	  /// Build up an Error Exception with just the Standard Error Information </summary>
	  /// <param name="mgr"> </param>
	  /// <param name="ident"> </param>
	  /// <param name="error"> </param>
	  public SQLErrorDocument(DTMManager mgr, int ident, Exception error, SQLWarning warning, bool full) : base(mgr, ident)
	  {
		createExpandedNameTable();
		buildBasicStructure(error);

		SQLException se = null;
		int prev = m_MainMessageID;
		bool inWarnings = false;

		if (error != null && error is SQLException)
		{
			se = (SQLException)error;
		}
		else if (full && warning != null)
		{
			se = warning;
			inWarnings = true;
		}

		while (se != null)
		{
			int sqlError = addElement(2, inWarnings ? m_SQLWarning_TypeID : m_SQLError_TypeID, m_extErrorID, prev);
			prev = sqlError;
			int element = org.apache.xml.dtm.DTM_Fields.NULL;

			element = addElementWithData(new int?(se.ErrorCode), 3, m_Code_TypeID, sqlError, element);

			element = addElementWithData(se.LocalizedMessage, 3, m_Message_TypeID, sqlError, element);

			if (full)
			{
				string state = se.SQLState;
				if (!string.ReferenceEquals(state, null) && state.Length > 0)
				{
					element = addElementWithData(state, 3, m_State_TypeID, sqlError, element);
				}

				if (inWarnings)
				{
					se = ((SQLWarning)se).NextWarning;
				}
				else
				{
					se = se.NextException;
				}
			}
			else
			{
				se = null;
			}
		}
	  }

	  /// <summary>
	  /// Build up the basic structure that is common for each error. </summary>
	  /// <param name="e">
	  /// @return </param>
	  private void buildBasicStructure(Exception e)
	  {
		m_rootID = addElement(0, m_Document_TypeID, org.apache.xml.dtm.DTM_Fields.NULL, org.apache.xml.dtm.DTM_Fields.NULL);
		m_extErrorID = addElement(1, m_ErrorExt_TypeID, m_rootID, org.apache.xml.dtm.DTM_Fields.NULL);
		m_MainMessageID = addElementWithData(e != null ? e.LocalizedMessage : "SQLWarning", 2, m_Message_TypeID, m_extErrorID, org.apache.xml.dtm.DTM_Fields.NULL);
	  }

	  /// <summary>
	  /// Populate the Expanded Name Table with the Node that we will use.
	  /// Keep a reference of each of the types for access speed.
	  /// @return
	  /// </summary>
	  protected internal override void createExpandedNameTable()
	  {

		base.createExpandedNameTable();

		m_ErrorExt_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_EXT_ERROR, org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE);

		m_SQLError_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_SQL_ERROR, org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE);

		m_Message_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_MESSAGE, org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE);

		m_Code_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_CODE, org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE);

		m_State_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_STATE, org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE);

		m_SQLWarning_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_SQL_WARNING, org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE);
	  }

	}

}