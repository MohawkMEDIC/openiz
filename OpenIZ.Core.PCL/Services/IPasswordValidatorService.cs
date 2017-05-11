using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{

    /// <summary>
    /// Represents a password validation service
    /// </summary>
    public interface IPasswordValidatorService
    {

        /// <summary>
        /// Validate the password
        /// </summary>
        bool Validate(String password);
        
    }
}
