using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sinan.AMF3
{
    /// <summary>
    /// AMF3自定义序列化接口.
    /// </summary>
    public interface IExternalizable
    {
        ///// <summary>
        ///// 自定义的AMF3反序列化方法
        ///// </summary>
        ///// <param name="reader"></param>
        //void ReadExternal(AmfReader reader);

        /// <summary>
        /// 自定的AMF3义序列化方法
        /// </summary>
        /// <param name="writer"></param>
        void WriteExternal(IExternalWriter writer);
    }
}
