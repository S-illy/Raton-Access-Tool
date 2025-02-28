using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Stuff
{
    public class CustomBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type type = Type.GetType($"{typeName}, {assemblyName}");
            if (type == null)
            {
                type = Assembly.GetExecutingAssembly().GetType(typeName);
            }
            return type;
        }
    }
}
