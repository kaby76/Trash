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
 * $Id: LocationPathPattern.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Axis = org.apache.xml.dtm.Axis;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	public abstract class LocationPathPattern : Pattern
	{
		private Template _template;
		private int _importPrecedence;
		private double _priority = Double.NaN;
		private int _position = 0;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return Type.Void; // TODO
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		// TODO: What does it mean to translate a Pattern ?
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setTemplate(final Template template)
		public virtual Template Template
		{
			set
			{
			_template = value;
			_priority = value.Priority;
			_importPrecedence = value.ImportPrecedence;
			_position = value.Position;
			}
			get
			{
			return _template;
			}
		}


		public sealed override double Priority
		{
			get
			{
			return double.IsNaN(_priority) ? DefaultPriority : _priority;
			}
		}

		public virtual double DefaultPriority
		{
			get
			{
			return 0.5;
			}
		}

		/// <summary>
		/// This method is used by the Mode class to prioritise patterns and
		/// template. This method is called for templates that are in the same
		/// mode and that match on the same core pattern. The rules used are:
		///  o) first check precedence - highest precedence wins
		///  o) then check priority - highest priority wins
		///  o) then check the position - the template that occured last wins
		/// </summary>
		public virtual bool noSmallerThan(LocationPathPattern other)
		{
		if (_importPrecedence > other._importPrecedence)
		{
			return true;
		}
		else if (_importPrecedence == other._importPrecedence)
		{
			if (_priority > other._priority)
			{
			return true;
			}
			else if (_priority == other._priority)
			{
			if (_position > other._position)
			{
				return true;
			}
			}
		}
		return false;
		}

		public abstract StepPattern KernelPattern {get;}

		public abstract void reduceKernelPattern();

		public abstract bool Wildcard {get;}

		public virtual int Axis
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final StepPattern sp = getKernelPattern();
			StepPattern sp = KernelPattern;
			return (sp != null) ? sp.Axis : Axis.CHILD;
			}
		}

		public override string ToString()
		{
		return "root()";
		}
	}

}