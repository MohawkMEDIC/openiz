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
 * Date: 2016-8-3
 */
using System;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;
using System.Linq;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Persistence class for observations
    /// </summary>
    public abstract class ObservationPersistenceService<TObservation, TDbObservation> : ActDerivedPersistenceService<TObservation, TDbObservation>
        where TObservation : Core.Model.Acts.Observation, new()
        where TDbObservation : class, IDbObservation, new()
    {

        /// <summary>
        /// Convert from model instance
        /// </summary>
        public override object FromModelInstance(TObservation modelInstance, ModelDataContext context, IPrincipal princpal)
        {
            var retVal = m_mapper.MapModelInstance<TObservation, TDbObservation>(modelInstance);
            retVal.Observation = m_mapper.MapModelInstance<Core.Model.Acts.Observation, Data.Observation>(modelInstance);
            return retVal;
        }

        /// <summary>
        /// Convert a data act and observation instance to an observation
        /// </summary>
        public virtual TObservation ToModelInstance(TDbObservation dataInstance, Data.ActVersion actInstance, Data.Observation obsInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = m_actPersister.ToModelInstance<TObservation>(actInstance, context, principal);
            if(obsInstance.InterpretationConceptId != null)
                retVal.InterpretationConceptKey = obsInstance.InterpretationConceptId;

            return retVal;
        }

        /// <summary>
        /// Insert the specified observation into the database
        /// </summary>
        public override TObservation Insert(ModelDataContext context, TObservation data, IPrincipal principal)
        {
            data.InterpretationConcept?.EnsureExists(context, principal);
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;

            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Updates the specified observation
        /// </summary>
        public override TObservation Update(ModelDataContext context, TObservation data, IPrincipal principal)
        {
            data.InterpretationConcept?.EnsureExists(context, principal);
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;

            return base.Update(context, data, principal);
        }
    }

    /// <summary>
    /// Text observation service
    /// </summary>
    public class TextObservationPersistenceService : ObservationPersistenceService<Core.Model.Acts.TextObservation, Data.TextObservation>
    {
        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Acts.TextObservation modelInstance, ModelDataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as IDbObservation;
            retVal.Observation.ValueType = "ST";
            return retVal;
        }

        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public override Core.Model.Acts.TextObservation ToModelInstance(Data.TextObservation dataInstance, Data.ActVersion actInstance, Data.Observation obsInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context, principal);
            retVal.Value = dataInstance.Value;
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override Core.Model.Acts.TextObservation ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var iddat = dataInstance as IDbVersionedData;
            var textObs = dataInstance as Data.TextObservation ?? context.GetTable<Data.TextObservation>().Where(o => o.ActVersionId == iddat.VersionId).First();
            var dba = dataInstance as Data.ActVersion ?? context.GetTable<Data.ActVersion>().Where(o => o.ActVersionId == iddat.VersionId).First();
            var dbo = context.GetTable<Data.Observation>().Where(o => o.ActVersionId == textObs.ActVersionId).First();
            return this.ToModelInstance(textObs, dba, dbo, context, principal);
        }
    }

    /// <summary>
    /// Coded observation service
    /// </summary>
    public class CodedObservationPersistenceService : ObservationPersistenceService<Core.Model.Acts.CodedObservation, Data.CodedObservation>
    {
        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Acts.CodedObservation modelInstance, ModelDataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as IDbObservation;
            retVal.Observation.ValueType = "CD";
            return retVal;
        }

        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public override Core.Model.Acts.CodedObservation ToModelInstance(Data.CodedObservation dataInstance, Data.ActVersion actInstance, Data.Observation obsInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context, principal);
            if(dataInstance.ValueConcept != null)
                retVal.ValueKey = dataInstance.ValueConceptId;
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override Core.Model.Acts.CodedObservation ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var iddat = dataInstance as IDbVersionedData;
            var codeObs = dataInstance as Data.CodedObservation ?? context.GetTable<Data.CodedObservation>().Where(o => o.ActVersionId == iddat.VersionId).First();
            var dba = dataInstance as Data.ActVersion ?? context.GetTable<Data.ActVersion>().Where(o => o.ActVersionId == codeObs.ActVersionId).First();
            var dbo = context.GetTable<Data.Observation>().Where(o => o.ActVersionId == codeObs.ActVersionId).First();
            return this.ToModelInstance(codeObs, dba, dbo, context, principal);
        }

        /// <summary>
        /// Insert the observation
        /// </summary>
        public override Core.Model.Acts.CodedObservation Insert(ModelDataContext context, Core.Model.Acts.CodedObservation data, IPrincipal principal)
        {
            data.Value?.EnsureExists(context, principal);
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override Core.Model.Acts.CodedObservation Update(ModelDataContext context, Core.Model.Acts.CodedObservation data, IPrincipal principal)
        {
            data.Value?.EnsureExists(context, principal);
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            return base.Update(context, data, principal);
        }
    }

    /// <summary>
    /// Quantity observation persistence service
    /// </summary>
    public class QuantityObservationPersistenceService : ObservationPersistenceService<Core.Model.Acts.QuantityObservation, Data.QuantityObservation>
    {
        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Core.Model.Acts.QuantityObservation modelInstance, ModelDataContext context, IPrincipal princpal)
        {
            var retVal = base.FromModelInstance(modelInstance, context, princpal) as IDbObservation;
            retVal.Observation.ValueType = "PQ";
            return retVal;
        }

        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public override Core.Model.Acts.QuantityObservation ToModelInstance(Data.QuantityObservation dataInstance, Data.ActVersion actInstance, Data.Observation obsInstance, ModelDataContext context, IPrincipal principal)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context, principal);
            if (dataInstance.UnitOfMeasureConceptId != null)
                retVal.UnitOfMeasureKey = dataInstance.UnitOfMeasureConceptId;
            retVal.Value = dataInstance.Quantity;
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override Core.Model.Acts.QuantityObservation ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var iddat = dataInstance as IDbVersionedData;
            var qObs = dataInstance as Data.QuantityObservation ?? context.GetTable<Data.QuantityObservation>().Where(o => o.ActVersionId == iddat.VersionId).First();
            var dba = dataInstance as Data.ActVersion ?? context.GetTable<Data.ActVersion>().Where(o => o.ActVersionId == qObs.ActVersionId).First();
            var dbo = context.GetTable<Data.Observation>().Where(o => o.ActVersionId == qObs.ActVersionId).First();
            return this.ToModelInstance(qObs, dba, dbo, context, principal);
        }

        /// <summary>
        /// Insert the observation
        /// </summary>
        public override Core.Model.Acts.QuantityObservation Insert(ModelDataContext context, Core.Model.Acts.QuantityObservation data, IPrincipal principal)
        {
            data.UnitOfMeasure?.EnsureExists(context, principal);
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override Core.Model.Acts.QuantityObservation Update(ModelDataContext context, Core.Model.Acts.QuantityObservation data, IPrincipal principal)
        {
            data.UnitOfMeasure?.EnsureExists(context, principal);
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            return base.Update(context, data, principal);
        }
    }
}