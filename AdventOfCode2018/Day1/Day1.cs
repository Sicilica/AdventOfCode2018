using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2018
{
    public class Day1 : Challenge
    {
        public override string Part1()
        {
            int frequency = 0;

            using (var stream = GetResource("Day1/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    frequency += int.Parse(reader.ReadLine());
                }
            }

            return frequency.ToString();
        }

        public override string Part2()
        {
            int frequency = 0;
            var seenFrequencies = new HashSet<int>() { 0 };

            while (true)
            {
                using (var stream = GetResource("Day1/input.txt"))
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        frequency += int.Parse(reader.ReadLine());

                        if (seenFrequencies.Contains(frequency)) return frequency.ToString();
                        seenFrequencies.Add(frequency);
                    }
                }
            }
        }
    }
}
