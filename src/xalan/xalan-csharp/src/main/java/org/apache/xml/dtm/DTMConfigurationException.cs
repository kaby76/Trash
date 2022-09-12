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
 * $Id: DTMConfigurationException.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{

	/// <summary>
	/// Indicates a serious configuration error.
	/// </summary>
	public class DTMConfigurationException : DTMException
	{
		internal new const long serialVersionUID = -4607874078818418046L;

		/// <summary>
		/// Create a new <code>DTMConfigurationException</code> with no
		/// detail mesage.
		/// </summary>
		public DTMConfigurationException() : base("Configuration Error")
		{
		}

		/// <summary>
		/// Create a new <code>DTMConfigurationException</code> with
		/// the <code>String </code> specified as an error message.
		/// </summary>
		/// <param name="msg"> The error message for the exception. </param>
		public DTMConfigurationException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Create a new <code>DTMConfigurationException</code> with a
		/// given <code>Exception</code> base cause of the error.
		/// </summary>
		/// <param name="e"> The exception to be encapsulated in a
		/// DTMConfigurationException. </param>
		public DTMConfigurationException(Exception e) : base(e)
		{
		}

		/// <summary>
		/// Create a new <code>DTMConfigurationException</code> with the
		/// given <code>Exception</code> base cause and detail message.
		/// </summary>
		/// <param name="msg"> The detail message. </param>
		/// <param name="e"> The exception to be wrapped in a DTMConfigurationException </param>
		public DTMConfigurationException(string msg, Exception e) : base(msg, e)
		{
		}

		/// <summary>
		/// Create a new DTMConfigurationException from a message and a Locator.
		/// 
		/// <para>This constructor is especially useful when an application is
		/// creating its own exception from within a DocumentHandler
		/// callback.</para>
		/// </summary>
		/// <param name="message"> The error or warning message. </param>
		/// <param name="locator"> The locator object for the error or warning. </param>
		public DTMConfigurationException(string message, SourceLocator locator) : base(message, locator)
		{
		}

		/// <summary>
		/// Wrap an existing exception in a DTMConfigurationException.
		/// </summary>
		/// <param name="message"> The error or warning message, or null to
		///                use the message from the embedded exception. </param>
		/// <param name="locator"> The locator object for the error or warning. </param>
		/// <param name="e"> Any exception. </param>
		public DTMConfigurationException(string message, SourceLocator locator, Exception e) : base(message, locator, e)
		{
		}
	}

}