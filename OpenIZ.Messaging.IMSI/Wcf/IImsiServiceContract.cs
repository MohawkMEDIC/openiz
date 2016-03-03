/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-1-24
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Messaging.IMSI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace OpenIZ.Messaging.IMSI.Wcf
{
    /// <summary>
    /// The IMSI service interface
    /// </summary>
    [ServiceContract(Namespace = "http://openiz.org/imsi/1.0", Name = "IMSI", ConfigurationName = "IMSI_1.0")]
    [ServiceKnownType(typeof(Concept))]
    [ServiceKnownType(typeof(ReferenceTerm))]
    [ServiceKnownType(typeof(Act))]
    [ServiceKnownType(typeof(TextObservation))]
    [ServiceKnownType(typeof(CodedObservation))]
    [ServiceKnownType(typeof(QuantityObservation))]
    [ServiceKnownType(typeof(PatientEncounter))]
    [ServiceKnownType(typeof(SubstanceAdministration))]
    [ServiceKnownType(typeof(Entity))]
    [ServiceKnownType(typeof(Patient))]
    [ServiceKnownType(typeof(Provider))]
    [ServiceKnownType(typeof(Organization))]
    [ServiceKnownType(typeof(Place))]
    [ServiceKnownType(typeof(Material))]
    [ServiceKnownType(typeof(ManufacturedMaterial))]
    [ServiceKnownType(typeof(DeviceEntity))]
    [ServiceKnownType(typeof(ApplicationEntity))]
    [ServiceKnownType(typeof(Bundle))]
    [ServiceKnownType(typeof(ErrorResult))]
    [ServiceKnownType(typeof(ConceptSet))]
    public interface IImsiServiceContract 
    {

        /// <summary>
        /// Search for the specified resource type
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        [FaultContract(typeof(ErrorResult))]
        IdentifiedData Search(string resourceType);

        /// <summary>
        /// Get the specified resource
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Get(string resourceType, string id);

        /// <summary>
        /// Get history of an object
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/history", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData History(string resourceType, string id);

        /// <summary>
        /// Get a specific version
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/history/{versionId}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData GetVersion(string resourceType, string id, string versionId);

        /// <summary>
        /// Create the resource
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Create(string resourceType, IdentifiedData body);

        /// <summary>
        /// Update the specified resource
        /// </summary>
        [WebInvoke(Method = "PUT", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Update(string resourceType, string id, IdentifiedData body);

        /// <summary>
        /// Creates a resource or updates one
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData CreateUpdate(string resourceType, string id, IdentifiedData body);

        /// <summary>
        /// Creates a resource or updates one
        /// </summary>
        [WebInvoke(Method = "DELETE", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Delete(string resourceType, string id);

        /// <summary>
        /// Get the current time
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/time")]
        DateTime Time();

        /// <summary>
        /// Get the schema
        /// </summary>
        [WebGet(UriTemplate = "/?xsd={schemaId}")]
        XmlSchema GetSchema(int schemaId);
    }
}
