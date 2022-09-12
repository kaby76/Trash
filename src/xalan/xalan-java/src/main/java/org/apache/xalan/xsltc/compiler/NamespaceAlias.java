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
 * $Id: NamespaceAlias.java 468650 2006-10-28 07:03:30Z minchau $
 */

package org.apache.xalan.xsltc.compiler;

import org.apache.xalan.xsltc.compiler.util.ClassGenerator;
import org.apache.xalan.xsltc.compiler.util.MethodGenerator;
import org.apache.xalan.xsltc.compiler.util.Type;
import org.apache.xalan.xsltc.compiler.util.TypeCheckError;

/**
 * @author Jacek Ambroziak
 * @author Santiago Pericas-Geertsen
 */
final class NamespaceAlias extends TopLevelElement {

    private String sPrefix;
    private String rPrefix;
	
    /*
     * The namespace alias definitions given here have an impact only on
     * literal elements and literal attributes.
     */
    public void parseContents(Parser parser) {
	sPrefix = getAttribute("stylesheet-prefix");
	rPrefix = getAttribute("result-prefix");
	parser.getSymbolTable().addPrefixAlias(sPrefix,rPrefix);
    }
	
    public Type typeCheck(SymbolTable stable) throws TypeCheckError {
	return Type.Void;
    }
	
    public void translate(ClassGenerator classGen, MethodGenerator methodGen) {
	// do nada
    }
}
