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
 * Date: 2017-1-21
 */
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using System.Security.Principal;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model.Extensibility;
using OpenIZ.Persistence.Data.ADO.Data.Model.DataType;
using OpenIZ.Core.Model;
using OpenIZ.OrmLite;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service which persists ACT classes
    /// </summary>
    public class ActPersistenceService : VersionedDataPersistenceService<Core.Model.Acts.Act, DbActVersion, DbAct>
    {
        private const String ControlAct = "B35488CE-B7CD-4DD4-B4DE-5F83DC55AF9F";
        private const String SubstanceAdministration = "932A3C7E-AD77-450A-8A1F-030FC2855450";
        private const String Condition = "1987C53C-7AB8-4461-9EBC-0D428744A8C0";
        private const String Registration = "6BE8D358-F591-4A3A-9A57-1889B0147C7E";
        private const String Observation = "28D022C6-8A8B-47C4-9E6A-2BC67308739E";
        private const String Inform = "192F1768-D39E-409D-87BE-5AFD0EE0D1FE";
        private const String Encounter = "54B52119-1709-4098-8911-5DF6D6C84140";
        private const String Battery = "676DE278-64AA-44F2-9B69-60D61FC1F5F5";
        private const String Act = "D874424E-C692-4FD8-B94E-642E1CBF83E9";
        private const String Procedure = "8CC5EF0D-3911-4D99-937F-6CFDC2A27D55";
        private const String CareProvision = "1071D24E-6FE9-480F-8A20-B1825AE4D707";
        private const String AccountManagement = "CA44A469-81D7-4484-9189-CA1D55AFECBC";
        private const String Supply = "A064984F-9847-4480-8BEA-DDDF64B3C77C";

        /// <summary>
        /// To model instance
        /// </summary>
        public virtual TActType ToModelInstance<TActType>(DbActVersion dbInstance, DbAct actInstance, DataContext context, IPrincipal principal) where TActType : Core.Model.Acts.Act, new()
        {

            var retVal = m_mapper.MapDomainInstance<DbActVersion, TActType>(dbInstance);
            if (retVal == null) return null;

            retVal.ClassConceptKey = actInstance?.ClassConceptKey;
            retVal.MoodConceptKey = actInstance?.MoodConceptKey;
            retVal.TemplateKey = actInstance?.TemplateKey;
            return retVal;
        }

        /// <summary>
        /// Create an appropriate entity based on the class code
        /// </summary>
        public override Core.Model.Acts.Act ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            // Alright first, which type am I mapping to?

            if (dataInstance == null) return null;

            DbActVersion dbActVersion = (dataInstance as CompositeResult)?.Values.OfType<DbActVersion>().FirstOrDefault() ?? dataInstance as DbActVersion ?? context.FirstOrDefault<DbActVersion>(o => o.VersionKey == (dataInstance as DbActSubTable).ParentKey);
            DbAct dbAct = (dataInstance as CompositeResult)?.Values.OfType<DbAct>().FirstOrDefault() ?? context.FirstOrDefault<DbAct>(o => o.Key == dbActVersion.Key);
            Act retVal = null;

            // 
            switch (dbAct.ClassConceptKey.ToString().ToUpper())
            {
                case ControlAct:
                    retVal = new ControlActPersistenceService().ToModelInstance(
                                (dataInstance as CompositeResult)?.Values.OfType<DbControlAct>().FirstOrDefault() ?? context.FirstOrDefault<DbControlAct>(o => o.ParentKey == dbActVersion.VersionKey),
                                dbActVersion,
                                dbAct,
                                context,
                                principal);
                    break;
                case SubstanceAdministration:
                    retVal = new SubstanceAdministrationPersistenceService().ToModelInstance(
                                (dataInstance as CompositeResult)?.Values.OfType<DbSubstanceAdministration>().FirstOrDefault() ?? context.FirstOrDefault<DbSubstanceAdministration>(o => o.ParentKey == dbActVersion.VersionKey),
                                dbActVersion,
                                dbAct,
                                context,
                                principal);
                    break;
                case Observation:
                    var dbObs = (dataInstance as CompositeResult)?.Values.OfType<DbObservation>().FirstOrDefault() ?? context.FirstOrDefault<DbObservation>(o => o.ParentKey == dbActVersion.VersionKey);
                    if (dbObs == null)
                    {
                        this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Warning, -10293, "Observation {0} is missing observation data! Even though class code is {1}", dbAct.Key, dbAct.ClassConceptKey);
                        retVal = this.ToModelInstance<Core.Model.Acts.Act>(dbActVersion, dbAct, context, principal);
                    }
                    else
                        switch (dbObs.ValueType)
                        {
                            case "ST":
                                retVal = new TextObservationPersistenceService().ToModelInstance(
                                    (dataInstance as CompositeResult)?.Values.OfType<DbTextObservation>().FirstOrDefault() ?? context.FirstOrDefault<DbTextObservation>(o => o.ParentKey == dbObs.ParentKey),
                                    dbObs,
                                    dbActVersion,
                                    dbAct,
                                    context,
                                    principal);
                                break;
                            case "CD":
                                retVal = new CodedObservationPersistenceService().ToModelInstance(
                                    (dataInstance as CompositeResult)?.Values.OfType<DbCodedObservation>().FirstOrDefault() ?? context.FirstOrDefault<DbCodedObservation>(o => o.ParentKey == dbObs.ParentKey),
                                    dbObs,
                                    dbActVersion,
                                    dbAct,
                                    context,
                                    principal);
                                break;
                            case "PQ":
                                retVal = new QuantityObservationPersistenceService().ToModelInstance(
                                    (dataInstance as CompositeResult)?.Values.OfType<DbQuantityObservation>().FirstOrDefault() ?? context.FirstOrDefault<DbQuantityObservation>(o => o.ParentKey == dbObs.ParentKey),
                                    dbObs,
                                    dbActVersion,
                                    dbAct,
                                    context,
                                    principal);
                                break;
                            default:
                                retVal = new ObservationPersistenceService().ToModelInstance(
                                    dbObs,
                                    dbActVersion,
                                    dbAct,
                                    context,
                                    principal);
                                break;
                        }
                    break;
                case Encounter:
                    retVal = new EncounterPersistenceService().ToModelInstance(
                                (dataInstance as CompositeResult)?.Values.OfType<DbPatientEncounter>().FirstOrDefault() ?? context.FirstOrDefault<DbPatientEncounter>(o => o.ParentKey == dbActVersion.VersionKey),
                                dbActVersion,
                                dbAct,
                                context,
                                principal);
                    break;
                case Condition:
                default:
                    retVal = this.ToModelInstance<Core.Model.Acts.Act>(dbActVersion, dbAct, context, principal);
                    break;
            }

            retVal.LoadAssociations(context, principal);
            return retVal;
        }

        /// <summary>
        /// Override cache conversion
        /// </summary>
        protected override Act CacheConvert(object dataInstance, DataContext context, IPrincipal principal)
        {
            if (dataInstance == null) return null;
            DbActVersion dbActVersion = (dataInstance as CompositeResult)?.Values.OfType<DbActVersion>().FirstOrDefault() ?? dataInstance as DbActVersion ?? context.FirstOrDefault<DbActVersion>(o => o.VersionKey == (dataInstance as DbActSubTable).ParentKey);
            DbAct dbAct = (dataInstance as CompositeResult)?.Values.OfType<DbAct>().FirstOrDefault() ?? context.FirstOrDefault<DbAct>(o => o.Key == dbActVersion.Key);
            Act retVal = null;
            var cache= new AdoPersistenceCache(context);

            if (!dbActVersion.ObsoletionTime.HasValue)
                switch (dbAct.ClassConceptKey.ToString().ToUpper())
                {
                    case ControlAct:
                        retVal = cache?.GetCacheItem<ControlAct>(dbAct.Key);
                        break;
                    case SubstanceAdministration:
                        retVal = cache?.GetCacheItem<SubstanceAdministration>(dbAct.Key);
                        break;
                    case Observation:
                        var dbObs = (dataInstance as CompositeResult)?.Values.OfType<DbObservation>().FirstOrDefault() ?? context.FirstOrDefault<DbObservation>(o => o.ParentKey == dbActVersion.VersionKey);
                        if (dbObs != null)
                            switch (dbObs.ValueType)
                            {
                                case "ST":
                                    retVal = cache?.GetCacheItem<TextObservation>(dbAct.Key);
                                    break;
                                case "CD":
                                    retVal = cache?.GetCacheItem<CodedObservation>(dbAct.Key);
                                    break;
                                case "PQ":
                                    retVal = cache?.GetCacheItem<QuantityObservation>(dbAct.Key);
                                    break;
                            }
                        break;
                    case Encounter:
                        retVal = cache?.GetCacheItem<PatientEncounter>(dbAct.Key);
                        break;
                    case Condition:
                    default:
                        retVal = cache?.GetCacheItem<Act>(dbAct.Key);
                        break;
                }

            // Return cache value
            if (retVal != null)
                return retVal;
            else
                return base.CacheConvert(dataInstance, context, principal);
        }

        /// <summary>
        /// Insert the act into the database
        /// </summary>
        public Core.Model.Acts.Act InsertCoreProperties(DataContext context, Core.Model.Acts.Act data, IPrincipal principal)
        {
            if (data.ClassConcept != null) data.ClassConcept = data.ClassConcept?.EnsureExists(context, principal) as Concept;
            if (data.MoodConcept != null) data.MoodConcept = data.MoodConcept?.EnsureExists(context, principal) as Concept;
            if (data.ReasonConcept != null) data.ReasonConcept = data.ReasonConcept?.EnsureExists(context, principal) as Concept;
            if (data.StatusConcept != null) data.StatusConcept = data.StatusConcept?.EnsureExists(context, principal) as Concept;
            if (data.TypeConcept != null) data.TypeConcept = data.TypeConcept?.EnsureExists(context, principal) as Concept;
            if (data.Template != null) data.Template = data.Template?.EnsureExists(context, principal) as TemplateDefinition;

            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.MoodConceptKey = data.MoodConcept?.Key ?? data.MoodConceptKey;
            data.ReasonConceptKey = data.ReasonConcept?.Key ?? data.ReasonConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey ?? StatusKeys.New;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;

            // Do the insert
            var retVal = base.InsertInternal(context, data, principal);

            if (data.Extensions != null && data.Extensions.Any())
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActExtension, DbActExtension>(
                   data.Extensions.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Identifiers != null && data.Identifiers.Any())
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActIdentifier, DbActIdentifier>(
                   data.Identifiers.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Notes != null && data.Notes.Any())
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActNote, DbActNote>(
                   data.Notes.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Participations != null && data.Participations.Any())
            {
                data.Participations = data.Participations.Where(o => o != null && !o.IsEmpty()).Select(o => new ActParticipation(o.ParticipationRole?.EnsureExists(context, principal)?.Key ?? o.ParticipationRoleKey , o.PlayerEntityKey) { Quantity = o.Quantity }).ToList();
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActParticipation, DbActParticipation>(
                   data.Participations,
                    retVal,
                    context,
                    principal);
            }

            if (data.Relationships != null && data.Relationships.Any())
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActRelationship, DbActRelationship>(
                   data.Relationships.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Tags != null && data.Tags.Any())
                base.UpdateAssociatedItems<Core.Model.DataTypes.ActTag, DbActTag>(
                   data.Tags.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Protocols != null && data.Protocols.Any())
                foreach (var p in data.Protocols)
                {
                    var proto = p.Protocol?.EnsureExists(context, principal);
                    if (proto == null) // maybe we can retrieve the protocol from the protocol repository?
                    {
                        int t = 0;
                        proto = ApplicationContext.Current.GetService<IClinicalProtocolRepositoryService>().FindProtocol(o => o.Key == p.ProtocolKey, 0, 1, out t).FirstOrDefault();
                        proto = proto.EnsureExists(context, principal);
                    }
                    if (proto != null)
                        context.Insert(new DbActProtocol()
                        {
                            SourceKey = retVal.Key.Value,
                            ProtocolKey = proto.Key.Value,
                            Sequence = p.Sequence
                        });
                }

            return retVal;
        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        public Core.Model.Acts.Act UpdateCoreProperties(DataContext context, Core.Model.Acts.Act data, IPrincipal principal)
        {
            if (data.ClassConcept != null) data.ClassConcept = data.ClassConcept?.EnsureExists(context, principal) as Concept;
            if (data.MoodConcept != null) data.MoodConcept = data.MoodConcept?.EnsureExists(context, principal) as Concept;
            if (data.ReasonConcept != null) data.ReasonConcept = data.ReasonConcept?.EnsureExists(context, principal) as Concept;
            if (data.StatusConcept != null) data.StatusConcept = data.StatusConcept?.EnsureExists(context, principal) as Concept;
            if (data.TypeConcept != null) data.TypeConcept = data.TypeConcept?.EnsureExists(context, principal) as Concept;
            if (data.Template != null) data.Template = data.Template?.EnsureExists(context, principal) as TemplateDefinition;

            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.MoodConceptKey = data.MoodConcept?.Key ?? data.MoodConceptKey;
            data.ReasonConceptKey = data.ReasonConcept?.Key ?? data.ReasonConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey ?? StatusKeys.New;

            // Do the update
            var retVal = base.UpdateInternal(context, data, principal);

            if (data.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActExtension, DbActExtension>(
                   data.Extensions.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Identifiers != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActIdentifier, DbActIdentifier>(
                   data.Identifiers.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActNote, DbActNote>(
                   data.Notes.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            if (data.Participations != null)
            {
                // Correct mixed keys
                if(AdoPersistenceService.GetConfiguration().DataCorrectionKeys.Contains("edmonton-participation-keyfix"))
                {
                    // Obsolete all
                    foreach(var itm in context.Query<DbActParticipation>(o=>o.SourceKey == retVal.Key && o.ObsoleteVersionSequenceId == null && o.ParticipationRoleKey == ActParticipationKey.Consumable)) {
                        itm.ObsoleteVersionSequenceId = retVal.VersionSequence;
                        context.Update(itm);
                    }
                    // Now we want to re-point to correct the issue
                    foreach (var itm in context.Query<DbActParticipation>(o => o.SourceKey == retVal.Key && o.ParticipationRoleKey == ActParticipationKey.Consumable && o.ObsoleteVersionSequenceId == retVal.VersionSequence))
                    {

                        var dItm = data.Participations.Find(o => o.Key == itm.Key);
                        if(dItm != null)
                            itm.TargetKey = dItm.PlayerEntityKey.Value;
                        itm.ObsoleteVersionSequenceId = null;
                        context.Update(itm);
                    }
                }

                // Update versioned association items
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActParticipation, DbActParticipation>(
                      data.Participations.Where(o => o != null && !o.IsEmpty()),
                        retVal,
                        context,
                        principal);


            }

            if (data.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActRelationship, DbActRelationship>(
                   data.Relationships.Where(o => o != null && !o.IsEmpty() && (o.SourceEntityKey == data.Key || !o.SourceEntityKey.HasValue)),
                    retVal,
                    context,
                    principal);

            if (data.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.ActTag, DbActTag>(
                   data.Tags.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Obsolete the act
        /// </summary>
        /// <param name="context"></param>
        public override Core.Model.Acts.Act ObsoleteInternal(DataContext context, Core.Model.Acts.Act data, IPrincipal principal)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Perform insert
        /// </summary>
        public override Act InsertInternal(DataContext context, Act data, IPrincipal principal)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case ControlAct:
                    return new ControlActPersistenceService().InsertInternal(context, data.Convert<ControlAct>(), principal);
                case SubstanceAdministration:
                    return new SubstanceAdministrationPersistenceService().InsertInternal(context, data.Convert<SubstanceAdministration>(), principal);
                case Observation:
                    switch (data.GetType().Name)
                    {
                        case "TextObservation":
                            return new TextObservationPersistenceService().InsertInternal(context, data.Convert<TextObservation>(), principal);
                        case "CodedObservation":
                            return new CodedObservationPersistenceService().InsertInternal(context, data.Convert<CodedObservation>(), principal);
                        case "QuantityObservation":
                            return new QuantityObservationPersistenceService().InsertInternal(context, data.Convert<QuantityObservation>(), principal);
                        default:
                            return this.InsertCoreProperties(context, data, principal);
                    }
                case Encounter:
                    return new EncounterPersistenceService().InsertInternal(context, data.Convert<PatientEncounter>(), principal);
                case Condition:
                default:
                    return this.InsertCoreProperties(context, data, principal);

            }
        }

        /// <summary>
        /// Perform update
        /// </summary>
        public override Act UpdateInternal(DataContext context, Act data, IPrincipal principal)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case ControlAct:
                    return new ControlActPersistenceService().UpdateInternal(context, data.Convert<ControlAct>(), principal);
                case SubstanceAdministration:
                    return new SubstanceAdministrationPersistenceService().UpdateInternal(context, data.Convert<SubstanceAdministration>(), principal);
                case Observation:
                    switch (data.GetType().Name)
                    {
                        case "TextObservation":
                            return new TextObservationPersistenceService().UpdateInternal(context, data.Convert<TextObservation>(), principal);
                        case "CodedObservation":
                            return new CodedObservationPersistenceService().UpdateInternal(context, data.Convert<CodedObservation>(), principal);
                        case "QuantityObservation":
                            return new QuantityObservationPersistenceService().UpdateInternal(context, data.Convert<QuantityObservation>(), principal);
                        default:
                            return this.UpdateCoreProperties(context, data, principal);
                    }
                case Encounter:
                    return new EncounterPersistenceService().UpdateInternal(context, data.Convert<PatientEncounter>(), principal);
                case Condition:
                default:
                    return this.UpdateCoreProperties(context, data, principal);

            }
        }
    }
}
