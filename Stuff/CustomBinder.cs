using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Stuff
{
    public class CustomBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            try
            {
                Type type = Type.GetType($"{typeName}, {assemblyName}");

                if (type == null)
                {
                    type = Assembly.GetExecutingAssembly().GetType(typeName);
                }

                if (type == null)
                {
                    Assembly assembly = Assembly.Load(assemblyName);
                    type = assembly?.GetType(typeName);
                }

                if (type == null)
                {
                    throw new SerializationException($"{typeName} in {assemblyName}");
                }

                return type;
            }
            catch (Exception ex)
            {
                throw new SerializationException($"{typeName} in {assemblyName} {ex.Message}", ex);
            }
        }
    }
}
