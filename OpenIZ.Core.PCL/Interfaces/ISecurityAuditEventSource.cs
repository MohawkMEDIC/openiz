using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Interfaces
{
    /// <summary>
    /// Arguments that related to a security event
    /// </summary>
    public class SecurityAuditDataEventArgs : AuditDataEventArgs
    {
        /// <summary>
        /// Security properties that changed
        /// </summary>
        public IEnumerable<String> ChangedProperties { get; set; }

        /// <summary>
        /// Creates new security data event args
        /// </summary>
        public SecurityAuditDataEventArgs(Object obj, params string[] properties) : base(obj)
        {
            this.ChangedProperties = properties;
        }
    }
    /// <summary>
    /// Security audit event source
    /// </summary>
    public interface ISecurityAuditEventSource : IAuditEventSource
    {

        /// <summary>
        /// Fired when security attributes related to an object change
        /// </summary>
        event EventHandler<SecurityAuditDataEventArgs> SecurityAttributesChanged;

    }
}
