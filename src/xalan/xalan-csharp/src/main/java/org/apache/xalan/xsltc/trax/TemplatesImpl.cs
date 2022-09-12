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
 * $Id: TemplatesImpl.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xalan.xsltc.trax
{



	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;

	/// <summary>
	/// @author Morten Jorgensen
	/// @author G. Todd Millerj
	/// @author Jochen Cordes <Jochen.Cordes@t-online.de>
	/// @author Santiago Pericas-Geertsen 
	/// </summary>
	[Serializable]
	public sealed class TemplatesImpl : Templates
	{
		internal const long serialVersionUID = 673094361519270707L;
		/// <summary>
		/// Name of the superclass of all translets. This is needed to
		/// determine which, among all classes comprising a translet, 
		/// is the main one.
		/// </summary>
		private static string ABSTRACT_TRANSLET = "org.apache.xalan.xsltc.runtime.AbstractTranslet";

		/// <summary>
		/// Name of the main class or default name if unknown.
		/// </summary>
		private string _name = null;

		/// <summary>
		/// Contains the actual class definition for the translet class and
		/// any auxiliary classes.
		/// </summary>
		private sbyte[][] _bytecodes = null;

		/// <summary>
		/// Contains the translet class definition(s). These are created when 
		/// this Templates is created or when it is read back from disk.
		/// </summary>
		private Type[] _class = null;

		/// <summary>
		/// The index of the main translet class in the arrays _class[] and
		/// _bytecodes.
		/// </summary>
		private int _transletIndex = -1;

		/// <summary>
		/// Contains the list of auxiliary class definitions.
		/// </summary>
		private Hashtable _auxClasses = null;

		/// <summary>
		/// Output properties of this translet.
		/// </summary>
		private Properties _outputProperties;

		/// <summary>
		/// Number of spaces to add for output indentation.
		/// </summary>
		private int _indentNumber;

		/// <summary>
		/// This URIResolver is passed to all Transformers.
		/// Declaring it transient to fix bug 22438 
		/// </summary>
		[NonSerialized]
		private URIResolver _uriResolver = null;

		/// <summary>
		/// Cache the DTM for the stylesheet in a thread local variable,
		/// which is used by the document('') function.
		/// Use ThreadLocal because a DTM cannot be shared between
		/// multiple threads. 
		/// Declaring it transient to fix bug 22438 
		/// </summary>
		[NonSerialized]
		private ThreadLocal _sdom = new ThreadLocal();

		/// <summary>
		/// A reference to the transformer factory that this templates
		/// object belongs to.
		/// </summary>
		[NonSerialized]
		private TransformerFactoryImpl _tfactory = null;

		internal sealed class TransletClassLoader : ClassLoader
		{
		internal TransletClassLoader(ClassLoader parent) : base(parent)
		{
		}

			/// <summary>
			/// Access to final protected superclass member from outer class.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: Class defineClass(final byte[] b)
		internal Type defineClass(sbyte[] b)
		{
				return defineClass(null, b, 0, b.Length);
		}
		}


		/// <summary>
		/// Create an XSLTC template object from the bytecodes.
		/// The bytecodes for the translet and auxiliary classes, plus the name of
		/// the main translet class, must be supplied.
		/// </summary>
		protected internal TemplatesImpl(sbyte[][] bytecodes, string transletName, Properties outputProperties, int indentNumber, TransformerFactoryImpl tfactory)
		{
		_bytecodes = bytecodes;
		_name = transletName;
		_outputProperties = outputProperties;
		_indentNumber = indentNumber;
		_tfactory = tfactory;
		}

		/// <summary>
		/// Create an XSLTC template object from the translet class definition(s).
		/// </summary>
		protected internal TemplatesImpl(Type[] transletClasses, string transletName, Properties outputProperties, int indentNumber, TransformerFactoryImpl tfactory)
		{
		_class = transletClasses;
		_name = transletName;
		_transletIndex = 0;
		_outputProperties = outputProperties;
		_indentNumber = indentNumber;
		_tfactory = tfactory;
		}


		/// <summary>
		/// Need for de-serialization, see readObject().
		/// </summary>
		public TemplatesImpl()
		{
		}

		/// <summary>
		///  Overrides the default readObject implementation since we decided
		///  it would be cleaner not to serialize the entire tranformer
		///  factory.  [ ref bugzilla 12317 ]
		///  We need to check if the user defined class for URIResolver also
		///  implemented Serializable
		///  if yes then we need to deserialize the URIResolver
		///  Fix for bugzilla bug 22438
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream is) throws java.io.IOException, ClassNotFoundException
		private void readObject(ObjectInputStream @is)
		{
		@is.defaultReadObject();
			if (@is.readBoolean())
			{
				_uriResolver = (URIResolver) @is.readObject();
			}

		_tfactory = new TransformerFactoryImpl();
		}


		/// <summary>
		///  This is to fix bugzilla bug 22438
		///  If the user defined class implements URIResolver and Serializable
		///  then we want it to get serialized
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream os) throws java.io.IOException, ClassNotFoundException
		private void writeObject(ObjectOutputStream os)
		{
			os.defaultWriteObject();
			if (_uriResolver is Serializable)
			{
				os.writeBoolean(true);
				os.writeObject((Serializable) _uriResolver);
			}
			else
			{
				os.writeBoolean(false);
			}
		}


		 /// <summary>
		 /// Store URIResolver needed for Transformers.
		 /// </summary>
		public URIResolver URIResolver
		{
			set
			{
				lock (this)
				{
				_uriResolver = value;
				}
			}
		}

		/// <summary>
		/// The TransformerFactory must pass us the translet bytecodes using this
		/// method before we can create any translet instances
		/// </summary>
		protected internal sbyte[][] TransletBytecodes
		{
			set
			{
				lock (this)
				{
				_bytecodes = value;
				}
			}
			get
			{
				lock (this)
				{
				return _bytecodes;
				}
			}
		}


		/// <summary>
		/// Returns the translet bytecodes stored in this template
		/// </summary>
		public Type[] TransletClasses
		{
			get
			{
				lock (this)
				{
				try
				{
					if (_class == null)
					{
						defineTransletClasses();
					}
				}
				catch (TransformerConfigurationException)
				{
					// Falls through
				}
				return _class;
				}
			}
		}

		/// <summary>
		/// Returns the index of the main class in array of bytecodes
		/// </summary>
		public int TransletIndex
		{
			get
			{
				lock (this)
				{
				try
				{
					if (_class == null)
					{
						defineTransletClasses();
					}
				}
				catch (TransformerConfigurationException)
				{
					// Falls through
				}
				return _transletIndex;
				}
			}
		}

		/// <summary>
		/// The TransformerFactory should call this method to set the translet name
		/// </summary>
		protected internal string TransletName
		{
			set
			{
				lock (this)
				{
				_name = value;
				}
			}
			get
			{
				lock (this)
				{
				return _name;
				}
			}
		}


		/// <summary>
		/// Defines the translet class and auxiliary classes.
		/// Returns a reference to the Class object that defines the main class
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void defineTransletClasses() throws javax.xml.transform.TransformerConfigurationException
		private void defineTransletClasses()
		{

		if (_bytecodes == null)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.NO_TRANSLET_CLASS_ERR);
			throw new TransformerConfigurationException(err.ToString());
		}

			TransletClassLoader loader = (TransletClassLoader) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass(this));

		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int classCount = _bytecodes.length;
			int classCount = _bytecodes.Length;
			_class = new Type[classCount];

			if (classCount > 1)
			{
				_auxClasses = new Hashtable();
			}

			for (int i = 0; i < classCount; i++)
			{
			_class[i] = loader.defineClass(_bytecodes[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class superClass = _class[i].getSuperclass();
			Type superClass = _class[i].BaseType;

			// Check if this is the main class
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			if (superClass.FullName.Equals(ABSTRACT_TRANSLET))
			{
				_transletIndex = i;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				_auxClasses.put(_class[i].FullName, _class[i]);
			}
			}

			if (_transletIndex < 0)
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.NO_MAIN_TRANSLET_ERR, _name);
			throw new TransformerConfigurationException(err.ToString());
			}
		}
		catch (ClassFormatError)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.TRANSLET_CLASS_ERR, _name);
			throw new TransformerConfigurationException(err.ToString());
		}
		catch (LinkageError)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.TRANSLET_OBJECT_ERR, _name);
			throw new TransformerConfigurationException(err.ToString());
		}
		}

		private class PrivilegedActionAnonymousInnerClass : PrivilegedAction
		{
			private readonly TemplatesImpl outerInstance;

			public PrivilegedActionAnonymousInnerClass(TemplatesImpl outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual object run()
			{
				return new TransletClassLoader(ObjectFactory.findClassLoader());
			}
		}

		/// <summary>
		/// This method generates an instance of the translet class that is
		/// wrapped inside this Template. The translet instance will later
		/// be wrapped inside a Transformer object.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.apache.xalan.xsltc.Translet getTransletInstance() throws javax.xml.transform.TransformerConfigurationException
		private Translet TransletInstance
		{
			get
			{
			try
			{
				if (string.ReferenceEquals(_name, null))
				{
					return null;
				}
    
				if (_class == null)
				{
					defineTransletClasses();
				}
    
				// The translet needs to keep a reference to all its auxiliary 
				// class to prevent the GC from collecting them
				AbstractTranslet translet = (AbstractTranslet) _class[_transletIndex].newInstance();
					translet.postInitialization();
				translet.Templates = this;
				if (_auxClasses != null)
				{
					translet.AuxiliaryClasses = _auxClasses;
				}
    
				return translet;
			}
			catch (InstantiationException)
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.TRANSLET_OBJECT_ERR, _name);
				throw new TransformerConfigurationException(err.ToString());
			}
			catch (IllegalAccessException)
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.TRANSLET_OBJECT_ERR, _name);
				throw new TransformerConfigurationException(err.ToString());
			}
			}
		}

		/// <summary>
		/// Implements JAXP's Templates.newTransformer()
		/// </summary>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized javax.xml.transform.Transformer newTransformer() throws javax.xml.transform.TransformerConfigurationException
		public Transformer newTransformer()
		{
			lock (this)
			{
			TransformerImpl transformer;
        
			transformer = new TransformerImpl(TransletInstance, _outputProperties, _indentNumber, _tfactory);
        
			if (_uriResolver != null)
			{
				transformer.URIResolver = _uriResolver;
			}
        
			if (_tfactory.getFeature(XMLConstants.FEATURE_SECURE_PROCESSING))
			{
				transformer.SecureProcessing = true;
			}
			return transformer;
			}
		}

		/// <summary>
		/// Implements JAXP's Templates.getOutputProperties(). We need to 
		/// instanciate a translet to get the output settings, so
		/// we might as well just instanciate a Transformer and use its
		/// implementation of this method.
		/// </summary>
		public Properties OutputProperties
		{
			get
			{
				lock (this)
				{
				try
				{
					return newTransformer().OutputProperties;
				}
				catch (TransformerConfigurationException)
				{
					return null;
				}
				}
			}
		}

		/// <summary>
		/// Return the thread local copy of the stylesheet DOM.
		/// </summary>
		public DOM StylesheetDOM
		{
			get
			{
				return (DOM)_sdom.get();
			}
			set
			{
				_sdom.set(value);
			}
		}

	}

}