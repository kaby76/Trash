// Generated from trgen <version>
use std::env;
use std::fs::File;
use std::io::{self, Read, BufRead, BufReader, Write};
use std::process;
use std::time::Instant;
use antlr4rust::common_token_stream::CommonTokenStream;
use antlr4rust::error_listener::ErrorListener;
use antlr4rust::input_stream::InputStream;
use antlr4rust::recognizer::Recognizer;
use antlr4rust::token::Token;
use antlr4rust::lexer::Lexer;
use antlr4rust::TokenSource;
use std::fs;
mod r#gen;
use r#gen::<lexer_name>;
use r#gen::<parser_name>;
use std::rc::Rc;
use std::cell::RefCell;
use std::cell::Cell;
use antlr4rust::token_factory::TokenFactory;
use antlr4rust::errors::ANTLRError;
use std::error::Error;
use std::sync::Arc;
use std::fmt::Display;
use antlr4rust::{Parser as AntlrParser};
use antlr4rust::tree::*;
use antlr4rust::trees::*;

/*
        if self.tee {
            writeln!(self.output, "line {}:{} {}", line, column, msg).ok();
        }
        if !self.quiet {
            eprintln!("line {}:{} {}", line, column, msg);
        }
    }
*/

fn parse_input(
    input_name: &str,
    is_filename: bool,
    idx: i32,
    flags: &Flags,
) -> usize {
    let parse_errors = Rc::new(RefCell::new(Vec::\<ParseError>::new()));
    let my_string_result = fs::read_to_string(input_name);
    let input = my_string_result.unwrap(); // Panics if Err
    let istream = InputStream::new(input.as_bytes());
    let mut lexer = <lexer_name>::new(istream);
    lexer.remove_error_listeners();
    let lec = Rc::new(RefCell::new(0));
    let mut lel = Box::new(ParserErrorListener {
        error_count: Rc::clone(&lec),
	parse_errors: parse_errors.clone(),
	});
    lexer.add_error_listener(lel);

    if flags.show_tokens {
	let mut i: isize = 0;
	loop {
		let mut token = lexer.next_token();
		let token_type = token.get_token_type();
		token.set_token_index(i);
		i = i + 1;
		eprintln!("{}", token.to_string());
		if token_type == -1 {
			break;
		}
	}
	// no lexer.reset();
    }

    let token_stream = CommonTokenStream::new(lexer);
    let mut parser = <parser_name>::new(token_stream);
    parser.remove_error_listeners();
    let pec = Rc::new(RefCell::new(0));
    let pel = Box::new(ParserErrorListener {
        error_count: Rc::clone(&pec),
	parse_errors: parse_errors.clone(),
	});
    parser.add_error_listener(pel);

    let mut tee_file = flags.tee.then(|| File::create(format!("{}.errors", input_name)).unwrap());
    let tee_writer: &mut dyn Write = tee_file.as_mut().map(|f| f as _).unwrap_or(&mut io::sink());

    let start = Instant::now();
    let tree = parser.<start_symbol>().expect("parsing failed setup");
    let elapsed = start.elapsed();

    let error_cnt = *lec.borrow() + *pec.borrow();

    if flags.show_tree {
	let tree_str = tree.to_string_tree(&*parser);
        if flags.tee {
            let mut f = File::create(format!("{}.tree", input_name)).unwrap();
            write!(f, "{}", tree_str).ok();
        } else {
	    eprintln!("{}", tree_str);
        }
    }

    if !flags.quiet {
        eprint!("{}Rust {} {} {} {:.3}\n",
            flags.prefix, idx, input_name,
            if error_cnt > 0 { "fail" } else { "success" },
            elapsed.as_secs_f64()
        );
    }

    if error_cnt > 0 { 1 } else { 0 }
}

struct Flags {
    inputs: Vec\<String>,
    is_fns: Vec\<bool>,
    prefix: String,
    show_tokens: bool,
    show_tree: bool,
    show_trace: bool,
    tee: bool,
    quiet: bool,
}

fn main() {
    let mut flags = Flags {
        inputs: Vec::new(),
        is_fns: Vec::new(),
        prefix: String::new(),
        show_tokens: false,
        show_tree: false,
        show_trace: false,
        tee: false,
        quiet: false,
    };

    let args: Vec\<String> = env::args().collect();
    let mut i = 1;
    while i \< args.len() {
        match args[i].as_str() {
            "-tokens" => flags.show_tokens = true,
            "-tree" => flags.show_tree = true,
            "-prefix" => {
                i += 1;
                flags.prefix = format!("{} ", args[i]);
            }
            "-input" => {
                i += 1;
                flags.inputs.push(args[i].clone());
                flags.is_fns.push(false);
            }
            "-tee" => flags.tee = true,
            "-q" => flags.quiet = true,
            "-trace" => flags.show_trace = true,
            "-x" => {
                let stdin = io::stdin();
                for line in stdin.lock().lines() {
                    let ln = line.unwrap();
                    flags.inputs.push(ln);
                    flags.is_fns.push(true);
                }
            }
            other => {
                flags.inputs.push(other.to_string());
                flags.is_fns.push(true);
            }
        }
        i += 1;
    }

    if flags.inputs.is_empty() {
            process::exit(1);
    } else {
        let mut error_code = 0;
        let start_all = Instant::now();
        for (idx, input) in flags.inputs.iter().enumerate() {
            let is_fn = flags.is_fns[idx];
	    parse_input(input, true, idx as i32, &flags);
        }
        let elapsed = start_all.elapsed();
	if !flags.quiet {
	    eprintln!("{}Total Time: {:.3}", flags.prefix, elapsed.as_secs_f64());
	}
        process::exit(error_code  as i32);
    }
}

struct ParserErrorListener {
    parse_errors: Rc\<RefCell\<Vec\<ParseError>>>,
    error_count: Rc\<RefCell\<i32>>,
}

impl\<'a, T: Recognizer\<'a>> ErrorListener\<'a, T> for ParserErrorListener {
    fn syntax_error(
        &self,
        _recognizer: &T,
        _offending_symbol: Option\<&\<T::TF as TokenFactory\<'a>>::Inner>,
        line: isize,
        column: isize,
        msg: &str,
        _error: Option\<&ANTLRError>,
    ) {
	*self.error_count.borrow_mut() += 1;
	eprintln!("Syn err {}", msg);
        self.parse_errors.borrow_mut().push(ParseError {
            source: None,
            pos: (line, column + 1),
            msg: format!("Syntax error: {msg}"),
        })
    }
}

#[allow(dead_code)]
#[derive(Debug)]
pub struct ParseError {
    pub source: Option\<Box\<dyn Error + Send + Sync + 'static>>,
    pub pos: (isize, isize),
    pub msg: String,
}

impl Display for ParseError {
    fn fmt(&self, f: &mut std::fmt::Formatter\<'_>) -> std::fmt::Result {
        write!(
            f,
            "ERROR: \<input>:{}:{}: {}",
            self.pos.0, self.pos.1, self.msg
        )?;
        Ok(())
    }
}

impl Error for ParseError {}
