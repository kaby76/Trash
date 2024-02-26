using System;
using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2012 Jesper Steen Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller - initial API and implementation
///     Jesper Steen Moller - bug 281028 - avg/min/max/sum work
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///    Lukasz Wycisk - bug 361060 - Aggregations with nil=�true� throw exceptions.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.utils
{


	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;

	/// <summary>
	/// Generic type promoter for handling subtype substitution and type promotions for functions and operators.
	/// </summary>
	public abstract class TypePromoter
	{
		private Type targetType = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType doPromote(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType value) throws org.eclipse.wst.xml.xpath2.processor.DynamicError;
		public abstract AnyAtomicType doPromote(AnyAtomicType value);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType promote(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyType value) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public AnyAtomicType promote(AnyType value)
		{
			// This is a short cut, really
			if (value.GetType() == TargetType)
			{
				return (AnyAtomicType)value;
			}

			AnyAtomicType atomized = atomize(value);
			if (atomized == null)
			{ // empty sequence
				return null;
			}
			return doPromote(atomized);
		}

		/// <param name="typeToConsider"> The </param>
		/// <returns> The supertype to treat it as (i.e. if a xs:nonNegativeInteger is treated as xs:number) </returns>
		protected internal abstract Type substitute(Type typeToConsider);

		protected internal abstract bool checkCombination(Type newType);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void considerType(Class typeToConsider) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual void considerType(Type typeToConsider)
		{
			Type baseType = substitute(typeToConsider);

			if (baseType == null)
			{
				throw DynamicError.argument_type_error(typeToConsider);
			}

			if (targetType == null)
			{
				targetType = baseType;
			}
			else
			{
				if (!checkCombination(baseType))
				{
					throw DynamicError.argument_type_error(typeToConsider);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void considerTypes(java.util.Collection typesToConsider) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual void considerTypes(ICollection typesToConsider)
		{
			for (IEnumerator iter = typesToConsider.GetEnumerator(); iter.MoveNext();)
			{
				considerType((Type)iter.Current);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void considerSequence(org.eclipse.wst.xml.xpath2.api.ResultSequence sequenceToConsider) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual void considerSequence(ResultSequence sequenceToConsider)
		{
			for (int i = 0; i < sequenceToConsider.size(); ++i)
			{
				Item item = sequenceToConsider.item(i);
				considerValue(item);
			}
		}

		public virtual Type TargetType
		{
			get
			{
				return targetType;
			}
			set
			{
				this.targetType = value;
			}
		}


		public virtual AnyAtomicType atomize(Item at)
		{
			if (at is NodeType)
			{
				ResultSequence nodeValues = ((NodeType)at).typed_value();
				if (nodeValues.empty())
				{
					return null;
				}
				return (AnyAtomicType)nodeValues.first();
			}
			else
			{
				return (AnyAtomicType)at;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void considerValue(org.eclipse.wst.xml.xpath2.api.Item at) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual void considerValue(Item at)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType atomize = this.atomize(at);
			AnyAtomicType atomize = this.atomize(at);
			if (atomize != null)
			{ // we known that it is not empty sequence
				this.considerType(atomize.GetType());
			}
		}


	}
}