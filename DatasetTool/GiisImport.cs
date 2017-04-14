using GIIS.DataLayer;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Extensions;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Persistence;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OizDevTool
{
    /// <summary>
    /// Import routines for GIIS
    /// </summary>
    [Description("Tooling for importing data from Generic Immunization Information System (GIIS)")]
    public static class GiisImport
    {
        /// <summary>
        /// Console parameters
        /// </summary>
        public class ConsoleParameters
        {
            /// <summary>
            /// Live migration
            /// </summary>
            [Parameter("live")]
            [Description("Indicates the migration should be done in place (opposed to generating files)")]
            public bool LiveMigration { get; set; }

            /// <summary>
            /// Output directory
            /// </summary>
            [Parameter("output")]
            [Parameter("o")]
            [Description("The directory in which to output generate files")]
            public String OutputDirectory { get; set; }

        }

        private static Guid industryManufacturer = Guid.NewGuid();
        private static Guid industryHealthDelivery = Guid.NewGuid();
        private static Guid industryOther = Guid.NewGuid();
        private static Dictionary<Int32, Guid> facilityMap = new Dictionary<int, Guid>();
        private static Dictionary<Int32, Guid> facilityTypeId = new Dictionary<int, Guid>();
        private static Dictionary<Int32, Guid> roleMap = new Dictionary<int, Guid>();
        private static Dictionary<Int32, Guid> userMap = new Dictionary<int, Guid>();
        private static Dictionary<Int32, Guid> userEntityMap = new Dictionary<int, Guid>();
        private static Dictionary<Int32, Guid> placeEntityMap = new Dictionary<int, Guid>();
        private static Dictionary<Int32, Guid> manufacturerMap = new Dictionary<int, Guid>();
        private static Dictionary<Int32, Guid> manufacturedMaterialMap = new Dictionary<Int32, Guid>();
        private static Dictionary<Int32, Guid> materialMap = new Dictionary<int, Guid>()
        {
            { 42, Guid.Parse("ED144BD2-A334-40A2-9A8F-B767A1397D07") },
            { 45, Guid.Parse("41B008A6-FCF8-40BC-AB96-7567E94BCF8F") },
            { 48, Guid.Parse("DD53B8FF-2F4D-4359-A031-9405AD384893") },
            { 61, Guid.Parse("6506FA75-9CD9-47DC-9B94-CBD55B6B6C8B") },
            { 46, Guid.Parse("7C5A4FF6-4E81-4C6C-88E9-FC75CE61A4FB") },
            { 60, Guid.Parse("E829C3D1-5243-474E-A2D6-BA35D99610C4") },
            { 59, Guid.Parse("D8049BE9-19D7-4DD8-9DC1-7D8F3886FF97") },
            { 44, Guid.Parse("790BE5CA-D07D-46C6-8FA0-9D4F5ADF388C") }
        };
        private static Dictionary<Int32, Guid> materialTypeMap = new Dictionary<int, Guid>()
        {
            { 42, Guid.Parse("19AFE679-EF94-48B4-9D6A-3C9827C4C8E2") },
            { 45, Guid.Parse("C20CE9A2-57FD-4FFB-9C12-04A3957D732A") },
            { 48, Guid.Parse("9606ACF1-8A70-4664-944B-87B3750CA7CC") },
            { 61, Guid.Parse("6549730D-EBBE-4700-8052-3DD3F390213F") },
            { 46, Guid.Parse("519AD6CD-2E07-4734-91C0-175242B387B8") },
            { 60, Guid.Parse("1185DEF1-6AD4-4995-B67A-406DB08945B5") },
            { 59, Guid.Parse("7B73529C-4C3D-4720-BB14-FDF8688F7D3B") },
            { 44, Guid.Parse("C7F4980B-E338-4363-83F7-2B3D38933E7E") }
        };

        public static readonly Guid PT_TERRITORY = Guid.NewGuid();
        public static readonly Guid PT_REGION = Guid.NewGuid();
        public static readonly Guid PT_DISTRICT = Guid.NewGuid();
        public static readonly Guid PT_DISTRICT_COUNCIL = Guid.NewGuid();
        public static readonly Guid PT_VILLAGE = Guid.NewGuid();

        private static EntityAddress MapAddress(GIIS.DataLayer.Place place)
        {
            var retVal = new EntityAddress();
            retVal.AddressUseKey = AddressUseKeys.Direct;
            if (!String.IsNullOrEmpty(place.Code))
                retVal.Component.Add(new EntityAddressComponent(AddressComponentKeys.CensusTract, place.Code));

            Queue<Guid> addressParts = new Queue<Guid>(new Guid[] {
                AddressComponentKeys.AdditionalLocator,
                AddressComponentKeys.StreetAddressLine,
                AddressComponentKeys.City,
                AddressComponentKeys.County,
                AddressComponentKeys.State,
                AddressComponentKeys.Country
            });

            // Queue places 
            Queue<GIIS.DataLayer.Place> domicileParts = new Queue<GIIS.DataLayer.Place>();
            GIIS.DataLayer.Place current = place;
            while (current != null)
            {
                domicileParts.Enqueue(current);
                current = current.Parent;
            }

            // Now trim
            while (addressParts.Count > domicileParts.Count)
                addressParts.Dequeue();

            // Now map
            while (domicileParts.Count > 0)
                retVal.Component.Add(new EntityAddressComponent(addressParts.Dequeue(), domicileParts.Dequeue().Name));
            return retVal;
        }
        /// <summary>
        /// Map a facility
        /// </summary>
        private static OpenIZ.Core.Model.Entities.ManufacturedMaterial MapMaterial(ItemLot item, DatasetInstall context)
        {
            Guid id = Guid.NewGuid();
            manufacturedMaterialMap.Add(item.Id, id);

            Guid materialId = Guid.Empty;
            if (!materialMap.TryGetValue(item.ItemId, out materialId))
            {
                materialId = Guid.NewGuid();
                Material material = new Material()
                {
                    Key = materialId,
                    ExpiryDate = item.ItemObject.ExitDate,
                    FormConceptKey = Guid.Parse(item.ItemObject.Name == "OPV" ? "66CBCE3A-2E77-401D-95D8-EE0361F4F076" : "9902267C-8F77-4233-BFD3-E6B068AB326A"),
                    DeterminerConceptKey = DeterminerKeys.Described,
                    Identifiers = new List<EntityIdentifier>()
                    {
                        new EntityIdentifier(new AssigningAuthority("TIIS_ITEM", "TIIS Item Identifiers", "1.3.6.1.4.1.33349.3.1.5.102.3.5.12"), item.ItemId.ToString())
                    },
                    Names = new List<EntityName>()
                    {
                        new EntityName(NameUseKeys.OfficialRecord, item.ItemObject.Name)
                    },
                    StatusConceptKey = item.ItemObject.IsActive ? StatusKeys.Active : StatusKeys.Obsolete
                };
                context.Action.Add(new DataUpdate() { InsertIfNotExists = true, Element = material });
                materialMap.Add(item.ItemId, materialId);
            }

            // Organization map?
            Guid organizationId = Guid.Empty;
            var gtinObject = ItemManufacturer.GetItemManufacturerByGtin(item.Gtin);
            if (gtinObject != null && !manufacturerMap.TryGetValue(gtinObject.ManufacturerId, out organizationId))
            {
                organizationId = Guid.NewGuid();
                Organization organization = new Organization()
                {
                    Key = organizationId,
                    Identifiers = new List<EntityIdentifier>()
                    {
                        new EntityIdentifier(new AssigningAuthority("MANUFACTURER_CODE", "Manufacturer Codes", "1.3.6.1.4.1.33349.3.1.5.102.3.5.14"), gtinObject .ManufacturerObject.Code),
                        new EntityIdentifier(new AssigningAuthority("TIIS_MANUFACTURER", "TIIS Manufacturer Identifiers", "1.3.6.1.4.1.33349.3.1.5.102.3.5.13"), gtinObject .ManufacturerId.ToString())
                    },
                    Names = new List<EntityName>()
                    {
                        new EntityName(NameUseKeys.OfficialRecord, gtinObject .ManufacturerObject.Name)
                    },
                    StatusConceptKey = gtinObject.ManufacturerObject.IsActive ? StatusKeys.Active : StatusKeys.Obsolete,
                    IndustryConceptKey = industryManufacturer
                };
                context.Action.Add(new DataUpdate() { InsertIfNotExists = true, Element = organization });
                manufacturerMap.Add(gtinObject.ManufacturerId, organizationId);
            }

            Guid typeConceptKey = Guid.Empty;
            materialTypeMap.TryGetValue(item.ItemId, out typeConceptKey);

            // TODO: Migrate over kit items
            // TODO: Link boxes/vials/doses
            // Core construction of place
            ManufacturedMaterial retVal = new ManufacturedMaterial()
            {
                Key = id,
                TypeConceptKey = typeConceptKey == Guid.Empty ? (Guid?)null : typeConceptKey,
                Relationships = new List<EntityRelationship>()
                {
                    new EntityRelationship(EntityRelationshipTypeKeys.ManufacturedProduct, id) { SourceEntityKey = materialId },
                },
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Assigned, String.Format("{0} ({1})", item.ItemObject.Name, gtinObject?.ManufacturerObject.Name))
                },
                ExpiryDate = item.ExpireDate,
                LotNumber = item.LotNumber,
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(new AssigningAuthority("GTIN", "GS1 Global Trade Identification Number (GTIN)", "1.3.160"), item.Gtin),
                    new EntityIdentifier(new AssigningAuthority("GIIS_ITEM_ID", "GIIS Item Identifiers", "1.3.6.1.4.1.33349.3.1.5.102.3.5.15"), item.Id.ToString())
                },
                IsAdministrative = false,
                StatusConceptKey = StatusKeys.Active
            };
            if (organizationId != Guid.Empty)
                retVal.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.WarrantedProduct, id) { SourceEntityKey = organizationId });

            return retVal;
        }


        /// <summary>
        /// Map a facility
        /// </summary>
        private static OpenIZ.Core.Model.Entities.Place MapFacility(HealthFacility hf)
        {
            Guid id = Guid.NewGuid();
            facilityMap.Add(hf.Id, id);

            // Core construction of place
            var retVal = new OpenIZ.Core.Model.Entities.Place()
            {
                Key = id,
                ClassConceptKey = EntityClassKeys.ServiceDeliveryLocation,
                TypeConceptKey = facilityTypeId[hf.TypeId],
                Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, hf.Name) },
                Identifiers = new List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>()
                {
                    new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("TIIS_FACID", "TIIS Facility Identifiers", "1.3.6.1.4.1.45219.1.3.5.5"), hf.Id.ToString()),
                    new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("TZ_FRID", "Facility Register Identifiers", "1.3.6.1.4.1.45219.1.3.5.10"), hf.Code)
                },
                StatusConceptKey = hf.IsActive ? StatusKeys.Active : StatusKeys.Nullfied,
                Extensions = new List<EntityExtension>()
                {
                    new EntityExtension() {
                        ExtensionType = new ExtensionType("http://openiz.org/extensions/contrib/tiis/isleaf", typeof(BooleanExtensionHandler)) {
                            Key = Guid.Parse("19449384-ba34-4b31-abc2-65e83032b794"),
                        },
                        ExtensionValue = hf.Leaf
                    },
                    new EntityExtension() {
                        ExtensionType = new ExtensionType("http://openiz.org/extensions/contrib/tiis/isVaccinationPoint", typeof(BooleanExtensionHandler)) {
                            Key = Guid.Parse("19449384-ba34-4b31-abc2-65e83032b743"),
                        },
                        ExtensionValue = hf.VaccinationPoint
                    },
                    new EntityExtension() {
                        ExtensionType = new ExtensionType("http://openiz.org/extensions/contrib/tiis/vaccineStore", typeof(BooleanExtensionHandler)) {
                            Key = Guid.Parse("19449384-ba34-4b31-abc2-65e83032b768"),
                        },
                        ExtensionValue = hf.VaccineStore
                    },
                    new EntityExtension()
                    {
                        ExtensionType = new ExtensionType("http://openiz.org/extensions/contrib/tiis/coldStorageCapacity", typeof(DecimalExtensionHandler))
                        {
                            Key = Guid.Parse("19449384-ba34-4b31-abc2-65e83032b79d"),
                        },
                        ExtensionValue = (Decimal)hf.ColdStorageCapacity
                    }
                },
                Tags = new List<EntityTag>()
                            {
                                new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
                            }
            };

            if (hf.ParentId != 0)
                retVal.Relationships = new List<EntityRelationship>()
                {
                    new EntityRelationship(EntityRelationshipTypeKeys.Parent, new Entity() { Key = facilityMap[hf.ParentId] })
                };

            // TODO: Fix author key needing to be present in DB
            /*
            if (!String.IsNullOrEmpty(hf.Notes))
            {
                if(!userEntityMap.ContainsKey(hf.ModifiedBy))
                    userEntityMap.Add(hf.ModifiedBy, Guid.NewGuid());
                retVal.Notes.Add(new EntityNote()
                {
                    AuthorKey = userEntityMap[hf.ModifiedBy],
                    Text = hf.Notes
                });
            }*/
            return retVal;
        }

        /// <summary>
        /// Map a facility
        /// </summary>
        private static OpenIZ.Core.Model.Entities.Place MapPlace(GIIS.DataLayer.Place plc)
        {
            Guid id = Guid.NewGuid();
            placeEntityMap.Add(plc.Id, id);

            Guid classCode = EntityClassKeys.Place, typeCode = PT_VILLAGE;
            switch (plc.Code.Length)
            {
                case 2: // EX: "TZ"
                    classCode = EntityClassKeys.Country;
                    typeCode = EntityClassKeys.Country;
                    break;
                case 5: // EX: TZ.NT
                    classCode = EntityClassKeys.Place;
                    typeCode = PT_TERRITORY;
                    break;
                case 8: // EX: TZ.NT.AS
                    classCode = EntityClassKeys.State;
                    typeCode = PT_REGION;
                    break;
                case 11: // EX: TZ.NT.AS.AM
                    classCode = EntityClassKeys.CountyOrParish;
                    typeCode = PT_DISTRICT;
                    break;
                default:
                    if (plc.Code.Length <= 14)
                    {
                        classCode = EntityClassKeys.CityOrTown;
                        typeCode = PT_DISTRICT_COUNCIL;
                    }
                    else
                    {
                        classCode = EntityClassKeys.Place;
                        typeCode = PT_VILLAGE;
                    }
                    break;
            }


            // Core construction of place
            var retVal = new OpenIZ.Core.Model.Entities.Place()
            {
                Key = id,
                ClassConceptKey = classCode,
                TypeConceptKey = typeCode,
                Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, plc.Name) },
                Identifiers = new List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>()
                {
                    new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("TIIS_PLCID", "TIIS Place Identifiers", "1.3.6.1.4.1.45219.1.3.5.12"), plc.Id.ToString()),
                    new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("TZ_FRID", "Facility Register Codes", "1.3.6.1.4.1.45219.1.3.5.11"), plc.Code)
                },
                StatusConceptKey = plc.IsActive ? StatusKeys.Active : StatusKeys.Nullfied,
                Extensions = new List<EntityExtension>()
                {
                    new EntityExtension() {
                        ExtensionType = new ExtensionType("http://openiz.org/extensions/contrib/tiis/isleaf", typeof(BooleanExtensionHandler)) {
                            Key = Guid.Parse("19449384-ba34-4b31-abc2-65e83032b794"),
                        },
                        ExtensionValue = plc.Leaf
                    }
                },
                Addresses = new List<EntityAddress>()
                {
                    MapAddress(plc)
                },
                Tags = new List<EntityTag>()
                            {
                                new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
                            }
            };

            if (plc.ParentId != 0)
                retVal.Relationships = new List<EntityRelationship>()
                {
                    new EntityRelationship(EntityRelationshipTypeKeys.Parent, new Entity() { Key = placeEntityMap[plc.ParentId] })
                };
            if (plc.HealthFacilityId != null && plc.HealthFacilityId != 0)
                retVal.Relationships = new List<EntityRelationship>()
                {
                    new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, new Entity() { Key = facilityMap[plc.HealthFacilityId.Value] })
                };
            // TODO: Fix author key needing to be present in DB
            /*
            if (!String.IsNullOrEmpty(hf.Notes))
            {
                if(!userEntityMap.ContainsKey(hf.ModifiedBy))
                    userEntityMap.Add(hf.ModifiedBy, Guid.NewGuid());
                retVal.Notes.Add(new EntityNote()
                {
                    AuthorKey = userEntityMap[hf.ModifiedBy],
                    Text = hf.Notes
                });
            }*/
            return retVal;
        }

        /// <summary>
        /// Imports the Core data
        /// </summary>
        [Description("Extracts the core data from GIIS (Places, Facilities, Materials, Lots, etc.). Note: Due to a limitation of the GIIS data layer API, there must be a configuration connection string called GIIS in the oizdt.exe.config file")]
        [ParameterClass(typeof(ConsoleParameters))]
        [Example("Extract all data from connection to a directory", @"--output=C:\data\giis")]
        public static void ImportCoreData(string[] args)
        {

            ConsoleParameters parms = new ParameterParser<ConsoleParameters>().Parse(args);

            // Concepts
            Console.WriteLine("Generating OpenIZ Concepts to support GIIS data");
            DatasetInstall conceptDataset = new DatasetInstall() { Id = "Concepts to support TIIS data", Action = new List<DataInstallAction>() };
            DataInsert healthFacilityTypes = new DataInsert()
            {
                Element = new ConceptSet()
                {
                    Key = Guid.NewGuid(),
                    Mnemonic = "HealthFacilityTypes",
                    Oid = "1.3.6.1.4.1.45219.1.3.5.99.1",
                    Name = "Health Facility Types",
                    Url = "http://ivd.moh.go.tz/valueset/timr/HealthFacilityTypes"
                },
                Association = new List<DataAssociation>()
            },
            placeTypes = new DataInsert()
            {
                Element = new ConceptSet()
                {
                    Key = Guid.NewGuid(),
                    Mnemonic = "PlaceTypes",
                    Oid = "1.3.6.1.4.1.45219.1.3.5.99.2",
                    Name = "Place Sub-Classifications",
                    Url = "http://openiz.org/valueset/timr/PlaceTypes"
                },
                Association = new List<DataAssociation>()
                {
                    new DataAssociation()
                    {
                        PropertyName = "Concepts",
                        Element = new Concept()
                        {
                            Key = PT_DISTRICT,
                            StatusConceptKey = StatusKeys.Active,
                            IsSystemConcept = false,
                            ClassKey = ConceptClassKeys.Other,
                            Mnemonic = "PlaceType-District",
                            ConceptNames = new List<ConceptName>()
                            {
                                new ConceptName()
                                {
                                    Language = "en",
                                    Name = "District"
                                }
                            }

                        }
                    },
                    new DataAssociation()
                    {
                        PropertyName = "Concepts",
                        Element = new Concept()
                        {
                            Key = PT_DISTRICT_COUNCIL,
                            StatusConceptKey = StatusKeys.Active,
                            IsSystemConcept = false,
                            ClassKey = ConceptClassKeys.Other,
                            Mnemonic = "PlaceType-DistrictCouncil",
                            ConceptNames = new List<ConceptName>()
                            {
                                new ConceptName()
                                {
                                    Language = "en",
                                    Name = "District Council"
                                }
                            }

                        }
                    },
                    new DataAssociation()
                    {
                        PropertyName = "Concepts",
                        Element = new Concept()
                        {
                            Key = PT_REGION,
                            StatusConceptKey = StatusKeys.Active,
                            IsSystemConcept = false,
                            ClassKey = ConceptClassKeys.Other,
                            Mnemonic = "PlaceType-Region",
                            ConceptNames = new List<ConceptName>()
                            {
                                new ConceptName()
                                {
                                    Language = "en",
                                    Name = "Region"
                                }
                            }

                        }
                    },
                    new DataAssociation()
                    {
                        PropertyName = "Concepts",
                        Element = new Concept()
                        {
                            Key = PT_TERRITORY,
                            StatusConceptKey = StatusKeys.Active,
                            IsSystemConcept = false,
                            ClassKey = ConceptClassKeys.Other,
                            Mnemonic = "PlaceType-Territory",
                            ConceptNames = new List<ConceptName>()
                            {
                                new ConceptName()
                                {
                                    Language = "en",
                                    Name = "Territory"
                                }
                            }

                        }
                    },
                                        new DataAssociation()
                    {
                        PropertyName = "Concepts",
                        Element = new Concept()
                        {
                            Key = PT_VILLAGE,
                            StatusConceptKey = StatusKeys.Active,
                            IsSystemConcept = false,
                            ClassKey = ConceptClassKeys.Other,
                            Mnemonic = "PlaceType-Village",
                            ConceptNames = new List<ConceptName>()
                            {
                                new ConceptName()
                                {
                                    Language = "en",
                                    Name = "Village"
                                }
                            }

                        }
                    }
                }
            };


            foreach (var itm in HealthFacilityType.GetHealthFacilityTypeList().OrderBy(o => o.Id))
            {
                facilityTypeId.Add(itm.Id, Guid.NewGuid());

                healthFacilityTypes.Association.Add(
                    new DataAssociation()
                    {
                        PropertyName = "Concepts",
                        Element = new Concept()
                        {
                            Key = facilityTypeId[itm.Id],
                            StatusConceptKey = StatusKeys.Active,
                            IsSystemConcept = false,
                            ClassKey = ConceptClassKeys.Other,
                            Mnemonic = "Facility-" + itm.Name.Replace(" ", ""),
                            ConceptNames = new List<ConceptName>()
                            {
                                new ConceptName()
                                {
                                    Language = "en",
                                    Name = itm.Name
                                }
                            }

                        }
                    }
                );
            }


            (healthFacilityTypes.Element as ConceptSet).ConceptsXml = healthFacilityTypes.Association.Select(o => o.Element.Key.Value).ToList();
            (placeTypes.Element as ConceptSet).ConceptsXml = placeTypes.Association.Select(o => o.Element.Key.Value).ToList();
            conceptDataset.Action.AddRange(healthFacilityTypes.Association.Select(o => new DataInsert() { Element = o.Element }));
            conceptDataset.Action.AddRange(placeTypes.Association.Select(o => new DataInsert() { Element = o.Element }));
            conceptDataset.Action.AddRange(new DataInstallAction[]
            {
                new DataInsert() { Element = new Concept() { Key = industryManufacturer, Mnemonic = "Industry-Manufacturing", ClassKey = ConceptClassKeys.Other, IsSystemConcept = false, StatusConceptKey = StatusKeys.Active, ConceptNames = new List<ConceptName>() { new ConceptName() { Language = "en", Name = "Manufacturing"  } }, ConceptSetsXml = new List<Guid>() { Guid.Parse("d1597e50-845a-46e1-b9ae-6f99ff93d9db") } } },
                new DataInsert() { Element = new Concept() { Key = industryOther, Mnemonic = "Industry-OtherUnknown", ClassKey = ConceptClassKeys.Other, IsSystemConcept = false, StatusConceptKey = StatusKeys.Active, ConceptNames = new List<ConceptName>() { new ConceptName() { Language = "en", Name = "Other/Unknown"  } } , ConceptSetsXml = new List<Guid>() { Guid.Parse("d1597e50-845a-46e1-b9ae-6f99ff93d9db") } } },
                new DataInsert() { Element = new Concept() { Key = industryHealthDelivery, Mnemonic = "Industry-HealthDelivery", ClassKey = ConceptClassKeys.Other, IsSystemConcept = false, StatusConceptKey = StatusKeys.Active , ConceptNames = new List<ConceptName>() { new ConceptName() { Language = "en", Name = "Healthcare"  } } , ConceptSetsXml = new List<Guid>() { Guid.Parse("d1597e50-845a-46e1-b9ae-6f99ff93d9db") } } }
            });
            healthFacilityTypes.Association.Clear();
            placeTypes.Association.Clear();
            conceptDataset.Action.Add(healthFacilityTypes);
            conceptDataset.Action.Add(placeTypes);

            // Facilities
            Console.WriteLine("Exporting GIIS Facilities to OpenIZ IMS Format");
            DatasetInstall facilityDataset = new DatasetInstall() { Action = new List<DataInstallAction>() };
            facilityDataset.Id = "Facilities from GIIS";
            foreach (var itm in HealthFacility.GetHealthFacilityList().OrderBy(o => o.Id))
                facilityDataset.Action.Add(new DataInsert()
                {
                    Element = MapFacility(itm)
                });

            // Places
            Console.WriteLine("Exporting GIIS Places to OpenIZ IMS Format");
            DatasetInstall placeDataset = new DatasetInstall() { Action = new List<DataInstallAction>() };
            placeDataset.Id = "Places from GIIS";
            foreach (var itm in GIIS.DataLayer.Place.GetPlaceList().OrderBy(o => o.ParentId))
                placeDataset.Action.Add(new DataInsert()
                {
                    Element = MapPlace(itm)
                });

            DBManager.ExecuteNonQueryCommand("UPDATE \"USER\" SET \"LASTLOGIN\" = NULL WHERE \"LASTLOGIN\" = '-infinity'", System.Data.CommandType.Text, new List<Npgsql.NpgsqlParameter>());
            DBManager.ExecuteNonQueryCommand("UPDATE \"USER\" SET \"PREVLOGIN\" = NULL WHERE \"PREVLOGIN\" = '-infinity'", System.Data.CommandType.Text, new List<Npgsql.NpgsqlParameter>());

            // Users
            Console.WriteLine("Exporting GIIS Users to OpenIZ IMS Format");
            DatasetInstall userDataset = new DatasetInstall() { Action = new List<DataInstallAction>() };
            userDataset.Id = "Users from TIIS";
            foreach (var itm in User.GetUserList())
            {
                if (userDataset.Action.Any(o => (o.Element as SecurityUser)?.UserName.Trim().ToLower() == itm.Username.Trim().ToLower()) ||
                    itm.Username.ToLower() == "administrator")
                    continue; /// Apparently user names are distinct based on case?
                Guid userId = Guid.NewGuid(), entityId = Guid.NewGuid();
                userMap.Add(itm.Id, userId);

                if (!userEntityMap.TryGetValue(itm.Id, out entityId))
                {
                    entityId = Guid.NewGuid();
                    userEntityMap.Add(itm.Id, entityId);
                }
                var securityUser = new SecurityUser()
                {
                    Key = userId,
                    UserName = itm.Username,
                    Email = itm.Email,
                    EmailConfirmed = !String.IsNullOrEmpty(itm.Email),
                    LastLoginTime = itm.Lastlogin,
                    SecurityHash = Guid.Empty.ToString(),
                    Lockout = itm.IsActive ? null : (DateTime?)DateTime.MaxValue,
                    PasswordHash = BitConverter.ToString(Convert.FromBase64String(itm.Password)).Replace("-", ""),
                    UserClass = UserClassKeys.HumanUser,
                    TwoFactorEnabled = false,
                    ObsoletionTime = itm.Deleted ? (DateTime?)DateTime.Now : null,
                    ObsoletedByKey = itm.Deleted ? (Guid?)Guid.Parse(AuthenticationContext.SystemUserSid) : null,
                };
                var userEntity = new UserEntity()
                {
                    Key = entityId,
                    Names = new List<EntityName>() { new EntityName(NameUseKeys.OfficialRecord, itm.Lastname, itm.Firstname) },

                    SecurityUserKey = userId,
                    Identifiers = new List<EntityIdentifier>()
                            {
                                new EntityIdentifier(new AssigningAuthority("TIIS_USER_ID", "TIIS User Identifiers", "1.3.6.1.4.1.45219.1.3.5.2"), itm.Id.ToString())
                            },
                    Tags = new List<EntityTag>()
                            {
                                new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
                            },
                    StatusConceptKey = itm.IsActive ? StatusKeys.Active : StatusKeys.Obsolete

                };
                if (!String.IsNullOrEmpty(itm.Email))
                    userEntity.Telecoms = new List<EntityTelecomAddress>() { new EntityTelecomAddress(TelecomAddressUseKeys.WorkPlace, itm.Email) };

                Guid facilityId = Guid.Empty;
                if (facilityMap.TryGetValue(itm.HealthFacilityId, out facilityId))
                    userEntity.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Employee, new Entity() { Key = facilityId }));

                // data element 
                var securityUserData = new DataInsert()
                {
                    Element = securityUser,
                    Association = new List<DataAssociation>()
                };

                // Role
                foreach (var r in Role.GetRolesOfUser(itm.Id))
                {
                    Guid roleId = Guid.Empty;
                    if (!roleMap.TryGetValue(r.Id, out roleId))
                    {
                        roleId = Guid.NewGuid();
                        roleMap.Add(r.Id, roleId);
                    }

                    var role = new SecurityRole()
                    {
                        Key = roleId,
                        Name = r.Name,
                        ObsoletionTime = r.IsActive ? null : (DateTime?)DateTime.Now,
                        ObsoletedByKey = r.IsActive ? null : (Guid?)Guid.Parse(AuthenticationContext.SystemUserSid)
                    };

                    // Add roles to the user
                    securityUserData.Association.Add(new DataAssociation()
                    {
                        PropertyName = "Roles",
                        Element = new SecurityRole() { Key = role.Key }
                    });

                    // Add role
                    userDataset.Action.Add(new DataInsert()
                    {
                        Element = role
                    });

                    // Vaccinator?
                    if (r.Name == "Vaccinator")
                    {

                        // Provider entity
                        var providerEntity = new Provider()
                        {
                            Key = Guid.NewGuid(),
                            Names = userEntity.Names,
                            Telecoms = userEntity.Telecoms,
                            Identifiers = userEntity.Identifiers.Select(o => new EntityIdentifier(new AssigningAuthority("PROVIDER_ID", "TImR Assigned Provider ID", "1.3.6.1.4.1.45219.1.3.5.80"), o.Value)).ToList(),
                            Tags = new List<EntityTag>()
                            {
                                new EntityTag("http://openiz.org/tags/contrib/importedData", "true")
                            },
                            StatusConceptKey = itm.IsActive ? StatusKeys.Active : StatusKeys.Obsolete
                        };
                        userDataset.Action.Add(new DataInsert() { Element = providerEntity });

                        // Create a heath care provider
                        userEntity.Relationships.Add(new EntityRelationship()
                        {
                            RelationshipTypeKey = EntityRelationshipTypeKeys.AssignedEntity,
                            TargetEntityKey = providerEntity.Key
                        });
                    }
                }


                userDataset.Action.Add(securityUserData);
                userDataset.Action.Add(new DataInsert()
                {
                    Element = userEntity
                });
            }

            
            var materialDataset = new DatasetInstall() { Id = "Manufactured Materials from GIIS" };
            foreach (var il in ItemLot.GetItemLotList())
            {
                var itm = MapMaterial(il, materialDataset);
                materialDataset.Action.Add(new DataUpdate() { InsertIfNotExists = true, Element = itm });
            }


            // Write datasets
            if (!parms.LiveMigration)
            {
                XmlSerializer xsz = new XmlSerializer(typeof(DatasetInstall));
                using (var fs = File.Create("990-tiis.concepts.dataset"))
                    xsz.Serialize(fs, conceptDataset);
                using (var fs = File.Create("991-tiis.facilities.dataset"))
                    xsz.Serialize(fs, facilityDataset);
                using (var fs = File.Create("992-tiis.places.dataset"))
                    xsz.Serialize(fs, placeDataset);
                using (var fs = File.Create("993-tiis.users.dataset"))
                    xsz.Serialize(fs, userDataset);
                using (var fs = File.Create("994-tiis.materials.dataset"))
                    xsz.Serialize(fs, materialDataset);
            }
            else
            {
                Console.WriteLine("Installing concepts...");
                var installer = new OpenIZ.Core.Persistence.DataInitializationService();
                installer.InstallDataset(conceptDataset);
                installer.InstallDataset(facilityDataset);
                installer.InstallDataset(placeDataset);
                installer.InstallDataset(userDataset);
                installer.InstallDataset(materialDataset);
            }
        }

    }
}
