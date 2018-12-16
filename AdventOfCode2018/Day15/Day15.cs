using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2018
{
    public class Day15 : Challenge
    {
        public override string Part1()
        {
            var game = GetGameStateFromInput();

            game.Simulate();

            var totalHealth = 0;
            foreach (var unit in game.Elves) totalHealth += unit.Health;
            foreach (var unit in game.Goblins) totalHealth += unit.Health;
            return (game.Round * totalHealth).ToString();
        }

        public override string Part2()
        {
            // these are the values that change how many hits it takes an elf to kill a goblin; other numbers don't matter
            var ATTACK_POWERS = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 19, 20, 23, 25, 29, 34, 40, 50, 67, 100, 200 };

            var game = GetGameStateFromInput();

            var attackPowerIndex = ATTACK_POWERS.Length - 1;
            var previousScore = -1;
            while (attackPowerIndex >= 0)
            {
                var copy = new GameState(game);

                foreach (var unit in copy.Elves) unit.AttackPower = ATTACK_POWERS[attackPowerIndex]; ;
                copy.Simulate();

                if (copy.Elves.Count != game.Elves.Count) break;

                var totalHealth = 0;
                foreach (var unit in copy.Elves) totalHealth += unit.Health;
                foreach (var unit in copy.Goblins) totalHealth += unit.Health;
                previousScore = copy.Round * totalHealth;

                attackPowerIndex--;
            }

            return previousScore.ToString();
        }



        private GameState GetGameStateFromInput()
        {
            using (var stream = GetResource("Day15/input.txt"))
            using (var reader = new StreamReader(stream))
            {
                return new GameState(reader.ReadToEnd());
            }
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

            public IEnumerable<Coordinate> GetAdjacentCoordinates()
            {
                yield return new Coordinate(X, Y - 1);
                yield return new Coordinate(X - 1, Y);
                yield return new Coordinate(X + 1, Y);
                yield return new Coordinate(X, Y + 1);
            }

            public bool IsBefore(Coordinate other)
            {
                return (Y == other.Y) ? X < other.X : Y < other.Y;
            }
        }

        private class GameState
        {
            public readonly HashSet<Unit> Elves = new HashSet<Unit>();
            public readonly HashSet<Unit> Goblins = new HashSet<Unit>();
            public int Round { get; private set; }
            public readonly Terrain Terrain;
            public UnitMap UnitMap;

            public GameState(string raw)
            {
                Terrain = new Terrain(raw);

                UnitMap = new UnitMap(Terrain.Width, Terrain.Height);

                raw = raw.Replace("\r\n", "");
                for (int i = raw.Length - 1; i >= 0; i--)
                {
                    var position = new Coordinate(i % Terrain.Width, i / Terrain.Width);

                    switch (raw[i])
                    {
                        case 'E':
                            Elves.Add(new Unit(this, UnitType.Elf, position));
                            break;
                        case 'G':
                            Goblins.Add(new Unit(this, UnitType.Goblin, position));
                            break;
                    }
                }
            }

            public GameState(GameState copy)
            {
                Terrain = copy.Terrain;
                UnitMap = new UnitMap(Terrain.Width, Terrain.Height);
                foreach (var unit in copy.Elves) Elves.Add(new Unit(this, unit));
                foreach (var unit in copy.Goblins) Goblins.Add(new Unit(this, unit));
            }
            


            public void Simulate()
            {
                Round = 0;

                while (true)
                {
                    ReadyAllUnits();

                    while (true)
                    {
                        var unit = GetNextReadyUnit();
                        if (unit == null) break;

                        var targets = unit.Type == UnitType.Elf ? Goblins : Elves;
                        if (targets.Count == 0) return;

                        unit.Ready = false;

                        var closestDestination = unit.Position;
                        var closestDistance = int.MaxValue;
                        foreach (var target in targets)
                        {
                            foreach (var destination in target.Position.GetAdjacentCoordinates())
                            {
                                if (Terrain.IsSolid(destination)) continue;
                                if (UnitMap[destination] != null)
                                {
                                    if (UnitMap[destination] == unit)
                                    {
                                        closestDestination = unit.Position;
                                        closestDistance = 0;
                                        break;
                                    }
                                    continue;
                                }

                                var distance = GetDistance(unit.Position, destination, closestDistance);
                                if (distance < 0) continue;
                                if (distance < closestDistance || (distance == closestDistance && destination.IsBefore(closestDestination)))
                                {
                                    closestDestination = destination;
                                    closestDistance = distance;
                                }
                            }
                        }

                        if (UnitMap[closestDestination] != unit)
                        {
                            var closestStep = unit.Position;

                            foreach (var adjacentCoordinate in unit.Position.GetAdjacentCoordinates())
                            {
                                if (Terrain.IsSolid(adjacentCoordinate) || UnitMap[adjacentCoordinate] != null) continue;
                                var distance = GetDistance(adjacentCoordinate, closestDestination, closestDistance);
                                if (distance < 0) continue;
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closestStep = adjacentCoordinate;
                                }
                            }

                            unit.Position = closestStep;
                        }

                        Unit attackTarget = null;
                        foreach (var adjacentCoordinate in unit.Position.GetAdjacentCoordinates())
                        {
                            var adjacentUnit = UnitMap[adjacentCoordinate];
                            if (adjacentUnit == null || adjacentUnit.Type == unit.Type) continue;

                            if (attackTarget == null || adjacentUnit.Health < attackTarget.Health) attackTarget = adjacentUnit;
                        }

                        if (attackTarget != null)
                        {
                            attackTarget.Health -= unit.AttackPower;
                            if (attackTarget.Health <= 0)
                            {
                                targets.Remove(attackTarget);
                                UnitMap[attackTarget.Position] = null;
                            }
                        }
                    }

                    Round++;
                }
            }



            private int GetDistance(Coordinate a, Coordinate b, int limit = int.MaxValue)
            {
                var pathFlags = new bool[Terrain.Width * Terrain.Height];
                var distance = 0;
                var oldCoordinates = new List<Coordinate>() { a };
                var newCoordinates = new List<Coordinate>();
                while (oldCoordinates.Count > 0 && distance <= limit)
                {
                    foreach (var coordinate in oldCoordinates)
                    {
                        if (coordinate.X == b.X && coordinate.Y == b.Y) return distance;

                        foreach (var adjacentCoordinate in coordinate.GetAdjacentCoordinates())
                        {
                            var pathFlagIndex = adjacentCoordinate.X + adjacentCoordinate.Y * Terrain.Width;
                            if (pathFlags[pathFlagIndex]) continue;
                            pathFlags[pathFlagIndex] = true;

                            if (Terrain.IsSolid(adjacentCoordinate)) continue;
                            if (UnitMap[adjacentCoordinate] != null) continue;
                            newCoordinates.Add(adjacentCoordinate);
                        }
                    }

                    var tmp = oldCoordinates;
                    oldCoordinates = newCoordinates;
                    newCoordinates = tmp;
                    newCoordinates.Clear();

                    distance++;
                }

                return -1;
            }

            private Unit GetNextReadyUnit()
            {
                for (int y = 0; y < Terrain.Height; y++)
                {
                    for (int x = 0; x < Terrain.Width; x++)
                    {
                        var unit = UnitMap[new Coordinate(x, y)];
                        if (unit != null && unit.Ready) return unit;
                    }
                }

                return null;
            }

            private void ReadyAllUnits()
            {
                foreach (var unit in Elves) unit.Ready = true;
                foreach (var unit in Goblins) unit.Ready = true;
            }
        }

        private class Terrain
        {
            private readonly bool[] data;
            
            public readonly int Width;
            public readonly int Height;
            
            public Terrain(string raw)
            {
                var lines = raw.Split('\n');
                Width = lines[0].Length - 1;
                Height = lines.Length;
                data = new bool[Width * Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        data[x + y * Width] = lines[y][x] == '#';
                    }
                }
            }



            public bool IsSolid(Coordinate c)
            {
                return data[c.X + c.Y * Width];
            }
        }

        private class Unit
        {
            private readonly GameState game;
            private Coordinate _position;

            public int AttackPower = 3;
            public int Health = 200;
            public bool Ready = true;
            public UnitType Type;
            
            public Coordinate Position
            {
                get { return _position; }
                set
                {
                    game.UnitMap[_position] = null;
                    _position = value;
                    game.UnitMap[value] = this;
                }
            }

            public Unit(GameState game, UnitType type, Coordinate position)
            {
                this.game = game;
                Type = type;
                _position = position;

                game.UnitMap[position] = this;
            }

            public Unit(GameState game, Unit copy)
            {
                this.game = game;
                AttackPower = copy.AttackPower;
                Health = copy.Health;
                Ready = copy.Ready;
                Type = copy.Type;
                _position = copy.Position;

                game.UnitMap[Position] = this;
            }
        }

        private struct UnitMap
        {
            private readonly Unit[] data;
            private readonly int width;

            public Unit this[Coordinate c]
            {
                get { return data[c.X + c.Y * width]; }
                set { data[c.X + c.Y * width] = value; }
            }

            public UnitMap(int width, int height)
            {
                data = new Unit[width * height];
                this.width = width;
            }
        }

        private enum UnitType
        {
            Elf,
            Goblin
        }
    }
}
