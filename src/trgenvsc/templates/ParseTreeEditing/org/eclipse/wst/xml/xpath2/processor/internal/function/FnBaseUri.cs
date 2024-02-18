using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0 
///     Mukul Gandhi - bug274725 - implementation of base-uri function
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Returns the value of the base-uri property for $arg as defined by the
	/// accessor function dm:base-uri() for that kind of node in Section 5.1 base-uri
	/// Accessor of the specification. If $arg is the empty sequence, the empty
	/// sequence is returned. Document, element and processing-instruction nodes have
	/// a base-uri property which may be empty. The base-uri property of all other
	/// node types is the empty sequence. The value of the base-uri property is
	/// returned if it exists and is not empty. Otherwise, if the node has a parent,
	/// the value of dm:base-uri() applied to its parent is returned, recursively. If
	/// the node does not have a parent, or if the recursive ascent up the ancestor
	/// chain encounters a node whose base-uri property is empty and it does not have
	/// a parent, the empty sequence is returned.
	/// </summary>
	public class FnBaseUri : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnBaseUri.
		/// </summary>
		public FnBaseUri() : base(new QName("base-uri"), 0, 1)
		{
		}

		/// <summary>
		/// Evaluate arguments.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of evaluation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return base_uri(args, ec);
		}

		/// <summary>
		/// Base-Uri operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="d_context">
		/// 			  Dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:base-uri operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence base_uri(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence base_uri(ICollection args, EvaluationContext ec)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			ResultSequence rs = null;

			if (cargs.Count == 0)
			{
			  // support for arity 0
			  // get base-uri from the context item.
			  Item contextItem = ec.ContextItem;
			  if (contextItem != null)
			  {
				rs = getBaseUri(contextItem);
			  }
			  else
			  {
				throw DynamicError.contextUndefined();
			  }
			}
			else if (cargs.Count == 1)
			{
			  // support for arity 1
              var i = cargs.GetEnumerator();
              i.MoveNext();
              ResultSequence arg1 = (ResultSequence) i.Current;
			  Item att = arg1 == null || arg1.empty() ? null : arg1.first();

			  rs = getBaseUri(att);
			}
			else
			{
			  // arity other than 0 or 1 is not allowed
			  throw DynamicError.throw_type_error();
			}

			return rs;
		}

		/*
		 * Helper function for base-uri support
		 */
		public static ResultSequence getBaseUri(Item att)
		{
			ResultBuffer rs = new ResultBuffer();
			XSAnyURI baseUri = null;
			  // depending on the node type, we get the base-uri for the node.
			  // if base-uri property in DOM is null, we set the base-uri as string "null". This
			  // avoids null pointer exception while comparing xs:anyURI values.

			  if (att is NodeType)
			  {
				  NodeType node = (NodeType) att;
				  Node domNode = node.node_value();
				  string buri = domNode.BaseURI;
				  if (!string.ReferenceEquals(buri, null))
				  {
					  baseUri = new XSAnyURI(buri);
				  }
				  else
				  {
					  baseUri = new XSAnyURI("null");
				  }
			  }

			  if (baseUri != null)
			  {
				rs.add(baseUri);
			  }

			  return rs.Sequence;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnBaseUri))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}