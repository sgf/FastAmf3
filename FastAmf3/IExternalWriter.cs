using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sinan.AMF3
{
    public interface IExternalWriter
    {

        void WriteDateTime(DateTime value);
        void WriteBoolean(bool value);
        void WriteByte(byte value);
        void WriteBytes(byte[] bytes, int offset, int length);
        void WriteDouble(double value);
        void WriteFloat(float value);
        void WriteInt(int value);
        void WriteNull();

        /// <summary>
        /// 将对象以AMF3序列化格式写入对象,
        /// 写入时会清理引用的缓存
        /// </summary>
        /// <param name="data"></param>
        void WriteObject(object data);


        void WriteUTF(string value);

        //void WriteU29(int value);
        //void WriteUndefined();
        //void WriteShort(short value);
        //void WriteUnsignedInt(uint value);
        //void WriteUTFBytes(string value);

        void WriteKey(string key);

        /// <summary>
        /// 写AMF3对象.不清空三个缓存.用于自定义方式写入对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool WriteValue(object data);
        bool WriteReference(object value);
        bool WriteIDictionary(IDictionary<string, int> value);
        bool WriteIDictionary(IDictionary<string, object> value);
    }
}

