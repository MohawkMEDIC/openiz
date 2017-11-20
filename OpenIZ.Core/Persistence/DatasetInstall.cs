/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Core.Persistence
{
    /// <summary>
    /// A class representing data operations
    /// </summary>
    [XmlRoot("dataset", Namespace = "http://openiz.org/data")]
    [XmlType(nameof(DatasetInstall), Namespace = "http://openiz.org/data")]
    public class DatasetInstall
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public DatasetInstall()
        {
            this.Action = new List<DataInstallAction>();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="DatasetInstall"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public DatasetInstall(string id) : this()
	    {
		    this.Id = id;
	    }

        /// <summary>
        /// Gets or sets the identifier of the dataset
        /// </summary>
        [XmlAttribute("id")]
        public String Id { get; set; }
        
        /// <summary>
        /// Actions to be performed
        /// </summary>
        [XmlElement("insert", Type = typeof(DataInsert))]
        [XmlElement("obsolete", Type = typeof(DataObsolete))]
        [XmlElement("update", Type = typeof(DataUpdate))]
        public List<DataInstallAction> Action { get; set; }

        /// <summary>
        /// Loads the specified file to dataset
        /// </summary>
        public static DatasetInstall Load(string datasetFile)
        {
            using (var fs = File.OpenRead(datasetFile))
            {
                XmlSerializer xs = new XmlSerializer(typeof(DatasetInstall));
                return xs.Deserialize(fs) as DatasetInstall;
            }
        }
    }

    /// <summary>
    /// Asset data action base
    /// </summary>
    [XmlType(nameof(DataInstallAction), Namespace = "http://openiz.org/data")]
    public abstract class DataInstallAction
    {

        /// <summary>
        /// Gets the action name
        /// </summary>
        public abstract String ActionName { get; }

		/// <summary>
		/// Gets the elements to be performed
		/// </summary>
		[XmlElement("ConceptReferenceTerm", typeof(ConceptReferenceTerm), Namespace = "http://openiz.org/model")]
		[XmlElement("ConceptName", typeof(ConceptName), Namespace = "http://openiz.org/model")]
		[XmlElement("EntityRelationship", typeof(EntityRelationship), Namespace = "http://openiz.org/model")]
		[XmlElement("Concept", typeof(Concept), Namespace = "http://openiz.org/model")]
        [XmlElement("ConceptSet", typeof(ConceptSet), Namespace = "http://openiz.org/model")]
        [XmlElement("ConceptRelationship", typeof(ConceptRelationship), Namespace = "http://openiz.org/model")]
        [XmlElement("AssigningAuthority", typeof(AssigningAuthority), Namespace = "http://openiz.org/model")]
        [XmlElement("ConceptClass", typeof(ConceptClass), Namespace = "http://openiz.org/model")]
        [XmlElement("SecurityPolicy", typeof(SecurityPolicy), Namespace = "http://openiz.org/model")]
        [XmlElement("SecurityRole", typeof(SecurityRole), Namespace = "http://openiz.org/model")]
        [XmlElement("SecurityUser", typeof(SecurityUser), Namespace = "http://openiz.org/model")]
        [XmlElement("ExtensionType", typeof(ExtensionType), Namespace = "http://openiz.org/model")]
        [XmlElement("CodeSystem", typeof(CodeSystem), Namespace = "http://openiz.org/model")]
        [XmlElement("ReferenceTerm", typeof(ReferenceTerm), Namespace = "http://openiz.org/model")]
        [XmlElement("IdentifierType", typeof(IdentifierType), Namespace = "http://openiz.org/model")]
        [XmlElement("UserEntity", typeof(UserEntity), Namespace = "http://openiz.org/model")]
        [XmlElement("Entity", typeof(Entity), Namespace = "http://openiz.org/model")]
        [XmlElement("Organization", typeof(Organization), Namespace = "http://openiz.org/model")]
        [XmlElement("Person", typeof(Person), Namespace = "http://openiz.org/model")]
        [XmlElement("Provider", typeof(Provider), Namespace = "http://openiz.org/model")]
        [XmlElement("Material", typeof(Material), Namespace = "http://openiz.org/model")]
        [XmlElement("ManufacturedMaterial", typeof(ManufacturedMaterial), Namespace = "http://openiz.org/model")]
        [XmlElement("Patient", typeof(Patient), Namespace = "http://openiz.org/model")]
        [XmlElement("Place", typeof(Place), Namespace = "http://openiz.org/model")]
        [XmlElement("Bundle", typeof(Bundle), Namespace = "http://openiz.org/model")]
        [XmlElement("Act", typeof(Act), Namespace = "http://openiz.org/model")]
        [XmlElement("SubstanceAdministration", typeof(SubstanceAdministration), Namespace = "http://openiz.org/model")]
        [XmlElement("QuantityObservation", typeof(QuantityObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("CodedObservation", typeof(CodedObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("EntityIdentifier", typeof(EntityIdentifier), Namespace = "http://openiz.org/model")]
        [XmlElement("TextObservation", typeof(TextObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("PatientEncounter", typeof(PatientEncounter), Namespace = "http://openiz.org/model")]
        public IdentifiedData Element { get; set; }

        /// <summary>
        /// Associate the specified data for stuff that cannot be serialized
        /// </summary>
        [XmlElement("associate")]
        public List<DataAssociation> Association { get; set; }

        /// <summary>
        /// Skip if errored
        /// </summary>
        [XmlAttribute("skipIfError")]
        public bool IgnoreErrors { get; set; }

    }

    /// <summary>
    /// Associate data
    /// </summary>
    [XmlType(nameof(DataAssociation), Namespace = "http://openiz.org/data")]
    public class DataAssociation : DataInstallAction
    {

        /// <summary>
        /// Action to be performed
        /// </summary>
        public override string ActionName
        {
            get
            {
                return "Add";
            }
        }


        /// <summary>
        /// The name of the property
        /// </summary>
        [XmlAttribute("property")]
        public String PropertyName { get; set; }
        
    }

    /// <summary>
    /// Asset data update
    /// </summary>
    [XmlType(nameof(DataUpdate), Namespace = "http://openiz.org/data")]
    public class DataUpdate : DataInstallAction
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
    [XmlType(nameof(DataObsolete), Namespace = "http://openiz.org/data")]
    public class DataObsolete : DataInstallAction
    {
        /// Gets the action name
        /// </summary>
        public override string ActionName { get { return "Obsolete"; } }

    }

    /// <summary>
    /// Data insert
    /// </summary>
    [XmlType(nameof(DataInsert), Namespace = "http://openiz.org/data")]
    public class DataInsert : DataInstallAction
    {
        /// Gets the action name
        /// </summary>
        public override string ActionName { get { return "Insert"; } }

        /// <summary>
        /// True if the insert should be skipped if it exists
        /// </summary>
        [XmlAttribute("skipIfExists")]
        public bool SkipIfExists { get; set; }
    }
}