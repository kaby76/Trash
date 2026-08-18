<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/1999/xhtml"
                xmlns:h="http://www.w3.org/1999/xhtml"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:t="https://github.com/invisibleXML/ixml/test-catalog"
                expand-text="yes"
                exclude-result-prefixes="#all"
                version="3.0">

<xsl:import href="utils.xsl"/>

<xsl:output method="html" html-version="5" encoding="utf-8" indent="no"/>

<xsl:param name="fake-out-the-optimizer" select="'irrelevant-but-not-the-empty-sequence'"/>

<xsl:variable name="root"
              select="replace(base-uri(/), '^(.*/)[^/]+$', '$1')"/>

<xsl:template match="/">
  <xsl:variable name="expanded" select="t:expanded-catalog(t:test-catalog)"/>
  <xsl:if test="not(t:unique-names($expanded))">
    <xsl:message terminate='yes' select="'Fix test names.'"/>
  </xsl:if>

  <html>
    <head>
      <title>Invisible XML Test Suite Catalog</title>
      <link rel="stylesheet" href="css/catalog.css"/>
    </head>
    <body id="home">
      <div class="breadcrumbs">
        <img class="logotype" src="img/logotype_white.png"/>
      </div>
      <xsl:apply-templates select="$expanded/t:test-catalog"/>
    </body>
  </html>
</xsl:template>

