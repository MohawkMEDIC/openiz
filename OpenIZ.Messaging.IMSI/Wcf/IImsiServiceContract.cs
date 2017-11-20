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
 * Date: 2016-6-14
 */
using OpenIZ.Core.Interop;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
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
	[ServiceKnownType(typeof(ConceptClass))]
	[ServiceKnownType(typeof(ConceptRelationship))]
	[ServiceKnownType(typeof(ReferenceTerm))]
    [ServiceKnownType(typeof(Act))]
    [ServiceKnownType(typeof(TextObservation))]
    [ServiceKnownType(typeof(CodedObservation))]
    [ServiceKnownType(typeof(QuantityObservation))]
    [ServiceKnownType(typeof(PatientEncounter))]
    [ServiceKnownType(typeof(SubstanceAdministration))]
    [ServiceKnownType(typeof(Entity))]
    [ServiceKnownType(typeof(Patient))]
	[ServiceKnownType(typeof(Person))]
    [ServiceKnownType(typeof(EntityRelationship))]
	[ServiceKnownType(typeof(Provider))]
    [ServiceKnownType(typeof(Organization))]
    [ServiceKnownType(typeof(Place))]
    [ServiceKnownType(typeof(ServiceOptions))]
    [ServiceKnownType(typeof(Material))]
    [ServiceKnownType(typeof(ExtensionType))]
    [ServiceKnownType(typeof(ManufacturedMaterial))]
    [ServiceKnownType(typeof(DeviceEntity))]
	[ServiceKnownType(typeof(UserEntity))]
	[ServiceKnownType(typeof(SecurityUser))]
	[ServiceKnownType(typeof(SecurityRole))]
	[ServiceKnownType(typeof(ApplicationEntity))]
	[ServiceKnownType(typeof(CarePlan))]
    [ServiceKnownType(typeof(Bundle))]
    [ServiceKnownType(typeof(Patch))]
    [ServiceKnownType(typeof(ErrorResult))]
    [ServiceKnownType(typeof(ConceptSet))]
	[ServiceKnownType(typeof(ConceptReferenceTerm))]
	public interface IImsiServiceContract 
    {

        /// <summary>
        /// Gets the operations that each resource in this IMS instance supports.
        /// </summary>
        [WebInvoke(UriTemplate = "/", Method = "OPTIONS", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Options();

        /// <summary>
        /// Performs a minimal PING request to test service uptime
        /// </summary>
        [WebInvoke(UriTemplate = "/", Method = "PING", BodyStyle = WebMessageBodyStyle.Bare)]
        void Ping();

        /// <summary>
        /// Performs a search for the specified resource, returning only current version items.
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        [FaultContract(typeof(ErrorResult))]
        IdentifiedData Search(string resourceType);

        /// <summary>
        /// Searches for the specified resource and returns only the HEADer metadata
        /// </summary>
        [WebInvoke(Method = "HEAD", UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        void HeadSearch(string resourceType);

        /// <summary>
        /// Retrieves the current version of the specified resource from the IMS.
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Get(string resourceType, string id);

        /// <summary>
        /// Retrieves only the metadata of the specified resource
        /// </summary>
        [WebInvoke(Method = "HEAD", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        void GetHead(string resourceType, string id);

        /// <summary>
        /// Gets a complete history of all changes made to the specified resource
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/history", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData History(string resourceType, string id);

        /// <summary>
        /// Updates the specified resource according to the instructions in the PATCH file
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "PATCH", UriTemplate = "/{resourceType}/{id}")]
        void Patch(string resourceType, string id , Patch body);

        /// <summary>
        /// Returns a list of patches for the specified resource 
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/patch")]
        Patch GetPatch(string resourceType, string id);

        /// <summary>
        /// Retrieves a specific version of the specified resource
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/history/{versionId}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData GetVersion(string resourceType, string id, string versionId);

        /// <summary>
        /// Creates the resource. If the resource already exists, then a 409 is thrown
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Create(string resourceType, IdentifiedData body);

        /// <summary>
        /// Updates the specified resource. If the resource does not exist than a 404 is thrown
        /// </summary>
        [WebInvoke(Method = "PUT", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Update(string resourceType, string id, IdentifiedData body);

        /// <summary>
        /// Creates or updates a resource. That is, creates the resource if it does not exist, or updates it if it does
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData CreateUpdate(string resourceType, string id, IdentifiedData body);

        /// <summary>
        /// Deletes the specified resource from the IMS instance
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
