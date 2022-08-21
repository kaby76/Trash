using System.Collections;

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

	/// <summary>
	/// @since 2.0
	/// </summary>
	public interface SimpleTypeDefinition : TypeDefinition
	{
		/// <summary>
		/// The variety is absent for the anySimpleType definition.
		/// </summary>
		/// <summary>
		/// <code>Atomic</code> type.
		/// </summary>
		/// <summary>
		/// <code>List</code> type.
		/// </summary>
		/// <summary>
		/// <code>Union</code> type.
		/// </summary>

		/// <summary>
		/// [variety]: one of {atomic, list, union} or absent. 
		/// </summary>
		short Variety {get;}

		/// <summary>
		/// If variety is <code>atomic</code> the primitive type definition (a 
		/// built-in primitive datatype definition or the simple ur-type 
		/// definition) is available, otherwise <code>null</code>. 
		/// </summary>
		SimpleTypeDefinition PrimitiveType {get;}

		/// <summary>
		/// Returns the closest built-in type category this type represents or 
		/// derived from. For example, if this simple type is a built-in derived 
		/// type integer the <code>INTEGER_DV</code> is returned.
		/// 
		/// KILL!
		/// </summary>
		short BuiltInKind {get;}

		/// <summary>
		/// If variety is <code>list</code> the item type definition (an atomic or 
		/// union simple type definition) is available, otherwise 
		/// <code>null</code>. 
		/// </summary>
		TypeDefinition ItemType {get;}

		/// <summary>
		/// If variety is <code>union</code> the list of member type definitions (a 
		/// non-empty sequence of simple type definitions) is available, 
		/// otherwise an empty <code>XSObjectList</code>. 
		/// </summary>
		IList MemberTypes {get;}

		/// <summary>
		/// [facets]: all facets defined on this type. The value is a bit 
		/// combination of FACET_XXX constants of all defined facets. 
		/// </summary>
	//    public short getDefinedFacets();

		/// <summary>
		/// Convenience method. [Facets]: check whether a facet is defined on this 
		/// type. </summary>
		/// <param name="facetName">  The name of the facet. </param>
		/// <returns>  True if the facet is defined, false otherwise. </returns>
	//    public boolean isDefinedFacet(short facetName);

		/// <summary>
		/// [facets]: all defined facets for this type which are fixed.
		/// </summary>
	//    public short getFixedFacets();

		/// <summary>
		/// Convenience method. [Facets]: check whether a facet is defined and 
		/// fixed on this type. </summary>
		/// <param name="facetName">  The name of the facet. </param>
		/// <returns>  True if the facet is fixed, false otherwise. </returns>
	//    public boolean isFixedFacet(short facetName);

		/// <summary>
		/// Convenience method. Returns a value of a single constraining facet for 
		/// this simple type definition. This method must not be used to retrieve 
		/// values for <code>enumeration</code> and <code>pattern</code> facets. </summary>
		/// <param name="facetName"> The name of the facet, i.e. 
		///   <code>FACET_LENGTH, FACET_TOTALDIGITS</code>.
		///   To retrieve the value for a pattern or 
		///   an enumeration, see <code>enumeration</code> and 
		///   <code>pattern</code>. </param>
		/// <returns> A value of the facet specified in <code>facetName</code> for 
		///   this simple type definition or <code>null</code>.  </returns>
	//    public String getLexicalFacetValue(short facetName);

		/// <summary>
		/// A list of enumeration values if it exists, otherwise an empty 
		/// <code>StringList</code>. 
		/// </summary>
	//    public StringList getLexicalEnumeration();

		/// <summary>
		/// A list of pattern values if it exists, otherwise an empty 
		/// <code>StringList</code>. 
		/// </summary>
	//    public StringList getLexicalPattern();

		/// <summary>
		///  Fundamental Facet: ordered. 
		/// </summary>
		short Ordered {get;}

		/// <summary>
		/// Fundamental Facet: cardinality. 
		/// </summary>
		bool Finite {get;}

		/// <summary>
		/// Fundamental Facet: bounded. 
		/// </summary>
		bool Bounded {get;}

		/// <summary>
		/// Fundamental Facet: numeric. 
		/// </summary>
		bool Numeric {get;}

		/// <summary>
		///  A list of constraining facets if it exists, otherwise an empty 
		/// <code>XSObjectList</code>. Note: This method must not be used to 
		/// retrieve values for <code>enumeration</code> and <code>pattern</code> 
		/// facets. 
		/// </summary>
	//    public XSObjectList getFacets();

		/// <summary>
		///  A list of enumeration and pattern constraining facets if it exists, 
		/// otherwise an empty <code>XSObjectList</code>. 
		/// </summary>
	//    public XSObjectList getMultiValueFacets();

		/// <summary>
		/// A sequence of [annotations] or an empty <code>XSObjectList</code>.
		/// </summary>
	//    public XSObjectList getAnnotations();


	}

	public static class SimpleTypeDefinition_Fields
	{
		public const short VARIETY_ABSENT = 0;
		public const short VARIETY_ATOMIC = 1;
		public const short VARIETY_LIST = 2;
		public const short VARIETY_UNION = 3;
	}

}