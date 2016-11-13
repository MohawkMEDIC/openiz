/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-3
 */
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service which persists ACT classes
    /// </summary>
    public class ActPersistenceService : VersionedDataPersistenceService<Core.Model.Acts.Act, Data.ActVersion, Data.Act>
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
        public virtual TActType ToModelInstance<TActType>(Data.ActVersion dbInstance, ModelDataContext context, IPrincipal principal) where TActType : Core.Model.Acts.Act, new()
        {
            var retVal = m_mapper.MapDomainInstance<Data.ActVersion, TActType>(dbInstance);

            return retVal;
        }

        /// <summary>
        /// Create an appropriate entity based on the class code
        /// </summary>
        public override Core.Model.Acts.Act ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            // Alright first, which type am I mapping to?
            var dbAct = dataInstance as Data.ActVersion;
            switch (dbAct.Act.ClassConceptId.ToString().ToUpper())
            {
                case ControlAct:
                    return new ControlActPersistenceService().ToModelInstance(dataInstance, context, principal);
                case SubstanceAdministration:
                    return new SubstanceAdministrationPersistenceService().ToModelInstance(dataInstance, context, principal);
                case Condition:
                case Observation:
                    var dbObs = context.GetTable<Data.Observation>().First(o => o.ActVersionId == dbAct.ActVersionId);
                    switch(dbObs.ValueType)
                    {
                        case "ST":
                            return new TextObservationPersistenceService().ToModelInstance(
                                context.GetTable<Data.TextObservation>().Where(o=>o.ActVersionId == dbObs.ActVersionId).First(),
                                dbAct, 
                                dbObs, 
                                context,
                                principal);
                        case "CD":
                            return new CodedObservationPersistenceService().ToModelInstance(
                                context.GetTable<Data.CodedObservation>().Where(o=>o.ActVersionId == dbObs.ActVersionId).First(),
                                dbAct, 
                                dbObs, 
                                context,
                                principal);
                        case "PQ":
                            return new QuantityObservationPersistenceService().ToModelInstance(
                                context.GetTable<Data.QuantityObservation>().Where(o=>o.ActVersionId == dbObs.ActVersionId).First(),
                                dbAct, 
                                dbObs, 
                                context, 
                                principal);
                        default:
                            return base.ToModelInstance(dataInstance, context, principal);
                    }
                case Encounter:
                    return new EncounterPersistenceService().ToModelInstance(dataInstance, context, principal);
                default:
                    return this.ToModelInstance<Core.Model.Acts.Act>(dbAct, context, principal);

            }
        }

        /// <summary>
        /// Insert the act into the database
        /// </summary>
        public override Core.Model.Acts.Act Insert(ModelDataContext context, Core.Model.Acts.Act data, IPrincipal principal)
        {
            data.ClassConcept?.EnsureExists(context, principal);
            data.MoodConcept?.EnsureExists(context, principal);
            data.ReasonConcept?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);
            data.TypeConcept?.EnsureExists(context, principal);

            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.MoodConceptKey = data.MoodConcept?.Key ?? data.MoodConceptKey;
            data.ReasonConceptKey = data.ReasonConcept?.Key ?? data.ReasonConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey ?? StatusKeys.New;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;

            // Do the insert
            var retVal = base.Insert(context, data, principal);

            if (retVal.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActExtension, Data.ActExtension>(
                    retVal.Extensions.GetLocked(),
                    retVal,
                    context, 
                    principal);

            if (retVal.Identifiers != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActIdentifier, Data.ActIdentifier>(
                    retVal.Identifiers.GetLocked(),
                    retVal,
                    context, 
                    principal);

            if (retVal.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActNote, Data.ActNote>(
                    retVal.Notes.GetLocked(),
                    retVal,
                    context, 
                    principal);

            if (retVal.Participations != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActParticipation, Data.ActParticipation>(
                    retVal.Participations.GetLocked(),
                    retVal,
                    context, 
                    principal);

            if (retVal.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActRelationship, Data.ActRelationship>(
                    retVal.Relationships.GetLocked(),
                    retVal,
                    context, 
                    principal);

            if (retVal.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.ActTag, Data.ActTag>(
                    retVal.Tags.GetLocked(),
                    retVal,
                    context, 
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        public override Core.Model.Acts.Act Update(ModelDataContext context, Core.Model.Acts.Act data, IPrincipal principal)
        {
            data.ClassConcept?.EnsureExists(context, principal);
            data.MoodConcept?.EnsureExists(context, principal);
            data.ReasonConcept?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);
            data.TypeConcept?.EnsureExists(context, principal);

            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.MoodConceptKey = data.MoodConcept?.Key ?? data.MoodConceptKey;
            data.ReasonConceptKey = data.ReasonConcept?.Key ?? data.ReasonConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey ?? StatusKeys.New;

            // Do the update
            var retVal = base.Update(context, data, principal);

            if (retVal.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActExtension, Data.ActExtension>(
                    retVal.Extensions.GetLocked(),
                    retVal,
                    context,
                    principal);

            if (retVal.Identifiers != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActIdentifier, Data.ActIdentifier>(
                    retVal.Identifiers.GetLocked(),
                    retVal,
                    context,
                    principal);

            if (retVal.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ActNote, Data.ActNote>(
                    retVal.Notes.GetLocked(),
                    retVal,
                    context,
                    principal);

            if (retVal.Participations != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActParticipation, Data.ActParticipation>(
                    retVal.Participations.GetLocked(),
                    retVal,
                    context,
                    principal);

            if (retVal.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Acts.ActRelationship, Data.ActRelationship>(
                    retVal.Relationships.GetLocked(),
                    retVal,
                    context,
                    principal);

            if (retVal.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.ActTag, Data.ActTag>(
                    retVal.Tags.GetLocked(),
                    retVal,
                    context,
                    principal);

            return retVal;
        }

        /// <summary>
        /// Obsolete the act
        /// </summary>
        /// <param name="context"></param>
        public override Core.Model.Acts.Act Obsolete(ModelDataContext context, Core.Model.Acts.Act data, IPrincipal principal)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Obsolete(context, data, principal);
        }
    }
}
