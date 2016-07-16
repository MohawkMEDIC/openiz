using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Identifies that a property containins no meaningful data 
    /// and is provided only for serialization
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataIgnoreAttribute : Attribute
    {
    }
}
