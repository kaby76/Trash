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
 * $Id: DOMErrorHandlerImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{

	using DOMError = org.w3c.dom.DOMError;
	using DOMErrorHandler = org.w3c.dom.DOMErrorHandler;

	/// <summary>
	/// This is the default implementation of the ErrorHandler interface and is 
	/// used if one is not provided.  The default implementation simply reports
	/// DOMErrors to System.err.
	/// 
	/// @xsl.usage internal
	/// </summary>
	internal sealed class DOMErrorHandlerImpl : DOMErrorHandler
	{

		/// <summary>
		/// Default Constructor 
		/// </summary>
		internal DOMErrorHandlerImpl()
		{
		}

		/// <summary>
		/// Implementation of DOMErrorHandler.handleError that
		/// adds copy of error to list for later retrieval.
		/// 
		/// </summary>
		public bool handleError(DOMError error)
		{
			bool fail = true;
			string severity = null;
			if (error.Severity == DOMError.SEVERITY_WARNING)
			{
				fail = false;
				severity = "[Warning]";
			}
			else if (error.Severity == DOMError.SEVERITY_ERROR)
			{
				severity = "[Error]";
			}
			else if (error.Severity == DOMError.SEVERITY_FATAL_ERROR)
			{
				severity = "[Fatal Error]";
			}

			Console.Error.WriteLine(severity + ": " + error.Message + "\t");
			Console.Error.WriteLine("Type : " + error.Type + "\t" + "Related Data: " + error.RelatedData + "\t" + "Related Exception: " + error.RelatedException);

			return fail;
		}
	}


}