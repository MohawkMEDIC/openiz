/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Map utility
    /// </summary>
    public static class MapUtil
    {
        /// <summary>
        /// Flavor validation
        /// </summary>
        private static Dictionary<string, MethodInfo> s_flavorValidation = new Dictionary<string, MethodInfo>();
        /// <summary>
        /// Maps from wire format to real format
        /// Key - string in the format {FROM}>{TO}
        /// Value - MethodInfo of the method that will perform the operation to convert
        /// </summary>
        private static Dictionary<string, MethodInfo> s_wireMaps = new Dictionary<string, MethodInfo>();
        

        /// <summary>
        /// Find the converter for the types specified
        /// </summary>
        /// <param name="scanType">The type to scan in</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="destType">The destination type</param>
        /// <returns></returns>
        internal static MethodInfo FindConverter(Type scanType, Type sourceType, Type destType)
        {

            MethodInfo retVal = null;
			lock (scanType) {
				var rtm = scanType.GetRuntimeMethods ();
				foreach (MethodInfo mi in rtm)
					if (mi.GetParameters ().Length == 2 &&
					                   (mi.ReturnType.GetTypeInfo ().IsSubclassOf (destType) || destType == mi.ReturnType) &&
					                   mi.GetParameters () [0].ParameterType.FullName == sourceType.FullName &&
					                   mi.GetParameters () [1].ParameterType.FullName == typeof(IFormatProvider).FullName)
						retVal = mi;
					else if (mi.GetParameters ().Length == 1 &&
					                        (mi.ReturnType.GetTypeInfo ().IsSubclassOf (destType) || destType == mi.ReturnType) &&
					                        mi.GetParameters () [0].ParameterType.FullName == sourceType.FullName && retVal == null)
						retVal = mi;
			}
            return retVal;

        }

        /// <summary>
        /// Attempt casting <paramref name="value"/> to <paramref name="destType"/> placing the result 
        /// in <paramref name="result"/>
        /// </summary>
        /// <remarks>If null is passed as the <paramref name="resultDetails"/> parameter then a <see cref="T:MARC.Everest.Exceptions.VocabularyException"/>
        /// will be thrown whenever vocabulary cannot be cast.</remarks>
        public static bool TryConvert(object value, Type destType, out object result)
        {
            // The type represents a wrapper for an enumeration
            Type m_destType = destType;

            bool requiresExplicitCastCall = false;
            if (value == null)
            {
                result = null;
                return true;
            }
            else if (m_destType.GetTypeInfo().IsGenericType && !value.GetType().GetTypeInfo().IsEnum)
            {
                m_destType = m_destType.GenericTypeArguments[0];
                requiresExplicitCastCall = true;
            }

            // Is there a cast?
            if (destType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo())) //  (m_destType.IsAssignableFrom(value.GetType())) // Same type
            {
                result = value;
                return true;
            }
            else if (m_destType == typeof(int) && value.GetType().GetTypeInfo().IsEnum)
            {
                result = (int)value;
                return true;
            }
            else if (m_destType.GetTypeInfo().IsEnum && value.GetType() == typeof(int))
            {
                result = value;
                return true;
            }
            else if (m_destType.GetTypeInfo().IsEnum && value.GetType() == typeof(String)) // No map exists yet
            {
                try
                {
                    result = Enum.Parse(m_destType, value.ToString(), true);
                    return true;
                }
                catch (Exception e)
                {
                    result = null;
                    return false;
                }

            }
            else if (destType.FullName.StartsWith("System.Nullable"))
                destType = m_destType; // Transparency for nullable types

            // Is there a built in method that can convert this
            MethodInfo mi;
            string converterKey = string.Format(CultureInfo.InvariantCulture, "{0}>{1}", value.GetType().FullName, destType.FullName);
            if (!s_wireMaps.TryGetValue(converterKey, out mi))
            {
                // Try to find a map first...
                // Using an operator overload
                mi = FindConverter(m_destType, value.GetType(), destType);
                if (mi == null)
                    mi = FindConverter(value.GetType(), value.GetType(), destType);
                if (mi == null && m_destType != destType) // Using container type
                    mi = FindConverter(destType, value.GetType(), destType);
                if (mi == null) // Using System.Xml.XmlConvert 
                    mi = FindConverter(typeof(System.Xml.XmlConvert), value.GetType(), destType);
                if (mi == null) // Using System.Convert as a last resort
                    mi = FindConverter(typeof(System.Convert), value.GetType(), destType);
                if (mi == null)
                    mi = FindConverter(typeof(OpenIZConvert), value.GetType(), destType);
                if (mi != null)
                {
                    lock (s_wireMaps)
                        if (!s_wireMaps.ContainsKey(converterKey))
                            s_wireMaps.Add(converterKey, mi);
                }
                else
                {
                    // Last ditch effort to parse
                    // Compare apples to apples
                    // We have two generic types, however the dest type generic doesn't match the 
                    // value type generic. This is common with type overrides where the object container
                    // doesn't match the value. For example, attempting to assign a CV<String> to a CS<ResponseMode>
                    if (value.GetType().GetTypeInfo().IsGenericType &&
                        destType.GetTypeInfo().IsGenericType &&
                        destType.GenericTypeArguments[0] != value.GetType().GenericTypeArguments[0] &&
                        destType.GetGenericTypeDefinition() != value.GetType().GetGenericTypeDefinition())
                    {
                        Type valueCastType = value.GetType().GetGenericTypeDefinition().MakeGenericType(
                            destType.GenericTypeArguments
                        );
                        try
                        {
                            return TryConvert(value, valueCastType, out result);
                        }
                        catch
                        {
                            result = null;
                            return false;
                        }
                    }
                    else
                    {
                        result = null;
                        return false;
                    }

                }
            }


            try
            {
                if (mi.GetParameters().Length == 2)
                    result = mi.Invoke(null, new object[] { value, CultureInfo.InvariantCulture }); // Invoke the conversion method;
                else
                    result = mi.Invoke(null, new object[] { value }); // Invoke the conversion method
                return result != null;
            }
            catch { result = null; return false; }
        }

    }
}
