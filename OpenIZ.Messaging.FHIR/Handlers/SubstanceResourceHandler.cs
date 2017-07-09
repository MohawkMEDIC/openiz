using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Represents a resource handler that can handle substances
    /// </summary>
    public class SubstanceResourceHandler : RepositoryResourceHandlerBase<Substance, Material>
    {
        /// <summary>
        /// Map the substance to FHIR
        /// </summary>
        protected override Substance MapToFhir(Material model, WebOperationContext webOperationContext)
        {
            var retVal = DataTypeConverter.CreateResource<Substance>(model);

            // Identifiers
            retVal.Identifier = model.Identifiers.Select(o => DataTypeConverter.ToFhirIdentifier<Entity>(o)).ToList();

            // sTatus
            if (model.StatusConceptKey == StatusKeys.Active)
                retVal.Status = SubstanceStatus.Active;
            else if (model.StatusConceptKey == StatusKeys.Nullified)
                retVal.Status = SubstanceStatus.Nullified;
            else if (model.StatusConceptKey == StatusKeys.Obsolete)
                retVal.Status = SubstanceStatus.Inactive;

            // Category and code
            if (model.LoadProperty<Concept>("TypeConcept").ConceptSetsXml.Any(o => o == ConceptSetKeys.VaccineTypeCodes))
                retVal.Category = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirCodeableConcept(new Uri("http://hl7.org/fhir/substance-category"), "drug");
            else
                retVal.Category = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirCodeableConcept(new Uri("http://hl7.org/fhir/substance-category"), "material");

            retVal.Code = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"), "http://hl7.org/fhir/ValueSet/substance-code");

            retVal.Description = model.LoadCollection<EntityName>("Names").FirstOrDefault(o => o.NameUseKey == NameUseKeys.OfficialRecord)?.LoadCollection<EntityNameComponent>("Components")?.FirstOrDefault()?.Value;

            // TODO: Instance or kind
            if (model.DeterminerConceptKey == DeterminerKeys.Specific)
            {
                var conceptRepo = ApplicationContext.Current.GetService<IConceptRepositoryService>();
                retVal.Instance = new List<MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.SubstanceInstance>()
                {
                    new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.SubstanceInstance()
                    {
                        Expiry = model.ExpiryDate,
                        Quantity = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirQuantity() {
                            Units = model.QuantityConceptKey.HasValue ? conceptRepo.GetConceptReferenceTerm(model.QuantityConceptKey.Value, "UCUM").Mnemonic : null,
                            Value = model.Quantity
                        }
                    }
                };
            }

            return retVal;
        }

        protected override Material MapToModel(Substance resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }
    }
}
