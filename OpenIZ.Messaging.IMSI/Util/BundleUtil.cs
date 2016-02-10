using MARC.Everest.Threading;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.Util
{
    /// <summary>
    /// Bundle utility
    /// </summary>
    public static class BundleUtil
    {
        /// <summary>
        /// Create a bundle
        /// </summary>
        public static Bundle CreateBundle(IEnumerable<IdentifiedData> resourceRoot, int totalResults, int offset)
        {
            Bundle retVal = new Bundle();
            retVal.Key = Guid.NewGuid();
            retVal.Count = resourceRoot.Count();
            retVal.Offset = offset;
            retVal.TotalResults = totalResults;
            if (resourceRoot == null)
                return retVal;

            using (WaitThreadPool wtp = new WaitThreadPool())
            {
                foreach (var itm in resourceRoot)
                {
                    if (itm == null)
                        continue;
                    if (!retVal.Item.Exists(o => o.Key == itm.Key))
                    {
                        retVal.Item.Add(itm.GetLocked());
                        wtp.QueueUserWorkItem((o) => Bundle.ProcessModel(o as IdentifiedData, retVal), itm.GetLocked());
                    }
                }
                wtp.WaitOne();
            }

            return retVal;
        }
    }
}
