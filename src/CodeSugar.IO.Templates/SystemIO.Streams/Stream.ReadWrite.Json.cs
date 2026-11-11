using System;

#if __REFERENCES_SYSTEMTEXTJSON

using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;


#nullable disable

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;
using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static T DeserializeJson<T>(this __STREAMFUNC stream, JsonTypeInfo<T> typeInfo)
        {
            using(var s = stream.OpenRead())
            {
                GuardReadable(s);

                return System.Text.Json.JsonSerializer.Deserialize(s, typeInfo);
            }            
        }        

        public static void SerializeJson<T>(this __STREAMFUNC stream, JsonTypeInfo<T> typeInfo, T value)
        {
            using (var s = stream.OpenWrite())
            {
                GuardWriteable(s);

                System.Text.Json.JsonSerializer.Serialize(s, value, typeInfo);
            }
        }

        public static async Task<T> DeserializeJsonAsync<T>(this __STREAMTASK stream, JsonTypeInfo<T> typeInfo)
        {
            using (var s = await stream.OpenReadAsync(CancellationToken.None))
            {
                GuardReadable(s);

                return await System.Text.Json.JsonSerializer.DeserializeAsync(s, typeInfo);
            }
        }

        public static async Task SerializeJson<T>(this __STREAMTASK stream, JsonTypeInfo<T> typeInfo, T value)
        {
            using (var s = await stream.OpenWriteAsync(CancellationToken.None))
            {
                GuardWriteable(s);

                await System.Text.Json.JsonSerializer.SerializeAsync(s, value, typeInfo);
            }
        }
    }
}

#endif
