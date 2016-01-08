using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace OpenIZ.Persistence.Data.MSSQL.Test
{
    [DeploymentItem(@"OpenIZ_Test.mdf")]
    [DeploymentItem(@"OpenIZ_Test_log.ldf")]
    public abstract class DataTest
    {


    }
}