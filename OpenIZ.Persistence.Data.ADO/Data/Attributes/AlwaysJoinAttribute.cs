using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Attributes
{
    /// <summary>
    /// Instructs the query planner to always join another reference table when executing queries
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AlwaysJoinAttribute : Attribute
    {
    }
}
