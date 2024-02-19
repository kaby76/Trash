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
///     Mukul Gandhi - bug274719 - implementation of equality of xs:anyURI values
///     David Carver (STAR) - bug 282223 - fixed casting to xs:anyURI only string,
///         untypedAtomic, and anyURI are allowed.
///     David Carver (STAR) - bug 283777 - implemented gt, lt comparison code.
///     Jesper Steen Moller - bug 281159 - added promotion of xs:anyURI to string (reverse case) 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using CmpGt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpGt;
	using CmpLt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpLt;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// Represents a Universal Resource Identifier (URI) reference
	/// </summary>
	public class XSAnyURI : CtrType, CmpEq, CmpGt, CmpLt
	{

		private const string XS_ANY_URI = "xs:anyURI";
		private string _value;

		/// <summary>
		/// Arity 1 Constructor
		/// </summary>
		/// <param name="x">
		///            String representation of the URI </param>
		public XSAnyURI(string x)
		{
			_value = x;
		}

		/// <summary>
		/// Arity 0 Constructor. Initiates URI to null.
		/// </summary>
		public XSAnyURI() : this(null)
		{
		}

		/// <summary>
		/// Retrieve full type pathname of this datatype
		/// </summary>
		/// <returns> "xs:anyURI", the full type pathname of this datatype </returns>
		public override string string_type()
		{
			return XS_ANY_URI;
		}

		/// <summary>
		/// Retrieve type name of this datatype
		/// </summary>
		/// <returns> "anyURI", the type name of this datatype </returns>
		public override string type_name()
		{
			return "anyURI";
		}

		/// <summary>
		/// Transforms and retrieves the URI value of this URI datatype in String
		/// format
		/// </summary>
		/// <returns> the URI value held by this instance of the URI datatype as a
		///         String </returns>
		public override string string_value()
		{
			return _value;
		}

		/// <summary>
		/// Creation of a result sequence consisting of a URI from a previous result
		/// sequence.
		/// </summary>
		/// <param name="arg">
		///            previous result sequence </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> new result sequence consisting of the URI supplied </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			AnyType aat = (AnyType) arg.first();

			if (!(aat.string_type().Equals("xs:string") || aat.string_type().Equals(XS_ANY_URI) || aat.string_type().Equals("xs:untypedAtomic")))
			{
				throw DynamicError.invalidType();
			}

			return new XSAnyURI(aat.string_value());
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation which
		/// must be of type xs:anyURI (or, by promotion of this, xs:string)
		/// </summary>
		/// <param name="arg">
		///            The representation to compare with </param>
		/// <returns> True if the two representation are of the same String. False
		///         otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			if (arg is XSAnyURI || arg is XSString)
			{
				if (this.string_value().Equals(arg.string_value()))
				{
					return true;
				}
			}
			else
			{
				throw DynamicError.throw_type_error();
			}

			return false;
		}

		/// <summary>
		/// Greater than comparison between this and the supplied representation which
		/// must be of type xs:anyURI (or, by promotion of this, xs:string)
		/// @since 1.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool gt(AnyType arg, DynamicContext context)
		{
			if (!(arg is XSAnyURI || arg is XSString))
			{
				throw DynamicError.throw_type_error();
			}

			string anyURI = this.string_value();
			string compareToURI = arg.string_value();
			if (anyURI.CompareTo(compareToURI) > 0)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Less than comparison between this and the supplied representation which
		/// must be of type xs:anyURI (or, by promotion of this, xs:string)
		/// 
		/// @since 1.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool lt(AnyType arg, DynamicContext context)
		{
			if (!(arg is XSAnyURI || arg is XSString))
			{
				throw DynamicError.throw_type_error();
			}

			string anyURI = this.string_value();
			string compareToURI = arg.string_value();
			if (anyURI.CompareTo(compareToURI) < 0)
			{
				return true;
			}

			return false;
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_ANYURI;
			}
		}

		public override string StringValue
		{
			get
			{
				return _value;
			}
		}
	}

}