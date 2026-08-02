using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace San_Pham_Do_An1.Models
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static string? GetSessionString(this ISession session, string key)
        {
            return session.GetString(key);
        }
    }
}
