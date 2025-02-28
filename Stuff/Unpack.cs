using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stuff
{
    public class Unpack
    {
        private Dictionary<string, object> Objects { get; set; }
        public void Unpacc(byte[] PackedData)
        {
            PacketStuff packetData;
            using (MemoryStream memoryStream = new MemoryStream(Zip.Decompress(PackedData)))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Binder = new CustomBinder();
                packetData = (PacketStuff)formatter.Deserialize(memoryStream);
            }
            Objects = packetData.Objects;
        }

        public object Get(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return Objects[key];
            }
            return null;
        }
        public string GetAsString(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return (string)Objects[key];
            }
            return null;
        }

        public string[] GetAsStringArray(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return (string[])Objects[key];
            }
            return null;
        }

        public bool GetAsBoolen(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return (bool)Objects[key];
            }
            return false;
        }
        public short GetAsShort(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return (short)Objects[key];
            }
            return 0;
        }
        public int GetAsInteger(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return (int)Objects[key];
            }
            return 0;
        }
        public long GetAsLong(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return (long)Objects[key];
            }
            return 0;
        }
        public byte[] GetAsByteArray(string key)
        {
            if (Objects.ContainsKey(key))
            {
                return (byte[])Objects[key];
            }
            return null;
        }
        public Dictionary<string, object> GetAll()
        {
            return Objects;
        }
    }
}
