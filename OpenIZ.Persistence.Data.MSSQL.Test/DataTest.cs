/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Persistence.Data.MSSQL.Services;
using System;
using System.IO;

namespace OpenIZ.Persistence.Data.MSSQL.Test
{
    [DeploymentItem(@"OpenIZ_Test.mdf")]
    [DeploymentItem(@"OpenIZ_Test_log.ldf")]
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
                var sqlPersistenceService = ApplicationContext.Current.GetService<SqlServerPersistenceService>();
                if (!sqlPersistenceService.IsRunning)
                {
                    ApplicationContext.Current.Configuration.ServiceProviders.Add(typeof(LocalConfigurationManager));
                    sqlPersistenceService.Start();
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