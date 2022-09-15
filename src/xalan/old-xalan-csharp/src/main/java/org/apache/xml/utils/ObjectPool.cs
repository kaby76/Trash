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
 * $Id: ObjectPool.java 475981 2006-11-16 23:35:53Z minchau $
 */
namespace org.apache.xml.utils
{

	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;


	/// <summary>
	/// Pool of object of a given type to pick from to help memory usage
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class ObjectPool
	{
		internal const long serialVersionUID = -8519013691660936643L;

	  /// <summary>
	  /// Type of objects in this pool.
	  ///  @serial          
	  /// </summary>
	  private readonly Type objectType;

	  /// <summary>
	  /// Stack of given objects this points to.
	  ///  @serial          
	  /// </summary>
	  private readonly ArrayList freeStack;

	  /// <summary>
	  /// Constructor ObjectPool
	  /// </summary>
	  /// <param name="type"> Type of objects for this pool </param>
	  public ObjectPool(Type type)
	  {
		objectType = type;
		freeStack = new ArrayList();
	  }

	  /// <summary>
	  /// Constructor ObjectPool
	  /// </summary>
	  /// <param name="className"> Fully qualified name of the type of objects for this pool. </param>
	  public ObjectPool(string className)
	  {
		try
		{
		  objectType = ObjectFactory.findProviderClass(className, ObjectFactory.findClassLoader(), true);
		}
		catch (ClassNotFoundException cnfe)
		{
		  throw new WrappedRuntimeException(cnfe);
		}
		freeStack = new ArrayList();
	  }


	  /// <summary>
	  /// Constructor ObjectPool
	  /// 
	  /// </summary>
	  /// <param name="type"> Type of objects for this pool </param>
	  /// <param name="size"> Size of vector to allocate </param>
	  public ObjectPool(Type type, int size)
	  {
		objectType = type;
		freeStack = new ArrayList(size);
	  }

	  /// <summary>
	  /// Constructor ObjectPool
	  /// 
	  /// </summary>
	  public ObjectPool()
	  {
		objectType = null;
		freeStack = new ArrayList();
	  }

	  /// <summary>
	  /// Get an instance of the given object in this pool if available
	  /// 
	  /// </summary>
	  /// <returns> an instance of the given object if available or null </returns>
	  public virtual object InstanceIfFree
	  {
		  get
		  {
			  lock (this)
			  {
            
				// Check if the pool is empty.
				if (freeStack.Count > 0)
				{
            
				  // Remove object from end of free pool.
				  object result = freeStack.Remove(freeStack.Count - 1);
				  return result;
				}
            
				return null;
			  }
		  }
	  }

	  /// <summary>
	  /// Get an instance of the given object in this pool 
	  /// 
	  /// </summary>
	  /// <returns> An instance of the given object </returns>
	  public virtual object Instance
	  {
		  get
		  {
			  lock (this)
			  {
            
				// Check if the pool is empty.
				if (freeStack.Count == 0)
				{
            
				  // Create a new object if so.
				  try
				  {
					return objectType.newInstance();
				  }
				  catch (InstantiationException)
				  {
				  }
				  catch (IllegalAccessException)
				  {
				  }
            
				  // Throw unchecked exception for error in pool configuration.
				  throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_EXCEPTION_CREATING_POOL, null)); //"exception creating new instance for pool");
				}
				else
				{
            
				  // Remove object from end of free pool.
				  object result = freeStack.Remove(freeStack.Count - 1);
            
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
	  public virtual void freeInstance(object obj)
	  {
		  lock (this)
		  {
        
			// Make sure the object is of the correct type.
			// Remove safety.  -sb
			// if (objectType.isInstance(obj))
			// {
			freeStack.Add(obj);
			// }
			// else
			// {
			//  throw new IllegalArgumentException("argument type invalid for pool");
			// }
		  }
	  }
	}

}