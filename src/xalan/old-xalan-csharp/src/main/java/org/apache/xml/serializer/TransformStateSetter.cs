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
 * $Id: TransformStateSetter.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	using Node = org.w3c.dom.Node;
	/// <summary>
	/// This interface is meant to be used by a base interface to
	/// TransformState, but which as only the setters which have non Xalan
	/// specific types in their signature, so that there are no dependancies
	/// of the serializer on Xalan.
	/// 
	/// This interface is not a public API, it is only public because it is
	/// used by Xalan.
	/// </summary>
	/// <seealso cref= org.apache.xalan.transformer.TransformState
	/// @xsl.usage internal </seealso>
	public interface TransformStateSetter
	{


	  /// <summary>
	  /// Set the current node.
	  /// </summary>
	  /// <param name="n"> The current node. </param>
	  Node CurrentNode {set;}

	  /// <summary>
	  /// Reset the state on the given transformer object.
	  /// </summary>
	  /// <param name="transformer"> </param>
	  void resetState(Transformer transformer);

	}

}