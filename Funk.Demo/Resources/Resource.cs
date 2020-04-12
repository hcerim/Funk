using System.Collections.Generic;
using Newtonsoft.Json;

namespace Funk.Demo
{
    public sealed class Resource : OneOf<Info, Publication>
    {
        public Resource(Info info)
            : base(info)
        {
        }

        public Resource(Publication publication)
            : base(publication)
        {
        }
    }

    public sealed class Info
    {
        public UserInfo Data { get; set; }

        public class UserInfo
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public string ImageUrl { get; set; }
        }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public sealed class Publication : OneOf<Publications, Contributors>
    {
        public Publication(Publications publications)
            : base(publications)
        {
        }

        public Publication(Contributors contributors)
            : base(contributors)
        {
        }
    }

    public class Publications
    {
        public List<Publication> Data { get; set; }

        public class Publication
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string ImageUrl { get; set; }
        }

        public override string ToString() => JsonConvert.SerializeObject(Data.AsNotEmptyList().ToString(), Formatting.Indented);
    }

    public class Contributors
    {
        public List<Contributor> Data { get; set; }

        public class Contributor
        {
            public string PublicationId { get; set; }
            public string UserId { get; set; }
            public string Role { get; set; }
        }

        public override string ToString() => JsonConvert.SerializeObject(Data.AsNotEmptyList().ToString(), Formatting.Indented);
    }
}
