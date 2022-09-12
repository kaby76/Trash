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
 * $Id: CustomStringPool.java 475904 2006-11-16 20:09:39Z minchau $
 */

namespace org.apache.xml.dtm.@ref
{

	/// <summary>
	/// <para>CustomStringPool is an example of appliction provided data structure
	/// for a DTM implementation to hold symbol references, e.g. elelment names.
	/// It will follow the DTMDStringPool interface and use two simple methods
	/// indexToString(int i) and stringToIndex(Sring s) to map between a set of
	/// string values and a set of integer index values.  Therefore, an application
	/// may improve DTM processing speed by substituting the DTM symbol resolution
	/// tables with application specific quick symbol resolution tables.</para>
	/// 
	/// %REVIEW% The only difference between this an DTMStringPool seems to be that
	/// it uses a java.lang.Hashtable full of Integers rather than implementing its
	/// own hashing. Joe deliberately avoided that approach when writing
	/// DTMStringPool, since it is both much more memory-hungry and probably slower
	/// -- especially in JDK 1.1.x, where Hashtable is synchronized. We need to
	/// either justify this implementation or discard it.
	/// 
	/// %REVIEW% Xalan-J has dropped support for 1.1.x and we can now use
	/// the colletion classes in 1.2, such as java.util.HashMap which is
	/// similar to java.util.Hashtable but not synchronized. For performance reasons
	/// one could change m_stringToInt to be a HashMap, but is it OK to do that?
	/// Are such CustomStringPool objects already used in a thread-safe way?
	/// 
	/// <para>Status: In progress, under discussion.</para>
	/// 
	/// </summary>
	public class CustomStringPool : DTMStringPool
	{
			//final Vector m_intToString;
			//static final int HASHPRIME=101;
			//int[] m_hashStart=new int[HASHPRIME];
			internal readonly Hashtable m_stringToInt = new Hashtable(); // can this be a HashMap instead?
			public new const int NULL = -1;

			public CustomStringPool() : base()
			{
					/*m_intToString=new Vector();
					System.out.println("In constructor m_intToString is " + 
					                                                                         ((null == m_intToString) ? "null" : "not null"));*/
					//m_stringToInt=new Hashtable();
					//removeAllElements();
			}

			public override void removeAllElements()
			{
					m_intToString.Clear();
					if (m_stringToInt != null)
					{
							m_stringToInt.Clear();
					}
			}

			/// <returns> string whose value is uniquely identified by this integer index. </returns>
			/// <exception cref="java.lang.ArrayIndexOutOfBoundsException">
			///  if index doesn't map to a string.
			///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String indexToString(int i) throws java.lang.ArrayIndexOutOfBoundsException
			public override string indexToString(int i)
			{
					return (string) m_intToString[i];
			}

			/// <returns> integer index uniquely identifying the value of this string. </returns>
			public override int stringToIndex(string s)
			{
					if (string.ReferenceEquals(s, null))
					{
						return NULL;
					}
					int? iobj = (int?)m_stringToInt[s];
					if (iobj == null)
					{
							m_intToString.Add(s);
							iobj = new int?(m_intToString.Count);
							m_stringToInt[s] = iobj;
					}
					return iobj.Value;
			}
	}

}