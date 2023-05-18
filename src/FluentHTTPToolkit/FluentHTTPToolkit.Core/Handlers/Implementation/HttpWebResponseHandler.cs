using FluentHTTPToolkit.Core.Exceptions;
using FluentHTTPToolkit.Core.Handlers.Interfaces;
using FluentHTTPToolkit.Extensions.Extensions;
using System.Net;

namespace FluentHTTPToolkit.Core.Handlers.Implementation
{
    public class HttpWebResponseHandler<T> : IHttpResponseHandler<T>
    {
        // Обработчики
        protected readonly IDictionary<HttpStatusCode, Func<HttpWebResponse, Task<T>>> _responseHandlers;
        protected Func<HttpWebResponse, Task<T>>? _successHandler;             // HTTP Code 200-299 
        protected Func<HttpWebResponse, Task<T>>? _clientErrorHandler;         // HTTP Code 400-499 
        protected Func<HttpWebResponse, Task<T>>? _internalServerErrorHandler; // HTTP Code 400-499 
        protected Func<HttpWebResponse, Task<T>>? _othersHandler;              // Для всех остальных кодов, которые не были указаны 

        public HttpWebResponseHandler()
        {
            _responseHandlers = new Dictionary<HttpStatusCode, Func<HttpWebResponse, Task<T>>>();
        }

        /// <summary>
        /// Обработать конкретный HTTP-код
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="responseHandler">Обработчик ответа от сервера, если код ответа равен <paramref name="statusCode"/></param>
        /// <returns></returns>
        public HttpWebResponseHandler<T> Handle(HttpStatusCode statusCode, Func<HttpWebResponse?, Task<T>> responseHandler)
        {
            _responseHandlers[statusCode] = responseHandler;

            return this;
        }

        /// <summary>
        /// Обработать HTTP-коды 2XX (Success)
        /// </summary>
        /// <param name="handler">Обработчик ответа от сервера, если код ответа 2XX</param>
        /// <returns></returns>
        public HttpWebResponseHandler<T> HandleSuccess(Func<HttpWebResponse, Task<T>> handler)
        {
            _successHandler = handler;

            return this;
        }

        /// <summary>
        /// Обработать HTTP-коды 4XX (Client Error)
        /// </summary>
        /// <param name="handler">Обработчик ответа от сервера, если код ответа 4XX</param>
        /// <returns></returns>
        public HttpWebResponseHandler<T> HandleClientError(Func<HttpWebResponse, Task<T>> handler)
        {
            _clientErrorHandler = handler;

            return this;
        }

        /// <summary>
        /// Обработать HTTP-коды 5XX (Internal Server Error)
        /// </summary>
        /// <param name="handler">Обработчик ответа от сервера, если код ответа 5XX</param>
        /// <returns></returns>
        public HttpWebResponseHandler<T> HandleInternalServerError(Func<HttpWebResponse, Task<T>> handler)
        {
            _internalServerErrorHandler = handler;

            return this;
        }

        /// <summary>
        /// Обработать остальные HTTP-коды, для которых не были указаны обработчики
        /// </summary>
        /// <param name="handler">Обработчик ответа от сервера</param>
        /// <returns></returns>
        public HttpWebResponseHandler<T> HandleOthers(Func<HttpWebResponse, Task<T>> handler)
        {
            _othersHandler = handler;

            return this;
        }

        /// <summary>
        /// Выполнить обработку запроса
        /// </summary>
        /// <param name="response">Ответ от сервера</param>
        /// <returns>Результат выполнения обработчика ответа от сервера</returns>
        /// <exception cref="UnexpectedStatusCodeException">Нет подходящего обработчика для полученного HTTP-кода</exception>
        public Task<T>? HandleResponse(HttpWebResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var statusCode = (int)response.StatusCode;

            if (_responseHandlers.TryGetValue(response.StatusCode, out var handler))
            {
                return handler(response);
            }
            else if (statusCode.Between(200, 299) && _successHandler is not null)
            {
                return _successHandler(response);
            }
            else if (statusCode.Between(400, 499) && _clientErrorHandler is not null)
            {
                return _clientErrorHandler(response);
            }
            else if (statusCode.Between(500, 599) && _internalServerErrorHandler is not null)
            {
                return _internalServerErrorHandler?.Invoke(response);
            }
            else
            {
                if (_othersHandler is null)
                    throw new UnexpectedStatusCodeException($"No suitable handler for HTTP code {statusCode}.");

                return _othersHandler.Invoke(response);
            }
        }
    }
}
