using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Linq
{
    /// <summary>
    /// Model converter utility
    /// </summary>
    public class ModelConverterUtil
    {

        // Lock object
        private static Object s_lockObject = new object();
        // Instance
        private static ModelConverterUtil s_instance;

        /// <summary>
        /// Model domain converter
        /// </summary>
        private Dictionary<Type, IModelConverter> m_converters = new Dictionary<Type, IModelConverter>();
        
        /// <summary>
        /// Gets the current instance of the model converter
        /// </summary>
        public static ModelConverterUtil Current
        {
            get
            {
                if(s_instance == null)
                    lock(s_lockObject)
                        if (s_instance == null)
                            s_instance = new ModelConverterUtil();
                return s_instance;
            }
        }

        /// <summary>
        /// Initializes the list of converters
        /// </summary>
        public ModelConverterUtil()
        {
            foreach(var t in typeof(ModelConverterUtil).Assembly.GetTypes().Where(o=>typeof(IModelConverter).IsAssignableFrom(o)))
            {
                if (!t.IsClass) continue; // only classes

                ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                    continue; // skip
                IModelConverter value = ci.Invoke(null) as IModelConverter;
                this.m_converters.Add(value.ModelType, value);
                this.m_converters.Add(value.DomainType, value);
            }
        }

        /// <summary>
        /// Get model converter from/to
        /// </summary>
        public IModelConverter GetModelConverter(Type fromType)
        {
            IModelConverter convertValue = null;
            if (this.m_converters.TryGetValue(fromType, out convertValue))
                return convertValue;
            return null;
        }
        
    }
}
