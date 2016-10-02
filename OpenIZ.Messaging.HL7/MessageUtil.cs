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
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Segment;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
		/// The public reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private static TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// The public reference to the HL7v2 Address use conversion map.
		/// </summary>
		private static readonly Dictionary<string, Guid> addressUseMap = new Dictionary<string, Guid>
		{
			{ "B", AddressUseKeys.WorkPlace },
			{ "BA", AddressUseKeys.BadAddress },
			{ "BDL", AddressUseKeys.TemporaryAddress },
			{ "BR", AddressUseKeys.HomeAddress },
			{ "C", AddressUseKeys.TemporaryAddress },
			{ "F", AddressUseKeys.TemporaryAddress },
			{ "H", AddressUseKeys.HomeAddress },
			{ "L", AddressUseKeys.PrimaryHome },
			{ "M", AddressUseKeys.PostalAddress },
			{ "N", AddressUseKeys.TemporaryAddress },
			{ "O", AddressUseKeys.WorkPlace },
			{ "P", AddressUseKeys.PrimaryHome },
			{ "RH", AddressUseKeys.PrimaryHome }
		};

		/// <summary>
		/// The public reference to the HL7v2 gender conversion map.
		/// </summary>
		private static readonly Dictionary<string, Guid> genderMap = new Dictionary<string, Guid>
		{
			{ "F", Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c") },
			{ "M", Guid.Parse("f4e3a6bb-612e-46b2-9f77-ff844d971198") },
			{ "U", Guid.Parse("ae94a782-1485-4241-9bca-5b09db2156bf") }
		};

		/// <summary>
		/// The public reference to the HL7v2 name use conversion map.
		/// </summary>
		private static readonly Dictionary<string, Guid> nameUseMap = new Dictionary<string, Guid>
		{
			{ "A", NameUseKeys.Pseudonym },
			{ "B", NameUseKeys.OfficialRecord },
			{ "C", NameUseKeys.OfficialRecord },
			{ "D", NameUseKeys.Ideographic },
			{ "I", NameUseKeys.Legal },
			{ "L", NameUseKeys.Legal },
			{ "M", NameUseKeys.MaidenName },
			{ "N", NameUseKeys.Pseudonym },
			{ "P", NameUseKeys.Search },
			{ "S", NameUseKeys.Anonymous },
			{ "T", NameUseKeys.Indigenous },
			{ "U", NameUseKeys.Anonymous }
		};

		/// <summary>
		/// The public reference to the HL7v2 to Telecommunications use conversion map.
		/// </summary>
		private static readonly Dictionary<string, Guid> telecommunicationsMap = new Dictionary<string, Guid>()
		{
			{ "ASN", TelecomAddressUseKeys.AnsweringService },
			{ "BPN", TelecomAddressUseKeys.Pager },
			{ "EMR", TelecomAddressUseKeys.EmergencyContact },
			{ "PRN", TelecomAddressUseKeys.Public },
			{ "CEL", TelecomAddressUseKeys.MobileContact },
			{ "WPN", TelecomAddressUseKeys.WorkPlace }
		};

		/// <summary>
		/// The public reference to the <see cref="IAssigningAuthorityRepositoryService"/> instance.
		/// </summary>
		private static IAssigningAuthorityRepositoryService assigningAuthorityRepositoryService;

		/// <summary>
		/// The public reference to the <see cref="IConceptRepositoryService"/> instance.
		/// </summary>
		private static IConceptRepositoryService conceptRespositoryService;

		/// <summary>
		/// The public reference to the <see cref="ILocalizationService"/> instance.
		/// </summary>
		private static ILocalizationService localizationService;

		static MessageUtil()
		{
			ApplicationContext.Current.Started += (o, e) =>
			{
				assigningAuthorityRepositoryService = ApplicationContext.Current.GetService<IAssigningAuthorityRepositoryService>();

				if (assigningAuthorityRepositoryService == null)
				{
					MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, string.Format("Unable to locate service: {0}", nameof(IAssigningAuthorityRepositoryService)));
				}

				conceptRespositoryService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

				if (conceptRespositoryService == null)
				{
					MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, string.Format("Unable to locate service: {0}", nameof(IConceptRepositoryService)));
				}

				localizationService = ApplicationContext.Current.GetService<ILocalizationService>();

				if (localizationService == null)
				{
					MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, string.Format("Unable to locate service: {0}", nameof(ILocalizationService)));
				}
			};
		}

		/// <summary>
		/// Converts a list of <see cref="XAD"/> addresses to a list of <see cref="EntityAddress"/> addresses.
		/// </summary>
		/// <param name="addresses">The addresses to be converted.</param>
		/// <returns>Returns a list of entity addresses.</returns>
		public static IEnumerable<EntityAddress> ConvertAddresses(XAD[] addresses)
		{
			List<EntityAddress> entityAddresses = new List<EntityAddress>();

			if (addresses.Length == 0)
			{
				entityAddresses.AsEnumerable();
			}

			for (int i = 0; i < addresses.Length; i++)
			{
				EntityAddress entityAddress = new EntityAddress();

				entityAddress.AddressUse = new Concept
				{
					Key = addressUseMap[addresses[i].AddressType.Value]
				};

				if (!string.IsNullOrEmpty(addresses[i].CensusTract.Value) && !string.IsNullOrWhiteSpace(addresses[i].CensusTract.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.CensusTract, addresses[i].CensusTract.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].City.Value) && !string.IsNullOrWhiteSpace(addresses[i].City.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.City, addresses[i].City.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].Country.Value) && !string.IsNullOrWhiteSpace(addresses[i].Country.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.Country, addresses[i].Country.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].ZipOrPostalCode.Value) && !string.IsNullOrWhiteSpace(addresses[i].ZipOrPostalCode.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.PostalCode, addresses[i].ZipOrPostalCode.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].StreetAddress.StreetOrMailingAddress.Value) && !string.IsNullOrWhiteSpace(addresses[i].StreetAddress.StreetOrMailingAddress.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.StreetAddressLine, addresses[i].StreetAddress.StreetOrMailingAddress.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].StreetAddress.StreetName.Value) && !string.IsNullOrWhiteSpace(addresses[i].StreetAddress.StreetName.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.StreetName, addresses[i].StreetAddress.StreetName.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].OtherDesignation.Value) && !string.IsNullOrWhiteSpace(addresses[i].OtherDesignation.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.UnitDesignator, addresses[i].OtherDesignation.Value));
				}

				entityAddresses.Add(entityAddress);
			}

			return entityAddresses.AsEnumerable();
		}

		/// <summary>
		/// Converts an <see cref="HD"/> instance to an <see cref="AssigningAuthority"/> instance.
		/// </summary>
		/// <param name="id">The id value to be converted.</param>
		/// <returns>Returns the converted assigning authority.</returns>
		public static AssigningAuthority ConvertAssigningAuthority(HD id, List<IResultDetail> details)
		{
			AssigningAuthority assigningAuthority = null;

			if (id == null)
			{
				details.Add(new MandatoryElementMissingResultDetail(ResultDetailType.Error, null, null));
				return assigningAuthority;
			}

			if (!string.IsNullOrEmpty(id.NamespaceID.Value))
			{
				assigningAuthority = MessageUtil.assigningAuthorityRepositoryService.Find(a => a.Oid == id.NamespaceID.Value).FirstOrDefault();
			}

			if (!string.IsNullOrEmpty(id.UniversalID.Value))
			{
				assigningAuthority = MessageUtil.assigningAuthorityRepositoryService.Find(a => a.Oid == id.UniversalID.Value).FirstOrDefault();
			}

			if (!string.IsNullOrEmpty(id.UniversalIDType.Value) && id.UniversalIDType.Value != "ISO")
			{
				details.Add(new NotImplementedResultDetail(ResultDetailType.Warning, MessageUtil.localizationService.GetString("MSGW016"), null));
			}

			if (assigningAuthority == null)
			{
				details.Add(new MandatoryElementMissingResultDetail(ResultDetailType.Error, null, null));
			}

			return assigningAuthority;
		}

		/// <summary>
		/// Converts an <see cref="CX"/> instance to an <see cref="EntityIdentifier"/> instance.
		/// </summary>
		/// <param name="identifier">The identifier to be converted.</param>
		/// <returns>Returns the converted identifier.</returns>
		public static EntityIdentifier ConvertIdentifier(CX identifier)
		{
			return MessageUtil.ConvertIdentifiers(new CX[] { identifier }).FirstOrDefault();
		}

		/// <summary>
		/// Converts a list of <see cref="CX"/> identifiers to a list of <see cref="EntityIdentifier"/> identifiers.
		/// </summary>
		/// <param name="identifiers">The list of identifiers to be converted.</param>
		/// <returns>Returns a list of entity identifiers.</returns>
		public static IEnumerable<EntityIdentifier> ConvertIdentifiers(CX[] identifiers)
		{
			List<EntityIdentifier> entityIdentifiers = new List<EntityIdentifier>();

			if (identifiers.Length == 0)
			{
				return entityIdentifiers.AsEnumerable();
			}

			for (int i = 0; i < identifiers.Length; i++)
			{
				EntityIdentifier entityIdentifier = new EntityIdentifier();

				var cx = identifiers[i];

				var assigningAuthority = MessageUtil.assigningAuthorityRepositoryService.Find(a => a.Oid == cx.AssigningAuthority.UniversalID.Value).FirstOrDefault();

				if (assigningAuthority == null)
				{
					MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, string.Format("Assigning authority OID not found: {0}", cx.AssigningAuthority.UniversalID.Value));
				}
				else
				{
#if DEBUG
					MessageUtil.tracer.TraceEvent(TraceEventType.Information, 0, string.Format("Adding {0}^^^&{1}&ISO to alternate identifiers", cx.IDNumber.Value, cx.AssigningAuthority.UniversalID.Value));
#endif
					MessageUtil.tracer.TraceEvent(TraceEventType.Information, 0, string.Format("Adding identifier from {0} domain to alternate identifiers", cx.AssigningAuthority.UniversalID.Value));

					entityIdentifier.Authority = assigningAuthority;
					entityIdentifier.Value = cx.IDNumber.Value;
				}

				entityIdentifiers.Add(entityIdentifier);
			}

			return entityIdentifiers.AsEnumerable();
		}

		/// <summary>
		/// Converts an <see cref="XPN"/> instance to an <see cref="EntityName"/> instance.
		/// </summary>
		/// <param name="name">The name to be converted.</param>
		/// <returns>Returns the converted name.</returns>
		public static EntityName ConvertName(XPN name)
		{
			return MessageUtil.ConvertNames(new XPN[] { name }).FirstOrDefault();
		}

		/// <summary>
		/// Converts a list of <see cref="XPN"/> names to a list of <see cref="EntityName"/> names.
		/// </summary>
		/// <param name="names">THe list of names to be converted.</param>
		/// <returns>Returns a list of entity names.</returns>
		public static IEnumerable<EntityName> ConvertNames(XPN[] names)
		{
			List<EntityName> entityNames = new List<EntityName>();

			if (names.Length == 0)
			{
				return entityNames.AsEnumerable();
			}

			for (int i = 0; i < names.Length; i++)
			{
				EntityName entityName = new EntityName();

				entityName.NameUse = new Concept
				{
					Key = nameUseMap[names[i].NameTypeCode.Value]
				};

				if (!string.IsNullOrEmpty(names[i].FamilyName.Surname.Value) && !string.IsNullOrWhiteSpace(names[i].FamilyName.Surname.Value))
				{
					entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Family, names[i].FamilyName.Surname.Value));
				}

				if (!string.IsNullOrEmpty(names[i].GivenName.Value) && !string.IsNullOrWhiteSpace(names[i].GivenName.Value))
				{
					entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Given, names[i].GivenName.Value));
				}

				if (!string.IsNullOrEmpty(names[i].PrefixEgDR.Value) && !string.IsNullOrWhiteSpace(names[i].PrefixEgDR.Value))
				{
					entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Prefix, names[i].PrefixEgDR.Value));
				}

				if (!string.IsNullOrEmpty(names[i].DegreeEgMD.Value) && !string.IsNullOrWhiteSpace(names[i].DegreeEgMD.Value))
				{
					entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Suffix, names[i].DegreeEgMD.Value));
				}

				entityNames.Add(entityName);
			}

			return entityNames.AsEnumerable();
		}

		/// <summary>
		/// Converts a <see cref="TS"/> instance to a <see cref="DateTime"/> instance.
		/// </summary>
		/// <param name="timestamp">The TS instance to be converted.</param>
		/// <returns>Returns the converted date time instance or null.</returns>
		public static DateTime? ConvertTS(TS timestamp)
		{
			DateTime? result = null;

			object dateTime = null;

			if (Util.TryFromWireFormat(timestamp.Time.Value, typeof(MARC.Everest.DataTypes.TS), out dateTime))
			{
				result = Convert.ToDateTime(dateTime);
			}
			else
			{
#if DEBUG
				MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, string.Format("Unable to convert date of birth value: {0}", timestamp.Time.Value));
#endif
				MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, "Unable to convert date of birth value");
			}

			return result;
		}

		/// <summary>
		/// Converts a <see cref="NHapi.Model.V231.Datatype.XTN"/> instance to a <see cref="EntityTelecomAddress"/> instance.
		/// </summary>
		/// <param name="xtn">The v2 XTN instance to be converted.</param>
		/// <returns>Returns the converted entity telecom address instance.</returns>
		public static EntityTelecomAddress ConvertXTN(XTN xtn)
		{
			Regex re = new Regex(@"([+0-9A-Za-z]{1,4})?\((\d{3})\)?(\d{3})\-(\d{4})X?(\d{1,6})?");
			var retVal = new EntityTelecomAddress();

			if (xtn.AnyText.Value == null)
			{
				StringBuilder sb = new StringBuilder("tel:");

				try
				{
					if (xtn.CountryCode.Value != null)
					{
						sb.AppendFormat("{0}-", xtn.CountryCode);
					}

					if (xtn.TelephoneNumber != null && xtn.TelephoneNumber.Value != null && !xtn.TelephoneNumber.Value.Contains("-"))
					{
						xtn.TelephoneNumber.Value = xtn.TelephoneNumber.Value.Insert(3, "-");
					}

					sb.AppendFormat("{0}-{1}", xtn.AreaCityCode, xtn.TelephoneNumber);

					if (xtn.Extension.Value != null)
					{
						sb.AppendFormat(";ext={0}", xtn.Extension);
					}
				}
				catch { }

				if (sb.ToString().EndsWith("tel:") || sb.ToString() == "tel:-")
				{
					retVal.Value = "tel:" + xtn.AnyText.Value;
				}
				else
				{
					retVal.Value = sb.ToString();
				}
			}
			else
			{
				var match = re.Match(xtn.AnyText.Value);

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

			if (!string.IsNullOrEmpty(xtn.TelecommunicationUseCode.Value) && !telecommunicationsMap.TryGetValue(xtn.TelecommunicationUseCode.Value, out use))
			{
				throw new InvalidOperationException(string.Format("{0} is not a known use code", xtn.TelecommunicationUseCode.Value));
			}

			retVal.AddressUseKey = use;

			// Capability
			return retVal;
		}

		public static Expression<Func<Patient, bool>> CreateIDQuery(QPD qpd)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Create NACK
		/// </summary>
		public static IMessage CreateNack(IMessage request, List<IResultDetail> errors, Type errType)
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

		/// <summary>
		/// Create NACK
		/// </summary>
		public static IMessage CreateNack(IMessage request, string responseCode, string errCode, string errDescription)
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
		/// Converts components of a HL7v2 message to a <see cref="Patient"/> instance.
		/// </summary>
		/// <param name="msh">The message header segment.</param>
		/// <param name="evn">The event segment.</param>
		/// <param name="pid">The patient identification segment.</param>
		/// <param name="pd1">The patient pd1 segment.</param>
		/// <param name="details">The list of result details used for validation.</param>
		/// <returns>Returns the patient instance.</returns>
		public static Patient CreatePatient(MSH msh, EVN evn, PID pid, PD1 pd1, List<IResultDetail> details)
		{
			Patient patient = new Patient();

			patient.Addresses = MessageUtil.ConvertAddresses(pid.GetPatientAddress()).ToList();

			var dateOfBirth = MessageUtil.ConvertTS(pid.DateTimeOfBirth);

			if (dateOfBirth.HasValue)
			{
				patient.DateOfBirth = dateOfBirth;
			}
			else
			{
				details.Add(new MandatoryElementMissingResultDetail(ResultDetailType.Error, null, null));
				MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, "Patient doesn't have date of birth");
			}

			var dateOfDeath = MessageUtil.ConvertTS(pid.PatientDeathDateAndTime);

			if (dateOfDeath.HasValue)
			{
				patient.DeceasedDate = dateOfDeath.Value;
			}

			try
			{
				patient.GenderConcept = new Concept
				{
					Key = genderMap[pid.AdministrativeSex.Value]
				};
			}
			catch (KeyNotFoundException)
			{
				details.Add(new NotSupportedChoiceResultDetail(ResultDetailType.Error, null, null));
#if DEBUG
				MessageUtil.tracer.TraceEvent(TraceEventType.Error, 0, string.Format("Gender value {0} not found in map", pid.AdministrativeSex.Value));
#endif
				MessageUtil.tracer.TraceEvent(TraceEventType.Error, 0, "Gender value not found in map");
			}

			if (!string.IsNullOrEmpty(pid.PatientID.IDNumber.Value) && !string.IsNullOrWhiteSpace(pid.PatientID.IDNumber.Value))
			{
				patient.Identifiers.AddRange(MessageUtil.ConvertIdentifiers(new CX[] { pid.PatientID }));
			}

			patient.Identifiers.AddRange(MessageUtil.ConvertIdentifiers(pid.GetAlternatePatientIDPID()));

			patient.LanguageCommunication.Add(new PersonLanguageCommunication(pid.PrimaryLanguage.Identifier.Value, true));

			if (!string.IsNullOrEmpty(pid.MultipleBirthIndicator.Value) && !string.IsNullOrWhiteSpace(pid.MultipleBirthIndicator.Value))
			{
				patient.MultipleBirthOrder = Convert.ToInt32(pid.MultipleBirthIndicator.Value);
			}

			patient.Names = MessageUtil.ConvertNames(pid.GetPatientName()).ToList();

			foreach (var item in pid.GetPhoneNumberHome())
			{
				patient.Telecoms.Add(MessageUtil.ConvertXTN(item));
			}

			foreach (var item in pid.GetPhoneNumberBusiness())
			{
				patient.Telecoms.Add(MessageUtil.ConvertXTN(item));
			}

			return patient;
		}

		public static IMessage CreateRSPK23(IEnumerable<Patient> result, List<IResultDetail> dtls)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Map detail to error code
		/// </summary>
		/// <param name="dtl"></param>
		/// <returns></returns>
		public static string MapErrCode(IResultDetail dtl)
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
		/// Update an ERR
		/// </summary>
		public static int UpdateERR(NHapi.Model.V231.Segment.ERR err, IResultDetail dtl)
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
		/// Validate the message
		/// </summary>
		public static List<IResultDetail> Validate(IMessage message, List<IResultDetail> details)
		{
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

			try
			{
				Terser terser = new Terser(message);

				var msh = terser.getSegment("MSH") as MSH;

				if (msh != null)
				{
					MessageUtil.ConvertAssigningAuthority(msh.SendingApplication, details);
				}
			}
			catch (Exception e)
			{
				details.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
			}

			return details;
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
				foreach (var tcu in telecommunicationsMap)
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
	}
}