<xsl:template match="t:test-catalog|t:test-set">
  <xsl:param name="path" as="xs:string" select="''"/>
  <xsl:param name="preamble" select="()"/>

  <xsl:variable name="level"
                select="min((count(ancestor::*),2))+1"/>

  <xsl:variable name="intro">
    <xsl:element name="h{$level}" namespace="http://www.w3.org/1999/xhtml">
      <xsl:attribute name="class" select="'title'"/>
      <xsl:sequence select="string(@name)"/>
    </xsl:element>
    <xsl:if test="@release-date">
      <xsl:element name="h{$level + 1}" namespace="http://www.w3.org/1999/xhtml">
        <xsl:attribute name="class" select="'reldate'"/>
        <xsl:sequence select="t:date(@release-date)"/>

        <xsl:if test="empty(parent::*)">
          <xsl:variable name="dates" as="xs:string*">
            <xsl:for-each select="//@release-date[. castable as xs:date]
                                  | //@on[. castable as xs:date]">
              <xsl:sort select="." order="descending"/>
              <xsl:sequence select="."/>
            </xsl:for-each>
          </xsl:variable>
          <xsl:variable name="most-recent" select="$dates[1]"/>
          <xsl:if test="@release-date != $most-recent">
            <xsl:text> ({t:date($most-recent)})</xsl:text>
          </xsl:if>
        </xsl:if>
      </xsl:element>
    </xsl:if>
    <xsl:apply-templates select="t:created|t:modified|t:description"/>
  </xsl:variable>

  <main>
    <img class="{if (empty(parent::*)) then 'logo' else 'logosm'}"
         src="{t:relative-root($path)}img/logo_gradient.png"/>
    <xsl:sequence select="$preamble"/>
    <xsl:sequence select="$intro"/>

    <xsl:if test="empty(parent::*)">
      <details class="note">
        <summary>A note about the catalog structure</summary>
        <p>This version of the catalog provides one HTML page for each
        test case and navigation pages from the root of the catalog.
        The page hierarchy is based on the logical structure
        of the test catalog, not the directory structure of the tests
        themselves. For context, the catalog(s) and set(s) to which each
        page belongs are repeated at the top of each page.</p>
      </details>
    </xsl:if>

    <xsl:if test="t:test-catalog|t:test-set|t:grammar-test|t:test-case">
      <div class="toc">
        <h4>Table of contents</h4>
        <ul>
          <xsl:apply-templates
              select="t:test-catalog|t:test-set|t:grammar-test|t:test-case"
              mode="t:toc">
            <xsl:with-param name="show-tests" select="exists(t:grammar-test|t:test-case)"/>
            <xsl:with-param name="path" select="$path"/>
          </xsl:apply-templates>
        </ul>
      </div>
    </xsl:if>

    <xsl:if test="empty(parent::*)">
      <div class="index">
        <h4>Test index</h4>
        <ul>
          <xsl:for-each select=".//t:test-case|.//t:grammar-test">
            <xsl:sort select="t:name(.) || ' ' || t:name(..)"/>
            <li>
              <a href="{t:filepath(.)}.html">{t:name(.)}</a>
              <xsl:text> ({t:name(..)})</xsl:text>
              <xsl:choose>
                <xsl:when test=".//t:assert-not-a-grammar
                                |.//t:assert-dynamic-error">
                  <span class="check-fail"> ✘</span>
                </xsl:when>
                <xsl:when test=".//t:assert-not-a-sentence">
                  <span class="check-ok"> ✘</span>
                </xsl:when>
                <xsl:otherwise>
                  <span class="check-ok"> ✔</span>
                </xsl:otherwise>
              </xsl:choose>
            </li>
          </xsl:for-each>
        </ul>
      </div>
    </xsl:if>
  </main>

  <xsl:for-each select="t:test-catalog|t:test-set">
    <xsl:variable name="path" select="t:filepath(.) || '/index.html'"/>
    <xsl:result-document href="{$path}">
      <html>
        <head>
          <title>Invisible XML Test Suite Catalog</title>
          <link rel="stylesheet" href="{t:relative-root($path)}css/catalog.css"/>
        </head>
        <body>
          <div class="breadcrumbs">
            <xsl:variable name="apath" select="t:filepath(.) || '/index.html'"/>
            <xsl:for-each select="ancestor::*">
              <xsl:choose>
                <xsl:when test="position() = 1">
                  <a href="{t:relative-root($apath)}{t:filepath(.)}">
                    <img class="logotype"
                         src="{t:relative-root($apath)}/img/logotype_white.png"/>
                  </a>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text> / </xsl:text>
                  <a href="{t:relative-root($apath)}{t:filepath(.)}">{string(@name)}</a>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>
            <xsl:text> / {string(@name)}</xsl:text>
          </div>
          <xsl:apply-templates select=".">
            <xsl:with-param name="path" select="$path"/>
            <xsl:with-param name="preamble">
              <xsl:sequence select="$preamble"/>
              <xsl:sequence select="$intro/*[@class='title']"/>
              <details>
                <xsl:if test="$intro/*[@class='reldate']">
                  <summary>
                    <xsl:sequence select="$intro/*[@class='reldate']/node()"/>
                  </summary>
                </xsl:if>
                <xsl:sequence select="$intro/*[not(@class='title' or @class='reldate')]"/>
              </details>
            </xsl:with-param>
          </xsl:apply-templates>
        </body>
      </html>
    </xsl:result-document>
  </xsl:for-each>

  <xsl:variable name="context"
                select="*[not(self::t:created
                          or self::t:modified
                          or self::t:description
                          or self::t:test-case
                          or self::t:grammar-test)]"/>

  <xsl:for-each select="t:grammar-test|t:test-case">
    <xsl:variable name="path" select="t:filepath(.) || '.html'"/>
    <xsl:result-document href="{$path}">
      <html>
        <head>
          <title>Invisible XML Test Suite Catalog</title>
          <link rel="stylesheet" href="{t:relative-root($path)}css/catalog.css"/>
        </head>
        <body>
          <div class="breadcrumbs">
            <xsl:for-each select="ancestor::*">
              <xsl:choose>
                <xsl:when test="position() = 1">
                  <a href="{t:relative-root($path)}{t:filepath(.)}">
                    <img class="logotype"
                         src="{t:relative-root($path)}/img/logotype_white.png"/>
                  </a>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text> / </xsl:text>
                  <a href="{t:relative-root($path)}{t:filepath(.)}">{string(@name)}</a>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>
            <xsl:text> / {t:name(.)}</xsl:text>
          </div>
          <main>
            <img class="logosm" src="{t:relative-root($path)}img/logo_gradient.png"/>
            <xsl:sequence select="$preamble"/>
            <xsl:sequence select="$intro"/>
            <xsl:apply-templates select="$context"/>
            <h1>Test case: {t:name(.)}</h1>
            <details>
              <p>Repository URI: <code>…/tests/{substring-after(base-uri(.), '/tests/')}</code></p>
            </details>
            <xsl:apply-templates/>
          </main>
        </body>
      </html>
    </xsl:result-document>
  </xsl:for-each>

</xsl:template>

<!-- ============================================================ -->

