using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.IMSI.Util
{
    /// <summary>
    /// Object expansion tool
    /// </summary>
    public static class ObjectExpander
    {
        // Trace source
        private static TraceSource m_tracer = new TraceSource("OpenIZ.Messaging.IMSI");

        // Sync lock
        private static Object s_syncLock = new object();

        // Related load methods
        private static Dictionary<Type, MethodInfo> m_relatedLoadMethods = new Dictionary<Type, MethodInfo>();

        // Reloated load association
        private static Dictionary<Type, MethodInfo> m_relatedLoadAssociations = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Expand properties
        /// </summary>
        public static void ExpandProperties(IdentifiedData returnValue, NameValueCollection qp, Stack<Guid> keyStack = null)
        {

            // Set the stack
            if (keyStack == null)
                keyStack = new Stack<Guid>();
            else if (keyStack.Contains(returnValue.Key.Value))
                return;

            keyStack.Push(returnValue.Key.Value);

            try
            {
                // Expand property?
                if (qp.ContainsKey("_expand") && qp.ContainsKey("_all"))
                    return;


                if (qp.ContainsKey("_all"))
                {
                    foreach (var pi in returnValue.GetType().GetRuntimeProperties().Where(o=>o.GetCustomAttribute<SerializationReferenceAttribute>() != null &&
                    o.GetCustomAttribute<DataIgnoreAttribute>() == null))
                    {
                        var scope = pi.GetValue(returnValue);
                        if (scope is IdentifiedData)
                            ExpandProperties(scope as IdentifiedData, qp, keyStack);
                            //foreach (var pi2 in scope.GetType().GetRuntimeProperties())
                            //    pi2.GetValue(scope);
                        else if (scope is IList)
                            foreach (var itm in scope as IList)
                                if (itm is IdentifiedData)
                                    ExpandProperties(itm as IdentifiedData, qp, keyStack);
                    }
                }
                else if(qp.ContainsKey("_expand"))
                    foreach (var nvs in qp["_expand"])
                    {
                        // Get the property the user wants to expand
                        object scope = returnValue;
                        foreach (var property in nvs.Split('.'))
                        {
                            if (scope is IList)
                            {
                                foreach (var sc in scope as IList)
                                {
                                    PropertyInfo keyPi = sc.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault()?.ElementName == property);
                                    if (keyPi == null)
                                        continue;
                                    // Get the backing property
                                    PropertyInfo expandProp = sc.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttributes<SerializationReferenceAttribute>().FirstOrDefault()?.RedirectProperty == keyPi.Name);
                                    if (expandProp != null)
                                        scope = expandProp.GetValue(sc);
                                    else
                                        scope = keyPi.GetValue(sc);

                                }
                            }
                            else
                            {
                                PropertyInfo keyPi = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault()?.ElementName == property);
                                if (keyPi == null)
                                    continue;
                                // Get the backing property
                                PropertyInfo expandProp = scope.GetType().GetProperties().SingleOrDefault(o => o.GetCustomAttributes<SerializationReferenceAttribute>().FirstOrDefault()?.RedirectProperty == keyPi.Name);

                                Object existing = null;
                                Object keyValue = keyPi.GetValue(scope);

                                if (expandProp != null && expandProp.CanWrite)
                                {
                                    expandProp.SetValue(scope, existing);
                                    scope = existing;
                                }
                                else
                                {
                                    if (expandProp != null)
                                        scope = expandProp.GetValue(scope);
                                    else
                                        scope = keyValue;
                                }
                            }
                        }
                    }

                ApplicationContext.Current.GetService<IDataCachingService>()?.Add(returnValue);
            }
            finally
            {
                keyStack.Pop();
            }
        }
    }
}
