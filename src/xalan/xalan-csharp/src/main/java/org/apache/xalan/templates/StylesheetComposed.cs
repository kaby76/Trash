using System;
using System.Collections;

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
 * $Id: StylesheetComposed.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	/// <summary>
	/// Represents a stylesheet that has methods that resolve includes and
	/// imports.  It has methods on it that
	/// return "composed" properties, which mean that:
	/// <ol>
	/// <li>Properties that are aggregates, like OutputProperties, will
	/// be composed of properties declared in this stylsheet and all
	/// included stylesheets.</li>
	/// <li>Properties that aren't found, will be searched for first in
	/// the includes, and, if none are located, will be searched for in
	/// the imports.</li>
	/// <li>Properties in that are not atomic on a stylesheet will
	/// have the form getXXXComposed. Some properties, like version and id,
	/// are not inherited, and so won't have getXXXComposed methods.</li>
	/// </ol>
	/// <para>In some cases getXXXComposed methods may calculate the composed
	/// values dynamically, while in other cases they may store the composed
	/// values.</para>
	/// </summary>
	[Serializable]
	public class StylesheetComposed : Stylesheet
	{
		internal new const long serialVersionUID = -3444072247410233923L;

	  /// <summary>
	  /// Uses an XSL stylesheet document. </summary>
	  /// <param name="parent">  The including or importing stylesheet. </param>
	  public StylesheetComposed(Stylesheet parent) : base(parent)
	  {
	  }

	  /// <summary>
	  /// Tell if this can be cast to a StylesheetComposed, meaning, you
	  /// can ask questions from getXXXComposed functions.
	  /// </summary>
	  /// <returns> True since this is a StylesheetComposed  </returns>
	  public override bool AggregatedType
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Adds all recomposable values for this precedence level into the recomposableElements Vector
	  /// that was passed in as the first parameter.  All elements added to the
	  /// recomposableElements vector should extend ElemTemplateElement. </summary>
	  /// <param name="recomposableElements"> a Vector of ElemTemplateElement objects that we will add all of
	  ///        our recomposable objects to. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void recompose(java.util.Vector recomposableElements) throws javax.xml.transform.TransformerException
	  public virtual void recompose(ArrayList recomposableElements)
	  {

		//recomposeImports();         // Calculate the number of this import.
		//recomposeIncludes(this);    // Build the global include list for this stylesheet.

		// Now add in all of the recomposable elements at this precedence level

		int n = IncludeCountComposed;

		for (int i = -1; i < n; i++)
		{
		  Stylesheet included = getIncludeComposed(i);

		  // Add in the output elements

		  int s = included.OutputCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getOutput(j));
		  }

		  // Next, add in the attribute-set elements

		  s = included.AttributeSetCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getAttributeSet(j));
		  }

		  // Now the decimal-formats

		  s = included.DecimalFormatCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getDecimalFormat(j));
		  }

		  // Now the keys

		  s = included.KeyCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getKey(j));
		  }

		  // And the namespace aliases

		  s = included.NamespaceAliasCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getNamespaceAlias(j));
		  }

		  // Next comes the templates

		  s = included.TemplateCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getTemplate(j));
		  }

		  // Then, the variables

		  s = included.VariableOrParamCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getVariableOrParam(j));
		  }

		  // And lastly the whitespace preserving and stripping elements

		  s = included.StripSpaceCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getStripSpace(j));
		  }

		  s = included.PreserveSpaceCount;
		  for (int j = 0; j < s; j++)
		  {
			recomposableElements.Add(included.getPreserveSpace(j));
		  }
		}
	  }

	  /// <summary>
	  /// Order in import chain.
	  ///  @serial         
	  /// </summary>
	  private int m_importNumber = -1;

	  /// <summary>
	  /// The precedence of this stylesheet in the global import list.
	  ///  The lowest precedence stylesheet is 0.  A higher
	  ///  number has a higher precedence.
	  ///  @serial
	  /// </summary>
	  private int m_importCountComposed;

	  /* The count of imports composed for this stylesheet */
	  private int m_endImportCountComposed;

	  /// <summary>
	  /// Recalculate the precedence of this stylesheet in the global
	  /// import list.  The lowest precedence stylesheet is 0.  A higher
	  /// number has a higher precedence.
	  /// </summary>
	  internal virtual void recomposeImports()
	  {

		m_importNumber = StylesheetRoot.getImportNumber(this);

		StylesheetRoot root = StylesheetRoot;
		int globalImportCount = root.GlobalImportCount;

		m_importCountComposed = (globalImportCount - m_importNumber) - 1;

		// Now get the count of composed imports from this stylesheet's imports
		int count = ImportCount;
		if (count > 0)
		{
		  m_endImportCountComposed += count;
		  while (count > 0)
		  {
			m_endImportCountComposed += this.getImport(--count).EndImportCountComposed;
		  }
		}

		// Now get the count of composed imports from this stylesheet's
		// composed includes.
		count = IncludeCountComposed;
		while (count > 0)
		{
		  int imports = getIncludeComposed(--count).ImportCount;
		  m_endImportCountComposed += imports;
		  while (imports > 0)
		  {
			m_endImportCountComposed += getIncludeComposed(count).getImport(--imports).EndImportCountComposed;
		  }

		}
	  }

	  /// <summary>
	  /// Get a stylesheet from the "import" list. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of stylesheet in import list 
	  /// </param>
	  /// <returns> The stylesheet at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public StylesheetComposed getImportComposed(int i) throws ArrayIndexOutOfBoundsException
	  public virtual StylesheetComposed getImportComposed(int i)
	  {

		StylesheetRoot root = StylesheetRoot;

		// Get the stylesheet that is offset past this stylesheet.
		// Thus, if the index of this stylesheet is 3, an argument 
		// to getImportComposed of 0 will return the 4th stylesheet 
		// in the global import list.
		return root.getGlobalImport(1 + m_importNumber + i);
	  }

	  /// <summary>
	  /// Get the precedence of this stylesheet in the global import list.
	  /// The lowest precedence is 0.  A higher number has a higher precedence. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	  ////>
	  /// <returns> the precedence of this stylesheet in the global import list. </returns>
	  public virtual int ImportCountComposed
	  {
		  get
		  {
			return m_importCountComposed;
		  }
	  }

	  /// <summary>
	  /// Get the number of import in this stylesheet's composed list.
	  /// </summary>
	  /// <returns> the number of imports in this stylesheet's composed list. </returns>
	  public virtual int EndImportCountComposed
	  {
		  get
		  {
			return m_endImportCountComposed;
		  }
	  }


	  /// <summary>
	  /// The combined list of includes.
	  /// @serial
	  /// </summary>
	  [NonSerialized]
	  private ArrayList m_includesComposed;

	  /// <summary>
	  /// Recompose the value of the composed include list.  Builds a composite
	  /// list of all stylesheets included by this stylesheet to any depth.
	  /// </summary>
	  /// <param name="including"> Stylesheet to recompose </param>
	  internal virtual void recomposeIncludes(Stylesheet including)
	  {

		int n = including.IncludeCount;

		if (n > 0)
		{
		  if (null == m_includesComposed)
		  {
			m_includesComposed = new ArrayList();
		  }

		  for (int i = 0; i < n; i++)
		  {
			Stylesheet included = including.getInclude(i);
			m_includesComposed.Add(included);
			recomposeIncludes(included);
		  }
		}
	  }

	  /// <summary>
	  /// Get an "xsl:include" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.include">include in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of stylesheet in "include" list 
	  /// </param>
	  /// <returns> The stylesheet at the given index in the "include" list 
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Stylesheet getIncludeComposed(int i) throws ArrayIndexOutOfBoundsException
	  public virtual Stylesheet getIncludeComposed(int i)
	  {

		if (-1 == i)
		{
		  return this;
		}

		if (null == m_includesComposed)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (Stylesheet) m_includesComposed[i];
	  }

	  /// <summary>
	  /// Get the number of included stylesheets. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of included stylesheets. </returns>
	  public virtual int IncludeCountComposed
	  {
		  get
		  {
			return (null != m_includesComposed) ? m_includesComposed.Count : 0;
		  }
	  }

	  /// <summary>
	  /// For compilation support, we need the option of overwriting
	  /// (rather than appending to) previous composition.
	  /// We could phase out the old API in favor of this one, but I'm
	  /// holding off until we've made up our minds about compilation.
	  /// ADDED 9/5/2000 to support compilation experiment.
	  /// NOTE: GLP 29-Nov-00 I've left this method in so that CompilingStylesheetHandler will compile.  However,
	  ///                     I'm not sure why it's needed or what it does and I've commented out the body.
	  /// </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"/>
	  /// <param name="flushFirst"> Flag indicating the option of overwriting
	  /// (rather than appending to) previous composition.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void recomposeTemplates(boolean flushFirst) throws javax.xml.transform.TransformerException
	  public virtual void recomposeTemplates(bool flushFirst)
	  {
	/// <summary>
	///*************************************  KEEP METHOD IN FOR COMPILATION
	///    if (flushFirst)
	///      m_templateList = new TemplateList(this);
	/// 
	///    recomposeTemplates();
	/// ****************************************
	/// </summary>
	  }
	}

}