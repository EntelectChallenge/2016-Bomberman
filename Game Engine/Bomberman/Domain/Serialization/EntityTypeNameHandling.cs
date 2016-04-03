using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Serialization
{
    public class EntityTypeNameHandling : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            return typeName.Contains("PowerUp") ? Type.GetType("Domain.Entities.PowerUps." + typeName) : Type.GetType("Domain.Entities." + typeName);
        }

        //public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        //{
        //    assemblyName = null;
        //    typeName = serializedType.Name;
        //}
    }
}
