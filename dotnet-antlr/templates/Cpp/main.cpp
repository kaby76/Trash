#include \<chrono>
#include \<atomic>
#include \<iostream>
#include \<utility>
#include \<thread>

#include \<pthread.h>

#include "HTMLLexer.h"
#include "HTMLParser.h"
#include "HTMLParserBaseListener.h"
#include "HTMLParserBaseVisitor.h"

using namespace std::chrono;

using namespace antlr4;
using namespace antlr4::atn;

using namespace antlrhtmltest;

std::string formatDuration(uint64_t duration) {
	std::stringstream oss;

	oss \<\< std::setfill('0')
			\<\< (duration % 60000000000) / 60000000
			\<\< ":"
			\<\< std::setw(2)
			\<\< (duration % 60000000) / 1000000
			\<\< "."
			\<\< std::setw(3)
			\<\< (duration % 1000000) / 1000
			\<\< "."
			\<\< std::setw(3)
			\<\< duration % 1000;

	return oss.str();
}

void parse(std::istream &stream, std::string title, bool dumpTokenStream, bool dumpParseTree) {
	ANTLRInputStream input("");
	input.load(stream);

	HTMLLexer lexer(&input);
	CommonTokenStream tokens(&lexer);
	HTMLParser parser(&tokens);

	parser.setBuildParseTree(false);

  // First parse with the bail error strategy to get quick feedback for correct queries.
	parser.setErrorHandler(std::make_shared\<BailErrorStrategy>());
	parser.getInterpreter\<ParserATNSimulator>()->setPredictionMode(PredictionMode::SLL);
	parser.removeErrorListeners();

	tree::ParseTree *tree;
	auto start = std::chrono::steady_clock::now();
	try {
		tree = parser.htmlDocument();
	} catch (ParseCancellationException &pce) {
    // If parsing was cancelled we either really have a syntax error or we need to do a second step,
    // now with the default strategy and LL parsing.
		tokens.reset();
		parser.reset();
		parser.setErrorHandler(std::make_shared\<DefaultErrorStrategy>());
		parser.getInterpreter\<ParserATNSimulator>()->setPredictionMode(PredictionMode::LL);
		parser.addErrorListener(&ConsoleErrorListener::INSTANCE);
		tree = parser.htmlDocument();
	}

	auto duration = std::chrono::duration_cast\<std::chrono::microseconds>(std::chrono::steady_clock::now() - start);

	if (parser.getNumberOfSyntaxErrors() > 0 || lexer.getNumberOfSyntaxErrors() > 0) {
		std::cout \<\< "Errors encountered: " \<\< parser.getNumberOfSyntaxErrors() + lexer.getNumberOfSyntaxErrors()\<\< std::endl;
	}

  /*if (duration.count() > 1000000)
    std::cout \<\< title \<\< duration.count() / 1000000.0 \<\< " s" \<\< std::endl;
  else if (duration.count() > 1000)
    std::cout \<\< title \<\< duration.count() / 1000.0 \<\< " ms" \<\< std::endl;
  else
    std::cout \<\< title \<\< duration.count() \<\< " µs" \<\< std::endl;*/

	std::cout \<\< title \<\< formatDuration(duration.count()) \<\< std::endl;

	if (dumpParseTree && tree != nullptr) {
		std::cout \<\< std::endl \<\< "Parse tree: " \<\< tree->toStringTree(&parser) \<\< std::endl;
	}
}

void profile(const std::string &sql, bool dumpParseTree) {
	ANTLRInputStream input(sql);
	HTMLLexer lexer(&input);
	CommonTokenStream tokens(&lexer);
	HTMLParser parser(&tokens);

	parser.setBuildParseTree(true);
	parser.setProfile(true);

	tree::ParseTree *tree;
	auto start = std::chrono::steady_clock::now();
	parser.getInterpreter\<ParserATNSimulator>()->setPredictionMode(PredictionMode::LL);
	tree = parser.htmlDocument();

	auto duration = std::chrono::duration_cast\<std::chrono::microseconds>(std::chrono::steady_clock::now() - start);

	if (parser.getNumberOfSyntaxErrors() > 0 || lexer.getNumberOfSyntaxErrors() > 0) {
		std::cout \<\< "Errors encountered: " \<\< parser.getNumberOfSyntaxErrors() + lexer.getNumberOfSyntaxErrors()\<\< std::endl;
		std::cout \<\< "Query: " \<\< sql \<\< std::endl;
	}

	std::cout \<\< "Profiling time: " \<\< duration.count() / 1000.0 \<\< " ms" \<\< std::endl;

	if (dumpParseTree && tree != nullptr) {
		std::cout \<\< std::endl \<\< "Parse tree: " \<\< tree->toStringTree(&parser) \<\< std::endl;
	}

	misc::IntervalSet test;
	auto parseInfo = parser.getParseInfo();
	std::cout \<\< "LL Decisions: ";
	for (size_t decision : parseInfo.getLLDecisions())
		std::cout \<\< decision \<\< " ";
	std::cout \<\< std::endl;

	std::cout \<\< "Total time in prediction: " \<\< parseInfo.getTotalTimeInPrediction() / 1000000.0 \<\< "ms" \<\< std::endl;
	std::cout \<\< "Total SLL lookahead ops: " \<\< parseInfo.getTotalSLLLookaheadOps() \<\< std::endl;
	std::cout \<\< "Total LL lookahead ops: " \<\< parseInfo.getTotalLLLookaheadOps() \<\< std::endl;
	std::cout \<\< "Total SLL ATN transitions: " \<\< parseInfo.getTotalSLLATNLookaheadOps() \<\< std::endl;
	std::cout \<\< "Total LL ATN transitions: " \<\< parseInfo.getTotalLLATNLookaheadOps() \<\< std::endl;
	std::cout \<\< "Total of all ATN transitions: " \<\< parseInfo.getTotalATNLookaheadOps() \<\< std::endl;
	std::cout \<\< "DFA size: " \<\< parseInfo.getDFASize() \<\< std::endl;
}

void parseFile(const std::string &fileName) {

	std::fstream input("/Volumes/Extern/Work/projects/antlr4-test/html/test-data/" + fileName);
	std::cout \<\< std::endl \<\< fileName \<\< std::endl;
	parse(input, "cold: ", false, false);
	parse(input, "1. warm: ", false, false);
	parse(input, "2. warm: ", false, false);
}

int main(int argc, const char * argv[]) {
	std::cout \<\< "Start" \<\< std::endl;

	auto start = std::chrono::steady_clock::now();

	parseFile("abc.com.html");
	parseFile("attvalues.html");
	parseFile("digg.html");
	parseFile("gnu.html");
	parseFile("metafilter.html");
	parseFile("reddit2.html");
	parseFile("uglylink.html");
	parseFile("aljazeera.com.html");
	parseFile("bbc.html");
	parseFile("freebsd.html");
	parseFile("google.html");
	parseFile("nbc.com.html");
	parseFile("script1.html");
	parseFile("wikipedia.html");
	parseFile("antlr.html");
	parseFile("cnn1.html");
	parseFile("github.html");
	parseFile("html4.html");
	parseFile("reddit.html");
	parseFile("style1.html");
	parseFile("youtube.html");

	auto duration = std::chrono::duration_cast\<std::chrono::milliseconds>(std::chrono::steady_clock::now() - start);
	std::cout \<\< std::endl \<\< "Total time: " \<\< duration.count() / 1000.0 \<\< " secs" \<\< std::endl;

	return 0;
}
