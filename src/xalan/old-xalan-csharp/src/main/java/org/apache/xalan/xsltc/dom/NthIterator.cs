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
 * $Id: NthIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class NthIterator : DTMAxisIteratorBase
	{
		// ...[N]
		private DTMAxisIterator _source;
		private readonly new int _position;
		private bool _ready;

		public NthIterator(DTMAxisIterator source, int n)
		{
		_source = source;
		_position = n;
		}

		public override bool Restartable
		{
			set
			{
			_isRestartable = value;
			_source.Restartable = value;
			}
		}

		public override DTMAxisIterator cloneIterator()
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NthIterator clone = (NthIterator) super.clone();
			NthIterator clone = (NthIterator) base.clone();
			clone._source = _source.cloneIterator(); // resets source
			clone._isRestartable = false;
			return clone;
		}
		catch (CloneNotSupportedException e)
		{
			BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
			return null;
		}
		}

		public override int next()
		{
		if (_ready)
		{
			_ready = false;
			return _source.getNodeByPosition(_position);
		}
		return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		/*
		if (_ready && _position > 0) {
				final int pos = _source.isReverse()
										   ? _source.getLast() - _position + 1
										   : _position;
	
		    _ready = false;
		    int node;
		    while ((node = _source.next()) != DTMAxisIterator.END) {
			if (pos == _source.getPosition()) {
			    return node;
			}
		    }
		}
		return DTMAxisIterator.END;
		*/
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator setStartNode(final int node)
		public override DTMAxisIterator setStartNode(int node)
		{
		if (_isRestartable)
		{
			_source.StartNode = node;
			_ready = true;
		}
		return this;
		}

		public override DTMAxisIterator reset()
		{
		_source.reset();
		_ready = true;
		return this;
		}

		public override int Last
		{
			get
			{
			return 1;
			}
		}

		public override int Position
		{
			get
			{
			return 1;
			}
		}

		public override void setMark()
		{
		_source.setMark();
		}

		public override void gotoMark()
		{
		_source.gotoMark();
		}
	}

}