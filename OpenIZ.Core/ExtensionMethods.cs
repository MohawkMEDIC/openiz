/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;

namespace OpenIZ.Core
{
	/// <summary>
	/// Represents a series of extension methods for the <see cref="ApplicationContext" /> class.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Get application provider service
		/// </summary>
		/// <param name="me">The current application context.</param>
		/// <returns>Returns an instance of the <see cref="IApplicationIdentityProviderService"/>.</returns>
		public static IApplicationIdentityProviderService GetApplicationProviderService(this ApplicationContext me)
		{
			return me.GetService<IApplicationIdentityProviderService>();
		}

		/// <summary>
		/// Gets the assigning authority repository service.
		/// </summary>
		/// <param name="me">The current application context.</param>
		/// <returns>Returns an instance of the <see cref="IAssigningAuthorityRepositoryService"/>.</returns>
		public static IAssigningAuthorityRepositoryService GetAssigningAuthorityService(this ApplicationContext me)
		{
			return me.GetService<IAssigningAuthorityRepositoryService>();
		}

		/// <summary>
		/// Gets the business rules service for a specific information model.
		/// </summary>
		/// <typeparam name="T">The type of information for which to retrieve the business rules engine instance.</typeparam>
		/// <param name="me">The application context.</param>
		/// <returns>Returns an instance of the business rules service.</returns>
		public static IBusinessRulesService<T> GetBusinessRulesService<T>(this ApplicationContext me) where T : IdentifiedData
		{
			return me.GetService<IBusinessRulesService<T>>();
		}

		/// <summary>
		/// Get the concept service.
		/// </summary>
		/// <param name="me">The current application context.</param>
		/// <returns>Returns an instance of the <see cref="IConceptRepositoryService"/>.</returns>
		public static IConceptRepositoryService GetConceptService(this ApplicationContext me)
		{
			return me.GetService<IConceptRepositoryService>();
		}

		/// <summary>
		/// Gets a locale string value for a given string id.
		/// </summary>
		/// <param name="me">The current application context.</param>
		/// <param name="stringId">The string identifier.</param>
		/// <returns>Returns a locale string.</returns>
		public static string GetLocaleString(this ApplicationContext me, string stringId)
		{
			var locale = me.GetService<ILocalizationService>();

			if (stringId == OpenIzConstants.GeneralPanicErrorCode)
			{
				return OpenIzConstants.GeneralPanicErrorText;
			}
			else if (locale == null)
			{
				return stringId;
			}
			else
			{
				return locale.GetString(stringId);
			}
		}
	}
}