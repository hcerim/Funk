using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk.Demo
{
    public class ResourceService
    {
        private readonly Auth _auth;

        public ResourceService(string token)
        {
            _auth = new Auth(token);
        }

        public async Task<Exc<Resource, Error>> GetResource(ResourceType type, string publicationId = null)
        {
            var info = await Get<Info>(new Uri($"{Medium.BaseUrl}{Medium.Info}"));
            return await type.Match(
                ResourceType.Info, _ => info.Match(
                    __ => Task.FromResult(Exc.Empty<Resource, Error>()),
                    i => Task.FromResult(success<Resource, Error>(new Resource(i))),
                    e => Task.FromResult(failure<Resource, Error>(e))
                ),
                ResourceType.Publications, async _ => await info.Match(
                    __ => Task.FromResult(Exc.Empty<Resource, Error>()),
                    async i =>
                    {
                        var result = await Get<Publications>(new Uri($"{Medium.BaseUrl}{Medium.Users}/{i.Data.Id}/{Medium.Publications}"));
                        return result.Match(
                            __ => empty,
                            p => success<Resource, Error>(new Resource(new Publication(p))),
                            failure<Resource, Error>
                        );
                    },
                    e => Task.FromResult(failure<Resource, Error>(e))
                ),
                ResourceType.Contributors, async _ => await info.Match(
                    __ => Task.FromResult(Exc.Empty<Resource, Error>()),
                    async i =>
                    {
                        return await publicationId.AsNotEmptyString().Match(
                            __ => Task.FromResult(failure<Resource, Error>(new InvalidRequestError("Publication id cannot be empty."))),
                            async id =>
                            {
                                var result = await Get<Contributors>(new Uri($"{Medium.BaseUrl}{Medium.Publications}/{id}/{Medium.Contributors}"));
                                return result.Match(
                                    __ => empty,
                                    c => success<Resource, Error>(new Resource(new Publication(c))),
                                    failure<Resource, Error>
                                );
                            }
                        );
                    },
                    e => Task.FromResult(failure<Resource, Error>(e))
                ),
                _ => Task.FromResult(Exc.Empty<Resource, Error>())
            );
        }

        private async Task<Exc<T, Error>> Get<T>(Uri uri)
        {
            return await _auth.Token.ToExc<string, Error>(_ => new InvalidRequestError("Token cannot be empty.")).Match(
                token => Http.SendAsync(CreateGetRequest(uri, token)).GetContent().FlatMapAsync(r => 
                    Task.FromResult(r.SafeDeserialize<T>().AsSuccess().Match(
                        _ => failure<T, Error>(new JsonError("Response could not be deserialized correctly.")),
                        success<T, Error>
                    ))
                ),
                e => Task.FromResult(failure<T, Error>(e))
            );
        }

        private static HttpRequestMessage CreateGetRequest(Uri uri, string token)
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
        Contributors,
        Posts,
        Images
    }
}
