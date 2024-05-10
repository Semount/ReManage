using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace ReManage.Core
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(TableModel) && property.PropertyName == "RemoveCommand")
            {
                property.ShouldSerialize = instance => false;
            }

            return property;
        }
    }
}
