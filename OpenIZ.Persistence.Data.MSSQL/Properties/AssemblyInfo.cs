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
using MARC.HI.EHRS.SVC.Core.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("OpenIZ SQL Server Persistence")]
[assembly: AssemblyDescription("OpenImmunize Microsoft SQL Server Data Persistence")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mohawk College of Applied Arts and Technology")]
[assembly: AssemblyProduct("Open Immunize (http://openiz.org)")]
[assembly: AssemblyCopyright("Copyright (C) 2015-2017, Mohawk College of Applied Arts and Technology")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c4e96db4-d4fa-4703-a3fd-5d9c30dfbb1e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("0.9.9.*")][assembly: AssemblyInformationalVersion("Fredericton CTP1")]
[assembly: AssemblyVersion("0.9.9.*")][assembly: AssemblyInformationalVersion("Fredericton CTP1")]
[assembly: AssemblyFileVersion("0.9.9.0")]

// Plugin stuff
[assembly: AssemblyPlugin()]

// Depends: OpenIZ.Core v1.0.0.0
[assembly: AssemblyPluginDependency("OpenIZ.Core", "1.0.0.0")]