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
 * $Id: StackGuard.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Constants = org.apache.xalan.templates.Constants;
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ObjectStack = org.apache.xml.utils.ObjectStack;

	/// <summary>
	/// Class to guard against recursion getting too deep.
	/// </summary>
	public class StackGuard
	{

	  /// <summary>
	  /// Used for infinite loop check. If the value is -1, do not
	  /// check for infinite loops. Anyone who wants to enable that
	  /// check should change the value of this variable to be the
	  /// level of recursion that they want to check. Be careful setting
	  /// this variable, if the number is too low, it may report an
	  /// infinite loop situation, when there is none.
	  /// Post version 1.0.0, we'll make this a runtime feature.
	  /// </summary>
	  private int m_recursionLimit = -1;

	  internal TransformerImpl m_transformer;

	  /// <summary>
	  /// Get the recursion limit.
	  /// Used for infinite loop check. If the value is -1, do not
	  /// check for infinite loops. Anyone who wants to enable that
	  /// check should change the value of this variable to be the
	  /// level of recursion that they want to check. Be careful setting
	  /// this variable, if the number is too low, it may report an
	  /// infinite loop situation, when there is none.
	  /// Post version 1.0.0, we'll make this a runtime feature.
	  /// </summary>
	  /// <returns> The recursion limit. </returns>
	  public virtual int RecursionLimit
	  {
		  get
		  {
			return m_recursionLimit;
		  }
		  set
		  {
			m_recursionLimit = value;
		  }
	  }


	  /// <summary>
	  /// Constructor StackGuard
	  /// 
	  /// </summary>
	  public StackGuard(TransformerImpl transformerImpl)
	  {
		m_transformer = transformerImpl;
	  }

	  /// <summary>
	  /// Overide equal method for StackGuard objects 
	  /// 
	  /// </summary>
	  public virtual int countLikeTemplates(ElemTemplate templ, int pos)
	  {
		  ObjectStack elems = m_transformer.CurrentTemplateElements;
		  int count = 1;
		for (int i = pos - 1; i >= 0; i--)
		{
			if ((ElemTemplateElement)elems.elementAt(i) == templ)
			{
				count++;
			}
		}

		return count;
	  }


	  /// <summary>
	  /// Get the next named or match template down from and including 
	  /// the given position. </summary>
	  /// <param name="pos"> the current index position in the stack. </param>
	  /// <returns> null if no matched or named template found, otherwise 
	  /// the next named or matched template at or below the position. </returns>
	  private ElemTemplate getNextMatchOrNamedTemplate(int pos)
	  {
		  ObjectStack elems = m_transformer.CurrentTemplateElements;
		for (int i = pos; i >= 0; i--)
		{
			ElemTemplateElement elem = (ElemTemplateElement) elems.elementAt(i);
			if (null != elem)
			{
				if (elem.XSLToken == Constants.ELEMNAME_TEMPLATE)
				{
					return (ElemTemplate)elem;
				}
			}
		}
		  return null;
	  }

	  /// <summary>
	  /// Check if we are in an infinite loop
	  /// </summary>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void checkForInfinateLoop() throws javax.xml.transform.TransformerException
	  public virtual void checkForInfinateLoop()
	  {
		int nTemplates = m_transformer.CurrentTemplateElementsCount;
		if (nTemplates < m_recursionLimit)
		{
			return;
		}

		if (m_recursionLimit <= 0)
		{
			return; // Safety check.
		}

		// loop from the top index down to the recursion limit (I don't think 
		// there's any need to go below that).
		for (int i = (nTemplates - 1); i >= m_recursionLimit; i--)
		{
			ElemTemplate template = getNextMatchOrNamedTemplate(i);

			if (null == template)
			{
				break;
			}

			int loopCount = countLikeTemplates(template, i);

			if (loopCount >= m_recursionLimit)
			{
				// throw new TransformerException("Template nesting too deep. nesting = "+loopCount+
				//   ", template "+((null == template.getName()) ? "name = " : "match = ")+
				//   ((null != template.getName()) ? template.getName().toString() 
				//   : template.getMatch().getPatternString()));

				string idIs = XSLMessages.createMessage(((null != template.Name) ? "nameIs" : "matchPatternIs"), null);
				object[] msgArgs = new object[]
				{
					new int?(loopCount),
					idIs,
					((null != template.Name) ? template.Name.ToString() : template.Match.getPatternString())
				};
				string msg = XSLMessages.createMessage("recursionTooDeep", msgArgs);

				throw new TransformerException(msg);
			}
		}
	  }

	}

}