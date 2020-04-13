using System;
using System.Threading.Tasks;

namespace Funk.Demo
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Enter your token:");
            var service = new ResourceService(Console.ReadLine());
            Console.WriteLine("See: Info (i), Publications (p), Contributors (c)");
            var input = Console.ReadLine();
            var resource = await service.GetResource(
                input.Match(
                    "i", _ => ResourceType.Info,
                    "p", _ => ResourceType.Publications,
                    "c", _ => ResourceType.Contributors,
                    _ => ResourceType.Undefined
                ),
                input.SafeEquals("c").AsTrue().Match(
                    f => default,
                    t =>
                    {
                        Console.WriteLine("Enter publication id:");
                        return Console.ReadLine();
                    }
                )
            );
            resource.Match(
                _ => Console.WriteLine("No information found."),
                Console.WriteLine,
                e => Console.WriteLine(e.Root)
            );
        }
    }
}
