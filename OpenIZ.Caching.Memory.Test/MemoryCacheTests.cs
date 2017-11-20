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
 * Date: 2016-8-2
 */
using OpenIZ.Caching.Memory;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.DataTypes;
using System.Threading;

namespace OpenIZ.Caching.Memory.Test
{
    [TestClass]
    public class MemoryCacheTests
    {

        /// <summary>
        /// This test will fill the cache with some data and then reduce the pressure on the cache
        /// </summary>
        [TestMethod()]
        public void ReduceMemoryCachePressureTest()
        {
            MemoryCache.Current.Clear();
            MemoryCache.Current.RegisterCacheType(typeof(AssigningAuthority), 10);
            // Add some assigning authorities to the cache
            for(int i = 0; i < 100; i++)
                MemoryCache.Current.AddUpdateEntry(new AssigningAuthority(i.ToString(), i.ToString(), i.ToString()) { Key = Guid.NewGuid() });
            Assert.AreEqual(100, MemoryCache.Current.GetSize());

            // Add an entry
            Thread.Sleep(1000); // Let some time pass
            Guid knownKey = Guid.NewGuid();
            MemoryCache.Current.AddUpdateEntry(new AssigningAuthority("SOMENEW", "SOMENEW", "SOMENEW") { Key = knownKey });
            // Ensure added
            Assert.AreEqual(101, MemoryCache.Current.GetSize());
            Assert.IsNotNull(MemoryCache.Current.TryGetEntry(knownKey));

            // Now reduce pressure, should *not* reduce the size of the cache as not enough time has passed
            MemoryCache.Current.ReducePressure();
            Thread.Sleep(3000); // Give the memory cache threads time to clean
            Assert.AreEqual(101, MemoryCache.Current.GetSize());

            // Now set the min age to 2 seconds
            MemoryCache.Current.SetMinAge(new TimeSpan(0, 0, 1));
            MemoryCache.Current.ReducePressure(); // this should reduce to 10
            Thread.Sleep(3000); // Give the memory cache threads time to clean
            Assert.AreEqual(10, MemoryCache.Current.GetSize());
            MemoryCache.Current.SetMinAge(new TimeSpan(0, 0, 30));

        }

        /// <summary>
        /// Clean memory cache after elapsed time
        /// </summary>
        [TestMethod()]
        public void CleanMemoryCacheTest()
        {
            MemoryCache.Current.Clear();
            MemoryCache.Current.RegisterCacheType(typeof(CodeSystem), 10, new TimeSpan(0,0,5).Ticks);
            // Add some assigning authorities to the cache
            for (int i = 0; i < 50; i++)
                MemoryCache.Current.AddUpdateEntry(new CodeSystem(i.ToString(), i.ToString(), i.ToString()) { Key = Guid.NewGuid() });
            Assert.AreEqual(50, MemoryCache.Current.GetSize());
            Thread.Sleep(2000); // Let some time pass
            Guid knownKey = Guid.NewGuid();
            MemoryCache.Current.AddUpdateEntry(new CodeSystem("SOMENEW", "SOMENEW", "SOMENEW") { Key = knownKey });
            // Ensure added
            Assert.AreEqual(51, MemoryCache.Current.GetSize());
            Assert.IsNotNull(MemoryCache.Current.TryGetEntry(knownKey));

            // Try to clean, shouldn't do anything everything still valid
            MemoryCache.Current.Clean();
            Thread.Sleep(3000); // Give the memory cache threads time to clean
            Assert.AreEqual(51, MemoryCache.Current.GetSize());

            // Now set the min age to 2 seconds
            // Since we last cleaned there should now be some data to clean
            MemoryCache.Current.Clean();
            Thread.Sleep(2000); // Give the memory cache threads time to clean
            Assert.AreEqual(1, MemoryCache.Current.GetSize());
        }

    }
}