<xsl:template match="t:test-catalog|t:test-set" mode="t:toc">
  <!-- hack about a bit so that we can compute relative paths back
       to the root of the hierarchy so that the location on a web
       server doesn't matter. -->
  <xsl:param name="show-tests" as="xs:boolean"/>
  <xsl:param name="path" as="xs:string"/>

  <xsl:variable name="single"
                select="not(t:test-set) and count(t:test-case|t:grammar-test) = 1"/>

  <xsl:variable name="next-level" as="element()*">
    <xsl:choose>
      <xsl:when test="$show-tests and not($single)">
        <xsl:sequence select="t:test-catalog|t:test-set|t:grammar-test|t:test-case"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:sequence select="t:test-catalog|t:test-set"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>

  <xsl:variable name="anchor" as="element()">
    <xsl:choose>
      <xsl:when test="$single">
        <a href="{t:relative-root($path)}{t:filepath(t:test-case|t:grammar-test)}.html">
          <xsl:text>{string(@name)}</xsl:text>
        </a>
      </xsl:when>
      <xsl:otherwise>
        <a href="{t:relative-root($path)}{t:filepath(.)}/index.html">
          <xsl:text>{string(@name)}</xsl:text>
        </a>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>

  <li>
    <xsl:sequence select="$anchor"/>
    <xsl:if test="$next-level">
      <ul>
        <xsl:apply-templates select="$next-level" mode="t:toc">
          <xsl:with-param name="show-tests" select="$show-tests"/>
          <xsl:with-param name="path" select="$path"/>
        </xsl:apply-templates>
      </ul>
    </xsl:if>
  </li>
</xsl:template>

<xsl:template match="t:grammar-test|t:test-case" mode="t:toc">
  <!-- hack about a bit so that we can compute relative paths back
       to the root of the hierarchy so that the location on a web
       server doesn't matter. -->
  <xsl:param name="path" as="xs:string"/>

  <li>
    <a href="{t:relative-root($path)}{t:filepath(.)}.html">
      <xsl:text>{t:name(.)}</xsl:text>
    </a>
  </li>
</xsl:template>

<!-- ============================================================ -->

<xsl:template match="*">
  <xsl:message select="'No template for ' || local-name(.)"/>
  <div style="color:red;">
    <xsl:text>&lt;{node-name(.)}&gt;</xsl:text>
  </div>
  <xsl:apply-templates/>
  <div style="color:red;">
    <xsl:text>&lt;/{node-name(.)}&gt;</xsl:text>
  </div>
</xsl:template>

<xsl:template name="t:metadata">
  <div class="metadata">
    <xsl:apply-templates select="t:created"/>
    <xsl:variable name="mods"
                  select="reverse(sort(t:modified, (), function($mod) { $mod/@on }))"/>
    <xsl:apply-templates select="t:modified[1]">
      <xsl:with-param name="mods" select="$mods[position() gt 1]"/>
    </xsl:apply-templates>
  </div>
</xsl:template>

<xsl:template match="t:created">
  <p>Created {t:date(@on)} by {string(@by)}</p>
</xsl:template>

<xsl:template match="t:modified">
  <xsl:param name="mods" as="element(t:modified)*"/>

  <details>
    <summary>Updated {t:date(@on)} by {string(@by)}</summary>
    <xsl:if test="@change">
      <p>{string(@change)}</p>
    </xsl:if>
    <xsl:apply-templates select="$mods[1]">
      <xsl:with-param name="mods" select="$mods[position() gt 1]"/>
    </xsl:apply-templates>
  </details>
</xsl:template>

<xsl:template match="t:dependencies[not(preceding-sibling::t:dependencies)]">
  <xsl:variable name="uver" as="xs:string*">
    <xsl:for-each select="../t:dependencies/@Unicode-version/string()">
      <xsl:sort select="." data-type="number"/>
      <xsl:sequence select="."/>
    </xsl:for-each>
  </xsl:variable>
  <xsl:variable name="other" select="../t:dependencies/@* except ../t:dependencies/@Unicode-version"/>

  <xsl:if test="not(empty($other))">
    <xsl:message select="'Unexpected attributes on t:dependencies element'"/>
  </xsl:if>

  <xsl:choose expand-text="yes">
    <xsl:when test="empty($uver)"/>
    <xsl:when test="count($uver) = 1">
      <p>Depends on Unicode version {$uver}.</p>
    </xsl:when>
    <xsl:when test="count($uver) = 2">
      <p>Depends on Unicode version {$uver[1]} or {$uver[2]}.</p>
    </xsl:when>
    <xsl:otherwise>
      <p>
        <xsl:text>Depends on Unicode version </xsl:text>
        <xsl:for-each select="$uver">
          <xsl:if test="position() gt 1">, </xsl:if>
          <xsl:if test="position() eq last()">or </xsl:if>
          <xsl:sequence select="."/>
        </xsl:for-each>
        <xsl:text>.</xsl:text>
      </p>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

