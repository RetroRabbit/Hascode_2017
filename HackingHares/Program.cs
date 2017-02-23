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
    class Program
    {
        static void Main(string[] args)
        {
            var input = ReadInput(args[0]);
            var output = Process(input);
            WriteOutput(output, args[1]);
        }

        /// <summary>
        /// Reads the input
        /// </summary>
        /// <param name="filename">The file to read from</param>
        private static InputStructure ReadInput(string filename)
        {
            using (var reader = File.OpenText(filename))
            {
                //for example
                List<string> lines = new List<string>();

                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                return new InputStructure
                {
                    Lines = lines.ToArray()
                };
            }
        }

        /// <summary>
        /// Process input into output
        /// </summary>
        /// <param name="input">The input to process</param>
        private static OutputStructure Process(InputStructure input)
        {
            //remove duplicate items within lines
            var narrowed = input.Lines.Select(x => Regex.Split(x, @"\s+").Distinct(StringComparer.OrdinalIgnoreCase));

            //flatten
            var flat = narrowed.SelectMany(x => x);

            //group
            var grouped = flat.GroupBy(x => x, StringComparer.OrdinalIgnoreCase);

            //count
            var dictionary = grouped.ToDictionary(x => x.Key, x => x.Count());

            //structure
            var result = new OutputStructure
            {
                WordOccurrences = dictionary
            };

            return result;
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
                foreach (var word in output.WordOccurrences)
                {
                    outfile.WriteLine($"{word.Key}\t{word.Value}");
                }
            }
        }

    }
}
