using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HackingHares
{
    public class Scorer
    {
        public static int Score(InputStructure input, OutputStructure output)
        {
            var cacheServers = output.CacheServerDescriptions
                .ToDictionary(
                    x => x.Id,
                    x => x.VideoIds.ToArray());

            long sumSaved = 0;
            long sumRequests = 0;

            input.Descriptions
                .AsParallel()
                .ForAll(req =>
                {
                    var Rv = req.VideoId;
                    var Re = req.EndpointId;
                    var L = input.Endpoints[Re];
                    var Ld = input.Endpoints[Re].Latency;

                    var fastestCacheServer = L.Connections
                        .Select(x => new { Latency = x.Latency, Cache = cacheServers[x.Id] })
                        .Where(x => x.Cache.Contains(Rv))
                        .OrderBy(x => x.Latency)
                        .FirstOrDefault();

                    var cacheServerLatency = fastestCacheServer?.Latency ?? int.MaxValue;

                    var Lmin = Math.Min(cacheServerLatency, Ld);

                    var saved = Ld - Lmin;
                    var savedMicro = saved;
                    Interlocked.Add(ref sumSaved, savedMicro * req.NumRequests);
                    Interlocked.Add(ref sumRequests, req.NumRequests);
                });

            return (int)((sumSaved * 1000) / sumRequests);
        }
    }
}
