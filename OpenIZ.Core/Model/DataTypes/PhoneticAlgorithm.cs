/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-19
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a phonetic algorithm record in the model
    /// </summary>
    [Serializable]
    [DataContract(Name = "PhoneticAlgorithm", Namespace = "http://openiz.org/model")]
    public class PhoneticAlgorithm : IdentifiedData
    {
        // Phonetic algorithm
        private static PhoneticAlgorithm s_nullPhoneticAlgorithm = null;
        private static object s_lockObject = new object();

        /// <summary>
        /// Gets the phonetic algorithm which is the "empty" algorithm
        /// </summary>
        public static PhoneticAlgorithm EmptyAlgorithm
        {
            get
            {
                if(s_nullPhoneticAlgorithm == null)
                    lock(s_lockObject)
                        if(s_nullPhoneticAlgorithm == null)
                        {
                            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<PhoneticAlgorithm>>();
                            s_nullPhoneticAlgorithm = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(Guid.Parse("402CD339-D0E4-46CE-8FC2-12A4B0E17226")), null, true);
                        }
                return s_nullPhoneticAlgorithm;
            }
        }

        /// <summary>
        /// Gets the name of the phonetic algorithm
        /// </summary>
        [DataMember(Name = "name")]
        public String Name { get; set; }
        /// <summary>
        /// Gets the handler (or generator) for the phonetic algorithm
        /// </summary>
        [IgnoreDataMember]
        public Type Handler { get; set; }

    }
}