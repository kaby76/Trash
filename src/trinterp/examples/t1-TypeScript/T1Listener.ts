// Generated from T1.g4 by ANTLR 4.13.2

import {ParseTreeListener} from "antlr4";


import { SContext } from "./T1Parser.js";


/**
 * This interface defines a complete listener for a parse tree produced by
 * `T1Parser`.
 */
export default class T1Listener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by `T1Parser.s`.
	 * @param ctx the parse tree
	 */
	enterS?: (ctx: SContext) => void;
	/**
	 * Exit a parse tree produced by `T1Parser.s`.
	 * @param ctx the parse tree
	 */
	exitS?: (ctx: SContext) => void;
}

