using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a date precision object
    /// </summary>
    [DataContract(Name = "DatePrecision", Namespace = "http://openiz.org/model")]
    public enum DatePrecision
    {
        [EnumMember(Value = "Y")]
        Year,
        [EnumMember(Value = "m")]
        Month,
        [EnumMember(Value = "D")]
        Day,
        [EnumMember(Value = "H")]
        Hour,
        [EnumMember(Value = "M")]
        Minute,
        [EnumMember(Value = "S")]
        Second
    }
}
