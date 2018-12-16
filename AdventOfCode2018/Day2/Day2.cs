using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2018
{
    public class Day2 : Challenge
    {
        public override string Part1()
        {
            var charCounts = new Dictionary<char, int>();

            int doubleCount = 0;
            int tripleCount = 0;

            using (var stream = GetResource("Day2/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    charCounts.Clear();
                    foreach (var c in reader.ReadLine())
                    {
                        charCounts[c] = 1 + (charCounts.ContainsKey(c) ? charCounts[c] : 0);
                    }
                    if (charCounts.ContainsValue(2)) doubleCount++;
                    if (charCounts.ContainsValue(3)) tripleCount++;
                }
            }

            return (doubleCount * tripleCount).ToString();
        }

        public override string Part2()
        {
            var seenIDs = new HashSet<string>();

            using (var stream = GetResource("Day2/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var id = reader.ReadLine();

                    foreach (var other in seenIDs)
                    {
                        var diffIndex = FindSingleDifference(id, other);
                        if (diffIndex == -1) continue;

                        return id.Substring(0, diffIndex) + id.Substring(diffIndex + 1);
                    }

                    seenIDs.Add(id);
                }
            }

            return $"ERROR found no common IDs out of {seenIDs.Count} total";
        }


        
        private int FindSingleDifference(string a, string b)
        {
            if (a.Length != b.Length) throw new System.Exception("Unable to compare strings of different length");

            int foundIndex = -1;

            for (int i = a.Length - 1; i >= 0; i--)
            {
                if (a[i] != b[i])
                {
                    if (foundIndex != -1) return -1;
                    foundIndex = i;
                }
            }

            return foundIndex;
        }
    }
}
