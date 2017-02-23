using System.Collections.Generic;

namespace HackingHares
{
    /// <summary>
    /// The output structure. It contains a collection of words with
    /// the number of lines in which each ocurred.
    /// </summary>
    public class OutputStructure
    {
        //for example
        public IDictionary<string, int> WordOccurrences { get; set; }
    }
}