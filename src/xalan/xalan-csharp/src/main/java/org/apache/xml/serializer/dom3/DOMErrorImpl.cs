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
 * $Id: DOMErrorImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{
	using DOMError = org.w3c.dom.DOMError;
	using DOMLocator = org.w3c.dom.DOMLocator;

	/// <summary>
	/// Implementation of the DOM Level 3 DOMError interface.
	/// 
	/// <para>See also the <a href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407/core.html#ERROR-Interfaces-DOMError'>DOMError Interface definition from Document Object Model (DOM) Level 3 Core Specification</a>.
	/// 
	/// @xsl.usage internal 
	/// </para>
	/// </summary>

	internal sealed class DOMErrorImpl : DOMError
	{

		/// <summary>
		/// private data members </summary>

		// The DOMError Severity
		private short fSeverity = DOMError.SEVERITY_WARNING;

		// The Error message
		private string fMessage = null;

		//  A String indicating which related data is expected in relatedData. 
		private string fType;

		// The platform related exception
		private Exception fException = null;

		//  
		private object fRelatedData;

		// The location of the exception
		private DOMLocatorImpl fLocation = new DOMLocatorImpl();


		//
		// Constructors
		//

		/// <summary>
		/// Default constructor. 
		/// </summary>
		internal DOMErrorImpl()
		{
		}

		/// <param name="severity"> </param>
		/// <param name="message"> </param>
		/// <param name="type"> </param>
		internal DOMErrorImpl(short severity, string message, string type)
		{
			fSeverity = severity;
			fMessage = message;
			fType = type;
		}

		/// <param name="severity"> </param>
		/// <param name="message"> </param>
		/// <param name="type"> </param>
		/// <param name="exception"> </param>
		internal DOMErrorImpl(short severity, string message, string type, Exception exception)
		{
			fSeverity = severity;
			fMessage = message;
			fType = type;
			fException = exception;
		}

		/// <param name="severity"> </param>
		/// <param name="message"> </param>
		/// <param name="type"> </param>
		/// <param name="exception"> </param>
		/// <param name="relatedData"> </param>
		/// <param name="location"> </param>
		internal DOMErrorImpl(short severity, string message, string type, Exception exception, object relatedData, DOMLocatorImpl location)
		{
			fSeverity = severity;
			fMessage = message;
			fType = type;
			fException = exception;
			fRelatedData = relatedData;
			fLocation = location;
		}


		/// <summary>
		/// The severity of the error, either <code>SEVERITY_WARNING</code>, 
		/// <code>SEVERITY_ERROR</code>, or <code>SEVERITY_FATAL_ERROR</code>.
		/// </summary>
		/// <returns> A short containing the DOMError severity </returns>
		public short Severity
		{
			get
			{
				return fSeverity;
			}
		}

		/// <summary>
		/// The DOMError message string.
		/// </summary>
		/// <returns> String </returns>
		public string Message
		{
			get
			{
				return fMessage;
			}
		}

		/// <summary>
		/// The location of the DOMError.
		/// </summary>
		/// <returns> A DOMLocator object containing the DOMError location. </returns>
		public DOMLocator Location
		{
			get
			{
				return fLocation;
			}
		}

		/// <summary>
		/// The related platform dependent exception if any.
		/// </summary>
		/// <returns> A java.lang.Exception  </returns>
		public object RelatedException
		{
			get
			{
				return fException;
			}
		}

		/// <summary>
		/// Returns a String indicating which related data is expected in relatedData.
		/// </summary>
		/// <returns> A String </returns>
		public string Type
		{
			get
			{
				return fType;
			}
		}

		/// <summary>
		/// The related DOMError.type dependent data if any.
		/// </summary>
		/// <returns> java.lang.Object  </returns>
		public object RelatedData
		{
			get
			{
				return fRelatedData;
			}
		}

		public void reset()
		{
			fSeverity = DOMError.SEVERITY_WARNING;
			fException = null;
			fMessage = null;
			fType = null;
			fRelatedData = null;
			fLocation = null;
		}

	} // class DOMErrorImpl

}