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
 * User: khannan
 * Date: 2016-8-14
 */
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Base.validation.impl;
using NHapi.Model.V231.Datatype;
using NHapi.Model.V231.Message;
using NHapi.Model.V25.Segment;
using OpenIZ.Core;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.ResultsDetails;
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
using ERR = NHapi.Model.V231.Segment.ERR;
using EVN = NHapi.Model.V231.Segment.EVN;
using MSH = NHapi.Model.V231.Segment.MSH;
using PD1 = NHapi.Model.V231.Segment.PD1;
using PID = NHapi.Model.V231.Segment.PID;

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
		private static readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Converts an <see cref="XAD"/> address instance to an <see cref="EntityAddress"/> instance.
		/// </summary>
		/// <param name="address">The address to be converted.</param>
		/// <returns>Returns the converted entity address.</returns>
		public static EntityAddress ConvertAddress(XAD address)
		{
			return MessageUtil.ConvertAddresses(new XAD[] { address }).FirstOrDefault();
		}

		/// <summary>
		/// Converts a list of <see cref="XAD"/> addresses to a list of <see cref="EntityAddress"/> addresses.
		/// </summary>
		/// <param name="addresses">The addresses to be converted.</param>
		/// <returns>Returns a list of entity addresses.</returns>
		public static IEnumerable<EntityAddress> ConvertAddresses(XAD[] addresses)
		{
			var entityAddresses = new List<EntityAddress>();

			if (addresses.Length == 0)
			{
				return entityAddresses.AsEnumerable();
			}

			for (int i = 0; i < addresses.Length; i++)
			{
				var entityAddress = new EntityAddress();

				var addressUse = AddressUseKeys.TemporaryAddress;

				if (!string.IsNullOrEmpty(addresses[i].AddressType.Value) && !string.IsNullOrWhiteSpace(addresses[i].AddressType.Value))
				{
					var concept = GetConcept(addresses[i].AddressType.Value, "urn:oid:2.16.840.1.113883.5.1");

					// TODO: cleanup
					if (concept == null)
					{
						throw new ArgumentException("Code not known");
					}

					addressUse = concept.Key.Value;
				}

				entityAddress.AddressUse = new Concept
				{
					Key = addressUse
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

				if (!string.IsNullOrEmpty(addresses[i].StreetAddress.Value) && !string.IsNullOrWhiteSpace(addresses[i].StreetAddress.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.StreetAddressLine, addresses[i].StreetAddress.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].StateOrProvince.Value) && !string.IsNullOrWhiteSpace(addresses[i].StateOrProvince.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.State, addresses[i].StateOrProvince.Value));
				}

				if (!string.IsNullOrEmpty(addresses[i].StreetAddress.Value) && !string.IsNullOrWhiteSpace(addresses[i].StreetAddress.Value))
				{
					entityAddress.Component.Add(new EntityAddressComponent(AddressComponentKeys.StreetName, addresses[i].StreetAddress.Value));
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
			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetAssigningAuthorityService();

			AssigningAuthority assigningAuthority = null;

			if (id == null)
			{
				details.Add(new MandatoryElementMissingResultDetail(ResultDetailType.Error, null, null));
				return null;
			}

			if (!string.IsNullOrEmpty(id.NamespaceID.Value))
			{
				assigningAuthority = assigningAuthorityRepositoryService.Find(a => a.Name == id.NamespaceID.Value).FirstOrDefault();
			}

			if (!string.IsNullOrEmpty(id.UniversalID.Value))
			{
				assigningAuthority = assigningAuthorityRepositoryService.Find(a => a.Oid == id.UniversalID.Value).FirstOrDefault();
			}

			if (!string.IsNullOrEmpty(id.UniversalIDType.Value) && id.UniversalIDType.Value != "ISO")
			{
				details.Add(new NotImplementedResultDetail(ResultDetailType.Warning, ApplicationContext.Current.GetLocaleString("MSGW016"), null));
			}

			return assigningAuthority;
		}

		/// <summary>
		/// Converts an <see cref="NHapi.Model.V25.Datatype.HD"/> instance to an <see cref="AssigningAuthority"/> instance.
		/// </summary>
		/// <param name="id">The id value to be converted.</param>
		/// <returns>Returns the converted assigning authority.</returns>
		public static AssigningAuthority ConvertAssigningAuthority(NHapi.Model.V25.Datatype.HD id, List<IResultDetail> details)
		{
			var assigningAuthorityRepositoryService = ApplicationContext.Current.GetAssigningAuthorityService();

			AssigningAuthority assigningAuthority = null;

			if (id == null)
			{
				details.Add(new MandatoryElementMissingResultDetail(ResultDetailType.Error, null, null));
				return assigningAuthority;
			}

			if (!string.IsNullOrEmpty(id.NamespaceID.Value))
			{
				assigningAuthority = assigningAuthorityRepositoryService.Find(a => a.Name == id.NamespaceID.Value).FirstOrDefault();
			}

			if (!string.IsNullOrEmpty(id.UniversalID.Value))
			{
				assigningAuthority = assigningAuthorityRepositoryService.Find(a => a.Oid == id.UniversalID.Value).FirstOrDefault();
			}

			if (!string.IsNullOrEmpty(id.UniversalIDType.Value) && id.UniversalIDType.Value != "ISO")
			{
				details.Add(new NotImplementedResultDetail(ResultDetailType.Warning, ApplicationContext.Current.GetLocaleString("MSGW016"), null));
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
			var entityIdentifiers = new List<EntityIdentifier>();

			if (identifiers.Length == 0)
			{
				return entityIdentifiers.AsEnumerable();
			}

			for (var i = 0; i < identifiers.Length; i++)
			{
				var entityIdentifier = new EntityIdentifier();

				var cx = identifiers[i];

				var assigningAuthorityService = ApplicationContext.Current.GetAssigningAuthorityService();

				if (assigningAuthorityService == null)
				{
					throw new InvalidOperationException($"Unable to locate {nameof(IAssigningAuthorityRepositoryService)}");
				}

				var assigningAuthority = assigningAuthorityService.Find(a => a.Oid == cx.AssigningAuthority.UniversalID.Value).FirstOrDefault();

				if (assigningAuthority == null)
				{
					tracer.TraceEvent(TraceEventType.Warning, 0, $"Assigning authority OID not found: {cx.AssigningAuthority.UniversalID.Value}");
				}
				else
				{
#if DEBUG
					tracer.TraceEvent(TraceEventType.Information, 0, $"Adding {cx.ID.Value}^^^&{cx.AssigningAuthority.UniversalID.Value}&ISO to alternate identifiers");
#endif
					tracer.TraceEvent(TraceEventType.Information, 0, $"Adding identifier from {cx.AssigningAuthority.UniversalID.Value} domain to alternate identifiers");

					entityIdentifier.Authority = assigningAuthority;
					entityIdentifier.Value = cx.ID.Value;
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
			var entityNames = new List<EntityName>();

			if (names.Length == 0)
			{
				return entityNames.AsEnumerable();
			}

			for (var i = 0; i < names.Length; i++)
			{
				var entityName = new EntityName();

				var nameUse = NameUseKeys.Search;

				if (!string.IsNullOrEmpty(names[i].NameTypeCode.Value) && !string.IsNullOrWhiteSpace(names[i].NameTypeCode.Value))
				{
					var concept = GetConcept(names[i].NameTypeCode.Value, "urn:oid:2.16.840.1.113883.5.1120");

					// TODO: cleanup
					if (concept == null)
					{
						throw new ArgumentException("Code not known");
					}

					nameUse = concept.Key.Value;
				}

				entityName.NameUse = new Concept
				{
					Key = nameUse
				};

				if (!string.IsNullOrEmpty(names[i].FamilyLastName.FamilyName.Value) && !string.IsNullOrWhiteSpace(names[i].FamilyLastName.FamilyName.Value))
				{
					entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Family, names[i].FamilyLastName.FamilyName.Value));
				}

				if (!string.IsNullOrEmpty(names[i].GivenName.Value) && !string.IsNullOrWhiteSpace(names[i].GivenName.Value))
				{
					entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Given, names[i].GivenName.Value));
				}

				if (!string.IsNullOrEmpty(names[i].MiddleInitialOrName.Value) && !string.IsNullOrWhiteSpace(names[i].MiddleInitialOrName.Value))
				{
					entityName.Component.Add(new EntityNameComponent(NameComponentKeys.Given, names[i].MiddleInitialOrName.Value));
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

			if (timestamp.TimeOfAnEvent.Value == null)
			{
				return result;
			}

			object dateTime;

			if (Util.TryFromWireFormat(timestamp.TimeOfAnEvent.Value, typeof(MARC.Everest.DataTypes.TS), out dateTime))
			{
				result = ((MARC.Everest.DataTypes.TS)dateTime).DateValue;
			}
			else
			{
#if DEBUG
				tracer.TraceEvent(TraceEventType.Warning, 0, $"Unable to convert TS value: {timestamp.TimeOfAnEvent.Value}");
#endif
				tracer.TraceEvent(TraceEventType.Warning, 0, "Unable to convert TS value");
			}

			return result;
		}

		/// <summary>
		/// Converts a <see cref="XTN"/> instance to a <see cref="EntityTelecomAddress"/> instance.
		/// </summary>
		/// <param name="xtn">The v2 XTN instance to be converted.</param>
		/// <returns>Returns the converted entity telecom address instance.</returns>
		public static EntityTelecomAddress ConvertXTN(XTN xtn)
		{
			var re = new Regex(@"([+0-9A-Za-z]{1,4})?\((\d{3})\)?(\d{3})\-(\d{4})X?(\d{1,6})?");
			var retVal = new EntityTelecomAddress();

			if (xtn.AnyText.Value == null)
			{
				var sb = new StringBuilder("tel:");

				try
				{
					if (xtn.CountryCode.Value != null)
					{
						sb.AppendFormat("{0}-", xtn.CountryCode);
					}

					if (xtn.PhoneNumber?.Value != null && !xtn.PhoneNumber.Value.Contains("-"))
					{
						xtn.PhoneNumber.Value = xtn.PhoneNumber.Value.Insert(3, "-");
					}

					sb.AppendFormat("{0}-{1}", xtn.AreaCityCode, xtn.PhoneNumber);

					if (xtn.Extension.Value != null)
					{
						sb.AppendFormat(";ext={0}", xtn.Extension);
					}
				}
				catch
				{
					// ignored
				}

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

				var sb = new StringBuilder("tel:");

				for (var i = 1; i < 5; i++)
				{
					if (!string.IsNullOrEmpty(match.Groups[i].Value))
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
			var use = Guid.Empty;

			if (!string.IsNullOrEmpty(xtn.TelecommunicationUseCode.Value))
			{
				var concept = GetConcept(xtn.TelecommunicationUseCode.Value, "urn:oid:2.16.840.1.113883.5.1011");

				// TODO: cleanup
				if (concept == null)
				{
					throw new ArgumentException("Code not known");
				}

				use = concept.Key.Value;
			}

			retVal.AddressUseKey = use;

			return retVal;
		}

		public static Expression<Func<Patient, bool>> CreateIDQuery(QPD qpd)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a negative acknowledgement message.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="errors">The errors.</param>
		/// <param name="errType">Type of the error.</param>
		/// <returns>Returns the created negative acknowledgement message instance.</returns>
		public static IMessage CreateNack(IMessage request, List<IResultDetail> errors, Type errType)
		{
			var ack = errType.GetConstructor(Type.EmptyTypes).Invoke(null) as IMessage;

			if (errors.Any())
			{
				tracer.TraceEvent(TraceEventType.Warning, 0, "Validation Errors:");

				errors.ForEach(o => tracer.TraceEvent(TraceEventType.Error, 0, $"\t{o.Type} : {o.Message}"));
			}

			var terser = new Terser(ack);

			MessageUtil.UpdateMSH(terser, request);

			var errLevel = 0;

			var ec = 0;

			foreach (var dtl in errors)
			{
				try
				{
					ISegment errSeg;
					if (ack.Version == "2.5")
						errSeg = terser.getSegment($"/ERR({ec++})");
					else
						errSeg = terser.getSegment(string.Format("/ERR", ec++));

					if (errSeg is ERR)
					{
						var tErr = UpdateERR(errSeg as ERR, dtl);

						if (tErr > errLevel)
						{
							errLevel = tErr;
						}
					}
					else if (errSeg is NHapi.Model.V25.Segment.ERR)
					{
						var tErr = UpdateERR(errSeg as ERR, dtl);

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
		/// Creates a negative acknowledgement message.
		/// </summary>
		/// <param name="request">The request message to use to create the response message.</param>
		/// <param name="responseCode">The response code of the message.</param>
		/// <param name="errCode">The error code of the message.</param>
		/// <param name="errDescription">The error description of the message.</param>
		/// <returns>Returns the created negative acknowledgement message instance.</returns>
		public static IMessage CreateNack(IMessage request, string responseCode, string errCode, string errDescription)
		{
			MessageUtil.tracer.TraceEvent(TraceEventType.Warning, 0, $"NACK Condition : {errDescription}");

			if (request.Version == "2.3.1")
			{
				var ack = new ACK();

				var terser = new Terser(ack);

				terser.Set("/MSA-1", responseCode);
				terser.Set("/MSA-3", "Error occurred");
				terser.Set("/MSA-6-1", errCode);
				terser.Set("/MSA-6-2", errDescription);

				UpdateMSH(terser, request);

				return ack;
			}
			else
			{
				var ack = new NHapi.Model.V25.Message.ACK();

				var terser = new Terser(ack);

				terser.Set("/MSA-1", responseCode);

				UpdateMSH(terser, request);

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
			var patient = new Patient
			{
				Addresses = ConvertAddresses(pid.GetPatientAddress()).ToList(),
				DateOfBirth = ConvertTS(pid.DateTimeOfBirth),
				DeceasedDate = ConvertTS(pid.PatientDeathDateAndTime),
				GenderConceptKey = GetConcept(pid.Sex.Value, "urn:oid:2.16.840.1.113883.5.1")?.Key,
				Names = ConvertNames(pid.GetPatientName()).ToList(),
			};

			if (patient.GenderConceptKey == null || (patient.GenderConceptKey.HasValue && patient.GenderConceptKey.Value == Guid.Empty))
			{
				details.Add(new MandatoryElementMissingResultDetail(ResultDetailType.Error, null, null));
				details.Add(new NotSupportedChoiceResultDetail(ResultDetailType.Error, null, null));
			}

			patient.Identifiers.AddRange(MessageUtil.ConvertIdentifiers(pid.GetPatientIdentifierList()));
			patient.Identifiers.AddRange(MessageUtil.ConvertIdentifiers(pid.GetAlternatePatientIDPID()));

			if (!string.IsNullOrEmpty(pid.PrimaryLanguage.Identifier.Value) && !string.IsNullOrWhiteSpace(pid.PrimaryLanguage.Identifier.Value))
			{
				patient.LanguageCommunication.Add(new PersonLanguageCommunication(pid.PrimaryLanguage.Identifier.Value, true));
			}

			if (!string.IsNullOrEmpty(pid.MultipleBirthIndicator.Value) && !string.IsNullOrWhiteSpace(pid.MultipleBirthIndicator.Value))
			{
				patient.MultipleBirthOrder = Convert.ToInt32(pid.MultipleBirthIndicator.Value);
			}

			patient.Telecoms.AddRange(pid.GetPhoneNumberHome().Select(ConvertXTN));
			patient.Telecoms.AddRange(pid.GetPhoneNumberBusiness().Select(ConvertXTN));

			if (pid.MotherSIdentifierRepetitionsUsed > 0 || pid.MotherSMaidenNameRepetitionsUsed > 0)
			{
				var person = new Person
				{
					Identifiers = ConvertIdentifiers(pid.GetMotherSIdentifier()).ToList(),
					Names = ConvertNames(pid.GetMotherSMaidenName()).ToList(),
				};

				patient.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Mother, person));
			}

			return patient;
		}

		public static IMessage CreateRSPK23(IEnumerable<Patient> result, List<IResultDetail> dtls)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the code.
		/// </summary>
		/// <param name="conceptKey">The concept key.</param>
		/// <param name="codeSystemKey">The code system key.</param>
		/// <returns>System.String.</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		public static string GetCode(Guid conceptKey, Guid codeSystemKey)
		{
			var conceptService = ApplicationContext.Current.GetConceptService();

			var concept = conceptService.GetConcept(conceptKey, Guid.Empty) as Concept;

			if (concept == null)
			{
				throw new InvalidOperationException($"Concept not found using key: {conceptKey}");
			}

			// force load the reference terms because we need to compare against the code system
			foreach (var conceptReferenceTerm in concept.ReferenceTerms.Where(cr => cr.ReferenceTerm == null && cr.ReferenceTermKey != null))
			{
				conceptReferenceTerm.ReferenceTerm = conceptService.GetReferenceTerm(conceptReferenceTerm.ReferenceTermKey.Value);
			}

			return concept.ReferenceTerms.FirstOrDefault(c => c.ReferenceTerm.CodeSystemKey == codeSystemKey)?.ReferenceTerm.Mnemonic;
		}

		/// <summary>
		/// Gets the concept.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="codeSystem">The code system.</param>
		/// <returns>Concept.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// code - Value cannot be null
		/// or
		/// codeSystem - Value cannot be null
		/// </exception>
		public static Concept GetConcept(string code, string codeSystem)
		{
			var conceptService = ApplicationContext.Current.GetConceptService();

			if (code == null)
			{
				throw new ArgumentNullException(nameof(code), "Value cannot be null");
			}

			if (codeSystem == null)
			{
				throw new ArgumentNullException(nameof(codeSystem), "Value cannot be null");
			}

			return conceptService.FindConceptsByReferenceTerm(code, new Uri(codeSystem)).FirstOrDefault();
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
			var errCode = MapErrCode(dtl);

			err.HL7ErrorCode.Identifier.Value = errCode;
			err.HL7ErrorCode.Text.Value = locale?.GetString($"HL7{errCode}");

			if (dtl.Location != null && dtl.Location.Contains("^"))
			{
				var cmp = dtl.Location.Split('^');
				for (int i = 0; i < cmp.Length; i++)
				{
					var st = err.GetErrorLocation(0).Components[i] as ST;

					if (st != null)
					{
						st.Value = cmp[i];
					}
					else
					{
						var nm = err.GetErrorLocation(0).Components[i] as NM;

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
		/// Validates a message.
		/// </summary>
		/// <param name="message">The message to be validated.</param>
		/// <param name="details">The result details for storing the message validation results.</param>
		/// <returns>Returns a list of validation results.</returns>
		public static List<IResultDetail> Validate(IMessage message, List<IResultDetail> details)
		{
			var pipeParser = new PipeParser() { ValidationContext = new DefaultValidation() };

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
				var terser = new Terser(message);

				var msh = terser.getSegment("MSH");

				if (msh is MSH)
				{
					var v231Msh = (MSH)msh;

					var result = ConvertAssigningAuthority(v231Msh.SendingApplication, details);

					if (result == null)
					{
						details.Add(new UnrecognizedSenderResultDetail(ResultDetailType.Error, v231Msh.SendingApplication.NamespaceID.Value, "MSH^3"));
					}
				}
				else if (msh is NHapi.Model.V25.Segment.MSH)
				{
					var v25Msh = (NHapi.Model.V25.Segment.MSH)msh;

					var result = ConvertAssigningAuthority(v25Msh.SendingApplication, details);

					if (result == null)
					{
						details.Add(new UnrecognizedSenderResultDetail(ResultDetailType.Error, v25Msh.SendingApplication.NamespaceID.Value, "MSH^3"));
					}
				}
				else
				{
					details.Add(new MandatoryElementMissingResultDetail(ResultDetailType.Error, "Missing MSH", "MSH"));
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
				var concept = GetConcept(instance.TelecommunicationUseCode.Value, "urn:oid:2.16.840.1.113883.5.1011");

				// TODO: cleanup
				if (concept == null)
				{
					throw new ArgumentException("Code not known");
				}

				instance.TelecommunicationUseCode.Value = concept.Mnemonic;
			}
		}

		/// <summary>
		/// Converts an <see cref="EntityTelecomAddress"/> instance to an <see cref="NHapi.Model.V231.Datatype.XTN"/> instance.
		/// </summary>
		/// <param name="tel">The entity telecom address instance to be converted.</param>
		/// <param name="instance">The converted XTN instance.</param>
		public static void XTNFromTel(EntityTelecomAddress tel, NHapi.Model.V231.Datatype.XTN instance)
		{
			var v25Instance = new NHapi.Model.V25.Datatype.XTN(instance.Message);

			XTNFromTel(tel, v25Instance);

			for (var i = 0; i < v25Instance.Components.Length; i++)
			{
				if (v25Instance.Components[i] is AbstractPrimitive && i < instance.Components.Length)
				{
					(instance.Components[i] as AbstractPrimitive).Value = (v25Instance.Components[i] as AbstractPrimitive).Value;
				}
			}
		}
	}
}