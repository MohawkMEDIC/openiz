using OpenIZ.Core;
using OpenIZ.Core.Applets.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript
{
    public class AppletDataReferenceResolver : IDataReferenceResolver
    {
        /// <summary>
        /// Resolve asset
        /// </summary>
        public Stream Resolve(string reference)
        {
            // From loaded applets we resolve the references
            var appletManager = ApplicationServiceContext.Current.GetService(typeof(IAppletManagerService)) as IAppletManagerService;
            var itm = appletManager.LoadedApplets.SelectMany(a => a.Assets).FirstOrDefault(a => a.Name.EndsWith(reference));
            if (itm == null)
                return null;
            return new MemoryStream(appletManager.LoadedApplets.RenderAssetContent(itm));
        }

    }
}
