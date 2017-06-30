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
 * User: fyfej
 * Date: 2017-3-24
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.GS1.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using System.Diagnostics;
using OpenIZ.Core.Diagnostics;

namespace OpenIZ.Messaging.GS1.Wcf
{
    /// <summary>
    /// Stock service behavior
    /// </summary>
    [ServiceBehavior(Name = "GS1BMS_Behavior", ConfigurationName = "GS1BMS")]
    public class StockServiceBehavior : IStockService
    {

        // IMSI Trace host
        private readonly TraceSource traceSource = new TraceSource("OpenIZ.Messaging.GS1");

        /// <summary>
        /// Requests the issuance of a BMS1 inventory report request
        /// </summary>
        public LogisticsInventoryReportMessageType IssueInventoryReportRequest(LogisticsInventoryReportRequestMessageType parameters)
        {
            // Status
            LogisticsInventoryReportMessageType retVal = new LogisticsInventoryReportMessageType()
            {
                StandardBusinessDocumentHeader = new StandardBusinessDocumentHeader()
                {
                    HeaderVersion = "1.0",
                    DocumentIdentification = new DocumentIdentification()
                    {
                        Standard = "GS1",
                        TypeVersion = "3.2",
                        InstanceIdentifier = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString("X"),
                        Type = "Logistics Inventory Report",
                        MultipleType = false,
                        MultipleTypeSpecified = true,
                        CreationDateAndTime = DateTime.Now
                    },
                    Sender = new Partner[] {
                        new Partner() {
                            Identifier = new PartnerIdentification() {  Authority = ApplicationContext.Current.Configuration.Custodianship.Id.AssigningAuthority?.Name, Value = ApplicationContext.Current.Configuration.Custodianship?.Id?.Id },
                            ContactInformation = new ContactInformation[] {
                                new ContactInformation()
                                {
                                    Contact = ApplicationContext.Current.Configuration.Custodianship?.Name,
                                    ContactTypeIdentifier = "REGION"
                                }
                            }
                        }
                    }
                }
            };

            // Next, we want to get all active manufactured materials for the specified objects
            IStockManagementRepositoryService stockService = ApplicationContext.Current.GetService<IStockManagementRepositoryService>();
            IPlaceRepositoryService placeService = ApplicationContext.Current.GetService<IPlaceRepositoryService>();
            IActRepositoryService actService = ApplicationContext.Current.GetService<IActRepositoryService>();
            IMaterialRepositoryService materialService = ApplicationContext.Current.GetService<IMaterialRepositoryService>();

            // Date / time of report

            DateTime? reportFrom = parameters.logisticsInventoryReportRequest.First().reportingPeriod?.beginDate ?? DateTime.MinValue,
                reportTo = parameters.logisticsInventoryReportRequest.First().reportingPeriod?.endDate ?? DateTime.Now;

            // return value
            LogisticsInventoryReportType report = new LogisticsInventoryReportType()
            {
                creationDateTime = DateTime.Now,
                documentStatusCode = DocumentStatusEnumerationType.ORIGINAL,
                documentActionCode = DocumentActionEnumerationType.CHANGE_BY_REFRESH,
                logisticsInventoryReportIdentification = new EntityIdentificationType() { entityIdentification = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString("X") },
                structureTypeCode = new StructureTypeCodeType() { Value = "LOCATION_BY_ITEM" },
                documentActionCodeSpecified = true
            };

            var locationStockStatuses = new List<LogisticsInventoryReportInventoryLocationType>();

            // Next, we want to know which facilities for which we're getting the inventory report
            List<Place> filterPlaces = null;
            if (parameters.logisticsInventoryReportRequest.First().logisticsInventoryRequestLocation != null &&
                parameters.logisticsInventoryReportRequest.First().logisticsInventoryRequestLocation.Length > 0)
            {
                foreach (var filter in parameters.logisticsInventoryReportRequest.First().logisticsInventoryRequestLocation)
                {
                    int tc = 0;
                    var id = filter.inventoryLocation.gln ?? filter.inventoryLocation.additionalPartyIdentification?.FirstOrDefault()?.Value;
                    var place = placeService.Find(o => o.Identifiers.Any(i => i.Value == id), 0, 1, out tc).FirstOrDefault();
                    if (place == null)
                        throw new FileNotFoundException($"Place {filter.inventoryLocation.gln} not found");
                    if (filterPlaces == null)
                        filterPlaces = new List<Place>() { place };
                    else
                        filterPlaces.Add(place);
                }
            }
            else
                filterPlaces = placeService.Find(o => o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation).ToList();

            // Get the GLN AA data
            var oidService = ApplicationContext.Current.GetService<IOidRegistrarService>();
            var gln = oidService.GetOid("GLN");
            var gtin = oidService.GetOid("GTIN");

            if (gln == null || gln.Oid == null)
                throw new InvalidOperationException("GLN configuration must carry OID and be named GLN in repository");
            if (gtin == null || gtin.Oid == null)
                throw new InvalidOperationException("GTIN configuration must carry OID and be named GTIN in repository");

            // Create the inventory report
            filterPlaces.AsParallel().ForAll(place =>
            {

                try
                {
                    var locationStockStatus = new LogisticsInventoryReportInventoryLocationType();
                    lock (locationStockStatuses)
                        locationStockStatuses.Add(locationStockStatus);

                    // TODO: Store the GLN configuration domain name
                    locationStockStatus.inventoryLocation = new TransactionalPartyType()
                    {
                        gln = place.Identifiers.FirstOrDefault(o => o.Authority.Oid == gln.Oid)?.Value,
                        address = new AddressType()
                        {
                            state = place.Addresses.FirstOrDefault()?.Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.State)?.Value,
                            city = place.Addresses.FirstOrDefault()?.Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.City)?.Value,
                            countryCode = new CountryCodeType() { Value = place.Addresses.FirstOrDefault()?.Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.Country)?.Value },
                            countyCode = place.Addresses.FirstOrDefault()?.Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.County)?.Value,
                            postalCode = place.Addresses.FirstOrDefault()?.Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.PostalCode)?.Value,
                        },
                        additionalPartyIdentification = place.Identifiers.Select(o => new AdditionalPartyIdentificationType()
                        {
                            additionalPartyIdentificationTypeCode = o.Authority.DomainName,
                            Value = o.Value
                        }).ToArray(),
                        organisationDetails = new OrganisationType()
                        {
                            organisationName = place.Names.FirstOrDefault()?.Component.FirstOrDefault()?.Value
                        }
                    };

