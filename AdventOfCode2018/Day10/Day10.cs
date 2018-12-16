using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day10 : Challenge
    {
        public override string Part1()
        {
            var stars = GetStarsFromInput();

            var iterations = 0;
            var prevSize = int.MaxValue;
            while (true)
            {
                var minX = int.MaxValue;
                var minY = int.MaxValue;
                var maxX = int.MinValue;
                var maxY = int.MinValue;
                foreach (var star in stars)
                {
                    var np = star.NextPosition;
                    minX = Math.Min(minX, np.X);
                    minY = Math.Min(minY, np.Y);
                    maxX = Math.Max(maxX, np.X);
                    maxY = Math.Max(maxY, np.Y);
                }
                var width = maxX - minX;
                var height = maxY - minY;
                var size = width + height;
                
                if (size > prevSize)
                {
                    var buffer = new StringBuilder($"After {iterations} seconds:");

                    for (int y = minY; y <= maxY; y++)
                    {
                        buffer.Append('\n');

                        for (int x = minX; x <= maxX; x++)
                        {
                            var hasStar = false;
                            foreach (var star in stars)
                            {
                                if (star.Position.X == x && star.Position.Y == y)
                                {
                                    hasStar = true;
                                    break;
                                }
                            }

                            buffer.Append(hasStar ? '#' : '.');
                        }
                    }

                    return buffer.ToString();
                }

                iterations++;
                prevSize = size;

                foreach (var star in stars) star.Position = star.NextPosition;
            }
        }

        public override string Part2()
        {
            return Part1();
        }



        private List<Star> GetStarsFromInput()
        {
            var stars = new List<Star>();

            using (var stream = GetResource("Day10/input.txt"))
            using (var reader = new System.IO.StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    stars.Add(new Star(reader.ReadLine()));
                }
            }

            return stars;
        }



        private class Star
        {
            private static readonly Regex rx = new Regex(@"^position=< *(-?\d+), *(-?\d+)> velocity=< *(-?\d+), *(-?\d+)>$", RegexOptions.Compiled);

            public Vec2 Position;
            public readonly Vec2 Velocity;

            public Vec2 NextPosition => Position + Velocity;

            public Star(string raw)
            {
                var match = rx.Match(raw);
                Position = new Vec2(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                Velocity = new Vec2(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            }
        }

        private struct Vec2
        {
            public int X;
            public int Y;

            public Vec2(int x, int y)
            {
                X = x;
                Y = y;
            }



            public static Vec2 operator +(Vec2 a, Vec2 b)
            {
                return new Vec2(a.X + b.X, a.Y + b.Y);
            }
        }
    }
}
