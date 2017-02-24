using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hashcode
{
    class Program
    {
        static void Main()
        {
            new Program().init();
        }

        public void init()
        {
            string filePath = "trending_today";
            string[] lines = File.ReadAllLines(filePath + ".in", Encoding.UTF8);

            int listLength = lines.Length;
            string state = "start";

            Input input = new Input();
            int cacheDescriptionIndex = 0;
            int requestDescriptionIndex = 0;
            int endpointIndex = 0;

            for (int i = 0; i < listLength; i++)
            {
                int[] vercc = Array.ConvertAll(lines[i].Split(' '), s => int.Parse(s));

                switch (state)
                {
                    case "start":
                        input.Endpoints = new Endpoint[vercc[1]];
                        input.RequestDescriptions = vercc[2];
                        input.CachingServers = new CachingServer[vercc[3]];
                        input.CacheSize = vercc[4];
                        state = "videos";
                        break;
                    case "videos":
                        input.Videos = vercc.Select((value, index) => new Video
                        {
                            Id = index,
                            Size = value
                        }).ToArray();

                        state = "endpoint";
                        break;
                    case "endpoint":
                        input.Endpoints[endpointIndex] = new Endpoint
                        {
                            Id = endpointIndex,
                            CachesLinked = vercc[1],
                            DataCenterLatency = vercc[0]
                        };

                        if (vercc[1] > 0)
                            state = "caching_servers";
                        else
                            state = "video_descriptions";

                        break;
                    case "caching_servers":
                        if (input.CachingServers[vercc[0]] == null)
                            input.CachingServers[vercc[0]] = new CachingServer
                            {
                                Id = vercc[0],
                                RemainingSize = input.CacheSize,
                                EndpointsLinked = new List<EndpointsLinked>(),
                                VideosCached = new List<int>()
                            };

                        input.CachingServers[vercc[0]].EndpointsLinked.Add(new EndpointsLinked
                        {
                            Endpoint = input.Endpoints[endpointIndex],
                            Latency = vercc[1]
                        });

                        cacheDescriptionIndex++;

                        if (cacheDescriptionIndex == input.Endpoints[endpointIndex].CachesLinked)
                        {
                            endpointIndex++;
                            cacheDescriptionIndex = 0;

                            if (endpointIndex < input.Endpoints.Length)
                            {
                                state = "endpoint";
                            }
                            else
                            {
                                state = "video_description";
                            }
                        }

                        break;

                    case "video_description":
                        input.Videos[vercc[0]].Views += vercc[2];

                        break;
                }
            }

            List<Video> cached = new List<Video>();

            input.CachingServers = input.CachingServers.Where(x => x != null).ToArray();


            input.Videos = input.Videos.Where(x => !cached.Contains(x)).OrderByDescending(x => x.Views).ToArray();

            input.CachingServers = FiilCachingServers(input.CachingServers, input.Videos, cached);

            input.CachingServers = input.CachingServers.OrderByDescending(x => x.RemainingSize).ToArray();

            input.CachingServers = FiilCachingServers(input.CachingServers, input.Videos, cached);

            input.Videos = input.Videos.Where(x => !cached.Contains(x)).OrderBy(x => x.Size).ToArray();

            input.CachingServers = input.CachingServers.OrderByDescending(x => x.RemainingSize).ToArray();

            input.CachingServers = FiilCachingServers(input.CachingServers, input.Videos, cached);

            input.CachingServers = input.CachingServers.OrderByDescending(x => x.RemainingSize).ToArray();

            input.CachingServers = FiilCachingServers(input.CachingServers, input.Videos, cached);

            int cachesUsed = input.CachingServers.Count(x => x.VideosCached.Count > 0);

            string[] outlines = new string[1];
            string[] cacheServers = input.CachingServers
                .Where(x => x.VideosCached.Count > 0)
                .Select(x => x.Id + " " + string.Join(" ", x.VideosCached.ToArray())).ToArray();
            outlines[0] = cachesUsed.ToString();
            outlines = outlines.Concat(cacheServers).ToArray();

            File.WriteAllLines(filePath + ".out", outlines);
        }

        public CachingServer[] FiilCachingServers(CachingServer[] servers, Video[] videos, List<Video> cached)
        {
            int videoIndex = 0;
            int videoCount = videos.Count();

            for (int i = 0; i < servers.Count(); i++)
            {
                int remaining = servers[i].RemainingSize - videos.ElementAt(videoIndex).Size;

                while (remaining >= 0 && videoIndex < videoCount)
                {
                    int videoId = videos.ElementAt(videoIndex).Id;

                    if (!servers[i].VideosCached.Contains(videoId))
                    {
                        servers[i].RemainingSize = remaining;
                        servers[i].VideosCached.Add(videos.ElementAt(videoIndex).Id);
                    }

                    videoIndex++;
                    remaining = servers[i].RemainingSize - videos.ElementAt(videoIndex).Size;
                }
            }

            return servers;
        }

        public class Input
        {
            public Endpoint[] Endpoints { get; set; }
            public Video[] Videos { get; set; }
            public CachingServer[] CachingServers { get; set; }
            public int CacheSize { get; set; }
            public int RequestDescriptions { get; set; }
        }

        public class Video
        {
            public int Id { get; set; }
            public int Views { get; set; }
            public int Size { get; set; }
        }

        public class Endpoint
        {
            public int Id { get; set; }
            public int DataCenterLatency { get; set; }
            public int CachesLinked { get; set; }
            public IDictionary<int, int> VideoRequests { get; set; }
        }

        public class CachingServer
        {
            public int Id { get; set; }
            public int RemainingSize { get; set; }
            public ICollection<int> VideosCached { get; set; }
            public ICollection<EndpointsLinked> EndpointsLinked { get; set; }
        }

        public class EndpointsLinked
        {
            public Endpoint Endpoint { get; set; }
            public int Latency { get; set; }
        }
    }
}
