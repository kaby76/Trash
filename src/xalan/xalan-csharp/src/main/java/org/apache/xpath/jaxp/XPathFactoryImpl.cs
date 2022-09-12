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
// $Id: XPathFactoryImpl.java 1225277 2011-12-28 18:50:56Z mrglavas $

namespace org.apache.xpath.jaxp
{

	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using XSLMessages = org.apache.xalan.res.XSLMessages;


	/// <summary>
	/// The XPathFactory builds XPaths.
	/// 
	/// @version $Revision: 1225277 $
	/// @author  Ramesh Mandava
	/// </summary>
	public class XPathFactoryImpl : XPathFactory
	{

		/// <summary>
		/// <para>Name of class as a constant to use for debugging.</para>
		/// </summary>
		private const string CLASS_NAME = "XPathFactoryImpl";

		/// <summary>
		/// <para>XPathFunctionResolver for this XPathFactory and created XPaths.</para>
		/// </summary>
		private XPathFunctionResolver xPathFunctionResolver = null;

		/// <summary>
		/// <para>XPathVariableResolver for this XPathFactory and created XPaths</para>
		/// </summary>
		private XPathVariableResolver xPathVariableResolver = null;

		/// <summary>
		/// <para>State of secure processing feature.</para>
		/// </summary>
		private bool featureSecureProcessing = false;

		/// <summary>
		/// <para>Is specified object model supported by this 
		/// <code>XPathFactory</code>?</para>
		/// </summary>
		/// <param name="objectModel"> Specifies the object model which the returned
		/// <code>XPathFactory</code> will understand.
		/// </param>
		/// <returns> <code>true</code> if <code>XPathFactory</code> supports 
		/// <code>objectModel</code>, else <code>false</code>.
		/// </returns>
		/// <exception cref="NullPointerException"> If <code>objectModel</code> is <code>null</code>. </exception>
		/// <exception cref="IllegalArgumentException"> If <code>objectModel.length() == 0</code>. </exception>
		public virtual bool isObjectModelSupported(string objectModel)
		{

				if (string.ReferenceEquals(objectModel, null))
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_OBJECT_MODEL_NULL, new object[] {this.GetType().FullName});

					throw new System.NullReferenceException(fmsg);
				}

