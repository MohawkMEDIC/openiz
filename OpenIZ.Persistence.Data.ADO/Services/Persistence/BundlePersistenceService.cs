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
                // Are there any relationships
                if (itm is Entity)
                {
                    var ent = itm as Entity;
                    foreach(var rel in ent.Relationships)
                    {
                        var bitm = bundle.Item.FirstOrDefault(o => o.Key == rel.TargetEntityKey);
                        if (bitm == null) continue;

                        if (retVal.Item.Any(o => o.Key == rel.TargetEntityKey))
                            continue;
                        retVal.Item.Add(bitm); // make sure it gets inserted first
                    }

                }
                else if(itm is Act)
                {
                    var act = itm as Act;
                    foreach (var rel in act.Relationships)
                    {
                        var bitm = bundle.Item?.FirstOrDefault(o => o.Key == rel?.TargetActKey);
                        if (bitm == null) continue;

                        if (retVal.Item.Any(o => o.Key == rel.TargetActKey))
                            continue;
                        retVal.Item.Add(bitm); // make sure it gets inserted first
                    }

                    foreach (var rel in act.Participations)
                    {
                        var bitm = bundle.Item?.FirstOrDefault(o => o.Key == rel?.PlayerEntityKey);
                        if (bitm == null) continue;

                        if (retVal.Item.Any(o => o.Key == rel.PlayerEntityKey))
                            continue;
                        retVal.Item.Add(bitm); // make sure it gets inserted first
                    }
                }

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
            data = this.ReorganizeForInsert(data);
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

                this.ProgressChanged?.Invoke(this, new ProgressChangedEventArgs((float)(i + 1) / data.Item.Count, itm));

                var mi = svc.GetType().GetRuntimeMethod(method, new Type[] { typeof(DataContext), itm.GetType(), typeof(IPrincipal) });

                data.Item[i] = mi.Invoke(svc, new object[] { context, itm, principal }) as IdentifiedData;
            }

            // Cache items
            foreach (var itm in data.Item)
            {
                itm.LoadState = LoadState.FullLoad;
                ApplicationContext.Current.GetService<IDataCachingService>()?.Add(itm);
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
