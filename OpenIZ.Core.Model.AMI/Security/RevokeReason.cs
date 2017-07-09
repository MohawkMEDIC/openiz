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
 * User: khannan
 * Date: 2016-8-2
 */
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Security
{
	/// <summary>
	/// The reason something is revoked
	/// </summary>
	[XmlType(nameof(RevokeReason), Namespace = "http://openiz.org/ami")]
	public enum RevokeReason : uint
	{
		[XmlEnum("UNSPECIFIED")]
		Unspecified = 0x0,

		[XmlEnum("KEY COMPROMISED")]
		KeyCompromise = 0x1,

		[XmlEnum("CA COMPROMISED")]
		CaCompromise = 0x2,

		[XmlEnum("AFFILIATION CHANGED")]
		AffiliationChange = 0x3,

		[XmlEnum("SUPERSEDED")]
		Superseded = 0x4,

		[XmlEnum("CESSATION OF OPERATION")]
		CessationOfOperation = 0x5,

		[XmlEnum("CERTIFICATE ON HOLD")]
		CertificateHold = 0x6,

		[XmlEnum("REINSTANTIATE")]
		Reinstate = 0xFFFFFFFF
	}
}