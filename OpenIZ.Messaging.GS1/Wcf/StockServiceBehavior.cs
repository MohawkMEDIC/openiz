using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Messaging.GS1.Model;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Messaging.GS1.Wcf
{
    /// <summary>
    /// Stock service behavior
    /// </summary>
    [ServiceBehavior(Name = "GS1BMS_Behavior", ConfigurationName = "GS1BMS_Behavior")]
    public class StockServiceBehavior : IStockService
    {
        /// <summary>
        /// Requests the issuance of a BMS1 inventory report request
        /// </summary>
        public LogisticsInventoryReportMessageType IssueInventoryReportRequest()
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
                            Identifier = new PartnerIdentification() {  Authority = ApplicationContext.Current.Configuration.Custodianship.Id.AssigningAuthority.Name, Value = ApplicationContext.Current.Configuration.Custodianship.Id.Id },
                            ContactInformation = new ContactInformation[] {
                                new ContactInformation()
                                {
                                    Contact = ApplicationContext.Current.Configuration.Custodianship.Contact.Name,
                                    EmailAddress = ApplicationContext.Current.Configuration.Custodianship.Contact.Email,
                                    ContactTypeIdentifier = "REGION"
                                }
                            }
                        }
                    }
                }
            };

            // Next, we want to get all active manufactured materials
            IStockManagementRepositoryService stockService = ApplicationContext.Current.GetService<IStockManagementRepositoryService>();
            IPlaceRepositoryService placeService = ApplicationContext.Current.GetService<IPlaceRepositoryService>();

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
            foreach (var place in placeService.Find(o=>o.Relationships.Any(r=>r.RelationshipTypeKey == EntityRelationshipTypeKeys.HeldEntity) && o.ClassConceptKey == EntityClassKeys.ServiceDeliveryLocation))
            {
                var locationStockStatus = new LogisticsInventoryReportInventoryLocationType();
                locationStockStatuses.Add(locationStockStatus);

                // TODO: Store the GLN configuration domain name
                locationStockStatus.inventoryLocation = new TransactionalPartyType() {
                    gln = place.Identifiers.First(o => o.Authority.DomainName == "GLN")?.Value,
                    address = new AddressType()
                    {
                        state = place.Addresses.First().Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.State)?.Value,
                        city = place.Addresses.First().Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.City)?.Value,
                        countryCode = new CountryCodeType() { Value = place.Addresses.First().Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.Country)?.Value },
                        countyCode = place.Addresses.First().Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.County)?.Value,
                        postalCode = place.Addresses.First().Component.FirstOrDefault(o => o.ComponentTypeKey == AddressComponentKeys.PostalCode)?.Value,
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
                foreach(var rel in place.Relationships.Where(o=>o.RelationshipTypeKey == EntityRelationshipTypeKeys.HeldEntity))
                {

                    var mmat = rel.TargetEntity as ManufacturedMaterial;
                    if (!(mmat is ManufacturedMaterial))
                        continue;

                    var mat = mmat.Relationships.FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.ManufacturedProduct).TargetEntity as Material;


                    // First we need the GTIN for on-hand balance
                    tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                    {
                        gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                        additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                        {
                            additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                            Value = o.Value
                        }).ToArray(),
                        inventoryDateTime = DateTime.Now,
                        inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "ON_HAND" },
                        tradeItemQuantity = new QuantityType()
                        {
                            measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                            {
                                additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                Value = o.ReferenceTerm.Mnemonic
                            }).FirstOrDefault()?.Value,
                            Value = rel.Quantity
                        }
                    });

                    // Second get the adjustments the adjustment acts are allocations and transfers 
                    var adjustments = stockService.FindAdjustments(mmat.Key.Value, place.Key.Value, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

                    // Broken vials?
                    tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                    {
                        gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                        additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                        {
                            additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                            Value = o.Value
                        }).ToArray(),
                        inventoryDateTime = DateTime.Now,
                        inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "DAMAGED" },
                        tradeItemQuantity = new QuantityType()
                        {
                            measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                            {
                                additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                Value = o.ReferenceTerm.Mnemonic
                            }).FirstOrDefault()?.Value,
                            Value = adjustments.Where(a=>a.ReasonConceptKey.Value == ActReasonKeys.Broken).Sum(o=>o.Participations.First(p=>p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity)
                        }
                    });

                    // Cold storage failure?
                    tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                    {
                        gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                        additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                        {
                            additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                            Value = o.Value
                        }).ToArray(),
                        inventoryDateTime = DateTime.Now,
                        inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "COLDCHAIN_FAILED" },
                        tradeItemQuantity = new QuantityType()
                        {
                            measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                            {
                                additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                Value = o.ReferenceTerm.Mnemonic
                            }).FirstOrDefault()?.Value,
                            Value = adjustments.Where(a => a.ReasonConceptKey.Value == ActReasonKeys.ColdStorageFailure).Sum(o => o.Participations.First(p => p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity)
                        }
                    });

                    // Cold storage failure?
                    tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                    {
                        gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                        additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                        {
                            additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                            Value = o.Value
                        }).ToArray(),
                        inventoryDateTime = DateTime.Now,
                        inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "EXPIRED" },
                        tradeItemQuantity = new QuantityType()
                        {
                            measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                            {
                                additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                Value = o.ReferenceTerm.Mnemonic
                            }).FirstOrDefault()?.Value,
                            Value = adjustments.Where(a => a.ReasonConceptKey.Value == ActReasonKeys.ColdStorageFailure).Sum(o => o.Participations.First(p => p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity)
                        }
                    });

                    // Other / Thrown away?
                    tradeItemStatuses.Add(new TradeItemInventoryStatusType()
                    {
                        gtin = mmat.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN")?.Value,
                        additionalTradeItemIdentification = mmat.Identifiers.Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                        {
                            additionalTradeItemIdentificationTypeCode = o.Authority.DomainName,
                            Value = o.Value
                        }).ToArray(),
                        inventoryDateTime = DateTime.Now,
                        inventoryDispositionCode = new InventoryDispositionCodeType() { Value = "WASTED" },
                        tradeItemQuantity = new QuantityType()
                        {
                            measurementUnitCode = (mmat.QuantityConcept ?? mat?.QuantityConcept)?.ReferenceTerms.Select(o => new AdditionalLogisticUnitIdentificationType()
                            {
                                additionalLogisticUnitIdentificationTypeCode = o.ReferenceTerm.CodeSystem.Name,
                                Value = o.ReferenceTerm.Mnemonic
                            }).FirstOrDefault()?.Value,
                            Value = adjustments.Where(a => a.ReasonConceptKey.Value == NullReasonKeys.Other).Sum(o => o.Participations.First(p => p.ParticipationRoleKey == ActParticipationKey.Consumable).Quantity)
                        }
                    });

                }
            }

            report.logisticsInventoryReportInventoryLocation = locationStockStatuses.ToArray();
            retVal.logisticsInventoryReport = new LogisticsInventoryReportType[] { report };
            return retVal;
        }
    }
}
