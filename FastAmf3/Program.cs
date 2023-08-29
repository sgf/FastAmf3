using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sinan.AMF3
{
    class Program
    {
        static void Main(string[] args)
        {

            Amf3Writer writer = new Amf3Writer(65536);
            writer.WriteObject(243543523);
            writer.WriteObject(4543524.4d);
            writer.WriteObject(null);
            writer.WriteObject(false);
            writer.WriteObject(true);
            writer.WriteObject(DateTime.UtcNow);
            writer.WriteObject("字符串");

            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("Name", "姓名");
            obj.Add("Address", "地址");
            obj.Add("ID", 88888888);

            writer.WriteObject(obj);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);
            writer.WriteObject(2);

            Amf3Reader<Dictionary<string, object>> reader = new Amf3Reader<Dictionary<string, object>>(writer.Array, 0, writer.Count);

            while (reader.Unfinished)
            {
                object o = reader.ReadNextObject();
                Console.WriteLine(o);
            }
            Console.ReadLine();
        }

    }
}
