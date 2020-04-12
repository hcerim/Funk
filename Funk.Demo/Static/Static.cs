using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Funk.Prelude;

namespace Funk.Demo
{
    public static class Medium
    {
        public const string BaseUrl = "https://api.medium.com/v1/";
        public const string Info = "me";
        public const string Users = "users";
        public const string Publications = "publications";
        public const string Contributors = "contributors";
        public const string Posts = "posts";
        public const string Images = "images";
    }

    public static class Http
    {
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => await new HttpClient { Timeout = new TimeSpan(0, 0, minutes: 1, 0) }.DisposeAfterAsync(c => c.SendAsync(request));

        public static async Task<Exc<string, Error>> GetContent(this Task<HttpResponseMessage> message) => await GetContent(await message);

        public static async Task<Exc<string, Error>> GetContent(this HttpResponseMessage message)
        {
            var response = await message.Content.ReadAsStringAsync();
            return message.StatusCode.Match(
                HttpStatusCode.Unauthorized, _ => failure<string, Error>(new UnauthorizedError(response)),
                HttpStatusCode.Forbidden, _ => failure<string, Error>(new ForbiddenError(response)),
                HttpStatusCode.BadRequest, _ => failure<string, Error>(new InvalidRequestError(response)),
                HttpStatusCode.OK, _ => success<string, Error>(response),
                _ => empty
            );
        }
    }

    public static class Serializer
    {
        public static Exc<T, JsonException> SafeDeserialize<T>(this string content) => Exc.Create<T, JsonException>(_ => JsonConvert.DeserializeObject<T>(content));
    }
}
