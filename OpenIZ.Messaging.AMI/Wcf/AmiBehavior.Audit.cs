using MARC.HI.EHRS.SVC.Auditing.Services;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.AMI.Wcf
{
    /// <summary>
    /// AMI Behavior for auditing
    /// </summary>
    public partial class AmiBehavior
    {

        /// <summary>
        /// Create/send an audit 
        /// </summary>
        public void CreateAudit(AuditInfo audit)
        {

            // Audit the access
            var auditService = ApplicationContext.Current.GetService<IAuditorService>();
            if (auditService == null)
                throw new SecurityException("Audit service does not exist!");
            auditService.SendAudit(audit.Audit);

            // Persist this audit as well?
            var auditRepositoryService = ApplicationContext.Current.GetService<IAuditRepositoryService>();
            if (auditRepositoryService != null)
                audit.Audit = auditRepositoryService.Insert(audit.Audit);

            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NoContent;

        }
    }
}
