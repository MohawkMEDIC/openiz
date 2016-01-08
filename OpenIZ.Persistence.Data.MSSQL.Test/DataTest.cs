using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace OpenIZ.Persistence.Data.MSSQL.Test
{
    public abstract class DataTest
    {

        /// <summary>
        /// Initialize the class
        /// </summary>
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                "DataDirectory",
                Path.Combine(context.TestDeploymentDir, string.Empty));
        }
    }
}