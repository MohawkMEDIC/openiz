using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a regular expression password validator
    /// </summary>
    public abstract class RegexPasswordValidator : IPasswordValidatorService
    {

        // Default password pattern
        public const string DefaultPasswordPattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$";

        // Regex for password validation
        private readonly Regex m_passwordRegex;

        /// <summary>
        /// Create regex password validator with specified expression
        /// </summary>
        public RegexPasswordValidator(String passwordMatch)
        {
            this.m_passwordRegex = new Regex(passwordMatch);
        }

        /// <summary>
        /// Validate the specified password
        /// </summary>
        public bool Validate(string password)
        {
            return this.m_passwordRegex.IsMatch(password);
        }
    }
}
