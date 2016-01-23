using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Security
{
    /// <summary>
    /// Represents a local policy
    /// </summary>
    public class SqlSecurityPolicy : ILocalPolicy, IIdentifiedEntity
    {

        // Handler cache
        private static Dictionary<String, IPolicyHandler> s_handlers = new Dictionary<String, IPolicyHandler>();
        private static Object s_lockObject = new object();

        // Policy handler
        private IPolicyHandler m_handler;

        /// <summary>
        /// Create a local security policy
        /// </summary>
        public SqlSecurityPolicy(Policy policy)
        {

            this.CanOverride = policy.CanElevate;
            this.Key = policy.PolicyId;
            this.Name = policy.Name;
            this.Oid = policy.PolicyOid;
            this.IsActive = policy.ObsoletionTime == null;

            if(!s_handlers.TryGetValue(policy.HandlerClass, out this.m_handler))
            {
                Type handlerType = Type.GetType(policy.HandlerClass);
                if (handlerType == null)
                    throw new InvalidOperationException("Cannot find policy handler");
                var ci = handlerType.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                    throw new InvalidOperationException("Cannot find parameterless constructor");
                this.m_handler = ci.Invoke(null) as IPolicyHandler;
                if (this.m_handler == null)
                    throw new InvalidOperationException("Policy handler does not implement IPolicyHandler");
                lock(s_lockObject)
                    s_handlers.Add(policy.HandlerClass, this.m_handler);
            }
        }
        
        /// <summary>
        /// Gets an indicator of whether the policy can be overridden
        /// </summary>
        public bool CanOverride { get; private set; }

        /// <summary>
        /// Gets or sets the policy identifier
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets the name of the policy
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the OID of the policy
        /// </summary>
        public string Oid { get; private set; }

        /// <summary>
        /// Policy handler
        /// </summary>
        public IPolicyHandler Handler
        {
            get
            {
                return this.m_handler;
            }
        }

        /// <summary>
        /// Is active?
        /// </summary>
        public bool IsActive { get; private set; }
    }
}
