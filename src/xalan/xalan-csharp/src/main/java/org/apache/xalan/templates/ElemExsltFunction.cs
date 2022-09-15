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
 * $Id: ElemExsltFunction.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using ExtensionNamespaceSupport = org.apache.xalan.extensions.ExtensionNamespaceSupport;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;


	/// <summary>
	/// Implement func:function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class ElemExsltFunction : ElemTemplate
	{
		internal new const long serialVersionUID = 272154954793534771L;
	  /// <summary>
	  /// Get an integer representation of the element type.
	  /// </summary>
	  /// <returns> An integer representation of the element, defined in the
	  ///     Constants class. </returns>
	  /// <seealso cref="org.apache.xalan.templates.Constants"/>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.EXSLT_ELEMNAME_FUNCTION;
		  }
	  }

	   /// <summary>
	   /// Return the node name, defined in the
	   ///     Constants class. </summary>
	   /// <seealso cref="org.apache.xalan.templates.Constants"/>
	   /// <returns> The node name
	   ///  </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.EXSLT_ELEMNAME_FUNCTION_STRING;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer, org.apache.xpath.objects.XObject[] args) throws javax.xml.transform.TransformerException
	  public virtual void execute(TransformerImpl transformer, XObject[] args)
	  {
		XPathContext xctxt = transformer.XPathContext;
		VariableStack vars = xctxt.VarStack;

		// Increment the frame bottom of the variable stack by the
		// frame size
		int thisFrame = vars.StackFrame;
		int nextFrame = vars.link(m_frameSize);

		if (m_inArgsSize < args.Length)
		{
		  throw new TransformerException("function called with too many args");
		}

		// Set parameters,
		// have to clear the section of the stack frame that has params.
		if (m_inArgsSize > 0)
		{
		  vars.clearLocalSlots(0, m_inArgsSize);

		  if (args.Length > 0)
		  {
			vars.StackFrame = thisFrame;
			NodeList children = this.ChildNodes;

			for (int i = 0; i < args.Length; i++)
			{
			  Node child = children.item(i);
			  if (children.item(i) is ElemParam)
			  {
				ElemParam param = (ElemParam)children.item(i);
				vars.setLocalVariable(param.Index, args[i], nextFrame);
			  }
			}

			vars.StackFrame = nextFrame;
		  }
		}

		//  Removed ElemTemplate 'push' and 'pop' of RTFContext, in order to avoid losing the RTF context 
		//  before a value can be returned. ElemExsltFunction operates in the scope of the template that called 
		//  the function.
		//  xctxt.pushRTFContext();

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		vars.StackFrame = nextFrame;
		transformer.executeChildTemplates(this, true);

		// Reset the stack frame after the function call
		vars.unlink(thisFrame);

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}

		// Following ElemTemplate 'pop' removed -- see above.
		// xctxt.popRTFContext(); 

	  }

	  /// <summary>
	  /// Called after everything else has been
	  /// recomposed, and allows the function to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		base.compose(sroot);

		// Register the function namespace (if not already registered).
		string @namespace = Name.getNamespace();
		string handlerClass = sroot.ExtensionHandlerClass;
		object[] args = new object[] {@namespace, sroot};
		ExtensionNamespaceSupport extNsSpt = new ExtensionNamespaceSupport(@namespace, handlerClass, args);
		sroot.ExtensionNamespacesManager.registerExtension(extNsSpt);
		// Make sure there is a handler for the EXSLT functions namespace
		// -- for isElementAvailable().    
		if (!(@namespace.Equals(Constants.S_EXSLT_FUNCTIONS_URL)))
		{
		  @namespace = Constants.S_EXSLT_FUNCTIONS_URL;
		  args = new object[]{@namespace, sroot};
		  extNsSpt = new ExtensionNamespaceSupport(@namespace, handlerClass, args);
		  sroot.ExtensionNamespacesManager.registerExtension(extNsSpt);
		}
	  }
	}

}