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
 * $Id: SerializerFactory.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{


	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// This class is a public API, it is a factory for creating serializers.
	/// 
	/// The properties object passed to the getSerializer() method should be created by
	/// the OutputPropertiesFactory. Although the properties object
	/// used to create a serializer does not need to be obtained 
	/// from OutputPropertiesFactory,
	/// using this factory ensures that the default key/value properties
	/// are set for the given output "method".
	/// 
	/// <para>
	/// The standard property keys supported are: "method", "version", "encoding",
	/// "omit-xml-declaration", "standalone", doctype-public",
	/// "doctype-system", "cdata-section-elements", "indent", "media-type". 
	/// These property keys and their values are described in the XSLT recommendation,
	/// see <seealso cref="<a href="http://www.w3.org/TR/1999/REC-xslt-19991116"> XSLT 1.0 recommendation</a>"/>
	/// 
	/// </para>
	/// <para>
	/// The value of the "cdata-section-elements" property key is a whitespace
	/// separated list of elements. If the element is in a namespace then 
	/// value is passed in this format: {uri}localName 
	///   
	/// </para>
	/// <para>
	/// The non-standard property keys supported are defined in <seealso cref="OutputPropertiesFactory"/>.
	///   
	/// </para>
	/// </summary>
	/// <seealso cref= OutputPropertiesFactory </seealso>
	/// <seealso cref= Method </seealso>
	/// <seealso cref= Serializer </seealso>
	public sealed class SerializerFactory
	{
	  /// <summary>
	  /// This constructor is private just to prevent the creation of such an object.
	  /// </summary>

	  private SerializerFactory()
	  {

	  }
	  /// <summary>
	  /// Associates output methods to default output formats.
	  /// </summary>
	  private static Hashtable m_formats = new Hashtable();

	  /// <summary>
	  /// Returns a serializer for the specified output method. The output method
	  /// is specified by the value of the property associated with the "method" key.
	  /// If no implementation exists that supports the specified output method
	  /// an exception of some type will be thrown.
	  /// For a list of the output "method" key values see <seealso cref="Method"/>.
	  /// </summary>
	  /// <param name="format"> The output format, minimally the "method" property must be set. </param>
	  /// <returns> A suitable serializer. </returns>
	  /// <exception cref="IllegalArgumentException"> if method is
	  /// null or an appropriate serializer can't be found </exception>
	  /// <exception cref="Exception"> if the class for the serializer is found but does not
	  /// implement ContentHandler. </exception>
	  /// <exception cref="WrappedRuntimeException"> if an exception is thrown while trying to find serializer </exception>
	  public static Serializer getSerializer(Properties format)
	  {
		  Serializer ser;

		  try
		  {
			string method = format.getProperty(OutputKeys.METHOD);

			if (string.ReferenceEquals(method, null))
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_FACTORY_PROPERTY_MISSING, new object[] {OutputKeys.METHOD});
				throw new System.ArgumentException(msg);
			}

			string className = format.getProperty(OutputPropertiesFactory.S_KEY_CONTENT_HANDLER);


			if (null == className)
			{
				// Missing Content Handler property, load default using OutputPropertiesFactory
				Properties methodDefaults = OutputPropertiesFactory.getDefaultMethodProperties(method);
				className = methodDefaults.getProperty(OutputPropertiesFactory.S_KEY_CONTENT_HANDLER);
				if (null == className)
				{
					string msg = Utils.messages.createMessage(MsgKey.ER_FACTORY_PROPERTY_MISSING, new object[] {OutputPropertiesFactory.S_KEY_CONTENT_HANDLER});
					throw new System.ArgumentException(msg);
				}

			}



			ClassLoader loader = ObjectFactory.findClassLoader();

			Type cls = ObjectFactory.findProviderClass(className, loader, true);

			// _serializers.put(method, cls);

			object obj = cls.newInstance();

			if (obj is SerializationHandler)
			{
				  // this is one of the supplied serializers
				ser = (Serializer) cls.newInstance();
				ser.OutputFormat = format;
			}
			else
			{
				  /*
				   *  This  must be a user defined Serializer.
				   *  It had better implement ContentHandler.
				   */
				   if (obj is ContentHandler)
				   {

					  /*
					   * The user defined serializer defines ContentHandler,
					   * but we need to wrap it with ToXMLSAXHandler which
					   * will collect SAX-like events and emit true
					   * SAX ContentHandler events to the users handler.
					   */
					  className = SerializerConstants_Fields.DEFAULT_SAX_SERIALIZER;
					  cls = ObjectFactory.findProviderClass(className, loader, true);
					  SerializationHandler sh = (SerializationHandler) cls.newInstance();
					  sh.ContentHandler = (ContentHandler) obj;
					  sh.OutputFormat = format;

					  ser = sh;
				   }
				   else
				   {
					  // user defined serializer does not implement
					  // ContentHandler, ... very bad
					   throw new Exception(Utils.messages.createMessage(MsgKey.ER_SERIALIZER_NOT_CONTENTHANDLER, new object[] {className}));
				   }

			}
		  }
		  catch (Exception e)
		  {
			throw new org.apache.xml.serializer.utils.WrappedRuntimeException(e);
		  }

		  // If we make it to here ser is not null.
		  return ser;
	  }
	}

}