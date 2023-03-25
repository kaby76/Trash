using System;
using System.Collections;
using System.Reflection;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011, 2018 IBM Corporation and others.
/// This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     IBM Corporation - initial API and implementation
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin
{
//	using InvocationTargetException = java.lang.reflect.InvocationTargetException;
    using QName = javax.xml.@namespace.QName;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using Attr = org.w3c.dom.Attr;
	using Element = org.w3c.dom.Element;

	public class BuiltinTypeDefinition : AtomicTypeDefinition
	{

		public const string XS_NS = "http://www.w3.org/2001/XMLSchema";

		private readonly QName name;
		private readonly Type implementationClass;
		private readonly Type nativeType;
		private readonly BuiltinTypeDefinition baseType;
		private readonly object/* Method */ constructorMethod;
		private readonly object/* Method */ constructorFromNativeMethod;

		public BuiltinTypeDefinition(QName name, BuiltinTypeDefinition baseType) : this(name, null, null, baseType)
		{
		}

		public BuiltinTypeDefinition(string name, BuiltinTypeDefinition baseType) : this(name, null, null, baseType)
		{
		}

		public BuiltinTypeDefinition(QName name, Type implementationClass, Type nativeType, BuiltinTypeDefinition baseType)
		{
			this.name = name;
			this.implementationClass = implementationClass;
			this.nativeType = nativeType;
			this.baseType = baseType;

		//	Method m = null;
			try
			{
		//		m = implementationClass != null ? implementationClass.GetMethod("constructor", typeof(ResultSequence)) : null;
			}
			catch
			{
				throw;
			}
	//		this.constructorMethod = m;

//m = null;
			try
			{
	//			m = implementationClass != null ? implementationClass.GetMethod("constructor", nativeType) : null;
			}
            catch
            {
                throw;
            }
	//		this.constructorFromNativeMethod = m;

		}

		public virtual bool Abstract
		{
			get
			{
				return implementationClass == null;
			}
		}

		public BuiltinTypeDefinition(string name, Type implementationClass, Type nativeType, BuiltinTypeDefinition baseType) : this(new QName(XS_NS, name), implementationClass, nativeType, baseType)
		{
		}

		public virtual string Namespace
		{
			get
			{
				return name.NamespaceURI;
			}
		}

		public virtual string Name
		{
			get
			{
				return name.LocalPart;
			}
		}

		public virtual TypeDefinition BaseType
		{
			get
			{
				return baseType;
			}
		}

		public virtual bool derivedFromType(TypeDefinition ancestorType, short derivationMethod)
		{
			if (ancestorType == this)
			{
				return true;
			}
			if (baseType == null)
			{
				return false;
			}
			return baseType.derivedFromType(ancestorType, derivationMethod);
		}

		public virtual bool derivedFrom(string @namespace, string name, short derivationMethod)
		{
			if (@namespace.Equals(Namespace) && name.Equals(Name))
			{
				return true;
			}

			if (baseType == null)
			{
				return false;
			}

			return baseType.derivedFrom(@namespace, name, derivationMethod);
		}

		public virtual IList getSimpleTypes(Attr attr)
		{
			return new ArrayList();
		}

		public virtual IList getSimpleTypes(Element attr)
		{
			return new ArrayList();
		}

		/* (non-Javadoc)
		 * @see org.eclipse.wst.xml.xpath2.processor.internal.types.builtin.AtomicTypeDefinition#construct(org.eclipse.wst.xml.xpath2.api.ResultSequence)
		 */
		public virtual SingleItemSequence construct(ResultSequence rs)
		{
			try
			{
				if (implementationClass == null)
				{
					throw new XPathError("Type " + Name + " is abstract!");
				}
			//	return (SingleItemSequence)constructorMethod.invoke(null, new object[] {rs});
            return null;
            }
            catch
            {
                throw;
            }
		}

		public virtual SingleItemSequence constructNative(object obj)
		{
			try
			{
				if (constructorFromNativeMethod == null)
				{
					throw new XPathError("Type " + Name + " cannot be constructed from native object!");
				}
	//			return (SingleItemSequence)constructorFromNativeMethod.invoke(null, new object[] {obj});
    return null;
            }
            catch
            {
                throw;
            }
		}

		public virtual Type NativeType
		{
			get
			{
				return nativeType;
			}
		}
	}

}