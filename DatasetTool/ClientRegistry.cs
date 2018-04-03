using MARC.Everest.Threading;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using NHapi.Base.Util;
using NHapi.Model.V25.Message;
using NHapi.Model.V25.Segment;
using OpenIZ.Core;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using MohawkCollege.Util.Console.Parameters;
using System.ComponentModel;
using System.Collections.Specialized;
using OpenIZ.Messaging.HL7;
using OpenIZ.Messaging.HL7.Notifier;
using OpenIZ.Core.Services;
using OpenIZ.Caching.Memory;

namespace OizDevTool
{
    /// <summary>
    /// Client registry
    /// </summary>
    [Description("MPI Tooling")]
    public static class ClientRegistry
    {

        /// <summary>
        /// Client registry parameters
        /// </summary>
        public class ClientRegistryParms
        {

            /// <summary>
            /// Gets or sets the MPI endpoint
            /// </summary>
            [Parameter("mpi")]
            [Description("Sets the MPI endpoint")]
            public String MpiEndpoint { get; set; }

            /// <summary>
            /// Gets or sets the mpi endpoint
            /// </summary>
            [Parameter("auth")]
            [Description("Identifies the authorities to search the MPI for duplication")]
            public StringCollection Authorities { get; set; }

            [Parameter("target")]
            [Description("Identifies the target MSG-3|4 (example: CR|DEPL)")]
            public string TargetDeviceId { get; internal set; }
        }

        /// <summary>
        /// Pushes all patients from the OpenIZ instance to an MPI using V2 messages
        /// </summary>
        public static void PushAll(string[] args)
        {
            ApplicationContext.Current.Start();
            ApplicationServiceContext.Current = ApplicationContext.Current;
            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
            EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());
            var patientService = ApplicationContext.Current.GetService<IStoredQueryDataPersistenceService<Patient>>();

            var parms = new ParameterParser<ClientRegistryParms>().Parse(args);

            // Step 1: Grab the LLP address
            var wtp = new WaitThreadPool();
            
            // Now we want to load all patients
            Guid qry = Guid.NewGuid();
            int ofs = 0, tr = 1, complete = 0, skip = 0, nobar = 0, erc = 0;
            while(ofs < tr)
            {
                var results = patientService.Query(o => o.StatusConceptKey == StatusKeys.Active, ofs, 100, AuthenticationContext.Current.Principal, out tr);
                ofs += 100;
                if(ofs % 1000 == 0)
                    ApplicationContext.Current.GetService<IDataCachingService>().Clear();

                foreach(var res in results)
                    wtp.QueueUserWorkItem(o => {
                        try
                        {
                            
                            // Lookup by pkey to save even loading from db
                            var pdqRequest = CreatePDQSearch(
                                new KeyValuePair<string, string>("@PID.3.1", o.ToString())
                            );
                            var messageSender = new MllpMessageSender(new Uri(parms.MpiEndpoint), null, null);
                            var pdqResponse = messageSender.SendAndReceive(pdqRequest) as RSP_K21;
                            if (pdqResponse.QUERY_RESPONSERepetitionsUsed > 0)
                            {
                                Interlocked.Increment(ref skip);
                                return;
                            }

                            var patient = patientService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>((Guid)o), AuthenticationContext.SystemPrincipal, true);
                            var searchIdentifier = patient.LoadCollection<EntityIdentifier>("Identifiers").FirstOrDefault(i => parms.Authorities?.Contains(i.Authority.DomainName) == true || parms.Authorities == null);
                            Console.CursorLeft = 0;

                            if (searchIdentifier == null)
                            {
                                Interlocked.Increment(ref nobar);
                            }
                            else
                            {
                                // Phase 1: Make sure the patient doesn't already exist on the MPI
                                pdqRequest = CreatePDQSearch(
                                    new KeyValuePair<string, string>("@PID.3.1", searchIdentifier.Value),
                                    new KeyValuePair<string, string>("@PID.3.4.1", searchIdentifier.Authority.DomainName)
                                );

                                // Process PDQ response
                                pdqResponse = messageSender.SendAndReceive(pdqRequest) as RSP_K21;
                                if (pdqResponse == null || pdqResponse.MSA.AcknowledgmentCode.Value != "AA")
                                {
                                    foreach (var err in pdqResponse.ERR.GetErrorCodeAndLocation())
                                        Console.WriteLine("MPI ERR: {0} ({1})", err.CodeIdentifyingError.Text, err.CodeIdentifyingError.AlternateText);
                                    Interlocked.Increment(ref erc);
                                    return;
                                }

                                // Were there any results?
                                if (pdqResponse.QUERY_RESPONSERepetitionsUsed > 0)
                                    Interlocked.Increment(ref skip);
                                else
                                {
                                    // Notify of registration
                                    var patientIdentitySrc = new PAT_IDENTITY_SRC()
                                    {
                                        TargetConfiguration = new OpenIZ.Messaging.HL7.Configuration.TargetConfiguration(
                                            "oizdt-target",
                                            parms.MpiEndpoint,
                                            "PAT_IDENTITY_SRC",
                                            parms.TargetDeviceId
                                            )
                                    };
                                    var pixRequest = patientIdentitySrc.CreateMessage<Patient>(new NotificationQueueWorkItem<Patient>(patient, ActionType.Create));

                                    var pixResponse = messageSender.SendAndReceive(pixRequest) as NHapi.Model.V231.Message.ACK;
                                    // Process PDQ response
                                    if (pixResponse == null || !pixResponse.MSA.AcknowledgementCode.Value.EndsWith("A"))
                                    {
                                        foreach (var err in pdqResponse.ERR.GetErrorCodeAndLocation())
                                            Console.WriteLine("MPI ERR: {0} ({1})", err.CodeIdentifyingError.Text, err.CodeIdentifyingError.AlternateText);
                                        Interlocked.Increment(ref erc);
                                    }
                                    Interlocked.Increment(ref complete);

                                }
                            }

                        }
                        catch(Exception e)
                        {
                            Interlocked.Increment(ref erc);
                            Console.WriteLine(e.Message);
                        }

                        lock (parms)
                        {
                            Console.CursorLeft = 0;
                            Console.Write("    Pushing patients to MPI ([SK: {0}, UL: {1}, NI: {2}, ER: {3}]/{4}) {5:0%}    ", skip, complete, nobar, erc, tr, (float)(skip + complete + nobar + erc) / (float)tr);
                            if (complete > 0 && (complete % 1000) <= 1)
                            {
                                MemoryCache.Current.Clear();
                                System.GC.Collect();
                            }
                        }

                    }, res.Key);
            }

