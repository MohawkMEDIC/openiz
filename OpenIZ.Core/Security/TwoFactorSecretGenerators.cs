using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represents a TFA secret generator which uses the server's clock
    /// </summary>
    public class SimpleTfaSecretGenerator : ITwoFactorSecretGenerator
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name
        {
            get
            {
                return "Simple TFA generator";
            }
        }

        /// <summary>
        /// Generate the TFA secret
        /// </summary>
        public string GenerateTfaSecret()
        {
            var secretInt = DateTime.Now.Ticks % 9999;
            return String.Format("{0:0000}", secretInt);
        }

        /// <summary>
        /// Validate the secret
        /// </summary>
        public bool Validate(string secret)
        {
            int toss;
            return secret.Length == 4 && Int32.TryParse(secret, out toss);
        }
    }
}
