using System.Threading.Tasks;

namespace Funk
{
    public static class TaskExt
    {
        public static Task<Unit> WithResult(this Task task)
        {
            return Task.Run(async () =>
            {
                await task;
                return Unit.Value;
            });
        }
    }
}
