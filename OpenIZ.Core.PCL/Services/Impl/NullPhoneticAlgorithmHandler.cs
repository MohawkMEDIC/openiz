using OpenIZ.Core.Model.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// null algorithm phonetic algorithm
    /// </summary>
    public class NullPhoneticAlgorithmHandler : IPhoneticAlgorithmHandler
    {
        /// <summary>
        /// Gets the algorithm id
        /// </summary>
        public Guid AlgorithmId
        {
            get
            {
                return PhoneticAlgorithmKeys.None;
            }
        }

        /// <summary>
        /// Generate the phonetic code
        /// </summary>
        public string GenerateCode(string input)
        {
            return null;
        }
    }
}
