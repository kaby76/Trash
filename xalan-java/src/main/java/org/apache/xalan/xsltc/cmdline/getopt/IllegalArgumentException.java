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
 * $Id: IllegalArgumentException.java 468647 2006-10-28 06:59:33Z minchau $
 */

package org.apache.xalan.xsltc.cmdline.getopt;


class IllegalArgumentException extends GetOptsException{
    static final long serialVersionUID = 8642122427294793651L;
    public IllegalArgumentException(String msg){
	super(msg);
    }
}
