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
 * Date: 2016-11-30
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security.Tfa.Twilio.Configuration;
using OpenIZ.Core.Security.Tfa.Twilio.Resources;
using OpenIZ.Core.Services;
using System;
using System.Diagnostics;
using System.Linq;
using TW = Twilio;

namespace OpenIZ.Core.Security.Tfa.Twilio
{
	/// <summary>
	/// Represents a TFA mechanism that uses TWILIO
	/// </summary>
	public class TfaTwilioMechanism : ITfaMechanism
	{
		// Configuration
		private MechanismConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.core.security.tfa.twilio") as MechanismConfiguration;

		private TraceSource m_tracer = new TraceSource("OpenIZ.Core.Security.Tfa.Twilio");

		/// <summary>
		/// Challenge string
		/// </summary>
		public string Challenge
		{
			get
			{
				return Strings.challenge_text;
			}
		}

		/// <summary>
		/// Identifier of the mechanism
		/// </summary>
		public Guid Id
		{
			get
			{
				return Guid.Parse("08124835-6C24-43C9-8650-9D605F6B5BD6");
			}
		}

		/// <summary>
		/// Gets the name
		/// </summary>
		public string Name
		{
			get
			{
				return Strings.mechanism_name;
			}
		}

		/// <summary>
		/// Send the secret
		/// </summary>
		public void Send(SecurityUser user, string challengeResponse, string tfaSecret)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));
			else if (String.IsNullOrEmpty(challengeResponse))
				throw new ArgumentNullException(nameof(challengeResponse));
			else if (tfaSecret == null)
				throw new ArgumentNullException(nameof(tfaSecret));

			// First, does this user have a phone number
			string toNumber = user.PhoneNumber;
			if (toNumber == null)
			{
				// Get preferred language for the user
				var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
				var userEntity = securityService?.FindUserEntity(o => o.SecurityUserKey == user.Key).FirstOrDefault();
				if (userEntity != null)
					toNumber = userEntity.Telecoms.FirstOrDefault(o => o.AddressUseKey == TelecomAddressUseKeys.MobileContact)?.Value;
			}

			// To numbers fail
			if (toNumber == null || challengeResponse.Length != 4 || !toNumber.EndsWith(challengeResponse))
			{
				this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "Validation of {0} failed", user.UserName);
			}
			else
			{
				try
				{
					var client = new TW.TwilioRestClient(this.m_configuration.Sid, this.m_configuration.Auth);
					var response = client.SendMessage(this.m_configuration.From, toNumber, String.Format(Strings.default_body, tfaSecret));

					if (response.RestException != null)
						throw new Exception(response.RestException.Message ?? "" + " " + (response.RestException.Code ?? "") + " " + (response.RestException.MoreInfo ?? "") + " " + (response.RestException.Status ?? ""));
				}
				catch (Exception ex)
				{
					this.m_tracer.TraceEvent(TraceEventType.Error, ex.HResult, "Error sending SMS: {0}", ex);
				}
			}
		}
	}
}