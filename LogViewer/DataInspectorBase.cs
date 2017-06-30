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
 * Date: 2017-3-31
 */
namespace LogViewer
{
    /// <summary>
    /// Data inspector
    /// </summary>
    public abstract class DataInspectorBase
    {

        /// <summary>
        /// Get the name of the inspector
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Inspect
        /// </summary>
        public abstract string Inspect(string source);

        /// <summary>
        /// Represent as string
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }
    }
}