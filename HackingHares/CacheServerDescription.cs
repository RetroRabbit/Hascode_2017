using System.Collections.Generic;

namespace HackingHares
{
    public class CacheServerDescription
    {
        public int Id { get; set; }
        public List<int> VideoIds { get; set; } = new List<int>();

        /// <summary>
        /// Not calculated, just for convenience
        /// </summary>
        public int Usage { get; set; }

        public override string ToString()
        {
            return $"Cache #{Id} has {VideoIds.Count} videos with total size of {Usage}MB";
        }
    }
}