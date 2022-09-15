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
 * $Id: BoolStack.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// Simple stack for boolean values.
	/// @xsl.usage internal
	/// </summary>
	public sealed class BoolStack : ICloneable
	{

	  /// <summary>
	  /// Array of boolean values </summary>
	  private bool[] m_values;

	  /// <summary>
	  /// Array size allocated </summary>
	  private int m_allocatedSize;

	  /// <summary>
	  /// Index into the array of booleans </summary>
	  private int m_index;

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public BoolStack() : this(32)
	  {
	  }

	  /// <summary>
	  /// Construct a IntVector, using the given block size.
	  /// </summary>
	  /// <param name="size"> array size to allocate </param>
	  public BoolStack(int size)
	  {

		m_allocatedSize = size;
		m_values = new bool[size];
		m_index = -1;
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> Current length of the list </returns>
	  public int size()
	  {
		return m_index + 1;
	  }

	  /// <summary>
	  /// Clears the stack.
	  /// 
	  /// </summary>
	  public void clear()
	  {
		  m_index = -1;
	  }

	  /// <summary>
	  /// Pushes an item onto the top of this stack.
	  /// 
	  /// </summary>
	  /// <param name="val"> the boolean to be pushed onto this stack. </param>
	  /// <returns>  the <code>item</code> argument. </returns>
	  public bool push(bool val)
	  {

		if (m_index == m_allocatedSize - 1)
		{
		  grow();
		}

		return (m_values[++m_index] = val);
	  }

	  /// <summary>
	  /// Removes the object at the top of this stack and returns that
	  /// object as the value of this function.
	  /// </summary>
	  /// <returns>     The object at the top of this stack. </returns>
	  /// <exception cref="EmptyStackException">  if this stack is empty. </exception>
	  public bool pop()
	  {
		return m_values[m_index--];
	  }

	  /// <summary>
	  /// Removes the object at the top of this stack and returns the
	  /// next object at the top as the value of this function.
	  /// 
	  /// </summary>
	  /// <returns> Next object to the top or false if none there </returns>
	  public bool popAndTop()
	  {

		m_index--;

		return (m_index >= 0) ? m_values[m_index] : false;
	  }

	  /// <summary>
	  /// Set the item at the top of this stack  
	  /// 
	  /// </summary>
	  /// <param name="b"> Object to set at the top of this stack </param>
	  public bool Top
	  {
		  set
		  {
			m_values[m_index] = value;
		  }
	  }

	  /// <summary>
	  /// Looks at the object at the top of this stack without removing it
	  /// from the stack.
	  /// </summary>
	  /// <returns>     the object at the top of this stack. </returns>
	  /// <exception cref="EmptyStackException">  if this stack is empty. </exception>
	  public bool peek()
	  {
		return m_values[m_index];
	  }

	  /// <summary>
	  /// Looks at the object at the top of this stack without removing it
	  /// from the stack.  If the stack is empty, it returns false.
	  /// </summary>
	  /// <returns>     the object at the top of this stack. </returns>
	  public bool peekOrFalse()
	  {
		return (m_index > -1) ? m_values[m_index] : false;
	  }

	  /// <summary>
	  /// Looks at the object at the top of this stack without removing it
	  /// from the stack.  If the stack is empty, it returns true.
	  /// </summary>
	  /// <returns>     the object at the top of this stack. </returns>
	  public bool peekOrTrue()
	  {
		return (m_index > -1) ? m_values[m_index] : true;
	  }

	  /// <summary>
	  /// Tests if this stack is empty.
	  /// </summary>
	  /// <returns>  <code>true</code> if this stack is empty;
	  ///          <code>false</code> otherwise. </returns>
	  public bool Empty
	  {
		  get
		  {
			return (m_index == -1);
		  }
	  }

	  /// <summary>
	  /// Grows the size of the stack
	  /// 
	  /// </summary>
	  private void grow()
	  {

		m_allocatedSize *= 2;

		bool[] newVector = new bool[m_allocatedSize];

		Array.Copy(m_values, 0, newVector, 0, m_index + 1);

		m_values = newVector;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public object clone()
	  {
		return base.clone();
	  }

	}

}