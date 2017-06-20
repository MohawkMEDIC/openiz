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
using MARC.HI.EHRS.SVC.Auditing.Data;

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
		/// <param name="audit">The audit.</param>
		/// <exception cref="System.Security.SecurityException">Audit service does not exist!</exception>
		public void CreateAudit(AuditInfo audit)
        {
            // Audit the access
            var auditService = ApplicationContext.Current.GetService<IAuditorService>();
            if (auditService == null)
                throw new SecurityException("Audit service does not exist!");

			ApplicationContext.Current.GetService<IThreadPoolService>()?.QueueNonPooledWorkItem(o =>
			{
				var adt = o as AuditInfo;

				adt.Audit.ForEach(a => auditService.SendAudit(a));

				// Persist this audit as well?
				var auditRepositoryService = ApplicationContext.Current.GetService<IAuditRepositoryService>();

				if (auditRepositoryService != null)
				{
					adt.Audit = adt.Audit.Select(a => auditRepositoryService.Insert(a)).ToList();
				}
			}, audit);

	        WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NoContent;

        }
    }
}
