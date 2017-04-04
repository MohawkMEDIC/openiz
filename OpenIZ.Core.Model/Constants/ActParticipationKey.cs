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
 * Date: 2016-8-2
 */
using System;

namespace OpenIZ.Core.Model.Constants
{
	/// <summary>
	/// Represents the participation concepts which an entity can participate in an act
	/// </summary>
	public static class ActParticipationKey
	{
		public static readonly Guid Admitter = Guid.Parse("A0174216-6439-4351-9483-A241A48029B7");

		public static readonly Guid Attender = Guid.Parse("6CBF29AD-AC51-48C9-885A-CFE3026ECF6E");

		public static readonly Guid Authenticator = Guid.Parse("1B2DBF82-A503-4CF4-9ECB-A8E111B4674E");

		public static readonly Guid Authororiginator = Guid.Parse("F0CB3FAF-435D-4704-9217-B884F757BC14");

		public static readonly Guid Baby = Guid.Parse("479896B0-35D5-4842-8109-5FDBEE14E8A4");

		public static readonly Guid Beneficiary = Guid.Parse("28C744DF-D889-4A44-BC1A-2E9E9D64AF13");

		public static readonly Guid CallbackContact = Guid.Parse("9C4C40AE-2C15-4581-A496-BE1ABFE4EB66");

		public static readonly Guid CausativeAgent = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54");

		public static readonly Guid Consultant = Guid.Parse("0A364AD7-F961-4D8A-93F0-1FD4176548B3");

		public static readonly Guid Consumable = Guid.Parse("A5CAC7F7-E3B7-4DD8-872C-DB0E7FCC2D84");

		public static readonly Guid CoverageTarget = Guid.Parse("4B5471D4-E3FE-45F7-85A2-AE2B4F224757");

		public static readonly Guid Custodian = Guid.Parse("649D6D69-139C-4006-AE45-AFF4649D6079");

		public static readonly Guid DataEnterer = Guid.Parse("C50D66D2-E5DA-4A34-B2B7-4CD4FE4EF2C4");

		public static readonly Guid Destination = Guid.Parse("727B3624-EA62-46BB-A68B-B9E49E302ECA");

		public static readonly Guid Device = Guid.Parse("1373FF04-A6EF-420A-B1D0-4A07465FE8E8");

		public static readonly Guid DirectTarget = Guid.Parse("D9F63423-BA9B-48D9-BA38-C404B784B670");

		public static readonly Guid Discharger = Guid.Parse("A2594E6E-E8FE-4C68-82A5-D3A46DBEC87D");

		public static readonly Guid Distributor = Guid.Parse("693F08FA-625A-40D2-B928-6856099C0349");

		public static readonly Guid Donor = Guid.Parse("BE1235EE-710A-4732-88FD-6E895DE7C56D");

		/// <summary>
		/// The location where the act was entered
		/// </summary>
		public static readonly Guid EntryLocation = Guid.Parse("AC05185B-5A80-47A8-B924-060DEB6D0EB2");

		public static readonly Guid Escort = Guid.Parse("727A61ED-2F35-4E09-8BB6-6D09E2BA8FEC");

		public static readonly Guid Exposure = Guid.Parse("5A6A6766-8E1D-4D36-AE50-9B7D82D8A182");

		public static readonly Guid ExposureAgent = Guid.Parse("EA60A5A9-E971-4F0D-BB5D-DC7A0C74A2C9");

		public static readonly Guid ExposureSource = Guid.Parse("CBB6297B-743C-453C-8476-BA4C10A1C965");

		public static readonly Guid ExposureTarget = Guid.Parse("EC401B5C-4C33-4229-9C72-428FC5DB37FF");

		public static readonly Guid GuarantorParty = Guid.Parse("28FB791E-179E-461A-B16C-CAC13A04BD0A");

		public static readonly Guid Holder = Guid.Parse("2452B691-F122-4121-B9DF-76D990B43F35");

