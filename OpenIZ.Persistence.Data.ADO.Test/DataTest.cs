using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Configuration;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Persistence.Data.ADO.Services;

namespace OpenIZ.Persistence.Data.ADO.Test
{
    /// <summary>
    /// Represents an abstract data test tool
    /// </summary>
    public abstract class DataTest
    {
        public static class DataTestUtil
        {
            static bool started = false;

            /// <summary>
            /// Start the test context
            /// </summary>
            public static void Start()
            {
                if (started) return;

                EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());
                ApplicationContext.Current.Start();

                // Start the daemon services
                var adoPersistenceService = ApplicationContext.Current.GetService<AdoPersistenceService>();
                if (!adoPersistenceService.IsRunning)
                {
                    ApplicationContext.Current.Configuration.ServiceProviders.Add(typeof(LocalConfigurationManager));
                    //adoPersistenceService.Start();
                    ApplicationContext.Current.Start();
                }
                started = true;
            }
        }

        public DataTest()
        {
            DataTestUtil.Start();
        }
    }
}