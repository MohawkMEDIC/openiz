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
 * Date: 2017-4-14
 */
using OpenIZ.Core.Applets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Applets.Services
{
    /// <summary>
    /// Represents a service which manages applets in the host environment
    /// </summary>
    public interface IAppletManagerService
    {

        /// <summary>
        /// Gets the loaded applets from the manager
        /// </summary>
        ReadonlyAppletCollection Applets { get; }

        /// <summary>
        /// Uninstall a package
        /// </summary>
        bool UnInstall(String appletId);

        /// <summary>
        /// Installs or upgrades an existing applet collection via package
        /// </summary>
        bool Install(AppletPackage package, bool isUpgrade = false);

        /// <summary>
        /// Get the specified applet manifest
        /// </summary>
        AppletManifest GetApplet(String appletId);

        /// <summary>
        /// Performs necessary loading functions for an applet
        /// </summary>
        bool LoadApplet(AppletManifest applet);

        /// <summary>
        /// Gets the installed applet package source for the specified applet
        /// </summary>
        byte[] GetPackage(String appletId);

    }
}
