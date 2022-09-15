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
 * $Id: ExtensionEvent.java 468644 2006-10-28 06:56:42Z minchau $
 */

namespace org.apache.xalan.trace
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;

	/// <summary>
	/// An event representing an extension call.
	/// </summary>
	public class ExtensionEvent
	{

		public const int DEFAULT_CONSTRUCTOR = 0;
		public const int METHOD = 1;
		public const int CONSTRUCTOR = 2;

		public readonly int m_callType;
		public readonly TransformerImpl m_transformer;
		public readonly object m_method;
		public readonly object m_instance;
		public readonly object[] m_arguments;


		public ExtensionEvent(TransformerImpl transformer, System.Reflection.MethodInfo method, object instance, object[] arguments)
		{
			m_transformer = transformer;
			m_method = method;
			m_instance = instance;
			m_arguments = arguments;
			m_callType = METHOD;
		}

		public ExtensionEvent(TransformerImpl transformer, System.Reflection.ConstructorInfo constructor, object[] arguments)
		{
			m_transformer = transformer;
			m_instance = null;
			m_arguments = arguments;
			m_method = constructor;
			m_callType = CONSTRUCTOR;
		}

		public ExtensionEvent(TransformerImpl transformer, Type clazz)
		{
			m_transformer = transformer;
			m_instance = null;
			m_arguments = null;
			m_method = clazz;
			m_callType = DEFAULT_CONSTRUCTOR;
		}

	}


}