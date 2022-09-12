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
 * $Id: VariableStack.java 524812 2007-04-02 15:52:03Z zongaro $
 */
namespace org.apache.xpath
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Defines a class to keep track of a stack for
	/// template arguments and variables.
	/// 
	/// <para>This has been changed from the previous incarnations of this
	/// class to be fairly low level.</para>
	/// @xsl.usage internal
	/// </summary>
	public class VariableStack : ICloneable
	{
	  /// <summary>
	  /// limitation for 1K
	  /// </summary>
	  public const int CLEARLIMITATION = 1024;

	  /// <summary>
	  /// Constructor for a variable stack.
	  /// </summary>
	  public VariableStack()
	  {
		reset();
	  }

	  /// <summary>
	  /// Constructor for a variable stack. </summary>
	  /// <param name="initStackSize"> The initial stack size.  Must be at least one.  The
	  ///                      stack can grow if needed. </param>
	  public VariableStack(int initStackSize)
	  {
		// Allow for twice as many variables as stack link entries
		reset(initStackSize, initStackSize * 2);
	  }

	  /// <summary>
	  /// Returns a clone of this variable stack.
	  /// </summary>
	  /// <returns>  a clone of this variable stack.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized Object clone() throws CloneNotSupportedException
	  public virtual object clone()
	  {
		  lock (this)
		  {
        
			VariableStack vs = (VariableStack) base.clone();
        
			// I *think* I can get away with a shallow clone here?
			vs._stackFrames = (XObject[]) _stackFrames.Clone();
			vs._links = (int[]) _links.Clone();
        
			return vs;
		  }
	  }

	  /// <summary>
	  /// The stack frame where all variables and params will be kept.
	  /// @serial
	  /// </summary>
	  internal XObject[] _stackFrames;

	  /// <summary>
	  /// The top of the stack frame (<code>_stackFrames</code>).
	  /// @serial
	  /// </summary>
	  internal int _frameTop;

	  /// <summary>
	  /// The bottom index of the current frame (relative to <code>_stackFrames</code>).
	  /// @serial
	  /// </summary>
	  private int _currentFrameBottom;

	  /// <summary>
	  /// The stack of frame positions.  I call 'em links because of distant
	  /// <a href="http://math.millikin.edu/mprogers/Courses/currentCourses/CS481-ComputerArchitecture/cs481.Motorola68000.html">
	  /// Motorola 68000 assembler</a> memories.  :-)
	  /// @serial
	  /// </summary>
	  internal int[] _links;

	  /// <summary>
	  /// The top of the links stack.
	  /// </summary>
	  internal int _linksTop;

	  /// <summary>
	  /// Get the element at the given index, regardless of stackframe.
	  /// </summary>
	  /// <param name="i"> index from zero.
	  /// </param>
	  /// <returns> The item at the given index. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject elementAt(final int i)
	  public virtual XObject elementAt(int i)
	  {
		return _stackFrames[i];
	  }

	  /// <summary>
	  /// Get size of the stack.
	  /// </summary>
	  /// <returns> the total size of the execution stack. </returns>
	  public virtual int size()
	  {
		return _frameTop;
	  }

	  /// <summary>
	  /// Reset the stack to a start position.
	  /// </summary>
	  public virtual void reset()
	  {
		// If the stack was previously allocated, assume that about the same
		// amount of stack space will be needed again; otherwise, use a very
		// large stack size.
		int linksSize = (_links == null) ? XPathContext.RECURSIONLIMIT : _links.Length;
		int varArraySize = (_stackFrames == null) ? XPathContext.RECURSIONLIMIT * 2 : _stackFrames.Length;
		reset(linksSize, varArraySize);
	  }

	  /// <summary>
	  /// Reset the stack to a start position. </summary>
	  /// <param name="linksSize"> Initial stack size to use </param>
	  /// <param name="varArraySize"> Initial variable array size to use </param>
	  protected internal virtual void reset(int linksSize, int varArraySize)
	  {
		_frameTop = 0;
		_linksTop = 0;

		// Don't bother reallocating _links array if it exists already
		if (_links == null)
		{
		  _links = new int[linksSize];
		}

		// Adding one here to the stack of frame positions will allow us always 
		// to look one under without having to check if we're at zero.
		// (As long as the caller doesn't screw up link/unlink.)
		_links[_linksTop++] = 0;

		// Get a clean _stackFrames array and discard the old one.
		_stackFrames = new XObject[varArraySize];
	  }

	  /// <summary>
	  /// Set the current stack frame.
	  /// </summary>
	  /// <param name="sf"> The new stack frame position. </param>
	  public virtual int StackFrame
	  {
		  set
		  {
			_currentFrameBottom = value;
		  }
		  get
		  {
			return _currentFrameBottom;
		  }
	  }


	  /// <summary>
	  /// Allocates memory (called a stackframe) on the stack; used to store
	  /// local variables and parameter arguments.
	  /// 
	  /// <para>I use the link/unlink concept because of distant
	  /// <a href="http://math.millikin.edu/mprogers/Courses/currentCourses/CS481-ComputerArchitecture/cs481.Motorola68000.html">
	  /// Motorola 68000 assembler</a> memories.</para>
	  /// </summary>
	  /// <param name="size"> The size of the stack frame allocation.  This ammount should
	  /// normally be the maximum number of variables that you can have allocated
	  /// at one time in the new stack frame.
	  /// </param>
	  /// <returns> The bottom of the stack frame, from where local variable addressing
	  /// should start from. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int link(final int size)
	  public virtual int link(int size)
	  {

		_currentFrameBottom = _frameTop;
		_frameTop += size;

		if (_frameTop >= _stackFrames.Length)
		{
		  XObject[] newsf = new XObject[_stackFrames.Length + XPathContext.RECURSIONLIMIT + size];

		  Array.Copy(_stackFrames, 0, newsf, 0, _stackFrames.Length);

		  _stackFrames = newsf;
		}

		if (_linksTop + 1 >= _links.Length)
		{
		  int[] newlinks = new int[_links.Length + (CLEARLIMITATION * 2)];

		  Array.Copy(_links, 0, newlinks, 0, _links.Length);

		  _links = newlinks;
		}

		_links[_linksTop++] = _currentFrameBottom;

		return _currentFrameBottom;
	  }

	  /// <summary>
	  /// Free up the stack frame that was last allocated with
	  /// <seealso cref="#link(int size)"/>.
	  /// </summary>
	  public virtual void unlink()
	  {
		_frameTop = _links[--_linksTop];
		_currentFrameBottom = _links[_linksTop - 1];
	  }

	  /// <summary>
	  /// Free up the stack frame that was last allocated with
	  /// <seealso cref="#link(int size)"/>. </summary>
	  /// <param name="currentFrame"> The current frame to set to 
	  /// after the unlink. </param>
	  public virtual void unlink(int currentFrame)
	  {
		_frameTop = _links[--_linksTop];
		_currentFrameBottom = currentFrame;
	  }

	  /// <summary>
	  /// Set a local variable or parameter in the current stack frame.
	  /// 
	  /// </summary>
	  /// <param name="index"> Local variable index relative to the current stack
	  /// frame bottom.
	  /// </param>
	  /// <param name="val"> The value of the variable that is being set. </param>
	  public virtual void setLocalVariable(int index, XObject val)
	  {
		_stackFrames[index + _currentFrameBottom] = val;
	  }

	  /// <summary>
	  /// Set a local variable or parameter in the specified stack frame.
	  /// 
	  /// </summary>
	  /// <param name="index"> Local variable index relative to the current stack
	  /// frame bottom. </param>
	  /// NEEDSDOC <param name="stackFrame">
	  /// </param>
	  /// <param name="val"> The value of the variable that is being set. </param>
	  public virtual void setLocalVariable(int index, XObject val, int stackFrame)
	  {
		_stackFrames[index + stackFrame] = val;
	  }

	  /// <summary>
	  /// Get a local variable or parameter in the current stack frame.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath context, which must be passed in order to
	  /// lazy evaluate variables.
	  /// </param>
	  /// <param name="index"> Local variable index relative to the current stack
	  /// frame bottom.
	  /// </param>
	  /// <returns> The value of the variable.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getLocalVariable(XPathContext xctxt, int index) throws javax.xml.transform.TransformerException
	  public virtual XObject getLocalVariable(XPathContext xctxt, int index)
	  {

		index += _currentFrameBottom;

		XObject val = _stackFrames[index];

		if (null == val)
		{
		  throw new TransformerException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_VARIABLE_ACCESSED_BEFORE_BIND, null), xctxt.SAXLocator);
		}
		  // "Variable accessed before it is bound!", xctxt.getSAXLocator());

		// Lazy execution of variables.
		if (val.Type == XObject.CLASS_UNRESOLVEDVARIABLE)
		{
		  return (_stackFrames[index] = val.execute(xctxt));
		}

		return val;
	  }

	  /// <summary>
	  /// Get a local variable or parameter in the current stack frame.
	  /// 
	  /// </summary>
	  /// <param name="index"> Local variable index relative to the given
	  /// frame bottom. </param>
	  /// NEEDSDOC <param name="frame">
	  /// </param>
	  /// <returns> The value of the variable.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getLocalVariable(int index, int frame) throws javax.xml.transform.TransformerException
	  public virtual XObject getLocalVariable(int index, int frame)
	  {

		index += frame;

		XObject val = _stackFrames[index];

		return val;
	  }

	  /// <summary>
	  /// Get a local variable or parameter in the current stack frame.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath context, which must be passed in order to
	  /// lazy evaluate variables.
	  /// </param>
	  /// <param name="index"> Local variable index relative to the current stack
	  /// frame bottom.
	  /// </param>
	  /// <returns> The value of the variable.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getLocalVariable(XPathContext xctxt, int index, boolean destructiveOK) throws javax.xml.transform.TransformerException
	  public virtual XObject getLocalVariable(XPathContext xctxt, int index, bool destructiveOK)
	  {

		index += _currentFrameBottom;

		XObject val = _stackFrames[index];

		if (null == val)
		{
		  throw new TransformerException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_VARIABLE_ACCESSED_BEFORE_BIND, null), xctxt.SAXLocator);
		}
		  // "Variable accessed before it is bound!", xctxt.getSAXLocator());

		// Lazy execution of variables.
		if (val.Type == XObject.CLASS_UNRESOLVEDVARIABLE)
		{
		  return (_stackFrames[index] = val.execute(xctxt));
		}

		return destructiveOK ? val : val.Fresh;
	  }

	  /// <summary>
	  /// Tell if a local variable has been set or not.
	  /// </summary>
	  /// <param name="index"> Local variable index relative to the current stack
	  /// frame bottom.
	  /// </param>
	  /// <returns> true if the value at the index is not null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isLocalSet(int index) throws javax.xml.transform.TransformerException
	  public virtual bool isLocalSet(int index)
	  {
		return (_stackFrames[index + _currentFrameBottom] != null);
	  }

	  /// <summary>
	  /// NEEDSDOC Field m_nulls </summary>
	  private static XObject[] m_nulls = new XObject[CLEARLIMITATION];

	  /// <summary>
	  /// Use this to clear the variables in a section of the stack.  This is
	  /// used to clear the parameter section of the stack, so that default param
	  /// values can tell if they've already been set.  It is important to note that
	  /// this function has a 1K limitation.
	  /// </summary>
	  /// <param name="start"> The start position, relative to the current local stack frame. </param>
	  /// <param name="len"> The number of slots to be cleared. </param>
	  public virtual void clearLocalSlots(int start, int len)
	  {

		start += _currentFrameBottom;

		Array.Copy(m_nulls, 0, _stackFrames, start, len);
	  }

	  /// <summary>
	  /// Set a global variable or parameter in the global stack frame.
	  /// 
	  /// </summary>
	  /// <param name="index"> Local variable index relative to the global stack frame
	  /// bottom.
	  /// </param>
	  /// <param name="val"> The value of the variable that is being set. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setGlobalVariable(final int index, final org.apache.xpath.objects.XObject val)
	  public virtual void setGlobalVariable(int index, XObject val)
	  {
		_stackFrames[index] = val;
	  }

	  /// <summary>
	  /// Get a global variable or parameter from the global stack frame.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath context, which must be passed in order to
	  /// lazy evaluate variables.
	  /// </param>
	  /// <param name="index"> Global variable index relative to the global stack
	  /// frame bottom.
	  /// </param>
	  /// <returns> The value of the variable.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getGlobalVariable(XPathContext xctxt, final int index) throws javax.xml.transform.TransformerException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual XObject getGlobalVariable(XPathContext xctxt, int index)
	  {

		XObject val = _stackFrames[index];

		// Lazy execution of variables.
		if (val.Type == XObject.CLASS_UNRESOLVEDVARIABLE)
		{
		  return (_stackFrames[index] = val.execute(xctxt));
		}

		return val;
	  }

	  /// <summary>
	  /// Get a global variable or parameter from the global stack frame.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath context, which must be passed in order to
	  /// lazy evaluate variables.
	  /// </param>
	  /// <param name="index"> Global variable index relative to the global stack
	  /// frame bottom.
	  /// </param>
	  /// <returns> The value of the variable.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getGlobalVariable(XPathContext xctxt, final int index, boolean destructiveOK) throws javax.xml.transform.TransformerException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual XObject getGlobalVariable(XPathContext xctxt, int index, bool destructiveOK)
	  {

		XObject val = _stackFrames[index];

		// Lazy execution of variables.
		if (val.Type == XObject.CLASS_UNRESOLVEDVARIABLE)
		{
		  return (_stackFrames[index] = val.execute(xctxt));
		}

		return destructiveOK ? val : val.Fresh;
	  }

	  /// <summary>
	  /// Get a variable based on it's qualified name.
	  /// This is for external use only.
	  /// </summary>
	  /// <param name="xctxt"> The XPath context, which must be passed in order to
	  /// lazy evaluate variables.
	  /// </param>
	  /// <param name="qname"> The qualified name of the variable.
	  /// </param>
	  /// <returns> The evaluated value of the variable.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getVariableOrParam(XPathContext xctxt, org.apache.xml.utils.QName qname) throws javax.xml.transform.TransformerException
	  public virtual XObject getVariableOrParam(XPathContext xctxt, org.apache.xml.utils.QName qname)
	  {

		org.apache.xml.utils.PrefixResolver prefixResolver = xctxt.NamespaceContext;

		// Get the current ElemTemplateElement, which must be pushed in as the 
		// prefix resolver, and then walk backwards in document order, searching 
		// for an xsl:param element or xsl:variable element that matches our 
		// qname.  If we reach the top level, use the StylesheetRoot's composed
		// list of top level variables and parameters.

		if (prefixResolver is org.apache.xalan.templates.ElemTemplateElement)
		{

		  org.apache.xalan.templates.ElemVariable vvar;

		  org.apache.xalan.templates.ElemTemplateElement prev = (org.apache.xalan.templates.ElemTemplateElement) prefixResolver;

		  if (!(prev is org.apache.xalan.templates.Stylesheet))
		  {
			while (!(prev.ParentNode is org.apache.xalan.templates.Stylesheet))
			{
			  org.apache.xalan.templates.ElemTemplateElement savedprev = prev;

			  while (null != (prev = prev.PreviousSiblingElem))
			  {
				if (prev is org.apache.xalan.templates.ElemVariable)
				{
				  vvar = (org.apache.xalan.templates.ElemVariable) prev;

				  if (vvar.Name.Equals(qname))
				  {
					return getLocalVariable(xctxt, vvar.Index);
				  }
				}
			  }
			  prev = savedprev.ParentElem;
			}
		  }

		  vvar = prev.StylesheetRoot.getVariableOrParamComposed(qname);
		  if (null != vvar)
		  {
			return getGlobalVariable(xctxt, vvar.Index);
		  }
		}

		throw new TransformerException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_VAR_NOT_RESOLVABLE, new object[]{qname.ToString()})); //"Variable not resolvable: " + qname);
	  }
	} // end VariableStack


}