                    var tradeItemStatuses = new List<TradeItemInventoryStatusType>();

                    // What are the relationships of held entities
                    foreach (var rel in place.Relationships.Where(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.OwnedEntity))
                    {
                        if (rel.TargetEntity == null)
                            rel.TargetEntity = materialService.GetManufacturedMaterial(rel.TargetEntityKey.Value, Guid.Empty);

                        var mmat = rel.TargetEntity as ManufacturedMaterial;
                        if (!(mmat is ManufacturedMaterial))
                            continue;

                        var mat = materialService.FindMaterial(o => o.Relationships.Where(r => r.RelationshipType.Mnemonic == "ManufacturedProduct").Any(r => r.TargetEntity.Key == mmat.Key)).FirstOrDefault();

                        decimal balanceOH = rel.Quantity ?? 0;

                        // get the adjustments the adjustment acts are allocations and transfers
                        var adjustments = stockService.FindAdjustments(mmat.Key.Value, place.Key.Value, reportFrom, reportTo);

                        // We want to roll back to the start time and re-calc balance oh at time?
                        if (reportTo.Value.Date < DateTime.Now.Date)
                        {
                            var prevAdjustments = stockService.FindAdjustments(mmat.Key.Value, place.Key.Value, reportTo, DateTime.Now);
                            balanceOH -= (decimal)prevAdjustments.Sum(o => o.Participations.FirstOrDefault(p => p.ParticipationRoleKey == ActParticipationKey.Consumable)?.Quantity);
                        }

                        var cvx = mat.LoadProperty<Concept>("TypeConcept").LoadCollection<ConceptReferenceTerm>("ReferenceTerms")?.FirstOrDefault(o => o.LoadProperty<ReferenceTerm>("ReferenceTerm").CodeSystemKey == Guid.Parse("eba4f94a-2cad-4bb3-aca7-f4e54eaac4bd"))?.ReferenceTerm;

                        var typeItemCode = new ItemTypeCodeType()
                        {
                            Value = cvx?.Mnemonic ?? mmat.TypeConcept?.Mnemonic ?? mat.Key.Value.ToString(),
                            codeListVersion = cvx?.LoadProperty<CodeSystem>("CodeSystem")?.Name ?? "OpenIZ-MaterialId"
                        };

                        // First we need the GTIN for on-hand balance
                        tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                        {
                            gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.Oid == gtin.Oid)?.Value,
                            tradeItemDescription = mmat.Names.Select(o => new Description200Type() { Value = o.Component.FirstOrDefault()?.Value }).FirstOrDefault(),
                            itemTypeCode = typeItemCode,
                            additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.Oid != gtin.Oid).Select(o => new AdditionalTradeItemIdentificationType()
                            {
                                additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                                Value = o.Value
                            }).ToArray(),
                            inventoryDateTime = DateTime.Now,
                            inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "ON_HAND" },
                            transactionalItemData = new TransactionalItemDataType[]
                            {
                            new TransactionalItemDataType()
                            {
                                tradeItemQuantity = new QuantityType()
                                {
                                    measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                                    {
                                        additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                        Value = o.ReferenceTerm.Mnemonic
                                    }).FirstOrDefault()?.Value,
                                    Value = balanceOH
                                },
                                batchNumber = mmat.LotNumber,
                                itemExpirationDate = mmat.ExpiryDate.Value,
                                itemExpirationDateSpecified = true
                            }
                            }
                        });

                        // Broken vials?
                        tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                        {
                            gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                            itemTypeCode = typeItemCode,
                            additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                            {
                                additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                                Value = o.Value
                            }).ToArray(),
                            tradeItemDescription = mmat.Names.Select(o => new Description200Type() { Value = o.Component.FirstOrDefault()?.Value }).FirstOrDefault(),
                            inventoryDateTime = DateTime.Now,
                            inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "DAMAGED" },
                            transactionalItemData = new TransactionalItemDataType[]
                            {
                            new TransactionalItemDataType()
                            {
                                tradeItemQuantity = new QuantityType()
                                {
                                    measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                                    {
                                        additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                        Value = o.ReferenceTerm.Mnemonic
                                    }).FirstOrDefault()?.Value,
                                    Value = Math.Abs(adjustments.Where(a => a.ReasonConceptKey.Value == ActReasonKeys.Broken).Sum(o => o.Participations.First(p => p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity.Value))
                                },
                                batchNumber = mmat.LotNumber,
                                itemExpirationDate = mmat.ExpiryDate.Value,
                                itemExpirationDateSpecified = true
                            }
                            }
                        });

                        // Cold storage failure?
                        tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                        {
                            gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                            itemTypeCode = typeItemCode,
                            additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                            {
                                additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                                Value = o.Value
                            }).ToArray(),
                            tradeItemDescription = mmat.Names.Select(o => new Description200Type() { Value = o.Component.FirstOrDefault()?.Value }).FirstOrDefault(),
                            inventoryDateTime = DateTime.Now,
                            inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "COLDCHAIN_FAILED" },
                            transactionalItemData = new TransactionalItemDataType[]
                            {
                            new TransactionalItemDataType()
                            {
                                tradeItemQuantity = new QuantityType()
                                {
                                    measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                                    {
                                        additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                        Value = o.ReferenceTerm.Mnemonic
                                    }).FirstOrDefault()?.Value,
                                    Value = Math.Abs(adjustments.Where(a => a.ReasonConceptKey.Value == ActReasonKeys.ColdStorageFailure).Sum(o => o.Participations.First(p => p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity.Value))
                                },
                                batchNumber = mmat.LotNumber,
                                itemExpirationDate = mmat.ExpiryDate.Value,
                                itemExpirationDateSpecified = true,

                            }
                            }
                        });

                        // Cold storage failure?
                        tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                        {
                            gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                            tradeItemDescription = mmat.Names.Select(o => new Description200Type() { Value = o.Component.FirstOrDefault()?.Value }).FirstOrDefault(),
                            itemTypeCode = typeItemCode,
                            additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                            {
                                additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                                Value = o.Value
                            }).ToArray(),
                            inventoryDateTime = DateTime.Now,
                            inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "EXPIRED" },
                            transactionalItemData = new TransactionalItemDataType[]
                            {
                            new TransactionalItemDataType()
                            {
                                tradeItemQuantity = new QuantityType()
                                {
                                    measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                                    {
                                        additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                        Value = o.ReferenceTerm.Mnemonic
                                    }).FirstOrDefault()?.Value,
                                    Value = Math.Abs(adjustments.Where(a => a.ReasonConceptKey.Value == ActReasonKeys.Expired).Sum(o => o.Participations.First(p => p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity.Value))
                                },
                                batchNumber = mmat.LotNumber,
                                itemExpirationDate = mmat.ExpiryDate.Value,
                                itemExpirationDateSpecified = true
                            }
                            }
                        });

                        // Other / Thrown away?
                        tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                        {
                            gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                            itemTypeCode = typeItemCode,
                            tradeItemDescription = mmat.Names.Select(o => new Description200Type() { Value = o.Component.FirstOrDefault()?.Value }).FirstOrDefault(),
                            additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                            {
                                additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                                Value = o.Value
                            }).ToArray(),
                            inventoryDateTime = DateTime.Now,
                            inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "WASTED" },
                            transactionalItemData = new TransactionalItemDataType[]
                            {
                            new TransactionalItemDataType()
                            {
                                tradeItemQuantity = new QuantityType()
                                {
                                    measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                                    {
                                        additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                        Value = o.ReferenceTerm.Mnemonic
                                    }).FirstOrDefault()?.Value,
                                    Value = Math.Abs(adjustments.Where(a => a.ReasonConceptKey.Value == NullReasonKeys.Other).Sum(o => o.Participations.First(p => p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity.Value))
                                },
                                batchNumber = mmat.LotNumber,
                                itemExpirationDate = mmat.ExpiryDate.Value,
                                itemExpirationDateSpecified = true
                            }
                            }
                        });

                    }

                    // Reduce
                    locationStockStatus.tradeItemInventoryStatus = tradeItemStatuses.ToArray();
                }
                catch(Exception e)
                {
                    traceSource.TraceError("Error fetching stock data : {0}", e);  
                }
                // TODO: Reduce and Group by GTIN
            });

            report.logisticsInventoryReportInventoryLocation = locationStockStatuses.ToArray();
            retVal.logisticsInventoryReport = new LogisticsInventoryReportType[] { report };
            return retVal;
        }
    }
}