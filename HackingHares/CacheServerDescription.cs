using System.Collections.Generic;

namespace HackingHares
{
    public class CacheServerDescription
    {
        public int Id { get; set; }
        public List<int> VideoIds { get; set; } = new List<int>();
        public int Usage { get; set; }
    }
}