using System.Net;

namespace FluentHTTPToolkit.Extensions.Extensions
{
    public static class WebRequestExtensions
    {
        public static async Task<WebResponse> GetWebResponseAsync(this WebRequest request)
        {
            try
            {
                return await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                return ex.Response;
            }
        }
    }
}
