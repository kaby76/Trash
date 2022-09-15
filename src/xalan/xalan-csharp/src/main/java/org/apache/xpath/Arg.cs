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
 * $Id: Arg.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{
	using QName = org.apache.xml.utils.QName;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This class holds an instance of an argument on
	/// the stack. The value of the argument can be either an
	/// XObject or a String containing an expression.
	/// @xsl.usage internal
	/// </summary>
	public class Arg
	{

	  /// <summary>
	  /// Field m_qname: The name of this argument, expressed as a QName
	  /// (Qualified Name) object. </summary>
	  /// <seealso cref="getQName"/>
	  /// <seealso cref="setQName"
	  /// />
	  private QName m_qname;

	  /// <summary>
	  /// Get the qualified name for this argument.
	  /// </summary>
	  /// <returns> QName object containing the qualified name </returns>
	  public QName QName
	  {
		  get
		  {
			return m_qname;
		  }
		  set
		  {
			m_qname = value;
		  }
	  }


	  /// <summary>
	  /// Field m_val: Stored XObject value of this argument </summary>
	  /// <seealso cref=".getVal()"/>
	  /// <seealso cref=".setVal()"/>
	  private XObject m_val;

	  /// <summary>
	  /// Get the value for this argument.
	  /// </summary>
	  /// <returns> the argument's stored XObject value. </returns>
	  /// <seealso cref=".setVal(XObject)"/>
	  public XObject Val
	  {
		  get
		  {
			return m_val;
		  }
		  set
		  {
			m_val = value;
		  }
	  }


	  /// <summary>
	  /// Have the object release it's resources.
	  /// Call only when the variable or argument is going out of scope.
	  /// </summary>
	  public virtual void detach()
	  {
		if (null != m_val)
		{
		  m_val.allowDetachToRelease(true);
		  m_val.detach();
		}
	  }


	  /// <summary>
	  /// Field m_expression: Stored expression value of this argument. </summary>
	  /// <seealso cref=".setExpression"/>
	  /// <seealso cref=".getExpression"
	  /// />
	  private string m_expression;

	  /// <summary>
	  /// Get the value expression for this argument.
	  /// </summary>
	  /// <returns> String containing the expression previously stored into this
	  /// argument </returns>
	  /// <seealso cref=".setExpression"/>
	  public virtual string Expression
	  {
		  get
		  {
			return m_expression;
		  }
		  set
		  {
			m_expression = value;
		  }
	  }


	  /// <summary>
	  /// True if this variable was added with an xsl:with-param or
	  /// is added via setParameter.
	  /// </summary>
	  private bool m_isFromWithParam;

	  /// <summary>
	  /// Tell if this variable is a parameter passed with a with-param or as 
	  /// a top-level parameter.
	  /// </summary>
	   public virtual bool FromWithParam
	   {
		   get
		   {
			return m_isFromWithParam;
		   }
	   }

	  /// <summary>
	  /// True if this variable is currently visible.  To be visible,
	  /// a variable needs to come either from xsl:variable or be 
	  /// a "received" parameter, ie one for which an xsl:param has
	  /// been encountered.
	  /// Set at the time the object is constructed and updated as needed.
	  /// </summary>
	  private bool m_isVisible;

	  /// <summary>
	  /// Tell if this variable is currently visible.
	  /// </summary>
	   public virtual bool Visible
	   {
		   get
		   {
			return m_isVisible;
		   }
	   }

	  /// <summary>
	  /// Update visibility status of this variable.
	  /// </summary>
	   public virtual bool IsVisible
	   {
		   set
		   {
			m_isVisible = value;
		   }
	   }

	  /// <summary>
	  /// Construct a dummy parameter argument, with no QName and no
	  /// value (either expression string or value XObject). isVisible
	  /// defaults to true.
	  /// </summary>
	  public Arg()
	  {

		m_qname = new QName("");
		; // so that string compares can be done.
		m_val = null;
		m_expression = null;
		m_isVisible = true;
		m_isFromWithParam = false;
	  }

	  /// <summary>
	  /// Construct a parameter argument that contains an expression.
	  /// </summary>
	  /// <param name="qname"> Name of the argument, expressed as a QName object. </param>
	  /// <param name="expression"> String to be stored as this argument's value expression. </param>
	  /// <param name="isFromWithParam"> True if this is a parameter variable. </param>
	  public Arg(QName qname, string expression, bool isFromWithParam)
	  {

		m_qname = qname;
		m_val = null;
		m_expression = expression;
		m_isFromWithParam = isFromWithParam;
		m_isVisible = !isFromWithParam;
	  }

	  /// <summary>
	  /// Construct a parameter argument which has an XObject value.
	  /// isVisible defaults to true.
	  /// </summary>
	  /// <param name="qname"> Name of the argument, expressed as a QName object. </param>
	  /// <param name="val"> Value of the argument, expressed as an XObject </param>
	  public Arg(QName qname, XObject val)
	  {

		m_qname = qname;
		m_val = val;
		m_isVisible = true;
		m_isFromWithParam = false;
		m_expression = null;
	  }

	  /// <summary>
	  /// Equality function specialized for the variable name.  If the argument 
	  /// is not a qname, it will deligate to the super class.
	  /// </summary>
	  /// <param name="obj">   the reference object with which to compare. </param>
	  /// <returns>  <code>true</code> if this object is the same as the obj
	  ///          argument; <code>false</code> otherwise. </returns>
	  public override bool Equals(object obj)
	  {
		if (obj is QName)
		{
		  return m_qname.Equals(obj);
		}
		else
		{
		  return base.Equals(obj);
		}
	  }

	  /// <summary>
	  /// Construct a parameter argument.
	  /// </summary>
	  /// <param name="qname"> Name of the argument, expressed as a QName object. </param>
	  /// <param name="val"> Value of the argument, expressed as an XObject </param>
	  /// <param name="isFromWithParam"> True if this is a parameter variable. </param>
	  public Arg(QName qname, XObject val, bool isFromWithParam)
	  {

		m_qname = qname;
		m_val = val;
		m_isFromWithParam = isFromWithParam;
		m_isVisible = !isFromWithParam;
		m_expression = null;
	  }
	}

}