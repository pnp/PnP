<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <xsl:output method="text" indent="no" />
  <xsl:param name="now" />
  
<xsl:template match="/">
#PnP Provisioning Schema
----------
*Topic automatically generated on <xsl:value-of select="$now" />*

##Namespace
The namespace of the PnP Provisioning Schema is:

<xsl:value-of select="/xsd:schema/@targetNamespace" />

All the elements have to be declared with that namespace reference.

##Root Elements
Here follows the list of root elements available in the PnP Provisioning Schema.
  
<xsl:for-each select="xsd:schema/xsd:element">

<!-- Save the current type, which will be something like pnp:complexType -->
<xsl:variable name="currentType" select="substring(@type, 5)" />

<xsl:call-template name="RenderComplexType">
<xsl:with-param name="complexType" select="/xsd:schema/xsd:complexType[@name = $currentType]" />
<xsl:with-param name="renderTitle" select="number(0)" />
</xsl:call-template>
  
</xsl:for-each>

##Child Elements and Complex Types
Here follows the list of all the other child elements and complex types that can be used in the PnP Provisioning Schema.
<xsl:for-each select="/xsd:schema/xsd:complexType/child::*/xsd:element[not(@type)]/xsd:complexType | /xsd:schema/xsd:complexType[@name != 'Provisioning' and @name != 'ProvisioningTemplate']">
<xsl:call-template name="RenderComplexType">
<xsl:with-param name="complexType" select="." />
</xsl:call-template>
</xsl:for-each>

</xsl:template>

<xsl:template name="RenderComplexType">
<xsl:param name="complexType" />

<!-- Skip any abstract complexType -->
<xsl:if test="not(/xsd:schema/xsd:complexType//xsd:extension[@base = $complexType/@name])">
    
<!-- Get the complexType name -->
<xsl:if test="$complexType/@name | $complexType/parent::xsd:element/@name">
<xsl:variable name="typeName">
<xsl:choose>
<xsl:when test="$complexType/@name">
<xsl:value-of select="$complexType/@name" />
</xsl:when>
<xsl:otherwise>
<xsl:value-of select="$complexType/parent::xsd:element/@name" />
</xsl:otherwise>
</xsl:choose> 
</xsl:variable>

<!-- Create an anchor to the current complexType -->
<xsl:call-template name="LinkXmlTag"><xsl:with-param name="tagName" select="$typeName" /></xsl:call-template>
###<xsl:value-of select="$typeName"/><xsl:call-template name="CRLF" />

<!-- Write the complexType description -->
<xsl:variable name="documentation">
<xsl:choose>
<xsl:when test="$complexType/xsd:annotation/xsd:documentation != ''">
<xsl:value-of select="$complexType/xsd:annotation/xsd:documentation"/>
</xsl:when>
<xsl:when test="$complexType/parent::xsd:element/xsd:annotation/xsd:documentation != ''">
<xsl:value-of select="$complexType/parent::xsd:element/xsd:annotation/xsd:documentation"/>
</xsl:when>
<xsl:otherwise>
</xsl:otherwise>
</xsl:choose>
</xsl:variable>
<xsl:if test="$documentation">
<xsl:value-of select="normalize-space($documentation)" /><xsl:call-template name="CRLF" />
</xsl:if>

<!-- Save the current type of the complexType, which will be something like pnp:complexType -->
<xsl:variable name="currentType" select="substring($complexType/@type, 5)" />
<!-- Save the child elements of the current complexType -->
<xsl:variable name="currentTypeElements" select="$complexType/child::*/xsd:element | $complexType/child::*/child::*/xsd:element" />
<!-- Save the attributes of the current complexType -->
<xsl:variable name="currentTypeAttributes" select="$complexType/xsd:attribute | /xsd:schema/xsd:complexType[@name = $complexType//xsd:extension/@base]/xsd:attribute | /xsd:schema/xsd:attributeGroup[@name = substring(/xsd:schema/xsd:complexType[@name = $complexType//xsd:extension/@base]/xsd:attributeGroup/@ref, 5)]/xsd:attribute | $complexType//xsd:extension/xsd:attribute" />
  
<!-- Show an XML preview of the element -->
```xml
<xsl:call-template name="OpenXmlTag"><xsl:with-param name="tagName" select="$typeName" /><xsl:with-param name="attributes" select="$currentTypeAttributes" /></xsl:call-template>
<xsl:call-template name="CRLF" />
<!-- If there are any child elements in the complexType, show them in a table -->
<xsl:for-each select="$currentTypeElements">
<xsl:text>   </xsl:text><xsl:call-template name="SelfClosingSimpleXmlTag"><xsl:with-param name="tagName" select="./@name" /></xsl:call-template>
<xsl:call-template name="CRLF" />
</xsl:for-each>
<!-- If the current complexType includes an xsd:any element -->
<xsl:if test="$complexType/child::*/xsd:any">
<xsl:text disable-output-escaping="yes">   &lt;!-- Any other XML content --&gt;</xsl:text>
<xsl:call-template name="CRLF" />
</xsl:if>
<xsl:call-template name="CloseXmlTag"><xsl:with-param name="tagName" select="$typeName" /></xsl:call-template>
```

