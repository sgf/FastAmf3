using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Sinan.AMF3
{
    /// <summary>
    /// AMF解码类..
    /// </summary>
    sealed public class Amf3Reader<T> where T : class, IDictionary<string, object>, new()
    {
        static UTF8Encoding utf8 = new UTF8Encoding(false, true);

        List<object> m_objectReferences;
        List<string> m_stringReferences;
        List<ClassDefinition> m_classDefinitions;

        int m_index;
        readonly int m_maxIndex;
        readonly int m_offset;
        readonly byte[] m_bin;

        public int Offset
        {
            get { return m_offset; }
        }

        public byte[] Array
        {
            get { return m_bin; }
        }

        /// <summary>
        /// 已读取的数量
        /// </summary>
        public int Count
        {
            get { return m_index - m_offset; }
        }

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity
        {
            get { return m_maxIndex - m_offset; }
        }

        /// <summary>
        /// 未完,仍有数据可以读取
        /// </summary>
        public bool Unfinished
        {
            get { return m_index < m_maxIndex; }
        }

        Amf3Reader()
        {
            m_objectReferences = new List<object>(3);
            m_stringReferences = new List<string>(5);
            m_classDefinitions = new List<ClassDefinition>();
        }

        /// <summary>
        /// Initializes a new instance of the AMFReader class
        /// </summary>
        /// <param name="stream"></param>
        public Amf3Reader(byte[] bin, int offset, int size) :
            this()
        {
            m_bin = bin;
            m_offset = offset;
            m_index = m_offset;
            m_maxIndex = offset + size;
        }

        /// <summary>
        /// Initializes a new instance of the AMFReader class
        /// </summary>
        /// <param name="stream"></param>
        public Amf3Reader(ArraySegment<byte> segment) :
            this()
        {
            m_bin = segment.Array;
            m_offset = segment.Offset;
            m_index = m_offset;
            m_maxIndex = m_offset + segment.Count;
        }

        /// <summary>
        /// 跳过指定长度的字节
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool TrySkip(int length)
        {
            if (m_index + length < m_maxIndex)
            {
                m_index += length;
                return true;
            }
            return false;
        }

        public int ReadByte()
        {
            return m_bin[m_index++];
        }

        void TryReadType(int amftype)
        {
            if (m_bin[m_index] == amftype)
            {
                m_index++;
                return;
            }
            throw new AmfException("Can not convert type " + m_bin[m_index] + " to " + amftype);
        }

        private string ReadUTF8(int length)
        {
            //if (length == 0) return string.Empty;
            string decodedString = utf8.GetString(m_bin, m_index, length);
            m_index += length;
            return decodedString;
        }

        private string ReadAmf3UTF8()
        {
            int handle = ReadAmf3Int();
            if ((handle & 1) == 0)
            {
                return m_stringReferences[handle >> 1];
            }
            int length = GetLength(handle);
            if (length == 0)
            {
                return string.Empty;
            }
            string str = ReadUTF8(length);
            m_stringReferences.Add(str);
            return str;
        }

        private int ReadAmf3Int()
        {
            int acc = ReadByte();
            if (acc < 128)
            {
                return acc;
            }
            acc = (acc & 0x7f) << 7;
            int tmp = ReadByte();
            if (tmp < 128)
            { acc = acc | tmp; }
            else
            {
                acc = (acc | tmp & 0x7f) << 7;
                tmp = ReadByte();
                if (tmp < 128)
                { acc = acc | tmp; }
                else
                {
                    acc = (acc | tmp & 0x7f) << 8;
                    tmp = ReadByte();
                    acc = acc | tmp;
                }
            }
            int r = -(acc & 0x10000000) | acc;
            return r;
        }
#if unsafe
        private unsafe double ReadAmf3Double()
        {
            ulong num3 = ((uint)m_bin[m_index + 0] << 0x18) | ((uint)m_bin[m_index + 1] << 0x10) | ((uint)m_bin[m_index + 2] << 8) | (uint)m_bin[m_index + 3];
            uint num4 = ((uint)m_bin[m_index + 4] << 0x18) | ((uint)m_bin[m_index + 5] << 0x10) | ((uint)m_bin[m_index + 6] << 8) | (uint)m_bin[m_index + 7];
            ulong num = num4 | (num3 << 32);
            m_index += 8;
            return *(double*)(&num);
        }
#else
        private double ReadAmf3Double()
        {
            byte[] bin = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bin[i] = m_bin[(m_index + 7) - i];
            }
            m_index += 8;
            return BitConverter.ToDouble(bin, 0);
        }
#endif

        #region 读取类定义
        /// <summary>
        /// 读取对象定义
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private ClassDefinition ReadClassDefinition(int handle)
        {
            if ((handle & 1) == 0)
            {
                return m_classDefinitions[handle >> 1];
            }
            handle = handle >> 1;
            //inline class-def
            string typeIdentifier = ReadAmf3UTF8();
            //flags that identify the way the object is serialized/deserialized
            bool externalizable = ((handle & 1) != 0);
            handle = handle >> 1;
            bool isDynamic = ((handle & 1) != 0);
            handle = handle >> 1;

            ClassMember[] members = new ClassMember[handle];
            for (int i = 0; i < handle; i++)
            {
                string name = ReadAmf3UTF8();
                ClassMember classMember = new ClassMember(name, BindingFlags.Default, MemberTypes.Custom);
                members[i] = classMember;
            }
            ClassDefinition classDefinition = new ClassDefinition(typeIdentifier, members, externalizable, isDynamic);
            m_classDefinitions.Add(classDefinition);
            return classDefinition;
        }

        /// <summary>
        /// 从定义中读取AMF3对象.
        /// </summary>
        /// <param name="classDefinition"></param>
        /// <returns></returns>
        private object ReadAmf3Object(ClassDefinition classDefinition)
        {
            T instance = new T();
            if (!string.IsNullOrEmpty(classDefinition.ClassName))
            {
                instance.Add(Amf3Type.TypeName, classDefinition.ClassName);
            }

            m_objectReferences.Add(instance);
            for (int i = 0; i < classDefinition.MemberCount; i++)
            {
                string key = classDefinition.Members[i].Name;
                object value = ReadAmf3Data();
                instance.Add(key, value);
            }
            if (classDefinition.IsDynamic)
            {
                string key = ReadAmf3UTF8();
                while (key != string.Empty)
                {
                    object value = ReadAmf3Data();
                    instance.Add(key, value);
                    key = ReadAmf3UTF8();
                }
            }
            return instance;
        }

        /// <summary>
        ///  读取AMF3动态对象.
        /// </summary>
        /// <returns></returns>
        private object ReadAmf3Dynamic()
        {
            T instance = new T();
            m_objectReferences.Add(instance);
            string key = ReadAmf3UTF8();
            while (key != string.Empty)
            {
                object value = ReadAmf3Data();
                instance.Add(key, value);
                key = ReadAmf3UTF8();
            }
            return instance;
        }
        #endregion

        public bool ReadAmf3Bool()
        {
            byte b = m_bin[m_index];
            if (b == Amf3Type.BooleanFalse)
            {
                m_index++;
                return false;
            }
            if (b == Amf3Type.BooleanTrue)
            {
                m_index++;
                return true;
            }
            throw new AmfException("Can not convert type " + b + " to Bool");
        }

        public int ReadAmf3Integer()
        {
            TryReadType(Amf3Type.Integer);
            return ReadAmf3Int();
        }

        public double ReadAmf3Number()
        {
            TryReadType(Amf3Type.Number);
            return ReadAmf3Double();
        }

        public string ReadAmf3String()
        {
            TryReadType(Amf3Type.String);
            return ReadAmf3UTF8();
        }

        public DateTime ReadAmf3DateTime(bool readType = true)
        {
            if (readType) { TryReadType(Amf3Type.DateTime); }

            int handle = ReadAmf3Int();
            if ((handle & 1) == 0)
            {
                return (DateTime)m_objectReferences[handle >> 1];
            }

            double milliseconds = this.ReadAmf3Double();
            DateTime date = Amf3Type.UnixEpoch.AddMilliseconds(milliseconds);
            m_objectReferences.Add(date);
            return date;
        }

        public object ReadAmf3Array(bool readType = true)
        {
            if (readType) { TryReadType(Amf3Type.Array); }

            int handle = ReadAmf3Int();
            if ((handle & 1) == 0)
            {
                return m_objectReferences[handle >> 1];
            }

            int length = GetLength(handle);
            T hashtable = null;
            string key = ReadAmf3UTF8();
            while (key != string.Empty)
            {
                if (hashtable == null)
                {
                    hashtable = new T();
                    m_objectReferences.Add(hashtable);
                }
                object value = ReadAmf3Data();
                hashtable.Add(key, value);
                key = ReadAmf3UTF8();
            }
            //Not an associative array
            if (hashtable == null)
            {
                IList array = new object[length];
                m_objectReferences.Add(array);
                for (int i = 0; i < length; i++)
                {
                    array[i] = ReadAmf3Data();
                }
                return array;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    object value = ReadAmf3Data();
                    hashtable.Add(i.ToString(), value);
                }
                return hashtable;
            }
        }

        public XmlDocument ReadAmf3XmlDocument(bool readType = true)
        {
            if (readType)
            {
                int amftype = m_bin[m_index];
                if (amftype != Amf3Type.Xml && amftype != Amf3Type.XmlDoc)
                {
                    throw new AmfException("Can not convert type " + amftype + " to Xml");
                }
                m_index++;
            }
            int handle = ReadAmf3Int();
            //对象引用 
            if ((handle & 1) == 0)
            {
                return m_objectReferences[handle >> 1] as XmlDocument;
            }

            int length = GetLength(handle);
            string xml = length > 0 ? this.ReadUTF8(length) : string.Empty;

            XmlDocument xmlDocument = new XmlDocument();
            if (!string.IsNullOrEmpty(xml))
            {
                xmlDocument.LoadXml(xml);
            }
            m_objectReferences.Add(xmlDocument);
            return xmlDocument;
        }

        public object ReadAmf3Object(bool readType = true)
        {
            if (readType) { TryReadType(Amf3Type.Object); }
            int handle = ReadAmf3Int();
            if ((handle & 1) == 0)
            {
                return m_objectReferences[handle >> 1];
            }

            //读取对象定义..
            ClassDefinition classDefinition = ReadClassDefinition(handle >> 1);
            if (classDefinition.IsExternalizable)
            {
                throw new AmfException("Not support externalizable class");
            }
            if (classDefinition.IsDynamic)
            {
                return ReadAmf3Dynamic();
            }
            object obj = ReadAmf3Object(classDefinition);
            return obj;
        }

        private int GetLength(int handle)
        {
            int length = handle >> 1;
            if (length < 0 || m_index + length > m_maxIndex)
            {
                throw new AmfException("Decoding error,more than long:" + length);
            }
            return length;
        }

        public byte[] ReadAmf3ByteArray(bool readType = true)
        {
            if (readType) { TryReadType(Amf3Type.Array); }
            int handle = ReadAmf3Int();
            if ((handle & 1) == 0)
            {
                return m_objectReferences[handle >> 1] as byte[];
            }

            int length = GetLength(handle);
            byte[] buffer = new byte[length];
            System.Buffer.BlockCopy(m_bin, m_index, buffer, 0, length);
            m_index += length;
            m_objectReferences.Add(buffer);
            return buffer;
        }

        object ReadAmf3Data()
        {
            int type = ReadByte();
            switch (type)
            {
                case Amf3Type.Undefined:
                    return DBNull.Value;
                case Amf3Type.Null:
                    return null;
                case Amf3Type.BooleanFalse:
                    return false;
                case Amf3Type.BooleanTrue:
                    return true;
                case Amf3Type.Integer:
                    return ReadAmf3Int();
                case Amf3Type.Number:
                    return ReadAmf3Double();
                case Amf3Type.String:
                    return ReadAmf3UTF8();
                case Amf3Type.DateTime:
                    return ReadAmf3DateTime(false);
                case Amf3Type.Array:
                    return ReadAmf3Array(false);
                case Amf3Type.Object:
                    return ReadAmf3Object(false);
                case Amf3Type.Xml:
                case Amf3Type.XmlDoc:
                    return ReadAmf3XmlDocument(false);
                case Amf3Type.ByteArray:
                    return ReadAmf3ByteArray(false);
                case Amf3Type.Amf3Tag:
                    return ReadAmf3Data();
                default:
                    break;
            }
            throw new AmfException("Unknown type:" + type);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        public object ReadNextObject()
        {
            m_objectReferences.Clear();
            m_stringReferences.Clear();
            m_classDefinitions.Clear();
            return ReadAmf3Data();
        }
    }
}
