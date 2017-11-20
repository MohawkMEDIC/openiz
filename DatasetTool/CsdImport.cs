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
 * Date: 2017-4-8
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Persistence;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OizDevTool
{
    /// <summary>
    /// Represents a CSD import utility.
    /// </summary>
    [Description("Care Services Discovery (CSD) tooling")]
    public partial class CsdImport
    {
        /// <summary>
        /// The address component code system.
        /// </summary>
        private const string AddressComponentCodeSystem = "urn:ihe:iti:csd:2013:address";

        /// <summary>
        /// The address type code system.
        /// </summary>
        private const string AddressTypeCodeSystem = "urn:ihe:iti:csd:2013:addressType";

        /// <summary>
        /// The imported data tag.
        /// </summary>
        private const string ImportedDataTag = "http://openiz.org/tags/contrib/importedData";

        /// <summary>
        /// The program exit message.
        /// </summary>
        private const string ProgramExitMessage = "Unable to continue import CSD document, press any key to exit.";

        /// <summary>
        /// The concept keys.
        /// </summary>
        private static readonly Dictionary<CompositeKey, Guid> conceptKeys = new Dictionary<CompositeKey, Guid>();

        /// <summary>
        /// The related entities.
        /// </summary>
        private static readonly Dictionary<string, Entity> entityMap = new Dictionary<string, Entity>();

        /// <summary>
        /// The emergency message.
        /// </summary>
        private static string emergencyMessage;


        /// <summary>
        /// Initializes a new instance of the <see cref="CsdImport"/> class.
        /// </summary>
        public CsdImport()
        {
        }

        /// <summary>
        /// Imports the CSD.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [ParameterClass(typeof(CsdOptions))]
        [Description("Converts a Care Services Discovery (CSD) export to DATASET import file")]
        [Example("Import a Care Services Discovery (CSD) export to a DATASET import file", "--tool=CsdImport --operation=ImportCsd --file=CSD-Organizations-Connectathon-20150120.xml --live")]
        public static void ImportCsd(string[] args)
        {
            ApplicationContext.Current.Start();

            ShowInfoMessage("Adding service providers...");

            ApplicationContext.Current.AddServiceProvider(typeof(LocalEntityRepositoryService));
            ApplicationContext.Current.AddServiceProvider(typeof(LocalMetadataRepositoryService));
            ApplicationContext.Current.AddServiceProvider(typeof(LocalConceptRepositoryService));

            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.SystemPrincipal);

            ApplicationContext.Current.Started += (o, e) =>
            {
                emergencyMessage = ApplicationContext.Current.GetLocaleString("01189998819991197253");
            };

            var parameters = new ParameterParser<CsdOptions>().Parse(args);

            var csdDatasetInstall = new DatasetInstall("HFR via CSD, Organizations, Places, Providers, Services");

            var actions = new List<DataInstallAction>();

            var serializer = new XmlSerializer(typeof(CSD));

            var fileInfo = new FileInfo(parameters.File);

            ShowInfoMessage($"Loading file: {fileInfo.Name}...");

            var csd = (CSD)serializer.Deserialize(new StreamReader(parameters.File));

            ShowInfoMessage($"File: {fileInfo.Name} loaded successfully, starting mapping process...");

            int limit = Int32.MaxValue;
            if (!String.IsNullOrEmpty(parameters.Limit))
                limit = Int32.Parse(parameters.Limit);

            if (parameters.RelationshipsOnly)
            {
                int idx = 0;
                foreach(var fac in csd.facilityDirectory.Take(limit))
                {
                    var place = GetOrCreateEntity<Place>(fac.entityID, parameters.EntityUidAuthority, parameters);
                    var rels = CreateEntityRelationships(place, fac, csd.organizationDirectory, parameters);
                    actions.AddRange(rels.Select(o => new DataUpdate()
                    {
                        Element = o, 
                        InsertIfNotExists = true,
                        IgnoreErrors = true
                    }));

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Mapped place: ({idx++}/{csd.facilityDirectory.Take(limit).Count()}) {place.Key.Value} {string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value))}");
                    Console.ResetColor();
                }
            }
            else
            {

                var stopwatch = new Stopwatch();

                stopwatch.Start();

                // map organizations
                var organizations = !parameters.SkipOrganizations ? MapOrganizations(csd.organizationDirectory.Take(limit), parameters).Select(o => new DataUpdate
                {
                    InsertIfNotExists = true,
                    Element = o
                }) : new List<DataUpdate>();

                actions.AddRange(organizations);

                // map places
                var places = !parameters.SkipFacilties ? MapPlaces(csd.facilityDirectory.Take(limit), csd.organizationDirectory, parameters).Select(p => new DataUpdate
                {
                    InsertIfNotExists = true,
                    Element = p
                }) : new List<DataUpdate>();

                actions.AddRange(places);


                // map providers
                var providers = MapProviders(csd.providerDirectory.Take(limit), parameters).Select(p => new DataUpdate
                {
                    InsertIfNotExists = true,
                    Element = p
                });

                actions.AddRange(providers);

                // map services
                var services = MapServices(csd.serviceDirectory.Take(limit)).Select(s => new DataUpdate
                {
                    InsertIfNotExists = true,
                    Element = s
                });

                actions.AddRange(services);

                stopwatch.Stop();

                ShowPerformanceMessage($"Mapped {places.Count()} Places, {providers.Count()} Providers, {organizations.Count()} Organizations, and {services.Count()} Services in {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds");

            }


            if (parameters.RelationshipsOnly)
            {
                csdDatasetInstall.Action = actions;
            }
            else
            {
                var entities = new List<Entity>();
                var relationships = new List<EntityRelationship>();
                foreach (var entity in actions.Where(a => a.Element is Entity).Select(c => c.Element as Entity))
                {
                    entity.Relationships.ForEach(o => o.SourceEntityKey = entity.Key);
                    relationships.AddRange(entity.Relationships);

                    // HACK: clear the entity relationships because we are going to import them separately
                    entity.Relationships.Clear();

                    entities.Add(entity);
                }

                // add entities to the list of items to import
                csdDatasetInstall.Action.AddRange(entities.Select(e => new DataUpdate
                {
                    InsertIfNotExists = true,
                    Element = e
                }).ToList());

                // add relationships to the list of items to import
                csdDatasetInstall.Action.AddRange(relationships.Select(e => new DataUpdate
                {
                    InsertIfNotExists = true,
                    Element = e
                }).ToList());
            }
               
            csdDatasetInstall.Action = csdDatasetInstall.Action.Distinct(new EntityComparison()).ToList();

            if (parameters.RelationshipsOnly)
                csdDatasetInstall.Action.RemoveAll(o => !(o.Element is EntityRelationship) || (o.Element as EntityRelationship).RelationshipTypeKey == EntityRelationshipTypeKeys.Parent);

            serializer = new XmlSerializer(typeof(DatasetInstall));
            
            var filename = $"999-CSD-import-{fileInfo.Name}.dataset";

            using (var fileStream = File.Create(filename))
            {
                serializer.Serialize(fileStream, csdDatasetInstall);
            }

            ShowInfoMessage($"Dataset file created: {filename}");

            if (parameters.Live)
            {
                ShowWarningMessage("Warning, the live flag is set to true, data will be imported directly into the database");

                ShowInfoMessage("Starting live import");

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var bundle = new Bundle
                {
                    Item = actions.Select(a => a.Element).ToList()
                };

                var bundlePersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();

                ShowInfoMessage("Importing data directly into the database...");

                bundlePersistenceService.Insert(bundle, AuthenticationContext.SystemPrincipal, TransactionMode.Commit);

                ShowInfoMessage("The CSD live import is now complete");

                stopwatch.Stop();

                ShowPerformanceMessage($"Imported {bundle.Item.OfType<Place>().Count()} Places, {bundle.Item.OfType<Provider>().Count()} Providers, {bundle.Item.OfType<Organization>().Count()} Organizations, and {bundle.Item.OfType<PlaceService>().Count()} Services in {stopwatch.Elapsed.Minutes} minutes and {stopwatch.Elapsed.Seconds} seconds");
            }
        }

        /// <summary>
        /// Looks up the entity by entity identifier. This will also create a new entity if one is not found.
        /// </summary>
        /// <typeparam name="T">The type of entity to lookup.</typeparam>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>Returns the entity instance.</returns>
        private static T GetOrCreateEntity<T>(string entityId, String authorityName, CsdOptions options) where T : Entity, new()
        {
            Entity entity;

            if (entityMap.TryGetValue(entityId, out entity))
            {
                return entity as T;
            }

            var entityService = ApplicationContext.Current.GetService<IDataPersistenceService<T>>();

            int totalResults = 0;

            if (!options.NoDbCheck)
                entity = entityService.Query(c => c.Identifiers.Any(i => i.Authority.DomainName == authorityName && i.Value == entityId) && c.ObsoletionTime == null, 0, 1, AuthenticationContext.SystemPrincipal, out totalResults).FirstOrDefault();

            if (totalResults > 1)
            {
                ShowWarningMessage($"Warning, found multiple entities with the same entityID: '{entityId}', will default to: '{entity.Key.Value}' {Environment.NewLine}");
            }

            if (entity != null || options.NoCreate)
            {
                return (T)entity;
            }

            ShowWarningMessage("Warning, ENTITY NOT FOUND, will create one");

            // setup basic properties of the entity instance
            entity = new T
            {
                CreationTime = DateTimeOffset.Now,
                Key = Guid.NewGuid(),
                StatusConceptKey = StatusKeys.Active,
                Tags = new List<EntityTag>
                {
                    new EntityTag(ImportedDataTag, "true")
                }
            };

            entity.Identifiers.Add(new EntityIdentifier(authorityName, entityId));

            return (T)entity;
        }

        /// <summary>
        /// Reconciles the versioned associations.
        /// </summary>
        /// <param name="existingAssociations">The existing associations.</param>
        /// <param name="newAssociations">The new associations.</param>
        /// <returns>Returns a list of version association items which are new, i.e. not a part of the existing associations.</returns>
        private static IEnumerable<VersionedAssociation<Entity>> ReconcileVersionedAssociations(IEnumerable<VersionedAssociation<Entity>> existingAssociations, IEnumerable<VersionedAssociation<Entity>> newAssociations)
        {
            // if there are no existing associations, we can just return the new associations, as items to be added
            if (!existingAssociations.Any())
            {
                return newAssociations;
            }

            // if there are no new associations, we can just return the empty list of new associations
            if (!newAssociations.Any())
            {
                return newAssociations;
            }

            return (from newAssociation
                    in newAssociations
                    from existingAssociation
                    in existingAssociations
                    where !existingAssociation.SemanticEquals(newAssociation)
                    select newAssociation).ToList();
        }

        /// <summary>
        /// Exits the application, when an entity is not found.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void ShowErrorOnNotFound(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.WriteLine(ProgramExitMessage);
            Console.ResetColor();
            Console.ReadKey();
            Environment.Exit(999);
        }

        /// <summary>
        /// Prints an informational message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void ShowInfoMessage(string message)
        {
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine($"{message} {Environment.NewLine}");
            //Console.ResetColor();
        }

        /// <summary>
        /// Prints a performance message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void ShowPerformanceMessage(string message)
        {
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine($"{message} {Environment.NewLine}");
            //Console.ResetColor();
        }

        /// <summary>
        /// Prints a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void ShowWarningMessage(string message)
        {
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine(message);
            //Console.WriteLine($"{message} {Environment.NewLine}");
            //Console.ResetColor();
        }

        /// <summary>
        /// Prints a warning when an entity is not found.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="defaultValueName">Default name of the value.</param>
        /// <param name="defaultValue">The default value.</param>
        private static void ShowWarningOnNotFound(string message, string defaultValueName, Guid defaultValue)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.WriteLine($"Defaulting to {defaultValueName} {defaultValue} {Environment.NewLine}");
            Console.ResetColor();
        }

        /// <summary>
        /// Represents CSD options.
        /// </summary>
        private class CsdOptions
        {
            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            [Parameter("file")]
            [Description("The path to the CSD file")]
            public string File { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="CsdOptions"/> is live.
            /// </summary>
            /// <value><c>true</c> if true, data is directly imported into the database vs generating dataset files to be imported at a later date; otherwise, <c>false</c>.</value>
            [Parameter("live")]
            [Description("Directly import data into the database vs generating dataset files to import at a later date")]
            public bool Live { get; set; }

            /// <summary>
            /// Gets or sets the entity UID authority
            /// </summary>
			[Description("Sets the authority of the entity UUID")]
            [Parameter("entityAuthority")]
            public String EntityUidAuthority { get; set; }

            /// <summary>
            /// Organizations as places
            /// </summary>
            [Description("When true, interprets organizations as places")]
            [Parameter("orgsAsPlaces")]
            public bool OrganizationsAsPlaces { get; set; }

            /// <summary>
            /// Cascade assigned facilities
            /// </summary>
            [Description("Cascade assigned facilities to children")]
            [Parameter("cascadeFacilities")]
            public bool CascadeAssignedFacilities { get; internal set; }

            /// <summary>
            /// Use hacked facility parent locatio nalgorithm
            /// </summary>
            [Description("Used hacked facility parent location algorithm")]
            [Parameter("hack-parentFind")]
            public bool HackParentFind { get; set; }

            /// <summary>
            /// Parent org code
            /// </summary>
            [Description("Parent Code Levels for the hacked parent")]
            [Parameter("parent-orgCode")]
            public string ParentOrgCode { get; set; }

            /// <summary>
            /// Parent org code
            /// </summary>
            [Description("Code of parent facility object which classifies as parent")]
            [Parameter("parent-facType")]
            public StringCollection ParentCodeType { get; set; }

            /// <summary>
            /// Set facility type extension
            /// </summary>
            [Description("Sets the type of the facility type extension")]
            [Parameter("type-extension")]
            public string FacilityTypeExtension { get; set; }

            /// <summary>
            /// Don't check db
            /// </summary>
            [Description("Does not check the db for existing items")]
            [Parameter("nodb")]
            public bool NoDbCheck { get; internal set; }

            /// <summary>
            /// Skip import of organizations
            /// </summary>
            [Parameter("skip-orgs")]
            [Description("Skip import of organizations")]
            public bool SkipOrganizations { get; set; }

            /// <summary>
            /// True if the import should ignore facilities
            /// </summary>
            [Parameter("skip-facilities")]
            [Description("Skip import of facilities")]
            public bool SkipFacilties { get; set; }

            /// <summary>
            /// Only scans for new relationships
            /// </summary>
            [Parameter("relsOnly")]
            [Description("Relationships only")]
            public bool RelationshipsOnly { get; set; }

            /// <summary>
            /// Only scans for new relationships
            /// </summary>
            [Parameter("noCreate")]
            [Description("Don't create new objects")]
            public bool NoCreate { get; set; }

            /// <summary>
            /// Limit
            /// </summary>
            [Parameter("limit")]
            [Description("Limit the number of facilities processed (for debugging)")]
            public String Limit { get; set; }

        }
    }

    /// <summary>
    /// Compares entities
    /// </summary>
    internal class EntityComparison : IEqualityComparer<DataInstallAction>
    {
        public bool Equals(DataInstallAction x, DataInstallAction y)
        {
            return x.Element?.Key == y.Element?.Key;
        }

        public int GetHashCode(DataInstallAction obj)
        {
            if (obj.Element?.Key.HasValue == true)
                return (int)obj.Element?.Key.GetHashCode();
            else
                return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a composite key.
    /// </summary>
    internal class CompositeKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeKey" /> class.
        /// </summary>
        public CompositeKey()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeKey"/> class.
        /// </summary>
        /// <param name="firstKey">The first key.</param>
        /// <param name="secondKey">The second key.</param>
        public CompositeKey(string firstKey, string secondKey)
        {
            this.FirstKey = firstKey;
            this.SecondKey = secondKey;
        }

        /// <summary>
        /// Gets or sets the first key.
        /// </summary>
        /// <value>The first key.</value>
        public string FirstKey { get; }

        /// <summary>
        /// Gets or sets the second key.
        /// </summary>
        /// <value>The second key.</value>
        public string SecondKey { get; }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(CompositeKey left, CompositeKey right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(CompositeKey left, CompositeKey right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            return left?.FirstKey == right?.FirstKey && left?.SecondKey == right?.SecondKey;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as CompositeKey;

            if (other == null)
            {
                return false;
            }

            return this.FirstKey == other.FirstKey && this.SecondKey == other.SecondKey;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.FirstKey.GetHashCode() ^ this.SecondKey.GetHashCode();
        }
    }
}