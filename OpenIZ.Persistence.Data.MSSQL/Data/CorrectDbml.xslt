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
    public String CorrectAssociationName(String associationName, String typeName, String memberName, String thisKey, String otherKey, Boolean isForeignKey)
    {
      if(typeName == "SecurityUser")
        System.Diagnostics.Debugger.Break();
        
      String preferredName = memberName;
      // Member is another class pointing at me
      System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\d");
      if(otherKey != thisKey)
      {
        if(associationName.StartsWith(typeName) && !isForeignKey)
        {
          preferredName = regex.Replace(preferredName, "") + otherKey;
        }
        else if(thisKey.EndsWith("Id"))
          preferredName = thisKey.Substring(0, thisKey.Length - 2);
        else
          preferredName = thisKey + "Entity";
          
        if(preferredName == typeName && otherKey.EndsWith("Id"))
          preferredName = otherKey.Substring(0, otherKey.Length - 2);
      }
      else
        preferredName = regex.Replace(preferredName, "");
      return preferredName;
    }
    ]]>
  </msxsl:script>
  <xsl:template match="linq:Column">
    <xsl:copy>
      <xsl:apply-templates select="@*"/>
      <xsl:if test="@IsPrimaryKey = 'true' or @Name = 'CreationTime'">
        <xsl:attribute name="IsDbGenerated">
          <xsl:value-of select="'true'"/> 
        </xsl:attribute>
      </xsl:if>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="linq:Association">
        <Association Name="{@Name}" Member="{openiz:CorrectAssociationName(@Name, ../../linq:Type/@Name, @Member, @ThisKey, @OtherKey, @IsForeignKey)}" ThisKey="{@ThisKey}" OtherKey="{@OtherKey}" Type="{@Type}">
          <xsl:if test="@Cardinality">
            <xsl:attribute name="Cardinality">
              <xsl:value-of select="@Cardinality"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:if test="@IsForeignKey">
            <xsl:attribute name="IsForeignKey">
              <xsl:value-of select="@IsForeignKey"/>
            </xsl:attribute>
          </xsl:if>
        </Association>

  </xsl:template>
  
    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
