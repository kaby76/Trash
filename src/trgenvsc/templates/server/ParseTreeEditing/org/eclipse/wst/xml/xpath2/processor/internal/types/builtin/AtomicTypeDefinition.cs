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

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;

	public interface AtomicTypeDefinition : TypeDefinition
	{

		SingleItemSequence construct(ResultSequence rs);
		SingleItemSequence constructNative(object rs);
	}
}