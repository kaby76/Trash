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
 * $Id: QName.java 669372 2008-06-19 03:39:52Z zongaro $
 */

namespace org.apache.xalan.xsltc.compiler
{

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class QName
	{
		private readonly string _localname;
		private string _prefix;
		private string _namespace;
		private string _stringRep;
		private int _hashCode;

		public QName(string @namespace, string prefix, string localname)
		{
		_namespace = @namespace;
		_prefix = prefix;
		_localname = localname;

		_stringRep = (!string.ReferenceEquals(@namespace, null) && !@namespace.Equals(Constants_Fields.EMPTYSTRING)) ? (@namespace + ':' + localname) : localname;

		_hashCode = _stringRep.GetHashCode() + 19; // cached for speed
		}

		public void clearNamespace()
		{
		_namespace = Constants_Fields.EMPTYSTRING;
		}

		public override string ToString()
		{
		return _stringRep;
		}

		public string StringRep
		{
			get
			{
			return _stringRep;
			}
		}

		public override bool Equals(object other)
		{
		return (this == other) || (other is QName && _stringRep.Equals(((QName) other).StringRep));
		}

		public string LocalPart
		{
			get
			{
			return _localname;
			}
		}

		public string Namespace
		{
			get
			{
			return _namespace;
			}
		}

		public string Prefix
		{
			get
			{
			return _prefix;
			}
		}

		public override int GetHashCode()
		{
		return _hashCode;
		}

		public string dump()
		{
		return "QName: " + _namespace + "(" + _prefix + "):" + _localname;
		}
	}

}