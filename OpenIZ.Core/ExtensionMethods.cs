/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-6-14
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core
{
    /// <summary>
    /// Application context extensions
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get locale
        /// </summary>
        public static String GetLocaleString(this ApplicationContext me, String stringId)
        {
            var locale = me.GetService<ILocalizationService>();
            if (stringId == OpenIzConstants.GeneralPanicErrorCode)
                return OpenIzConstants.GeneralPanicErrorText;
            else if (locale == null)
                return stringId;
            else
                return locale.GetString(stringId);
        }

        /// <summary>
        /// Get the concept service
        /// </summary>
        public static IConceptRepositoryService GetConceptService(this ApplicationContext me)
        {
            return me.GetService<IConceptRepositoryService>();
        }

		/// <summary>
		/// Gets the assigning authority repository service.
		/// </summary>
		/// <param name="me">The current application context.</param>
		/// <returns>Returns the assigning authority repository service.</returns>
		public static IAssigningAuthorityRepositoryService GetAssigningAuthorityService(this ApplicationContext me)
		{
			return me.GetService<IAssigningAuthorityRepositoryService>();
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
