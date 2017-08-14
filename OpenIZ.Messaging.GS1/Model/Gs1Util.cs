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
 * Date: 2017-7-4
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.GS1.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1.Model
{
    /// <summary>
    /// GS1 Utility class
    /// </summary>
    public class Gs1Util
    {
        // Configuration
        private Gs1ConfigurationSection m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.gs1") as Gs1ConfigurationSection;

        // Act repository
        private IActRepositoryService m_actRepository;
        // Material repository
        private IMaterialRepositoryService m_materialRepository;
        // Place repository
        private IPlaceRepositoryService m_placeRepository;
        // Stock service
        private IStockManagementRepositoryService m_stockService;

        /// <summary>
        /// GS1 Utility class
        /// </summary>
        public Gs1Util()
        {
            this.m_actRepository = ApplicationContext.Current.GetService<IActRepositoryService>();
            this.m_materialRepository = ApplicationContext.Current.GetService<IMaterialRepositoryService>();
            this.m_placeRepository = ApplicationContext.Current.GetService<IPlaceRepositoryService>();
            this.m_stockService = ApplicationContext.Current.GetService<IStockManagementRepositoryService>();
        }

        /// <summary>
        /// Converts the specified IMS place into a TransactionalPartyType
        /// </summary>
        public TransactionalPartyType CreateLocation(Place place)
        {
            if (place == null) return null;

            var oidService = ApplicationContext.Current.GetService<IOidRegistrarService>();
            var gln = oidService.GetOid("GLN");
            return new TransactionalPartyType()
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
        }

        /// <summary>
        /// Gets the specified facility from the gs1 party information
        /// </summary>
        public Place GetLocation(TransactionalPartyType gs1Party)
        {

            if (gs1Party == null) return null;

            Place retVal = null;
            int tr = 0;

            // First, we will attempt to look up by GLN
            if (!String.IsNullOrEmpty(gs1Party.gln))
            {
                retVal = this.m_placeRepository.Find(p => p.Identifiers.Any(o => o.Value == gs1Party.gln && o.Authority.DomainName == "GLN"), 0, 1, out tr).FirstOrDefault();
                if (retVal == null)
                    throw new KeyNotFoundException($"Facility with GLN {gs1Party.gln} not found");
            }

            // let's look it up by alternate identifier then
            foreach (var id in gs1Party.additionalPartyIdentification)
            {
                retVal = this.m_placeRepository.Find(p => p.Identifiers.Any(i => i.Value == id.Value && i.Authority.DomainName == id.additionalPartyIdentificationTypeCode), 0, 1, out tr).FirstOrDefault();
                if (retVal != null) break;
            }

            return retVal;
        }

        /// <summary>
        /// Get order information
        /// </summary>
        public Act GetOrder(Ecom_DocumentReferenceType documentReference, Guid moodConceptKey)
        {
            if (documentReference == null)
                throw new ArgumentNullException("documentReference", "Document reference must be supplied for correlation lookup");
            else if (String.IsNullOrEmpty(documentReference.entityIdentification))
                throw new ArgumentException("Document reference must carry entityIdentification", "documentReference");

            Guid orderId = Guid.Empty;
            Act retVal = null;

            if (Guid.TryParse(documentReference.entityIdentification, out orderId))
                retVal = this.m_actRepository.Get<Act>(orderId, Guid.Empty);
            if (retVal == null)
            {
                var oidService = ApplicationContext.Current.GetService<IOidRegistrarService>();
                var gln = oidService.GetOid("GLN");
                var issuingAuthority = oidService.FindData($"{gln.Oid}.{documentReference.contentOwner?.gln}");
                if (issuingAuthority == null)
                    issuingAuthority = oidService.GetOid(this.m_configuration.DefaultContentOwnerAssigningAuthority);
                if (issuingAuthority == null)
                    throw new InvalidOperationException("Could not find assigning authority linked with document reference owner. Please specify a default in the configuration");

                int tr = 0;
                retVal = this.m_actRepository.Find<Act>(o => o.ClassConceptKey == ActClassKeys.Supply && o.MoodConceptKey == moodConceptKey && o.Identifiers.Any(i => i.Value == documentReference.entityIdentification && i.Authority.DomainName == issuingAuthority.Mnemonic), 0, 1, out tr).FirstOrDefault();
            }
            return retVal;
        }

        /// <summary>
        /// Create receive line item
        /// </summary>
        public ReceivingAdviceLogisticUnitType CreateReceiveLineItem(ActParticipation orderReceivePtcpt, ActParticipation orderSentPtcpt)
        {
            if (orderSentPtcpt == null)
                throw new ArgumentNullException(nameof(orderSentPtcpt), "Missing sending order participation");
            else if (orderReceivePtcpt == null)
                throw new ArgumentNullException(nameof(orderReceivePtcpt), "Missing receiving order participation");

            // Quantity code
            var quantityCode = ApplicationContext.Current.GetService<IConceptRepositoryService>().GetConceptReferenceTerm(orderReceivePtcpt.LoadProperty<Material>("PlayerEntity").QuantityConceptKey.Value, "UCUM");

            if (quantityCode == null)
                throw new InvalidOperationException($"Missing quantity code for {orderReceivePtcpt.LoadProperty<Material>("PlayerEntity").QuantityConceptKey.Value}");

            // Receiving logistic unit type
            return new ReceivingAdviceLogisticUnitType()
            {
                receivingAdviceLineItem = new ReceivingAdviceLineItemType[]
                {
                    new ReceivingAdviceLineItemType()
                    {
                        quantityDespatched = new QuantityType()
                        {
                            Value = Math.Abs((decimal)orderSentPtcpt.Quantity),
                            measurementUnitCode = quantityCode.Mnemonic ?? "dose", codeListVersion = "UCUM"
                        },
                        quantityAccepted = new QuantityType()
                        {
                            Value = (decimal)orderReceivePtcpt.Quantity,
                            measurementUnitCode = quantityCode.Mnemonic ?? "dose", codeListVersion = "UCUM"
                        },
                        transactionalTradeItem = this.CreateTradeItem(orderReceivePtcpt.LoadProperty<Material>("PlayerEntity")),
                        receivingConditionInformation = new ReceivingConditionInformationType[]
                        {
                            new ReceivingConditionInformationType()
                            {
                                receivingConditionCode = new ReceivingConditionCodeType() { Value = "DAMAGED_PRODUCT_OR_CONTAINER" },
                                receivingConditionQuantity = new QuantityType()
                                {
                                    Value = (decimal)(Math.Abs(orderSentPtcpt.Quantity.Value) - orderReceivePtcpt.Quantity),
                                    measurementUnitCode = quantityCode.Mnemonic ?? "dose", codeListVersion = "UCUM"
                                }
                            },
                            new ReceivingConditionInformationType()
                            {
                                receivingConditionCode = new ReceivingConditionCodeType() { Value = "GOOD_CONDITION" },
                                receivingConditionQuantity = new QuantityType()
                                {
                                    Value = (decimal)orderReceivePtcpt.Quantity,
                                    measurementUnitCode = quantityCode.Mnemonic ?? "dose", codeListVersion = "UCUM"
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Order line item type
        /// </summary>
        public OrderLineItemType CreateOrderLineItem(ActParticipation participation)
        {
            var material = participation.LoadProperty<Material>("PlayerEntity");
            var quantityCode = ApplicationContext.Current.GetService<IConceptRepositoryService>().GetConceptReferenceTerm(material.QuantityConceptKey.Value, "UCUM");
            return new OrderLineItemType()
            {
                requestedQuantity = new QuantityType() { Value = (decimal)participation.Quantity, measurementUnitCode = quantityCode?.Mnemonic ?? "dose", codeListVersion = "UCUM" },
                additionalOrderLineInstruction =
                    material.LoadProperty<Concept>("TypeConcept")?.Mnemonic.StartsWith("VaccineType") == true ?
                        new Description200Type[] {
                            new Description200Type() { languageCode = "en", Value = "FRAGILE" },
                            new Description200Type() { languageCode = "en", Value = "KEEP REFRIGERATED" }
                        } : null,
                transactionalTradeItem = this.CreateTradeItem(material)
            };
        }

        /// <summary>
        /// Create a trade item
        /// </summary>
        public TransactionalTradeItemType CreateTradeItem(Material material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material), "Missing material instance");

            ReferenceTerm cvx = null;
            if(material.TypeConceptKey.HasValue)
                cvx = ApplicationContext.Current.GetService<IConceptRepositoryService>().GetConceptReferenceTerm(material.TypeConceptKey.Value, "CVX");
            var typeItemCode = new ItemTypeCodeType()
            {
                Value = cvx?.Mnemonic ?? material.TypeConcept?.Mnemonic ?? material.Key.Value.ToString(),
                codeListVersion = cvx?.LoadProperty<CodeSystem>("CodeSystem")?.Authority ?? "OpenIZ-MaterialType"
            };

            // Manufactured material?
            if (material is ManufacturedMaterial)
            {
                var mmat = material as ManufacturedMaterial;
                var mat = this.m_materialRepository.FindMaterial(o => o.Relationships.Where(r => r.RelationshipType.Mnemonic == "Instance").Any(r => r.TargetEntity.Key == mmat.Key)).FirstOrDefault();

                return new TransactionalTradeItemType()
                {
                    additionalTradeItemIdentification = material.LoadCollection<EntityIdentifier>("Identifiers").Where(o => o.Authority.DomainName != "GTIN").Select(o => new AdditionalTradeItemIdentificationType()
                    {
                        Value = o.Value,
                        additionalTradeItemIdentificationTypeCode = o.LoadProperty<AssigningAuthority>("Authority").DomainName
                    }).ToArray(),
                    tradeItemClassification = new TradeItemClassificationType()
                    {
                        additionalTradeItemClassificationCode = mat.LoadCollection<EntityIdentifier>("Identifiers").Select(o => new AdditionalTradeItemClassificationCodeType()
                        {
                            Value = o.Value,
                            codeListVersion = o.LoadProperty<AssigningAuthority>("Authority").DomainName
                        }).ToArray()
                    },
                    gtin = material.Identifiers.FirstOrDefault(o => o.Authority.DomainName == "GTIN").Value,
                    itemTypeCode = typeItemCode,
                    tradeItemDescription = material.Names.Select(o => new Description200Type() { Value = o.Component.FirstOrDefault()?.Value }).FirstOrDefault(),
                    transactionalItemData = new TransactionalItemDataType[]
                    {
                        new TransactionalItemDataType() {
                            batchNumber = mmat.LotNumber,
                            itemExpirationDate = mmat.ExpiryDate.Value,
                            itemExpirationDateSpecified = true
                        }
                    }
                };
            }
            else // Material code
            {
                return new TransactionalTradeItemType()
                {
                    tradeItemClassification = new TradeItemClassificationType()
                    {
                        additionalTradeItemClassificationCode = material.LoadCollection<EntityIdentifier>("Identifiers").Select(o => new AdditionalTradeItemClassificationCodeType()
                        {
                            Value = o.Value,
                            codeListVersion = o.LoadProperty<AssigningAuthority>("Authority").DomainName
                        }).ToArray()
                    },
                    itemTypeCode = typeItemCode,
                    tradeItemDescription = cvx?.LoadCollection<ReferenceTermName>("DisplayNames")?.Select(o => new Description200Type() { Value = o.Name })?.FirstOrDefault() ??
                        material.Names.Select(o => new Description200Type() { Value = o.Component.FirstOrDefault()?.Value }).FirstOrDefault(),
                };
            }
        }

        /// <summary>
        /// Gets the manufactured material from the specified trade item
        /// </summary>
        public ManufacturedMaterial GetManufacturedMaterial(TransactionalTradeItemType tradeItem, bool createIfNotFound = false)
        {
            if (tradeItem == null)
                throw new ArgumentNullException("tradeItem", "Trade item must have a value");
            else if (String.IsNullOrEmpty(tradeItem.gtin))
                throw new ArgumentException("Trade item is missing GTIN", "tradeItem");


            var oidService = ApplicationContext.Current.GetService<IOidRegistrarService>();
            var gtin = oidService.GetOid("GTIN");

            // Lookup material by lot number / gtin
            int tr = 0;
            var lotNumberString = tradeItem.transactionalItemData[0].lotNumber;
            ManufacturedMaterial retVal = this.m_materialRepository.FindManufacturedMaterial(m => m.Identifiers.Any(o => o.Value == tradeItem.gtin && o.Authority.DomainName == "GTIN") && m.LotNumber == lotNumberString, 0, 1, out tr).FirstOrDefault();
            if (retVal == null && createIfNotFound)
            {
                var additionalData = tradeItem.transactionalItemData[0];
                if (!additionalData.itemExpirationDateSpecified)
                    throw new InvalidOperationException("Cannot auto-create material, expiration date is missing");

                // Material
                retVal = new ManufacturedMaterial()
                {
                    Key = Guid.NewGuid(),
                    LotNumber = additionalData.lotNumber,
                    Identifiers = new List<EntityIdentifier>()
                    {
                        new EntityIdentifier(new AssigningAuthority(gtin.Mnemonic, gtin.Name, gtin.Oid), tradeItem.gtin)
                    },
                    ExpiryDate = additionalData.itemExpirationDate,
                    Names = new List<EntityName>()
                    {
                        new EntityName(NameUseKeys.Assigned, tradeItem.tradeItemDescription.Value)
                    },
                    StatusConceptKey = StatusKeys.Active,
                    QuantityConceptKey = Guid.Parse("a4fc5c93-31c2-4f87-990e-c5a4e5ea2e76"),
                    Quantity = 1
                };

                // Store additional identifiers
                if (tradeItem.additionalTradeItemIdentification != null)
                    foreach (var id in tradeItem.additionalTradeItemIdentification)
                    {
                        var oid = oidService.GetOid(id.additionalTradeItemIdentificationTypeCode);
                        if (oid == null) continue;
                        retVal.Identifiers.Add(new EntityIdentifier(new AssigningAuthority(oid.Mnemonic, oid.Name, oid.Oid), id.Value));
                    }

                if (String.IsNullOrEmpty(tradeItem.itemTypeCode?.Value))
                    throw new InvalidOperationException("Cannot auto-create material, type code must be specified");
                else // lookup type code
                {
                    var concept = ApplicationContext.Current.GetService<IConceptRepositoryService>().FindConceptsByReferenceTerm(tradeItem.itemTypeCode.Value, tradeItem.itemTypeCode.codeListVersion).FirstOrDefault();
                    if (concept == null && tradeItem.itemTypeCode.codeListVersion == "OpenIZ-MaterialType")
                        concept = ApplicationContext.Current.GetService<IConceptRepositoryService>().GetConcept(tradeItem.itemTypeCode.Value);

                    // Type code not found
                    if (concept == null)
                        throw new InvalidOperationException($"Material type {tradeItem.itemTypeCode.Value} is not a valid concept");

                    // Get the material and set the type
                    retVal.TypeConceptKey = concept.Key;

                }

                // Find the type of material
                Material materialReference = null;
                if (tradeItem.tradeItemClassification != null)
                    foreach (var id in tradeItem.tradeItemClassification.additionalTradeItemClassificationCode)
                    {
                        materialReference = this.m_materialRepository.FindMaterial(o => o.Identifiers.Any(i => i.Value == id.Value && i.Authority.DomainName == id.codeListVersion) && o.ClassConceptKey == EntityClassKeys.Material && o.StatusConceptKey != StatusKeys.Obsolete, 0, 1, out tr).SingleOrDefault();
                        if (materialReference != null) break;
                    }
                if (materialReference == null)
                    materialReference = this.m_materialRepository.FindMaterial(o => o.TypeConceptKey == retVal.TypeConceptKey && o.ClassConceptKey == EntityClassKeys.Material && o.StatusConceptKey != StatusKeys.Obsolete, 0, 1, out tr).SingleOrDefault();
                if (materialReference == null)
                    throw new InvalidOperationException("Cannot find the base Material from trade item type code");

                // Material relationship
                EntityRelationship materialRelationship = new EntityRelationship()
                {
                    RelationshipTypeKey = EntityRelationshipTypeKeys.Instance,
                    Quantity = (int)(additionalData.tradeItemQuantity?.Value ?? 1),
                    SourceEntityKey = materialReference.Key,
                    TargetEntityKey = retVal.Key,
                    EffectiveVersionSequenceId = materialReference.VersionSequence
                };

                // TODO: Manufacturer won't be known

                // Insert the material && relationship
                ApplicationContext.Current.GetService<IBatchRepositoryService>().Insert(new Bundle()
                {
                    Item = new List<IdentifiedData>()
                    {
                        retVal,
                        materialRelationship
                    }
                });

            }
            else if (tradeItem.additionalTradeItemIdentification != null) // We may want to keep track of other identifiers this software knows as
            {
                bool shouldSave = false;
                foreach (var id in tradeItem.additionalTradeItemIdentification)
                {
                    var oid = oidService.GetOid(id.additionalTradeItemIdentificationTypeCode);
                    if (oid == null) continue;
                    if (!retVal.Identifiers.Any(o => o.LoadProperty<AssigningAuthority>("Authority").DomainName == oid.Mnemonic))
                    {
                        retVal.Identifiers.Add(new EntityIdentifier(new AssigningAuthority(oid.Mnemonic, oid.Name, oid.Oid), id.Value));
                        shouldSave = true;
                    }
                }

                if (shouldSave)
                    this.m_materialRepository.Save(retVal);

            }

            return retVal;
        }

        /// <summary>
        /// Creates the document header
        /// </summary>
        /// <returns></returns>
        public StandardBusinessDocumentHeader CreateDocumentHeader(String messageType, Entity senderInformation)
        {
            return new StandardBusinessDocumentHeader()
            {
                HeaderVersion = "1.0",
                DocumentIdentification = new DocumentIdentification()
                {
                    Standard = "GS1",
                    TypeVersion = "3.3",
                    InstanceIdentifier = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString("X"),
                    Type = messageType,
                    MultipleType = false,
                    MultipleTypeSpecified = true,
                    CreationDateAndTime = DateTime.Now
                },
                Sender = senderInformation == null ? new Partner[] {
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
                    } : new Partner[]
                    {
                        new Partner()
                        {
                            Identifier = new PartnerIdentification() { Authority = ApplicationContext.Current.Configuration.Custodianship.Id.AssigningAuthority?.Name, Value = senderInformation.Key.Value.ToString() },
                            ContactInformation = new ContactInformation[]
                            {
                                new ContactInformation()
                                {
                                    Contact = senderInformation.LoadCollection<EntityName>("Names").FirstOrDefault()?.ToString(),
                                    EmailAddress = senderInformation.LoadCollection<EntityTelecomAddress>("Telecoms").FirstOrDefault(o=>o.Value.Contains("@"))?.Value,
                                    TelephoneNumber = senderInformation.Telecoms.FirstOrDefault(o=>!o.Value.Contains("@"))?.Value
                                }
                            }
                    }
                }
            };
        }

    }
}
