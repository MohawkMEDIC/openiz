using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Phonetic algorithm handler
    /// </summary>
    public interface IPhoneticAlgorithmHandler
    {
        /// <summary>
        /// Gets the algorithm ID
        /// </summary>
        Guid AlgorithmId { get; }

        /// <summary>
        /// Generate a phonetic code from the input data
        /// </summary>
        String GenerateCode(String input);
    }
}
