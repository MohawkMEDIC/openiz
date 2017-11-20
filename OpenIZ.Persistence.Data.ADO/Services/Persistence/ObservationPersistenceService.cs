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
using System;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using System.Security.Principal;
using System.Linq;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.OrmLite;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Persistence class for observations
    /// </summary>
    internal class ObservationPersistenceService : ActDerivedPersistenceService<Observation, DbObservation>
    {

        /// <summary>
        /// Convert from model instance
        /// </summary>
        public override object FromModelInstance(Observation modelInstance, DataContext context, IPrincipal princpal)
        {
            var retVal = m_mapper.MapModelInstance<Observation, DbObservation>(modelInstance);
            if (modelInstance is TextObservation)
                retVal.ValueType = "ST";
            else if (modelInstance is CodedObservation)
                retVal.ValueType = "CD";
            else if (modelInstance is QuantityObservation)
                retVal.ValueType = "PQ";
            return retVal;
        }

        /// <summary>
        /// Convert a data act and observation instance to an observation
        /// </summary>
        public virtual Observation ToModelInstance(DbObservation dataInstance, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            return this.ToModelInstance<Observation>(dataInstance, actVersionInstance, actInstance, context, principal);
        }


        /// <summary>
        /// Convert a data act and observation instance to an observation
        /// </summary>
        public virtual TObservation ToModelInstance<TObservation>(DbObservation dataInstance, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
            where TObservation : Observation, new()
        {
            var retVal = m_actPersister.ToModelInstance<TObservation>(actVersionInstance, actInstance, context, principal);
            if (retVal == null) return null;

            if (dataInstance.InterpretationConceptKey != null)
                retVal.InterpretationConceptKey = dataInstance.InterpretationConceptKey;

            return retVal;
        }

        /// <summary>
        /// Insert the specified observation into the database
        /// </summary>
        public override Observation InsertInternal(DataContext context, Observation data, IPrincipal principal)
        {
            if(data.InterpretationConcept != null) data.InterpretationConcept = data.InterpretationConcept?.EnsureExists(context, principal) as Concept;
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Updates the specified observation
        /// </summary>
        public override Observation UpdateInternal(DataContext context, Observation data, IPrincipal principal)
        {
            if (data.InterpretationConcept != null) data.InterpretationConcept = data.InterpretationConcept?.EnsureExists(context, principal) as Concept;
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;

            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Updates the specified observation
        /// </summary>
        public override Observation ObsoleteInternal(DataContext context, Observation data, IPrincipal principal)
        {
            if (data.InterpretationConcept != null) data.InterpretationConcept = data.InterpretationConcept?.EnsureExists(context, principal) as Concept;
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;

            return base.ObsoleteInternal(context, data, principal);
        }
    }

    /// <summary>
    /// Text observation service
    /// </summary>
    public class TextObservationPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.TextObservation, DbTextObservation, CompositeResult<DbTextObservation, DbObservation, DbActVersion, DbAct>>
    {

        private ObservationPersistenceService m_observationPersistence = new ObservationPersistenceService();

        /// <summary>
        /// Query internal
        /// </summary>
        public override IEnumerable<TextObservation> QueryInternal(DataContext context, Expression<Func<TextObservation, bool>> query, Guid queryId, int offset, int? count, out int totalResults, IPrincipal principal, bool countResults = true)
        {
            var parm = query.Parameters[0];
            query = Expression.Lambda<Func<TextObservation, bool>>(Expression.MakeBinary(ExpressionType.AndAlso, query.Body, Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(parm, typeof(Observation).GetProperty(nameof(Observation.ValueType))), Expression.Constant("ED"))), parm);
            return base.QueryInternal(context, query, queryId, offset, count, out totalResults, principal, countResults);
        }

        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public Core.Model.Acts.TextObservation ToModelInstance(DbTextObservation dataInstance, DbObservation obsInstance, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            var retVal = this.m_observationPersistence.ToModelInstance<TextObservation>(obsInstance, actVersionInstance, actInstance, context, principal);
            if (retVal == null) return null;
            retVal.Value = dataInstance.Value;
            return retVal;
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public override TextObservation InsertInternal(DataContext context, TextObservation data, IPrincipal principal)
        {
            var obsData = this.m_observationPersistence.InsertInternal(context, data, principal);
            context.Insert(new DbTextObservation()
            {
                ParentKey = obsData.VersionKey.Value,
                Value = data.Value
            });
            return data;
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        public override TextObservation UpdateInternal(DataContext context, TextObservation data, IPrincipal principal)
        {
            var obsData = this.m_observationPersistence.UpdateInternal(context, data, principal);
            context.Insert(new DbTextObservation()
            {
                ParentKey = obsData.VersionKey.Value,
                Value = data.Value
            });
            return data;
        }

        /// <summary>
        /// Obsolete the observation
        /// </summary>
        public override TextObservation ObsoleteInternal(DataContext context, TextObservation data, IPrincipal principal)
        {
            var obsData = this.m_observationPersistence.ObsoleteInternal(context, data, principal);
            context.Insert(new DbTextObservation()
            {
                ParentKey = obsData.VersionKey.Value,
                Value = data.Value
            });
            return data;
        }
    }

    /// <summary>
    /// Coded observation service
    /// </summary>
    public class CodedObservationPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.CodedObservation, DbCodedObservation, CompositeResult<DbCodedObservation, DbObservation, DbActVersion, DbAct>>
    {

        private ObservationPersistenceService m_observationPersistence = new ObservationPersistenceService();
        /// <summary>
        /// Query internal
        /// </summary>
        public override IEnumerable<CodedObservation> QueryInternal(DataContext context, Expression<Func<CodedObservation, bool>> query, Guid queryId, int offset, int? count, out int totalResults, IPrincipal principal, bool countResults = true)
        {
            var parm = query.Parameters[0];
            query = Expression.Lambda<Func<CodedObservation, bool>>(Expression.MakeBinary(ExpressionType.AndAlso, query.Body, Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(parm, typeof(Observation).GetProperty(nameof(Observation.ValueType))), Expression.Constant("CD"))), parm);
            return base.QueryInternal(context, query, queryId, offset, count, out totalResults, principal, countResults);
        }

        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public Core.Model.Acts.CodedObservation ToModelInstance(DbCodedObservation dataInstance, DbObservation obsInstance, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            var retVal = this.m_observationPersistence.ToModelInstance<CodedObservation>(obsInstance, actVersionInstance, actInstance, context, principal);
            if (retVal == null) return null;
            if (dataInstance.Value != null)
                retVal.ValueKey = dataInstance.Value;
            return retVal;
        }

        /// <summary>
        /// Insert the observation
        /// </summary>
        public override Core.Model.Acts.CodedObservation InsertInternal(DataContext context, Core.Model.Acts.CodedObservation data, IPrincipal principal)
        {
            if(data.Value != null) data.Value =data.Value?.EnsureExists(context, principal) as Concept;
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            var obsData = this.m_observationPersistence.InsertInternal(context, data, principal);
            context.Insert(new DbCodedObservation()
            {
                ParentKey = obsData.VersionKey.Value,
                Value = data.ValueKey
            });
            return data;
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override Core.Model.Acts.CodedObservation UpdateInternal(DataContext context, Core.Model.Acts.CodedObservation data, IPrincipal principal)
        {
            if (data.Value != null) data.Value = data.Value?.EnsureExists(context, principal) as Concept;
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            var obsData = this.m_observationPersistence.UpdateInternal(context, data, principal);
            context.Insert(new DbCodedObservation()
            {
                ParentKey = obsData.VersionKey.Value,
                Value = data.ValueKey
            });
            return data;

        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override Core.Model.Acts.CodedObservation ObsoleteInternal(DataContext context, Core.Model.Acts.CodedObservation data, IPrincipal principal)
        {
            if (data.Value != null) data.Value = data.Value?.EnsureExists(context, principal) as Concept;
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            var obsData = this.m_observationPersistence.ObsoleteInternal(context, data, principal);
            context.Insert(new DbCodedObservation()
            {
                ParentKey = obsData.VersionKey.Value,
                Value = data.ValueKey
            });
            return data;

        }
    }

    /// <summary>
    /// Quantity observation persistence service
    /// </summary>
    public class QuantityObservationPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.QuantityObservation, DbQuantityObservation, CompositeResult<DbQuantityObservation, DbObservation, DbActVersion, DbAct>>
    {

        private ObservationPersistenceService m_observationPersistence = new ObservationPersistenceService();

        /// <summary>
        /// Query internal
        /// </summary>
        public override IEnumerable<QuantityObservation> QueryInternal(DataContext context, Expression<Func<QuantityObservation, bool>> query, Guid queryId, int offset, int? count, out int totalResults, IPrincipal principal, bool countResults = true)
        {
            var parm = query.Parameters[0];
            query = Expression.Lambda<Func<QuantityObservation, bool>>(Expression.MakeBinary(ExpressionType.AndAlso, query.Body, Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(parm, typeof(Observation).GetProperty(nameof(Observation.ValueType))), Expression.Constant("PQ"))), parm);
            return base.QueryInternal(context, query, queryId, offset, count, out totalResults, principal, countResults);
        }


        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public Core.Model.Acts.QuantityObservation ToModelInstance(DbQuantityObservation dataInstance, DbObservation obsInstance, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            var retVal = this.m_observationPersistence.ToModelInstance<QuantityObservation>(obsInstance, actVersionInstance, actInstance, context, principal);
            if (retVal == null) return null;
            if (dataInstance.UnitOfMeasureKey != null)
                retVal.UnitOfMeasureKey = dataInstance.UnitOfMeasureKey;
            retVal.Value = dataInstance.Value;
            return retVal;
        }
        
        /// <summary>
        /// Insert the observation
        /// </summary>
        public override Core.Model.Acts.QuantityObservation InsertInternal(DataContext context, Core.Model.Acts.QuantityObservation data, IPrincipal principal)
        {
            if(data.UnitOfMeasure != null) data.UnitOfMeasure = data.UnitOfMeasure?.EnsureExists(context, principal) as Concept;
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            var retVal = this.m_observationPersistence.InsertInternal(context, data, principal);
            context.Insert(new DbQuantityObservation()
            {
                ParentKey = data.VersionKey.Value,
                UnitOfMeasureKey = data.UnitOfMeasureKey.Value,
                Value = data.Value

            });
            return data;
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override Core.Model.Acts.QuantityObservation UpdateInternal(DataContext context, Core.Model.Acts.QuantityObservation data, IPrincipal principal)
        {
            if(data.UnitOfMeasure != null) data.UnitOfMeasure = data.UnitOfMeasure?.EnsureExists(context, principal) as Concept;
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            this.m_observationPersistence.UpdateInternal(context, data, principal);
            context.Insert(new DbQuantityObservation()
            {
                ParentKey = data.VersionKey.Value,
                UnitOfMeasureKey = data.UnitOfMeasureKey.Value,
                Value = data.Value

            });
            return data;
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override Core.Model.Acts.QuantityObservation ObsoleteInternal(DataContext context, Core.Model.Acts.QuantityObservation data, IPrincipal principal)
        {
            if (data.UnitOfMeasure != null) data.UnitOfMeasure = data.UnitOfMeasure?.EnsureExists(context, principal) as Concept;
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            this.m_observationPersistence.ObsoleteInternal(context, data, principal);
            context.Insert(new DbQuantityObservation()
            {
                ParentKey = data.VersionKey.Value,
                UnitOfMeasureKey = data.UnitOfMeasureKey.Value,
                Value = data.Value

            });
            return data;
        }
    }
}