using System.Reflection;
using System.Runtime.Serialization;

namespace SpotiConnector.Application.Extensions
{
    public static class EnumValueExtractorExtension
    {
        public static string GetEnumMemberValue(this Enum value)
        {
            var type = value.GetType();
            var member = type.GetMember(value.ToString()).FirstOrDefault();

            var attribute = member?
                .GetCustomAttribute<EnumMemberAttribute>();

            return attribute?.Value ?? value.ToString();
        }
    }
}
