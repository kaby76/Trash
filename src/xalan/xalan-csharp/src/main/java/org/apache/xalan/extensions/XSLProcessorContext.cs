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
 * $Id: XSLProcessorContext.java 468637 2006-10-28 06:51:02Z minchau $
 */
namespace org.apache.xalan.extensions
{

	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using ClonerToResultTree = org.apache.xalan.transformer.ClonerToResultTree;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using SerializerUtils = org.apache.xalan.serialize.SerializerUtils;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using QName = org.apache.xml.utils.QName;
	using XPathContext = org.apache.xpath.XPathContext;
	using DescendantIterator = org.apache.xpath.axes.DescendantIterator;
	using OneStepIterator = org.apache.xpath.axes.OneStepIterator;
	using XBoolean = org.apache.xpath.objects.XBoolean;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;
	using XRTreeFrag = org.apache.xpath.objects.XRTreeFrag;
	using XString = org.apache.xpath.objects.XString;
	using DocumentFragment = org.w3c.dom.DocumentFragment;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	// import org.apache.xalan.xslt.*;

	/// <summary>
	/// Provides transformer context to be passed to an extension element.
	/// 
	/// @author Sanjiva Weerawarana (sanjiva@watson.ibm.com)
	/// @xsl.usage general
	/// </summary>
	public class XSLProcessorContext
	{

	  /// <summary>
	  /// Create a processor context to be passed to an extension.
	  /// (Notice it is a package-only constructor).
	  /// </summary>
	  /// <param name="transformer"> non-null transformer instance </param>
	  /// <param name="stylesheetTree"> The owning stylesheet </param>
	  public XSLProcessorContext(TransformerImpl transformer, Stylesheet stylesheetTree)
	  {

		this.transformer = transformer;
		this.stylesheetTree = stylesheetTree;
		// %TBD%
		XPathContext xctxt = transformer.XPathContext;
		this.mode = transformer.Mode;
		this.sourceNode = xctxt.CurrentNode;
		this.sourceTree = xctxt.getDTM(this.sourceNode);
	  }

	  /// <summary>
	  /// An instance of a transformer </summary>
	  private TransformerImpl transformer;

	  /// <summary>
	  /// Get the transformer.
	  /// </summary>
	  /// <returns> the transformer instance for this context </returns>
	  public virtual TransformerImpl Transformer
	  {
		  get
		  {
			return transformer;
		  }
	  }

	  /// <summary>
	  /// The owning stylesheet for this context </summary>
	  private Stylesheet stylesheetTree;

	  /// <summary>
	  /// Get the Stylesheet being executed.
	  /// </summary>
	  /// <returns> the Stylesheet being executed. </returns>
	  public virtual Stylesheet Stylesheet
	  {
		  get
		  {
			return stylesheetTree;
		  }
	  }

	  /// <summary>
	  ///  The root of the source tree being executed. </summary>
	  private DTM sourceTree;

	  /// <summary>
	  /// Get the root of the source tree being executed.
	  /// </summary>
	  /// <returns> the root of the source tree being executed. </returns>
	  public virtual org.w3c.dom.Node SourceTree
	  {
		  get
		  {
			return sourceTree.getNode(sourceTree.getDocumentRoot(sourceNode));
		  }
	  }

	  /// <summary>
	  /// the current context node. </summary>
	  private int sourceNode;

	  /// <summary>
	  /// Get the current context node.
	  /// </summary>
	  /// <returns> the current context node. </returns>
	  public virtual org.w3c.dom.Node ContextNode
	  {
		  get
		  {
			return sourceTree.getNode(sourceNode);
		  }
	  }

	  /// <summary>
	  /// the current mode being executed. </summary>
	  private QName mode;

	  /// <summary>
	  /// Get the current mode being executed.
	  /// </summary>
	  /// <returns> the current mode being executed. </returns>
	  public virtual QName Mode
	  {
		  get
		  {
			return mode;
		  }
	  }

