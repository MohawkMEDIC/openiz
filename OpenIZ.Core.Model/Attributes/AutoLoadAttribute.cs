using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Auto load attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoLoadAttribute : Attribute
    {
    }
}
