using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OpenIZ.Core.Model.Serialization
{
    /// <summary>
    /// Model binding 
    /// </summary>
    public class ModelSerializationBinder : SerializationBinder
    {
        /// <summary>
        /// Bind to type
        /// </summary>
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Assembly to search
            Assembly asm = typeof(ModelSerializationBinder).GetTypeInfo().Assembly;
            if (!String.IsNullOrEmpty(assemblyName))
                asm = Assembly.Load(new AssemblyName(assemblyName));

            // The type
            var type = asm.ExportedTypes.SingleOrDefault(
                t => t.GetTypeInfo().GetCustomAttribute<JsonObjectAttribute>()?.Id == typeName
                );
            if (type == null)
                type = asm.GetType(typeName);
            return type ?? typeof(IdentifiedData);
        }
    }
}
