using System;
using System.IO;

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
 * $Id: SerializerSwitcher.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{


	using Serializer = org.apache.xml.serializer.Serializer;
	using SerializerFactory = org.apache.xml.serializer.SerializerFactory;
	using Method = org.apache.xml.serializer.Method;
	using OutputProperties = org.apache.xalan.templates.OutputProperties;

	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// This is a helper class that decides if Xalan needs to switch
	/// serializers, based on the first output element.
	/// </summary>
	public class SerializerSwitcher
	{

	  /// <summary>
	  /// Switch to HTML serializer if element is HTML
	  /// 
	  /// </summary>
	  /// <param name="transformer"> Non-null transformer instance </param>
	  /// <param name="ns"> Namespace URI of the element </param>
	  /// <param name="localName"> Local part of name of element
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void switchSerializerIfHTML(TransformerImpl transformer, String ns, String localName) throws javax.xml.transform.TransformerException
	  public static void switchSerializerIfHTML(TransformerImpl transformer, string ns, string localName)
	  {

		if (null == transformer)
		{
		  return;
		}

		if (((null == ns) || (ns.Length == 0)) && localName.Equals("html", StringComparison.OrdinalIgnoreCase))
		{
		  // System.out.println("transformer.getOutputPropertyNoDefault(OutputKeys.METHOD): "+
		  //              transformer.getOutputPropertyNoDefault(OutputKeys.METHOD));     
		  // Access at level of hashtable to see if the method has been set.
		  if (null != transformer.getOutputPropertyNoDefault(OutputKeys.METHOD))
		  {
			return;
		  }

		  // Getting the output properties this way won't cause a clone of 
		  // the properties.
		  Properties prevProperties = transformer.OutputFormat.getProperties();

		  // We have to make sure we get an output properties with the proper 
		  // defaults for the HTML method.  The easiest way to do this is to 
		  // have the OutputProperties class do it.
		  OutputProperties htmlOutputProperties = new OutputProperties(Method.HTML);

		  htmlOutputProperties.copyFrom(prevProperties, true);
		  Properties htmlProperties = htmlOutputProperties.Properties;

		  try
		  {
	//        Serializer oldSerializer = transformer.getSerializer();
			Serializer oldSerializer = null;

			if (null != oldSerializer)
			{
			  Serializer serializer = SerializerFactory.getSerializer(htmlProperties);

			  Writer writer = oldSerializer.Writer;

			  if (null != writer)
			  {
				serializer.Writer = writer;
			  }
			  else
			  {
				Stream os = oldSerializer.OutputStream;

				if (null != os)
				{
				  serializer.OutputStream = os;
				}
			  }

	//          transformer.setSerializer(serializer);

			  ContentHandler ch = serializer.asContentHandler();

			  transformer.ContentHandler = ch;
			}
		  }
		  catch (java.io.IOException e)
		  {
			throw new TransformerException(e);
		  }
		}
	  }

	  /// <summary>
	  /// Get the value of a property, without using the default properties.  This 
	  /// can be used to test if a property has been explicitly set by the stylesheet 
	  /// or user.
	  /// </summary>
	  /// <param name="name"> The property name, which is a fully-qualified URI.
	  /// </param>
	  /// <returns> The value of the property, or null if not found.
	  /// </returns>
	  /// <exception cref="IllegalArgumentException"> If the property is not supported, 
	  /// and is not namespaced. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static String getOutputPropertyNoDefault(String qnameString, java.util.Properties props) throws IllegalArgumentException
	  private static string getOutputPropertyNoDefault(string qnameString, Properties props)
	  {
		string value = (string)props.get(qnameString);

		return value;
	  }

	  /// <summary>
	  /// Switch to HTML serializer if element is HTML
	  /// 
	  /// </summary>
	  /// <param name="ns"> Namespace URI of the element </param>
	  /// <param name="localName"> Local part of name of element
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
	  /// <returns> new contentHandler. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.apache.xml.serializer.Serializer switchSerializerIfHTML(String ns, String localName, java.util.Properties props, org.apache.xml.serializer.Serializer oldSerializer) throws javax.xml.transform.TransformerException
	  public static Serializer switchSerializerIfHTML(string ns, string localName, Properties props, Serializer oldSerializer)
	  {
		Serializer newSerializer = oldSerializer;

		if (((null == ns) || (ns.Length == 0)) && localName.Equals("html", StringComparison.OrdinalIgnoreCase))
		{
		  // System.out.println("transformer.getOutputPropertyNoDefault(OutputKeys.METHOD): "+
		  //              transformer.getOutputPropertyNoDefault(OutputKeys.METHOD));     
		  // Access at level of hashtable to see if the method has been set.
		  if (null != getOutputPropertyNoDefault(OutputKeys.METHOD, props))
		  {
			return newSerializer;
		  }

		  // Getting the output properties this way won't cause a clone of 
		  // the properties.
		  Properties prevProperties = props;

		  // We have to make sure we get an output properties with the proper 
		  // defaults for the HTML method.  The easiest way to do this is to 
		  // have the OutputProperties class do it.
		  OutputProperties htmlOutputProperties = new OutputProperties(Method.HTML);

		  htmlOutputProperties.copyFrom(prevProperties, true);
		  Properties htmlProperties = htmlOutputProperties.Properties;

		  {
	//      try
			if (null != oldSerializer)
			{
			  Serializer serializer = SerializerFactory.getSerializer(htmlProperties);

			  Writer writer = oldSerializer.Writer;

			  if (null != writer)
			  {
				serializer.Writer = writer;
			  }
			  else
			  {
				Stream os = serializer.OutputStream;

				if (null != os)
				{
				  serializer.OutputStream = os;
				}
			  }
			  newSerializer = serializer;
			}
		  }
	//      catch (java.io.IOException e)
	//      {
	//        throw new TransformerException(e);
	//      }
		}
		return newSerializer;
	  }

	}

}