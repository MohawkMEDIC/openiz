using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Represents a database observation
    /// </summary>
    public interface IDbObservation : IDbIdentified
    {

        /// <summary>
        /// Gets or sets observation
        /// </summary>
        Observation Observation { get; set; }
        
    }

    /// <summary>
    /// Quantity observation
    /// </summary>
    public partial class QuantityObservation : IDbObservation
    {

    }

    /// <summary>
    /// Quantity observation
    /// </summary>
    public partial class CodedObservation : IDbObservation
    {

    }

    /// <summary>
    /// Text observation
    /// </summary>
    public partial class TextObservation :IDbObservation
    {

    }
}