<!-- If there are any child elements in the complexType, show them in a table -->
<xsl:if test="$currentTypeElements">
Here follow the available child elements for the <xsl:value-of select="@name"/> element.<xsl:call-template name="CRLF" />

Element|Type|Description
-------|----|-----------
<xsl:for-each select="$currentTypeElements">

<!-- Get the name of the current child element -->
<xsl:variable name="currentElementName" select="./@name" />

<!-- Get the type of the current child element -->
<xsl:variable name="currentElementType">
<xsl:choose>
<xsl:when test="./@type and substring(./@type, 1, 4) = 'pnp:'">
<xsl:value-of select="substring(./@type, 5)"/>
</xsl:when>
<xsl:when test="./@type">
<xsl:value-of select="./@type"/>
</xsl:when>
<xsl:otherwise>
<xsl:value-of select="./@name"/>
</xsl:otherwise>
</xsl:choose>

</xsl:variable>

<!-- Show the current child element -->
<xsl:choose>
<xsl:when test="substring($currentElementType, 1, 4) = 'xsd:'">
<xsl:value-of select="$currentElementName"/><xsl:text>|</xsl:text><xsl:value-of select="$currentElementType"/><xsl:text>|</xsl:text><xsl:if test="./xsd:annotation/xsd:documentation != ''"><xsl:value-of select="normalize-space(./xsd:annotation/xsd:documentation)" /></xsl:if><xsl:call-template name="CRLF" />
</xsl:when>
<xsl:otherwise>
<xsl:value-of select="$currentElementName"/><xsl:text>|</xsl:text><xsl:text>[</xsl:text><xsl:value-of select="$currentElementType"/><xsl:text>](#</xsl:text><xsl:value-of select="translate($currentElementType, 'ABCDEFGHILMNOPQRSTUVZWYJKX', 'abcdefghilmnopqrstuvzwyjkx')" /><xsl:text>)|</xsl:text><xsl:if test="./xsd:annotation/xsd:documentation != ''"><xsl:value-of select="normalize-space(./xsd:annotation/xsd:documentation)" /></xsl:if><xsl:call-template name="CRLF" />
</xsl:otherwise>
</xsl:choose>
</xsl:for-each>
</xsl:if>

<!-- If there are any attributes in the complexType, show them in a table -->
<xsl:if test="$currentTypeAttributes">
Here follow the available attributes for the <xsl:value-of select="@name"/> element.<xsl:call-template name="CRLF" />

Attibute|Type|Description
--------|----|-----------
<xsl:for-each select="$currentTypeAttributes">

<!-- Determine the attribute type -->  
<xsl:variable name="attibuteType">
<xsl:choose>
<xsl:when test="substring(./@type, 1, 4) = 'pnp:'">
<xsl:value-of select="substring(./@type, 5)"/>
</xsl:when>
<xsl:otherwise>
<xsl:value-of select="./@type"/>
</xsl:otherwise>
</xsl:choose>
</xsl:variable>
  
<xsl:value-of select="./@name"/>|<xsl:value-of select="$attibuteType"/>|<xsl:if test="./xsd:annotation/xsd:documentation != ''">
<xsl:value-of select="normalize-space(./xsd:annotation/xsd:documentation)" />
</xsl:if><xsl:call-template name="CRLF" />
</xsl:for-each>
</xsl:if>

</xsl:if>
</xsl:if>

</xsl:template>

<!-- Utility Templates -->
  
<xsl:template name="CRLF">
<xsl:text>&#xD;&#xA;</xsl:text>
</xsl:template>

<xsl:template name="OpenXmlTag">
<xsl:param name="tagName" />
<xsl:param name="attributes" />
<xsl:text disable-output-escaping="yes">&lt;</xsl:text><xsl:value-of select="$tagName"/><xsl:for-each select="$attributes"><xsl:call-template name="CRLF" /><xsl:text>      </xsl:text><xsl:value-of select="./@name"/>="<xsl:value-of select="./@type"/>"</xsl:for-each><xsl:text disable-output-escaping="yes">&gt;</xsl:text>
</xsl:template>

<xsl:template name="CloseXmlTag">
<xsl:param name="tagName" />
<xsl:text disable-output-escaping="yes">&lt;/</xsl:text><xsl:value-of select="$tagName"/><xsl:text disable-output-escaping="yes">&gt;</xsl:text>
</xsl:template>

<xsl:template name="SelfClosingSimpleXmlTag">
<xsl:param name="tagName" />
<xsl:text disable-output-escaping="yes">&lt;</xsl:text><xsl:value-of select="$tagName"/><xsl:text disable-output-escaping="yes"> /&gt;</xsl:text>
</xsl:template>

<xsl:template name="LinkXmlTag">
<xsl:param name="tagName" />
<xsl:text disable-output-escaping="yes">&lt;a name="</xsl:text><xsl:value-of select="translate($tagName, 'ABCDEFGHILMNOPQRSTUVZWYJKX', 'abcdefghilmnopqrstuvzwyjkx')"/><xsl:text disable-output-escaping="yes">"&gt;&lt;/a&gt;</xsl:text>
</xsl:template>

</xsl:stylesheet>