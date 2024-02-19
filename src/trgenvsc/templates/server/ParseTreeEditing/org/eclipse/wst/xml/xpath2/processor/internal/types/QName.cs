using System.Diagnostics;
using System.Text;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0 
///     Jesper Moller- bug 281159 - debugging convenience toString method 
///     David Carver (STAR) - bug 288886 - add unit tests and fix fn:resolve-qname function
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{
	using org.eclipse.wst.xml.xpath2.api;
	using org.eclipse.wst.xml.xpath2.api.typesystem;
	using org.eclipse.wst.xml.xpath2.processor.@internal.function;
	using org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin;

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	using XMLConstants = java.xml.XMLConstants;

	/// <summary>
	/// A representation of a QName datatype (name of a node)
	/// </summary>
	public class QName : CtrType, CmpEq
	{
		private const string XS_Q_NAME = "xs:QName";
		private string _namespace;
		private string _prefix;
		private string _local_part;
		private bool _expanded;

		/// <summary>
		/// Initialises using the supplied parameters
		/// </summary>
		/// <param name="prefix">
		///            Prefix of the node name </param>
		/// <param name="local_part">
		///            The node name itself </param>
		/// <param name="ns">
		///            The namespace this nodename belongs to </param>
		public QName(string prefix, string local_part, string ns) : this(prefix, local_part)
		{
		//	if (ns != null) {
				set_namespace(ns);
		//	}
		}

		/// <summary>
		/// Initialises using the supplied parameters
		/// </summary>
		/// <param name="prefix">
		///            Prefix of the node name </param>
		/// <param name="local_part">
		///            The node name itself </param>
		public QName(string prefix, string local_part)
		{
			_prefix = prefix;
			_local_part = local_part;
			_expanded = false;
		}

		/// <summary>
		/// Initialises using only the node name (no prefix)
		/// </summary>
		/// <param name="local_part">
		///            The node name </param>
		public QName(string local_part) : this(null, local_part)
		{
			set_namespace(null);
		}

		/// <summary>
		/// Initialises with a null prefix and null node name
		/// </summary>
		public QName() : this(null, null)
		{
		}

		public QName(javax.xml.@namespace.QName name) : this(XMLConstants.DEFAULT_NS_PREFIX.Equals(name.Prefix) ? null : name.Prefix, name.LocalPart)
		{
			if (!XMLConstants.NULL_NS_URI.Equals(name.NamespaceURI))
			{
				set_namespace(name.NamespaceURI);
			}
			_expanded = true;
		}

		/// <summary>
		/// Creates a new QName by parsing a String representation of the node name
		/// </summary>
		/// <param name="str">
		///            String representation of the name </param>
		/// <returns> null </returns>
		public static QName parse_QName(string str)
		{
			int occurs = 0;

			char[] strChrArr = str.ToCharArray();
			for (int chrIndx = 0; chrIndx < strChrArr.Length; chrIndx++)
			{
			  if (strChrArr[chrIndx] == ':')
			  {
				 occurs += 1;
			  }
			}

			if (occurs > 1)
			{
				return null;
			}

			string[] tokens = str.Split(":", true);

			if (tokens.Length == 1)
			{
				return new QName(tokens[0]);
			}

			if (tokens.Length == 2)
			{
					return new QName(tokens[0], tokens[1]);
			}

			return null;
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable QName in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to extract from </param>
		/// <returns> New ResultSequence consisting of the QName supplied </returns>
		/// <exception cref="DynamicError"> </exception>
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				DynamicError.throw_type_error();
			}

			AnyAtomicType aat = (AnyAtomicType) arg.first();

			if (!(aat is XSString) && !(aat is QName))
			{
				DynamicError.throw_type_error();
			}

			string sarg = aat.StringValue;

			QName qname = parse_QName(sarg);
			if (qname == null)
			{
				return null;
			}
			return qname;
		}

		/// <summary>
		/// Retrieves a String representation of the node name. This method is
		/// functionally identical to string()
		/// </summary>
		/// <returns> String representation of the node name </returns>
		public override string StringValue
		{
			get
			{
				return @string();
			}
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:QName" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_Q_NAME;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "QName" which is the datatype's name </returns>
		public override string type_name()
		{
			return "QName";
		}

		/// <summary>
		/// Retrieves a String representation of the node name. This method is
		/// functionally identical to string_value()
		/// </summary>
		/// <returns> String representation of the node name </returns>
		public virtual string @string()
		{
			string res = "";

			if (!string.ReferenceEquals(_prefix, null))
			{
				res = _prefix + ":";
			}

			res += _local_part;
			return res;
		}

		/// <summary>
		/// Retrieves the full pathname including the namespace. This method must not
		/// be called if a namespace does exist for this node
		/// </summary>
		/// <returns> Full pathname including namespace </returns>
		public virtual string expanded_name()
		{
			Debug.Assert(_expanded);
			// if(!_expanded)
			// return null;

			string e = "";
			if (!string.ReferenceEquals(_namespace, null))
			{
				e += _namespace + ":";
			}

			return e + _local_part;
		}

		/// <summary>
		/// Retrieves the prefix of the node's pathname
		/// </summary>
		/// <returns> Prefix of the node's pathname </returns>
		public virtual string prefix()
		{
			return _prefix;
		}

		/// <summary>
		/// Sets the namespace for this node
		/// </summary>
		/// <param name="n">
		///            Namespace this node belongs in </param>
		public virtual void set_namespace(string n)
		{
			_namespace = !string.ReferenceEquals(n, null) ? (n.Length == 0 ? null : n) : null;
			_expanded = true;
		}

		/// <summary>
		/// Retrieves the namespace that this node belongs in. This method must not
		/// be called if the node does not belong in a namespace
		/// </summary>
		/// <returns> Namespace that this node belongs in </returns>
		public virtual string @namespace()
		{
			Debug.Assert(_expanded);
			return _namespace;
		}

		/// <summary>
		/// Retrieves the node's name
		/// </summary>
		/// <returns> Node's name </returns>
		public virtual string local()
		{
			return _local_part;
		}

		/// <summary>
		/// Check for whether a namespace has been defined for this node
		/// </summary>
		/// <returns> True if a namespace has been defined for node. False otherwise </returns>
		public virtual bool expanded()
		{
			return _expanded;
		}

		/// <summary>
		/// Equality comparison between this QName and a supplied QName
		/// </summary>
		/// <param name="obj">
		///            The object to compare with. Should be of type QName </param>
		/// <returns> True if the two represent the same node. False otherwise </returns>
		public override bool Equals(object obj)
		{

			// make sure we are comparing a qname
			if (!(obj is QName))
			{
				return false;
			}

			QName arg = (QName) obj;

			// if they aren't expanded... we can't compare them
			if (!_expanded || !arg.expanded())
			{
				Debug.Assert(false); // XXX not stricly necessary
				return false;
			}

			// two cases: null == null, or .equals(other)
			string argn = arg.@namespace();
			if (!string.ReferenceEquals(_namespace, null))
			{
				if (!_namespace.Equals(argn))
				{
					return false;
				}
			}
			else
			{
				if (!string.ReferenceEquals(argn, null))
				{
					return false;
				}
			}

			string argl = arg.local();
			// XXX local part should always be non null ?
			if (!string.ReferenceEquals(_local_part, null))
			{
				if (!_local_part.Equals(argl))
				{
					return false;
				}
			}
			else
			{
				if (!string.ReferenceEquals(argl, null))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Calculates the hashcode for the full pathname
		/// </summary>
		/// <returns> The hashcode for the full pathname </returns>
		public override int GetHashCode()
		{
			int @namespace = 3;
			int local = 4;
			int result = 0;

			Debug.Assert(expanded());

			if (!string.ReferenceEquals(_namespace, null))
			{
				@namespace = _namespace.GetHashCode();
			}
			if (!string.ReferenceEquals(_local_part, null))
			{
				local = _local_part.GetHashCode();
			}

			result = @namespace;
			result ^= (2 * local);

			if (_expanded)
			{
				result ^= (result + 1);
			}

			return result;
		}

		/// <summary>
		/// Equality comparison between this QName and the supplied QName
		/// </summary>
		/// <param name="arg">
		///            The QName to compare with </param>
		/// <returns> True if the two represent the same node. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			QName val = (QName) NumericType.get_single_type(arg, typeof(QName));
			return Equals(val);
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_QNAME;
			}
		}

		public virtual javax.xml.@namespace.QName asQName()
		{
			return new javax.xml.@namespace.QName(@namespace(),
                local(), !string.ReferenceEquals(prefix(), null)
                    ? prefix() : XMLConstants.DEFAULT_NS_PREFIX);
		}

		public override object NativeValue
		{
			get
			{
				return asQName();
			}
		}
    }

}