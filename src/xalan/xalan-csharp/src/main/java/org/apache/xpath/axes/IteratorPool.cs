using System;
using System.Collections;

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
 * $Id: IteratorPool.java 475981 2006-11-16 23:35:53Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using WrappedRuntimeException = org.apache.xml.utils.WrappedRuntimeException;

	/// <summary>
	/// Pool of object of a given type to pick from to help memory usage
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public sealed class IteratorPool
	{
		internal const long serialVersionUID = -460927331149566998L;

	  /// <summary>
	  /// Type of objects in this pool.
	  /// </summary>
	  private readonly DTMIterator m_orig;

	  /// <summary>
	  /// Stack of given objects this points to.
	  /// </summary>
	  private readonly ArrayList m_freeStack;

	  /// <summary>
	  /// Constructor IteratorPool
	  /// </summary>
	  /// <param name="original"> The original iterator from which all others will be cloned. </param>
	  public IteratorPool(DTMIterator original)
	  {
		m_orig = original;
		m_freeStack = new ArrayList();
	  }

	  /// <summary>
	  /// Get an instance of the given object in this pool 
	  /// </summary>
	  /// <returns> An instance of the given object </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized org.apache.xml.dtm.DTMIterator getInstanceOrThrow() throws CloneNotSupportedException
	  public DTMIterator InstanceOrThrow
	  {
		  get
		  {
			  lock (this)
			  {
				// Check if the pool is empty.
				if (m_freeStack.Count == 0)
				{
            
				  // Create a new object if so.
				  return (DTMIterator)m_orig.clone();
				}
				else
				{
				  // Remove object from end of free pool.
				  DTMIterator result = (DTMIterator)m_freeStack.Remove(m_freeStack.Count - 1);
				  return result;
				}
			  }
		  }
	  }

	  /// <summary>
	  /// Get an instance of the given object in this pool 
	  /// </summary>
	  /// <returns> An instance of the given object </returns>
	  public DTMIterator Instance
	  {
		  get
		  {
			  lock (this)
			  {
				// Check if the pool is empty.
				if (m_freeStack.Count == 0)
				{
            
				  // Create a new object if so.
				  try
				  {
					return (DTMIterator)m_orig.clone();
				  }
				  catch (Exception ex)
				  {
					throw new WrappedRuntimeException(ex);
				  }
				}
				else
				{
				  // Remove object from end of free pool.
				  DTMIterator result = (DTMIterator)m_freeStack.Remove(m_freeStack.Count - 1);
				  return result;
				}
			  }
		  }
	  }

	  /// <summary>
	  /// Add an instance of the given object to the pool  
	  /// 
	  /// </summary>
	  /// <param name="obj"> Object to add. </param>
	  public void freeInstance(DTMIterator obj)
	  {
		  lock (this)
		  {
			m_freeStack.Add(obj);
		  }
	  }
	}

}