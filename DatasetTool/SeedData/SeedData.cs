using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MARC.Everest.DataTypes;
using System.IO;
using System.Reflection;

namespace DatasetTool.SeedData
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
                        String fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SeedData\\SeedData.xml");
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
