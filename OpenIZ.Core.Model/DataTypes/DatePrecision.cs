using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a date precision object
    /// </summary>
    [XmlType("DatePrecision", Namespace = "http://openiz.org/model")]
    public enum DatePrecision
    {
        [XmlEnum("Y")]
        Year,
        [XmlEnum("m")]
        Month,
        [XmlEnum("D")]
        Day,
        [XmlEnum("H")]
        Hour,
        [XmlEnum("M")]
        Minute,
        [XmlEnum("S")]
        Second
    }
}
