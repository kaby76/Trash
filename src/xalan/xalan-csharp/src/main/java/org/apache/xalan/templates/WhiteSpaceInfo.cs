using System;

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
 * $Id: WhiteSpaceInfo.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XPath = org.apache.xpath.XPath;

	/// <summary>
	/// This is used as a special "fake" template that can be
	/// handled by the TemplateList to do pattern matching
	/// on nodes.
	/// </summary>
	[Serializable]
	public class WhiteSpaceInfo : ElemTemplate
	{
		internal new const long serialVersionUID = 6389208261999943836L;

	  /// <summary>
	  /// Flag indicating whether whitespaces should be stripped.
	  ///  @serial        
	  /// </summary>
	  private bool m_shouldStripSpace;

	  /// <summary>
	  /// Return true if this element specifies that the node that
	  /// matches the match pattern should be stripped, otherwise
	  /// the space should be preserved.
	  /// </summary>
	  /// <returns> value of m_shouldStripSpace flag </returns>
	  public virtual bool ShouldStripSpace
	  {
		  get
		  {
			return m_shouldStripSpace;
		  }
	  }

	  /// <summary>
	  /// Constructor WhiteSpaceInfo </summary>
	  /// <param name="thisSheet"> The current stylesheet </param>
	  public WhiteSpaceInfo(Stylesheet thisSheet)
	  {
		  Stylesheet = thisSheet;
	  }


	  /// <summary>
	  /// Constructor WhiteSpaceInfo
	  /// 
	  /// </summary>
	  /// <param name="matchPattern"> Match pattern </param>
	  /// <param name="shouldStripSpace"> Flag indicating whether or not
	  /// to strip whitespaces </param>
	  /// <param name="thisSheet"> The current stylesheet </param>
	  public WhiteSpaceInfo(XPath matchPattern, bool shouldStripSpace, Stylesheet thisSheet)
	  {

		m_shouldStripSpace = shouldStripSpace;

		Match = matchPattern;

		Stylesheet = thisSheet;
	  }

	  /// <summary>
	  /// This function is called to recompose() all of the WhiteSpaceInfo elements.
	  /// </summary>
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeWhiteSpaceInfo(this);
	  }

	}

}