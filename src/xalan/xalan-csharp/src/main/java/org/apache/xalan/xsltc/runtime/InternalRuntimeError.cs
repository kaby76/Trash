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
 * $Id: InternalRuntimeError.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xalan.xsltc.runtime
{
	/// <summary>
	/// Class to express failed assertions and similar for the xsltc runtime.
	/// As java.lang.AssertionError was introduced in JDK 1.4 we can't use that yet.
	/// </summary>
	public class InternalRuntimeError : Exception
	{

		public InternalRuntimeError(string message) : base(message)
		{
		}

	}

}