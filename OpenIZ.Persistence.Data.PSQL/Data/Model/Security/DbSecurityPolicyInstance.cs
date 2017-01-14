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
using PetaPoco;
using System;



namespace OpenIZ.Persistence.Data.PSQL.Data.Model.Security
{
	/// <summary>
	/// Represents a security policy instance which includes a link to a policy and
	/// to a decision
	/// </summary>
	public abstract class DbSecurityPolicyInstance : DbAssociation
	{
        /// <summary>
        /// Gets or sets the key 
        /// </summary>
        [Column("pol_inst_id")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the type of the grant.
        /// </summary>
        /// <value>The type of the grant.</value>
        [Column("pol_act")]
		public int GrantType {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("pol_id")]
        public int PolicyId
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Represents a relationship between an entity and security policy
	/// </summary>
	[TableName("ent_sec_pol_assoc_tbl")]
	public class DbEntitySecurityPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("ent_id")]
        public override Guid SourceKey
        {
            get;
            set;
        }
        

    }

    /// <summary>
    /// Represents a security policy applied to an act
    /// </summary>
    [TableName("act_pol_assoc_tbl")]
	public class DbActSecurityPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("act_id")]
        public override Guid SourceKey
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Represents a security policy applied to a role
    /// </summary>
    [TableName("sec_rol_pol_assoc_tbl")]
	public class DbSecurityRolePolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("role_id")]
        public override Guid SourceKey
        {
            get;
            set;
        }

       
    }

	/// <summary>
	/// Represents a security policy applied to an application (this is "my" data)
	/// </summary>
	[TableName("sec_app_pol_assoc_tbl")]
	public class DbSecurityApplicationPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("app_id")]
        public override Guid SourceKey
        {
            get;
            set;
        }
        
    }

	/// <summary>
	/// Represents a security policy applied to a device
	/// </summary>
	[TableName("sec_dev_pol_assoc_tbl")]
	public class DbSecurityDevicePolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("dev_id")]
        public override Guid SourceKey
        {
            get;
            set;
        }


    }
}

