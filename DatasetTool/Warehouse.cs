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
        }


        /// <summary>
        /// Calculates the AMC for all facilities in the system
        /// </summary>
        public static int Amc(string[] args)
        {

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

            foreach (var plc in placeService.Query(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation, AuthenticationContext.Current.Principal))
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
                    MaterialKey = matlService.QueryFast(m => m.Relationships.Any(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.Instance && r.TargetEntityKey == o.Key), Guid.Empty, 0, 1, AuthenticationContext.Current.Principal, out t).FirstOrDefault().Key
                }).ToList();

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
                    stockPolicyObject = (stockPolicyExtension.GetValue() as dynamic[]).Select(o=>new
                    {
                        MaterialEntityId = Guid.Parse(o["MaterialEntityId"].ToString()),
                        ReorderQuantity = (int)(o["ReorderQuantity"]),
                        SafetyQuantity = (int)(o["SafetyQuantity"]),
                        AMC = (int)(o["AMC"]),
                        Multiplier = (int)(o["Multiplier"])
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
                        existingPolicy = new {
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

                if(hasChanged)
                    placeService.Update(plc, AuthenticationContext.Current.Principal, TransactionMode.Commit);
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
            WaitThreadPool wtp = new WaitThreadPool();
            DateTime start = DateTime.Now;

            while (ofs < tr)
            {

                var prodPatients = patientPersistence.Query(o => o.StatusConcept.Mnemonic != "OBSOLETE" && o.ModifiedOn > lastRefresh, queryId, ofs, 15, AuthenticationContext.SystemPrincipal, out tr);
                ofs += 15;

                foreach (var p in prodPatients.Where(o => !warehousePatients.Any(w => w.patient_id == o.Key)))
                {
                    tq++;
                    wtp.QueueUserWorkItem(state =>
                    {

                        AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

                        Patient pState = (Patient)state;

                        List<Object> warehousePlan = new List<Object>();

                        Interlocked.Increment(ref calc);
                        lock (parms)
                        {
                            var ips = (((double)(DateTime.Now - start).Ticks / calc) * (tq - calc));
                            Console.CursorLeft = 0;
                            Console.Write("    Calculating care plan {0}/{1} ({2:0%}) [ETA: {3}]  ", calc, tq, (float)calc / tq, new TimeSpan((long)ips).ToString("hh'h 'mm'm 'ss's'"));
                        }

                        var data = p; //  ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>().Get(p.Key.Value);

                        // First, we clear the warehouse
                        warehouseService.Delete(dataMart.Id, new { patient_id = data.Key.Value });
                        var careplanService = ApplicationContext.Current.GetService<ICarePlanService>();

                        // Now calculate
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
                            dose_seq = (o as SubstanceAdministration)?.SequenceId
                        }));

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
