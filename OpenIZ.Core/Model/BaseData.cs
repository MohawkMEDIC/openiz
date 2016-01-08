using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents the root of all model classes in the OpenIZ Core
    /// </summary>
    public abstract class BaseData : IIdentified<Guid>
    {
        
        /// <summary>
        /// Gets or sets the Id of the base data
        /// </summary>
        public virtual Identifier<Guid> Id
        {
            get
            {
                return new Identifier<Guid>()
                {
                    Id = this.Key
                };
            }
            set
            {
                // TODO: Compare the AA to configuration
                this.Key = value.Id;
            }
        }

        /// <summary>
        /// The internal primary key value of the entity
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Creation Time
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Obsoletion time
        /// </summary>
        public DateTimeOffset? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the user that created this base data
        /// </summary>
        public SecurityUser CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user that obsoleted this base data
        /// </summary>
        public SecurityUser ObsoletedBy { get; set; }
    }
}
