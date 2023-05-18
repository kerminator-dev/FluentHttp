# Fluent Http Toolkit - библиотека с Fluent-интерфейсом для построения HTTP-запросов и обработки HTTP-ответов в .NET

**HttpWebRequestBuilder** - построитель запросов. Позволяет настроить параметры запроса, 
такие как HTTP-метод, JWT-Token, заголовки и Json-тело запроса. Реализует паттерн Builder.

**HttpWebResponseHandler** - обработчик ответов от сервера, для обработки используется цепочка методов-обработчиков, которые различают успешные ответы (2xx), 
ответы с клиентскими ошибками (4xx), ответы с ошибками сервера (5xx), конкретные HTTP-коды и т. д. Реализует паттерн Chain of responsibility.

### Грубый пример формирования HTTP-запросов и его обработка обычным образом
```csharp

string url = @"https://host:XXXX/api/GetSomething";

// Построение запроса
HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
request.AutomaticDecompression = DecompressionMethods.GZip;

// Выполнение запроса
using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
{
    using (Stream stream = response.GetResponseStream())
    {
        using (StreamReader reader = new StreamReader(stream))
        {
            // Обработка ответа от сервера
            var body = reader.ReadToEnd();
        }
    }
}
```

### Пример формирования HTTP-запроса и его обработка при помощи Fluent Http Toolkit
```csharp
using FluentHTTPToolkit.Core.Builders.Implementation;
using FluentHTTPToolkit.Core.Handlers.Implementation;
using FluentHTTPToolkit.Extensions;

string url = @"https://host:XXXX/api/PostSomething";
string bearerToken = "...";
RequestObjectDTO requestBody = new RequestObjectDTO();

// Построение запроса
var request = await new HttpWebRequestBuilder(url)
                        .WithHttpMethod(HttpMethod.Post)
                        .WithJWT(bearerToken)
                        .WithJsonBody(requestBody)
                        .Build();

// Выполнение запроса
using (var response = await request.GetWebResponseAsync() as HttpWebResponse)
{
    // Обработка ответа от сервера
    var result = await new HttpWebResponseHandler<ResponseModelType>()
                           .HandleSuccess(handler: async (response) =>
                           {
                               return await response.TryDeserializeAsync<ResponseModelType>();
                           })
                           .HandleClientError(handler: (response) =>
                           {
                               throw new Exception("Возникла ошибка 4XX");
                           })
                           .HandleInternalServerError(handler: (response) =>
                           {
                               throw new Exception("Возникла ошибка 5XX");
                           })
                           .Handle(HttpStatusCode.401, handler: (response) =>
                           {
                               throw new Exception("Не авторизован!");
                           }
                           .HandleOthers(handler: (response) =>
                           {
                               throw new Exception();
                           })
                           .HandleResponse(response);
}
```
