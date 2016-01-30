using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Cache item in the object cache
    /// </summary>
    public struct CacheItem
    {

        /// <summary>
        /// Creates a new cache item
        /// </summary>
        public CacheItem(IdentifiedData modelItem, Object domainItem)
        {
            this.ModelInstance = modelItem;
            this.DomainItem = domainItem;
        }

        /// <summary>
        /// Gets or sets the model class
        /// </summary>
        public IdentifiedData ModelInstance { get; set; }
        /// <summary>
        /// Domain class
        /// </summary>
        public Object DomainItem { get; set; }
    }
}
