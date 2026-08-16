This directory contains a version of Steven Pemberton's tests.zip
file, modified by Michael Sperberg-McQueen, further modified by Norm
Tovey-Walsh, and packaged with a test catalog using the test-catalog
vocabulary defined in MSM's ixml-tests repository.

Note that a large number of tests have been changed vis-a-vis
tests.zip.  In many cases this only involved removing non-grammatical
whitespace from the end of the input, but in other cases larger
interventions were made.  The details are in the catalog.

In each directory, there are three files per test:
* file.ixml: the ixml grammar for the test
* file.inp: sample input for that grammar
* file.req: the expected output for that input

MSM has made test catalogs for these tests using the test-catalog vocabulary
defined in his ixml-tests repository.

* SP-syntaxtests-package.zip
* tests-SP-MSM

For the syntaxtests directory, the result is packaged in a zip file
containing the tests and catalogs.  (No input files or result files
are included, because none are needed: none of the tests consume input
other than the grammar files, and none produces output.)

The SP-syntaxtests-package.zip file contains three catalogs, which
specify the tests in different ways:

* In `catalog-as-instance-tests-ixml.xml`, the catalog specifies the
tests as instance tests, using `../../../ixml.ixml` as the grammar
against which they are to be parsed.

* In `catalog-as-instance-tests-xml.xml`, the catalog specifies the
tests as instance tests, using `../../../ixml.xml` as the grammar
against which they are to be parsed.  Processors which don't support
the XML form of ixml grammars won't want to bother running these.

* In `catalog-as-grammar-tests.xml`, the processor is to use its
inbuilt ixml grammar.  Assuming the processor is using the current
ixml grammar, the results should be the same.

In the directory tests-SP-MSM, a test catalog is packaged with
corrected input and result files.  As noted case by case in the
catalog, a number of changes were made, often stripping ungrammatical
whitespace from the input files and in some case stripping
nonsignficant whitespace from the expected result, to avoid causing
problems for XML comparators using deep-equal(), or for other
comparators looking at output with different pretty-printing
practices.

