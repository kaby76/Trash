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
 * $Id: NodeLocator.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xml.dtm.@ref
{

	/// <summary>
	/// <code>NodeLocator</code> maintains information on an XML source
	/// node.
	/// 
	/// @author <a href="mailto:ovidiu@cup.hp.com">Ovidiu Predescu</a>
	/// @since May 23, 2001
	/// </summary>
	public class NodeLocator : SourceLocator
	{
	  protected internal string m_publicId;
	  protected internal string m_systemId;
	  protected internal int m_lineNumber;
	  protected internal int m_columnNumber;

	  /// <summary>
	  /// Creates a new <code>NodeLocator</code> instance.
	  /// </summary>
	  /// <param name="publicId"> a <code>String</code> value </param>
	  /// <param name="systemId"> a <code>String</code> value </param>
	  /// <param name="lineNumber"> an <code>int</code> value </param>
	  /// <param name="columnNumber"> an <code>int</code> value </param>
	  public NodeLocator(string publicId, string systemId, int lineNumber, int columnNumber)
	  {
		this.m_publicId = publicId;
		this.m_systemId = systemId;
		this.m_lineNumber = lineNumber;
		this.m_columnNumber = columnNumber;
	  }

	  /// <summary>
	  /// <code>getPublicId</code> returns the public ID of the node.
	  /// </summary>
	  /// <returns> a <code>String</code> value </returns>
	  public virtual string PublicId
	  {
		  get
		  {
			return m_publicId;
		  }
	  }

	  /// <summary>
	  /// <code>getSystemId</code> returns the system ID of the node.
	  /// </summary>
	  /// <returns> a <code>String</code> value </returns>
	  public virtual string SystemId
	  {
		  get
		  {
			return m_systemId;
		  }
	  }

	  /// <summary>
	  /// <code>getLineNumber</code> returns the line number of the node.
	  /// </summary>
	  /// <returns> an <code>int</code> value </returns>
	  public virtual int LineNumber
	  {
		  get
		  {
			return m_lineNumber;
		  }
	  }

	  /// <summary>
	  /// <code>getColumnNumber</code> returns the column number of the
	  /// node.
	  /// </summary>
	  /// <returns> an <code>int</code> value </returns>
	  public virtual int ColumnNumber
	  {
		  get
		  {
			return m_columnNumber;
		  }
	  }

	  /// <summary>
	  /// <code>toString</code> returns a string representation of this
	  /// NodeLocator instance.
	  /// </summary>
	  /// <returns> a <code>String</code> value </returns>
	  public override string ToString()
	  {
		return "file '" + m_systemId + "', line #" + m_lineNumber + ", column #" + m_columnNumber;
	  }
	}

}