using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Services;

namespace OpenIZ.Core.Protocol
{
    /// <summary>
    /// Represents a care plan service that can bundle protocol acts together 
    /// based on their start/stop times
    /// </summary>
    public class SimpleCarePlanService : ICarePlanService
    {
        // Protocols
        private IClinicalProtocolRepositoryService m_repository = null;

        /// <summary>
        /// Constructs the aggregate care planner
        /// </summary>
        public SimpleCarePlanService()
        {
        }

        /// <summary>
        /// Gets the protocols
        /// </summary>
        public List<IClinicalProtocol> Protocols
        {
            get
            {
                var retVal = new List<IClinicalProtocol>();
                int c;
                foreach (var proto in this.m_repository.FindProtocol(o => !o.ObsoletionTime.HasValue, 0, null, out c))
                {
                    var protocolClass = Activator.CreateInstance(proto.HandlerClass) as IClinicalProtocol;
                    protocolClass.Load(proto);
                    retVal.Add(protocolClass);
                }
                return retVal;
            }
        }

        /// <summary>
        /// Gets or sets the repository to be used
        /// </summary>
        public IClinicalProtocolRepositoryService Repository
        {
            get
            {
                return this.m_repository;
            }
            set
            {
                this.m_repository = value;

            }
        }

        /// <summary>
        /// Create a care plan
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IEnumerable<Act> CreateCarePlan(Patient p)
        {
            var protocolActs = this.Protocols.SelectMany(o => o.Calculate(p));

            // TODO: Aggregate
            return protocolActs.ToList();
        }
    }
}
