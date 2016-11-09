﻿using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security.Tfa.Twilio.Configuration;
using OpenIZ.Core.Security.Tfa.Twilio.Resources;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TW = Twilio;
using System.Net.Http;
using OpenIZ.Core.Http;

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
            if(toNumber == null)
            {
                // Get preferred language for the user
                var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                var userEntity = securityService?.FindUserEntity(o => o.SecurityUserKey == user.Key).FirstOrDefault();
                if (userEntity != null)
                    toNumber = userEntity.Telecoms.FirstOrDefault(o => o.AddressUseKey == TelecomAddressUseKeys.MobileContact)?.Value;
            }

            // To numbers fail
            if (toNumber == null || challengeResponse.Length != 4 || !toNumber.EndsWith(challengeResponse)) 
                this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "Validation of {0} failed", user.UserName);

            try
            {
                var client = new TW.TwilioRestClient(this.m_configuration.Sid, this.m_configuration.Auth);
                client.SendMessage(this.m_configuration.From, toNumber, String.Format(Strings.default_body, tfaSecret));
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceEvent(TraceEventType.Error, ex.HResult, "Error sending SMS: {0}", ex);
                throw;
            }
        }
    }
}