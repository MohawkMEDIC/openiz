using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core
{
    /// <summary>
    /// Application context extensions
    /// </summary>
    public static class ApplicationContextExtensions
    {

        /// <summary>
        /// Get locale
        /// </summary>
        public static String GetLocaleString(this ApplicationContext me, String stringId)
        {
            var locale = me.GetService<ILocalizationService>();
            if (locale == null)
                return stringId;
            else
                return locale.GetString(stringId);
        }

        /// <summary>
        /// Get the concept service
        /// </summary>
        public static IConceptService GetConceptService(this ApplicationContext me)
        {
            return me.GetService<IConceptService>();
        }

        /// <summary>
        /// Get application provider service
        /// </summary>
        public static IApplicationIdentityProviderService GetApplicationProviderService(this ApplicationContext me)
        {
            return me.GetService<IApplicationIdentityProviderService>();
        }
    }
}
