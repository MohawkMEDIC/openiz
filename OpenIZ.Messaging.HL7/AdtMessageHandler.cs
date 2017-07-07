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
using MARC.HI.EHRS.SVC.Messaging.HAPI;
using MARC.HI.EHRS.SVC.Messaging.HAPI.TransportProtocol;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Model.V25.Message;
using OpenIZ.Core;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using NHapi.Model.V231.Segment;
using OpenIZ.Core.Security;

namespace OpenIZ.Messaging.HL7
{
	/// <summary>
	/// Represents a message handler that can handle ADT messages from remote systems.
	/// </summary>
	public class AdtMessageHandler : IHL7MessageHandler
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource traceSource = new TraceSource("OpenIZ.Messaging.HL7");

    
        /// <summary>
        /// Handle a received message on the LLP interface.
        /// </summary>
        /// <param name="e">The HL7 message received event arguments.</param>
        public IMessage HandleMessage(Hl7MessageReceivedEventArgs e)
		{
			IMessage response = null;

			try
			{
				if (e.Message.Version == "2.3.1" || e.Message.Version == "2.5")
				{
					var identityProvider = ApplicationContext.Current.GetService<IDeviceIdentityProviderService>();

					var msh = e.Message.GetStructure("MSH") as MSH;

					// get the device identity by device name, as the device will have to be registered in OpenIZ
					var deviceIdentity = identityProvider.GetIdentity(msh.SendingApplication.NamespaceID.Value);

					AuthenticationContext.Current = new AuthenticationContext(new GenericPrincipal(deviceIdentity, new string[] {}));

					// Get the MSH segment
					var terser = new Terser(e.Message);
					var trigger = terser.Get("/MSH-9-2");

					this.traceSource.TraceEvent(TraceEventType.Information, 0, "Message is of type {0} {1}", e.Message.GetType().FullName, trigger);

					switch (trigger)
					{
						case "Q23":
							if (e.Message is NHapi.Model.V25.Message.QBP_Q21)
								response = HandlePixQuery(e.Message as NHapi.Model.V25.Message.QBP_Q21, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						case "A01":
						case "A04":
						case "A05":
							if (e.Message is NHapi.Model.V231.Message.ADT_A01)
								response = HandleAdmit((NHapi.Model.V231.Message.ADT_A01)e.Message, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						case "A08":
							if (e.Message is NHapi.Model.V231.Message.ADT_A01)
								response = HandlePixUpdate(e.Message as NHapi.Model.V231.Message.ADT_A01, e);
							else if (e.Message is NHapi.Model.V231.Message.ADT_A08)
								response = HandlePixUpdate(e.Message as NHapi.Model.V231.Message.ADT_A08, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						case "A40":
							if (e.Message is NHapi.Model.V25.Message.ADT_A39)
								response = HandleMerge(e.Message as NHapi.Model.V231.Message.ADT_A39, e);
							else if (e.Message is NHapi.Model.V231.Message.ADT_A40)
								response = HandleMerge(e.Message as NHapi.Model.V231.Message.ADT_A40, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						default:
							response = MessageUtil.CreateNack(e.Message, "AR", "201", ApplicationContext.Current.GetLocaleString("HL7201"));
							this.traceSource.TraceEvent(TraceEventType.Error, 0, "{0} is not a supported trigger", trigger);
							break;
					}
				}

				// response still null?
				if (response == null)
				{
					response = MessageUtil.CreateNack(e.Message, "AR", "203", ApplicationContext.Current.GetLocaleString("HL7203"));
				}
			}
			catch (Exception ex)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, ex.HResult, "Error processing message {0} : {1}", e.Message?.Version, ex);
				response = MessageUtil.CreateNack(e.Message, "AR", "207", ex.Message);
			}

			return response;
		}

		/// <summary>
		/// Handles a merge.
		/// </summary>
		/// <param name="message">The ADT_A40 message.</param>
		/// <param name="e">The <see cref="Hl7MessageReceivedEventArgs"/> instance containing the event data.</param>
		/// <returns>Returns the response message from the merge event.</returns>
		private IMessage HandleMerge(NHapi.Model.V231.Message.ADT_A40 message, Hl7MessageReceivedEventArgs e)
		{
			var parser = new PipeParser();

			message.MSH.MessageType.MessageStructure.Value = "ADT_A39";

			var encodedMessage = parser.Parse(parser.Encode(message));

			if (encodedMessage is NHapi.Model.V231.Message.ADT_A39)
			{
				return this.HandleMerge(encodedMessage as NHapi.Model.V231.Message.ADT_A39, e);
			}

			return MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
		}

		/// <summary>
		/// Handles the admit.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="eventArgs">The <see cref="Hl7MessageReceivedEventArgs"/> instance containing the event data.</param>
		/// <returns>IMessage.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// </exception>
		internal IMessage HandleAdmit(NHapi.Model.V231.Message.ADT_A01 message, Hl7MessageReceivedEventArgs eventArgs)
		{
			var patientRepositoryService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

			var details = new List<IResultDetail>();

			MessageUtil.Validate(message, details);

			IMessage response;

			if (message == null)
			{
				return null;
			}

			try
			{
				var patient = MessageUtil.CreatePatient(message.MSH, message.EVN, message.PID, message.PD1, details);

				if (details.Count(d => d.Type == ResultDetailType.Error) > 0)
				{
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));
				}

				var result = patientRepositoryService.Insert(patient);

				if (result?.VersionKey == null)
				{
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("DTPE001"));
				}

				response = MessageUtil.CreateNack(message, details, typeof(ACK));

				MessageUtil.UpdateMSH(new Terser(response), message);

				(response as ACK).MSH.MessageType.TriggerEvent.Value = message.MSH.MessageType.TriggerEvent.Value;
				(response as ACK).MSH.MessageType.MessageStructure.Value = "ACK";
			}
			catch (Exception e)
			{
#if DEBUG
				this.traceSource.TraceEvent(TraceEventType.Error, 0, e.StackTrace);
#endif
				this.traceSource.TraceEvent(TraceEventType.Error, 0, e.Message);

				if (!details.Exists(o => o.Message == e.Message || o.Exception == e))
				{
					details.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
				}

				response = MessageUtil.CreateNack(message, details, typeof(ACK));
			}

