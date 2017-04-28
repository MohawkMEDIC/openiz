using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using Newtonsoft.Json;
using OpenIZ.Caching.Redis.Configuration;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Serialization;
using OpenIZ.Core.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Caching.Redis
{
    /// <summary>
    /// Redis memory caching service
    /// </summary>
    public class RedisMemoryCacheService : IDataCachingService, IDaemonService
    {

        // Serializer
        private Dictionary<Type, XmlSerializer> m_serializerCache = new Dictionary<Type, XmlSerializer>();

        // Connection
        private ConnectionMultiplexer m_connection;

        // Subscriber
        private ISubscriber m_subscriber;

        // Configuration
        private RedisConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.caching.redis") as RedisConfiguration;

        // Binder
        private ModelSerializationBinder m_binder = new ModelSerializationBinder();

        /// <summary>
        /// Is the service running 
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_connection != null;
            }
        }

        // Data was added to the cache
        public event EventHandler<DataCacheEventArgs> Added;
        // Data was removed from the cache
        public event EventHandler<DataCacheEventArgs> Removed;
        // Started 
        public event EventHandler Started;
        // Starting
        public event EventHandler Starting;
        // Stopped
        public event EventHandler Stopped;
        // Stopping
        public event EventHandler Stopping;
        // Data was updated on the cache
        public event EventHandler<DataCacheEventArgs> Updated;


        /// <summary>
        /// Serialize objects
        /// </summary>
        private HashEntry[] SerializeObject(IdentifiedData data)
        {
            XmlSerializer xsz = null;
            if(!this.m_serializerCache.TryGetValue(data.GetType(), out xsz))
            {
                xsz = new XmlSerializer(data.GetType());
                lock (this.m_serializerCache)
                    this.m_serializerCache.Add(data.GetType(), xsz);
            }

            HashEntry[] retVal = new HashEntry[2];
            retVal[0] = new HashEntry("type", data.GetType().AssemblyQualifiedName);
            using (var sw = new StringWriter())
            {
                xsz.Serialize(sw, data);
                retVal[1] = new HashEntry("value", sw.ToString());
            }
            return retVal;
        }

        /// <summary>
        /// Serialize objects
        /// </summary>
        private IdentifiedData DeserializeObject(HashEntry[] data)
        {
            if (data == null) return null;

            Type type = Type.GetType(data.FirstOrDefault(o => o.Name == "type").Value);
            String value = data.FirstOrDefault(o => o.Name == "value").Value;

            // Find serializer
            XmlSerializer xsz = null;
            if (!this.m_serializerCache.TryGetValue(type, out xsz))
            {
                xsz = new XmlSerializer(type);
                lock (this.m_serializerCache)
                    this.m_serializerCache.Add(type, xsz);
            }
            using (var sr = new StringReader(value))
                return xsz.Deserialize(sr) as IdentifiedData;
        }

        /// <summary>
        /// Add an object to the REDIS cache
        /// </summary>
        public void Add(IdentifiedData data)
        {
            // We want to add
            if (this.m_connection == null || data == null || !data.Key.HasValue)
                return;

            // Add
            var redisDb = this.m_connection.GetDatabase();
            var existing = redisDb.KeyExists(data.Key.Value.ToString());
            redisDb.HashSet(data.Key.Value.ToString(), this.SerializeObject(data));
            if (existing)
                this.m_connection.GetSubscriber().Publish("oiz.events", $"PUT http://{Environment.MachineName}/{data.GetType().Name}/{data.Key.Value}");
            else
                this.m_connection.GetSubscriber().Publish("oiz.events", $"POST http://{Environment.MachineName}/{data.GetType().Name}/{data.Key.Value}");

        }

        /// <summary>
        /// Get a cache item
        /// </summary>
        public object GetCacheItem(Type tdata, Guid key)
        {
            // We want to add
            if (this.m_connection == null)
                return null;

            // Add
            var redisDb = this.m_connection.GetDatabase();
            return this.DeserializeObject(redisDb.HashGetAll(key.ToString()));

        }

        /// <summary>
        /// Get cache item of type
        /// </summary>
        public TData GetCacheItem<TData>(Guid key) where TData : IdentifiedData
        {
            return (TData)this.GetCacheItem(typeof(TData), key);
        }

        /// <summary>
        /// Remove a hash key item
        /// </summary>
        public void Remove(Type tdata, Guid key)
        {
            // We want to add
            if (this.m_connection == null)
                return;
            // Add
            var redisDb = this.m_connection.GetDatabase();
            redisDb.KeyDelete(key.ToString());
            this.m_connection.GetSubscriber().Publish("oiz.events", $"DELETE http://{Environment.MachineName}/{tdata.Name}/{key}");
        }

        /// <summary>
        /// Start the connection manager
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            var configuration = new ConfigurationOptions()
            {
                Password = this.m_configuration.Password
            };
            foreach (var itm in this.m_configuration.Servers)
                configuration.EndPoints.Add(itm);
            this.m_connection = ConnectionMultiplexer.Connect(configuration);
            this.m_subscriber = this.m_connection.GetSubscriber();

            // Subscribe to OpenIZ events
            m_subscriber.Subscribe("oiz.events", (channel, message) =>
            {
                var messageParts = ((string)message).Split(' ');
                var verb = messageParts[0];
                var uri = new Uri(messageParts[1]);

                string resource = uri.AbsolutePath.Replace("imsi/", ""),
                    id = uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf("/") + 1);

                switch(verb.ToLower())
                {
                    case "post":
                        this.Added?.Invoke(this, new DataCacheEventArgs(this.GetCacheItem(this.m_binder.BindToType(typeof(IdentifiedData).Assembly.FullName, resource), Guid.Parse(id))));
                        break;
                    case "put":
                        this.Updated?.Invoke(this, new DataCacheEventArgs(this.GetCacheItem(this.m_binder.BindToType(typeof(IdentifiedData).Assembly.FullName, resource), Guid.Parse(id))));
                        break;
                    case "delete":
                        var instance = Activator.CreateInstance(this.m_binder.BindToType(typeof(IdentifiedData).Assembly.FullName, resource)) as IdentifiedData;
                        instance.Key = Guid.Parse(id);
                        this.Removed?.Invoke(this, new DataCacheEventArgs(instance));
                        break;
                }
            });

            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stop the connection
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.m_connection.Dispose();
            this.m_connection = null;
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
