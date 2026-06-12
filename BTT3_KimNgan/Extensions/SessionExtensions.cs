using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BTT3_KimNgan.Extensions
{
    public static class SessionExtensions
    {
        // Hàm lưu một Đối tượng vào Session dưới dạng chuỗi JSON
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Hàm đọc một Đối tượng từ Session bằng cách chuyển đổi ngược từ chuỗi JSON
        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}