			return response;
		}

		/// <summary>
		/// Handle a PIX query.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="eventArgs">The <see cref="Hl7MessageReceivedEventArgs" /> instance containing the event data.</param>
		/// <returns>Returns the message result from the query.</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		internal IMessage HandlePixQuery(QBP_Q21 request, Hl7MessageReceivedEventArgs eventArgs)
		{
			var patientRepositoryService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

			var details = new List<IResultDetail>();

			MessageUtil.Validate(request, details);

			IMessage response = null;

			// Control
			if (request == null)
			{
				return null;
			}

			try
			{
				// Create Query Data
				var query = MessageUtil.CreateIDQuery(request.QPD);

				if (query == null)
				{
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));
				}

				var count = int.Parse(request?.RCP?.QuantityLimitedRequest?.Quantity?.Value ?? "0");
				var offset = 0;
				var totalCount = 0;
				var result = patientRepositoryService.Find(query, offset, count, out totalCount);

				// Now process the result
				response = MessageUtil.CreateRSPK23(result, details);

				try
				{
					(response as RSP_K23).QPD.MessageQueryName.Identifier.Value = request.QPD.MessageQueryName.Identifier.Value;

					Terser reqTerser = new Terser(request), rspTerser = new Terser(response);

					rspTerser.Set("/QPD-1", reqTerser.Get("/QPD-1"));
					rspTerser.Set("/QPD-2", reqTerser.Get("/QPD-2"));
					rspTerser.Set("/QPD-3-1", reqTerser.Get("/QPD-3-1"));
					rspTerser.Set("/QPD-3-4-1", reqTerser.Get("/QPD-3-4-1"));
					rspTerser.Set("/QPD-3-4-2", reqTerser.Get("/QPD-3-4-2"));
					rspTerser.Set("/QPD-3-4-3", reqTerser.Get("/QPD-3-4-3"));
					rspTerser.Set("/QPD-4-1", reqTerser.Get("/QPD-4-1"));
					rspTerser.Set("/QPD-4-4-1", reqTerser.Get("/QPD-4-4-1"));
					rspTerser.Set("/QPD-4-4-2", reqTerser.Get("/QPD-4-4-2"));
					rspTerser.Set("/QPD-4-4-3", reqTerser.Get("/QPD-4-4-3"));
				}
				catch (Exception e)
				{
					this.traceSource.TraceEvent(TraceEventType.Error, 0, e.ToString());
				}

				MessageUtil.UpdateMSH(new Terser(response), request);
			}
			catch (Exception e)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, 0, e.ToString());

				response = MessageUtil.CreateNack(request, details, typeof(RSP_K23));

				var errTerser = new Terser(response);

				// HACK: Fix the generic ACK with a real ACK for this message
				errTerser.Set("/MSH-9-2", "K23");
				errTerser.Set("/MSH-9-3", "RSP_K23");
				errTerser.Set("/QAK-2", "AE");
				errTerser.Set("/MSA-1", "AE");
				errTerser.Set("/QAK-1", request.QPD.QueryTag.Value);
			}

			return response;
		}

		/// <summary>
		/// Handles a PIX merge request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="evt">The <see cref="Hl7MessageReceivedEventArgs" /> instance containing the event data.</param>
		/// <returns>IMessage.</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
		internal IMessage HandleMerge(NHapi.Model.V231.Message.ADT_A39 request, Hl7MessageReceivedEventArgs evt)
		{
			// Get config
			var dataService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

			// Create a details array
			List<IResultDetail> details = new List<IResultDetail>();

			// Validate the inbound message
			MessageUtil.Validate((IMessage)request, details);

			IMessage response = null;

			// Control
			if (request == null)
			{
				return null;
			}

			try
			{
				// Create Data
				for (var i = 0; i < request.PATIENTRepetitionsUsed; i++)
				{
					var patientGroup = request.GetPATIENT(i);

					var survivor = MessageUtil.CreatePatient(request.MSH, request.EVN, patientGroup.PID, patientGroup.PD1, details);

					if (survivor == null)
					{
						throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));
					}

					var victim = dataService.Find(o => o.Identifiers.Any(id => id.Authority.DomainName == patientGroup.MRG.GetPriorAlternatePatientID(0).AssigningAuthority.NamespaceID.Value && id.Value == patientGroup.MRG.GetPriorAlternatePatientID(0).ID.Value)).FirstOrDefault();

					if (victim == null)
					{
						throw new KeyNotFoundException();
					}

					var result = dataService.Merge(survivor, victim);
				}

				response = MessageUtil.CreateNack(request, details, typeof(ACK));
				MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.TriggerEvent.Value = request.MSH.MessageType.TriggerEvent.Value;
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.MessageStructure.Value = "ACK";
			}
			catch (Exception e)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, 0, e.ToString());

				if (!details.Exists(o => o.Message == e.Message || o.Exception == e))
				{
					details.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
				}

				response = MessageUtil.CreateNack(request, details, typeof(NHapi.Model.V25.Message.ACK));
			}

			return response;
		}

		/// <summary>
		/// Handles an update.
		/// </summary>
		/// <param name="message">The ADT_A08 message.</param>
		/// <param name="eventArgs">The <see cref="Hl7MessageReceivedEventArgs" /> instance containing the event data.</param>
		/// <returns>Returns the response message from the merge event.</returns>
		internal IMessage HandlePixUpdate(NHapi.Model.V231.Message.ADT_A08 message, Hl7MessageReceivedEventArgs eventArgs)
		{
			var parser = new PipeParser();

			message.MSH.MessageType.MessageStructure.Value = "ADT_A01";

			var encodedMessage = parser.Parse(parser.Encode(message));

			if (encodedMessage is NHapi.Model.V231.Message.ADT_A01)
			{
				return this.HandlePixUpdate(encodedMessage as NHapi.Model.V231.Message.ADT_A01, eventArgs);
			}

			return MessageUtil.CreateNack(eventArgs.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
		}

		/// <summary>
		/// Handles an update.
		/// </summary>
		/// <param name="message">The request.</param>
		/// <param name="evt">The <see cref="Hl7MessageReceivedEventArgs" /> instance containing the event data.</param>
		/// <returns>Returns the response message from the merge event.</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		internal IMessage HandlePixUpdate(NHapi.Model.V231.Message.ADT_A01 message, Hl7MessageReceivedEventArgs evt)
		{
			var dataService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

			// Create a details array
			var details = new List<IResultDetail>();

			// Validate the inbound message
			MessageUtil.Validate(message, details);

			IMessage response = null;

			// Control
			if (message == null)
			{
				return null;
			}

			try
			{
				// Create Query Data
				var data = MessageUtil.CreatePatient(message.MSH, message.EVN, message.PID, message.PD1, details);
				if (data == null)
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));

				var result = dataService.Save(data);

				if (result == null || result.VersionKey == null)
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("DTPE001"));

				//audit = auditUtil.CreateAuditData("ITI-8", result.VersionId.UpdateMode == UpdateModeType.Update ? ActionType.Update : ActionType.Create, OutcomeIndicator.Success, evt, new List<VersionedDomainIdentifier>() { result.VersionId });
				// Now process the result
				response = MessageUtil.CreateNack(message, details, typeof(NHapi.Model.V25.Message.ACK));
				MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), message);
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.TriggerEvent.Value = message.MSH.MessageType.TriggerEvent.Value;
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.MessageStructure.Value = "ACK";
			}
			catch (Exception e)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, 0, e.ToString());

				if (!details.Exists(o => o.Message == e.Message || o.Exception == e))
				{
					details.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
				}

				response = MessageUtil.CreateNack(message, details, typeof(NHapi.Model.V25.Message.ACK));
			}

			return response;
		}
	}
}