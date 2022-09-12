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
 * $Id: WrappedRuntimeException.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer.utils
{

	/// <summary>
	/// This class is for throwing important checked exceptions
	/// over non-checked methods.  It should be used with care,
	/// and in limited circumstances.
	/// 
	/// This class is a copy of the one in org.apache.xml.utils. 
	/// It exists to cut the serializers dependancy on that package.
	/// 
	/// This class is not a public API, it is only public because it is
	/// used by org.apache.xml.serializer.
	/// @xsl.usage internal
	/// </summary>
	public sealed class WrappedRuntimeException : Exception
	{
		internal const long serialVersionUID = 7140414456714658073L;

	  /// <summary>
	  /// Primary checked exception.
	  ///  @serial          
	  /// </summary>
	  private Exception m_exception;

	  /// <summary>
	  /// Construct a WrappedRuntimeException from a
	  /// checked exception.
	  /// </summary>
	  /// <param name="e"> Primary checked exception </param>
	  public WrappedRuntimeException(Exception e) : base(e.Message)
	  {


		m_exception = e;
	  }

	  /// <summary>
	  /// Constructor WrappedRuntimeException
	  /// 
	  /// </summary>
	  /// <param name="msg"> Exception information. </param>
	  /// <param name="e"> Primary checked exception </param>
	  public WrappedRuntimeException(string msg, Exception e) : base(msg)
	  {


		m_exception = e;
	  }

	  /// <summary>
	  /// Get the checked exception that this runtime exception wraps.
	  /// </summary>
	  /// <returns> The primary checked exception </returns>
	  public Exception Exception
	  {
		  get
		  {
			return m_exception;
		  }
	  }
	}

}