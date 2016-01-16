using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Attribute which instructs mapper to ignore a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DelayLoadAttribute : Attribute
    {
    }
}
