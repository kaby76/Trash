using System;
using System.Collections;
using System.Text;

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
 * $Id: MethodType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class MethodType : Type
	{
		private readonly Type _resultType;
		private readonly ArrayList _argsType;

		public MethodType(Type resultType)
		{
		_argsType = null;
		_resultType = resultType;
		}

		public MethodType(Type resultType, Type arg1)
		{
		if (arg1 != Type.Void)
		{
			_argsType = new ArrayList();
			_argsType.Add(arg1);
		}
		else
		{
			_argsType = null;
		}
		_resultType = resultType;
		}

		public MethodType(Type resultType, Type arg1, Type arg2)
		{
		_argsType = new ArrayList(2);
		_argsType.Add(arg1);
		_argsType.Add(arg2);
		_resultType = resultType;
		}

		public MethodType(Type resultType, Type arg1, Type arg2, Type arg3)
		{
		_argsType = new ArrayList(3);
		_argsType.Add(arg1);
		_argsType.Add(arg2);
		_argsType.Add(arg3);
		_resultType = resultType;
		}

		public MethodType(Type resultType, ArrayList argsType)
		{
		_resultType = resultType;
		_argsType = argsType.Count > 0 ? argsType : null;
		}

		public override String ToString()
		{
		StringBuilder result = new StringBuilder("method{");
		if (_argsType != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _argsType.size();
			int count = _argsType.Count;
			for (int i = 0; i < count; i++)
			{
			result.Append(_argsType[i]);
			if (i != (count - 1))
			{
				result.Append(',');
			}
			}
		}
		else
		{
			result.Append("void");
		}
		result.Append('}');
		return result.ToString();
		}

		public override String toSignature()
		{
		return toSignature("");
		}

		/// <summary>
		/// Returns the signature of this method that results by adding
		/// <code>lastArgSig</code> to the end of the argument list.
		/// </summary>
		public String toSignature(string lastArgSig)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
		StringBuilder buffer = new StringBuilder();
		buffer.Append('(');
		if (_argsType != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _argsType.size();
			int n = _argsType.Count;
			for (int i = 0; i < n; i++)
			{
			buffer.Append(((Type)_argsType[i]).toSignature());
			}
		}
		return buffer.Append(lastArgSig).Append(')').Append(_resultType.toSignature()).ToString();
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return null; // should never be called
		}

		public override bool identicalTo(Type other)
		{
		bool result = false;
		if (other is MethodType)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MethodType temp = (MethodType) other;
			MethodType temp = (MethodType) other;
			if (_resultType.identicalTo(temp._resultType))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = argsCount();
			int len = argsCount();
			result = len == temp.argsCount();
			for (int i = 0; i < len && result; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Type arg1 = (Type)_argsType.elementAt(i);
				Type arg1 = (Type)_argsType[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Type arg2 = (Type)temp._argsType.elementAt(i);
				Type arg2 = (Type)temp._argsType[i];
				result = arg1.identicalTo(arg2);
			}
			}
		}
		return result;
		}

		public override int distanceTo(Type other)
		{
		int result = int.MaxValue;
		if (other is MethodType)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MethodType mtype = (MethodType) other;
			MethodType mtype = (MethodType) other;
			if (_argsType != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = _argsType.size();
			int len = _argsType.Count;
			if (len == mtype._argsType.Count)
			{
				result = 0;
				for (int i = 0; i < len; i++)
				{
				Type arg1 = (Type) _argsType[i];
				Type arg2 = (Type) mtype._argsType[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int temp = arg1.distanceTo(arg2);
				int temp = arg1.distanceTo(arg2);
				if (temp == int.MaxValue)
				{
					result = temp; // return MAX_VALUE
					break;
				}
				else
				{
					result += arg1.distanceTo(arg2);
				}
				}
			}
			}
			else if (mtype._argsType == null)
			{
			result = 0; // both methods have no args
			}
		}
		return result;
		}

		public Type resultType()
		{
		return _resultType;
		}

		public ArrayList argsType()
		{
		return _argsType;
		}

		public int argsCount()
		{
		return _argsType == null ? 0 : _argsType.Count;
		}
	}

}