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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenIZ.Core.Event;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Local patient repository service
	/// </summary>
	public class LocalPatientRepositoryService : IPatientRepositoryService
	{
		/// <summary>
		/// Locates the specified patient
		/// </summary>
		public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Patient>();

			var results = persistenceService.Query(predicate, AuthenticationContext.Current.Principal);

			return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
		}

		/// <summary>
		/// Finds the specified patient with query controls
		/// </summary>
		public IEnumerable<Patient> Find(Expression<Func<Patient, bool>> predicate, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Patient>();

			var results = persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);

			return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
		}

		/// <summary>
		/// Gets the specified patient
		/// </summary>
		public Patient Get(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Patient>();

			var result = persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);

			return businessRulesService != null ? businessRulesService.AfterRetrieve(result) : result;
		}

		/// <summary>
		/// Insert the specified patient
		/// </summary>
		public Patient Insert(Patient patient)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Patient>();

			if (businessRulesService != null)
			{
				patient = businessRulesService.BeforeInsert(patient);
			}

			patient = persistenceService.Insert(patient, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			patient = businessRulesService != null ? businessRulesService.AfterInsert(patient) : patient;

			var clientRegistryNotificationService = ApplicationContext.Current.GetService<IClientRegistryNotificationService>();

			clientRegistryNotificationService?.NotifyRegister(new NotificationEventArgs<Patient>(patient));

			return patient;
		}

		Patient IPatientRepositoryService.Get(Guid id, Guid versionId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Merges two patients together
		/// </summary>
		public Patient Merge(Patient survivor, Patient victim)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			var clientRegistryNotificationService = ApplicationContext.Current.GetService<IClientRegistryNotificationService>();

			clientRegistryNotificationService?.NotifyDuplicatesResolved(new NotificationEventArgs<Patient>(survivor));


			// TODO: Do this
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsoletes the specified patient
		/// </summary>
		public Patient Obsolete(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			var patient = this.Find(p => p.Key == key).FirstOrDefault();

			if (patient == null)
			{
				throw new InvalidOperationException("Patient not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Patient>();

			if (businessRulesService != null)
			{
				patient = businessRulesService.BeforeObsolete(patient);
			}

			patient = persistenceService.Obsolete(patient, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService != null ? businessRulesService.AfterObsolete(patient) : patient;
		}

		/// <summary>
		/// Save / update the patient
		/// </summary>
		public Patient Save(Patient patient)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Patient>();

			try
			{
				if (businessRulesService != null)
				{
					patient = businessRulesService.BeforeUpdate(patient);
				}

				patient = persistenceService.Update(patient, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				if (businessRulesService != null)
				{
					patient = businessRulesService.AfterUpdate(patient);
				}

				var clientRegistryNotificationService = ApplicationContext.Current.GetService<IClientRegistryNotificationService>();

				clientRegistryNotificationService?.NotifyRegister(new NotificationEventArgs<Patient>(patient));
			}
			catch (KeyNotFoundException)
			{
				if (businessRulesService != null)
				{
					patient = businessRulesService.BeforeInsert(patient);
				}

				patient = persistenceService.Insert(patient, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				if (businessRulesService != null)
				{
					patient = businessRulesService.AfterInsert(patient);
				}

				var clientRegistryNotificationService = ApplicationContext.Current.GetService<IClientRegistryNotificationService>();

				clientRegistryNotificationService?.NotifyRegister(new NotificationEventArgs<Patient>(patient));
			}

			return patient;
		}

		/// <summary>
		/// Un-merge two patients
		/// </summary>
		public Patient UnMerge(Patient patient, Guid versionKey)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No persistence service found");

			throw new NotImplementedException();
		}

		/// <summary>
		/// Validate the patient and prepare for storage
		/// </summary>
		public Patient Validate(Patient p)
		{
			p = p.Clean() as Patient; // clean up messy data

			return p;
		}
	}
}