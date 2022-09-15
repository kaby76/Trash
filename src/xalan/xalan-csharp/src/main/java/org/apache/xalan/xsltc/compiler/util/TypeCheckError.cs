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
 * $Id: TypeCheckError.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using SyntaxTreeNode = org.apache.xalan.xsltc.compiler.SyntaxTreeNode;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class TypeCheckError : Exception
	{
		internal const long serialVersionUID = 3246224233917854640L;
		internal ErrorMsg _error = null;
		internal SyntaxTreeNode _node = null;

		public TypeCheckError(SyntaxTreeNode node) : base()
		{
		_node = node;
		}

		public TypeCheckError(ErrorMsg error) : base()
		{
		_error = error;
		}

		public TypeCheckError(string code, object param) : base()
		{
		_error = new ErrorMsg(code, param);
		}

		public TypeCheckError(string code, object param1, object param2) : base()
		{
		_error = new ErrorMsg(code, param1, param2);
		}

		public virtual ErrorMsg ErrorMsg
		{
			get
			{
				return _error;
			}
		}

		public virtual string Message
		{
			get
			{
				return ToString();
			}
		}

		public override string ToString()
		{
		string result;

		if (_error == null)
		{
				if (_node != null)
				{
					_error = new ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.TYPE_CHECK_ERR, _node.ToString());
				}
			else
			{
				_error = new ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.TYPE_CHECK_UNK_LOC_ERR);
			}
		}

		return _error.ToString();
		}
	}

}