<xsl:template match="t:dependencies[preceding-sibling::t:dependencies]"/>

<xsl:template match="t:ixml-grammar-ref">
  <div class="grammar">
    <h4>Invisible XML Grammar</h4>
    <xsl:try>
      <pre>
        <code>
          <xsl:sequence select="unparsed-text(resolve-uri(@href, base-uri(.)))"/>
        </code>
      </pre>
      <xsl:catch>
        <p><em>Input cannot be inlined in XML.</em>
        <xsl:value-of select="@href"/></p>
      </xsl:catch>
    </xsl:try>
  </div>
</xsl:template>

<xsl:template match="t:vxml-grammar-ref">
  <div class="grammar">
    <h4>Invisible XML Grammar</h4>
    <xsl:try>
      <xsl:variable name="xml" select="doc(resolve-uri(@href, base-uri(.)))"/>
      <pre>
        <code>
          <xsl:sequence select="serialize($xml,
                                map { 'method': 'xml',
                                      'omit-xml-declaration': true(),
                                      'indent': true() })"/>
        </code>
      </pre>
      <xsl:catch>
        <p><em>Input cannot be inlined in XML.</em></p>
      </xsl:catch>
    </xsl:try>
  </div>
</xsl:template>

<xsl:template match="t:ixml-grammar">
  <div class="grammar">
    <h4>Invisible XML Grammar</h4>
    <pre>
      <code>
        <xsl:sequence select="string(.)"/>
      </code>
    </pre>
  </div>
</xsl:template>

<xsl:template match="t:test-string|t:test-string-ref">
  <!-- Work out if the string was readable, bearing in mind that we have
       to defeat the Saxon optimizer so that it'll actually evaluate the
       unparsed-text() call!!!
  -->
  <xsl:variable name="ok-in-xml" as="xs:boolean">
    <xsl:try>
      <xsl:choose>
        <xsl:when test="self::t:test-string">
          <xsl:sequence select="true()"/>
        </xsl:when>
        <!-- a test that will force the Saxon to read the file. -->
        <xsl:when test="contains(unparsed-text(resolve-uri(@href, base-uri(.))), 'X')">
          <xsl:sequence select="true()"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:sequence select="exists($fake-out-the-optimizer)"/>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:catch>
        <xsl:sequence select="false()"/>
      </xsl:catch>
    </xsl:try>
  </xsl:variable>

  <xsl:variable name="input" as="xs:string">
    <xsl:try>
      <xsl:sequence select="if (self::t:test-string)
                            then string(.)
                            else unparsed-text(resolve-uri(@href, base-uri(.)))"/>
      <xsl:catch>
        <xsl:sequence select="''"/>
      </xsl:catch>
    </xsl:try>
  </xsl:variable>

  <div class="test-string">
    <h4>
      <xsl:choose>
        <xsl:when test="$ok-in-xml">
          <xsl:text>Input string ({string-length($input)}</xsl:text>
          <xsl:text> character{if (string-length($input) ne 1) then 's' else ()})</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>Input string</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </h4>
    <pre>
      <code>
        <xsl:try>
          <xsl:sequence select="if (self::t:test-string)
                                then string(.)
                                else unparsed-text(resolve-uri(@href, base-uri(.)))"/>
          <xsl:catch>
            <!-- We can only get here if there was a URI... -->
            <xsl:variable name="uri" select="resolve-uri(@href, base-uri(.))"/>
            <xsl:variable name="path" select="substring-after($uri, '/tests/')"/>
            <p>
              <em>
                <a href="https://github.com/invisibleXML/ixml/blob/master/tests/{$path}">
                  <xsl:text>String value</xsl:text>
                </a>
                <xsl:text> cannot be inlined in web page.</xsl:text>
              </em>
            </p>
          </xsl:catch>
        </xsl:try>
      </code>
    </pre>
  </div>
</xsl:template>

<xsl:template match="t:assert-xml-ref">
  <div class="xml-result">
    <xsl:try>
      <xsl:variable name="xml" select="doc(resolve-uri(@href, base-uri(.)))"/>
      <pre>
        <code>
          <xsl:sequence select="serialize($xml,
                                map { 'method': 'xml',
                                      'omit-xml-declaration': true(),
                                      'indent': true() })"/>
        </code>
      </pre>
      <xsl:catch>
        <p><em>Input cannot be inlined in XML.</em></p>
      </xsl:catch>
    </xsl:try>
  </div>
