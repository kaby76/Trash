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
 * $Id: VariableRefBase.java 476471 2006-11-18 08:36:27Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Morten Jorgensen
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal class VariableRefBase : Expression
	{

		/// <summary>
		/// A reference to the associated variable.
		/// </summary>
		protected internal VariableBase _variable;

		/// <summary>
		/// A reference to the enclosing expression/instruction for which a
		/// closure is needed (Predicate, Number or Sort).
		/// </summary>
		protected internal Closure _closure = null;

		public VariableRefBase(VariableBase variable)
		{
		_variable = variable;
		variable.addReference(this);
		}

		public VariableRefBase()
		{
		_variable = null;
		}

		/// <summary>
		/// Returns a reference to the associated variable
		/// </summary>
		public virtual VariableBase Variable
		{
			get
			{
			return _variable;
			}
		}

		/// <summary>
		/// If this variable reference is in a top-level element like 
		/// another variable, param or key, add a dependency between
		/// that top-level element and the referenced variable. For
		/// example,
		/// 
		///   <xsl:variable name="x" .../>
		///   <xsl:variable name="y" select="$x + 1"/>
		/// 
		/// and assuming this class represents "$x", add a reference 
		/// between variable y and variable x.
		/// </summary>
		public virtual void addParentDependency()
		{
		SyntaxTreeNode node = this;
		while (node != null && node is TopLevelElement == false)
		{
			node = node.Parent;
		}

			TopLevelElement parent = (TopLevelElement) node;
			if (parent != null)
			{
				VariableBase @var = _variable;
				if (_variable._ignore)
				{
					if (_variable is Variable)
					{
						@var = parent.SymbolTable.lookupVariable(_variable._name);
					}
					else if (_variable is Param)
					{
						@var = parent.SymbolTable.lookupParam(_variable._name);
					}
				}

				parent.addDependency(@var);
			}
		}

		/// <summary>
		/// Two variable references are deemed equal if they refer to the 
		/// same variable.
		/// </summary>
		public override bool Equals(object obj)
		{
		try
		{
			return (_variable == ((VariableRefBase) obj)._variable);
		}
		catch (System.InvalidCastException)
		{
			return false;
		}
		}

		/// <summary>
		/// Returns a string representation of this variable reference on the
		/// format 'variable-ref(<var-name>)'. </summary>
		/// <returns> Variable reference description </returns>
		public override string ToString()
		{
		return "variable-ref(" + _variable.Name + '/' + _variable.Type + ')';
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		// Returned cached type if available
		if (_type != null)
		{
			return _type;
		}

		// Find nearest closure to add a variable reference
		if (_variable.Local)
		{
			SyntaxTreeNode node = Parent;
			do
			{
			if (node is Closure)
			{
				_closure = (Closure) node;
				break;
			}
			if (node is TopLevelElement)
			{
				break; // way up in the tree
			}
			node = node.Parent;
			} while (node != null);

			if (_closure != null)
			{
			_closure.addVariable(this);
			}
		}

			// Attempt to get the cached variable type
			_type = _variable.Type;

			// If that does not work we must force a type-check (this is normally
			// only needed for globals in included/imported stylesheets
			if (_type == null)
			{
				_variable.typeCheck(stable);
				_type = _variable.Type;
			}

			// If in a top-level element, create dependency to the referenced var
			addParentDependency();

			// Return the type of the referenced variable
			return _type;
		}

	}

}