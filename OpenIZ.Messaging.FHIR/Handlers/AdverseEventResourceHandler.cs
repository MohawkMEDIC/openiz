using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using MARC.Everest.Connectors;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Adverse event resource handler
    /// </summary>
    public class AdverseEventResourceHandler : RepositoryResourceHandlerBase<AdverseEvent, Act>
    {
        /// <summary>
        /// Maps the specified act to an adverse event
        /// </summary>
        protected override AdverseEvent MapToFhir(Act model, WebOperationContext webOperationContext)
        {
            var retVal = DataTypeConverter.CreateResource<AdverseEvent>(model);

            retVal.Identifier = DataTypeConverter.ToFhirIdentifier<Act>(model.Identifiers.FirstOrDefault());
            retVal.Category = AdverseEventCategory.AdverseEvent;
            retVal.Type = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"));

            var recordTarget = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget);
            if (recordTarget != null)
                retVal.Subject = DataTypeConverter.CreateReference<Patient>(recordTarget.LoadProperty<Entity>("PlayerEntity"));

            // Main topic of the concern
            var subject = model.LoadCollection<ActRelationship>("Relationships").FirstOrDefault(o => o.RelationshipTypeKey == ActRelationshipTypeKeys.HasSubject)?.LoadProperty<Act>("TargetAct");
            if (subject == null) throw new InvalidOperationException("This act does not appear to be an adverse event");
            retVal.Date = subject.ActTime.DateTime;

            // Reactions = HasManifestation
            var reactions = subject.LoadCollection<ActRelationship>("Relationships").Where(o => o.RelationshipTypeKey == ActRelationshipTypeKeys.HasManifestation);
            retVal.Reaction = reactions.Select(o => DataTypeConverter.CreateReference<Condition>(o.LoadProperty<Act>("TargetAct"))).ToList();

            var location = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Location);
            if (location != null)
                retVal.Location = DataTypeConverter.CreateReference<Location>(location.LoadProperty<Entity>("PlayerEntity"));

            // Severity
            var severity = subject.LoadCollection<ActRelationship>("Relationships").First(o => o.RelationshipTypeKey == ActRelationshipTypeKeys.HasComponent && o.LoadProperty<Act>("TargetAct").TypeConceptKey == ObservationTypeKeys.Severity);
            if (severity != null)
                retVal.Seriousness = DataTypeConverter.ToFhirCodeableConcept(severity.LoadProperty<CodedObservation>("TargetAct").Value, "http://hl7.org/fhir/adverse-event-seriousness");

            // Did the patient die?
            var causeOfDeath = model.LoadCollection<ActRelationship>("Relationships").FirstOrDefault(o=>o.RelationshipTypeKey == ActRelationshipTypeKeys.IsCauseOf && o.LoadProperty<Act>("TargetAct").TypeConceptKey == ObservationTypeKeys.ClinicalState && (o.TargetAct as CodedObservation)?.ValueKey == Guid.Parse("6df3720b-857f-4ba2-826f-b7f1d3c3adbb"));
            if (causeOfDeath != null)
                retVal.Outcome = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirCodeableConcept(new Uri("http://hl7.org/fhir/adverse-event-outcome"), "fatal");
            else if (model.StatusConceptKey == StatusKeys.Active)
                retVal.Outcome = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirCodeableConcept(new Uri("http://hl7.org/fhir/adverse-event-outcome"), "ongoing");
            else if (model.StatusConceptKey == StatusKeys.Completed)
                retVal.Outcome = new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirCodeableConcept(new Uri("http://hl7.org/fhir/adverse-event-outcome"), "resolved");

            var author = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator);
            if (author != null)
                retVal.Recorder = DataTypeConverter.CreateReference<Practitioner>(author.LoadProperty<Entity>("PlayerEntity"));

            // Suspect entities
            var refersTo = model.LoadCollection<ActRelationship>("Relationships").Where(o => o.RelationshipTypeKey == ActRelationshipTypeKeys.RefersTo);
            retVal.SuspectEntity = refersTo.Select(o => o.LoadProperty<Act>("TargetAct")).OfType<SubstanceAdministration>().Select(o=>
            {
                var consumable = o.LoadCollection<ActParticipation>("Participations").FirstOrDefault(x => x.ParticipationRoleKey == ActParticipationKey.Consumable)?.LoadProperty<ManufacturedMaterial>("PlayerEntity");
                if (consumable == null)
                {
                    var product = o.LoadCollection<ActParticipation>("Participations").FirstOrDefault(x => x.ParticipationRoleKey == ActParticipationKey.Product)?.LoadProperty<ManufacturedMaterial>("PlayerEntity");
                    return new AdverseEventSuspectEntity() { Instance = DataTypeConverter.CreateReference<Substance>(product) };
                }
                else
                    return new AdverseEventSuspectEntity() { Instance = DataTypeConverter.CreateReference<Medication>(consumable) };
            }).ToList();

            return retVal;
        }

        protected override Act MapToModel(AdverseEvent resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Act> Query(Expression<Func<Act, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
        {

            var typeReference = Expression.MakeBinary(ExpressionType.Equal, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.TypeConceptKey))), typeof(Guid)), Expression.Constant(ActClassKeys.Condition));

            var anyRef = base.CreateConceptSetFilter(ConceptSetKeys.AdverseEventActs, query.Parameters[0]);
            query = Expression.Lambda<Func<Act, bool>>(Expression.AndAlso(Expression.AndAlso(query.Body, anyRef), typeReference), query.Parameters);

            return base.Query(query, issues, queryId, offset, count, out totalResults);
        }
    }
}
