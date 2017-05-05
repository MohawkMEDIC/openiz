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
 * Date: 2016-8-14
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MARC.Everest.DataTypes;
using System.IO;
using System.Reflection;

namespace OizDevTool.SeedData
{

    
    /// <summary>
    /// Given Name / Gender pair data
    /// </summary>
    [XmlType("nameGenderPair", Namespace = "urn:ideaworks.ca:pat")]
    public class GivenNameGenderPair
    {

        /// <summary>
        /// Gender code for the pair
        /// </summary>
        [XmlAttribute("gender")]
        public string GenderCode { get; set; }

        /// <summary>
        /// Name for the pair
        /// </summary>
        [XmlText]
        public string Name { get; set; }

    }

    [XmlType("oidData", Namespace = "urn:ideaworks.ca:pat")]
    public class OidData
    {
        [XmlAttribute("name")]
        public String Name { get; set; }
        [XmlAttribute("oid")]
        public String Oid { get; set; }
    }

    /// <summary>
    /// Random seed data
    /// </summary>
    [XmlRoot("data", Namespace = "urn:ideaworks.ca:pat")]
    [XmlType("data", Namespace = "urn:ideaworks.ca:pat")]
    public class SeedData
    {
        private Random m_seed = new Random((int)DateTime.Now.Ticks);

        private static SeedData s_context;
        private static Object s_lockObject = new object();

        public static SeedData Current
        {
            get
            {
                if(s_context == null)
                    lock (s_lockObject)
                    {
                        XmlSerializer xsz = new XmlSerializer(typeof(SeedData));
                        String fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SeedData.xml");
                        using (FileStream fs = File.OpenRead(fileName))
                            s_context = xsz.Deserialize(fs) as SeedData;
                    }
                return s_context;
            }
        }

        [XmlElement("oid")]
        public OidData[] Oids { get; set; }

        /// <summary>
        /// Represents a family name
        /// </summary>
        [XmlElement("familyName")]
        public string[] FamilyNames { get; set; }

        /// <summary>
        /// Given name
        /// </summary>
        [XmlElement("givenName")]
        public GivenNameGenderPair[] GivenName { get; set; }

        [XmlElement("streetName")]
        public string[] StreetNames { get; set; }

        /// <summary>
        /// Get an oid matching the name
        /// </summary>
        internal string GetOid(string name)
        {
            return this.Oids.FirstOrDefault(o => o.Name == name).Oid;
        }

        /// <summary>
        /// Pick a random given name
        /// </summary>
        internal GivenNameGenderPair PickRandomGivenName(string gender)
        {
            var genderNames = this.GivenName.Where(o => o.GenderCode == gender).ToArray();
            int idx = this.m_seed.Next(genderNames.Length);
            return genderNames[idx];
        }

        /// <summary>
        /// Pick a random given name
        /// </summary>
        internal GivenNameGenderPair PickRandomGivenName()
        {
            int idx = this.m_seed.Next(this.GivenName.Length);
            return this.GivenName[idx];
        }

        /// <summary>
        /// Pick random family name
        /// </summary>
        internal string PickRandomFamilyName()
        {
            int idx = this.m_seed.Next(this.FamilyNames.Length);
            return this.FamilyNames[idx];
        }

    }
}
