using System;
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
///     David Carver (STAR) - bug 280972 - fix fn:lang implementation so it matches spec. 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     David Carver (STAR) - bug 262765 - correct invalidType to throw XPTY0004 instead of FORG0006
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using Attr = org.w3c.dom.Attr;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// This function tests whether the language of $node, or the context node if the
	/// second argument is omitted, as specified by xml:lang attributes is the same
	/// as, or is a sublanguage of, the language specified by $testlang. The language
	/// of the argument node, or the context node if the second argument is omitted,
	/// is determined by the value of the xml:lang attribute on the node, or, if the
	/// node has no such attribute, by the value of the xml:lang attribute on the
	/// nearest ancestor of the node that has an xml:lang attribute. If there is no
	/// such ancestor, then the function returns false.
	/// </summary>
	public class FnLang : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnLang.
		/// </summary>
		public FnLang() : base(new QName("lang"), 1, 2)
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
			return lang(args, ec);
		}

		/// <summary>
		/// Language operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:lang operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence lang(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence lang(ICollection args, EvaluationContext ec)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get arg
			IEnumerator citer = cargs.GetEnumerator();
            citer.MoveNext();
            ResultSequence arg1 = (ResultSequence) citer.Current;
			ResultSequence arg2 = null;
			if (cargs.Count == 1)
			{
				if (ec.ContextItem == null)
				{
					throw DynamicError.contextUndefined();
				}
				arg2 = (AnyType) ec.ContextItem;
			}
			else
            {
                citer.MoveNext();
                arg2 = (ResultSequence) citer.Current;
            }

			string lang = "";

			if (!(arg1 == null || arg1.empty()))
			{
				lang = ((XSString) arg1.first()).value();
			}


			if (!(arg2.first() is NodeType))
			{
				throw DynamicError.invalidType();
			}

			NodeType an = (NodeType) arg2.first();

			return new XSBoolean(test_lang(an.node_value(), lang));
		}

		/// <summary>
		/// Language test operation.
		/// </summary>
		/// <param name="node">
		///            Node to test. </param>
		/// <param name="lang">
		///            Language to test for. </param>
		/// <returns> Boolean result of operation. </returns>
		private static bool test_lang(Node node, string lang)
		{
			NamedNodeMap attrs = node.Attributes;

			if (attrs != null)
			{
				for (int i = 0; i < attrs.Length; i++)
				{
					Attr attr = (Attr) attrs.item(i);

					if (!"xml:lang".Equals(attr.Name))
					{
						continue;
					}

					string xmllangValue = attr.Value;
					int hyphenIndex = xmllangValue.IndexOf('-');

					if (hyphenIndex > -1)
					{
						xmllangValue = xmllangValue.Substring(0, hyphenIndex);
					}


					string langLanguage = lang;
					if (lang.Length > 2)
					{
						langLanguage = lang.Substring(0, 2);
					}

					return xmllangValue.Equals(langLanguage, StringComparison.CurrentCultureIgnoreCase);
				}
			}

			Node parent = node.ParentNode;
			if (parent == null)
			{
				return false;
			}

			return test_lang(parent, lang);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnLang))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
        
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_QMARK));
					_expected_args.Add(new SeqType(SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}

	}

}