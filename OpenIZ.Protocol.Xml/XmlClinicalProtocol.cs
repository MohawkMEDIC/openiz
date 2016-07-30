using OpenIZ.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Core.Diagnostics;
using System.IO;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Protocol.Xml
{
    /// <summary>
    /// Clinicl protocol that is stored/loaded via XML
    /// </summary>
    public class XmlClinicalProtocol : IClinicalProtocol
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(XmlClinicalProtocol));

        /// <summary>
        /// Evaluation context
        /// </summary>
        [ThreadStatic]
        internal static Dictionary<String, IEnumerable<Act>> EvaluationContext;

        /// <summary>
        /// Default ctor
        /// </summary>
        public XmlClinicalProtocol()
        {

        }

        /// <summary>
        /// Creates a new protocol from the specified definition
        /// </summary>
        /// <param name="defn"></param>
        public XmlClinicalProtocol(ProtocolDefinition defn)
        {
            this.Definition = defn;
        }

        /// <summary>
        /// Gets or sets the definition of the protocol
        /// </summary>
        public ProtocolDefinition Definition { get; set; }

        /// <summary>
        /// Gets or sets the id of the protocol
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.Definition.Uuid;
            }
        }

        /// <summary>
        /// Gets or sets the name of the protocol
        /// </summary>
        public string Name
        {
            get
            {
                return this.Definition?.Name;
            }
        }

        /// <summary>
        /// Calculate the protocol against a atient
        /// </summary>
        public List<Act> Calculate(Patient p)
        {
            EvaluationContext = new Dictionary<string, IEnumerable<Act>>();

            this.m_tracer.TraceInfo("Calculate ({0}) for {1}...", this.Id, p);

            // Evaluate eligibility
            if(!this.Definition.When?.Evaluate(p) == false)
            {
                this.m_tracer.TraceInfo("{0} does not meet criteria for {1}", p, this.Id);
                return new List<Act>();
            }

            List<Act> retVal = new List<Act>();

            // Rules
            foreach (var rule in this.Definition.Rules)
                if (rule.When.Evaluate(p))
                {
                    var acts = rule.Then.Evaluate(p);
                    retVal.AddRange(acts);
                    EvaluationContext.Add(rule.Id, acts);
                }
                else
                    this.m_tracer.TraceInfo("{0} does not meet criteria for rule {1}.{2}", p, this.Id, rule.Uuid);

            // Assign protocol
            foreach (var itm in retVal)
                itm.Protocols.Add(new ActProtocol()
                {
                    ProtocolKey = this.Id
                });

            return retVal;
        }

        /// <summary>
        /// Get protocol data
        /// </summary>
        public Core.Model.Acts.Protocol GetProtcolData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                this.Definition.Save(ms);
                return new Core.Model.Acts.Protocol()
                {
                    HandlerClass = this.GetType(),
                    Name = this.Name,
                    Definition = ms.ToArray(),
                    Key = this.Id
                };
            }
        }

        /// <summary>
        /// Create the protocol data from the protocol instance
        /// </summary>
        public void Load(Core.Model.Acts.Protocol protocolData)
        {
            if (protocolData == null) throw new ArgumentNullException(nameof(protocolData));
            using (MemoryStream ms = new MemoryStream(protocolData.Definition))
                this.Definition = ProtocolDefinition.Load(ms);

        }

        /// <summary>
        /// Updates an existing plan
        /// </summary>
        public List<Act> Update(Patient p, List<Act> existingPlan)
        {
            throw new NotImplementedException();
        }
    }
}
