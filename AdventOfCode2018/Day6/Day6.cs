using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2018
{
    public class Day6 : Challenge
    {
        public override string Part1()
        {
            var coordinates = GetCoordinatesFromInput();

            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            foreach (var coordinate in coordinates)
            {
                minX = Math.Min(minX, coordinate.X);
                minY = Math.Min(minY, coordinate.Y);
                maxX = Math.Max(maxX, coordinate.X);
                maxY = Math.Max(maxY, coordinate.Y);
            }
            var width = maxX - minX + 1;
            var height = maxY - minY + 1;

            var areaSizes = new int[coordinates.Count];
            var map = new MapEntry[width * height];
            for (int i = map.Length - 1; i >= 0; i--)
            {
                map[i].AreaIndex = -1;
                map[i].Distance = int.MaxValue;
            }

            int areaIndex = -1;
            var oldCoordinates = new List<Coordinate>();
            var newCoordinates = new List<Coordinate>();
            foreach (var startCoordinate in coordinates)
            {
                areaIndex++;
                
                oldCoordinates.Add(startCoordinate);

                int distance = 0;
                while (oldCoordinates.Count > 0)
                {
                    foreach (var coordinate in oldCoordinates)
                    {
                        var i = (coordinate.X - minX) + (coordinate.Y - minY) * width;

                        if (map[i].AreaIndex == areaIndex) continue;

                        if (distance <= map[i].Distance)
                        {
                            if (map[i].AreaIndex >= 0)
                            {
                                areaSizes[map[i].AreaIndex]--;
                                map[i].AreaIndex = -1;
                            }

                            if (distance < map[i].Distance)
                            {
                                map[i].Distance = distance;
                                map[i].AreaIndex = areaIndex;
                                areaSizes[areaIndex]++;

                                if (coordinate.X > minX) newCoordinates.Add(new Coordinate(coordinate.X - 1, coordinate.Y));
                                if (coordinate.Y > minY) newCoordinates.Add(new Coordinate(coordinate.X, coordinate.Y - 1));
                                if (coordinate.X < maxX) newCoordinates.Add(new Coordinate(coordinate.X + 1, coordinate.Y));
                                if (coordinate.Y < maxY) newCoordinates.Add(new Coordinate(coordinate.X, coordinate.Y + 1));
                            }
                        }
                    }

                    var tmp = oldCoordinates;
                    oldCoordinates = newCoordinates;
                    newCoordinates = tmp;
                    newCoordinates.Clear();

                    distance++;
                }
            }

            var largestAreaSize = int.MinValue;
            for (areaIndex = areaSizes.Length - 1; areaIndex >= 0; areaIndex--) {
                if (areaSizes[areaIndex] <= largestAreaSize) continue;

                bool areaIsInfinite = false;

                for (int x = width - 1; x >= 0; x--)
                {
                    if (map[x].AreaIndex == areaIndex || map[x + (height - 1) * width].AreaIndex == areaIndex)
                    {
                        areaIsInfinite = true;
                        break;
                    }
                }
                if (areaIsInfinite) continue;

                for (int y = height - 1; y >= 0; y--)
                {
                    if (map[y * width].AreaIndex == areaIndex || map[(width - 1) + y * width].AreaIndex == areaIndex)
                    {
                        areaIsInfinite = true;
                        break;
                    }
                }
                if (areaIsInfinite) continue;

                largestAreaSize = areaSizes[areaIndex];
            }

            return largestAreaSize.ToString();
        }

        public override string Part2()
        {
            const int MAX_TOTAL_DISTANCE = 10000;

            var coordinates = GetCoordinatesFromInput();

            // this is a bit problematic because the world is theoretically infinite (and could extend up to 10000 units outside the bounds of the most distant points)
            // one solution to this is to start with the most central point (which is guaranteed to be in our region, else the region has area=0) and continue from there

            var totalX = 0;
            var totalY = 0;
            foreach (var coordinate in coordinates)
            {
                totalX += coordinate.X;
                totalY += coordinate.Y;
            }

            var region = new HashSet<Coordinate>();
            var oldCoordinates = new List<Coordinate>();
            var newCoordinates = new List<Coordinate>();
            oldCoordinates.Add(new Coordinate(totalX / coordinates.Count, totalY / coordinates.Count));
            while (oldCoordinates.Count > 0)
            {
                foreach (var coordinate in oldCoordinates)
                {
                    if (region.Contains(coordinate)) continue;

                    var totalDistance = 0;
                    foreach (var pointOfInterest in coordinates)
                    {
                        totalDistance += Math.Abs(coordinate.X - pointOfInterest.X) + Math.Abs(coordinate.Y - pointOfInterest.Y);
                    }
                    if (totalDistance >= MAX_TOTAL_DISTANCE) continue;

                    region.Add(coordinate);

                    newCoordinates.Add(new Coordinate(coordinate.X - 1, coordinate.Y));
                    newCoordinates.Add(new Coordinate(coordinate.X, coordinate.Y - 1));
                    newCoordinates.Add(new Coordinate(coordinate.X + 1, coordinate.Y));
                    newCoordinates.Add(new Coordinate(coordinate.X, coordinate.Y + 1));
                }

                var tmp = oldCoordinates;
                oldCoordinates = newCoordinates;
                newCoordinates = tmp;
                newCoordinates.Clear();
            }

            return region.Count.ToString();
        }



        private List<Coordinate> GetCoordinatesFromInput()
        {
            var coordinates = new List<Coordinate>();

            using (var stream = GetResource("Day6/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    coordinates.Add(new Coordinate(reader.ReadLine()));
                }
            }

            return coordinates;
        }


        
        private struct Coordinate
        {
            public readonly int X;
            public readonly int Y;

            public Coordinate(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Coordinate(string raw)
            {
                var parts = raw.Split(',');
                X = int.Parse(parts[0]);
                Y = int.Parse(parts[1].Substring(1));
            }



            public override bool Equals(object obj)
            {
                if (!(obj is Coordinate other)) return false;

                return X == other.X && Y == other.Y;
            }

            public override int GetHashCode()
            {
                return X * 17 + Y * 7;
            }
        }

        private struct MapEntry
        {
            public int AreaIndex;
            public int Distance;
        }
    }
}
