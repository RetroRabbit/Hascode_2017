using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HackingHares
{
    /// <summary>
    /// Program that counts the number of lines in which each word occurs.
    /// </summary>
    public static class Program
    {
        static void Main(string[] args)
        {
            var directory = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Input");
            var files = Directory.GetFiles($"{directory}", "*.in");

            long total = 0;

            foreach (var infile in files)
            {
                var filename = Path.GetFileNameWithoutExtension(infile);
                var outfile = $"{Path.Combine(directory, filename)}.out";

                var input = ReadInput(infile);
                var output = Process(input);

                int score = Scorer.Score(input, output);
                Console.WriteLine($"File: {filename}");
                Console.WriteLine($"Input summary: {input}");
                Console.WriteLine($"Output summary: {output}");
                Console.WriteLine($"Score: {score}");
                Console.WriteLine();
                Console.WriteLine();
                total += score;

                WriteOutput(output, outfile);
            }


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"TOTAL SCORE: {total}");

            Console.ReadLine();
        }

        /// <summary>
        /// Reads the input
        /// </summary>
        /// <param name="filename">The file to read from</param>
        private static InputStructure ReadInput(string filename)
        {
            InputStructure input = new InputStructure();

            using (var reader = File.OpenText(filename))
            {
                List<string> lines = new List<string>();
                var descriptor = reader.ReadIntegers();

                input.NumVideos = descriptor[0];
                input.NumEndpoints = descriptor[1];
                input.NumRequestDescriptions = descriptor[2];
                input.NumCacheServers = descriptor[3];
                input.Capacity = descriptor[4];

                input.VideoSizes = reader.ReadIntegers();

                input.Endpoints = Enumerable.Range(0, input.NumEndpoints)
                    .Select(_ =>
                    {
                        var epDescriptor = reader.ReadIntegers();
                        int numConnections = epDescriptor[1];

                        return new Endpoint
                        {
                            Number = _,
                            Latency = epDescriptor[0],
                            Connections = Enumerable.Range(0, numConnections)
                                .Select(c =>
                                {
                                    var connLine = reader.ReadIntegers();
                                    return new Connection
                                    {
                                        Id = connLine[0],
                                        Latency = connLine[1]
                                    };
                                }).ToArray()
                        };
                    })
                    .ToArray();

                input.Descriptions = Enumerable.Range(0, input.NumRequestDescriptions)
                    .Select(_ =>
                    {
                        var dDescriptor = reader.ReadIntegers();

                        return new Description
                        {
                            VideoId = dDescriptor[0],
                            EndpointId = dDescriptor[1],
                            NumRequests = dDescriptor[2],
                        };
                    }).ToArray();
            }

            return input;
        }

        public static int[] ReadIntegers(this StreamReader reader) => reader.ReadLine().Split(' ').Select(x => int.Parse(x)).ToArray();

        /// <summary>
        /// Process input into output
        /// </summary>
        /// <param name="input">The input to process</param>
        private static OutputStructure Process(InputStructure input)
        {
            int currentCache= 0;
            CacheServerDescription[] caches = new CacheServerDescription[input.NumCacheServers];
            for (var c = 0; c < caches.Length; c++) caches[c] = new CacheServerDescription { Id = c };

            for (int videoId=0;videoId< input.VideoSizes.Length;videoId++)
            {
                var videoSize = input.VideoSizes[videoId];

                //look for a cache that has space
                while (currentCache < caches.Length &&
                    caches[currentCache].Usage + videoSize > input.Capacity)
                {
                    currentCache++;
                }

                if (currentCache >= caches.Length) break;

                //put v in cache server
                caches[currentCache].VideoIds.Add(videoId);
                caches[currentCache].Usage += videoSize;
            }

            return new OutputStructure
            {
                CacheServerDescriptions = caches
            };
        }

        /// <summary>
        /// Writes an output structure to a file
        /// </summary>
        /// <param name="output">The structure to write</param>
        /// <param name="outputFileName">The file to write to</param>
        private static void WriteOutput(OutputStructure output, string outputFileName)
        {
            //for example
            using (var outfile = new StreamWriter(File.OpenWrite(outputFileName)))
            {
                if (output == null) return;

                outfile.WriteLine(output.CacheServerDescriptions.Length);

                foreach (var o in output.CacheServerDescriptions)
                {
                    outfile.Write($"{o.Id} ");
                    foreach (var v in o.VideoIds) outfile.Write($"{v} ");
                    outfile.WriteLine();
                }
            }
        }
    }
}
