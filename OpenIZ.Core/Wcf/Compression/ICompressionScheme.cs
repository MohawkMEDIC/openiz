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
 * Date: 2016-11-30
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Wcf.Compression
{
    /// <summary>
    /// Represents a compression scheme
    /// </summary>
    public interface ICompressionScheme
    {

        /// <summary>
        /// Get the encoding 
        /// </summary>
        string Encoding { get; }
        /// <summary>
        /// Create a stream that compresses
        /// </summary>
        Stream CreateCompressionStream(Stream underlyingStream);
        /// <summary>
        /// Create a stream that de-compresses
        /// </summary>
        Stream CreateDecompressionStream(Stream underlyingStream);
    }


}
