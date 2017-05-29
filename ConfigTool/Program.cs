using MARC.HI.EHRS.SVC.Configuration;
using MARC.HI.EHRS.SVC.Configuration.Data;
using MARC.HI.EHRS.SVC.Configuration.UI;
using MARC.HI.EHRS.SVC.Core.Configuration;
using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Attempt to open a restricted resource (see if we're administrator)
            frmSplash splash = new frmSplash();
            splash.Show();
            try
            {

                StringBuilder argString = new StringBuilder();

#if !DEBUG
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (Environment.OSVersion.Platform == PlatformID.Win32NT && 
                    !principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    string cmdLine = Environment.CommandLine.Substring(Environment.CommandLine.IndexOf(".exe") + 4);
                    cmdLine = cmdLine.Contains(' ') ? cmdLine.Substring(cmdLine.IndexOf(" ")) : null;
                    ProcessStartInfo psi = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, cmdLine);
                    psi.Verb = "runas";
                    Trace.TraceInformation("Not administrator!");
                    Process proc = Process.Start(psi);
                    Application.Exit();
                    return;
                }
#endif 

                // Scan for configuration options
                ConfigurationApplicationContext.Initialize();
                splash.Close();
#if DEBUG
                ConfigurationApplicationContext.s_configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "OpenIZ.exe.config.test");
#else
                ConfigurationApplicationContext.s_configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "OpenIZ.exe.config");
#endif

                ParameterParser<ConsoleParameters> parser = new ParameterParser<ConsoleParameters>();

                var consoleParms = parser.Parse(args);

                if (consoleParms.ListDeploy)
                {
                    StringBuilder options = new StringBuilder("Available deployment modules: \r\n");
                    foreach (var pnl in ConfigurationApplicationContext.s_configurationPanels.FindAll(o => o.AlwaysConfigure))
                        options.AppendFormat("{0}\r\n", pnl.Name);
                    MessageBox.Show(options.ToString());
                }
                else if (consoleParms.Deploy != null && consoleParms.Deploy.Count > 0)
                    try
                    {
                        DeployUtil.Deploy(consoleParms.Deploy, consoleParms.Options);
                        ConfigurationApplicationContext_ConfigurationApplied(null, EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format("Could not deploy requested component : {0}", e), "Error Deploying");
                    }
                else
                {
                    ConfigurationApplicationContext.s_configurationPanels.Sort((a, b) => a.Name.CompareTo(b.Name));
                    ConfigurationApplicationContext.ConfigurationApplied += new EventHandler(ConfigurationApplicationContext_ConfigurationApplied);


                    // Configuration File exists?
                    if (!File.Exists(ConfigurationApplicationContext.s_configFile))
                    {
                        frmStartScreen start = new frmStartScreen();
                        if (start.ShowDialog() == DialogResult.Cancel)
                            return;
                    }

                    Application.Run(new frmMain());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                splash.Dispose();
            }
        }

        /// <summary>
        /// Configuration has been applied
        /// </summary>
        static void ConfigurationApplicationContext_ConfigurationApplied(object sender, EventArgs e)
        {
            if (ServiceTools.ServiceInstaller.ServiceIsInstalled("OpenIZ Host Process") &&
                ServiceTools.ServiceInstaller.GetServiceStatus("OpenIZ Host Process") == ServiceTools.ServiceState.Starting &&
                MessageBox.Show("The OpenIZ Host Process service must be restarted for these changes to take affect, restart it now?", "Confirm Service Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ServiceTools.ServiceInstaller.StopService("OpenIZ Host Process");
                while (ServiceTools.ServiceInstaller.GetServiceStatus("OpenIZ Host Process") == ServiceTools.ServiceState.Starting)
                    Thread.Sleep(200);
                ServiceTools.ServiceInstaller.StartService("OpenIZ Host Process");
                while (ServiceTools.ServiceInstaller.GetServiceStatus("OpenIZ Host Process") != ServiceTools.ServiceState.Starting)
                    Thread.Sleep(200);

            }
        }

        
    }
}
