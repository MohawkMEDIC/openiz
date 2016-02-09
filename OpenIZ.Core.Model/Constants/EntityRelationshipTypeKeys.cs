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
 * Date: 2016-1-24
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Base entity relationship type keys
    /// </summary>
    public static class EntityRelationshipTypeKeys
    {
        /// <summary>
        /// The related entity is a distributed material
        /// </summary>
        public static readonly Guid DistributedMaterial = Guid.Parse("F5547ADA-1EB9-40BB-B163-081567D869E7");
        /// <summary>
        /// The related entity is a theraputic agent
        /// </summary>
        public static readonly Guid TherapeuticAgent = Guid.Parse("D6657FDB-4EF3-4131-AF79-14E01A21FACA");
        /// <summary>
        /// The related entity is a retailed material
        /// </summary>
        public static readonly Guid RetailedMaterial = Guid.Parse("703DF8F4-B124-44C5-9506-1AB74DDFD91D");
        /// <summary>
        /// The related entity is a qualified entity
        /// </summary>
        public static readonly Guid QualifiedEntity = Guid.Parse("6521DD09-334B-4FBF-9C89-1AD5A804326C");
        /// <summary>
        /// The related entity is a next of kin
        /// </summary>
        public static readonly Guid NextOfKin = Guid.Parse("1EE4E74F-542D-4544-96F6-266A6247F274");
        public static readonly Guid AssignedEntity = Guid.Parse("77B7A04B-C065-4FAF-8EC0-2CDAD4AE372B");
        public static readonly Guid ExposedEntity = Guid.Parse("AB39087C-17D3-421A-A1E3-2DE4E0AB9FAF");
        public static readonly Guid PersonalRelationship = Guid.Parse("ABFD3FE8-9526-48FB-B366-35BACA9BD170");
        public static readonly Guid IncidentalServiceDeliveryLocation = Guid.Parse("41BAF7AA-5FFD-4421-831F-42D4AB3DE38A");
        public static readonly Guid RegulatedProduct = Guid.Parse("20E98D17-E24D-4C64-B09E-521A177CCD05");
        public static readonly Guid Affiliate = Guid.Parse("8DE7B5E7-C941-42BD-B735-52D750EFC5E6");
        public static readonly Guid Employee = Guid.Parse("B43C9513-1C1C-4ED0-92DB-55A904C122E6");
        public static readonly Guid ResearchSubject = Guid.Parse("EF597FFE-D965-4398-B55A-650530EBB997");
        public static readonly Guid LicensedEntity = Guid.Parse("B9FE057E-7F57-42EB-89D7-67C69646C0C4");
        public static readonly Guid TerritoryOfAuthority = Guid.Parse("C6B92576-1D62-4896-8799-6F931F8AB607");
        public static readonly Guid Agent = Guid.Parse("867FD445-D490-4619-804E-75C04B8A0E57");
        public static readonly Guid Access = Guid.Parse("DDC1B705-C768-4C7A-8F69-76AD4B167B40");
        public static readonly Guid AdministerableMaterial = Guid.Parse("B52C7E95-88B8-4C4C-836A-934277AFDB92");
        public static readonly Guid OwnedEntity = Guid.Parse("117DA15C-0864-4F00-A987-9B9854CBA44E");
        public static readonly Guid HeldEntity = Guid.Parse("9C02A621-8565-46B4-94FF-A2BD210989B1");
        public static readonly Guid PolicyHolder = Guid.Parse("CEC017EF-4E49-41AF-8596-ABAD1A91C9D0");
        public static readonly Guid Dependent = Guid.Parse("F28ED78F-85AB-47A1-BA08-B5051E62D6C3");
        public static readonly Guid DedicatedServiceDeliveryLocation = Guid.Parse("455F1772-F580-47E8-86BD-B5CE25D351F9");
        public static readonly Guid MaintainedEntity = Guid.Parse("77B6D8CD-05A0-4B1F-9E14-B895203BF40C");
        public static readonly Guid PlaceOfDeath = Guid.Parse("9BBE0CFE-FAAB-4DC9-A28F-C001E3E95E6E");
        public static readonly Guid Specimen = Guid.Parse("BCE17B21-05B2-4F02-BF7A-C6D3561AA948");
        public static readonly Guid UsedEntity = Guid.Parse("08FFF7D9-BAC7-417B-B026-C9BEE52F4A37");
        public static readonly Guid ManufacturedProduct = Guid.Parse("6780DF3B-AFBD-44A3-8627-CBB3DC2F02F6");
        public static readonly Guid IdentifiedEntity = Guid.Parse("C5C8B935-294F-4C90-9D81-CBF914BF5808");
        public static readonly Guid Subscriber = Guid.Parse("F31A2A5B-CE13-47E1-A0FB-D704F31547DB");
        public static readonly Guid ProgramEligible = Guid.Parse("CBE2A00C-E1D5-44E9-AAE3-D7D03E3C2EFA");
        public static readonly Guid Isolate = Guid.Parse("020C28A0-7C52-42F4-A046-DB9E329D5A42");
        public static readonly Guid Underwriter = Guid.Parse("A8FCD83F-808B-494B-8A1C-EC2C6DBC3DFA");
        public static readonly Guid SignificantOther = Guid.Parse("2EAB5298-BC83-492C-9004-ED3499246AFE");
        public static readonly Guid WarrantedProduct = Guid.Parse("639B4B8F-AFD3-4963-9E79-EF0D3928796A");
        public static readonly Guid InvestigationSubject = Guid.Parse("0C522BD1-DFA2-43CB-A98E-F6FF137968AE");
        public static readonly Guid Part = Guid.Parse("B2FEB552-8EAF-45FE-A397-F789D6F4728A");
        public static readonly Guid HealthcareProvider = Guid.Parse("6B04FED8-C164-469C-910B-F824C2BDA4F0");
    }
}
