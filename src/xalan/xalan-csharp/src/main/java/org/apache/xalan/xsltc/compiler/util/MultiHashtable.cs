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
 * $Id: MultiHashtable.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{


	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class MultiHashtable : Hashtable
	{
		internal const long serialVersionUID = -6151608290510033572L;
		public object put(object key, object value)
		{
		ArrayList vector = (ArrayList)this[key];
		if (vector == null)
		{
			base[key] = vector = new ArrayList();
		}
		vector.Add(value);
		return vector;
		}

		public object maps(object from, object to)
		{
		if (from == null)
		{
			return null;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector vector = (java.util.Vector) get(from);
		ArrayList vector = (ArrayList) this[from];
		if (vector != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = vector.size();
			int n = vector.Count;
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object item = vector.elementAt(i);
					object item = vector[i];
			if (item.Equals(to))
			{
				return item;
			}
			}
		}
		return null;
		}
	}

}