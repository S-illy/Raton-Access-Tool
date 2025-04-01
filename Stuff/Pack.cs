using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Stuff
{
    public class Pack
    {
        private Dictionary<string, object> Objects { get; set; }
        public Pack()
        {
            Objects = new Dictionary<string, object>();
        }
        public void Set(string key, object value)
        {
            if (Objects.ContainsKey(key))
            {
                Objects[key] = value;
            }
            else
            {
                Objects.Add(key, value);
            }
        }

        public byte[] Pacc()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, new PacketStuff() { Objects = Objects });
                return Zip.Compress(memoryStream.ToArray());
            }
        }
    }
}
