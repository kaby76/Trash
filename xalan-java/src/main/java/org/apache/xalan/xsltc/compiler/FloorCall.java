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
 * $Id: FloorCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

package org.apache.xalan.xsltc.compiler;

import java.util.Vector;

import org.apache.bcel.generic.INVOKESTATIC;
import org.apache.xalan.xsltc.compiler.util.ClassGenerator;
import org.apache.xalan.xsltc.compiler.util.MethodGenerator;

/**
 * @author Jacek Ambroziak
 * @author Santiago Pericas-Geertsen
 */
final class FloorCall extends FunctionCall {
    public FloorCall(QName fname, Vector arguments) {
	super(fname, arguments);
    }

    public void translate(ClassGenerator classGen, MethodGenerator methodGen) {
	argument().translate(classGen, methodGen);
	methodGen.getInstructionList()
	    .append(new INVOKESTATIC(classGen.getConstantPool()
				     .addMethodref(MATH_CLASS,
						   "floor", "(D)D")));
    }
}