	  /// <summary>
	  /// Output an object to the result tree by doing the right conversions.
	  /// This is public for access by extensions.
	  /// 
	  /// </summary>
	  /// <param name="stylesheetTree"> The owning stylesheet </param>
	  /// <param name="obj"> the Java object to output. If its of an X<something> type
	  ///        then that conversion is done first and then sent out.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
	  /// <exception cref="java.io.FileNotFoundException"> </exception>
	  /// <exception cref="java.io.IOException"> </exception>
	  /// <exception cref="java.net.MalformedURLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void outputToResultTree(org.apache.xalan.templates.Stylesheet stylesheetTree, Object obj) throws TransformerException, java.net.MalformedURLException, java.io.FileNotFoundException, java.io.IOException
	  public virtual void outputToResultTree(Stylesheet stylesheetTree, object obj)
	  {

		try
		{
		  SerializationHandler rtreeHandler = transformer.ResultTreeHandler;
		  XPathContext xctxt = transformer.XPathContext;
		  XObject value;

		  // Make the return object into an XObject because it
		  // will be easier below.  One of the reasons to do this
		  // is to keep all the conversion functionality in the
		  // XObject classes.
		  if (obj is XObject)
		  {
			value = (XObject) obj;
		  }
		  else if (obj is string)
		  {
			value = new XString((string) obj);
		  }
		  else if (obj is Boolean)
		  {
			value = new XBoolean(((bool?) obj).Value);
		  }
		  else if (obj is Double)
		  {
			value = new XNumber(((double?) obj).Value);
		  }
		  else if (obj is DocumentFragment)
		  {
			int handle = xctxt.getDTMHandleFromNode((DocumentFragment)obj);

			value = new XRTreeFrag(handle, xctxt);
		  }
		  else if (obj is DTM)
		  {
			DTM dtm = (DTM)obj;
			DTMIterator iterator = new DescendantIterator();
			// %%ISSUE%% getDocument may not be valid for DTMs shared by multiple
			// document trees, eg RTFs. But in that case, we shouldn't be trying
			// to iterate over the whole DTM; we should be iterating over 
			// dtm.getDocumentRoot(rootNodeHandle), and folks should have told us
			// this by passing a more appropriate type.
			iterator.setRoot(dtm.Document, xctxt);
			value = new XNodeSet(iterator);
		  }
		  else if (obj is DTMAxisIterator)
		  {
			DTMAxisIterator iter = (DTMAxisIterator)obj;
			DTMIterator iterator = new OneStepIterator(iter, -1);
			value = new XNodeSet(iterator);
		  }
		  else if (obj is DTMIterator)
		  {
			value = new XNodeSet((DTMIterator) obj);
		  }
		  else if (obj is NodeIterator)
		  {
			value = new XNodeSet(new org.apache.xpath.NodeSetDTM(((NodeIterator)obj), xctxt));
		  }
		  else if (obj is org.w3c.dom.Node)
		  {
			value = new XNodeSet(xctxt.getDTMHandleFromNode((org.w3c.dom.Node) obj), xctxt.DTMManager);
		  }
		  else
		  {
			value = new XString(obj.ToString());
		  }

		  int type = value.Type;
		  string s;

		  switch (type)
		  {
		  case XObject.CLASS_BOOLEAN :
		  case XObject.CLASS_NUMBER :
		  case XObject.CLASS_STRING :
			s = value.str();

			rtreeHandler.characters(s.ToCharArray(), 0, s.Length);
			break;

		  case XObject.CLASS_NODESET : // System.out.println(value);
			DTMIterator nl = value.iter();

			int pos;

			while (DTM.NULL != (pos = nl.nextNode()))
			{
			  DTM dtm = nl.getDTM(pos);
			  int top = pos;

			  while (DTM.NULL != pos)
			  {
				rtreeHandler.flushPending();
				ClonerToResultTree.cloneToResultTree(pos, dtm.getNodeType(pos), dtm, rtreeHandler, true);

				int nextNode = dtm.getFirstChild(pos);

				while (DTM.NULL == nextNode)
				{
				  if (DTM.ELEMENT_NODE == dtm.getNodeType(pos))
				  {
					rtreeHandler.endElement("", "", dtm.getNodeName(pos));
				  }

				  if (top == pos)
				  {
					break;
				  }

				  nextNode = dtm.getNextSibling(pos);

				  if (DTM.NULL == nextNode)
				  {
					pos = dtm.getParent(pos);

					if (top == pos)
					{
					  if (DTM.ELEMENT_NODE == dtm.getNodeType(pos))
					  {
						rtreeHandler.endElement("", "", dtm.getNodeName(pos));
					  }

					  nextNode = DTM.NULL;

					  break;
					}
				  }
				}

				pos = nextNode;
			  }
			}
			break;
		  case XObject.CLASS_RTREEFRAG :
			SerializerUtils.outputResultTreeFragment(rtreeHandler, value, transformer.XPathContext);
	//        rtreeHandler.outputResultTreeFragment(value,
	//                                              transformer.getXPathContext());
			break;
		  }
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se);
		}
	  }

	  /// <summary>
	  /// I need a "Node transformNode (Node)" method somewhere that the
	  /// user can call to process the transformation of a node but not
	  /// serialize out automatically. ????????????????
	  /// 
	  /// Does ElemTemplateElement.executeChildTemplates() cut it? It sends
	  /// results out to the stream directly, so that could be a problem.
	  /// </summary>
	}

}