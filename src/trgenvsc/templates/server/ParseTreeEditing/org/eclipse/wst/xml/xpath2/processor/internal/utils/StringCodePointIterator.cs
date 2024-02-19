/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Jesper Steen Moeller and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moeller - bug 282096 - initial API and implementation
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

using System;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.utils
{


	public sealed class StringCodePointIterator : CodePointIterator
	{
		private string text;
		private int end;
		// invariant: 0 <= pos <= end
		private int pos;
		private int cpPos;

		/// <summary>
		/// Constructs an iterator with an initial index of 0.
		/// </summary>
		public StringCodePointIterator(string text)
		{
			if (string.ReferenceEquals(text, null))
			{
				throw new System.NullReferenceException();
			}

			this.text = text;
			this.end = text.Length;
			//if (end > 0 && UCharacter.isHighSurrogate(text[end - 1]))
			//{
			//	throw new System.ArgumentException("Invalid UTF-16 sequence ending with a high surrogate");
			//}

			this.pos = 0;
			this.cpPos = 0;
		}

		/// <summary>
		/// Reset this iterator to point to a new string.  This package-visible
		/// method is used by other java.text classes that want to avoid allocating
		/// new StringCodePointIterator objects every time their setText method
		/// is called.
		/// </summary>
		/// <param name="text">   The String to be iterated over
		/// @since 1.2 </param>
		public string Text
		{
			set
			{
				if (string.ReferenceEquals(value, null))
				{
					throw new System.NullReferenceException();
				}
				this.text = value;
				this.end = value.Length;
				this.pos = 0;
				this.cpPos = 0;
			}
		}

		/// <summary>
		/// Implements CodePointIterator.first() for String. </summary>
		/// <seealso cref= CodePointIterator#first </seealso>
		public int first()
		{
			pos = 0;
			cpPos = 0;
			return current();
		}

		/// <summary>
		/// Implements CodePointIterator.last() for String. </summary>
		/// <seealso cref= CodePointIterator#last </seealso>
		public int last()
		{
			pos = end;
			cpPos = pos;
			return previous();
		}

		/// <summary>
		/// Implements CodePointIterator.current() for String. </summary>
		/// <seealso cref= CodePointIterator#current </seealso>
		public int current()
		{
			if (pos < end)
			{
				char ch1 = text[pos];
				//if (UCharacter.isHighSurrogate(ch1))
				//{
				//   return UCharacter.toCodePoint(ch1, text[pos + 1]);
				//}

				return ch1;
			}
			else
			{
				return CodePointIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Implements CodePointIterator.next() for String. </summary>
		/// <seealso cref= CodePointIterator#next </seealso>
		public int next()
		{
			if (pos < end - 1)
			{
				pos++;
				//if (UCharacter.isLowSurrogate(text[pos]))
				//{
				//	pos++;
				//}
				cpPos++;
				return current();
			}
			else
			{
				pos = end;
				return CodePointIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Implements CodePointIterator.previous() for String. </summary>
		/// <seealso cref= CodePointIterator#previous </seealso>
		public int previous()
		{
			if (pos > 0)
			{
				pos--;
				//if (UCharacter.isLowSurrogate(text[pos]))
				//{
				//	pos--;
				//}
				cpPos--;
				return current();
			}
			else
			{
				return CodePointIterator_Fields.DONE;
			}
		}

		/// <summary>
		/// Implements CodePointIterator.getIndex() for String. </summary>
		/// <seealso cref= CodePointIterator#getIndex </seealso>
		public int Index
		{
			get
			{
				return cpPos;
			}
		}

		/// <summary>
		/// Compares the equality of two StringCodePointIterator objects. </summary>
		/// <param name="obj"> the StringCodePointIterator object to be compared with. </param>
		/// <returns> true if the given obj is the same as this
		/// StringCodePointIterator object; false otherwise. </returns>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is StringCodePointIterator))
			{
				return false;
			}

			StringCodePointIterator that = (StringCodePointIterator) obj;

			if (GetHashCode() != that.GetHashCode())
			{
				return false;
			}
			if (!text.Equals(that.text))
			{
				return false;
			}
			if (pos != that.pos || end != that.end)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Computes a hashcode for this iterator. </summary>
		/// <returns> A hash code </returns>
		public override int GetHashCode()
		{
			return text.GetHashCode() ^ pos ^ end;
		}

        public object Clone()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
		/// Creates a copy of this iterator. </summary>
		/// <returns> A copy of this </returns>
		public object clone()
		{
			try
            {
                StringCodePointIterator other = (StringCodePointIterator) base.MemberwiseClone();
				return other;
			}
			catch
			{
				throw new Exception();
			}
		}

	}

}