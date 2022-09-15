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
 * $Id: DOMEnhancedForDTM.java 468648 2006-10-28 07:00:06Z minchau $
 */
namespace org.apache.xalan.xsltc
{
	/// <summary>
	/// Interface for SAXImpl which adds methods used at run-time, over and above
	/// those provided by the XSLTC DOM interface. An attempt to avoid the current
	/// "Is the DTM a DOM, if so is it a SAXImpl, . . .
	/// which was producing some ugly replicated code
	/// and introducing bugs where that multipathing had not been
	/// done.  This makes it easier to provide other DOM/DOMEnhancedForDTM
	/// implementations, rather than hard-wiring XSLTC to SAXImpl.
	/// 
	/// @author Joseph Kesselman
	/// 
	/// </summary>
	public interface DOMEnhancedForDTM : DOM
	{
		short[] getMapping(string[] names, string[] uris, int[] types);
		int[] getReverseMapping(string[] names, string[] uris, int[] types);
		short[] getNamespaceMapping(string[] namespaces);
		short[] getReverseNamespaceMapping(string[] namespaces);
		string DocumentURI {get;set;}
		int getExpandedTypeID2(int nodeHandle);
		bool hasDOMSource();
		int getElementById(string idString);
	}

}