<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:old-c="https://github.com/cmsmcq/ixml-tests"
                xmlns:c="https://github.com/invisibleXML/ixml/test-catalog"
                exclude-result-prefixes="#all"
                expand-text="yes"
                version="3.0">

<!-- This stylesheet recursively processes an ixml catalog file
     testing that href values it contains are correct pointers to files
     in the filesystem. -->

<xsl:output method="text" encoding="utf-8"/>

<xsl:param name="show-files" select="'false'"/>

<xsl:variable name="NL" select="'&#10;'"/>

<xsl:mode on-no-match="shallow-copy"/>

<xsl:template match="/">
  <xsl:if test="not(c:test-catalog)">
    <xsl:message terminate="yes" select="'Expected a test catalog, got: ' || local-name(.)"/>
  </xsl:if>
  <xsl:apply-templates select="*"/>
</xsl:template>

<xsl:template match="c:test-catalog|c:test-set|c:test-case|c:grammar-test|c:result">
  <xsl:apply-templates select="*"/>
</xsl:template>

<xsl:template match="c:test-set-ref">
  <xsl:if test="$show-files = 'true'">
    <xsl:text>{@href}{$NL}</xsl:text>
  </xsl:if>
  <xsl:choose>
    <xsl:when test="doc-available(resolve-uri(@href, base-uri(.)))">
      <xsl:apply-templates select="doc(resolve-uri(@href, base-uri(.)))"/>
    </xsl:when>
    <xsl:otherwise>
      <xsl:text>Missing test-set-ref: {resolve-uri(@href, base-uri(.))}{$NL}</xsl:text>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

<xsl:template match="c:assert-xml-ref|c:vxml-grammar-ref">
  <xsl:if test="$show-files = 'true'">
    <xsl:text>{@href}{$NL}</xsl:text>
  </xsl:if>
  <xsl:if test="not(doc-available(resolve-uri(@href, base-uri(.))))">
    <xsl:text>Missing {local-name(.)}: {resolve-uri(@href, base-uri(.))}{$NL}</xsl:text>
  </xsl:if>
</xsl:template>

<xsl:template match="c:ixml-grammar-ref|c:test-string-ref">
  <xsl:if test="$show-files = 'true'">
    <xsl:text>{@href}{$NL}</xsl:text>
  </xsl:if>
  <xsl:if test="not(unparsed-text-available(resolve-uri(@href, base-uri(.))))">
    <xsl:text>Missing {local-name(.)}: {resolve-uri(@href, base-uri(.))}{$NL}</xsl:text>
  </xsl:if>
</xsl:template>

<xsl:template match="c:description|c:created|c:modified
                     |c:assert-not-a-sentence|c:assert-not-a-grammar"/>

<xsl:template match="*">
  <xsl:message select="'??? ' || local-name(.)"/>
</xsl:template>

</xsl:stylesheet>
