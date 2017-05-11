using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represents a local regex password validator
    /// </summary>
    public class LocalPasswordValidationService : RegexPasswordValidator
    {
        /// <summary>
        /// Local password validation service
        /// </summary>
        public LocalPasswordValidationService() : base((ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(OpenIzConstants.OpenIZConfigurationName) as Configuration.OpenIzConfiguration).Security.PasswordRegex ?? RegexPasswordValidator.DefaultPasswordPattern)
        {
            
        }
    }
}
