namespace Funk
{
    public static partial class Prelude
    {
        /// <summary>
        /// Represents empty value.
        /// Can be used on OneOf and its corresponding implementations (including Exceptional monad) and Maybe monad.
        /// </summary>
        public static readonly Unit Empty = Unit.Value;
    }
}
