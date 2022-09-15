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
 * $Id: StringBufferPool.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{
	/// <summary>
	/// This class pools string buffers, since they are reused so often.
	/// String buffers are good candidates for pooling, because of 
	/// their supporting character arrays.
	/// @xsl.usage internal
	/// </summary>
	public class StringBufferPool
	{

	  /// <summary>
	  /// The global pool of string buffers. </summary>
	  private static ObjectPool m_stringBufPool = new ObjectPool(typeof(org.apache.xml.utils.FastStringBuffer));

	  /// <summary>
	  /// Get the first free instance of a string buffer, or create one 
	  /// if there are no free instances.
	  /// </summary>
	  /// <returns> A string buffer ready for use. </returns>
	  public static FastStringBuffer get()
	  {
		  lock (typeof(StringBufferPool))
		  {
			return (FastStringBuffer) m_stringBufPool.Instance;
		  }
	  }

	  /// <summary>
	  /// Return a string buffer back to the pool.
	  /// </summary>
	  /// <param name="sb"> Must be a non-null reference to a string buffer. </param>
	  public static void free(FastStringBuffer sb)
	  {
		  lock (typeof(StringBufferPool))
		  {
			// Since this isn't synchronized, setLength must be 
			// done before the instance is freed.
			// Fix attributed to Peter Speck <speck@ruc.dk>.
			sb.Length = 0;
			m_stringBufPool.freeInstance(sb);
		  }
	  }
	}

}