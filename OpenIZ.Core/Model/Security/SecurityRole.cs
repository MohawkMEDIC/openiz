using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Security role
    /// </summary>
    public class SecurityRole : SecurityEntity
    {

        // User delay load
        private List<SecurityUser> m_users;
        
        /// <summary>
        /// Gets or sets the name of the security role
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Description of the role
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the security users in the role
        /// </summary>
        [DelayLoad]
        public List<SecurityUser> Users {
            get
            {
                if (this.DelayLoad && this.m_users == null)
                {
                    var dataLayer = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
                    this.m_users = dataLayer.Query(u => u.Roles.Any(r => r.Key == this.Key), null).ToList();
                }
                return this.m_users;
            }
        }

    
    }
}
