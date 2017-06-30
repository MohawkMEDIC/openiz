using System;
using System.IO;

namespace OpenIZ.BusinessRules.JavaScript.Test
{
    /// <summary>
    /// Represents a data resolver
    /// </summary>
    internal class TestDataReferenceResolver : IDataReferenceResolver
    {
        /// <summary>
        /// Resolve reference
        /// </summary>
        public Stream Resolve(string reference)
        {
            return typeof(TestDataReferenceResolver).Assembly.GetManifestResourceStream("OpenIZ.BusinessRules.JavaScript.Test.RefData." + reference);
        }
    }
}