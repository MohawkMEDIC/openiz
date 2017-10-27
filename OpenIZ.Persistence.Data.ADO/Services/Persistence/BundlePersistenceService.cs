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
using OpenIZ.Core.Model.Collection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using System.Linq.Expressions;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using System.Reflection;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.OrmLite;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Bundle persistence service
    /// </summary>
    public class BundlePersistenceService : AdoBasePersistenceService<Bundle>, IReportProgressChanged
    {
        // Progress has changed
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Bundle modelInstance, DataContext context, IPrincipal principal)
        {
            return m_mapper.MapModelInstance<Bundle, Object>(modelInstance);
        }


        /// <summary>
        /// Reorganize all the major items for insert
        /// </summary>
        private Bundle ReorganizeForInsert(Bundle bundle)
        {
            Bundle retVal = new Bundle() { Item = new List<IdentifiedData>() };
            foreach(var itm in bundle.Item.Where(o=>o != null))
            {
                this.m_tracer.TraceInformation("Reorganizing {0}..", itm.Key);
                // Are there any relationships
                if (itm is Entity)
                {
                    var ent = itm as Entity;
                    foreach(var rel in ent.Relationships)
                    {
                        this.m_tracer.TraceInformation("Processing {0} / relationship / {1} ..", itm.Key, rel.TargetEntityKey);

                        var bitm = bundle.Item.FirstOrDefault(o => o.Key == rel.TargetEntityKey);
                        if (bitm == null) continue;

                        if (retVal.Item.Any(o => o.Key == rel.TargetEntityKey))
                            continue;
                        this.m_tracer.TraceInformation("Bumping (due to relationship): {0}", bitm);

                        retVal.Item.Add(bitm); // make sure it gets inserted first
                    }

                }
                else if(itm is Act)
                {
                    var act = itm as Act;
                    foreach (var rel in act.Relationships)
                    {
                        this.m_tracer.TraceInformation("Processing {0} / relationship / {1} ..", itm.Key, rel.TargetActKey);
                        var bitm = bundle.Item?.FirstOrDefault(o => o.Key == rel?.TargetActKey);
                        if (bitm == null) continue;

                        if (retVal.Item.Any(o => o.Key == rel.TargetActKey))
                            continue;
                        this.m_tracer.TraceInformation("Bumping (due to relationship): {0}", bitm);
                        retVal.Item.Add(bitm); // make sure it gets inserted first
                    }

                    foreach (var rel in act.Participations)
                    {
                        this.m_tracer.TraceInformation("Processing {0} / participation / {1} ..", itm.Key, rel.PlayerEntityKey);
                        var bitm = bundle.Item?.FirstOrDefault(o => o.Key == rel?.PlayerEntityKey);
                        if (bitm == null) continue;

                        if (retVal.Item.Any(o => o.Key == rel.PlayerEntityKey))
                            continue;

                        this.m_tracer.TraceInformation("Bumping (due to participation): {0}", bitm);
                        retVal.Item.Add(bitm); // make sure it gets inserted first
                    }


                    // Old versions of the mobile had an issue with missing record targets
                    if(AdoPersistenceService.GetConfiguration().DataCorrectionKeys.Contains("correct-missing-rct"))
                    {
                        var patientEncounter = bundle.Item.OfType<PatientEncounter>().FirstOrDefault();

                        if (patientEncounter != null)
                        {
                            var rct = act.Participations.FirstOrDefault(o => o.ParticipationRoleKey == OpenIZ.Core.Model.Constants.ActParticipationKey.RecordTarget);
                            if (!(rct == null || rct.PlayerEntityKey.HasValue))
                            {
                                var perct = patientEncounter.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget);

                                act.Participations.Remove(rct);
                                act.Participations.Add(new ActParticipation(ActParticipationKey.RecordTarget, perct.PlayerEntityKey));
                            }
                        }
                    }
                }

                this.m_tracer.TraceInformation("Re-adding: {0}", itm);
                retVal.Item.Add(itm);
            }

            return retVal;
        }

        /// <summary>
        /// Insert or update contents of the bundle
        /// </summary>
        /// <returns></returns>
        public override Bundle InsertInternal(DataContext context, Bundle data, IPrincipal principal)
        {

            if (data.Item == null) return data;
            this.m_tracer.TraceInformation("Bundle has {0} objects...", data.Item.Count);
            data = this.ReorganizeForInsert(data);
            this.m_tracer.TraceInformation("After reorganization has {0} objects...", data.Item.Count);

            context.PrepareStatements = true;
            for(int i  = 0; i < data.Item.Count; i++)
            {
                var itm = data.Item[i];
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var svc = ApplicationContext.Current.GetService(idp);
				var method = "Insert";

	            if (itm.TryGetExisting(context, principal, true) != null)
	            {
					method = "Update";
				}

                this.m_tracer.TraceInformation("Will {0} object from bundle {1}...", method, itm);
                this.ProgressChanged?.Invoke(this, new ProgressChangedEventArgs((float)(i + 1) / data.Item.Count, itm));

                var mi = svc.GetType().GetRuntimeMethod(method, new Type[] { typeof(DataContext), itm.GetType(), typeof(IPrincipal) });
                try
                {
                    data.Item[i] = mi.Invoke(svc, new object[] { context, itm, principal }) as IdentifiedData;
                }
                catch(TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }

            // Cache items
            foreach (var itm in data.Item)
            {
                itm.LoadState = LoadState.FullLoad;
                context.AddCacheCommit(itm);
            }
            return data;
        }

        /// <summary>
        /// Obsolete each object in the bundle
        /// </summary>
        public override Bundle ObsoleteInternal(DataContext context, Bundle data, IPrincipal principal)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
				var svc = ApplicationContext.Current.GetService(idp);
				var mi = svc.GetType().GetRuntimeMethod("Obsolete", new Type[] { typeof(DataContext), itm.GetType(), typeof(IPrincipal) });

				itm.CopyObjectData(mi.Invoke(ApplicationContext.Current.GetService(idp), new object[] { context, itm, principal }));
            }
            return data;
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public override IEnumerable<Bundle> QueryInternal(DataContext context, Expression<Func<Bundle, bool>> query, Guid queryId, int offset, int? count, out int totalResults, IPrincipal principal, bool countResults = true)
        {
            totalResults = 0;
            return new List<Bundle>().AsQueryable();
        }

        /// <summary>
        /// Model instance 
        /// </summary>
        public override Bundle ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {
            return m_mapper.MapModelInstance<Object, Bundle>(dataInstance);

        }

        /// <summary>
        /// Update all items in the bundle
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public override Bundle UpdateInternal(DataContext context, Bundle data, IPrincipal principal)
        {
            return this.InsertInternal(context, data, principal);
        }

    
    }
}