		/// <summary>
		/// The entity not directly present in the act but which will be the focust of th act
		/// </summary>
		public static readonly Guid IndirectTarget = Guid.Parse("3A9F0C2F-E322-4639-A8E7-0DF67CAC761B");

		public static readonly Guid Informant = Guid.Parse("39604248-7812-4B60-BC54-8CC1FFFB1DE6");

		public static readonly Guid InformationRecipient = Guid.Parse("9790B291-B8A3-4C85-A240-C2C38885AD5D");

		public static readonly Guid LegalAuthenticator = Guid.Parse("0716A333-CD46-439D-BFD6-BF788F3885FA");

		/// <summary>
		/// The location where the service was performed
		/// </summary>
		public static readonly Guid Location = Guid.Parse("61848557-D78D-40E5-954F-0B9C97307A04");

		public static readonly Guid NonreuseableDevice = Guid.Parse("6792DB6C-FD5C-4AB8-96F5-ACE5665BDCB9");

		public static readonly Guid Origin = Guid.Parse("5D175F21-1963-4589-A400-B5EF5F64842C");

		public static readonly Guid Participation = Guid.Parse("C704A23D-86EF-4E11-9050-F8AA10919FF2");

		public static readonly Guid Performer = Guid.Parse("FA5E70A4-A46E-4665-8A20-94D4D7B86FC8");

		public static readonly Guid PrimaryInformationRecipient = Guid.Parse("02BB7934-76B5-4CC5-BD42-58570F15EB4D");

		public static readonly Guid PrimaryPerformer = Guid.Parse("79F6136C-1465-45E8-917E-E7832BC8E3B2");

		public static readonly Guid Product = Guid.Parse("99E77288-CB09-4050-A8CF-385513F32F0A");

		public static readonly Guid Receiver = Guid.Parse("53C694B8-27D8-43DD-95A4-BB318431D17C");

		public static readonly Guid RecordTarget = Guid.Parse("3F92DBEE-A65E-434F-98CE-841FEEB02E3F");

		public static readonly Guid ReferredBy = Guid.Parse("6DA3A6CA-2AB0-4D32-9588-E094F277F06D");

		public static readonly Guid ReferredTo = Guid.Parse("353F9255-765E-4336-8007-1D61AB09AAD6");

		public static readonly Guid Referrer = Guid.Parse("5E8E0F8B-BC23-4847-82AB-49B8DD79981E");

		public static readonly Guid Remote = Guid.Parse("3C1225DE-194E-49CE-A41A-0F9376B04C11");

		public static readonly Guid ResponsibleParty = Guid.Parse("64474C12-B978-4BB6-A584-46DADEC2D952");

		public static readonly Guid ReusableDevice = Guid.Parse("76990D3D-3F27-4B39-836B-BA87EEBA3328");

		/// <summary>
		/// The secondary performing person (support clinician)
		/// </summary>
		public static readonly Guid SecondaryPerformer = Guid.Parse("4FF91E06-2E39-44E3-9FBE-0D828FE318FE");

		public static readonly Guid Specimen = Guid.Parse("BCE17B21-05B2-4F02-BF7A-C6D3561AA948");

		public static readonly Guid Subject = Guid.Parse("03067700-CE37-405F-8ED3-E4965BA2F601");

		public static readonly Guid Tracker = Guid.Parse("C3BE013A-20C5-4C20-840C-D9DBB15D040E");

		/// <summary>
		/// The person who transcribed data from the original act
		/// </summary>
		public static readonly Guid Transcriber = Guid.Parse("DE3F7527-E3C9-45EF-8574-00CA4495F767");

		public static readonly Guid UgentNotificationContact = Guid.Parse("01B87999-85A7-4F5C-9B7E-892F1195CFE3");
		public static readonly Guid Verifier = Guid.Parse("F9DC5787-DD4D-42C6-A082-AC7D11956FDA");
		public static readonly Guid Via = Guid.Parse("5B0FAC74-5AC6-44E6-99A4-6813C0E2F4A9");
		public static readonly Guid Witness = Guid.Parse("0B82357F-5AE0-4543-AB8E-A33E9B315BAB");
	}
}