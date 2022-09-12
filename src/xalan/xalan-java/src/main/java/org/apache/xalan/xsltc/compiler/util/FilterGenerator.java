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
 * $Id: FilterGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

package org.apache.xalan.xsltc.compiler.util;

import org.apache.bcel.generic.ALOAD;
import org.apache.bcel.generic.Instruction;
import org.apache.xalan.xsltc.compiler.Stylesheet;

/**
 * This class implements auxliary classes needed to compile 
 * filters (predicates). These classes defined a single method
 * of type <tt>TestGenerator</tt>.
 * @author Jacek Ambroziak
 * @author Santiago Pericas-Geertsen
 */
public final class FilterGenerator extends ClassGenerator {
    private static int TRANSLET_INDEX = 5;   // translet
    private final Instruction _aloadTranslet;

    public FilterGenerator(String className, String superClassName,
			   String fileName,
			   int accessFlags, String[] interfaces,
			   Stylesheet stylesheet) {
	super(className, superClassName, fileName,
	      accessFlags, interfaces, stylesheet);
	
	_aloadTranslet = new ALOAD(TRANSLET_INDEX);
    }

    /**
     * The index of the translet pointer within the execution of
     * the test method.
     */
    public final Instruction loadTranslet() {
	return _aloadTranslet;
    }
    
    /**
     * Returns <tt>true</tt> since this class is external to the
     * translet.
     */
    public boolean isExternal() {
	return true;
    }
}
