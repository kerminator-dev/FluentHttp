using Newtonsoft.Json;
using System.Net;

namespace FluentHTTPToolkit.Extensions
{
    public static class HttpWebResponseExtensions
    {
        public async static Task<T> TryDeserializeAsync<T>(this HttpWebResponse httpWebResponse)
        {
            using (var reader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                var content = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public async static Task<string> GetBodyString(this HttpWebResponse httpWebResponse)
        {
            using (var reader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
