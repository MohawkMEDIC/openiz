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
 * Date: 2016-8-2
 */
using System;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// OpenIZ conversion helper functions
    /// </summary>
    public class OpenIZConvert
    {

        /// <summary>
        /// Guid > Byte[]
        /// </summary>
        public static byte[] NullGuidToByte(Guid? g)
        {
            return g.HasValue ? g.Value.ToByteArray() : null;
        }

        /// <summary>
        /// Byte[] > GUID
        /// </summary>
        public static Guid? ByteToNullGuid(byte[] b)
        {
            return b == null ? null : (Guid?)new Guid(b);
        }

        /// <summary>
        /// Boolean to int
        /// </summary>
        public static int BooleanToInt(Boolean b)
        {
            return b ? 1 : 0;
        }

        /// <summary>
        /// Boolean to int
        /// </summary>
        public static Boolean IntToBoolean(Int32 i)
        {
            return i != 0;
        }

        /// <summary>
        /// Guid > Byte[]
        /// </summary>
        public static byte[] GuidToByte(Guid g)
        {
            return g.ToByteArray();
        }

        /// <summary>
        /// Byte[] > GUID
        /// </summary>
        public static Guid ByteToGuid(byte[] b)
        {
            return new Guid(b);
        }


        /// <summary>
        /// DT > DTO
        /// </summary>
        public static DateTimeOffset? DateTimeToDateTimeOffset(DateTime? dt)
        {
            return dt;
        }

        /// <summary>
        /// DTO > DT
        /// </summary>
        public static DateTime? DateTimeOffsetToDateTime(DateTimeOffset? dto)
        {
            return dto?.DateTime;
        }

        /// <summary>
        /// DT > DTO
        /// </summary>
        public static DateTimeOffset DateTimeToDateTimeOffset(DateTime dt)
        {
            return dt;
        }

        /// <summary>
        /// DTO > DT
        /// </summary>
        public static DateTime DateTimeOffsetToDateTime(DateTimeOffset dto)
        {
            return dto.DateTime;
        }

        // Constant epoch
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Parse a date time into an object
        /// </summary>
        public static Int32 ToDateTime(DateTime date)
        {
            return (int)(date.ToUniversalTime() - EPOCH).TotalSeconds;
        }

        /// <summary>
        /// Parse a date time from an object
        /// </summary>
        public static DateTime ParseDateTime(Int32 date)
        {
            return EPOCH.AddSeconds(date).ToLocalTime();
        }

        /// <summary>
        /// Parse a date time into an object
        /// </summary>
        public static Int32 ToDateTimeOffset(DateTimeOffset date)
        {
            return (int)(date.ToLocalTime() - EPOCH).TotalSeconds;
        }

        /// <summary>
        /// Parse a date time from an object
        /// </summary>
        public static DateTimeOffset ParseDateTimeOffset(Int32 date)
        {
            return EPOCH.AddSeconds(date).ToLocalTime();
        }
    }
}