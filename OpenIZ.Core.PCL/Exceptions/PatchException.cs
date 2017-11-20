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
using OpenIZ.Core.Model.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Patch exception
    /// </summary>
    public class PatchException : Exception
    {

        /// <summary>
        /// The patch operation which failed
        /// </summary>
        public PatchOperation Operation { get; set; }

        /// <summary>
        /// Creates a new patch exception
        /// </summary>
        public PatchException()
        {

        }

        /// <summary>
        /// Creates a new patch exception with the specified <paramref name="message"/>
        /// </summary>
        public PatchException(string message) : base(message)
        {
        }

        /// <summary>
        /// Represents a patch exception
        /// </summary>
        public PatchException(string message, PatchOperation operation) : base(message)
        {
            this.Operation = operation;
        }


    }

    /// <summary>
    /// Represents an exceptional condition for the application of a patch with assertion
    /// </summary>
    public class PatchAssertionException : PatchException
    {

        /// <summary>
        /// Creates a new patch assertion exception
        /// </summary>
        public PatchAssertionException()
        {

        }

        /// <summary>
        /// Creates a new patch assertion
        /// </summary>
        public PatchAssertionException(string message) : base(message)
        {

        }

        /// <summary>
        /// Creates a new patch operation
        /// </summary>
        public PatchAssertionException(Object expected, Object actual, PatchOperation op) : base($"Assertion failed: {expected} expected but {actual} found at {op}", op)
        {

        }
    }
}
