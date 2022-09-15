using System;

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
 * $Id: StringComparable.java 468655 2006-10-28 07:12:06Z minchau $
 */

namespace org.apache.xml.utils
{



	/// <summary>
	/// International friendly string comparison with case-order
	/// @author Igor Hersht, igorh@ca.ibm.com
	/// </summary>
	public class StringComparable : IComparable
	{

		 public const int UNKNOWN_CASE = -1;
		 public const int UPPER_CASE = 1;
		 public const int LOWER_CASE = 2;

		 private string m_text;
		 private Locale m_locale;
		 private RuleBasedCollator m_collator;
		 private string m_caseOrder;
		 private int m_mask = unchecked((int)0xFFFFFFFF);

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public StringComparable(final String text, final java.util.Locale locale, final java.text.Collator collator, final String caseOrder)
		public StringComparable(string text, Locale locale, Collator collator, string caseOrder)
		{
			 m_text = text;
			 m_locale = locale;
			 m_collator = (RuleBasedCollator)collator;
			 m_caseOrder = caseOrder;
			 m_mask = getMask(m_collator.Strength);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public final static Comparable getComparator(final String text, final java.util.Locale locale, final java.text.Collator collator, final String caseOrder)
	   public static IComparable getComparator(string text, Locale locale, Collator collator, string caseOrder)
	   {
		   if ((string.ReferenceEquals(caseOrder, null)) || (caseOrder.Length == 0))
		   { // no case-order specified
				return ((RuleBasedCollator)collator).getCollationKey(text);
		   }
		   else
		   {
				return new StringComparable(text, locale, collator, caseOrder);
		   }
	   }

	   public sealed override string ToString()
	   {
		   return m_text;
	   }

	   public virtual int CompareTo(object o)
	   {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String pattern = ((StringComparable)o).toString();
	   string pattern = ((StringComparable)o).ToString();
	   if (m_text.Equals(pattern))
	   { //Code-point equals
		  return 0;
	   }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int savedStrength = m_collator.getStrength();
	   int savedStrength = m_collator.Strength;
	   int comp = 0;
		  // Is there difference more significant than case-order?     
		 if (((savedStrength == Collator.PRIMARY) || (savedStrength == Collator.SECONDARY)))
		 {
			 comp = m_collator.compare(m_text, pattern);
		 }
		 else
		 { // more than SECONDARY
			 m_collator.Strength = Collator.SECONDARY;
			 comp = m_collator.compare(m_text, pattern);
			 m_collator.Strength = savedStrength;
		 }
		 if (comp != 0)
		 { //Difference more significant than case-order
			return comp;
		 }

		  // No difference more significant than case-order.     
		  // Find case difference
		   comp = getCaseDiff(m_text, pattern);
		   if (comp != 0)
		   {
			   return comp;
		   }
		   else
		   { // No case differences. Less significant difference could exist
				return m_collator.compare(m_text, pattern);
		   }
	   }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private final int getCaseDiff(final String text, final String pattern)
	  private int getCaseDiff(string text, string pattern)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int savedStrength = m_collator.getStrength();
		 int savedStrength = m_collator.Strength;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int savedDecomposition = m_collator.getDecomposition();
		 int savedDecomposition = m_collator.Decomposition;
		 m_collator.Strength = Collator.TERTIARY; // not to ignore case
		 m_collator.Decomposition = Collator.CANONICAL_DECOMPOSITION; // corresponds NDF

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int diff[] =getFirstCaseDiff(text, pattern, m_locale);
		int[] diff = getFirstCaseDiff(text, pattern, m_locale);
		m_collator.Strength = savedStrength; // restore
		m_collator.Decomposition = savedDecomposition; //restore
		if (diff != null)
		{
		   if ((m_caseOrder).Equals("upper-first"))
		   {
				if (diff[0] == UPPER_CASE)
				{
					return -1;
				}
				else
				{
					return 1;
				}
		   }
		   else
		   { // lower-first
				if (diff[0] == LOWER_CASE)
				{
					return -1;
				}
				else
				{
					return 1;
				}
		   }
		}
	   else
	   { // No case differences
			return 0;
	   }

	  }



//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private final int[] getFirstCaseDiff(final String text, final String pattern, final java.util.Locale locale)
	  private int[] getFirstCaseDiff(string text, string pattern, Locale locale)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.CollationElementIterator targIter = m_collator.getCollationElementIterator(text);
			CollationElementIterator targIter = m_collator.getCollationElementIterator(text);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.CollationElementIterator patIter = m_collator.getCollationElementIterator(pattern);
			CollationElementIterator patIter = m_collator.getCollationElementIterator(pattern);
			int startTarg = -1;
			int endTarg = -1;
			int startPatt = -1;
			int endPatt = -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int done = getElement(java.text.CollationElementIterator.NULLORDER);
			int done = getElement(CollationElementIterator.NULLORDER);
			int patternElement = 0, targetElement = 0;
			bool getPattern = true, getTarget = true;