</xsl:template>

<xsl:template match="t:assert-xml">
  <div class="xml-result">
    <pre>
      <code>
        <xsl:sequence select="serialize(*,
                              map { 'method': 'xml',
                                    'omit-xml-declaration': true(),
                                    'indent': true() })"/>
      </code>
    </pre>
  </div>
</xsl:template>

<xsl:template match="t:assert-dynamic-error">
  <p class="dynamic-error">
    <xsl:text>Raises a dynamic error</xsl:text>
    <xsl:call-template name="t:error-codes"/>
    <xsl:text>.</xsl:text>
  </p>
</xsl:template>

<xsl:template match="t:assert-not-a-sentence">
  <p class="not-a-sentence">The input does not match the grammar.</p>
</xsl:template>

<xsl:template match="t:assert-not-a-grammar">
  <p class="not-a-grammar">
    <xsl:text>The grammar is invalid. Raises a static error</xsl:text>
    <xsl:call-template name="t:error-codes"/>
    <xsl:text>.</xsl:text>
  </p>
</xsl:template>

<xsl:template name="t:error-codes">
  <xsl:variable name="codes" select="tokenize(@error-code, '\s+')"/>
  <xsl:choose>
    <xsl:when test="empty($codes)"/>
    <xsl:when test="count($codes) eq 1">
      <xsl:if test="normalize-space($codes) != 'none'">
        <xsl:text>: </xsl:text>
        <xsl:sequence select="t:error-code($codes)"/>
      </xsl:if>
    </xsl:when>
    <xsl:otherwise>
      <xsl:text>: one of </xsl:text>
      <xsl:for-each select="$codes">
        <xsl:if test="position() gt 1">, </xsl:if>
        <xsl:sequence select="t:error-code(.)"/>
      </xsl:for-each>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

<!-- ============================================================ -->

<xsl:template match="t:result">
  <div>
    <xsl:attribute name="class">
      <xsl:choose>
        <xsl:when test="t:assert-not-a-grammar|t:assert-not-a-sentence|t:assert-dynamic-error">
          <xsl:sequence select="'result unsuccessful'"/>
        </xsl:when>
        <xsl:otherwise>result</xsl:otherwise>
      </xsl:choose>
    </xsl:attribute>
    <h4>Expected result{if (count(*) gt 1) then 's (one of)' else ()}</h4>
    <xsl:apply-templates/>
  </div>
</xsl:template>

<xsl:template match="t:app-info">
  <div class="app-info">
    <h4>Application specific extension</h4>
    <xsl:apply-templates select="t:options"/>
    <xsl:if test="not(empty(*[not(self::t:options)]))">
      <div>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="t:assert-not-a-grammar|t:assert-not-a-sentence|t:assert-dynamic-error">
              <xsl:sequence select="'result unsuccessful'"/>
            </xsl:when>
            <xsl:otherwise>result</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <h4>Expected result{if (count(*[not(self::t:options)]) gt 1) then 's (one of)' else ()}</h4>
        <xsl:apply-templates select="*[not(self::t:options)]"/>
      </div>
    </xsl:if>
  </div>
</xsl:template>

<xsl:template match="t:options">
  <div class="app-info-options">
    <ul>
      <xsl:for-each select="@*">
        <li>
          <code>
            <xsl:text>Q{{</xsl:text>
            <xsl:sequence select="namespace-uri(.)"/>
            <xsl:text>}}{local-name(.)}</xsl:text>
          </code>
          <xsl:text> = </xsl:text>
          <code>
            <xsl:sequence select="string(.)"/>
          </code>
        </li>
      </xsl:for-each>
    </ul>
  </div>
</xsl:template>

<xsl:template match="t:description">
  <div class="description">
    <xsl:apply-templates mode="html"/>
  </div>
</xsl:template>

<!-- ============================================================ -->

<xsl:template match="*" mode="html">
  <xsl:element name="{local-name(.)}" namespace="http://www.w3.org/1999/xhtml">
    <xsl:copy-of select="@*"/>
    <xsl:apply-templates mode="html"/>
  </xsl:element>
</xsl:template>

<!-- ============================================================ -->

</xsl:stylesheet>
