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
 * $Id: XObject.java 1225284 2011-12-28 18:56:49Z mrglavas $
 */
namespace org.apache.xpath.objects
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XMLString = org.apache.xml.utils.XMLString;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathException = org.apache.xpath.XPathException;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	using DocumentFragment = org.w3c.dom.DocumentFragment;
	using NodeList = org.w3c.dom.NodeList;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// This class represents an XPath object, and is capable of
	/// converting the object to various types, such as a string.
	/// This class acts as the base class to other XPath type objects,
	/// such as XString, and provides polymorphic casting capabilities.
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class XObject : Expression, ICloneable
	{
		internal new const long serialVersionUID = -821887098985662951L;

	  /// <summary>
	  /// The java object which this object wraps.
	  ///  @serial  
	  /// </summary>
	  protected internal object m_obj; // This may be NULL!!!

	  /// <summary>
	  /// Create an XObject.
	  /// </summary>
	  public XObject()
	  {
	  }

	  /// <summary>
	  /// Create an XObject.
	  /// </summary>
	  /// <param name="obj"> Can be any object, should be a specific type
	  /// for derived classes, or null. </param>
	  public XObject(object obj)
	  {
		Object = obj;
	  }

	  protected internal virtual object Object
	  {
		  set
		  {
			  m_obj = value;
		  }
	  }

	  /// <summary>
	  /// For support of literal objects in xpaths.
	  /// </summary>
	  /// <param name="xctxt"> The XPath execution context.
	  /// </param>
	  /// <returns> This object.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		return this;
	  }

	  /// <summary>
	  /// Specify if it's OK for detach to release the iterator for reuse.
	  /// This function should be called with a value of false for objects that are 
	  /// stored in variables.
	  /// Calling this with a value of false on a XNodeSet will cause the nodeset 
	  /// to be cached.
	  /// </summary>
	  /// <param name="allowRelease"> true if it is OK for detach to release this iterator
	  /// for pooling. </param>
	  public virtual void allowDetachToRelease(bool allowRelease)
	  {
	  }

	  /// <summary>
	  /// Detaches the <code>DTMIterator</code> from the set which it iterated
	  /// over, releasing any computational resources and placing the iterator
	  /// in the INVALID state. After <code>detach</code> has been invoked,
	  /// calls to <code>nextNode</code> or <code>previousNode</code> will
	  /// raise a runtime exception.
	  /// </summary>
	  public virtual void detach()
	  {
	  }

	  /// <summary>
	  /// Forces the object to release it's resources.  This is more harsh than
	  /// detach().
	  /// </summary>
	  public virtual void destruct()
	  {

		if (null != m_obj)
		{
		  allowDetachToRelease(true);
		  detach();

		  Object = null;
		}
	  }

	  /// <summary>
	  /// Reset for fresh reuse.
	  /// </summary>
	  public virtual void reset()
	  {
	  }

	  /// <summary>
	  /// Directly call the
	  /// characters method on the passed ContentHandler for the
	  /// string-value. Multiple calls to the
	  /// ContentHandler's characters methods may well occur for a single call to
	  /// this method.
	  /// </summary>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchCharactersEvents(org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
	  public virtual void dispatchCharactersEvents(org.xml.sax.ContentHandler ch)
	  {
		xstr().dispatchCharactersEvents(ch);
	  }

	  /// <summary>
	  /// Create the right XObject based on the type of the object passed.  This 
	  /// function can not make an XObject that exposes DOM Nodes, NodeLists, and 
	  /// NodeIterators to the XSLT stylesheet as node-sets.
	  /// </summary>
	  /// <param name="val"> The java object which this object will wrap.
	  /// </param>
	  /// <returns> the right XObject based on the type of the object passed. </returns>
	  public static XObject create(object val)
	  {
		return XObjectFactory.create(val);
	  }

	  /// <summary>
	  /// Create the right XObject based on the type of the object passed.
	  /// This function <emph>can</emph> make an XObject that exposes DOM Nodes, NodeLists, and 
	  /// NodeIterators to the XSLT stylesheet as node-sets.
	  /// </summary>
	  /// <param name="val"> The java object which this object will wrap. </param>
	  /// <param name="xctxt"> The XPath context.
	  /// </param>
	  /// <returns> the right XObject based on the type of the object passed. </returns>
	  public static XObject create(object val, XPathContext xctxt)
	  {
		return XObjectFactory.create(val, xctxt);
	  }

	  /// <summary>
	  /// Constant for NULL object type </summary>
	  public const int CLASS_NULL = -1;

	  /// <summary>
	  /// Constant for UNKNOWN object type </summary>
	  public const int CLASS_UNKNOWN = 0;

	  /// <summary>
	  /// Constant for BOOLEAN  object type </summary>
	  public const int CLASS_BOOLEAN = 1;

	  /// <summary>
	  /// Constant for NUMBER object type </summary>
	  public const int CLASS_NUMBER = 2;

	  /// <summary>
	  /// Constant for STRING object type </summary>
	  public const int CLASS_STRING = 3;

	  /// <summary>
	  /// Constant for NODESET object type </summary>
	  public const int CLASS_NODESET = 4;

	  /// <summary>
	  /// Constant for RESULT TREE FRAGMENT object type </summary>
	  public const int CLASS_RTREEFRAG = 5;

	  /// <summary>
	  /// Represents an unresolved variable type as an integer. </summary>
	  public const int CLASS_UNRESOLVEDVARIABLE = 600;

	  /// <summary>
	  /// Tell what kind of class this is.
	  /// </summary>
	  /// <returns> CLASS_UNKNOWN </returns>
	  public virtual int Type
	  {
		  get
		  {
			return CLASS_UNKNOWN;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string "#UNKNOWN" + object class name </returns>
	  public virtual string TypeString
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return "#UNKNOWN (" + @object().GetType().FullName + ")";
		  }
	  }

	  /// <summary>
	  /// Cast result object to a number. Always issues an error.
	  /// </summary>
	  /// <returns> 0.0
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double num() throws javax.xml.transform.TransformerException
	  public virtual double num()
	  {

		error(XPATHErrorResources.ER_CANT_CONVERT_TO_NUMBER, new object[]{TypeString}); //"Can not convert "+getTypeString()+" to a number");

		return 0.0;
	  }

	  /// <summary>
	  /// Cast result object to a number, but allow side effects, such as the 
	  /// incrementing of an iterator.
	  /// </summary>
	  /// <returns> numeric value of the string conversion from the 
	  /// next node in the NodeSetDTM, or NAN if no node was found </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double numWithSideEffects() throws javax.xml.transform.TransformerException
	  public virtual double numWithSideEffects()
	  {
		return num();
	  }

	  /// <summary>
	  /// Cast result object to a boolean. Always issues an error.
	  /// </summary>
	  /// <returns> false
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean bool() throws javax.xml.transform.TransformerException
	  public virtual bool @bool()
	  {

		error(XPATHErrorResources.ER_CANT_CONVERT_TO_NUMBER, new object[]{TypeString}); //"Can not convert "+getTypeString()+" to a number");

		return false;
	  }

	  /// <summary>
	  /// Cast result object to a boolean, but allow side effects, such as the 
	  /// incrementing of an iterator.
	  /// </summary>
	  /// <returns> True if there is a next node in the nodeset </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean boolWithSideEffects() throws javax.xml.transform.TransformerException
	  public virtual bool boolWithSideEffects()
	  {
		return @bool();
	  }


	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public virtual XMLString xstr()
	  {
		return XMLStringFactoryImpl.Factory.newstr(str());
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The object as a string </returns>
	  public virtual string str()
	  {
		return (m_obj != null) ? m_obj.ToString() : "";
	  }

	  /// <summary>
	  /// Return the string representation of the object
	  /// 
	  /// </summary>
	  /// <returns> the string representation of the object </returns>
	  public override string ToString()
	  {
		return str();
	  }

	  /// <summary>
	  /// Cast result object to a result tree fragment.
	  /// </summary>
	  /// <param name="support"> XPath context to use for the conversion
	  /// </param>
	  /// <returns> the objec as a result tree fragment. </returns>
	  public virtual int rtf(XPathContext support)
	  {

		int result = rtf();

		if (DTM.NULL == result)
		{
		  DTM frag = support.createDocumentFragment();

		  // %OPT%
		  frag.appendTextChild(str());

		  result = frag.Document;
		}

		return result;
	  }

	  /// <summary>
	  /// Cast result object to a result tree fragment.
	  /// </summary>
	  /// <param name="support"> XPath context to use for the conversion
	  /// </param>
	  /// <returns> the objec as a result tree fragment. </returns>
	  public virtual DocumentFragment rtree(XPathContext support)
	  {
		DocumentFragment docFrag = null;
		int result = rtf();

		if (DTM.NULL == result)
		{
		  DTM frag = support.createDocumentFragment();

		  // %OPT%
		  frag.appendTextChild(str());

		  docFrag = (DocumentFragment)frag.getNode(frag.Document);
		}
		else
		{
		  DTM frag = support.getDTM(result);
		  docFrag = (DocumentFragment)frag.getNode(frag.Document);
		}

		return docFrag;
	  }


	  /// <summary>
	  /// For functions to override.
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual DocumentFragment rtree()
	  {
		return null;
	  }

	  /// <summary>
	  /// For functions to override.
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual int rtf()
	  {
		return DTM.NULL;
	  }

	  /// <summary>
	  /// Return a java object that's closest to the representation
	  /// that should be handed to an extension.
	  /// </summary>
	  /// <returns> The object that this class wraps </returns>
	  public virtual object @object()
	  {
		return m_obj;
	  }

	  /// <summary>
	  /// Cast result object to a nodelist. Always issues an error.
	  /// </summary>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator iter() throws javax.xml.transform.TransformerException
	  public virtual DTMIterator iter()
	  {

		error(XPATHErrorResources.ER_CANT_CONVERT_TO_NODELIST, new object[]{TypeString}); //"Can not convert "+getTypeString()+" to a NodeList!");

		return null;
	  }

	  /// <summary>
	  /// Get a fresh copy of the object.  For use with variables.
	  /// </summary>
	  /// <returns> This object, unless overridden by subclass. </returns>
	  public virtual XObject Fresh
	  {
		  get
		  {
			return this;
		  }
	  }


	  /// <summary>
	  /// Cast result object to a nodelist. Always issues an error.
	  /// </summary>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.traversal.NodeIterator nodeset() throws javax.xml.transform.TransformerException
	  public virtual NodeIterator nodeset()
	  {

		error(XPATHErrorResources.ER_CANT_CONVERT_TO_NODELIST, new object[]{TypeString}); //"Can not convert "+getTypeString()+" to a NodeList!");

		return null;
	  }

	  /// <summary>
	  /// Cast result object to a nodelist. Always issues an error.
	  /// </summary>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.NodeList nodelist() throws javax.xml.transform.TransformerException
	  public virtual NodeList nodelist()
	  {

		error(XPATHErrorResources.ER_CANT_CONVERT_TO_NODELIST, new object[]{TypeString}); //"Can not convert "+getTypeString()+" to a NodeList!");

		return null;
	  }


	  /// <summary>
	  /// Cast result object to a nodelist. Always issues an error.
	  /// </summary>
	  /// <returns> The object as a NodeSetDTM.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.NodeSetDTM mutableNodeset() throws javax.xml.transform.TransformerException
	  public virtual NodeSetDTM mutableNodeset()
	  {

		error(XPATHErrorResources.ER_CANT_CONVERT_TO_MUTABLENODELIST, new object[]{TypeString}); //"Can not convert "+getTypeString()+" to a NodeSetDTM!");

		return (NodeSetDTM) m_obj;
	  }

	  /// <summary>
	  /// Cast object to type t.
	  /// </summary>
	  /// <param name="t"> Type of object to cast this to </param>
	  /// <param name="support"> XPath context to use for the conversion
	  /// </param>
	  /// <returns> This object as the given type t
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object castToType(int t, org.apache.xpath.XPathContext support) throws javax.xml.transform.TransformerException
	  public virtual object castToType(int t, XPathContext support)
	  {

		object result;

		switch (t)
		{
		case CLASS_STRING :
		  result = str();
		  break;
		case CLASS_NUMBER :
		  result = new double?(num());
		  break;
		case CLASS_NODESET :
		  result = iter();
		  break;
		case CLASS_BOOLEAN :
		  result = @bool() ? true : false;
		  break;
		case CLASS_UNKNOWN :
		  result = m_obj;
		  break;

		// %TBD%  What to do here?
		//    case CLASS_RTREEFRAG :
		//      result = rtree(support);
		//      break;
		default :
		  error(XPATHErrorResources.ER_CANT_CONVERT_TO_TYPE, new object[]{TypeString, Convert.ToString(t)}); //"Can not convert "+getTypeString()+" to a type#"+t);

		  result = null;
	  break;
		}

		return result;
	  }

	  /// <summary>
	  /// Tell if one object is less than the other.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if this object is less than the given object
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean lessThan(XObject obj2) throws javax.xml.transform.TransformerException
	  public virtual bool lessThan(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.  Because the arguments 
		// are backwards, we call the opposite comparison
		// function.
		if (obj2.Type == XObject.CLASS_NODESET)
		{
		  return obj2.greaterThan(this);
		}

		return this.num() < obj2.num();
	  }

	  /// <summary>
	  /// Tell if one object is less than or equal to the other.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if this object is less than or equal to the given object
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean lessThanOrEqual(XObject obj2) throws javax.xml.transform.TransformerException
	  public virtual bool lessThanOrEqual(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.  Because the arguments 
		// are backwards, we call the opposite comparison
		// function.
		if (obj2.Type == XObject.CLASS_NODESET)
		{
		  return obj2.greaterThanOrEqual(this);
		}

		return this.num() <= obj2.num();
	  }

	  /// <summary>
	  /// Tell if one object is greater than the other.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if this object is greater than the given object
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean greaterThan(XObject obj2) throws javax.xml.transform.TransformerException
	  public virtual bool greaterThan(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.  Because the arguments 
		// are backwards, we call the opposite comparison
		// function.
		if (obj2.Type == XObject.CLASS_NODESET)
		{
		  return obj2.lessThan(this);
		}

		return this.num() > obj2.num();
	  }

	  /// <summary>
	  /// Tell if one object is greater than or equal to the other.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if this object is greater than or equal to the given object
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean greaterThanOrEqual(XObject obj2) throws javax.xml.transform.TransformerException
	  public virtual bool greaterThanOrEqual(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.  Because the arguments 
		// are backwards, we call the opposite comparison
		// function.
		if (obj2.Type == XObject.CLASS_NODESET)
		{
		  return obj2.lessThanOrEqual(this);
		}

		return this.num() >= obj2.num();
	  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if this object is equal to the given object
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public virtual bool Equals(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.
		if (obj2.Type == XObject.CLASS_NODESET)
		{
		  return obj2.Equals(this);
		}

		if (null != m_obj)
		{
		  return m_obj.Equals(obj2.m_obj);
		}
		else
		{
		  return obj2.m_obj == null;
		}
	  }

	  /// <summary>
	  /// Tell if two objects are functionally not equal.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if this object is not equal to the given object
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean notEquals(XObject obj2) throws javax.xml.transform.TransformerException
	  public virtual bool notEquals(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.
		if (obj2.Type == XObject.CLASS_NODESET)
		{
		  return obj2.notEquals(this);
		}

		return !Equals(obj2);
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> Error message to issue
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void error(String msg) throws javax.xml.transform.TransformerException
	  protected internal virtual void error(string msg)
	  {
		error(msg, null);
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> Error message to issue </param>
	  /// <param name="args"> Arguments to use in the message
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void error(String msg, Object[] args) throws javax.xml.transform.TransformerException
	  protected internal virtual void error(string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHMessage(msg, args);

		// boolean shouldThrow = support.problem(m_support.XPATHPROCESSOR, 
		//                                      m_support.ERROR,
		//                                      null, 
		//                                      null, fmsg, 0, 0);
		// if(shouldThrow)
		{
		  throw new XPathException(fmsg, this);
		}
	  }


	  /// <summary>
	  /// XObjects should not normally need to fix up variables.
	  /// </summary>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		// no-op
	  }


	  /// <summary>
	  /// Cast result object to a string.
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="fsb"> </param>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public virtual void appendToFsb(org.apache.xml.utils.FastStringBuffer fsb)
	  {
		fsb.append(str());
	  }

	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  assertion(false, "callVisitors should not be called for this object!!!");
	  }
	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!isSameClass(expr))
		  {
			  return false;
		  }

		  // If equals at the expression level calls deepEquals, I think we're 
		  // still safe from infinite recursion since this object overrides 
		  // equals.  I hope.
		  if (!this.Equals((XObject)expr))
		  {
			  return false;
		  }

		  return true;
	  }

	}

}