			while (true)
			{
				if (getPattern)
				{
					 startPatt = patIter.Offset;
					 patternElement = getElement(patIter.next());
					 endPatt = patIter.Offset;
				}
				if ((getTarget))
				{
					 startTarg = targIter.Offset;
					 targetElement = getElement(targIter.next());
					 endTarg = targIter.Offset;
				}
				getTarget = getPattern = true;
				if ((patternElement == done) || (targetElement == done))
				{
					return null;
				}
				else if (targetElement == 0)
				{
				  getPattern = false;
				}
				else if (patternElement == 0)
				{
				  getTarget = false;
				}
				else if (targetElement != patternElement)
				{ // mismatch
					if ((startPatt < endPatt) && (startTarg < endTarg))
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subText = text.substring(startTarg, endTarg - startTarg);
						string subText = text.Substring(startTarg, endTarg - startTarg);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subPatt = pattern.substring(startPatt, endPatt - startPatt);
						string subPatt = pattern.Substring(startPatt, endPatt - startPatt);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subTextUp = subText.toUpperCase(locale);
						string subTextUp = subText.ToUpper(locale);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String subPattUp = subPatt.toUpperCase(locale);
						string subPattUp = subPatt.ToUpper(locale);
						if (m_collator.compare(subTextUp, subPattUp) != 0)
						{ // not case diffference
							continue;
						}

						int[] diff = new int[] {UNKNOWN_CASE, UNKNOWN_CASE};
						if (m_collator.compare(subText, subTextUp) == 0)
						{
							diff[0] = UPPER_CASE;
						}
						else if (m_collator.compare(subText, subText.ToLower(locale)) == 0)
						{
						   diff[0] = LOWER_CASE;
						}
						if (m_collator.compare(subPatt, subPattUp) == 0)
						{
							diff[1] = UPPER_CASE;
						}
						else if (m_collator.compare(subPatt, subPatt.ToLower(locale)) == 0)
						{
						   diff[1] = LOWER_CASE;
						}

						if (((diff[0] == UPPER_CASE) && (diff[1] == LOWER_CASE)) || ((diff[0] == LOWER_CASE) && (diff[1] == UPPER_CASE)))
						{
							return diff;
						}
						else
						{ // not case diff
						  continue;
						}
					}
					else
					{
						continue;
					}

				}
			}

	  }


	 // Return a mask for the part of the order we're interested in
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static final int getMask(final int strength)
		private static int getMask(int strength)
		{
			switch (strength)
			{
				case Collator.PRIMARY:
					return unchecked((int)0xFFFF0000);
				case Collator.SECONDARY:
					return unchecked((int)0xFFFFFF00);
				default:
					return unchecked((int)0xFFFFFFFF);
			}
		}
		//get collation element with given strength
		// from the element with max strength
	  private int getElement(int maxStrengthElement)
	  {

		return (maxStrengthElement & m_mask);
	  }

	} //StringComparable



}