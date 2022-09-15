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
 * $Id: RTFIterator.java 468655 2006-10-28 07:12:06Z minchau $
 */

/// <summary>
/// This class implements an RTF Iterator. Currently exists for sole
/// purpose of enabling EXSLT object-type function to return "RTF".
/// 
/// @xsl.usage advanced
/// </summary>
namespace org.apache.xpath.axes
{
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;

	[Serializable]
	public class RTFIterator : NodeSetDTM
	{
		internal new const long serialVersionUID = 7658117366258528996L;

		/// <summary>
		/// Constructor for RTFIterator
		/// </summary>
		public RTFIterator(int root, DTMManager manager) : base(root, manager)
		{
		}
	}


}