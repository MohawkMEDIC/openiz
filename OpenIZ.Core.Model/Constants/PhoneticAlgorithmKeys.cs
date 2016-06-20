using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Phonetic algorithm keys
    /// </summary>
    public static class PhoneticAlgorithmKeys
    {
        // The "NONE" phonetic algorith,
        public static readonly Guid None = Guid.Parse("402CD339-D0E4-46CE-8FC2-12A4B0E17226");

        // The "SOUNDEX" 
        public static readonly Guid Soundex = Guid.Parse("3352a79a-d2e0-4e0c-9b48-6fd2a202c681");

        // "METAPHONE"
        public static readonly Guid Metaphone = Guid.Parse("d79a4dc6-66a6-4602-8fcb-7dc09a895793");

    }
}
