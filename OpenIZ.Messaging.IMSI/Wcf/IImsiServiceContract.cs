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
using OpenIZ.Core.Interop;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Patch;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using OpenIZ.Messaging.IMSI.Model;
using SwaggerWcf.Attributes;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
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
        [SwaggerWcfPath("Get Service Options", "Retrieves a list of resources and operations supported by this IMSI service", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        IdentifiedData Options();

        /// <summary>
        /// Options for resource
        /// </summary>
        [WebInvoke(UriTemplate = "/{resourceType}", Method = "OPTIONS", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Get Service Options", "Retrieves a list of resources and operations supported by this IMSI service", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        void OptionsResource(string resourceType);

        /// <summary>
        /// Performs a minimal PING request to test service uptime
        /// </summary>
        [WebInvoke(UriTemplate = "/", Method = "PING", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Service Availability Status", "Forces the service to respond with a 204 if the IMSI is running at this endpoint", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        void Ping();

        /// <summary>
        /// Performs a search for the specified resource, returning only current version items.
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        [FaultContract(typeof(ErrorResult))]
        [SwaggerWcfPath("Search Resource", "Conducts a search of the identified resource type. The search query parameters are provided in IMSI query specification format", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        IdentifiedData Search(string resourceType);

        /// <summary>
        /// Searches for the specified resource and returns only the HEADer metadata
        /// </summary>
        [WebInvoke(Method = "HEAD", UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Head Search Resource", "Conducts a search of the identified resource type using the IMSI query provided, however only returns HTTP headers for the search", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        void HeadSearch(string resourceType);

        /// <summary>
        /// Retrieves the current version of the specified resource from the IMS.
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Get Resource", "Gets the identified resource from the IMS service data store")]
        IdentifiedData Get(string resourceType, string id);

        /// <summary>
        /// Retrieves only the metadata of the specified resource
        /// </summary>
        [WebInvoke(Method = "HEAD", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Head Resource", "Retrieves the HTTP header information for the identified resource", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        void Head(string resourceType, string id);

        /// <summary>
        /// Gets a complete history of all changes made to the specified resource
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/history", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Get Resource History", "Provides a complete list of all changes made to the identified resource", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        IdentifiedData History(string resourceType, string id);

        /// <summary>
        /// Updates the specified resource according to the instructions in the PATCH file
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "PATCH", UriTemplate = "/{resourceType}/{id}")]
        [SwaggerWcfPath("Patch Resource", "Performs a partial update of the resource using the provided patch information", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        void Patch(string resourceType, string id , Patch body);

        /// <summary>
        /// Returns a list of patches for the specified resource 
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/patch")]
        [SwaggerWcfPath("Get Changeset", "Retrieves the patches that have been applied to the identified resource", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        Patch GetPatch(string resourceType, string id);

        /// <summary>
        /// Retrieves a specific version of the specified resource
        /// </summary>
        [WebGet(UriTemplate = "/{resourceType}/{id}/history/{versionId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Get Resource Version", "Retrieves a specific version of a resource from the IMS datastore", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        IdentifiedData GetVersion(string resourceType, string id, string versionId);

        /// <summary>
        /// Creates the resource. If the resource already exists, then a 409 is thrown
        /// </summary>
        [SwaggerWcfPath("Create Resource", "Creates a the specified resources in the IMS datastore. If the resource already exists, a 409 conflict is returned", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        [WebInvoke(Method = "POST", UriTemplate = "/{resourceType}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Create(string resourceType, IdentifiedData body);

        /// <summary>
        /// Updates the specified resource. If the resource does not exist than a 404 is thrown
        /// </summary>
        [SwaggerWcfPath("Update Resource", "Creates a new version of the identified resource in the IMS datastore. If the resource does not exist, returns a 404", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        [WebInvoke(Method = "PUT", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Update(string resourceType, string id, IdentifiedData body);

        /// <summary>
        /// Creates or updates a resource. That is, creates the resource if it does not exist, or updates it if it does
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [SwaggerWcfPath("Create or Update Resource", "Updates the identified resource if it exists, otherwise creates it in the IMS data store", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        IdentifiedData CreateUpdate(string resourceType, string id, IdentifiedData body);

        /// <summary>
        /// Deletes the specified resource from the IMS instance
        /// </summary>
        [SwaggerWcfPath("Logically Delete Resource", "Performs a logical delete (obsoletion) of the identified resource", ExternalDocsUrl = "http://openiz.org/artifacts/1.0/imsi/", ExternalDocsDescription = "IMSI Data Contract Documentation")]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{resourceType}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        IdentifiedData Delete(string resourceType, string id);

        /// <summary>
        /// Get the current time
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/time")]
        [SwaggerWcfPath("Get Server Time", "Retrieves the current server time")]
        DateTime Time();

        /// <summary>
        /// Get the schema
        /// </summary>
        [WebGet(UriTemplate = "/?xsd={schemaId}")]
        [SwaggerWcfPath("Get Server Schema", "Gets a complete schema of the IMSI objects supported by this interface")]
        XmlSchema GetSchema(int schemaId);
    }
}
