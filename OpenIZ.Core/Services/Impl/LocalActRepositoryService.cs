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
 * Date: 2016-8-3
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents an act repository service.
	/// </summary>
	public class LocalActRepositoryService : IActRepositoryService, IRepositoryService<Act>,
        IRepositoryService<SubstanceAdministration>,
        IRepositoryService<QuantityObservation>,
        IRepositoryService<PatientEncounter>,
        IRepositoryService<CodedObservation>,
        IRepositoryService<TextObservation>
	{
		/// <summary>
		/// Finds acts based on a specific query.
		/// </summary>
		public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No concept persistence service found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			var results = persistenceService.Query(filter, offset, count, AuthenticationContext.Current.Principal, out totalResults);

			return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
		}

		/// <summary>
		/// Finds the specified data
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Act> Find(Expression<Func<Act, bool>> query)
		{
            int tr;
            return this.Find<Act>(query, 0, null, out tr);
		}

		/// <summary>
		/// Get the specified act
		/// </summary>
		public TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			var result = persistenceService.Get<Guid>(new Identifier<Guid>(key, versionId), AuthenticationContext.Current.Principal, true);

			return businessRulesService != null ? businessRulesService.AfterRetrieve(result) : result;
		}

		/// <summary>
		/// Gets the specified data
		/// </summary>
		public Act Get(Guid key)
		{
			return this.Get<Act>(key, Guid.Empty);
		}

		/// <summary>
		/// Insert the specified act
		/// </summary>
		public TAct Insert<TAct>(TAct act) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

			act = businessRulesService != null ? businessRulesService.BeforeInsert(act) : act;

			act = persistenceService.Insert(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService != null ? businessRulesService.AfterInsert(act) : act;
		}

		/// <summary>
		/// Inserts the specified data
		/// </summary>
		public Act Insert(Act data)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsolete the specified act
		/// </summary>
		public TAct Obsolete<TAct>(Guid key) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var act = persistenceService.Get<Guid>(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (act == null)
			{
				throw new InvalidOperationException("Act not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			act = businessRulesService != null ? businessRulesService.BeforeObsolete(act) : act;

			act = persistenceService.Obsolete(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService != null ? businessRulesService.AfterObsolete(act) : act;
		}

		/// <summary>
		/// Obsoletes the specified data
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Act Obsolete(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Insert or update the specified act
		/// </summary>
		/// <param name="act"></param>
		/// <returns></returns>
		public TAct Save<TAct>(TAct act) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			try
			{
				act = businessRulesService != null ? businessRulesService.BeforeUpdate(act) : act;

				act = persistenceService.Update(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				act = businessRulesService != null ? businessRulesService.AfterUpdate(act) : act;
			}
			catch (DataPersistenceException)
			{
				act = businessRulesService != null ? businessRulesService.BeforeInsert(act) : act;

				act = persistenceService.Insert(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				act = businessRulesService != null ? businessRulesService.AfterInsert(act) : act;
			}

			return act;
		}

		/// <summary>
		/// Saves the specified data
		/// </summary>
		public Act Save(Act data)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Validates an act.
		/// </summary>
		public TAct Validate<TAct>(TAct data) where TAct : Act
		{
			// Correct author information and controlling act information
			data = data.Clean() as TAct;

			ISecurityRepositoryService userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var currentUserEntity = userService.GetUserEntity(AuthenticationContext.Current.Principal.Identity);

			if (!data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator))
			{
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
			}

			return data;
		}

        /// <summary>
        /// Find specified act
        /// </summary>
        public IEnumerable<Act> Find(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<Act>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Get sbadm
        /// </summary>
        SubstanceAdministration IRepositoryService<SubstanceAdministration>.Get(Guid key)
        {
            return this.Get<SubstanceAdministration>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<SubstanceAdministration> Find(Expression<Func<SubstanceAdministration, bool>> query)
        {
            int tr = 0;
            return this.Find<SubstanceAdministration>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<SubstanceAdministration> Find(Expression<Func<SubstanceAdministration, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<SubstanceAdministration>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public SubstanceAdministration Insert(SubstanceAdministration data)
        {
            return this.Insert<SubstanceAdministration>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public SubstanceAdministration Save(SubstanceAdministration data)
        {
            return this.Save<SubstanceAdministration>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        SubstanceAdministration IRepositoryService<SubstanceAdministration>.Obsolete(Guid key)
        {
            return this.Obsolete<SubstanceAdministration>(key);
        }


        /// <summary>
        /// Get sbadm
        /// </summary>
        QuantityObservation IRepositoryService<QuantityObservation>.Get(Guid key)
        {
            return this.Get<QuantityObservation>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<QuantityObservation> Find(Expression<Func<QuantityObservation, bool>> query)
        {
            int tr = 0;
            return this.Find<QuantityObservation>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<QuantityObservation> Find(Expression<Func<QuantityObservation, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<QuantityObservation>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public QuantityObservation Insert(QuantityObservation data)
        {
            return this.Insert<QuantityObservation>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public QuantityObservation Save(QuantityObservation data)
        {
            return this.Save<QuantityObservation>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        QuantityObservation IRepositoryService<QuantityObservation>.Obsolete(Guid key)
        {
            return this.Obsolete<QuantityObservation>(key);
        }


        /// <summary>
        /// Get sbadm
        /// </summary>
        CodedObservation IRepositoryService<CodedObservation>.Get(Guid key)
        {
            return this.Get<CodedObservation>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<CodedObservation> Find(Expression<Func<CodedObservation, bool>> query)
        {
            int tr = 0;
            return this.Find<CodedObservation>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<CodedObservation> Find(Expression<Func<CodedObservation, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<CodedObservation>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public CodedObservation Insert(CodedObservation data)
        {
            return this.Insert<CodedObservation>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public CodedObservation Save(CodedObservation data)
        {
            return this.Save<CodedObservation>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        CodedObservation IRepositoryService<CodedObservation>.Obsolete(Guid key)
        {
            return this.Obsolete<CodedObservation>(key);
        }


        /// <summary>
        /// Get sbadm
        /// </summary>
        TextObservation IRepositoryService<TextObservation>.Get(Guid key)
        {
            return this.Get<TextObservation>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<TextObservation> Find(Expression<Func<TextObservation, bool>> query)
        {
            int tr = 0;
            return this.Find<TextObservation>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<TextObservation> Find(Expression<Func<TextObservation, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<TextObservation>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public TextObservation Insert(TextObservation data)
        {
            return this.Insert<TextObservation>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public TextObservation Save(TextObservation data)
        {
            return this.Save<TextObservation>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        TextObservation IRepositoryService<TextObservation>.Obsolete(Guid key)
        {
            return this.Obsolete<TextObservation>(key);
        }


        /// <summary>
        /// Get sbadm
        /// </summary>
        PatientEncounter IRepositoryService<PatientEncounter>.Get(Guid key)
        {
            return this.Get<PatientEncounter>(key, Guid.Empty);
        }

        /// <summary>
        /// Find sbadm
        /// </summary>
        public IEnumerable<PatientEncounter> Find(Expression<Func<PatientEncounter, bool>> query)
        {
            int tr = 0;
            return this.Find<PatientEncounter>(query, 0, null, out tr);
        }

        /// <summary>
        /// Find the specified oject
        /// </summary>
        public IEnumerable<PatientEncounter> Find(Expression<Func<PatientEncounter, bool>> query, int offset, int? count, out int totalResults)
        {
            return this.Find<PatientEncounter>(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Insert SBADM
        /// </summary>
        public PatientEncounter Insert(PatientEncounter data)
        {
            return this.Insert<PatientEncounter>(data);
        }

        /// <summary>
        /// Save sbadm
        /// </summary>
        public PatientEncounter Save(PatientEncounter data)
        {
            return this.Save<PatientEncounter>(data);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        PatientEncounter IRepositoryService<PatientEncounter>.Obsolete(Guid key)
        {
            return this.Obsolete<PatientEncounter>(key);
        }

    }
}