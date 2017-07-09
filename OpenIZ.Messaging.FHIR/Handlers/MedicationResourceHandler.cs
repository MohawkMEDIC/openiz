using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using MARC.Everest.Connectors;
using System.Linq.Expressions;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Represents a medication resource handler
    /// </summary>
    public class MedicationResourceHandler : RepositoryResourceHandlerBase<Medication, ManufacturedMaterial>
    {

        /// <summary>
        /// Map this manufactured material to FHIR
        /// </summary>
        protected override Medication MapToFhir(ManufacturedMaterial model, WebOperationContext webOperationContext)
        {
            var retVal = DataTypeConverter.CreateResource<Medication>(model);

            // Code of medication code
            retVal.Code = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"));

            if (model.StatusConceptKey == StatusKeys.Active)
                retVal.Status = SubstanceStatus.Active;
            else if (model.StatusConceptKey == StatusKeys.Obsolete)
                retVal.Status = SubstanceStatus.Inactive;
            else if (model.StatusConceptKey == StatusKeys.Nullified)
                retVal.Status = SubstanceStatus.Nullified;

            // Is brand?
            retVal.IsBrand = false;
            retVal.IsOverTheCounter = model.Tags.Any(o=>o.TagKey == "isOtc");

            var manufacturer = model.LoadCollection<EntityRelationship>("Relationships").FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.WarrantedProduct);
            if (manufacturer != null)
                retVal.Manufacturer = DataTypeConverter.CreateReference<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Organization>(manufacturer.LoadProperty<Entity>("TargetEntity"));

            // Form
            retVal.Form = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("FormConcept"), "http://hl7.org/fhir/ValueSet/medication-form-codes");
            retVal.Package = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.MedicationPackage();
            retVal.Package.Batch = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.MedicationBatch()
            {
                LotNumber = model.LotNumber,
                Expiration = model.ExpiryDate
            };

            // Picture of the object?

            var photo = model.LoadCollection<EntityExtension>("Extensions").FirstOrDefault(o => o.ExtensionTypeKey == ExtensionTypeKeys.JpegPhotoExtension);
            if (photo != null)
                retVal.Image = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.Attachment()
                {
                    ContentType = "image/jpg",
                    Data = photo.ExtensionValueXml
                };
            return retVal;
        }

        protected override ManufacturedMaterial MapToModel(Medication resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }

    }
}
