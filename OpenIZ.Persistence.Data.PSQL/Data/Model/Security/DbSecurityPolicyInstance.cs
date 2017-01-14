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
 * Date: 2016-6-14
 */
using System;



namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Security
{
	/// <summary>
	/// Represents a security policy instance which includes a link to a policy and
	/// to a decision
	/// </summary>
	public abstract class DbSecurityPolicyInstance : IDbVersionedAssociation
	{

		/// <summary>
		/// Gets or sets the type of the grant.
		/// </summary>
		/// <value>The type of the grant.</value>
		[Column("grant_type")]
		public int GrantType {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents a relationship between an entity and security policy
	/// </summary>
	[TableName("entity_security_policy")]
	public class DbEntitySecurityPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("entity_id"), Indexed(Name = "entity_security_policy_source_policy", Unique = true)]
        public int EntityId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "entity_security_policy_source_policy", Unique = true)]
        public int PolicyId
        {
            get;
            set;
        }


    }

    /// <summary>
    /// Represents a security policy applied to an act
    /// </summary>
    [TableName("act_security_policy")]
	public class DbActSecurityPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("act_id"), Indexed(Name = "act_security_policy_source_policy", Unique = true)]
        public int ActId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "act_security_policy_source_policy", Unique = true)]
        public int PolicyId
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Represents a security policy applied to a role
    /// </summary>
    [TableName("security_role_policy")]
	public class DbSecurityRolePolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("role_id"), Indexed(Name = "security_role_policy_source_policy", Unique = true)]
        public Guid RoleId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "security_role_policy_source_policy", Unique = true)]
        public Guid PolicyId
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Represents a security policy applied to an application (this is "my" data)
	/// </summary>
	[TableName("security_application_policy")]
	public class DbSecurityApplicationPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("application_id"), Indexed(Name = "security_application_policy_source_policy", Unique = true)]
        public Guid ApplicationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "security_application_policy_source_policy", Unique = true)]
        public Guid PolicyId
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Represents a security policy applied to a device
	/// </summary>
	[TableName("security_device_policy")]
	public class DbSecurityDevicePolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("device_id"), Indexed(Name = "security_device_policy_source_policy", Unique = true)]
        public Guid DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "security_device_policy_source_policy", Unique = true)]
        public Guid PolicyId
        {
            get;
            set;
        }
    }
}

