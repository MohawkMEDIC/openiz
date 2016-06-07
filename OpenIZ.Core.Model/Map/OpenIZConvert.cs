using System;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// OpenIZ conversion helper functions
    /// </summary>
    internal class OpenIZConvert
    {

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
    }
}