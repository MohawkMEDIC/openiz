<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl linq openiz"
                xmlns:linq="http://schemas.microsoft.com/linqtosql/dbml/2007"
                xmlns="http://schemas.microsoft.com/linqtosql/dbml/2007"
                xmlns:openiz="http://openiz.org/other/xml"
>
    <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>


  <msxsl:script language="cs" implements-prefix="openiz">
    <![CDATA[
    public String CorrectAssociationName(String associationName, String typeName, String memberName, String thisKey, String otherKey)
    {
      if(typeName == "ActTag")
        System.Diagnostics.Debugger.Break();
        
      String preferredName = memberName;
      // Member is another class pointing at me
      if(otherKey != thisKey)
      {
        //if(associationName.StartsWith(typeName) && otherKey.EndsWith("Id"))
        //  preferredName = otherKey;
        if(thisKey.EndsWith("Id"))
          preferredName = thisKey.Substring(0, thisKey.Length - 2);
        else
          preferredName = thisKey + "Entity";
          
        if(preferredName == typeName && otherKey.EndsWith("Id"))
          preferredName = otherKey.Substring(0, otherKey.Length - 2);
      }
      return preferredName;
    }
    ]]>
  </msxsl:script>
  <xsl:template match="linq:Association">
    <xsl:choose>

      <xsl:when test="@IsForeignKey">
        <Association Name="{@Name}" Member="{openiz:CorrectAssociationName(@Name, ../../linq:Type/@Name, @Member, @ThisKey, @OtherKey)}" ThisKey="{@ThisKey}" OtherKey="{@OtherKey}" Type="{@Type}" IsForeignKey="{@IsForeignKey}">
          <xsl:if test="@Cardinality">
            <xsl:attribute name="Cardinality">
              <xsl:value-of select="@Cardinality"/>
            </xsl:attribute>
          </xsl:if>
        </Association>
      </xsl:when>
      <xsl:otherwise>
        
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>
  
    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
