using MARC.HI.EHRS.SVC.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Persistence.Data.MSSQL.Services.Persistence;
using System;
using System.IO;

namespace OpenIZ.Persistence.Data.MSSQL.Test
{
    [DeploymentItem(@"OpenIZ_Test.mdf")]
    [DeploymentItem(@"OpenIZ_Test_log.ldf")]
    public abstract class DataTest
    {

        public DataTest()
        {
            // Start the daemon services
            var sqlPersistenceService = ApplicationContext.Current.GetService<SqlPersistenceService>();
            if(!sqlPersistenceService.IsRunning)
                sqlPersistenceService.Start();
        }
    }
}