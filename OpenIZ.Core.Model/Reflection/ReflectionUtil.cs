using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Reflection
{
    /// <summary>
    /// Reflection tools
    /// </summary>
    public static class ReflectionUtil
    {
        /// <summary>
        /// Get generic method
        /// </summary>
        public static MethodBase GetGenericMethod(this Type type, string name, Type[] typeArgs, Type[] argTypes)
        {
            int typeArity = typeArgs.Length;
            var methods = type.GetRuntimeMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Where(m => m.GetParameters().Length == argTypes.Length)
                .Select(m => m.MakeGenericMethod(typeArgs)).ToList()
                .Where(m => m.GetParameters().All(o => argTypes.Any(p => o.ParameterType.GetTypeInfo().IsAssignableFrom(p.GetTypeInfo()))));


            return methods.FirstOrDefault();
            //return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }


    }
}
