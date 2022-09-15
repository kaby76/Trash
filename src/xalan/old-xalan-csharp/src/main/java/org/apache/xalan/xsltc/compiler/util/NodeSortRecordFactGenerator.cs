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
 * $Id: NodeSortRecordFactGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// Generator for subclasses of NodeSortRecordFactory.
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class NodeSortRecordFactGenerator : ClassGenerator
	{

		public NodeSortRecordFactGenerator(string className, string superClassName, string fileName, int accessFlags, string[] interfaces, Stylesheet stylesheet) : base(className, superClassName, fileName, accessFlags, interfaces, stylesheet)
		{
		}

		/// <summary>
		/// Returns <tt>true</tt> since this class is external to the
		/// translet.
		/// </summary>
		public override bool External
		{
			get
			{
			return true;
			}
		}
	}

}