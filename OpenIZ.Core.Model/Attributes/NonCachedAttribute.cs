using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Instructs any data caching that the object should not be cached
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NonCachedAttribute : Attribute
    {
    }
}
