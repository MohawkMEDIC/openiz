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
        AppletCollection LoadedApplets { get; }

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
