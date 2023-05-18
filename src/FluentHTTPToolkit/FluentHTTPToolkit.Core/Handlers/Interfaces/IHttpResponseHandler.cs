using System.Net;

namespace FluentHTTPToolkit.Core.Handlers.Interfaces
{
    public interface IHttpResponseHandler<TResponseModel>
    {
        Task<TResponseModel?> HandleResponse(HttpWebResponse response);
    }
}
