/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.Everest.Threading;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core;
using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Extensions;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using System.Collections.Specialized;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Query;
using OpenIZ.Caching.Memory;

namespace OizDevTool
{
    /// <summary>
    /// Represents functions related to warehousing
    /// </summary>
    [Description("Warehouse related functions")]
    public static class Warehouse
    {

        /// <summary>
        /// Careplan parameters
        /// </summary>
        public class CareplanParameters
        {
            [Parameter("trunc")]
            [Description("Truncates (deletes) all calculated plans")]
            public bool Truncate { get; set; }

            [Parameter("create")]
            [Description("Create the warehouse tables if they don't exist")]
            public bool Create { get; set; }

            [Parameter("since")]
            [Description("Create plan for all those records modified since")]
            public string Since { get; set; }

            [Parameter("nofulfill")]
            [Description("Does not scan for fulfillments from acts")]
            public bool NoFulfill { get; set; }

            [Parameter("fulfill")]
            [Description("Calculate fulfillment for the specified type of act")]
            public StringCollection ActTypes { get; set; }

            [Parameter("patient")]
            [Description("Calculate care plan for specified patients")]
            public StringCollection PatientId { get; set; }

            [Parameter("facility")]
            [Description("Calculate care plan for the specified facility")]
            public String FacilityId { get; set; }

            [Parameter("limit")]
            [Description("Limit number of processing items")]
            public String Limit { get; set; }


            [Parameter("skip")]
            [Description("Skip number of processing items")]
            public String Skip { get; set; }
        }

        /// <summary>
        /// AMC parameters
        /// </summary>
        public class AmcParameters
        {
            [Parameter("name")]
            [Description("Name of the facility to calculate")]
            public string Name { get; set; }

        }

        /// <summary>
        /// Calculates the AMC for all facilities in the system
        /// </summary>
        public static int Amc(string[] args)
        {
            var parms = new ParameterParser<AmcParameters>().Parse(args);
            ApplicationContext.Current.Start();
            ApplicationServiceContext.Current = ApplicationContext.Current;
            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
            EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());

            // Get the place service
            var placeService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            var apService = ApplicationContext.Current.GetService<IDataPersistenceService<ActParticipation>>();
            var matlService = ApplicationContext.Current.GetService<IDataPersistenceService<Material>>() as IFastQueryDataPersistenceService<Material>;
            var erService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>() as IFastQueryDataPersistenceService<EntityRelationship>;
            DateTime startDate = DateTime.Now.AddMonths(-3);

            int tr = 1, ofs = 0;

            while (ofs < tr)
            {
                IEnumerable<Place> places = null;
                if (String.IsNullOrEmpty(parms.Name))
                    places = placeService.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, ofs, 100, AuthenticationContext.Current.Principal, out tr);
                else
                    places = placeService.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation && o.Names.Any(n => n.Component.Any(c => c.Value.Contains(parms.Name))), ofs, 100, AuthenticationContext.Current.Principal, out tr);

