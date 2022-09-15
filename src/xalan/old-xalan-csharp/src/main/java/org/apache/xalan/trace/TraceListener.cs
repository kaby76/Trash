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
 * $Id: TraceListener.java 468644 2006-10-28 06:56:42Z minchau $
 */
namespace org.apache.xalan.trace
{


	/// <summary>
	/// Interface the XSL processor calls when it matches a source node, selects a set of source nodes,
	/// or generates a result node.
	/// If you want an object instance to be called when a trace event occurs, use the TransformerImpl setTraceListener method. </summary>
	/// <seealso cref= org.apache.xalan.trace.TracerEvent </seealso>
	/// <seealso cref= org.apache.xalan.trace.TraceManager#addTraceListener
	/// @xsl.usage advanced </seealso>
	public interface TraceListener : java.util.EventListener
	{

	  /// <summary>
	  /// Method that is called when a trace event occurs.
	  /// The method is blocking.  It must return before processing continues.
	  /// </summary>
	  /// <param name="ev"> the trace event. </param>
	  void trace(TracerEvent ev);

	  /// <summary>
	  /// Method that is called just after the formatter listener is called.
	  /// </summary>
	  /// <param name="ev"> the generate event.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void selected(SelectionEvent ev) throws javax.xml.transform.TransformerException;
	  void selected(SelectionEvent ev);

	  /// <summary>
	  /// Method that is called just after the formatter listener is called.
	  /// </summary>
	  /// <param name="ev"> the generate event. </param>
	  void generated(GenerateEvent ev);
	}

}