using System;

#if __REFERENCES_SYSTEMTEXTJSON

using System.Text.Json.Serialization.Metadata;

#nullable disable

using __READSTREAM = System.IO.Stream;
using __WRITESTREAM = System.IO.Stream;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static T DeserializeJson<T>(this Func<__READSTREAM> stream, JsonTypeInfo<T> typeInfo)
        {
            using(var s = stream.Invoke())
            {
                return DeserializeJson(s, typeInfo);
            }            
        }

        public static T DeserializeJson<T>(this __READSTREAM stream, JsonTypeInfo<T> typeInfo)
        {
            GuardReadable(stream);

            return System.Text.Json.JsonSerializer.Deserialize(stream, typeInfo);
        }

        public static void SerializeJson<T>(this Func<__WRITESTREAM> stream, JsonTypeInfo<T> typeInfo, T value)
        {
            using (var s = stream.Invoke())
            {
                SerializeJson(s, typeInfo, value);
            }
        }

        public static void SerializeJson<T>(this __WRITESTREAM stream, JsonTypeInfo<T> typeInfo, T value)
        {
            GuardWriteable(stream);

            System.Text.Json.JsonSerializer.Serialize(stream, value, typeInfo);
        }

    }
}

#endif
