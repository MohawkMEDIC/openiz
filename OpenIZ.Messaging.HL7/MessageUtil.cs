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
 * User: khannan
 * Date: 2016-8-15
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Base.validation.impl;
using NHapi.Model.V25.Segment;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenIZ.Messaging.HL7
{
	/// <summary>
	/// Provides utility operations for HL7v2 messages.
	/// </summary>
	public class MessageUtil
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private static TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// The internal reference to the HL7v2 to Telecommunications use conversion map.
		/// </summary>
		private static Dictionary<string, Guid> v2TelUseConvert = new Dictionary<string, Guid>()
		{
			{ "ASN", TelecomAddressUseKeys.AnsweringService },
			{ "BPN", TelecomAddressUseKeys.Pager },
			{ "EMR", TelecomAddressUseKeys.EmergencyContact },
			{ "PRN", TelecomAddressUseKeys.Public },
			{ "CEL", TelecomAddressUseKeys.MobileContact },
			{ "WPN", TelecomAddressUseKeys.WorkPlace }
		};

		/// <summary>
		/// Converts components of a HL7v2 message to a <see cref="Patient"/> instance.
		/// </summary>
		/// <param name="msh">The message header segment.</param>
		/// <param name="evn">The event segment.</param>
		/// <param name="pid">The patient identification segment.</param>
		/// <param name="pd1">The patient pd1 segment.</param>
		/// <returns>Returns the patient instance.</returns>
		public static Patient CreatePatient(MSH msh, EVN evn, PID pid, PD1 pd1)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts a <see cref="NHapi.Model.V231.Datatype.XTN"/> instance to a <see cref="EntityTelecomAddress"/> instance.
		/// </summary>
		/// <param name="v2XTN">The v2 XTN instance to be converted.</param>
		/// <returns>Returns the converted entity telecom address instance.</returns>
		public static EntityTelecomAddress TelFromXTN(NHapi.Model.V231.Datatype.XTN v2XTN)
		{
			Regex re = new Regex(@"([+0-9A-Za-z]{1,4})?\((\d{3})\)?(\d{3})\-(\d{4})X?(\d{1,6})?");
			var retVal = new EntityTelecomAddress();

			if (v2XTN.Get9999999X99999CAnyText.Value == null)
			{
				StringBuilder sb = new StringBuilder("tel:");

				try
				{
					if (v2XTN.CountryCode.Value != null)
					{
						sb.AppendFormat("{0}-", v2XTN.CountryCode);
					}

					if (v2XTN.PhoneNumber != null && v2XTN.PhoneNumber.Value != null && !v2XTN.PhoneNumber.Value.Contains("-"))
					{
						v2XTN.PhoneNumber.Value = v2XTN.PhoneNumber.Value.Insert(3, "-");
					}

					sb.AppendFormat("{0}-{1}", v2XTN.AreaCityCode, v2XTN.PhoneNumber);

					if (v2XTN.Extension.Value != null)
					{
						sb.AppendFormat(";ext={0}", v2XTN.Extension);
					}
				}
				catch { }

				if (sb.ToString().EndsWith("tel:") || sb.ToString() == "tel:-")
				{
					retVal.Value = "tel:" + v2XTN.AnyText.Value;
				}
				else
				{
					retVal.Value = sb.ToString();
				}
			}
			else
			{
				var match = re.Match(v2XTN.Get9999999X99999CAnyText.Value);

				StringBuilder sb = new StringBuilder("tel:");

				for (int i = 1; i < 5; i++)
				{
					if (!String.IsNullOrEmpty(match.Groups[i].Value))
					{
						sb.AppendFormat("{0}{1}", match.Groups[i].Value, i == 4 ? "" : "-");
					}
				}

				if (!string.IsNullOrEmpty(match.Groups[5].Value))
				{
					sb.AppendFormat(";ext={0}", match.Groups[5].Value);
				}

				retVal.Value = sb.ToString();
			}

			// Use code conversion
			Guid use = Guid.Empty;

			if (!string.IsNullOrEmpty(v2XTN.TelecommunicationUseCode.Value) && !v2TelUseConvert.TryGetValue(v2XTN.TelecommunicationUseCode.Value, out use))
			{
				throw new InvalidOperationException(string.Format("{0} is not a known use code", v2XTN.TelecommunicationUseCode.Value));
			}

			retVal.AddressUseKey = use;

			// Capability
			return retVal;
		}

		/// <summary>
		/// Updates an error segment of a message.
		/// </summary>
		/// <param name="err">The error segment to be updated.</param>
		/// <param name="dtl">The result details.</param>
		/// <returns>Returns the error code.</returns>
		public static int UpdateERR(NHapi.Model.V25.Segment.ERR err, IResultDetail dtl)
		{
			var locale = ApplicationContext.Current.GetService(typeof(ILocalizationService)) as ILocalizationService;

			err.Severity.Value = dtl.Type.ToString()[0].ToString();

			// Determine the type of acknowledgement
			string errCode = MapErrCode(dtl);

			err.HL7ErrorCode.Identifier.Value = errCode;
			err.HL7ErrorCode.Text.Value = locale.GetString(String.Format("HL7{0}", errCode));

			if (dtl.Location != null && dtl.Location.Contains("^"))
			{
				var cmp = dtl.Location.Split('^');
				for (int i = 0; i < cmp.Length; i++)
				{
					var st = err.GetErrorLocation(0).Components[i] as NHapi.Model.V25.Datatype.ST;

					if (st != null)
					{
						st.Value = cmp[i];
					}
					else
					{
						var nm = err.GetErrorLocation(0).Components[i] as NHapi.Model.V25.Datatype.NM;

						if (nm != null)
						{
							nm.Value = cmp[i];
						}
					}
				}
			}

			err.UserMessage.Value = dtl.Message;

			return int.Parse(errCode[0].ToString());
		}

		/// <summary>
		/// Updates an MSH segment.
		/// </summary>
		/// <param name="terser">The terser reference.</param>
		/// <param name="inboundMsh">The inbound message to update.</param>
		public static void UpdateMSH(Terser terser, IMessage inboundMsh)
		{
			var config = ApplicationContext.Current.Configuration;

			Terser inboundTerser = new Terser(inboundMsh);

			terser.Set("/MSH-10", Guid.NewGuid().ToString());
			terser.Set("/MSH-3", config.DeviceName);
			terser.Set("/MSH-4", config.JurisdictionData.Name);
			terser.Set("/MSH-5", inboundTerser.Get("/MSH-3"));
			terser.Set("/MSH-6", inboundTerser.Get("/MSH-4"));
			terser.Set("/MSH-7", DateTime.Now.ToString("yyyyMMddHHmm"));
			terser.Set("/MSA-2", inboundTerser.Get("/MSH-10"));

			if (string.IsNullOrEmpty(terser.Get("/MSH-9-2")))
			{
				terser.Set("/MSH-9-2", inboundTerser.Get("/MSH-9-2"));
			}

			terser.Set("/MSH-11", inboundTerser.Get("/MSH-11"));
		}

		/// <summary>
		/// Converts an <see cref="EntityTelecomAddress"/> instance to an <see cref="NHapi.Model.V25.Datatype.XTN"/> instance.
		/// </summary>
		/// <param name="tel">The entity telecom address instance to be converted.</param>
		/// <param name="instance">The converted XTN instance.</param>
		public static void XTNFromTel(EntityTelecomAddress tel, NHapi.Model.V25.Datatype.XTN instance)
		{
			Regex re = new Regex(@"^(?<s1>(?<s0>[^:/\?#]+):)?(?<a1>//(?<a0>[^/\;#]*))?(?<p0>[^\;#]*)(?<q1>\;(?<q0>[^#]*))?(?<f1>#(?<f0>.*))?");

			// Match
			var match = re.Match(tel.Value);

			if (match.Groups[1].Value != "tel:")
			{
				instance.AnyText.Value = tel.Value;
				//instance.TelephoneNumber.Value = tel.Value;
				return;
			}

			// Telephone
			string[] comps = match.Groups[5].Value.Split('-');
			StringBuilder sb = new StringBuilder(), phone = new StringBuilder();

			for (int i = 0; i < comps.Length; i++)
			{
				if (i == 0 && comps[i].Contains("+"))
				{
					sb.Append(comps[i]);
					instance.CountryCode.Value = comps[i];
				}
				else if (sb.Length == 0 && comps.Length == 3 || comps.Length == 4 && i == 1) // area code?
				{
					sb.AppendFormat("({0})", comps[i]);
					instance.AreaCityCode.Value = comps[i];
				}
				else if (i != comps.Length - 1)
				{
					sb.AppendFormat("{0}-", comps[i]);
					phone.AppendFormat("{0}", comps[i]);
				}
				else
				{
					sb.Append(comps[i]);
					phone.Append(comps[i]);
				}
			}


			instance.LocalNumber.Value = phone.ToString().Replace("-", "");

			// Extension?
			string[] parms = match.Groups[7].Value.Split(';');

			foreach (var parm in parms)
			{
				string[] pData = parm.Split('=');

				if (pData[0] == "extension" || pData[0] == "ext" || pData[0] == "postd")
				{
					sb.AppendFormat("X{0}", pData[1]);
					instance.Extension.Value = pData[1];
				}
			}

			instance.TelephoneNumber.Value = sb.ToString();

			// Tel use
			if (tel.AddressUseKey != null)
			{
				foreach (var tcu in v2TelUseConvert)
				{
					if (tcu.Value == tel.AddressUseKey)
					{
						instance.TelecommunicationUseCode.Value = tcu.Key;
					}
				}
			}
		}

		/// <summary>
		/// Converts an <see cref="EntityTelecomAddress"/> instance to an <see cref="NHapi.Model.V231.Datatype.XTN"/> instance.
		/// </summary>
		/// <param name="tel">The entity telecom address instance to be converted.</param>
		/// <param name="instance">The converted XTN instance.</param>
		public static void XTNFromTel(EntityTelecomAddress tel, NHapi.Model.V231.Datatype.XTN instance)
		{
			NHapi.Model.V25.Datatype.XTN v25instance = new NHapi.Model.V25.Datatype.XTN(instance.Message);

			MessageUtil.XTNFromTel(tel, v25instance);

			for (int i = 0; i < v25instance.Components.Length; i++)
			{
				if (v25instance.Components[i] is AbstractPrimitive && i < instance.Components.Length)
				{
					(instance.Components[i] as AbstractPrimitive).Value = (v25instance.Components[i] as AbstractPrimitive).Value;
				}
			}
		}

		internal static Expression<Func<Patient, bool>> CreateIDQuery(QPD qpd)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Create NACK
		/// </summary>
		internal static IMessage CreateNack(IMessage request, string responseCode, string errCode, string errDescription)
		{
			System.Diagnostics.Trace.TraceWarning(String.Format("NACK Condition : {0}", errDescription));

			var config = ApplicationContext.Current.Configuration;

			if (request.Version == "2.3.1")
			{
				NHapi.Model.V231.Message.ACK ack = new NHapi.Model.V231.Message.ACK();
				Terser terser = new Terser(ack);
				terser.Set("/MSA-1", responseCode);
				terser.Set("/MSA-3", "Error occurred");
				terser.Set("/MSA-6-1", errCode);
				terser.Set("/MSA-6-2", errDescription);
				MessageUtil.UpdateMSH(terser, request);
				return ack;
			}
			else
			{
				NHapi.Model.V25.Message.ACK ack = new NHapi.Model.V25.Message.ACK();
				Terser terser = new Terser(ack);
				terser.Set("/MSA-1", responseCode);
				MessageUtil.UpdateMSH(terser, request);
				terser.Set("/ERR-3-1", errCode);
				terser.Set("/ERR-3-2", errDescription);
				return ack;
			}
		}

		/// <summary>
		/// Create NACK
		/// </summary>
		internal static IMessage CreateNack(IMessage request, List<IResultDetail> errors, Type errType)
		{
			var config = ApplicationContext.Current.Configuration;

			IMessage ack = errType.GetConstructor(Type.EmptyTypes).Invoke(null) as IMessage;

			MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, "Validation Errors:");

			errors.ForEach(o => Trace.TraceError("\t{0} : {1}", o.Type, o.Message));

			Terser terser = new Terser(ack);

			MessageUtil.UpdateMSH(terser, request);

			int errLevel = 0;

			int ec = 0;

			foreach (var dtl in errors)
			{
				try
				{
					ISegment errSeg;
					if (ack.Version == "2.5")
						errSeg = terser.getSegment(String.Format("/ERR({0})", ec++));
					else
						errSeg = terser.getSegment(String.Format("/ERR", ec++));

					if (errSeg is NHapi.Model.V231.Segment.ERR)
					{
						var tErr = MessageUtil.UpdateERR(errSeg as NHapi.Model.V231.Segment.ERR, dtl);

						if (tErr > errLevel)
						{
							errLevel = tErr;
						}
					}
					else if (errSeg is NHapi.Model.V25.Segment.ERR)
					{
						var tErr = MessageUtil.UpdateERR(errSeg as ERR, dtl);

						if (tErr > errLevel)
						{
							errLevel = tErr;
						}
					}
				}
				catch (Exception e)
				{
					Trace.TraceError(e.ToString());
				}
			}

			terser.Set("/MSA-1", errLevel == 0 ? "AA" : errLevel == 1 ? "AE" : "AR");

			return ack;
		}

		internal static IMessage CreateRSP_K23(IEnumerable<Patient> result, List<IResultDetail> dtls)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Validate the message
		/// </summary>
		internal static List<IResultDetail> Validate(IMessage message, List<IResultDetail> details)
		{
			var config = ApplicationContext.Current.Configuration;

			// Structure validation
			PipeParser pipeParser = new PipeParser() { ValidationContext = new DefaultValidation() };

			try
			{
				pipeParser.Encode(message);
			}
			catch (Exception e)
			{
				details.Add(new ValidationResultDetail(ResultDetailType.Error, e.Message, e));
			}

			return details;
		}

		/// <summary>
		/// Map detail to error code
		/// </summary>
		/// <param name="dtl"></param>
		/// <returns></returns>
		private static string MapErrCode(IResultDetail dtl)
		{
			string errCode = string.Empty;
			string errSys = "2.16.840.1.113883.5.1100";

			if (dtl is InsufficientRepetitionsResultDetail)
				errCode = "100";
			else if (dtl is MandatoryElementMissingResultDetail)
				errCode = "101";
			else if (dtl is NotImplementedElementResultDetail)
				errCode = "207";
			else if (dtl is RequiredElementMissingResultDetail)
				errCode = "101";
			else if (dtl.Exception is DataException)
				errCode = "207";
			else if (dtl is VocabularyIssueResultDetail)
				errCode = "103";
			else if (dtl is FixedValueMisMatchedResultDetail)
				errCode = "103";
			else if (dtl is FormalConstraintViolationResultDetail)
				errCode = "207";
			else if (dtl.Exception is VersionNotFoundException)
				errCode = "203";
			else if (dtl.Exception is NotImplementedException)
				errCode = "200";
			else if (dtl.Exception is KeyNotFoundException)
				errCode = "204";
			else if (dtl is SecurityException)
				errCode = "901";
			else
				errCode = "207";
			return errCode;
		}

		/// <summary>
		/// Update an ERR
		/// </summary>
		private static int UpdateERR(NHapi.Model.V231.Segment.ERR err, IResultDetail dtl)
		{
			var locale = ApplicationContext.Current.GetService(typeof(ILocalizationService)) as ILocalizationService;

			// Determine the type of acknowledgement

			var errCode = MessageUtil.MapErrCode(dtl);

			var eld = err.GetErrorCodeAndLocation(err.ErrorCodeAndLocationRepetitionsUsed);
			eld.CodeIdentifyingError.Text.Value = locale.GetString(String.Format("HL7{0}", errCode));
			eld.CodeIdentifyingError.AlternateText.Value = dtl.Message;
			eld.CodeIdentifyingError.Identifier.Value = errCode;

			if (dtl.Location != null && dtl.Location.Contains("^"))
			{
				var cmp = dtl.Location.Split('^');
				for (int i = 0; i < cmp.Length; i++)
				{
					var st = eld.SegmentID as NHapi.Model.V231.Datatype.ST;

					if (string.IsNullOrEmpty(st.Value))
					{
						st.Value = cmp[i];
					}
					else
					{
						var nm = eld.FieldPosition as NHapi.Model.V231.Datatype.NM;

						if (nm != null)
						{
							nm.Value = cmp[i];
						}
					}
				}
			}

			return Int32.Parse(errCode[0].ToString());
		}
	}
}