/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-6-14
 */
using MARC.Everest.Threading;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // Trace source
        private static TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.IMSI");

        /// <summary>
        /// Create a bundle
        /// </summary>
        public static Bundle CreateBundle(IEnumerable<IdentifiedData> resourceRoot, int totalResults, int offset, bool lean)
        {
            m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Creating bundle for results {0}..{1} of {2}", offset, offset + resourceRoot.Count(), totalResults);
            try
            {
                Bundle retVal = new Bundle();
                retVal.Key = Guid.NewGuid();
                retVal.Count = resourceRoot.Count();
                retVal.Offset = offset;
                retVal.TotalResults = totalResults;
                if (resourceRoot == null)
                    return retVal;

                foreach (var itm in resourceRoot)
                {
                    if (itm == null)
                        continue;
                    if (!retVal.HasTag(itm.Tag) && itm.Key.HasValue)
                    {
                        retVal.Add(itm);
                        Bundle.ProcessModel(itm.GetLocked(), retVal, !lean);
                    }
                }

                return retVal;

            }
            catch (Exception e)
            {
                m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Error building bundle: {0}", e);
                throw;
            }
        }
    }
}
