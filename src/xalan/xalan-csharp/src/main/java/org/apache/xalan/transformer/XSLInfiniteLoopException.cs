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
 * $Id: XSLInfiniteLoopException.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	/// <summary>
	/// Class used to create an Infinite Loop Exception 
	/// @xsl.usage internal
	/// </summary>
	internal class XSLInfiniteLoopException
	{

	  /// <summary>
	  /// Constructor XSLInfiniteLoopException
	  /// 
	  /// </summary>
	  internal XSLInfiniteLoopException() : base()
	  {
	  }

	  /// <summary>
	  /// Get Message associated with the exception
	  /// 
	  /// </summary>
	  /// <returns> Message associated with the exception </returns>
	  public virtual string Message
	  {
		  get
		  {
			return "Processing Terminated.";
		  }
	  }
	}

}