namespace HackingHares
{
    public class Endpoint
    {
        public int Latency { get; set; }
        public int NumConnections { get; set; }
        public Connection[] Connections { get; set; }
    }
}