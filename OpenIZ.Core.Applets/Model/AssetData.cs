/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// A class representing data operations
    /// </summary>
    [XmlType(nameof(AssetData), Namespace = "http://openiz.org/applet")]
    public class AssetData
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public AssetData()
        {
            this.Action = new List<AssetDataActionBase>();
        }

        /// <summary>
        /// Actions to be performed
        /// </summary>
        [XmlElement("insert", Type = typeof(AssetDataInsert))]
        [XmlElement("obsolete", Type = typeof(AssetDataObsolete))]
        [XmlElement("update", Type = typeof(AssetDataUpdate))]
        public List<AssetDataActionBase> Action { get; set; }


    }

    /// <summary>
    /// Asset data action base
    /// </summary>
    [XmlType(nameof(AssetDataActionBase), Namespace = "http://openiz.org/applet")]
    public abstract class AssetDataActionBase
    {

        /// <summary>
        /// Gets the action name
        /// </summary>
        public abstract String ActionName { get; }
        /// <summary>
        /// Gets the elements to be performed
        /// </summary>
        [XmlElement("Concept", typeof(Concept), Namespace = "http://openiz.org/model")]
        [XmlElement("ConceptSet", typeof(ConceptSet), Namespace = "http://openiz.org/model")]
        [XmlElement("AssigningAuthority", typeof(AssigningAuthority), Namespace = "http://openiz.org/model")]
        [XmlElement("ConceptClass", typeof(ConceptClass), Namespace = "http://openiz.org/model")]
        [XmlElement("SecurityPolicy", typeof(SecurityPolicy), Namespace = "http://openiz.org/model")]
        [XmlElement("SecurityRole", typeof(SecurityRole), Namespace = "http://openiz.org/model")]
        [XmlElement("ExtensionType", typeof(ExtensionType), Namespace = "http://openiz.org/model")]
        [XmlElement("IdentifierType", typeof(IdentifierType), Namespace = "http://openiz.org/model")]
        [XmlElement("Bundle", typeof(Bundle), Namespace = "http://openiz.org/model")]
        public IdentifiedData Element { get; set; }
    }
    /// <summary>
    /// Asset data update
    /// </summary>
    [XmlType(nameof(AssetDataUpdate), Namespace = "http://openiz.org/applet")]
    public class AssetDataUpdate : AssetDataActionBase
    {
        /// <summary>
        /// Gets the action name
        /// </summary>
        public override string ActionName {  get { return "Update"; } }

        /// <summary>
        /// Insert if not exists
        /// </summary>
        [XmlAttribute("insertIfNotExists")]
        public bool InsertIfNotExists { get; set; }


    }

    /// <summary>
    /// Obsoletes the specified data elements
    /// </summary>
    [XmlType(nameof(AssetDataObsolete), Namespace = "http://openiz.org/applet")]
    public class AssetDataObsolete : AssetDataActionBase
    {
        /// Gets the action name
        /// </summary>
        public override string ActionName { get { return "Obsolete"; } }

    }

    /// <summary>
    /// Data insert
    /// </summary>
    [XmlType(nameof(AssetDataInsert), Namespace = "http://openiz.org/applet")]
    public class AssetDataInsert : AssetDataActionBase
    {
        /// Gets the action name
        /// </summary>
        public override string ActionName { get { return "Insert"; } }

    }
}