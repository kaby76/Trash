using System;
using System.Diagnostics;
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
///     Jesper Steen Moeller - bug 285145 - don't silently allow empty sequences always
///     Jesper Steen Moeller - bug 297707 - Missing the empty-sequence() type
///     David Carver - bug 298267 - Correctly handle instof with elements.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using ItemType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ItemType;
	using KindTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.KindTest;
	using SequenceType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SequenceType;
	using ConstructorFL = org.eclipse.wst.xml.xpath2.processor.@internal.function.ConstructorFL;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.function.FunctionLibrary;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// represents a Sequence types used for matching expected arguments of functions
	/// </summary>
	public class SeqType
	{

		public const int OCC_NONE = 0;
		public const int OCC_STAR = 1;
		public const int OCC_PLUS = 2;
		public const int OCC_QMARK = 3;
		public const int OCC_EMPTY = 4;

		/// <summary>
		/// Path to w3.org XML Schema specification.
		/// </summary>
		public const string XML_SCHEMA_NS = "http://www.w3.org/2001/XMLSchema";

		private static readonly QName ANY_ATOMIC_TYPE = new QName("xs", "anyAtomicType", XML_SCHEMA_NS);

		[NonSerialized]
		private AnyType anytype = null;
		[NonSerialized]
		private int occ;
		[NonSerialized]
		private Type typeClass = null;
		[NonSerialized]
		private QName nodeName = null;
		[NonSerialized]
		private bool wild = false;

		/// <summary>
		/// sequence type
		/// </summary>
		/// <param name="t">
		///            is any type </param>
		/// <param name="occ">
		///            is an integer in the sequence. </param>
		public SeqType(AnyType t, int occ)
		{
			anytype = t;
			this.occ = occ;

			if (t != null)
			{
				typeClass = t.GetType();
			}
			else
			{
				typeClass = null;
			}
		}

		/// <param name="occ">
		///            is an integer in the sequence. </param>
		// XXX hack to represent AnyNode...
		public SeqType(int occ) : this((AnyType) null, occ)
		{

			typeClass = typeof(NodeType);
		}

		/// <param name="type_class">
		///            is a class which represents the expected type </param>
		/// <param name="occ">
		///            is an integer in the sequence. </param>
		public SeqType(Type type_class, int occ) : this((AnyType) null, occ)
		{

			this.typeClass = type_class;
		}

		/// <param name="st">
		///            is a sequence type. </param>
		/// <param name="sc">
		///            is a static context. </param>
		public SeqType(SequenceType st, StaticContext sc, ResultSequence rs)
		{

			occ = mapSequenceTypeOccurrence(st.occurrence());
			// figure out the item is
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.eclipse.wst.xml.xpath2.processor.internal.ast.ItemType item = st.item_type();
			ItemType item = st.item_type();
			KindTest ktest = null;
			switch (item.type())
			{
			case ItemType.ITEM:
				typeClass = typeof(AnyType);
				return;

				// XXX IMPLEMENT THIS
			case ItemType.QNAME:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType aat = make_atomic(sc, item.qname());
				AnyAtomicType aat = make_atomic(sc, item.qname());

				Debug.Assert(aat != null);
				anytype = aat;
				if (item.qname().Equals(ANY_ATOMIC_TYPE))
				{
					typeClass = typeof(AnyAtomicType);
				}
				else
				{
					typeClass = anytype.GetType();
				}
				return;

			case ItemType.KINDTEST:
				ktest = item.kind_test();
				break;

			}

			if (ktest == null)
			{
				return;
			}

			typeClass = ktest.XDMClassType;
			anytype = ktest.createTestType(rs, sc);
			nodeName = ktest.name();
			wild = ktest.Wild;
		}

		private AnyAtomicType make_atomic(StaticContext sc, QName qname)
		{
			string ns = qname.@namespace();

			var functionLibraries = sc.FunctionLibraries;
			if (!functionLibraries.ContainsKey(ns))
			{
				return null;
			}

			FunctionLibrary fl = (FunctionLibrary) functionLibraries[ns];

			if (!(fl is ConstructorFL))
			{
				return null;
			}

			ConstructorFL cfl = (ConstructorFL) fl;

			return cfl.atomic_type(qname);
		}

		public static int mapSequenceTypeOccurrence(int occurrence)
		{
			// convert occurrence
			switch (occurrence)
			{
			case SequenceType.EMPTY:
				return OCC_EMPTY;

			case SequenceType.NONE:
				return OCC_NONE;

			case SequenceType.QUESTION:
				return OCC_QMARK;

			case SequenceType.STAR:
				return OCC_STAR;

			case SequenceType.PLUS:
				return OCC_PLUS;

			default:
				Debug.Assert(false);
				return 0;
			}
		}

		/// <param name="t">
		///            is an any type. </param>
		public SeqType(AnyType t) : this(t, OCC_NONE)
		{
		}

		/// <returns> an integer. </returns>
		public virtual int occurence()
		{
			return occ;
		}

		/// <returns> a type. </returns>
		public virtual AnyType type()
		{
			return anytype;
		}

		/// <summary>
		/// matches args
		/// </summary>
		/// <param name="args">
		///            is a result sequence </param>
		/// <exception cref="a">
		///             dynamic error </exception>
		/// <returns> a result sequence </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence match(org.eclipse.wst.xml.xpath2.api.ResultSequence args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence match(ResultSequence args)
		{

			int occurrence = occurence();

			// Check for empty sequence first
			if (occurrence == OCC_EMPTY && !args.empty())
			{
				throw new DynamicError(TypeError.invalid_type(null));
			}

			int arg_count = 0;

			for (var i = args.iterator(); i.MoveNext();)
			{
				AnyType arg = (AnyType) i.Current;

				// make sure all args are the same type as expected type
				if (!(typeClass.IsInstanceOfType(arg)))
				{
					throw new DynamicError(TypeError.invalid_type(null));
				}

				if (anytype != null)
				{
					if ((nodeName != null || wild) && arg is NodeType)
					{
						NodeType nodeType = (NodeType) arg;
						Node node = nodeType.node_value();
						Node lnode = ((NodeType) anytype).node_value();
						if (lnode == null)
						{
							//throw new DynamicError(TypeError.invalid_type(null));
							continue;
						}
						if (!lnode.isEqualNode(node))
						{
							//throw new DynamicError(TypeError.invalid_type(null));
							continue;
						}
					}
				}

				arg_count++;

			}

			switch (occurrence)
			{
			case OCC_NONE:
				if (arg_count != 1)
				{
					throw new DynamicError(TypeError.invalid_type(null));
				}
				break;

			case OCC_PLUS:
				if (arg_count == 0)
				{
					throw new DynamicError(TypeError.invalid_type(null));
				}
				break;

			case OCC_STAR:
				break;

			case OCC_QMARK:
				if (arg_count > 1)
				{
					throw new DynamicError(TypeError.invalid_type(null));
				}
				break;

			default:
				Debug.Assert(false);

			break;
			}

			return args;
		}
	}

}