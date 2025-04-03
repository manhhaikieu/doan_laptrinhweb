using Newtonsoft.Json;
using System.Text.Json;
namespace LAB03.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value)); // Lưu giỏ hàng vào session dưới dạng JSON
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value); // Lấy giỏ hàng từ session
        }
    }

}