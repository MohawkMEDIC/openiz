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
 * Date: 2017-1-22
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.OrmLite.Attributes;
using OpenIZ.Persistence.Data.ADO.Data.Model.Security;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Alerts
{
	/// <summary>
	/// Represents an alert recipient.
	/// </summary>
	[Table("alrt_rcpt_to_tbl")]
	public class DbAlertRcptTo : DbAssociation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbAlertRcptTo"/> class.
		/// </summary>
		public DbAlertRcptTo()
		{
			
		}

		public DbAlertRcptTo(Guid alertId, Guid userId)
		{
			this.Key = alertId;
			this.SourceKey = userId;
		}

		/// <summary>
		/// Gets or sets the key of the object.
		/// </summary>
		[Column("alrt_id"), ForeignKey(typeof(DbAlertMessage), nameof(DbAlertMessage.Key))]
		public override Guid Key { get; set; }

		/// <summary>
		/// Gets or sets the key of the item associated with this object.
		/// </summary>
		[Column("usr_id"), ForeignKey(typeof(DbSecurityUser), nameof(DbSecurityUser.Key))]
		public override Guid SourceKey { get; set; }
	}
}
