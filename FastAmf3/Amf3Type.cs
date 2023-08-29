using System;
using System.Collections.Generic;
using System.Text;

namespace Sinan.AMF3
{
    /// <summary>
    /// AMF3 data types.
    /// </summary>
    public class Amf3Type
    {
        internal const long UnixEpochTicks = 621355968000000000L;
        /// <summary>
        /// UTC 1970年1月1日(UnixTime起始时间)
        /// </summary>
        internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public const string TypeName = "$type";

        /// <summary>
        /// AMF Undefined data type.
        /// </summary>
        public const byte Undefined = 0;
        /// <summary>
        /// AMF Null data type.
        /// </summary>
        public const byte Null = 1;
        /// <summary>
        /// AMF Boolean false data type.
        /// </summary>
        public const byte BooleanFalse = 2;
        /// <summary>
        /// AMF Boolean true data type.
        /// </summary>
        public const byte BooleanTrue = 3;
        /// <summary>
        /// AMF Integer data type.
        /// </summary>
        public const byte Integer = 4;
        /// <summary>
        /// AMF Number data type.
        /// </summary>
        public const byte Number = 5;
        /// <summary>
        /// AMF String data type.
        /// </summary>
        public const byte String = 6;
        /// <summary>
        /// AMF Xml data type.
        /// </summary>
        public const byte XmlDoc = 7;
        /// <summary>
        /// AMF DateTime data type.
        /// </summary>
        public const byte DateTime = 8;
        /// <summary>
        /// AMF Array data type.
        /// </summary>
        public const byte Array = 9;
        /// <summary>
        /// AMF Object data type.
        /// </summary>
        public const byte Object = 10;
        /// <summary>
        /// AMF Xml data type.
        /// </summary>
        public const byte Xml = 11;
        /// <summary>
        /// AMF ByteArray data type.
        /// </summary>
        public const byte ByteArray = 12;

        /// <summary>
        /// AMF3 Data
        /// </summary>
        public const byte Amf3Tag = 17;
    }
}