            wtp.WaitOne();
        }

        /// <summary>
        /// Create a PDQ search message
        /// </summary>
        /// <param name="filters">The parameters for query</param>
        private static QBP_Q21 CreatePDQSearch(params KeyValuePair<String, String>[] filters)
        {
            // Search - Construct a v2 message this is found in IHE ITI TF-2:3.21
            QBP_Q21 message = new QBP_Q21();

            UpdateMSH(message.MSH, "QBP_Q21", "QBP", "Q22");
            //message.MSH.VersionID.VersionID.Value = "2.3.1";

            // Message query
            message.QPD.MessageQueryName.Identifier.Value = "Patient Demographics Query";

            // Sometimes it is easier to use a terser
            Terser terser = new Terser(message);
            terser.Set("/QPD-2", Guid.NewGuid().ToString()); // Tag of the query
            terser.Set("/QPD-1", "Patient Demographics Query"); // Name of the query
            for (int i = 0; i < filters.Length; i++)
            {
                terser.Set(String.Format("/QPD-3({0})-1", i), filters[i].Key);
                terser.Set(String.Format("/QPD-3({0})-2", i), filters[i].Value);
            }

            return message;
        }
        
        /// <summary>
        /// Update the MSH header
        /// </summary>
        private static void UpdateMSH(MSH header, String message, String structure, String trigger)
        {
            // Message header
            header.AcceptAcknowledgmentType.Value = "AL"; // Always send response
            header.DateTimeOfMessage.Time.Value = DateTime.Now.ToString("yyyyMMddHHmmss"); // Date/time of creation of message
            header.MessageControlID.Value = Guid.NewGuid().ToString(); // Unique id for message
            header.MessageType.MessageStructure.Value = message; // Message structure type (Query By Parameter Type 21)
            header.MessageType.MessageCode.Value = structure; // Message Structure Code (Query By Parameter)
            header.MessageType.TriggerEvent.Value = trigger; // Trigger event (Event Query 22)
            header.ProcessingID.ProcessingID.Value = "P"; // Production
            header.ReceivingApplication.NamespaceID.Value = "CR"; // Client Registry
            header.ReceivingFacility.NamespaceID.Value = "MCAAT"; // SAMPLE
            header.SendingApplication.NamespaceID.Value = "OPENIZ"; // What goes here?
            header.SendingFacility.NamespaceID.Value = "MCAAT";
        }

    }
}
