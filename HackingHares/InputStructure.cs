namespace HackingHares
{
    /// <summary>
    /// The input structure. It contains all the input lines
    /// </summary>
    public class InputStructure
    {
        public int NumVideos { get; set; }
        public int NumEndpoints { get; set; }
        public int NumRequestDescriptions { get; set; }
        public int NumCacheServers { get; set; }
        public int Capacity { get; set; }

        public int[] VideoSizes { get; set; }

        public Endpoint[] Endpoints { get; set; }
        
        public Description[] Descriptions { get; set; }
    }
}