using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day3 : Challenge
    {
        private const int FABRIC_SIZE = 1000;



        public override string Part1()
        {
            var fabric = new int[FABRIC_SIZE * FABRIC_SIZE];
            var overlaps = 0;

            using (var stream = GetResource("Day3/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var claim = new Claim(reader.ReadLine());

                    for (int x = claim.X + claim.W - 1; x >= claim.X; x--)
                    {
                        for (int y = claim.Y + claim.H - 1; y >= claim.Y; y--)
                        {
                            var index = y * FABRIC_SIZE + x;
                            fabric[index]++;
                            if (fabric[index] == 2) overlaps++;
                        }
                    }
                }
            }

            return overlaps.ToString();
        }

        public override string Part2()
        {
            var claims = new List<Claim>();

            using (var stream = GetResource("Day3/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    claims.Add(new Claim(reader.ReadLine()));
                }
            }

            foreach (var claim in claims)
            {
                var noOverlaps = true;
                foreach (var other in claims)
                {
                    if (claim.ID == other.ID) continue;

                    if (claim.Overlaps(other))
                    {
                        noOverlaps = false;
                        break;
                    }
                }
                if (noOverlaps) return claim.ID.ToString();
            }

            return "ERROR";
        }



        private struct Claim
        {
            private static readonly Regex rx = new Regex(@"^#(\d+) @ (\d+),(\d+): (\d+)x(\d+)$", RegexOptions.Compiled);

            public readonly int ID;
            public readonly int X;
            public readonly int Y;
            public readonly int W;
            public readonly int H;

            public Claim(string input)
            {
                var match = rx.Match(input);

                ID = int.Parse(match.Groups[1].Value);
                X = int.Parse(match.Groups[2].Value);
                Y = int.Parse(match.Groups[3].Value);
                W = int.Parse(match.Groups[4].Value);
                H = int.Parse(match.Groups[5].Value);
            }



            public bool Overlaps(Claim other)
            {
                return !(X >= other.X + other.W || other.X >= X + W || Y >= other.Y + other.H || other.Y >= Y + H);
            }
        }
    }
}
