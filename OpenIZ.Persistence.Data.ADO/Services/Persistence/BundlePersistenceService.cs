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


namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Bundle persistence service
    /// </summary>
    public class BundlePersistenceService : AdoBasePersistenceService<Bundle>
    {
        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Bundle modelInstance, DataContext context, IPrincipal principal)
        {
            return m_mapper.MapModelInstance<Bundle, Object>(modelInstance);
        }

        /// <summary>
        /// Insert or update contents of the bundle
        /// </summary>
        /// <returns></returns>
        public override Bundle InsertInternal(DataContext context, Bundle data, IPrincipal principal)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var svc = ApplicationContext.Current.GetService(idp);
				var method = "Insert";

	            if (itm.TryGetExisting(context, principal) != null)
	            {
					method = "Update";
				}

                var mi = svc.GetType().GetRuntimeMethod(method, new Type[] { typeof(DataContext), itm.GetType(), typeof(IPrincipal) });

                itm.CopyObjectData(mi.Invoke(svc, new object[] { context, itm, principal }));
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
        public override IEnumerable<Bundle> QueryInternal(DataContext context, Expression<Func<Bundle, bool>> query, int offset, int? count, out int totalResults, IPrincipal principal, bool countResults = true)
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
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
	            var svc = ApplicationContext.Current.GetService(idp);
				var mi = svc.GetType().GetRuntimeMethod("Update", new Type[] { typeof(DataContext), itm.GetType(), typeof(IPrincipal) });

				itm.CopyObjectData(mi.Invoke(ApplicationContext.Current.GetService(idp), new object[] { context, itm, principal }));
			}
            return data;
        }

    
    }
}
