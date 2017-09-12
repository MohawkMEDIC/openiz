using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Linq.Expressions;
using MARC.Everest.Connectors;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Represents a resource handler for medication administration resources
    /// </summary>
    public class MedicationAdministrationResourceHandler : RepositoryResourceHandlerBase<MedicationAdministration, SubstanceAdministration>
    {
        /// <summary>
        /// Maps the object to model to fhir
        /// </summary>
        protected override MedicationAdministration MapToFhir(SubstanceAdministration model, WebOperationContext webOperationContext)
        {
            var retVal = DataTypeConverter.CreateResource<MedicationAdministration>(model);

            retVal.Identifier = model.LoadCollection<ActIdentifier>("Identifiers").Select(o => DataTypeConverter.ToFhirIdentifier(o)).ToList();

            if (model.StatusConceptKey == StatusKeys.Active)
                retVal.Status = MedicationAdministrationStatus.InProgress;
            else if (model.StatusConceptKey == StatusKeys.Completed)
                retVal.Status = MedicationAdministrationStatus.Completed;
            else if (model.StatusConceptKey == StatusKeys.Nullified)
                retVal.Status = MedicationAdministrationStatus.EnteredInError;
            else if (model.StatusConceptKey == StatusKeys.Cancelled)
                retVal.Status = MedicationAdministrationStatus.Stopped;

            retVal.Category = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"), "http://hl7.org/fhir/medication-admin-category");

            var consumableRelationship = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Consumable);
            var productRelationship = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Product);
            if (consumableRelationship != null)
                retVal.Medication = DataTypeConverter.CreateReference<Medication>(consumableRelationship.LoadProperty<ManufacturedMaterial>("PlayerEntity"), webOperationContext);
            else if (productRelationship != null)
            {
                retVal.Medication = DataTypeConverter.CreateReference<Substance>(productRelationship.LoadProperty<Material>("PlayerEntity"), webOperationContext);
                //retVal.Medication = DataTypeConverter.ToFhirCodeableConcept(productRelationship.LoadProperty<Material>("PlayerEntity").LoadProperty<Concept>("TypeConcept"));
            }

            var rct = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget);
            if (rct != null)
                retVal.Subject = DataTypeConverter.CreateReference<Patient>(rct.LoadProperty<Entity>("PlayerEntity"), webOperationContext);

            // Encounter
            var erService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();
            int tr = 0;
            var enc = erService.Query(o => o.TargetEntityKey == model.Key && o.RelationshipTypeKey == ActRelationshipTypeKeys.HasComponent, 0, 1, AuthenticationContext.Current.Principal, out tr).FirstOrDefault();
            if (enc != null)
            {
                // TODO: Encounter
            }

            // Effective time
            retVal.EffectiveDate = model.ActTime.DateTime;

            // performer
            var performer = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Performer) ??
                model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator);
            if (performer != null)
                retVal.Performer = new List<MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.MedicationPerformer>() {
                    new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.MedicationPerformer()
                {
                    Actor = DataTypeConverter.CreateReference<Practitioner>(performer.LoadProperty<Entity>("PlayerEntity"), webOperationContext)
                }
                };

            // Not given
            retVal.NotGiven = model.IsNegated;
            if (model.ReasonConceptKey.HasValue && model.IsNegated)
                retVal.ReasonNotGiven = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("ReasonConcept"));
            else if (model.ReasonConceptKey.HasValue)
                retVal.ReasonCode = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("ReasonConcept"));

            retVal.Dosage = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.MedicationDosage()
            {
                Site = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("Site")),
                Route = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("Route")),
                Dose = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirQuantity()
                {
                    Value = model.DoseQuantity,
                    Units = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("DoseUnit"), "http://hl7.org/fhir/sid/ucum").GetPrimaryCode()?.Code?.Value
                }
            };

            return retVal;
        }

        protected override SubstanceAdministration MapToModel(MedicationAdministration resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
		/// Query for substance administrations that aren't immunizations
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="issues">The issues.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns the list of models which match the given parameters.</returns>
		protected override IEnumerable<SubstanceAdministration> Query(Expression<Func<SubstanceAdministration, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
        {
            Guid initialImmunization = Guid.Parse("f3be6b88-bc8f-4263-a779-86f21ea10a47"),
                immunization = Guid.Parse("6e7a3521-2967-4c0a-80ec-6c5c197b2178"),
                boosterImmunization = Guid.Parse("0331e13f-f471-4fbd-92dc-66e0a46239d5");

            var obsoletionReference = Expression.MakeBinary(ExpressionType.Equal, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.StatusConceptKey))), typeof(Guid)), Expression.Constant(StatusKeys.Completed));
            var typeReference = Expression.MakeBinary(ExpressionType.And,
                Expression.MakeBinary(ExpressionType.And,
                    Expression.MakeBinary(ExpressionType.NotEqual, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.TypeConceptKey))), typeof(Guid)), Expression.Constant(initialImmunization)),
                    Expression.MakeBinary(ExpressionType.NotEqual, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.TypeConceptKey))), typeof(Guid)), Expression.Constant(immunization))
                ),
                Expression.MakeBinary(ExpressionType.NotEqual, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.TypeConceptKey))), typeof(Guid)), Expression.Constant(boosterImmunization))
            );

            query = Expression.Lambda<Func<SubstanceAdministration, bool>>(Expression.AndAlso(Expression.AndAlso(obsoletionReference, query.Body), typeReference), query.Parameters);

            if (queryId == Guid.Empty)
                return this.m_repository.Find(query, offset, count, out totalResults);
            else
                return (this.m_repository as IPersistableQueryRepositoryService).Find<SubstanceAdministration>(query, offset, count, out totalResults, queryId);
        }
    }
}
