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
 * Date: 2016-8-14
 */
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Auditing.Data;
using MARC.HI.EHRS.SVC.Auditing.Services;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Messaging.HAPI;
using MARC.HI.EHRS.SVC.Messaging.HAPI.TransportProtocol;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using NHapi.Model.V25.Message;
using OpenIZ.Core;
using OpenIZ.Core.Event;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Messaging.HL7
{
	/// <summary>
	/// Represents a message handler that can handle ADT messages from remote systems.
	/// </summary>
	[Description("ADT Message Handler")]
	public class AdtMessageHandler : IHL7MessageHandler
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Messaging.HL7");

		/// <summary>
		/// Handle a received message on the LLP interface.
		/// </summary>
		/// <param name="e">The HL7 message received event arguments.</param>
		public IMessage HandleMessage(Hl7MessageReceivedEventArgs e)
		{
			IMessage response = null;

			try
			{
				if (e.Message.Version == "2.5")
				{
					// Get the MSH segment
					var terser = new Terser(e.Message);
					var trigger = terser.Get("/MSH-9-2");
					Trace.TraceInformation("Message is of type {0} {1}", e.Message.GetType().FullName, trigger);

					switch (trigger)
					{
						case "Q23":
							if (e.Message is NHapi.Model.V25.Message.QBP_Q21)
								response = HandleIDQuery(e.Message as NHapi.Model.V25.Message.QBP_Q21, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						case "A01":
						case "A04":
						case "A05":
							if (e.Message is NHapi.Model.V25.Message.ADT_A01)
								response = HandleAdmit(e.Message as NHapi.Model.V25.Message.ADT_A01, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						case "A08":
							if (e.Message is NHapi.Model.V25.Message.ADT_A01)
								response = HandleUpdate(e.Message as NHapi.Model.V25.Message.ADT_A01, e);
							//else if (e.Message is NHapi.Model.V25.Message.ADT_A08)
							//    response = HandleUpdate(e.Message as NHapi.Model.V25.Message.ADT_A08, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						case "A40":
							if (e.Message is NHapi.Model.V25.Message.ADT_A39)
								response = HandleMerge(e.Message as NHapi.Model.V25.Message.ADT_A39, e);
							//else if (e.Message is NHapi.Model.V25.Message.ADT_A40)
							//    response = HandleMerge(e.Message as NHapi.Model.V25.Message.ADT_A40, e);
							else
								response = MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
							break;

						default:
							response = MessageUtil.CreateNack(e.Message, "AR", "201", ApplicationContext.Current.GetLocaleString("HL7201"));
							Trace.TraceError("{0} is not a supported trigger", trigger);
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
				this.tracer.TraceEvent(TraceEventType.Error, ex.HResult, "Error processing message {0} : {1}", e.Message?.Version, ex);
				response = MessageUtil.CreateNack(e.Message, "AR", "207", ex.Message);
			}

			return response;
		}

		//private IMessage HandleMerge(NHapi.Model.V25.Message.ADT_A40 aDT_A40, Hl7MessageReceivedEventArgs e)
		//{
		//    PipeParser parser = new PipeParser();
		//    aDT_A40.MSH.MessageType.MessageStructure.Value = "ADT_A39";
		//    var message = parser.Parse(parser.Encode(aDT_A40));
		//    if (message is NHapi.Model.V25.Message.ADT_A39)
		//        return this.HandleMerge(message as NHapi.Model.V25.Message.ADT_A39, e);
		//    else
		//        return MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
		//}

		/// <summary>
		/// Handles a patient registration event.
		/// </summary>
		/// <param name="request">The registration event message.</param>
		/// <param name="evt">The event arguments.</param>
		/// <returns>Returns the response message.</returns>
		internal IMessage HandleAdmit(NHapi.Model.V25.Message.ADT_A01 request, Hl7MessageReceivedEventArgs evt)
		{
			var clientRegistryNotificationService = ApplicationContext.Current.GetService<IClientRegistryNotificationService>();
			var patientRepositoryService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

			List<IResultDetail> details = new List<IResultDetail>();

			MessageUtil.Validate((IMessage)request, details);

			IMessage response = null;

			if (request == null)
			{
				return null;
			}

			AuditData audit = null;

			try
			{
				var patient = MessageUtil.CreatePatient(request.MSH, request.EVN, request.PID, request.PD1, details);

				if (details.Count(d => d.Type == ResultDetailType.Error) > 0)
				{
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));
				}

				var result = patientRepositoryService.Insert(patient);

				if (result == null || result.VersionKey == null)
				{
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("DTPE001"));
				}

				response = MessageUtil.CreateNack(request, details, typeof(ACK));

				MessageUtil.UpdateMSH(new Terser(response), request);

				(response as ACK).MSH.MessageType.TriggerEvent.Value = request.MSH.MessageType.TriggerEvent.Value;
				(response as ACK).MSH.MessageType.MessageStructure.Value = "ACK";

				 var eventArgs = new NotificationEventArgs<Patient>(patient, NotificationType.Create);

				clientRegistryNotificationService?.NotifyRegister(eventArgs);
			}
			catch (Exception e)
			{
#if DEBUG
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.StackTrace);
#endif
				this.tracer.TraceEvent(TraceEventType.Error, 0, e.Message);

				if (!details.Exists(o => o.Message == e.Message || o.Exception == e))
				{
					details.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
				}

				response = MessageUtil.CreateNack(request, details, typeof(ACK));
			}
			finally
			{
				IAuditorService auditService = ApplicationContext.Current.GetService<IAuditorService>();
				auditService?.SendAudit(audit);
			}

			return response;
		}

		/// <summary>
		/// Handle a PIX query
		/// </summary>
		internal IMessage HandleIDQuery(QBP_Q21 request, Hl7MessageReceivedEventArgs evt)
		{
			var patientRepositoryService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

			List<IResultDetail> details = new List<IResultDetail>();

			MessageUtil.Validate((IMessage)request, details);

			IMessage response = null;

			// Control
			if (request == null)
				return null;

			// Construct appropriate audit
			AuditData audit = null;

			// Data controller
			//AuditUtil auditUtil = new AuditUtil() { Context = this.Context };
			//DataUtil dataUtil = new DataUtil() { Context = this.Context };

			try
			{
				// Create Query Data
				Expression<Func<Patient, bool>> query = MessageUtil.CreateIDQuery(request.QPD);

				if (query == null)
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));

				int count = Int32.Parse(request?.RCP?.QuantityLimitedRequest?.Quantity?.Value ?? "0");
				int offset = 0;
				int totalCount = 0;
				var result = patientRepositoryService.Find(query, offset, count, out totalCount);

				// Update locations?
				/*
                foreach (var dtl in dtls)
                    if (dtl is PatientNotFoundResultDetail)
                        dtl.Location = "QPD^1^3^1^1";
                    else if (dtl is UnrecognizedPatientDomainResultDetail)
                        dtl.Location = "QPD^1^3^1^4";
                    else if (dtl is UnrecognizedTargetDomainResultDetail)
                        dtl.Location = "QPD^1^4^";

                audit = auditUtil.CreateAuditData("ITI-9", ActionType.Execute, OutcomeIndicator.Success, evt, result);
                */

				// Now process the result
				response = MessageUtil.CreateRSPK23(result, details);
				//var r = dcu.CreateRSP_K23(null, null);
				// Copy QPD
				try
				{
					(response as RSP_K23).QPD.MessageQueryName.Identifier.Value = request.QPD.MessageQueryName.Identifier.Value;
					Terser reqTerser = new Terser(request),
						rspTerser = new Terser(response);
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
					Trace.TraceError(e.ToString());
				}
				//MessageUtil.((response as NHapi.Model.V25.Message.RSP_K23).QPD, request.QPD);

				MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				response = MessageUtil.CreateNack(request, details, typeof(RSP_K23));
				Terser errTerser = new Terser(response);
				// HACK: Fix the generic ACK with a real ACK for this message
				errTerser.Set("/MSH-9-2", "K23");
				errTerser.Set("/MSH-9-3", "RSP_K23");
				errTerser.Set("/QAK-2", "AE");
				errTerser.Set("/MSA-1", "AE");
				errTerser.Set("/QAK-1", request.QPD.QueryTag.Value);
				//audit = auditUtil.CreateAuditData("ITI-9", ActionType.Execute, OutcomeIndicator.EpicFail, evt, new List<VersionedDomainIdentifier>());
			}
			finally
			{
				IAuditorService auditSvc = ApplicationContext.Current.GetService<IAuditorService>();
				if (auditSvc != null)
					auditSvc.SendAudit(audit);
			}

			return response;
		}

		/// <summary>
		/// Handle the PIX merge request
		/// </summary>
		internal IMessage HandleMerge(NHapi.Model.V25.Message.ADT_A39 request, Hl7MessageReceivedEventArgs evt)
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
				return null;

			// Data controller
			//DataUtil dataUtil = new DataUtil() { Context = this.Context };
			//AuditUtil auditUtil = new AuditUtil() { Context = this.Context };

			// Construct appropriate audit
			List<AuditData> audit = new List<AuditData>();
			try
			{
				// Create Data
				for (var i = 0; i < request.PATIENTRepetitionsUsed; i++)
				{
					var patientGroup = request.GetPATIENT(i);
					var survivor = MessageUtil.CreatePatient(request.MSH, request.EVN, patientGroup.PID, patientGroup.PD1, details);
					if (survivor == null)
						throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));
					var victim = dataService.Find(o => o.Identifiers.Any(id => id.Authority.DomainName == patientGroup.MRG.GetPriorAlternatePatientID(0).AssigningAuthority.NamespaceID.Value && id.Value == patientGroup.MRG.GetPriorAlternatePatientID(0).IDNumber.Value)).FirstOrDefault();
					if (victim == null)
						throw new KeyNotFoundException();

					// Merge
					var result = dataService.Merge(survivor, victim);
				}

				// Now audit
				//audit.Add(auditUtil.CreateAuditData("ITI-8", ActionType.Delete, OutcomeIndicator.Success, evt, deletedRecordIds));
				//audit.Add(auditUtil.CreateAuditData("ITI-8", ActionType.Update, OutcomeIndicator.Success, evt, updatedRecordIds));
				// Now process the result
				response = MessageUtil.CreateNack(request, details, typeof(NHapi.Model.V25.Message.ACK));
				MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.TriggerEvent.Value = request.MSH.MessageType.TriggerEvent.Value;
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.MessageStructure.Value = "ACK";
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				if (!details.Exists(o => o.Message == e.Message || o.Exception == e))
					details.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
				response = MessageUtil.CreateNack(request, details, typeof(NHapi.Model.V25.Message.ACK));
				//audit.Add(auditUtil.CreateAuditData("ITI-8", ActionType.Delete, OutcomeIndicator.EpicFail, evt, new List<VersionedDomainIdentifier>()));
			}
			finally
			{
				IAuditorService auditSvc = ApplicationContext.Current.GetService<IAuditorService>();
				if (auditSvc != null)
					foreach (var aud in audit)
						auditSvc.SendAudit(aud);
			}
			return response;
		}

		/// <summary>
		/// Handle PIX Update
		/// </summary>
		internal IMessage HandleUpdate(NHapi.Model.V25.Message.ADT_A09 aDT_A08, Hl7MessageReceivedEventArgs e)
		{
			PipeParser parser = new PipeParser();
			aDT_A08.MSH.MessageType.MessageStructure.Value = "ADT_A01";
			var message = parser.Parse(parser.Encode(aDT_A08));
			if (message is NHapi.Model.V25.Message.ADT_A01)
				return this.HandleUpdate(message as NHapi.Model.V25.Message.ADT_A01, e);
			else
				return MessageUtil.CreateNack(e.Message, "AR", "200", ApplicationContext.Current.GetLocaleString("MSGE074"));
		}

		/// <summary>
		/// Handle PIX update
		/// </summary>
		internal IMessage HandleUpdate(NHapi.Model.V25.Message.ADT_A01 request, Hl7MessageReceivedEventArgs evt)
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
				return null;

			// Data controller
			//DataUtil dataUtil = new DataUtil() { Context = this.Context };
			//AuditUtil auditUtil = new AuditUtil() { Context = this.Context };

			// Construct appropriate audit
			AuditData audit = null;
			try
			{
				// Create Query Data
				var data = MessageUtil.CreatePatient(request.MSH, request.EVN, request.PID, request.PD1, details);
				if (data == null)
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("MSGE00A"));

				var result = dataService.Save(data);

				if (result == null || result.VersionKey == null)
					throw new InvalidOperationException(ApplicationContext.Current.GetLocaleString("DTPE001"));

				//audit = auditUtil.CreateAuditData("ITI-8", result.VersionId.UpdateMode == UpdateModeType.Update ? ActionType.Update : ActionType.Create, OutcomeIndicator.Success, evt, new List<VersionedDomainIdentifier>() { result.VersionId });
				// Now process the result
				response = MessageUtil.CreateNack(request, details, typeof(NHapi.Model.V25.Message.ACK));
				MessageUtil.UpdateMSH(new NHapi.Base.Util.Terser(response), request);
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.TriggerEvent.Value = request.MSH.MessageType.TriggerEvent.Value;
				(response as NHapi.Model.V25.Message.ACK).MSH.MessageType.MessageStructure.Value = "ACK";
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				if (!details.Exists(o => o.Message == e.Message || o.Exception == e))
					details.Add(new ResultDetail(ResultDetailType.Error, e.Message, e));
				response = MessageUtil.CreateNack(request, details, typeof(NHapi.Model.V25.Message.ACK));
				//audit = auditUtil.CreateAuditData("ITI-8", ActionType.Create, OutcomeIndicator.EpicFail, evt, new List<VersionedDomainIdentifier>());
			}
			finally
			{
				IAuditorService auditSvc = ApplicationContext.Current.GetService<IAuditorService>();

				auditSvc?.SendAudit(audit);
			}

			return response;
		}
	}
}