                foreach (var plc in places)
                {
                    Console.WriteLine("Calculating AMC for {0}", plc.Names.FirstOrDefault().ToDisplay());

                    // First we want to get all entity relationships of type consumable related to this place
                    var consumablePtcpts = apService.Query(o => o.ParticipationRoleKey == ActParticipationKey.Consumable && o.SourceEntity.ActTime > startDate && o.SourceEntity.Participations.Any(p => p.ParticipationRoleKey == ActParticipationKey.Location && p.PlayerEntityKey == plc.Key), AuthenticationContext.Current.Principal);

                    // Now we want to group by consumable
                    int t = 0;

                    var groupedConsumables = consumablePtcpts.GroupBy(o => o.PlayerEntityKey).Select(o => new
                    {
                        ManufacturedMaterialKey = o.Key,
                        UsedQty = o.Sum(s => s.Quantity),
                        MaterialKey = matlService.QueryFast(m => m.Relationships.Any(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance && r.TargetEntityKey == o.Key), Guid.Empty, 0, 1, AuthenticationContext.Current.Principal, out t)?.FirstOrDefault()?.Key
                    }).ToList();

                    foreach (var i in groupedConsumables.Where(o => !o.MaterialKey.HasValue))
                        Console.WriteLine("MMAT {0} is not linked to any MAT", i.ManufacturedMaterialKey);

                    groupedConsumables.RemoveAll(o => !o.MaterialKey.HasValue);
                    // Now, we want to build the stock policy object
                    dynamic[] stockPolicyObject = new dynamic[0];
                    var stockPolicyExtension = plc.LoadCollection<EntityExtension>("Extensions").FirstOrDefault(o => o.ExtensionTypeKey == Guid.Parse("DFCA3C81-A3C4-4C82-A901-8BC576DA307C"));
                    if (stockPolicyExtension == null)
                    {
                        stockPolicyExtension = new EntityExtension()
                        {
                            ExtensionType = new ExtensionType("http://openiz.org/extensions/contrib/bid/stockPolicy", typeof(DictionaryExtensionHandler))
                            {
                                Key = Guid.Parse("DFCA3C81-A3C4-4C82-A901-8BC576DA307C")
                            },
                            ExtensionValue = stockPolicyObject
                        };
                        plc.Extensions.Add(stockPolicyExtension);
                    }
                    else
                        stockPolicyObject = (stockPolicyExtension.GetValue() as dynamic[]).GroupBy(o => o["MaterialEntityId"]).Select(o => new
                        {
                            MaterialEntityId = Guid.Parse(o.FirstOrDefault()["MaterialEntityId"].ToString()),
                            ReorderQuantity = (int?)(o.FirstOrDefault()["ReorderQuantity"]) ?? 0,
                            SafetyQuantity = (int?)(o.FirstOrDefault()["SafetyQuantity"]) ?? 0,
                            AMC = (int?)(o.FirstOrDefault()["AMC"]) ?? 0,
                            Multiplier = (int?)(o.FirstOrDefault()["Multiplier"]) ?? 0
                        }).ToArray();



                    // Now we want to calculate each amc
                    List<dynamic> calculatedStockPolicyObject = new List<dynamic>();
                    bool hasChanged = false;
                    foreach (var gkp in groupedConsumables.GroupBy(o => o.MaterialKey).Select(o => new { Key = o.Key, Value = o.Sum(p => p.UsedQty) }))
                    {

                        var amc = (int)((float)Math.Abs(gkp.Value ?? 0) / 3);
                        // Now correct for packaging
                        var pkging = erService.Query(o => o.SourceEntityKey == gkp.Key && o.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance, AuthenticationContext.Current.Principal).Max(o => o.Quantity);
                        if (pkging > 1)
                            amc = ((amc / pkging.Value) + 1) * pkging.Value;

                        // Is there an existing stock policy object?
                        var existingPolicy = stockPolicyObject.FirstOrDefault(o => o.MaterialEntityId == gkp.Key);
                        hasChanged |= amc != existingPolicy?.AMC;
                        if (existingPolicy != null && amc != existingPolicy?.AMC)
                            existingPolicy = new
                            {
                                MaterialEntityId = gkp.Key,
                                ReorderQuantity = existingPolicy.ReorderQuantity,
                                SafetyQuantity = existingPolicy.SafetyQuantity,
                                AMC = amc,
                                Multiplier = existingPolicy.Multiplier
                            };
                        else
                            existingPolicy = new
                            {
                                MaterialEntityId = gkp.Key,
                                ReorderQuantity = amc,
                                SafetyQuantity = (int)(amc * 0.33),
                                AMC = amc,
                                Multiplier = 1
                            };

                        // add policy
                        calculatedStockPolicyObject.Add(existingPolicy);
                    }

                    stockPolicyExtension.ExtensionValue = calculatedStockPolicyObject.ToArray();

                    if (hasChanged)
                        placeService.Update(plc, AuthenticationContext.Current.Principal, TransactionMode.Commit);
                }
                ofs += 100;
            }


            return 1;
        }

        /// <summary>
        /// Re-sets the original dates on all acts that don't have them
        /// </summary>
        [Description("Add the originalDate tag to acts which don't have them")]
        public static int AddOriginalDateTag(string[] argv)
        {
            ApplicationContext.Current.Start();
            ApplicationServiceContext.Current = ApplicationContext.Current;
            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
            EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());
            var warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
            var planService = ApplicationContext.Current.GetService<ICarePlanService>();
            var actPersistence = ApplicationContext.Current.GetService<IStoredQueryDataPersistenceService<SubstanceAdministration>>();
            var tagService = ApplicationContext.Current.GetService<ITagPersistenceService>();

            var oizcpDm = warehouseService.GetDatamart("oizcp");
            if (oizcpDm == null) {
                Console.WriteLine("OIZCP datamart does not exist!");
                return -1;
            }

            WaitThreadPool wtp = new WaitThreadPool();

            Guid queryId = Guid.NewGuid();
            int tr = 0, ofs = 0;
            var acts = actPersistence.Query(o => !o.Tags.Any(t => t.TagKey == "originalDate"), queryId, 0, 100, AuthenticationContext.SystemPrincipal, out tr);
            while(ofs < tr)
            {
                foreach(var itm in acts)
                {
                    wtp.QueueUserWorkItem((o) =>
                    {
                        var act = o as Act;

                        Console.WriteLine("Set originalDate for {0}", act.Key);

                        AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

                        var actProtocol = act.LoadCollection<ActProtocol>("Protocols").FirstOrDefault();
                        if (actProtocol != null) {
                            // Get the original date
                            var warehouseObj = warehouseService.AdhocQuery(oizcpDm.Id, new { protocol_id = actProtocol.ProtocolKey, sequence_id = actProtocol.Sequence });
                            if (warehouseObj.Any())
                            {
                                DateTime originalDate = warehouseObj.FirstOrDefault().act_date;
                                var originalEpochTime = (originalDate.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                                tagService.Save(act.Key.Value, new ActTag("originalDate", originalEpochTime.ToString()));
                            }
                        }
                    }, itm);
                }
                ofs += 100;
                acts = actPersistence.Query(o => !o.Tags.Any(t => t.TagKey == "originalDate"), queryId, ofs, 100, AuthenticationContext.SystemPrincipal, out tr);

            }

            wtp.WaitOne();
            return 0;
        }

        /// <summary>
        /// Generate a care plan
        /// </summary>
        [Description("Re-generates the data warehouse for all patients")]
        [ParameterClass(typeof(CareplanParameters))]
        public static int Careplan(string[] argv)
        {
            var parms = new ParameterParser<CareplanParameters>().Parse(argv);
            ApplicationContext.Current.Start();
            ApplicationServiceContext.Current = ApplicationContext.Current;
            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);
            EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());
            var warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
            var planService = ApplicationContext.Current.GetService<ICarePlanService>();

            if (warehouseService == null)
                throw new InvalidOperationException("Ad-hoc data warehouse service is not registered");
            if (planService == null)
                throw new InvalidOperationException("Missing care plan service");

            // Warehouse service
            foreach (var cp in planService.Protocols)
                Console.WriteLine("Loaded {0}...", cp.Name);

            // Deploy schema?
            var dataMart = warehouseService.GetDatamart("oizcp");
            if (dataMart == null)
            {
                if (parms.Create)
                {
                    Console.WriteLine("Datamart for care plan service doesn't exist, will have to create it...");
                    dataMart = warehouseService.CreateDatamart("oizcp", DatamartSchema.Load(typeof(Warehouse).Assembly.GetManifestResourceStream("OizDevTool.Resources.CarePlanWarehouseSchema.xml")));
                }
                else
                    throw new InvalidOperationException("Warehouse schema does not exist!");
            }

            // Truncate?
            if (parms.Truncate)
                warehouseService.Truncate(dataMart.Id);

            // Now we want to calculate
            var patientPersistence = ApplicationContext.Current.GetService<IStoredQueryDataPersistenceService<Patient>>();
            var lastRefresh = DateTime.Parse(parms.Since ?? "0001-01-01");

            // Should we calculate?
            int tr = 1, ofs = 0, calc = 0, tq = 0;
            var warehousePatients = warehouseService.StoredQuery(dataMart.Id, "consistency", new { }, out tq);
            tq = 0;
            Guid queryId = Guid.NewGuid();
            WaitThreadPool wtp = new WaitThreadPool(Environment.ProcessorCount);
            DateTime start = DateTime.Now;

            // Type filters
            List<Guid> typeFilter = new List<Guid>();
            if (parms.ActTypes?.Count > 0)
            {
                var cpcr = ApplicationContext.Current.GetService<IConceptRepositoryService>();
                foreach (var itm in parms.ActTypes)
                    typeFilter.Add(cpcr.GetConcept(itm).Key.Value);
            }

            if (!String.IsNullOrEmpty(parms.FacilityId))
            {
                warehouseService.Delete(dataMart.Id, new { location_id = parms.FacilityId });
                warehousePatients = new List<dynamic>();
            }

            int limit = Int32.MaxValue;
            if (!String.IsNullOrEmpty(parms.Limit))
                limit = Int32.Parse(parms.Limit);
            if (!String.IsNullOrEmpty(parms.Skip))
            {
                ofs += Int32.Parse(parms.Skip);
                tr = ofs + 1;
            }

            while (ofs < tr )
            {
                // Let the pressure die down
                if (tq - calc > 3000 || ofs % 5000 == 0)
                {
                    wtp.WaitOne();
                    MemoryCache.Current.Clear();
                    System.GC.Collect();
                }

                IEnumerable<Patient> prodPatients = null;
                if (!String.IsNullOrEmpty(parms.FacilityId))
                {
                    Guid facId = Guid.Parse(parms.FacilityId);
                    prodPatients = patientPersistence.Query(o => o.Relationships.Where(g => g.RelationshipType.Mnemonic == "DedicatedServiceDeliveryLocation").Any(r => r.TargetEntityKey == facId), queryId, ofs, 1000, AuthenticationContext.SystemPrincipal, out tr);
                }
                else if (parms.PatientId?.Count > 0)
                {
                    prodPatients = parms.PatientId.OfType<String>().Select(o => patientPersistence.Get(new Identifier<Guid>(Guid.Parse(o)), AuthenticationContext.SystemPrincipal, false));
                    ofs = parms.PatientId.Count;
                    tr = parms.PatientId.Count;
                }
                else
                {
                    // New patients directly modified
                    prodPatients = patientPersistence.Query(o => o.StatusConcept.Mnemonic != "OBSOLETE" && o.ModifiedOn > lastRefresh, queryId, ofs, 1000, AuthenticationContext.SystemPrincipal, out tr);
                    // Patients who have had 
                    prodPatients = prodPatients.Union(
                        patientPersistence.Query(o => o.StatusConcept.Mnemonic != "OBSOLETE" && o.Participations.Any(p=>p.ModifiedOn > lastRefresh), queryId, ofs, 1000, AuthenticationContext.SystemPrincipal, out tr)
                    );
                }



                if (lastRefresh == DateTime.MinValue)
                {
                    var sk = prodPatients.Count();
                    prodPatients = prodPatients.Where(o => !warehousePatients.Any(m => m.patient_id == o.Key));
                    sk = sk - prodPatients.Count();
                    tq += sk;
                    calc += sk;
                }
                ofs += 1000;

                foreach (var p in prodPatients.Distinct(new IdentifiedData.EqualityComparer<Patient>()))
                {
                    if (tq++ > limit)
                    {
                        ofs = tr + 1;
                        ApplicationContext.Current.GetService<MemoryQueryPersistenceService>()?.Clear();
                        break;
                    }

                    wtp.QueueUserWorkItem(state =>
                    {

                        AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

                        Patient pState = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>().Get(new Identifier<Guid>((Guid)state), AuthenticationContext.SystemPrincipal, true);

                        List<dynamic> warehousePlan = new List<dynamic>();

                        Interlocked.Increment(ref calc);
                        lock (parms)
                        {
                            var ips = (((double)(DateTime.Now - start).Ticks / calc) * (tq - calc));
                            var tps = (((double)(DateTime.Now - start).Ticks / calc) * (tr - calc));
                            Console.CursorLeft = 0;
                            Console.Write("    Calculating care plan {0}/{1} <<Scan: {4} ({5:0%})>> ({2:0%}) [ETA: {3} .. {7}] {6:0.##} R/S ", calc, tq, (float)calc / tq, new TimeSpan((long)ips).ToString("d'd 'h'h 'm'm 's's'"), ofs, (float)ofs / tr, ((double)calc / (double)(DateTime.Now - start).TotalSeconds), new TimeSpan((long)tps).ToString("d'd 'h'h 'm'm 's's'"));
                        }

                        var data = pState; //  ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>().Get(p.Key.Value);
                        //p.par
                        // First, we want a copy of the warehouse
                        warehouseService.Delete(dataMart.Id, new
                        {
                            patient_id = data.Key.Value
                        });
                        //warehouseService.Delete(dataMart.Id, new { patient_id = data.Key.Value });
                        var careplanService = ApplicationContext.Current.GetService<ICarePlanService>();

                        // Now calculate the care plan... 
                        var carePlan = careplanService.CreateCarePlan(data, false, new Dictionary<String, Object>() { { "isBackground", true } });
                        warehousePlan.AddRange(carePlan.Action.Select(o => new
                        {
                            creation_date = DateTime.Now,
                            patient_id = data.Key.Value,
                            location_id = data.Relationships.FirstOrDefault(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation || r.RelationshipType?.Mnemonic == "DedicatedServiceDeliveryLocation")?.TargetEntityKey.Value,
                            act_id = o.Key.Value,
                            class_id = o.ClassConceptKey.Value,
                            type_id = o.TypeConceptKey.Value,
                            protocol_id = o.Protocols.FirstOrDefault()?.ProtocolKey,
                            min_date = o.StartTime?.DateTime.Date,
                            max_date = o.StopTime?.DateTime.Date,
                            act_date = o.ActTime.DateTime.Date,
                            product_id = o.Participations?.FirstOrDefault(r => r.ParticipationRoleKey == ActParticipationKey.Product || r.ParticipationRole?.Mnemonic == "Product")?.PlayerEntityKey.Value,
                            sequence_id = o.Protocols.FirstOrDefault()?.Sequence,
                            dose_seq = (o as SubstanceAdministration)?.SequenceId,
                            fulfilled = false
                        }));

                        // Insert plans
                        warehouseService.Add(dataMart.Id, warehousePlan);
                    }, p.Key);
                }
            }

            wtp.WaitOne();

            return 0;
        }

    }
}
