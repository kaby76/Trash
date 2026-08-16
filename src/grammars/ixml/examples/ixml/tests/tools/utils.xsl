<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/1999/xhtml"
                xmlns:h="http://www.w3.org/1999/xhtml"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:t="https://github.com/invisibleXML/ixml/test-catalog"
                expand-text="yes"
                exclude-result-prefixes="#all"
                version="3.0">

<xsl:function name="t:date" as="xs:string">
  <xsl:param name="date" as="xs:string"/>

  <xsl:choose>
    <xsl:when test="$date castable as xs:dateTime">
      <xsl:sequence select="format-dateTime(xs:dateTime($date),
                            '[D01] [MNn,*-3] [Y0001]')"/>
    </xsl:when>
    <xsl:when test="$date castable as xs:date">
      <xsl:sequence select="format-date(xs:date($date),
                            '[D01] [MNn,*-3] [Y0001]')"/>
    </xsl:when>
    <xsl:otherwise>
      <xsl:sequence select="$date"/>
    </xsl:otherwise>
  </xsl:choose>
</xsl:function>

<xsl:function name="t:error-code">
  <xsl:param name="code" as="xs:string"/>
  <xsl:choose>
    <xsl:when test="matches($code, '^[DS]\d+')">
      <a href="https://invisiblexml.org/ixml-specification.html#ref-{lower-case($code)}">
        <xsl:sequence select="$code"/>
      </a>
    </xsl:when>
    <xsl:otherwise>
      <xsl:sequence select="$code"/>
    </xsl:otherwise>
  </xsl:choose>
</xsl:function>

<xsl:function name="t:name" as="xs:string">
  <xsl:param name="test" as="element()"/>
  <xsl:choose>
    <xsl:when test="$test/@name">
      <xsl:sequence select="string($test/@name)"/>
    </xsl:when>
    <xsl:when test="$test/self::t:grammar-test">
      <xsl:sequence select="'Grammar test'"/>
    </xsl:when>
    <xsl:otherwise>
      <xsl:message>Attempt to name unexpected element: {local-name($test)}</xsl:message>
      <xsl:sequence select="'???'"/>
    </xsl:otherwise>
  </xsl:choose>
</xsl:function>

<xsl:function name="t:filename" as="xs:string">
  <xsl:param name="ctx" as="element()"/>

  <xsl:choose>
    <xsl:when test="$ctx/self::t:grammar-test">
      <xsl:sequence select="'grammar-test'"/> <!-- hack -->
    </xsl:when>
    <xsl:when test="not($ctx/@name)">
      <xsl:message terminate="yes"
                   select="'Cannot create name for:', $ctx"/>
    </xsl:when>
    <xsl:otherwise>
      <xsl:sequence select="normalize-space($ctx/@name)
                            =&gt; translate(' .()', '__')
                            =&gt; lower-case()"/>
    </xsl:otherwise>
  </xsl:choose>
</xsl:function>

<xsl:function name="t:filepath" as="xs:string">
  <xsl:param name="ctx" as="element()"/>
  <xsl:variable name="path" select="($ctx/ancestor-or-self::*)[position() gt 1]"/>
  <xsl:sequence select="string-join($path ! t:filename(.), '/')"/>
</xsl:function>

<xsl:function name="t:relative-root" as="xs:string">
  <xsl:param name="path" as="xs:string"/>

  <xsl:variable name="segments-to-root"
                select="count(tokenize($path, '/'))"/>
  <xsl:variable name="prefix-segs" as="xs:string*">
    <xsl:for-each select="2 to $segments-to-root">
      <xsl:sequence select="'..'"/>
    </xsl:for-each>
  </xsl:variable>
  <xsl:sequence select="if (empty($prefix-segs))
                        then ''
                        else string-join($prefix-segs, '/') || '/'"/>
</xsl:function>

<!-- ============================================================ -->

<xsl:function name="t:expanded-catalog" as="document-node()"
              xmlns:tp="https://github.com/invisibleXML/ixml/test-catalog">
  <xsl:param name="catalog" as="element(t:test-catalog)"/>
  <xsl:document>
    <xsl:apply-templates select="$catalog" mode="tp:expand"/>
  </xsl:document>
</xsl:function>

<xsl:mode name="tp:expand" on-no-match="shallow-copy"
          xmlns:tp="https://github.com/invisibleXML/ixml/test-catalog"/>

<xsl:template match="t:test-catalog" mode="tp:expand"
              xmlns:tp="https://github.com/invisibleXML/ixml/test-catalog">
  <xsl:copy>
    <xsl:copy-of select="@*"/>
    <xsl:attribute name="xml:base" select="base-uri(.)"/>
    <xsl:apply-templates mode="tp:expand"/>
  </xsl:copy>
</xsl:template>

<xsl:template match="t:test-set-ref" mode="tp:expand"
              xmlns:tp="https://github.com/invisibleXML/ixml/test-catalog">
  <xsl:apply-templates select="doc(resolve-uri(@href, base-uri(.)))"
                       mode="tp:expand"/>
</xsl:template>

<!-- ============================================================ -->

<xsl:function name="t:unique-names" as="xs:boolean">
  <xsl:param name="catalog" as="node()"/>

  <xsl:variable name="set-names"
                select="$catalog//t:test-set ! t:filename(.)"/>

  <xsl:variable name="dup-set" as="xs:string*">
    <xsl:for-each select="$set-names">
      <xsl:variable name="name" select="."/>
      <xsl:if test="count($set-names[. = $name]) ne 1">
        <xsl:message select="'Duplicate test set name: ' || $name"/>
        <xsl:sequence select="$name"/>
      </xsl:if>
    </xsl:for-each>
  </xsl:variable>

  <xsl:variable name="dup-case" as="xs:string*">
    <xsl:for-each select="$catalog//t:test-set">
      <xsl:variable name="set" select="."/>
      <xsl:variable name="case-names"
                    select="(./t:grammar-test|./t:test-case) ! t:filename(.)"/>

      <xsl:for-each select="$case-names">
        <xsl:variable name="name" select="."/>
        <xsl:if test="count($case-names[. = $name]) ne 1">
          <xsl:message select="'Duplicate test case name: ' || $name
                               || ' in ' || t:filepath($set)"/>
          <xsl:sequence select="$name"/>
        </xsl:if>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:variable>

  <xsl:sequence select="empty($dup-set) and empty($dup-case)"/>
</xsl:function>

</xsl:stylesheet>
