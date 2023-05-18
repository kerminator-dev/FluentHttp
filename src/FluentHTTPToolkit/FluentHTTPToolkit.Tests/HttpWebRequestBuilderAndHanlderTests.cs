using FluentHTTPToolkit.Core.Builders.Implementation;
using FluentHTTPToolkit.Core.Handlers.Implementation;
using FluentHTTPToolkit.Core.Handlers.Interfaces;
using FluentHTTPToolkit.Extensions.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FluentHTTPToolkit.Tests
{
    public class HttpWebRequestBuilderAndHanlderTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task HttpGetRequest()
        {
            int year = 2020;
            string country = "RU";
            string apiRoute = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{country}";

            // Построение запроса
            var request = await new HttpWebRequestBuilder(apiRoute)
                                    .WithHttpMethod(HttpMethod.Get)
                                    .Build();

            // Выполнение запроса
            using (var response = await request.GetWebResponseAsync() as HttpWebResponse)
            {
                if (response == null)
                    Assert.Fail();


                List<Holiday> result = await new HttpWebResponseHandler<List<Holiday>>()
                                        .HandleSuccess(handler: async (response) =>
                                        {
                                            return await response.TryDeserializeAsync<List<Holiday>>();
                                        }).HandleResponse(response);

                if (result == null)
                {
                    Assert.Fail();
                }
                else
                {
                    Console.WriteLine($"Всего праздников на {year} год: {result.Count}");
                    Assert.Pass();
                }
            }
        }
    }

    internal class Holiday
    {
        [JsonProperty("date")]
        public DateOnly Date { get; set; }

        [JsonProperty("localName")]
        public string LocalName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("fixed")]
        public bool Fixed { get; set; }

        [JsonProperty("global")]
        public bool Global { get; set; }

        [JsonProperty("counties")]
        public IEnumerable<string> Countries { get; set; }

        [JsonProperty("launchYear")]
        public int? LaunchYear { get; set; }

        [JsonProperty("types")]
        public IEnumerable<string> Types { get; set; }
    }
}