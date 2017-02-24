using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HackingHares
{
    public static class Processor
    {
        private class Video
        {
            int Id { get; set; }
            int Size { get; set; }
        }

        public static OutputStructure Process(InputStructure input)
        {
            var capacity = input.Capacity;

            var cacheServers = Enumerable.Range(0, input.NumCacheServers)
                .Select(i => new CacheServerDescription
                {
                    Id = i,
                    Usage = 0,
                    VideoIds = new List<int>()
                }).ToDictionary(x => x.Id);


            var descs = input.Descriptions
                .AsParallel()
                .Select(req =>
                {
                    var Rv = req.VideoId;
                    var Re = req.EndpointId;
                    var L = input.Endpoints[Re];
                    var Ld = input.Endpoints[Re].Latency;
                    var videoSize = input.VideoSizes[Rv];

                    var fastestCacheServerLatency = L.Connections
                        .Select(x => new { Latency = x.Latency, Cache = cacheServers[x.Id] })
                        .OrderBy(x => x.Latency)
                        .Select(x => x.Latency)
                        .FirstOrDefault();

                    if (fastestCacheServerLatency == 0) fastestCacheServerLatency = Ld;

                    var gain = Ld - fastestCacheServerLatency;
                    var score = gain * req.NumRequests;

                    return new { x = req, Metric = score };
                })
                .OrderByDescending(x => x.Metric)
                .Select(x => x.x);

            foreach (var req in descs)
            {
                var Rv = req.VideoId;
                var Re = req.EndpointId;
                var L = input.Endpoints[Re];
                var Ld = input.Endpoints[Re].Latency;
                var videoSize = input.VideoSizes[Rv];

                var cacheServer = L.Connections
                    .Select(x => new { Latency = x.Latency, Cache = cacheServers[x.Id], Left = capacity - cacheServers[x.Id].Usage })
                    .OrderBy(x => x.Latency)
                    .Where(x => !x.Cache.VideoIds.Contains(Rv) && x.Left >= videoSize)
                    .FirstOrDefault();

                if (cacheServer == null) continue;

                cacheServer.Cache.VideoIds.Add(Rv);
                cacheServer.Cache.Usage += videoSize;
            };

            return new OutputStructure
            {
                CacheServerDescriptions = cacheServers.Values.ToArray()
            };
        }
    }
}
