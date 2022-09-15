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
 * $Id: SerializerTrace.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{
	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// This interface defines a set of integer constants that identify trace event
	/// types.
	/// 
	/// @xsl.usage internal
	/// </summary>

	public interface SerializerTrace
	{

	  /// <summary>
	  /// Event type generated when a document begins.
	  /// 
	  /// </summary>
	  public static int EVENTTYPE_STARTDOCUMENT = 1;

	  /// <summary>
	  /// Event type generated when a document ends.
	  /// </summary>
	  public static int EVENTTYPE_ENDDOCUMENT = 2;

	  /// <summary>
	  /// Event type generated when an element begins (after the attributes have been processed but before the children have been added).
	  /// </summary>
	  public static int EVENTTYPE_STARTELEMENT = 3;

	  /// <summary>
	  /// Event type generated when an element ends, after it's children have been added.
	  /// </summary>
	  public static int EVENTTYPE_ENDELEMENT = 4;

	  /// <summary>
	  /// Event type generated for character data (CDATA and Ignorable Whitespace have their own events).
	  /// </summary>
	  public static int EVENTTYPE_CHARACTERS = 5;

	  /// <summary>
	  /// Event type generated for ignorable whitespace (I'm not sure how much this is actually called.
	  /// </summary>
	  public static int EVENTTYPE_IGNORABLEWHITESPACE = 6;

	  /// <summary>
	  /// Event type generated for processing instructions.
	  /// </summary>
	  public static int EVENTTYPE_PI = 7;

	  /// <summary>
	  /// Event type generated after a comment has been added.
	  /// </summary>
	  public static int EVENTTYPE_COMMENT = 8;

	  /// <summary>
	  /// Event type generate after an entity ref is created.
	  /// </summary>
	  public static int EVENTTYPE_ENTITYREF = 9;

	  /// <summary>
	  /// Event type generated after CDATA is generated.
	  /// </summary>
	  public static int EVENTTYPE_CDATA = 10;

	  /// <summary>
	  /// Event type generated when characters might be written to an output stream,
	  ///  but  these characters never are. They will ultimately be written out via
	  /// EVENTTYPE_OUTPUT_CHARACTERS. This type is used as attributes are collected.
	  /// Whenever the attributes change this event type is fired. At the very end
	  /// however, when the attributes do not change anymore and are going to be
	  /// ouput to the document the real characters will be written out using the
	  /// EVENTTYPE_OUTPUT_CHARACTERS.
	  /// </summary>
	  public static int EVENTTYPE_OUTPUT_PSEUDO_CHARACTERS = 11;

	  /// <summary>
	  /// Event type generated when characters are written to an output stream.
	  /// </summary>
	  public static int EVENTTYPE_OUTPUT_CHARACTERS = 12;


	  /// <summary>
	  /// Tell if trace listeners are present.
	  /// </summary>
	  /// <returns> True if there are trace listeners </returns>
	  bool hasTraceListeners();

	  /// <summary>
	  /// Fire startDocument, endDocument events.
	  /// </summary>
	  /// <param name="eventType"> One of the EVENTTYPE_XXX constants. </param>
	  void fireGenerateEvent(int eventType);

	  /// <summary>
	  /// Fire startElement, endElement events.
	  /// </summary>
	  /// <param name="eventType"> One of the EVENTTYPE_XXX constants. </param>
	  /// <param name="name"> The name of the element. </param>
	  /// <param name="atts"> The SAX attribute list. </param>
	  void fireGenerateEvent(int eventType, string name, Attributes atts);

	  /// <summary>
	  /// Fire characters, cdata events.
	  /// </summary>
	  /// <param name="eventType"> One of the EVENTTYPE_XXX constants. </param>
	  /// <param name="ch"> The char array from the SAX event. </param>
	  /// <param name="start"> The start offset to be used in the char array. </param>
	  /// <param name="length"> The end offset to be used in the chara array. </param>
	  void fireGenerateEvent(int eventType, char[] ch, int start, int length);

	  /// <summary>
	  /// Fire processingInstruction events.
	  /// </summary>
	  /// <param name="eventType"> One of the EVENTTYPE_XXX constants. </param>
	  /// <param name="name"> The name of the processing instruction. </param>
	  /// <param name="data"> The processing instruction data. </param>
	  void fireGenerateEvent(int eventType, string name, string data);


	  /// <summary>
	  /// Fire comment and entity ref events.
	  /// </summary>
	  /// <param name="eventType"> One of the EVENTTYPE_XXX constants. </param>
	  /// <param name="data"> The comment or entity ref data. </param>
	  void fireGenerateEvent(int eventType, string data);

	}

}