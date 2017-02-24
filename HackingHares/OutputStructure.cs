using System.Collections.Generic;
using System.Linq;

namespace HackingHares
{
    /// <summary>
    /// The output structure. It contains a collection of words with
    /// the number of lines in which each ocurred.
    /// </summary>
    public class OutputStructure
    {
        public CacheServerDescription[] CacheServerDescriptions { get; set; }

        public override string ToString()
        {
            return $"Using {CacheServerDescriptions?.Length ?? 0} Cache servers with a total of {CacheServerDescriptions.Sum(b => b.VideoIds.Count)} Videos";
        }
    }
}