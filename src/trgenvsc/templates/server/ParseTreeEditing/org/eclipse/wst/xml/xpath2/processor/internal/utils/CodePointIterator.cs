using System;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009 Jesper Steen Moeller and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moeller - bug 282096 - initial API and implementation
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.utils
{

	public interface CodePointIterator : ICloneable
	{
		/// <summary>
		/// Sentinel value returned from iterator when the end is reached.
		/// The value is -1 which will never be a valid codepoint.
		/// </summary>

		/// <summary>
		/// Resets the position to 0 and returns the first code point. </summary>
		/// <returns> the first code point in the text, or DONE if the text is empty </returns>
		int first();

		/// <summary>
		/// Sets the position to the last possible position (or 0 if the text is empty)
		/// and returns the code point at that position. </summary>
		/// <returns> the last code point in the text, or DONE if the text is empty </returns>
		/// <seealso cref= #getEndIndex() </seealso>
		int last();

		/// <summary>
		/// Gets the code point at the current position (as returned by getIndex()). </summary>
		/// <returns> the code point at the current position or DONE if the current
		/// position is off the end of the text. </returns>
		/// <seealso cref= #getIndex() </seealso>
		int current();

		/// <summary>
		/// Increments the iterator's code point index by one and returns the code point
		/// at the new index.  If the resulting index is at the end of the string, the
		/// index is not incremented, and DONE is returned. </summary>
		/// <returns> the code point at the new position or DONE if the new
		/// position is after the text range. </returns>
		int next();

		/// <summary>
		/// Decrements the iterator's index by one and returns the character
		/// at the new index. If the current index is 0, the index
		/// remains at 0 and a value of DONE is returned.
		/// </summary>
		/// <returns> the code point at the new position (or DONE if the current
		/// position is 0) </returns>
		int previous();

		/// <summary>
		/// Returns the current index (as a codepoint, not a string index). </summary>
		/// <returns> the current index. </returns>
		int Index {get;}

		/// <summary>
		/// Create a copy of this code point iterator </summary>
		/// <returns> A copy of this </returns>
		object clone();

	}

	public static class CodePointIterator_Fields
	{
		public const int DONE = -1;
	}

}