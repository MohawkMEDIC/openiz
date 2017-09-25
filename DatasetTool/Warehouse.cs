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
 * Date: 2017-5-26
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
            
            while(ofs < tr) {
                IEnumerable<Place> places = null;
                if(String.IsNullOrEmpty(parms.Name))
                    places = placeService.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, ofs, 100, AuthenticationContext.Current.Principal, out tr);
                else
                    places = placeService.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation && o.Names.Any(n=>n.Component.Any(c=>c.Value.Contains(parms.Name))) , ofs, 100, AuthenticationContext.Current.Principal, out tr);

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
                        stockPolicyObject = (stockPolicyExtension.GetValue() as dynamic[]).GroupBy(o=>o["MaterialEntityId"]).Select(o => new
                        {
                            MaterialEntityId = Guid.Parse(o.FirstOrDefault()["MaterialEntityId"].ToString()),
                            ReorderQuantity = (int?)(o.FirstOrDefault()["ReorderQuantity"] ) ?? 0,
                            SafetyQuantity = (int?)(o.FirstOrDefault()["SafetyQuantity"]) ?? 0,
                            AMC = (int?)(o.FirstOrDefault()["AMC"] ) ?? 0,
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
            
            var warehousePatients = warehouseService.StoredQuery(dataMart.Id, "consistency", new { });
            Guid queryId = Guid.NewGuid();
            int tr = 1, ofs = 0, calc = 0, tq = 0;
            WaitThreadPool wtp = new WaitThreadPool(Environment.ProcessorCount * 2);
            DateTime start = DateTime.Now;

            // Type filters
            List<Guid> typeFilter = new List<Guid>();
            if (parms.ActTypes?.Count > 0)
            {
                var cpcr = ApplicationContext.Current.GetService<IConceptRepositoryService>();
                foreach (var itm in parms.ActTypes)
                    typeFilter.Add(cpcr.GetConcept(itm).Key.Value);
            }

            while (ofs < tr)
            {

                var prodPatients = patientPersistence.Query(o => o.StatusConcept.Mnemonic != "OBSOLETE" && o.ModifiedOn > lastRefresh, queryId, ofs, 100, AuthenticationContext.SystemPrincipal, out tr);
                ofs += 100;

                foreach (var p in prodPatients.Where(o => !warehousePatients.Any(w => w.patient_id == o.Key)))
                {
                    tq++;
                    wtp.QueueUserWorkItem(state =>
                    {

                        AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

                        Patient pState = (Patient)state;

                        List<dynamic> warehousePlan = new List<dynamic>();

                        Interlocked.Increment(ref calc);
                        lock (parms)
                        {
                            var ips = (((double)(DateTime.Now - start).Ticks / calc) * (tq - calc));
                            Console.CursorLeft = 0;
                            Console.Write("    Calculating care plan {0}/{1} <<Scan: {4} ({5:0%})>> ({2:0%}) [ETA: {3}] {6:0.##} R/S ", calc, tq, (float)calc / tq, new TimeSpan((long)ips).ToString("hh'h 'mm'm 'ss's'"), ofs, (float)ofs/tr, ((double)calc / (double)(DateTime.Now - start).TotalSeconds));
                        }

                        var data = p; //  ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>().Get(p.Key.Value);

                        // First, we want a copy of the warehouse
                        var existing = warehouseService.AdhocQuery(dataMart.Id, new
                        {
                            patient_id = data.Key.Value
                        });
                        warehouseService.Delete(dataMart.Id, new { patient_id = data.Key.Value });
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

                        var fulfillCalc = data.LoadCollection<ActParticipation>("Participations");
                        if (typeFilter.Count > 0)
                            fulfillCalc = fulfillCalc.Where(o => typeFilter.Contains(o.LoadProperty<Act>("Act").TypeConceptKey ?? Guid.Empty));

                        // Are there care plan items existing that dont exist in the calculated care plan, if so that means that the patient has completed steps and we need to indicate that
                        if (existing.Any(o=>!o.fulfilled)) // != true is needed because it can be null.
                        {
                            var fulfilled = existing.Where(o => !warehousePlan.Any(pl => pl.protocol_id == o.protocol_id && pl.sequence_id == o.sequence_id));
                            warehousePlan.AddRange(fulfilled.Select(o => new
                            {
                                creation_date = o.creation_date,
                                patient_id = o.patient_id,
                                location_id = o.location_id,
                                act_id = data.LoadCollection<ActParticipation>("Participations").FirstOrDefault(ap => ap.LoadProperty<Act>("Act").LoadCollection<ActProtocol>("Protocols").Any(pr => pr.ProtocolKey == o.protocol_id && pr.Sequence == o.sequence_id))?.Key ?? o.act_id,
                                class_id = o.class_id,
                                type_id = o.type_id,
                                protocol_id = o.protocol_id,
                                min_date = o.min_date,
                                max_date = o.max_date,
                                act_date = o.act_date,
                                product_id = o.product_id,
                                sequence_id = o.sequence_id,
                                dose_seq = o.dose_seq,
                                fulfilled = true
                            }));
                        }
                        else if(
                        !parms.NoFulfill &&
                        fulfillCalc.Any()) // not calculated anything but there are steps previously done, this is a little more complex
                        {
                            // When something is not yet calculated what we have to do is strip away each act done as part of the protocol and re-calculate when that action was supposed to occur
                            // For example: We calculate PCV we strip away PCV2 and re-run the plan to get the proposal of PCV3 then strip away PCV1 and re-run the plan to get the proposal of PCV2
                            var acts = fulfillCalc.Select(o => o.LoadProperty<Act>("Act"));
                            foreach(var itm in acts.GroupBy(o=>o.LoadCollection<ActProtocol>("Protocols").FirstOrDefault()?.ProtocolKey ?? Guid.Empty))
                            {
                                var steps = itm.OrderByDescending(o => o.LoadCollection<ActProtocol>("Protocols").FirstOrDefault()?.Sequence);
                                var patientClone = data.Clone() as Patient;
                                patientClone.Participations = new List<ActParticipation>(data.Participations);
                                foreach(var s in steps)
                                {
                                    patientClone.Participations.RemoveAll(o => o.ActKey == s.Key);
                                    // Run protocol 
                                    IEnumerable<Act> candidate; 
                                    if (itm.Key == Guid.Empty) // There is no protocol identifier
                                    {
                                        var tempPlan = careplanService.CreateCarePlan(patientClone, false, new Dictionary<String, Object>() { { "isBackground", true }, { "ignoreEntry", true } });
                                        if (tempPlan.Action.Count == 0) continue;
                                        candidate = tempPlan.Action.OfType<SubstanceAdministration>().Where(o => o.SequenceId == (s as SubstanceAdministration)?.SequenceId &&
                                                                                                             o.Participations.FirstOrDefault(pt=>pt.ParticipationRoleKey == ActParticipationKey.Product).PlayerEntityKey == s.Participations.FirstOrDefault(pt=>pt.ParticipationRoleKey == ActParticipationKey.Product).PlayerEntityKey);
                                        if (candidate.Count() != 1) continue;
                                    }
                                    else
                                    {
                                        var tempPlan = careplanService.CreateCarePlan(patientClone, false, new Dictionary<String, Object>() { { "isBackground", true }, { "ignoreEntry", true } }, itm.Key);
                                        if (tempPlan.Action.Count == 0) continue;
                                        candidate = tempPlan.Action.Where(o => o.Protocols.FirstOrDefault().Sequence == s.Protocols.FirstOrDefault().Sequence);
                                        if (candidate.Count() != 1)
                                        {
                                            candidate = tempPlan.Action.OfType<SubstanceAdministration>().Where(o => o.SequenceId == (s as SubstanceAdministration)?.SequenceId);
                                            if (candidate.Count() != 1) continue;
                                        }
                                    }

                                    var planned = candidate.FirstOrDefault();

                                    warehousePlan.Add(new
                                    {
                                        creation_date = DateTime.Now,
                                        patient_id = data.Key.Value,
                                        location_id = data.Relationships.FirstOrDefault(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation || r.RelationshipType?.Mnemonic == "DedicatedServiceDeliveryLocation")?.TargetEntityKey.Value,
                                        act_id = s.Key,
                                        class_id = planned.ClassConceptKey.Value,
                                        type_id = planned.TypeConceptKey.Value,
                                        protocol_id = itm.Key,
                                        min_date = planned.StartTime?.DateTime.Date,
                                        max_date = planned.StopTime?.DateTime.Date,
                                        act_date = planned.ActTime.DateTime.Date,
                                        product_id = planned.Participations?.FirstOrDefault(r => r.ParticipationRoleKey == ActParticipationKey.Product || r.ParticipationRole?.Mnemonic == "Product")?.PlayerEntityKey.Value,
                                        sequence_id = planned.Protocols.FirstOrDefault()?.Sequence,
                                        dose_seq = (planned as SubstanceAdministration)?.SequenceId, 
                                        fulfilled = true
                                    });
                                }
                            }
                        }

                        // Insert plans
                        warehouseService.Add(dataMart.Id, warehousePlan);
                    }, p);
                }
            }

            wtp.WaitOne();

            return 0;
        }

    }
}
