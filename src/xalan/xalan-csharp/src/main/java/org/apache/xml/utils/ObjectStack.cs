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
 * $Id: ObjectStack.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// Implement a stack of simple integers.
	/// 
	/// %OPT%
	/// This is currently based on ObjectVector, which permits fast acess but pays a
	/// heavy recopying penalty if/when its size is increased. If we expect deep
	/// stacks, we should consider a version based on ChunkedObjectVector.
	/// @xsl.usage internal
	/// </summary>
	public class ObjectStack : ObjectVector
	{

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public ObjectStack() : base()
	  {
	  }

	  /// <summary>
	  /// Construct a ObjectVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of block to allocate </param>
	  public ObjectStack(int blocksize) : base(blocksize)
	  {
	  }

	  /// <summary>
	  /// Copy constructor for ObjectStack
	  /// </summary>
	  /// <param name="v"> ObjectStack to copy </param>
	  public ObjectStack(ObjectStack v) : base(v)
	  {
	  }

	  /// <summary>
	  /// Pushes an item onto the top of this stack.
	  /// </summary>
	  /// <param name="i">   the int to be pushed onto this stack. </param>
	  /// <returns>  the <code>item</code> argument. </returns>
	  public virtual object push(object i)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  object[] newMap = new object[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		m_map[m_firstFree] = i;

		m_firstFree++;

		return i;
	  }

	  /// <summary>
	  /// Removes the object at the top of this stack and returns that
	  /// object as the value of this function.
	  /// </summary>
	  /// <returns>     The object at the top of this stack. </returns>
	  public virtual object pop()
	  {
		object val = m_map[--m_firstFree];
		m_map[m_firstFree] = null;

		return val;
	  }

	  /// <summary>
	  /// Quickly pops a number of items from the stack.
	  /// </summary>

	  public virtual void quickPop(int n)
	  {
		m_firstFree -= n;
	  }

	  /// <summary>
	  /// Looks at the object at the top of this stack without removing it
	  /// from the stack.
	  /// </summary>
	  /// <returns>     the object at the top of this stack. </returns>
	  /// <exception cref="EmptyStackException">  if this stack is empty. </exception>
	  public virtual object peek()
	  {
		try
		{
		  return m_map[m_firstFree - 1];
		}
		catch (System.IndexOutOfRangeException)
		{
		  throw new EmptyStackException();
		}
	  }

	  /// <summary>
	  /// Looks at the object at the position the stack counting down n items.
	  /// </summary>
	  /// <param name="n"> The number of items down, indexed from zero. </param>
	  /// <returns>     the object at n items down. </returns>
	  /// <exception cref="EmptyStackException">  if this stack is empty. </exception>
	  public virtual object peek(int n)
	  {
		try
		{
		  return m_map[m_firstFree - (1 + n)];
		}
		catch (System.IndexOutOfRangeException)
		{
		  throw new EmptyStackException();
		}
	  }

	  /// <summary>
	  /// Sets an object at a the top of the statck
	  /// 
	  /// </summary>
	  /// <param name="val"> object to set at the top </param>
	  /// <exception cref="EmptyStackException">  if this stack is empty. </exception>
	  public virtual object Top
	  {
		  set
		  {
			try
			{
			  m_map[m_firstFree - 1] = value;
			}
			catch (System.IndexOutOfRangeException)
			{
			  throw new EmptyStackException();
			}
		  }
	  }

	  /// <summary>
	  /// Tests if this stack is empty.
	  /// </summary>
	  /// <returns>  <code>true</code> if this stack is empty;
	  ///          <code>false</code> otherwise.
	  /// @since   JDK1.0 </returns>
	  public virtual bool empty()
	  {
		return m_firstFree == 0;
	  }

	  /// <summary>
	  /// Returns where an object is on this stack.
	  /// </summary>
	  /// <param name="o">   the desired object. </param>
	  /// <returns>  the distance from the top of the stack where the object is]
	  ///          located; the return value <code>-1</code> indicates that the
	  ///          object is not on the stack.
	  /// @since   JDK1.0 </returns>
	  public virtual int search(object o)
	  {

		int i = lastIndexOf(o);

		if (i >= 0)
		{
		  return size() - i;
		}

		return -1;
	  }

	  /// <summary>
	  /// Returns clone of current ObjectStack
	  /// </summary>
	  /// <returns> clone of current ObjectStack </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public override object clone()
	  {
		  return (ObjectStack) base.clone();
	  }

	}

}