using System.Diagnostics.Contracts;

namespace Funk
{
    /// <summary>
    /// Module that simplifies type definitions.
    /// Import it as a static reference to access functions directly. 
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// Returns newly created Unit.
        /// </summary>
        [Pure]
        public static Unit empty => Unit.Value;
    }
}
