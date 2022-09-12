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
 * $Id: SortSettings.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{


	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;

	/// <summary>
	/// Class for carrying settings that are to be used for a particular set
	/// of <code>xsl:sort</code> elements.
	/// </summary>
	internal sealed class SortSettings
	{
		/// <summary>
		/// A reference to the translet object for the transformation.
		/// </summary>
		private AbstractTranslet _translet;

		/// <summary>
		/// The sort order (ascending or descending) for each level of
		/// <code>xsl:sort</code>
		/// </summary>
		private int[] _sortOrders;

		/// <summary>
		/// The type of comparison (text or number) for each level of
		/// <code>xsl:sort</code>
		/// </summary>
		private int[] _types;

		/// <summary>
		/// The Locale for each level of <code>xsl:sort</code>, based on any lang
		/// attribute or the default Locale.
		/// </summary>
		private Locale[] _locales;

		/// <summary>
		/// The Collator object in effect for each level of <code>xsl:sort</code>
		/// </summary>
		private Collator[] _collators;

		/// <summary>
		/// Case ordering for each level of <code>xsl:sort</code>.
		/// </summary>
		private string[] _caseOrders;

		/// <summary>
		/// Create an instance of <code>SortSettings</code>. </summary>
		/// <param name="translet"> <seealso cref="org.apache.xalan.xsltc.runtime.AbstractTranslet"/>
		///                 object for the transformation </param>
		/// <param name="sortOrders"> an array specifying the sort order for each sort level </param>
		/// <param name="types"> an array specifying the type of comparison for each sort
		///              level (text or number) </param>
		/// <param name="locales"> an array specifying the Locale for each sort level </param>
		/// <param name="collators"> an array specifying the Collation in effect for each
		///                  sort level </param>
		/// <param name="caseOrders"> an array specifying whether upper-case, lower-case
		///                   or neither is to take precedence for each sort level.
		///                   The value of each element is equal to one of
		///                   <code>"upper-first", "lower-first", or ""</code>. </param>
		internal SortSettings(AbstractTranslet translet, int[] sortOrders, int[] types, Locale[] locales, Collator[] collators, string[] caseOrders)
		{
			_translet = translet;
			_sortOrders = sortOrders;
			_types = types;
			_locales = locales;
			_collators = collators;
			_caseOrders = caseOrders;
		}

		/// <returns> A reference to the translet object for the transformation. </returns>
		internal AbstractTranslet Translet
		{
			get
			{
				return _translet;
			}
		}

		/// <returns> An array containing the sort order (ascending or descending)
		///         for each level of <code>xsl:sort</code> </returns>
		internal int[] SortOrders
		{
			get
			{
				return _sortOrders;
			}
		}

		/// <returns> An array containing the type of comparison (text or number)
		///         to perform for each level of <code>xsl:sort</code> </returns>
		internal int[] Types
		{
			get
			{
				return _types;
			}
		}

		/// <returns> An array containing the Locale object in effect for each level
		///         of <code>xsl:sort</code> </returns>
		internal Locale[] Locales
		{
			get
			{
				return _locales;
			}
		}

		/// <returns> An array containing the Collator object in effect for each level
		///         of <code>xsl:sort</code> </returns>
		internal Collator[] Collators
		{
			get
			{
				return _collators;
			}
		}

		/// <returns> An array specifying the case ordering for each level of
		///         <code>xsl:sort</code>. </returns>
		internal string[] CaseOrders
		{
			get
			{
				return _caseOrders;
			}
		}
	}

}