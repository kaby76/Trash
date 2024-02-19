using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using javax.xml.@namespace;
using org.eclipse.wst.xml.xpath2.processor.@internal.ast;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2013 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.ast
{
    using QName = javax.xml.@namespace.QName;
    using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
    using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
    using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
    using XPath2Expression = org.eclipse.wst.xml.xpath2.api.XPath2Expression;
    using XPathNode = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathNode;
    using XPathVisitor = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathVisitor;

    /// <summary>
    /// Support for XPath.
    /// </summary>
    /// @deprecated This is only for internal use, use XPath2Expression instead 
    public class XPath : XPathNode, XPath2Expression, IEnumerable
    {
        private ICollection<Expr> _exprs;
        private StaticContext _staticContext;
        private ICollection<QName> _resolvedFunctions;
        private ICollection<string> _axes;
        private ICollection<QName> _freeVariables;
        private bool _rootUsed;

        /// <summary>
        /// Constructor for XPath.
        /// </summary>
        /// <param name="exprs">
        ///            XPath expressions. </param>
        public XPath(ICollection<Expr> exprs)
        {
            _exprs = exprs;
        }

        /// <summary>
        /// Support for Visitor interface.
        /// </summary>
        /// <returns> Result of Visitor operation. </returns>
        public override object accept(XPathVisitor v)
        {
            return v.visit(this);
        }

        /// <summary>
        /// Support for Iterator interface.
        /// </summary>
        /// <returns> Result of Iterator operation. </returns>
        public virtual IEnumerator<Expr> iterator()
        {
            return _exprs.GetEnumerator();
        }

        public string Expression { get; set; }

        /// <summary>
        /// @since 2.0
        /// </summary>
        public virtual ICollection<QName> FreeVariables
        {
            get { return _freeVariables; }
            set { this._freeVariables = value; }
        }


        /// <summary>
        /// @since 2.0
        /// </summary>
        public virtual ICollection<QName> ResolvedFunctions
        {
            get { return _resolvedFunctions; }
            set { this._resolvedFunctions = value; }
        }


        /// <summary>
        /// @since 2.0
        /// </summary>
        public virtual ICollection<string> Axes
        {
            get { return _axes; }
            set { this._axes = value; }
        }


        /// <summary>
        /// @since 2.0
        /// </summary>
        public virtual bool RootPathUsed
        {
            get { return _rootUsed; }
        }

        /// <summary>
        /// @since 2.0
        /// </summary>
        public virtual bool RootUsed
        {
            set { this._rootUsed = value; }
        }

        /// <summary>
        /// @since 2.0
        /// </summary>
        public virtual ResultSequence evaluate(DynamicContext dynamicContext, object[] contextItems)
        {
            if (_staticContext == null)
            {
                throw new System.InvalidOperationException("Static Context not set yet!");
            }

            return (new DefaultEvaluator(_staticContext, dynamicContext, contextItems)).evaluate2(this);
        }

        /// <summary>
        /// @since 2.0
        /// </summary>
        public virtual StaticContext StaticContext
        {
            get { return _staticContext; }
            set
            {
                if (_staticContext != null)
                {
                    throw new System.InvalidOperationException("Static Context already set!");
                }

                this._staticContext = value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _exprs.GetEnumerator();
        }


        public override ICollection<XPathNode> GetAllChildren()
        {
            return _exprs.Select(t => (XPathNode)t).ToList();
        }

        public override string QuickInfo()
        {
            return "";
        }
    }
}