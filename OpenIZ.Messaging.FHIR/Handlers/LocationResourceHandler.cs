using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Linq.Expressions;
using System.ServiceModel.Web;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Facility resource handler
    /// </summary>
    public class LocationResourceHandler : RepositoryResourceHandlerBase<Location, Place>
    {
        /// <summary>
        /// Map the inbound place to a FHIR model
        /// </summary>
        protected override Location MapToFhir(Place model, WebOperationContext webOperationContext)
        {

            Location retVal = DataTypeConverter.CreateResource<Location>(model);
            retVal.Identifier = model.Identifiers.Select(o => DataTypeConverter.ToFhirIdentifier<Entity>(o)).ToList();

            // Map status
            if (model.StatusConceptKey == StatusKeys.Active)
                retVal.Status = LocationStatus.Active;
            else if (model.StatusConceptKey == StatusKeys.Obsolete)
                retVal.Status = LocationStatus.Inactive;
            else
                retVal.Status = LocationStatus.Suspended;

            retVal.Name = model.LoadCollection<EntityName>("Names").FirstOrDefault(o => o.NameUseKey == NameUseKeys.OfficialRecord)?.LoadCollection<EntityNameComponent>("Components")?.FirstOrDefault()?.Value;
            retVal.Alias = model.LoadCollection<EntityName>("Names").Where(o => o.NameUseKey != NameUseKeys.OfficialRecord)?.Select(n=> (FhirString)n.LoadCollection<EntityNameComponent>("Components")?.FirstOrDefault()?.Value).ToList();

            // Convert the determiner code
            if (model.DeterminerConceptKey == DeterminerKeys.Described)
                retVal.Mode = LocationMode.Kind;
            else
                retVal.Mode = LocationMode.Instance;

            retVal.Type = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"), "http://hl7.org/fhir/ValueSet/v3-ServiceDeliveryLocationRoleType");

            retVal.Telecom = model.LoadCollection<EntityTelecomAddress>("Telecoms").Select(o => DataTypeConverter.ToFhirTelecom(o)).ToList();
            retVal.Address = DataTypeConverter.ToFhirAddress(model.LoadCollection<EntityAddress>("Addresses").FirstOrDefault());

            if (model.Lat.HasValue && model.Lng.HasValue)
                retVal.Position = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.Position()
                {
                    Latitude = (decimal)model.Lat.Value,
                    Longitude = (decimal)model.Lng.Value
                };

            // Part of?
            var parent = model.LoadCollection<EntityRelationship>("Relationships").FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);
            if (parent != null)
                retVal.PartOf = DataTypeConverter.CreateReference<Location>(parent.LoadProperty<Entity>("TargetEntity"));

            return retVal;
        }

        protected override Place MapToModel(Location resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }
    }
}
