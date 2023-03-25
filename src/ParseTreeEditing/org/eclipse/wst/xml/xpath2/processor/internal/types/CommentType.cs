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
///     Jesper Moller - bug 275610 - Avoid big time and memory overhead for externals
///     David Carver  - bug 281186 - implementation of fn:id and fn:idref
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using Comment = org.w3c.dom.Comment;

	/// <summary>
	/// A representation of the CommentType datatype
	/// </summary>
	public class CommentType : NodeType
	{
		private const string COMMENT = "comment";
		private Comment _value;

		/// <summary>
		/// Initialise according to the supplied parameters
		/// </summary>
		/// <param name="v">
		///            The comment being represented </param>
		public CommentType(Comment v, TypeModel tm) : base(v, tm)
		{
			_value = v;
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "comment" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return COMMENT;
		}

		/// <summary>
		/// Retrieves a String representation of the comment being stored
		/// </summary>
		/// <returns> String representation of the comment being stored </returns>
		public override string StringValue
		{
			get
			{
				return (string)_value.NodeValue;
			}
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the comment stored
		/// </summary>
		/// <returns> New ResultSequence consisting of the comment stored </returns>
		public override ResultSequence typed_value()
		{
			return new XSString(_value.Data);
		}

		/// <summary>
		/// Unsupported method for this node.
		/// </summary>
		/// <returns> null </returns>
		public override QName node_name()
		{
			return null;
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public override bool ID
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public override bool IDREF
		{
			get
			{
				return false;
			}
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNTYPEDATOMIC;
			}
		}
	}

}