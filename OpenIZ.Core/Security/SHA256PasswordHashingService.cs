using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// SHA256 password generator service
    /// </summary>
    public class SHA256PasswordHashingService : IPasswordHashingService
    {
        /// <summary>
        /// Encode a password using the SHA256 encoding
        /// </summary>
        public string EncodePassword(string password)
        {
            SHA256 hasher = SHA256.Create();
            return BitConverter.ToString(hasher.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
