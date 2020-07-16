using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    }

    public static class ResourceService
    {
        public static async Task<Exc<Resource, Error>> GetResource(this Identity identity, ResourceType type, string publicationId = null)
        {
            var info = await identity.Get<Info>(new Uri($"{Medium.BaseUrl}{Medium.Info}"));
            return await new AsyncPattern<Exc<Resource, Error>>
            {
                (ResourceType.Info, _ => info.Match(
                    i => success<Resource, Error>(new Resource(i)).ToTask(),
                    e => failure<Resource, Error>(e).ToTask()
                )),
                (ResourceType.Publications, _ => info.Match(
                    async i =>
                    {
                        var result = await identity.Get<Publications>(new Uri($"{Medium.BaseUrl}{Medium.Users}/{i.Data.Id}/{Medium.Publications}"));
                        return result.Match(
                            p => success<Resource, Error>(new Resource(new Publication(p))),
                            failure<Resource, Error>
                        );
                    },
                    e => result(failure<Resource, Error>(e))
                )),
                (ResourceType.Contributors, _ => info.Match(
                    i =>
                    {
                        return publicationId.AsNotEmptyString().Match(
                            __ => failure<Resource, Error>(new InvalidRequestError("Publication id cannot be empty.")).ToTask(),
                            async id =>
                            {
                                var result = await identity.Get<Contributors>(new Uri($"{Medium.BaseUrl}{Medium.Publications}/{id}/{Medium.Contributors}"));
                                return result.Match(
                                    c => success<Resource, Error>(new Resource(new Publication(c))),
                                    failure<Resource, Error>
                                );
                            }
                        );
                    },
                    e => failure<Resource, Error>(e).ToTask()
                ))
            }.Match(type).GetOrAsync(_ => Exc.Empty<Resource, Error>().ToTask());
        }

        private static Task<Exc<T, Error>> Get<T>(this Identity identity, Uri uri)
        {
            return identity.Token.ToExc<string, Error>(_ => new InvalidRequestError("Token cannot be empty.")).FlatMapAsync(token =>
                uri.CreateGetRequest(token).SendAsync().GetContent().FlatMapAsync(r => result(r.SafeDeserialize<T>().MapFailure(e =>
                    new Error(e.Root.Map(ex => ex.Message).GetOr(_ => "Unable to properly deserialize response returned by the server.")))
                ))
            );
        }

        private static HttpRequestMessage CreateGetRequest(this Uri uri, string token)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Method = HttpMethod.Get;
            request.RequestUri = uri;
            return request;
        }
    }

    public enum ResourceType
    {
        Undefined,
        Info,
        Publications,
        Contributors
    }

    public static class Http
    {
        public static Task<HttpResponseMessage> SendAsync(this HttpRequestMessage request) => new HttpClient { Timeout = new TimeSpan(0, 0, minutes: 1, 0) }.DisposeAfterAsync(c => c.SendAsync(request));

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
