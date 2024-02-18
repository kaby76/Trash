/// <summary>
///*****************************************************************************
/// Copyright (c) 2011 Jesper Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Moller - initial API and implementation
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.api.typesystem
{

	// TODO: Review all of this

	/// <summary>
	/// @since 2.0
	/// </summary>
	public interface ComplexTypeDefinition : TypeDefinition
	{

		// Content Model Types
		/// <summary>
		/// Represents an empty content type. A content type with the distinguished 
		/// value empty validates elements with no character or element 
		/// information item children. 
		/// </summary>
		/// <summary>
		/// Represents a simple content type. A content type which is simple 
		/// validates elements with character-only children. 
		/// </summary>
		/// <summary>
		/// Represents an element-only content type. An element-only content type 
		/// validates elements with children that conform to the supplied content 
		/// model. 
		/// </summary>
		/// <summary>
		/// Represents a mixed content type.
		/// </summary>

		/// <summary>
		/// [derivation method]: either <code>DERIVATION_EXTENSION</code>, 
		/// <code>DERIVATION_RESTRICTION</code>, or <code>DERIVATION_NONE</code> 
		/// (see <code>XSConstants</code>). 
		/// </summary>
		short DerivationMethod {get;}

		/// <summary>
		/// [abstract]: a boolean. Complex types for which <code>abstract</code> is 
		/// true must not be used as the type definition for the validation of 
		/// element information items. 
		/// </summary>
		bool Abstract {get;}

		/// <summary>
		///  A set of attribute uses if it exists, otherwise an empty 
		/// <code>XSObjectList</code>. 
		/// </summary>
	//    public XSObjectList getAttributeUses();

		/// <summary>
		/// An attribute wildcard if it exists, otherwise <code>null</code>. 
		/// </summary>
	//    public XSWildcard getAttributeWildcard();

		/// <summary>
		/// [content type]: one of empty (<code>CONTENTTYPE_EMPTY</code>), a simple 
		/// type definition (<code>CONTENTTYPE_SIMPLE</code>), mixed (
		/// <code>CONTENTTYPE_MIXED</code>), or element-only (
		/// <code>CONTENTTYPE_ELEMENT</code>). 
		/// </summary>
		short ContentType {get;}

		/// <summary>
		/// A simple type definition corresponding to a simple content model, 
		/// otherwise <code>null</code>. 
		/// </summary>
		SimpleTypeDefinition SimpleType {get;}

		/// <summary>
		/// A particle for a mixed or element-only content model, otherwise 
		/// <code>null</code>. 
		/// </summary>
	//    public XSParticle getParticle();

		/// <summary>
		/// [prohibited substitutions]: a subset of {extension, restriction} </summary>
		/// <param name="restriction">  Extension or restriction constants (see 
		///   <code>XSConstants</code>). </param>
		/// <returns> True if <code>restriction</code> is a prohibited substitution, 
		///   otherwise false. </returns>
		bool isProhibitedSubstitution(short restriction);

		/// <summary>
		///  [prohibited substitutions]: A subset of {extension, restriction} or 
		/// <code>DERIVATION_NONE</code> represented as a bit flag (see 
		/// <code>XSConstants</code>). 
		/// </summary>
		short ProhibitedSubstitutions {get;}

		/// <summary>
		/// A sequence of [annotations] or an empty <code>XSObjectList</code>.
		/// </summary>
	//    public XSObjectList getAnnotations();



	}

	public static class ComplexTypeDefinition_Fields
	{
		public const short CONTENT_EMPTY = 0;
		public const short CONTENT_SIMPLE = 1;
		public const short CONTENT_ELEMENT = 2;
		public const short CONTENT_MIXED = 3;
	}

}