using OpenIZ.Core.Model.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Linq.Expressions;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using System.Reflection;
using OpenIZ.Core.Model;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Bundle persistence service
    /// </summary>
    public class BundlePersistenceService : SqlServerBasePersistenceService<Bundle>
    {
        /// <summary>
        /// From model instance
        /// </summary>
        public override object FromModelInstance(Bundle modelInstance, ModelDataContext context, IPrincipal principal)
        {
            return m_mapper.MapModelInstance<Bundle, Object>(modelInstance);
        }

        /// <summary>
        /// Insert or update contents of the bundle
        /// </summary>
        /// <returns></returns>
        public override Bundle Insert(ModelDataContext context, Bundle data, IPrincipal principal)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var svc = ApplicationContext.Current.GetService(idp);
                String method = "Insert";
                if (itm.TryGetExisting(context, principal) != null)
                    method = "Update";
                var mi = svc.GetType().GetRuntimeMethod(method, new Type[] { typeof(ModelDataContext), itm.GetType(), typeof(IPrincipal) });
                itm.CopyObjectData(mi.Invoke(svc, new object[] { context, itm, principal }));
            }
            return data;
        }

        /// <summary>
        /// Obsolete each object in the bundle
        /// </summary>
        public override Bundle Obsolete(ModelDataContext context, Bundle data, IPrincipal principal)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var mi = idp.GetRuntimeMethod("Obsolete", new Type[] { typeof(ModelDataContext), itm.GetType(), typeof(IPrincipal) });
                itm.CopyObjectData(mi.Invoke(ApplicationContext.Current.GetService(idp), new object[] { context, itm, principal }));
            }
            return data;
        }

        /// <summary>
        /// Query the specified object
        /// </summary>
        public override IQueryable<Bundle> Query(ModelDataContext context, Expression<Func<Bundle, bool>> query, IPrincipal principal)
        {
            return new List<Bundle>().AsQueryable();
        }

        /// <summary>
        /// Model instance 
        /// </summary>
        public override Bundle ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
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
        public override Bundle Update(ModelDataContext context, Bundle data, IPrincipal principal)
        {
            foreach (var itm in data.Item)
            {
                var idp = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { itm.GetType() });
                var mi = idp.GetRuntimeMethod("Update", new Type[] { typeof(ModelDataContext), itm.GetType(), typeof(IPrincipal) });
                itm.CopyObjectData(mi.Invoke(ApplicationContext.Current.GetService(idp), new object[] { context, itm, principal }));
            }
            return data;
        }

    
    }
}