				if (objectModel.Length == 0)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_OBJECT_MODEL_EMPTY, new object[] {this.GetType().FullName});
					throw new System.ArgumentException(fmsg);
				}

			// know how to support default object model, W3C DOM
				if (objectModel.Equals(XPathFactory.DEFAULT_OBJECT_MODEL_URI))
				{
					return true;
				}

				// don't know how to support anything else
				return false;
		}

			/// <summary>
			/// <para>Returns a new <code>XPath</code> object using the underlying
			/// object model determined when the factory was instantiated.</para>
			/// </summary>
			/// <returns> New <code>XPath</code> </returns>
		public virtual javax.xml.xpath.XPath newXPath()
		{
			return new org.apache.xpath.jaxp.XPathImpl(xPathVariableResolver, xPathFunctionResolver, featureSecureProcessing);
		}

		/// <summary>
		/// <para>Set a feature for this <code>XPathFactory</code> and 
		/// <code>XPath</code>s created by this factory.</para>
		/// 
		/// <para>
		/// Feature names are fully qualified <seealso cref="java.net.URI"/>s.
		/// Implementations may define their own features.
		/// An <seealso cref="XPathFactoryConfigurationException"/> is thrown if this
		/// <code>XPathFactory</code> or the <code>XPath</code>s
		///  it creates cannot support the feature.
		/// It is possible for an <code>XPathFactory</code> to expose a feature
		/// value but be unable to change its state.
		/// </para>
		/// 
		/// <para>See <seealso cref="javax.xml.xpath.XPathFactory"/> for full documentation
		/// of specific features.</para>
		/// </summary>
		/// <param name="name"> Feature name. </param>
		/// <param name="value"> Is feature state <code>true</code> or <code>false</code>.
		/// </param>
		/// <exception cref="XPathFactoryConfigurationException"> if this 
		/// <code>XPathFactory</code> or the <code>XPath</code>s
		///   it creates cannot support this feature. </exception>
		/// <exception cref="NullPointerException"> if <code>name</code> is 
		/// <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFeature(String name, boolean value) throws javax.xml.xpath.XPathFactoryConfigurationException
		public virtual void setFeature(string name, bool value)
		{

				// feature name cannot be null
				if (string.ReferenceEquals(name, null))
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_FEATURE_NAME_NULL, new object[] {CLASS_NAME, value ? true : false});
					throw new System.NullReferenceException(fmsg);
				}

				// secure processing?
				if (name.Equals(XMLConstants.FEATURE_SECURE_PROCESSING))
				{

					featureSecureProcessing = value;

					// all done processing feature
					return;
				}

				// unknown feature
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_FEATURE_UNKNOWN, new object[] {name, CLASS_NAME, value ? true : false});
				throw new XPathFactoryConfigurationException(fmsg);
		}

		/// <summary>
		/// <para>Get the state of the named feature.</para>
		/// 
		/// <para>
		/// Feature names are fully qualified <seealso cref="java.net.URI"/>s.
		/// Implementations may define their own features.
		/// An <seealso cref="XPathFactoryConfigurationException"/> is thrown if this
		/// <code>XPathFactory</code> or the <code>XPath</code>s
		/// it creates cannot support the feature.
		/// It is possible for an <code>XPathFactory</code> to expose a feature 
		/// value but be unable to change its state.
		/// </para>
		/// </summary>
		/// <param name="name"> Feature name.
		/// </param>
		/// <returns> State of the named feature.
		/// </returns>
		/// <exception cref="XPathFactoryConfigurationException"> if this 
		/// <code>XPathFactory</code> or the <code>XPath</code>s
		///   it creates cannot support this feature. </exception>
		/// <exception cref="NullPointerException"> if <code>name</code> is 
		/// <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getFeature(String name) throws javax.xml.xpath.XPathFactoryConfigurationException
		public virtual bool getFeature(string name)
		{

				// feature name cannot be null
				if (string.ReferenceEquals(name, null))
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_GETTING_NULL_FEATURE, new object[] {CLASS_NAME});
					throw new System.NullReferenceException(fmsg);
				}

				// secure processing?
				if (name.Equals(XMLConstants.FEATURE_SECURE_PROCESSING))
				{
					return featureSecureProcessing;
				}

				// unknown feature
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_GETTING_UNKNOWN_FEATURE, new object[] {name, CLASS_NAME});

				throw new XPathFactoryConfigurationException(fmsg);
		}

		/// <summary>
		/// <para>Establish a default function resolver.</para>
		/// 
		/// <para>Any <code>XPath</code> objects constructed from this factory will use
		/// the specified resolver by default.</para>
		/// 
		/// <para>A <code>NullPointerException</code> is thrown if 
		/// <code>resolver</code> is <code>null</code>.</para>
		/// </summary>
		/// <param name="resolver"> XPath function resolver.
		/// </param>
		/// <exception cref="NullPointerException"> If <code>resolver</code> is 
		/// <code>null</code>. </exception>
			public virtual XPathFunctionResolver XPathFunctionResolver
			{
				set
				{
    
					// value cannot be null
					if (value == null)
					{
						string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_XPATH_FUNCTION_RESOLVER, new object[] {CLASS_NAME});
						throw new System.NullReferenceException(fmsg);
					}
    
					xPathFunctionResolver = value;
				}
			}

		/// <summary>
		/// <para>Establish a default variable resolver.</para>
		/// 
		/// <para>Any <code>XPath</code> objects constructed from this factory will use
		/// the specified resolver by default.</para>
		/// 
		/// <para>A <code>NullPointerException</code> is thrown if <code>resolver</code> is <code>null</code>.</para>
		/// </summary>
		/// <param name="resolver"> Variable resolver.
		/// </param>
		///  <exception cref="NullPointerException"> If <code>resolver</code> is 
		/// <code>null</code>. </exception>
		public virtual XPathVariableResolver XPathVariableResolver
		{
			set
			{
    
				// value cannot be null
				if (value == null)
				{
							string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_XPATH_VARIABLE_RESOLVER, new object[] {CLASS_NAME});
					throw new System.NullReferenceException(fmsg);
				}
    
				xPathVariableResolver = value;
			}
		}
	}




}