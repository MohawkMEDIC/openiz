using MARC.Everest.DataTypes.Primitives;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Claims
{
    /// <summary>
    /// A claim handler which validates the purpose of use claim
    /// </summary>
    public class PurposeOfUseClaimHandler : IClaimTypeHandler
    {

        private TraceSource m_traceSource = new TraceSource(OpenIzConstants.SecurityTraceSourceName);

        /// <summary>
        /// Gets the name of the claim being validated
        /// </summary>
        public string ClaimType
        {
            get
            {
                return OpenIzClaimTypes.XspaPurposeOfUseClaim;
            }
        }

        /// <summary>
        /// Validate the claim being made
        /// </summary>
        public bool Validate(IPrincipal principal, String value)
        {
            IConceptService conceptService = ApplicationContext.Current.GetService<IConceptService>();
            var claimValue = value.Split('|'); // Claim value is in FHIR token|namespace format for namespace is an OID

            try
            {
                OID namespaceOid = claimValue[1];
                var concepts = conceptService.FindConceptsByReferenceTerm(claimValue[0], namespaceOid);
                var pouConceptSet = conceptService.GetConceptSet("PurposeOfUse");
                if (pouConceptSet == null)
                    throw new ConfigurationException("PurposeOfUse concept set not found on this node");

                var pouConcept = concepts.SingleOrDefault(o => o.ConceptSets.Exists(s=>s.Key == pouConceptSet.Key));
                if (pouConcept == null)
                    throw new SecurityException("Purpose of use claim must be derived from PurposeOfUse concept set");

                return true;
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }
    }
}
