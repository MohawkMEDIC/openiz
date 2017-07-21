using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Industry type keys for built-in OpenIZ industry types
    /// </summary>
    public static class IndustryTypeKeys
    {
        /// <summary>
        /// Manufacturing industry
        /// </summary>
        public static readonly Guid Manufacturing = Guid.Parse("33b40c3c-c0c0-48b6-be74-0881d9c00ee6");

        /// <summary>
        /// Other industry
        /// </summary>
        public static readonly Guid OtherIndustry = Guid.Parse("87269826-c0c3-4c22-abb1-3572f6679c3f");

        /// <summary>
        /// Health delivery industry
        /// </summary>
        public static readonly Guid HealthDelivery = Guid.Parse("ef0b2e4e-9e02-4ce3-ab69-cece5f598e20");
    }
}
