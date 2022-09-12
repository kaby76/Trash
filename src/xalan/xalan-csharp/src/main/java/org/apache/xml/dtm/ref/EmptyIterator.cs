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
 * $Id: EmptyIterator.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref
{



	/// <summary>
	/// DTM Empty Axis Iterator. The class is immutable
	/// </summary>
	public sealed class EmptyIterator : DTMAxisIterator
	{
	  private static readonly EmptyIterator INSTANCE = new EmptyIterator();

	  public static DTMAxisIterator Instance
	  {
		  get
		  {
			  return INSTANCE;
		  }
	  }

	  private EmptyIterator()
	  {
	  }

	  public int next()
	  {
		  return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
	  }

	  public DTMAxisIterator reset()
	  {
		  return this;
	  }

	  public int Last
	  {
		  get
		  {
			  return 0;
		  }
	  }

	  public int Position
	  {
		  get
		  {
			  return 1;
		  }
	  }

	  public void setMark()
	  {
	  }

	  public void gotoMark()
	  {
	  }

	  public DTMAxisIterator setStartNode(int node)
	  {
		  return this;
	  }

	  public int StartNode
	  {
		  get
		  {
			  return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		  }
	  }

	  public bool Reverse
	  {
		  get
		  {
			  return false;
		  }
	  }

	  public DTMAxisIterator cloneIterator()
	  {
		  return this;
	  }

	  public bool Restartable
	  {
		  set
		  {
		  }
	  }

	  public int getNodeByPosition(int position)
	  {
		  return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
	  }
	}

}