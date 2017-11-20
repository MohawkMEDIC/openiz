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
 * Date: 2016-6-14
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Resource handler utility
	/// </summary>
	public class ResourceHandlerUtil
	{
		// Resource handler utility classes
		private static object m_lockObject = new object();

		private static ResourceHandlerUtil m_instance = null;

		// Handlers
		private Dictionary<String, IResourceHandler> m_handlers = new Dictionary<string, IResourceHandler>();

		/// <summary>
		/// Gets the current resource handler utility
		/// </summary>
		public static ResourceHandlerUtil Current
		{
			get
			{
				if (m_instance == null)
					lock (m_lockObject)
						if (m_instance == null)
							m_instance = new ResourceHandlerUtil();
				return m_instance;
			}
		}

		/// <summary>
		/// Get the current handlers
		/// </summary>
		public IEnumerable<IResourceHandler> Handlers => this.m_handlers.Values;

		/// <summary>
		/// Resource handler utility ctor
		/// </summary>
		private ResourceHandlerUtil()
		{
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					foreach (var t in asm.GetTypes().Where(o => typeof(IResourceHandler).IsAssignableFrom(o) && o.IsClass && !o.IsAbstract))
					{
						ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
						IResourceHandler rh = ci.Invoke(null) as IResourceHandler;
						m_handlers.Add(rh.ResourceName, rh);
					}
				}
				catch (ReflectionTypeLoadException)
				{
					// ignored
				}
			}
		}

		/// <summary>
		/// Get resource handler
		/// </summary>
		public IResourceHandler GetResourceHandler(String resourceName)
		{
			IResourceHandler retVal = null;
			this.m_handlers.TryGetValue(resourceName, out retVal);
			return retVal;
		}
	}
}