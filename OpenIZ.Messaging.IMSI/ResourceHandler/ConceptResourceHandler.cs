using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Data;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Security.Claims;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// A resource handler for a concept
    /// </summary>
    public class ConceptResourceHandler : IResourceHandler
    {
        /// <summary>
        /// Gets the resource name
        /// </summary>
        public string ResourceName {  get { return nameof(Concept); } }

        /// <summary>
        /// Get the specified instance
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            return persistenceService.Get(new Identifier<Guid>(id, versionId), null, true); // TODO: AUTH
        }
    }
}
