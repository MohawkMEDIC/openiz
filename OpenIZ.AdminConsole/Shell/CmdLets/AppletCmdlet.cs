using MohawkCollege.Util.Console.Parameters;
using OpenIZ.AdminConsole.Attributes;
using OpenIZ.Messaging.AMI.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Shell.CmdLets
{
    /// <summary>
    /// Applet commands
    /// </summary>
    [AdminCommandlet]
    public static class AppletCmdlet
    {

        /// <summary>
        /// Represents an applet parameter
        /// </summary>
        public class AppletParameter
        {

            /// <summary>
            /// Applet identifier
            /// </summary>
            [Parameter("*")]
            [Description("The identifier for the applet to stat")]
            public StringCollection AppletId { get; set; }

        }

        // Ami client
        private static AmiServiceClient m_client = new AmiServiceClient(ApplicationContext.Current.GetRestClient(Core.Interop.ServiceEndpointType.AdministrationIntegrationService));

        /// <summary>
        /// List applets
        /// </summary>
        [AdminCommand("lsapp", "Lists all applets installed on the server")]
        public static void ListApplets()
        {
            var applets = m_client.GetApplets();
            Console.WriteLine("ID{0}Name{1}Ver", new String(' ', 23), new String(' ', 36), new String(' ', 21));
            foreach(var itm in applets.CollectionItem)
            {
                var name = itm.AppletInfo.GetName("en", true);
                Console.WriteLine("{0}{1}{2}{3}{4}",
                    itm.AppletInfo.Id,
                    new String(' ', 25 - itm.AppletInfo.Id.Length),
                    name.Length > 38 ? name.Substring(0, 38) : name,
                    new String(' ', name.Length > 38 ? 2 : 40 - name.Length),
                    itm.AppletInfo.Version);
            }
        }

        /// <summary>
        /// Get a specific applet information
        /// </summary>
        [AdminCommand("appinfo", "Get applet information")]
        public static void GetApplet(AppletParameter parms)
        {

            foreach (var itm in parms.AppletId)
            {
                var applet = m_client.GetApplet(itm);

                Console.WriteLine("====== <START {0}>\r\nKey Token: {1}\r\nName: {2}\r\nAuthor: {3}\r\nVersion: {4}\r\nHash: {6}\r\nDependencies:",
                    applet.AppletInfo.Id, applet.AppletInfo.PublicKeyToken, applet.AppletInfo.GetName("en", true), applet.AppletInfo.Author, applet.AppletInfo.Version, BitConverter.ToString(applet.AppletInfo.Signature).Replace("-",""), BitConverter.ToString(applet.AppletInfo.Hash).Replace("-", ""));
                foreach (var i in applet.AppletInfo.Dependencies)
                    Console.WriteLine("\t- {0} (>= {1})", i.Id, i.Version);

                if(applet.PublisherData != null)
                {
                    Console.WriteLine("Signature:\r\n\tIssued By: {0}\r\n\tIssued To: {1}\r\n\tNot Before: {2}\r\n\tExpiration: {3}", applet.PublisherData.Issuer, applet.PublisherData.Subject, applet.PublisherData.NotBefore, applet.PublisherData.NotAfter);
                }
                Console.WriteLine("====== <END {0}>", applet.AppletInfo.Id);
            }


        }
    }
}
